// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.Loggers;

using Microsoft.Purview.DataGovernance.Provisioning.Configurations;
using Microsoft.Purview.DataGovernance.Provisioning.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <inheritdoc cref="ServiceLogger" />
/// <inheritdoc cref="IServiceRequestLogger" />
/// <summary>
/// Scoped logger implementation class.
/// </summary>
internal class ServiceRequestLogger : ServiceLogger, IServiceRequestLogger
{
    private readonly IRequestContextAccessor requestContextAccessor;

    /// <inheritdoc />
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceRequestLogger" /> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory to use</param>
    /// <param name="requestContextAccessor">The correlation context.</param>
    /// <param name="environmentConfiguration">Environment configuration.</param>
    public ServiceRequestLogger(
        ILoggerFactory loggerFactory,
        IRequestContextAccessor requestContextAccessor,
        IOptions<EnvironmentConfiguration> environmentConfiguration)
        : base(loggerFactory, environmentConfiguration)
    {
        this.requestContextAccessor = requestContextAccessor;
    }

    /// <inheritdoc />
    protected override IRequestHeaderContext GetRequestHeaderContext()
    {
        return this.requestContextAccessor?.GetRequestContext();
    }
}
