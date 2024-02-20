// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.Loggers;

using System;
using System.Net.Http;
using Polly;

/// <summary>
/// A factory for actions that log retries.
/// </summary>
public static class LoggerRetryActionFactory
{
    /// <summary>
    /// Creates an Action that logs retries of service cause as an error.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="serviceName"></param>
    /// <returns></returns>
    public static Action<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context> CreateHttpClientRetryAction(
        IServiceRequestLogger logger,
        string serviceName)
    {
        return (result, _, retryCount, _) =>
        {
            logger.LogError($"Retry attempt {retryCount} for {serviceName} after failure.",
                exception: result?.Exception);
        };
    }

    /// <summary>
    /// Creates an Action that logs retries of service cause as an error.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="serviceName"></param>
    /// <returns></returns>
    public static Action<Exception, TimeSpan, int, Context> CreateWorkerRetryAction(
        IServiceRequestLogger logger,
        string serviceName)
    {
        return (result, _, retryCount, _) =>
        {
            logger.LogError(
                $"Retry attempt {retryCount} for {serviceName} after failure.",
                result);
        };
    }
}
