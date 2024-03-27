// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.ApiService;

using Microsoft.Purview.DataGovernance.EventHub;

/// <summary>
/// Extension methods for configuring the Api Service.
/// </summary>
public static class ApiService
{
    /// <summary>
    /// Initializes the api service.
    /// </summary>
    /// <param name="services">Gives the api service a chance to configure its dependency injection.</param>
    public static IServiceCollection AddApiServices(
        this IServiceCollection services)
    {
        services.AddSingleton<IEventPublisher, EventHubPublisher>();

        return services;
    }
}
