// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.Core;

using Microsoft.AspNetCore.Http;
using Microsoft.Purview.DataGovernance.Provisioning.DataAccess;
using Microsoft.Purview.DataGovernance.Provisioning.Models;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides behavior on the core layer level.
/// </summary>
public static class CoreLayer
{
    /// <summary>
    /// Initializes the core layer.
    /// </summary>
    /// <param name="services">Gives the core layer a chance to configure its dependency injection.</param>
    public static IServiceCollection AddCoreLayer(this IServiceCollection services)
    {
        services.AddSingleton<ICertificateLoaderService, CertificateLoaderService>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IStorageCredentialsProvider, StorageCredentialsProvider>();
        services.AddSingleton<IKeyVaultAccessorService, KeyVaultAccessorService>();
        services.AddSingleton<ServiceHealthCheck>();
        services.AddSingleton<IProcessingStorageManager, ProcessingStorageManager>();

        services.AddScoped<IRequestHeaderContext, RequestHeaderContext>();

        services.AddTransient<IAzureResourceManagerFactory, AzureResourceManagerFactory>();

        services.AddHealthChecks().AddCheck<ServiceHealthCheck>("Ready");

        services.AddMemoryCache();

        return services;
    }
}
