// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.ApiService;

using Microsoft.Purview.DataGovernance.Provisioning.Configurations;
using Microsoft.Purview.DataGovernance.Provisioning.Loggers;
using Microsoft.Extensions.Options;

internal static class ExceptionMiddlewareExtensions
{
    /// <summary>
    /// Adds the exception handler middleware to the pipeline.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="logger"></param>
    /// <param name="envConfig"></param>
    /// <param name="requestContextAccessor"></param>
    public static void ConfigureExceptionHandler(this IApplicationBuilder app, IServiceRequestLogger logger, IOptions<EnvironmentConfiguration> envConfig, IRequestContextAccessor requestContextAccessor)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                await ExceptionHandler.HandleException(context, logger, envConfig.Value, requestContextAccessor);
            });
        });
    }
}
