﻿// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.ProvisioningService;

using Microsoft.Azure.ProjectBabylon.Metadata.Models;
using Microsoft.Purview.DataGovernance.Provisioning.Configurations;
using Microsoft.Purview.DataGovernance.Provisioning.DataAccess;
using Microsoft.Purview.DataGovernance.Provisioning.ProvisioningService.Configurations;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides behavior on the provisioning service.
/// </summary>
public static class Extension
{
    /// <summary>
    /// Initializes the core layer.
    /// </summary>
    /// <param name="services">Gives the core layer a chance to configure its dependency injection.</param>
    public static IServiceCollection AddProvisioningService(this IServiceCollection services)
    {
        services.AddScoped<IPartnerService<AccountServiceModel, IPartnerDetails>, AccountPartnerService>();
        services.AddPartnerHttpClient(PartnerServiceBase.PartnerClient);

        return services;
    }

    /// <summary>
    /// Register the default http client 
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="name">The user agent for the http client</param>
    /// <returns>Http client builder</returns>
    private static IHttpClientBuilder AddPartnerHttpClient(this IServiceCollection services, string name)
    {
        HttpClientSettings httpClientSettings = new()
        {
            Name = name,
            UserAgent = "DGProvisioning",
            RetryCount = 5
        };

        return services.AddCustomHttpClient<PartnerConfiguration>(httpClientSettings,
            (serviceProvider, request, policy) => { });
    }
}
