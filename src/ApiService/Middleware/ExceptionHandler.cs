﻿// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.ApiService;

using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Net.Http.Headers;
using Microsoft.Purview.DataGovernance.Provisioning.Common;
using Microsoft.Purview.DataGovernance.Loggers;
using Microsoft.Purview.DataGovernance.Common.Configuration;

internal static class ExceptionHandler
{
    private static readonly JsonSerializerSettings jsonSerializerSettings = new()
    {
        MaxDepth = 128,
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        NullValueHandling = NullValueHandling.Ignore
    };

    /// <summary>
    /// Handles converting the original exception to a known exception and writes the error model to the response.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    /// <param name="envConfig"></param>
    /// <param name="requestContextAccessor"></param>
    /// <returns></returns>
    public static async Task HandleException(HttpContext context, IServiceRequestLogger logger, EnvironmentConfiguration envConfig, IRequestContextAccessor requestContextAccessor)
    {
        IExceptionHandlerFeature contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (contextFeature == null)
        {
            return;
        }
       
        Exception exception = contextFeature.Error;
        HttpStatusCode statusCode = ExceptionConverter.GetHttpStatusCode(exception);

        ErrorResponseModel errorResponse = new()
        {
            Error = ExceptionConverter.CreateErrorModel(exception, statusCode, envConfig, requestContextAccessor.GetRequestContext())
        };

        await ModifyHttpResponse(context, statusCode, errorResponse);

        logger.LogError($"ExceptionMiddleware| Http context response has been written with status: {statusCode}.", contextFeature.Error);
    }

    /// <summary>
    /// Writes the error model to the response.
    /// </summary>
    /// <param name="context">The http context.</param>
    /// <param name="statusCode">The terminal status code.</param>
    /// <param name="errorResponse">The error model.</param>
    /// <returns></returns>
    private async static Task ModifyHttpResponse(HttpContext context, HttpStatusCode statusCode, ErrorResponseModel errorResponse)
    {
        // Build HTTP response
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
        await context.Response.WriteAsync(JsonConvert.SerializeObject(
            errorResponse,
            jsonSerializerSettings), Encoding.UTF8);
    }
}

