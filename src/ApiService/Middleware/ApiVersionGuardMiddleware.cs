﻿// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.ApiService;

using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Purview.DataGovernance.Provisioning.Common;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Microsoft.Purview.DataGovernance.Common.Configuration;
using Microsoft.Purview.DataGovernance.Common;

/// <summary>
/// Middleware that validates the api-version parameter of incoming requests.
/// </summary>
public class ApiVersionGuardMiddleware
{
    private const string controlPlaneRoutePrefix = "/controlplane/account";

    private readonly RequestDelegate next;
    private readonly EnvironmentConfiguration environmentConfiguration;

    /// <summary>
    /// Constructor for the <see cref="ApiVersionGuardMiddleware"/> type.
    /// </summary>
    /// <param name="next"></param>
    /// <param name="environmentConfiguration"></param>
    public ApiVersionGuardMiddleware(RequestDelegate next, IOptions<EnvironmentConfiguration> environmentConfiguration)
    {
        this.next = next;
        this.environmentConfiguration = environmentConfiguration.Value;
    }

    /// <summary>
    /// Validates that the api-version parameter is a supported version.
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="requestHeaderContext"></param>
    public async Task InvokeAsync(HttpContext httpContext, IRequestHeaderContext requestHeaderContext)
    {
        // Skip API version validation for control plane requests.
        if (httpContext.Request.Path.HasValue && httpContext.Request.Path.Value.StartsWith(controlPlaneRoutePrefix, StringComparison.OrdinalIgnoreCase))
        {
            await this.next(httpContext);
            return;
        }

        if (!this.environmentConfiguration.PermittedApiVersions.Any(version => string.Equals(version, requestHeaderContext.ApiVersion, StringComparison.OrdinalIgnoreCase)))
        {
            var responseBody = new ErrorResponseModel
            {
                Error = new ErrorModel(
                    ErrorCode.UnsupportedApiVersionParameter.Code.ToString(),
                    ErrorCode.UnsupportedApiVersionParameter.FormatMessage(requestHeaderContext.ApiVersion ?? "", string.Join(',', this.environmentConfiguration.PermittedApiVersions)))
            };

            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            // HttpResponse has a WriteAsJsonAsync method that serializes the
            // response and sets the content-type. It doesn't respect our
            // serializer settings so we do it manually.
            httpContext.Response.ContentType = "application/json; charset=utf-8";
            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(responseBody), Encoding.UTF8);

            return;
        }

        await this.next(httpContext);
    }
}

/// <summary>
/// Extension methods for adding <see cref="ApiVersionGuardMiddleware"/> to the request pipeline.
/// </summary>
public static class ApiVersionGuardMiddlewareExtensions
{
    /// <summary>
    /// Adds an instance of <see cref="ApiVersionGuardMiddleware"/> to the request pipeline.
    /// </summary>
    /// <param name="builder"></param>
    public static IApplicationBuilder UseApiVersionGuard(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ApiVersionGuardMiddleware>();
    }
}

