// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.Core;

using System.Threading;
using System.Threading.Tasks;
using global::Azure;
using global::Azure.Core;
using global::Azure.ResourceManager.Resources;
using global::Azure.ResourceManager.Storage.Models;
using global::Azure.ResourceManager.Storage;
using global::Azure.Storage.Files.DataLake;
using global::Azure.Storage.Files.DataLake.Models;
using global::Azure.Storage.Sas;
using Microsoft.Azure.Management.Storage.Models;
using Microsoft.Azure.ProjectBabylon.Metadata.Models;
using Microsoft.Purview.DataGovernance.Provisioning.Common.Utilities;
using Microsoft.Purview.DataGovernance.Provisioning.Configurations;
using Microsoft.Purview.DataGovernance.Provisioning.DataAccess;
using Microsoft.Purview.DataGovernance.Provisioning.Models;
using Microsoft.Extensions.Options;
using ProcessingStorageModel = Models.ProcessingStorageModel;
using StorageAccountKey = global::Azure.ResourceManager.Storage.Models.StorageAccountKey;
using StorageSasRequest = Models.StorageSasRequest;
using Microsoft.Purview.DataGovernance.Common;
using Microsoft.Purview.DataGovernance.Loggers;
using Microsoft.Purview.DataGovernance.Provisioning.Common;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.ComponentModel;

/// <summary>
/// Processing storage manager.
/// </summary>
internal class ProcessingStorageManager : StorageManager<ProcessingStorageConfiguration>, IProcessingStorageManager
{
    private readonly TokenCredential tokenCredential;
    private readonly IStorageAccountRepository<ProcessingStorageModel> storageAccountRepository;
    private readonly IMemoryCache cache;
    private readonly IRequestContextAccessor requestContextAccessor;
    private readonly IAccountExposureControlConfigProvider exposureControl;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessingStorageManager"/> class.
    /// </summary>
    /// <param name="credentialFactory"></param>
    /// <param name="storageAccountRepository"></param>
    /// <param name="azureResourceManagerFactory"></param>
    /// <param name="processingStorageConfiguration"></param>
    /// <param name="logger"></param>
    public ProcessingStorageManager(
        AzureCredentialFactory credentialFactory,
        IStorageAccountRepository<ProcessingStorageModel> storageAccountRepository,
        IAzureResourceManagerFactory azureResourceManagerFactory,
        IOptions<ProcessingStorageConfiguration> processingStorageConfiguration,
        IMemoryCache cache,
        IRequestContextAccessor requestContextAccessor,
        IAccountExposureControlConfigProvider exposureControl,
        IServiceRequestLogger logger) : base(azureResourceManagerFactory.Create<ProcessingStorageAuthConfiguration>(), processingStorageConfiguration, logger)
    {
        this.storageAccountRepository = storageAccountRepository;
        this.cache = cache;
        this.requestContextAccessor = requestContextAccessor;
        this.exposureControl = exposureControl;
        this.tokenCredential = credentialFactory.CreateDefaultAzureCredential();
    }

    /// <inheritdoc/>
    public async Task<ProcessingStorageModel> Get(AccountServiceModel accountServiceModel, CancellationToken cancellationToken)
    {
        return await this.Get(Guid.Parse(accountServiceModel.Id), cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<ProcessingStorageModel> Get(Guid accountId, CancellationToken cancellationToken)
    {
        StorageAccountLocator storageAccountKey = new(accountId.ToString(), this.storageConfiguration.StorageNamePrefix);

        return await this.storageAccountRepository.GetSingle(storageAccountKey, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<DeletionResult> Delete(AccountServiceModel accountServiceModel, CancellationToken cancellationToken)
    {
        StorageAccountLocator storageAccountKey = new(accountServiceModel.Id, this.storageConfiguration.StorageNamePrefix);
        ProcessingStorageModel storageAccountModel = await this.storageAccountRepository.GetSingle(storageAccountKey, cancellationToken);
        if (storageAccountModel == null)
        {
            this.logger.LogInformation($"Storage account not found for {storageAccountKey.Name}");

            return new DeletionResult()
            {
                DeletionStatus = DeletionStatus.ResourceNotFound,
            };
        }

        string resourceId = $"/subscriptions/{this.storageConfiguration.SubscriptionId}";
        SubscriptionResource subscription = this.azureResourceManager.GetSubscription(resourceId);
        ResourceGroupResource resourceGroup = await this.azureResourceManager.GetResourceGroup(subscription, this.storageConfiguration.ResourceGroupName, cancellationToken);
        ResourceIdentifier storageAccountId = new(storageAccountModel.Properties.ResourceId);
        await this.DeleteStorageAccount(resourceGroup, storageAccountId.Name, cancellationToken);
        await this.storageAccountRepository.Delete(storageAccountKey, cancellationToken);
        this.logger.LogInformation($"Successfully deleted storage account {storageAccountModel.Properties.ResourceId}");

        return new DeletionResult()
        {
            DeletionStatus = DeletionStatus.Deleted,
        };
    }

    /// <inheritdoc/>
    public async Task<string> Provision(AccountServiceModel accountServiceModel, CancellationToken cancellationToken)
    {
        string resourceId = $"/subscriptions/{this.storageConfiguration.SubscriptionId}";
        SubscriptionResource subscription = this.azureResourceManager.GetSubscription(resourceId);
        ResourceGroupResource resourceGroup = await this.azureResourceManager.GetResourceGroup(subscription, this.storageConfiguration.ResourceGroupName, cancellationToken);

        StorageAccountRequestModel processingStorageModelRequest = new()
        {
            Location = accountServiceModel.Location,
            ResourceGroup = this.storageConfiguration.ResourceGroupName,
            SubscriptionId = this.storageConfiguration.SubscriptionId,
            TenantId = this.storageConfiguration.TenantId,
        };
        ProcessingStorageModel existingStorageModel = await this.Get(accountServiceModel, cancellationToken);
        UpsertStorageResource upsertStorage = await this.CreateOrUpdateStorageResource(processingStorageModelRequest, subscription, resourceGroup, existingStorageModel, cancellationToken);
        ProcessingStorageModel newStorageModel = this.ToModel(upsertStorage.StorageAccount, accountServiceModel, existingStorageModel);

        string responseCode = ResponseStatus.Created;
        // only persist the model definition if the storage account is successfully created along with all the other resources
        if (upsertStorage.Update)
        {
            responseCode = ResponseStatus.Updated;
            await this.storageAccountRepository.Update(newStorageModel, accountServiceModel.Id, cancellationToken);
        }
        else
        {
            try
            {
                await this.CreateManagementPolicy(upsertStorage.StorageAccount, cancellationToken);
                await this.CreateDefaultContainer(upsertStorage.StorageAccount, accountServiceModel, cancellationToken);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to create dependencies for storage account {newStorageModel.Properties.ResourceId}", ex);
                await this.DeleteStorageAccount(resourceGroup, newStorageModel.Name, cancellationToken);
                this.logger.LogError($"Deleted storage account {newStorageModel.Properties.ResourceId}", ex);

                throw;
            }

            await this.storageAccountRepository.Create(newStorageModel, accountServiceModel.Id, cancellationToken);
        }
        this.logger.LogInformation($"Successfully created storage account {newStorageModel.Properties.ResourceId}");

        return responseCode;
    }

    /// <inheritdoc/>
    public async Task<Uri> GetProcessingStorageSasUri(ProcessingStorageModel processingStorageModel, StorageSasRequest parameters, string containerName, CancellationToken cancellationToken)
    {
        string serviceEndpoint = processingStorageModel.GetDfsEndpoint();
        DataLakeServiceClient serviceClient = new(new Uri(serviceEndpoint), this.tokenCredential);
        DataLakeFileSystemClient fileSystemClient = new(new Uri($"{serviceEndpoint}/{containerName}"), this.tokenCredential);
        DataLakeDirectoryClient directoryClient = fileSystemClient.GetDirectoryClient(parameters.Path);

        return await GetUserDelegationSasDirectory(directoryClient, serviceClient, parameters);
    }

    /// <inheritdoc/>
    public async Task<string> ConstructContainerPath(string containerName, Guid accountId, CancellationToken cancellationToken)
    {
        ProcessingStorageModel storageModel = await this.Get(accountId, cancellationToken);

        ArgumentNullException.ThrowIfNull(storageModel, nameof(storageModel));

        return $"{storageModel.GetDfsEndpoint()}/{containerName}";
    }

    /// <summary>
    /// Get a user delegation Sas URI to the data lake directory.
    /// </summary>
    /// <param name="directoryClient"></param>
    /// <param name="dataLakeServiceClient"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    private async static Task<Uri> GetUserDelegationSasDirectory(DataLakeDirectoryClient directoryClient, DataLakeServiceClient dataLakeServiceClient, StorageSasRequest parameters)
    {
        // Get a user delegation key that's valid for seven days.
        // Use the key to generate any number of shared access signatures over the lifetime of the key.
        UserDelegationKey userDelegationKey = await dataLakeServiceClient.GetUserDelegationKeyAsync(DateTimeOffset.UtcNow.Subtract(TimeSpan.FromMinutes(5)), DateTimeOffset.UtcNow.AddDays(7));

        // Create a SAS token
        DataLakeSasBuilder sasBuilder = new()
        {
            FileSystemName = directoryClient.FileSystemName,
            Resource = "d",
            IsDirectory = true,
            Path = parameters.Path,
            Protocol = SasProtocol.Https,
            ExpiresOn = DateTimeOffset.UtcNow.Add(parameters.TimeToLive)
        };

        sasBuilder.SetPermissions(parameters.Permissions);

        DataLakeUriBuilder fullUri = new(directoryClient.Uri)
        {
            Sas = sasBuilder.ToSasQueryParameters(userDelegationKey, dataLakeServiceClient.AccountName)
        };

        return fullUri.ToUri();
    }

    private async Task<string> GetStorageAccountKey(StorageAccountResource storageAccount, CancellationToken cancellationToken)
    {
        AsyncPageable<StorageAccountKey> keys = storageAccount.GetKeysAsync(cancellationToken: cancellationToken);
        List<StorageAccountKey> response = new();
        await foreach (StorageAccountKey key in keys)
        {
            response.Add(key);
        }

        return response.First(x => x.KeyName.Equals("key2", StringComparison.OrdinalIgnoreCase)).Value;
    }

    /// <summary>
    /// Create container.
    /// </summary>
    /// <param name="storageAccount"></param>
    /// <param name="accountServiceModel"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task CreateDefaultContainer(StorageAccountResource storageAccount, AccountServiceModel accountServiceModel, CancellationToken cancellationToken)
    {
        await this.CreateContainer(storageAccount, accountServiceModel.DefaultCatalogId, cancellationToken);
    }

    private async Task<string> DetermineUpgradedSkuName(SubscriptionResource subscription, string location, string currentSku, CancellationToken cancellationToken)
    {
        IRequestHeaderContext requestContext = this.requestContextAccessor.GetRequestContext();
        if (this.exposureControl.IsGeoReplicationEnabled(requestContext))
        {
            if (currentSku.Equals(StorageSkuName.StandardZrs.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                bool isgeoRedundant = await this.CheckStorageAvailability(subscription, location, StorageSkuName.StandardRagzrs, StorageSkuTier.Standard, Azure.Management.Storage.Models.Kind.StorageV2, cancellationToken);
                if (isgeoRedundant)
                {
                    return StorageSkuName.StandardRagzrs.ToString();
                }
            }
            
            if (currentSku.Equals(StorageSkuName.StandardLrs.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                bool isgeoRedundant = await this.CheckStorageAvailability(subscription, location, StorageSkuName.StandardRagrs, StorageSkuTier.Standard, Azure.Management.Storage.Models.Kind.StorageV2, cancellationToken);
                if (isgeoRedundant)
                {
                    return StorageSkuName.StandardRagrs.ToString();
                }
            }
        }

        return currentSku;
    }

    private async Task<string> DetermineDefaultSkuName(SubscriptionResource subscription, string location, CancellationToken cancellationToken)
    {
        IRequestHeaderContext requestContext = this.requestContextAccessor.GetRequestContext();
        if (this.exposureControl.IsGeoReplicationEnabled(requestContext))
        {
            bool isgeoRedundant = await this.CheckStorageAvailability(subscription, location, StorageSkuName.StandardRagzrs, StorageSkuTier.Standard, Azure.Management.Storage.Models.Kind.StorageV2, cancellationToken);
            if (isgeoRedundant)
            {
                return StorageSkuName.StandardRagrs.ToString();
            }
            
            isgeoRedundant = await this.CheckStorageAvailability(subscription, location, StorageSkuName.StandardRagrs, StorageSkuTier.Standard, Azure.Management.Storage.Models.Kind.StorageV2, cancellationToken);
            if (isgeoRedundant)
            {
                return StorageSkuName.StandardRagrs.ToString();
            }
        }
        
        bool isZoneRedundant = await this.CheckStorageAvailability(subscription, location, StorageSkuName.StandardZrs, StorageSkuTier.Standard, Azure.Management.Storage.Models.Kind.StorageV2, cancellationToken);
        if (isZoneRedundant)
        {
            return StorageSkuName.StandardZrs.ToString();
        }

        return StorageSkuName.StandardLrs.ToString();
    }

    /// <summary>
    /// Will call the storage SKU API to determine if storage supports zone redundant storage for the subscription and location. The result is cached in memory.
    /// </summary>
    /// <param name="subscription"></param>
    /// <param name="location"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Whether the location and subscription support zone redundant storage.</returns>
    private async Task<bool> CheckStorageAvailability(
        SubscriptionResource subscription,
        string location,
        StorageSkuName skuName,
        StorageSkuTier skuTier,
        string skuKind,
        CancellationToken cancellationToken)
    {
        string cacheKey = $"{subscription.Id}-{location}-{skuName}-{skuTier}-{skuKind}";

        if (this.cache.TryGetValue(cacheKey, out bool cachedResult))
        {
            return cachedResult;
        }

        TimeSpan absoluteExpirationRelativeToNow = TimeSpan.FromMinutes(71);
        AsyncPageable<StorageSkuInformation> skus = subscription.GetSkusAsync(cancellationToken);

        await foreach (StorageSkuInformation sku in skus)
        {
            this.logger.LogInformation($"ProcessingStorage SKU Information: {JsonSerializer.Serialize(sku)}");
            if (sku.Name == skuName &&
                sku.Tier == skuTier &&
                sku.Kind == skuKind &&
                !sku.Restrictions.Any() &&
                sku.Locations.Contains(location, StringComparer.OrdinalIgnoreCase))
            {
                return this.cache.Set(cacheKey, true, absoluteExpirationRelativeToNow);
            }
        }

        return this.cache.Set(cacheKey, false, absoluteExpirationRelativeToNow);
    }

    /// <summary>
    /// Convert to the processing storage model.
    /// </summary>
    /// <param name="storageAccount"></param>
    /// <param name="accountServiceModel"></param>
    /// <param name="existingStorageModel"></param>
    /// <returns></returns>
    private ProcessingStorageModel ToModel(StorageAccountResource storageAccount, AccountServiceModel accountServiceModel, ProcessingStorageModel existingStorageModel)
    {
        bool isPartitionDnsEnabled = storageAccount.Data.DnsEndpointType == DnsEndpointType.AzureDnsZone;
        Uri endpoint = storageAccount.Data.PrimaryEndpoints.BlobUri;
        string dnsZone = isPartitionDnsEnabled ? StorageUtilities.GetStorageDnsZoneFromEndpoint(endpoint) : string.Empty;
        string endpointSuffix = StorageUtilities.GetStorageEndpointSuffix(endpoint, isPartitionDnsEnabled);
        DateTime now = DateTime.UtcNow;

        return new()
        {
            AccountId = Guid.Parse(accountServiceModel.Id),
            Id = existingStorageModel?.Id ?? Guid.NewGuid(),
            Name = this.storageConfiguration.StorageNamePrefix,
            TenantId = Guid.Parse(accountServiceModel.TenantId),
            CatalogId = Guid.Parse(accountServiceModel.DefaultCatalogId),
            Properties = new()
            {
                CreatedAt = existingStorageModel?.Properties?.CreatedAt ?? now,
                DnsZone = dnsZone,
                EndpointSuffix = endpointSuffix,
                LastModifiedAt = now,
                Location = accountServiceModel.Location,
                ResourceId = storageAccount.Id,
                Sku = storageAccount.Data.Sku.Name.ToString()
            },
        };
    }

    private async Task<UpsertStorageResource> CreateOrUpdateStorageResource(
        StorageAccountRequestModel storageAccountRequest,
        SubscriptionResource subscription,
        ResourceGroupResource resourceGroup,
        ProcessingStorageModel existingStorageModel,
        CancellationToken cancellationToken)
    {
        StorageAccountResource storageAccount = null;
        if (existingStorageModel == null)
        {
            storageAccountRequest.Sku = await this.DetermineDefaultSkuName(subscription, storageAccountRequest.Location, cancellationToken);
            this.logger.LogInformation($"Storage Resource Request: {JsonSerializer.Serialize(storageAccountRequest)}");

            storageAccount = await this.CreateStorageAccount(subscription, resourceGroup, storageAccountRequest, cancellationToken);
        }
        else
        {
            storageAccountRequest.Sku = await this.DetermineUpgradedSkuName(subscription, storageAccountRequest.Location, existingStorageModel.Properties.Sku, cancellationToken);
            ResourceIdentifier storageId = new(existingStorageModel.Properties.ResourceId);
            storageAccountRequest.Name = storageId.Name;
            this.logger.LogInformation($"Storage Resource Request: {JsonSerializer.Serialize(storageAccountRequest)}");
            try
            {
                storageAccount = await this.UpdateStorageAccount(resourceGroup, storageAccountRequest, cancellationToken);
            }
            catch (RequestFailedException e) when (e.ErrorCode == "SkuUpdateNotAvailable")
            {
                // Adding the  logic to handle the SKU update failure because of the resource unaviability then failling back to StandardLRS
                this.logger.LogWarning($"SkuUpdateNotAvailable exception occurred while CreateOrUpdateStorageResource account with sku{storageAccountRequest.Sku} from {existingStorageModel.Properties.Sku}, " +
                    $" skipping the update considering the sku avialibility constraint");
            }
        }
        return (storageAccount, existingStorageModel != null);
    }

    private record struct UpsertStorageResource(StorageAccountResource StorageAccount, bool Update)
    {
        public static implicit operator UpsertStorageResource((StorageAccountResource, bool) value) => new(value.Item1, value.Item2);
    }
}
