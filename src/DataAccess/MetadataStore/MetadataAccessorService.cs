﻿// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.DataAccess;

using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.ProjectBabylon.Metadata;
using Microsoft.Azure.ProjectBabylon.Metadata.Models;
using Microsoft.Purview.DataGovernance.Loggers;
using Microsoft.Purview.DataGovernance.Provisioning.Common;
using Microsoft.Rest;

internal class MetadataAccessorService : IMetadataAccessorService
{
    private readonly IServiceRequestLogger logger;
    private readonly MetadataServiceClientFactory metadataServiceClientFactory;

    public MetadataAccessorService(
        MetadataServiceClientFactory metadataServiceClientFactory,
        IServiceRequestLogger logger)
    {
        this.metadataServiceClientFactory = metadataServiceClientFactory;
        this.logger = logger;
    }

    /// <inheritdoc/>
    public void Initialize()
    {
        this.GetMetadataServiceClient();
    }

    private IProjectBabylonMetadataClient GetMetadataServiceClient()
    {
        return this.metadataServiceClientFactory.GetClient();
    }

    /// <inheritdoc/>
    public async Task<StorageTokenKey> GetProcessingStorageSasToken(
        Guid accountId,
        string blobPath,
        CancellationToken cancellationToken)
    {
        StorageSasRequest storageSasRequest = new()
        {
            Services = "b",
            Permissions = "rl",
            BlobPath = blobPath,
            TimeToLive = TimeSpan.FromHours(1).ToString(@"hh\:mm\:ss")
        };

        try
        {
            IProjectBabylonMetadataClient client = this.GetMetadataServiceClient();
            HttpOperationResponse<StorageTokenKey> response = await client.AccountProcessingStorageSasToken.GetWithHttpMessagesAsync(accountId.ToString(), storageSasRequest, LookupType.ByAccountId, cancellationToken: cancellationToken);

            return response.Body;
        }
        catch (ErrorResponseModelException erx) when (erx.Response?.StatusCode == HttpStatusCode.NotFound)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw this.LogAndConvert(accountId.ToString(), exception);
        }
    }

    private ServiceException LogAndConvert(
        string accountId,
        Exception exception)
    {
        // Logging as error to avoid multiple criticals for each try. Higher level caller will log critical on failure.
        this.logger.LogError(
            FormattableString.Invariant(
                $"Failed to perform operation on {accountId}:"),
            exception);

        return new ServiceError(
                ErrorCategory.ServiceError,
                ErrorCode.MetadataServiceException,
                ErrorMessage.DownstreamDependency)
            .ToException();
    }
}
