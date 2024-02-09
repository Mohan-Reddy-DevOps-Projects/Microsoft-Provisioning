// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.Loggers;

using Microsoft.Purview.DataGovernance.Provisioning.Models;

/// <summary>
/// The request context accessor.
/// </summary>
public interface IRequestContextAccessor
{
    /// <summary>
    /// Gets the request context.
    /// </summary>
    /// <returns></returns>
    IRequestHeaderContext GetRequestContext();

    /// <summary>
    /// Sets the request context.
    /// </summary>
    /// <param name="requestContext"></param>
    void SetRequestContext(IRequestHeaderContext requestContext);
}
