﻿// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.Core;

using System;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using System.Threading;
using global::Azure;
using global::Azure.Identity;
using global::Azure.Security.KeyVault.Secrets;
using Microsoft.Purview.DataGovernance.Provisioning.Common.Extensions;
using Microsoft.Purview.DataGovernance.Provisioning.Configurations;
using Microsoft.Extensions.Options;
using ErrorCode = Common.ErrorCode;
using Microsoft.Purview.DataGovernance.Common;
using Microsoft.Purview.DataGovernance.Provisioning.Common;
using Microsoft.Purview.DataGovernance.Loggers;
using System.Text.Json;

/// <summary>
/// Access an Azure key vault using a managed identity
/// </summary>
public class KeyVaultAccessorService : IKeyVaultAccessorService, IDisposable
{
    private const string DefaultErrorMessage = "Failed to get key vault resource";
    private readonly IServiceRequestLogger logger;
    private readonly SecretClient secretClient;
    private bool isDisposed = false;

    /// <summary>
    /// Constructor for the key vault accessor service
    /// </summary>
    public KeyVaultAccessorService(
        AzureCredentialFactory credentialFactory,
        IOptions<KeyVaultConfiguration> keyVaultConfig,
        IServiceRequestLogger logger)
    {
        Uri authorityHost = new(keyVaultConfig.Value.Authority);
        DefaultAzureCredential tokenCredential = credentialFactory.CreateDefaultAzureCredential(authorityHost);
        this.secretClient = new SecretClient(
            new Uri(keyVaultConfig.Value.BaseUrl),
            tokenCredential);
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task<KeyVaultSecret> GetSecretAsync(string secretName, CancellationToken cancellationToken, string version = null)
    {
        if (string.IsNullOrEmpty(secretName))
        {
            throw new ArgumentNullException(nameof(secretName));
        }

        try
        {
            Response<KeyVaultSecret> response = await this.secretClient.GetSecretAsync(secretName, version, cancellationToken);

            return response.Value;
        }
        catch (RequestFailedException keyVaultException)
        {
            if (keyVaultException.Status == (int)HttpStatusCode.NotFound)
            {
                this.logger.LogWarning($"Secret {secretName} not found in key vault {this.secretClient.VaultUri}.");

                return null;
            }

            this.logger.LogError(
                FormattableString.Invariant($"Failed to read secret {secretName} from {this.secretClient.VaultUri}."),
                keyVaultException);

            throw new ServiceError(
                    ErrorCategory.ServiceError,
                    ErrorCode.KeyVault_GetSecretError,
                    JsonSerializer.Serialize(keyVaultException))
                .ToException();
        }
        catch (Exception exception)
        {
            this.logger.LogError(
                FormattableString.Invariant(
                    $"Failed to retrieve secret {secretName} from vault {this.secretClient.VaultUri}."),
                exception);

            throw new ServiceError(
                    ErrorCategory.ServiceError,
                    ErrorCode.KeyVault_GetSecretError,
                    DefaultErrorMessage)
                .ToException();
        }
    }

    /// <inheritdoc />
    public async Task<KeyVaultSecret> SetSecretAsync(string secretName, SecureString secretValue, CancellationToken cancellationToken)
    {
        return await this.secretClient.SetSecretAsync(secretName, secretValue.ToPlainString(), cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose key vault client
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing && !this.isDisposed)
        {
            this.isDisposed = true;
        }
    }
}
