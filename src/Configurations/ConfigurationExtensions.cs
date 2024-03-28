// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.Configurations;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Purview.DataGovernance.Common.Configuration;
using Microsoft.Purview.DataGovernance.EventHub.Configuration;

/// <summary>
/// Extension methods for adding configurations
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Configurations for api service.
    /// </summary>
    public static IServiceCollection AddApiServiceConfigurations(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCommonConfigurations(configuration)
            .Configure<AllowListedCertificateConfiguration>(configuration.GetSection("allowListedCertificate"))
            .AddProvisioningConfigurations(configuration);

        return services;
    }

    /// <summary>
    /// Configurations common for api and worker service.
    /// </summary>
    private static IServiceCollection AddCommonConfigurations(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions()
            .Configure<CertificateSetConfiguration>(configuration.GetSection("apiServiceCertificateSet"))
            .Configure<GenevaConfiguration>(configuration.GetSection("geneva"))
            .Configure<EnvironmentConfiguration>(configuration.GetSection("environment"))
            .Configure<JobConfiguration>(configuration.GetSection("jobManagerConfiguration"))
            .Configure<ServiceConfiguration>(configuration.GetSection("service"))
            .Configure<AccountStorageTableConfiguration>(configuration.GetSection("accountStorageTable"))
            .Configure<ProcessingStorageConfiguration>(configuration.GetSection("processingStorage"))
            .Configure<ProcessingStorageAuthConfiguration>(configuration.GetSection("processingStorageAuth"))
            .Configure<MetadataServiceConfiguration>(configuration.GetSection("metadataService"))
            .Configure<ExposureControlConfiguration>(configuration.GetSection("exposureControl"))
            .Configure<KeyVaultConfiguration>(configuration.GetSection("keyVault"))
            .Configure<EventHubPublisherConfiguration>(configuration.GetSection("provisionChangePublisher"));

        return services;
    }

    /// <summary>
    /// Configurations common for api and worker service.
    /// </summary>
    private static IServiceCollection AddProvisioningConfigurations(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions()
            .Configure<PartnerConfiguration>(configuration.GetSection("partner"));
        return services;
    }
}
