﻿// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.DataAccess;

using System;
using System.Net.Http;
using Microsoft.Azure.ProjectBabylon.Metadata;
using Microsoft.Purview.DataGovernance.Provisioning.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.Purview.DataGovernance.Loggers;

/// <inheritdoc/>
internal sealed class MetadataServiceClientFactory : ClientFactory<ProjectBabylonMetadataClient>, IDisposable
{
    private const string ApiVersion = "2019-11-01-preview";
    private readonly MetadataServiceConfiguration config;

    public static string HttpClientName { get; } = "MetadataServiceClient";

    protected override string ClientName => HttpClientName;

    /// <summary>
    /// Public constructor
    /// </summary>
    /// <param name="config">Metadata client configuration</param>
    /// <param name="httpClientFactory">Http client factory</param>
    /// <param name="logger">Logger</param>
    public MetadataServiceClientFactory(
        IOptions<MetadataServiceConfiguration> config,
        IHttpClientFactory httpClientFactory,
        IServiceRequestLogger logger) : base(httpClientFactory, logger)
    {
        this.config = config.Value;
    }

    protected override ProjectBabylonMetadataClient ConfigureClient(HttpClient httpClient)
    {
        return new ProjectBabylonMetadataClient(httpClient, true)
        {
            BaseUri = new Uri(this.config.Endpoint),
            ApiVersion = ApiVersion,
        };
    }
}
