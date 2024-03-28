// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.ApiService;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Purview.DataGovernance.Common;
using Microsoft.Purview.DataGovernance.EventHub;
using Microsoft.Purview.DataGovernance.EventHub.Configuration;
using Microsoft.Purview.DataGovernance.Loggers;
using System.Configuration;

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
        services.AddSingleton<IEventPublisher>(services => 
        {
            IServiceRequestLogger logger = services.GetService<IServiceRequestLogger>();
            IOptions<EventHubPublisherConfiguration> configuration = services.GetService<IOptions<EventHubPublisherConfiguration>>();
            AzureCredentialFactory credentialFactory = services.GetService<AzureCredentialFactory>();
            return new EventHubPublisher(logger, configuration.Value, credentialFactory);
        });

        return services;
    }
}
