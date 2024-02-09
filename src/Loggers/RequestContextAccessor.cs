// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.Loggers;

using Microsoft.AspNetCore.Http;
using Microsoft.Purview.DataGovernance.Provisioning.Models;

internal class RequestContextAccessor : IRequestContextAccessor
{
    private static readonly AsyncLocal<IRequestHeaderContext> asyncLocalRequestContext = new();
    private const string HttpContextKey = "PurviewRequestContext";
    private readonly IHttpContextAccessor httpContextAccessor;

    public RequestContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public IRequestHeaderContext GetRequestContext()
    {
        if (asyncLocalRequestContext.Value != null)
        {
            return asyncLocalRequestContext.Value;
        }

        if (this.httpContextAccessor?.HttpContext?.Items != null &&
            this.httpContextAccessor.HttpContext.Items.TryGetValue(HttpContextKey, out var item) &&
            item is IRequestHeaderContext typedRequestContext)
        {
            return typedRequestContext;
        }

        var requestContext = new RequestHeaderContext(this.httpContextAccessor);

        this.SetRequestContext(requestContext);

        return requestContext;
    }

    public void SetRequestContext(IRequestHeaderContext requestContext)
    {
        if (this.httpContextAccessor?.HttpContext?.Items != null)
        {
            this.httpContextAccessor.HttpContext.Items[HttpContextKey] = requestContext;
        }

        asyncLocalRequestContext.Value = requestContext;
    }
}
