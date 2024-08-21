// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.Core;

using Microsoft.Purview.DataGovernance.Provisioning.Models;
using Microsoft.Purview.DataGovernance.Provisioning.DataAccess;
using System;
using System.Threading.Tasks;
using Microsoft.Purview.DataGovernance.Loggers;
using global::Azure;
using System.Net;
using Microsoft.Purview.DataGovernance.Provisioning.Common;

public class CatalogConfigService : ICatalogConfigService
{
    private readonly ICatalogConfigRepository catalogConfigRepository;
    private readonly IServiceRequestLogger logger;

    public CatalogConfigService(ICatalogConfigRepository catalogConfigRepository, IServiceRequestLogger logger)
    {
        this.catalogConfigRepository = catalogConfigRepository;
        this.logger = logger;
    }

    public async Task<CatalogConfigModel> CreateCatalogConfigAsync(string accountId, string tenantId, CancellationToken cancellation)
    {
        this.logger.LogInformation($"Creating catalog config.");
        DateTime now = DateTime.UtcNow;
        var catalogConfigModel = new CatalogConfigModel()
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.Parse(tenantId),
            Sku = CatalogSkuName.Basic,
            Features = new CatalogFeaturesModel()
            {
                DataEstateHealth = new CatalogFeatureSettingsModel()
                {
                    Mode = CatalogSkuMode.On,
                },
                DataQuality = new CatalogFeatureSettingsModel()
                {
                    Mode = CatalogSkuMode.On,
                }
            },
            CreatedAt = now,
            ModifiedAt = now,
        };
        var existModel = await this.catalogConfigRepository.GetSingle(accountId, cancellation).ConfigureAwait(false);
        if (existModel == null)
        {
            var config = await this.catalogConfigRepository.Create(accountId, catalogConfigModel, cancellation).ConfigureAwait(false);
            this.logger.LogInformation($"Created catalog config for account: {accountId}");
            return config;
        }
        this.logger.LogInformation($"Catalog config exists for account: {accountId}");
        return existModel;
    }

    public async Task DeleteCatalogConfigAsync(string accountId, CancellationToken cancellationToken)
    {
        this.logger.LogInformation($"Deleting catalog config.");
        try
        {
            await this.catalogConfigRepository.Delete(accountId, cancellationToken).ConfigureAwait(false);
            this.logger.LogInformation($"Deleted catalog config.");
        }
        catch (ServiceException ex) when (ex.Category == ErrorCategory.ResourceNotFound)
        {
            this.logger.LogWarning($"Catalog config already deleted.");
        }
    }

    public async Task<CatalogConfigModel> GetCatalogConfigAsync(string accountId, CancellationToken cancellationToken)
    {
        CatalogConfigModel model = await this.catalogConfigRepository.GetSingle(accountId, cancellationToken).ConfigureAwait(false);
        if (model == null)
        {
            throw new ServiceError(
                ErrorCategory.ServiceError,
                ErrorCode.Config_GetConfigError,
                "Catalog config not found"
              ).ToException();
        }
        return model;
    }

    public async Task<CatalogConfigModel> SetCatalogConfigAsync(string accountId, CatalogConfigModel model, CancellationToken cancellationToken)
    {
        CatalogConfigModel currentModel = await this.catalogConfigRepository.GetSingle(accountId, cancellationToken).ConfigureAwait(false);
        if (currentModel == null)
        {
            throw new ServiceError(
                ErrorCategory.ServiceError,
                ErrorCode.Config_GetConfigError,
                "Catalog config not found"
              ).ToException();
        }
        CatalogConfigModel updatedModel = new CatalogConfigModel()
        {
            Id = currentModel.Id,
            TenantId = currentModel.TenantId,
            CreatedAt = currentModel.CreatedAt,
            Sku = model.Sku,
            Features = currentModel.Features,
            ModifiedAt = DateTime.UtcNow,
        };
        return await this.catalogConfigRepository.Update(accountId, updatedModel, cancellationToken).ConfigureAwait(false);
    }
}
