// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.ApiService.Extensions;

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Purview.DataGovernance.Loggers;
using OpenTelemetry.Audit.Geneva;
using System;

public class RequestAuditFilter : IActionFilter
{
    private readonly IServiceRequestLogger logger;

    public RequestAuditFilter(
        IServiceRequestLogger logger)
    {
        this.logger = logger;
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context != null)
        {
            this.Audit(context);
        }
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    private void Audit(ActionExecutedContext context)
    {
#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            var httpContext = context.HttpContext;
            var requestUri = httpContext.Request.GetDisplayUrl();
            var operationName = context.ActionDescriptor.DisplayName;

            string caller = "";

            if (requestUri.Contains("controlPlane"))
            {
                caller = "PurviewRP";
            }
            else
            {
                caller = "PurviewGateway";
            }

            var audit = new AuditRecord()
            {
                OperationName = operationName,
                OperationType = OperationType.Read,
                OperationResult = OperationResult.Success,
                OperationResultDescription = "Audit log",
                OperationAccessLevel = "Read",
                CallerAgent = caller,
                CallerIpAddress = httpContext.Connection?.RemoteIpAddress.ToString() ?? "1.1.1.1"
            };

            switch (httpContext.Request.Method)
            {
                case "GET":
                    audit.OperationType = OperationType.Read;
                    audit.OperationAccessLevel = "Read";
                    audit.AddCallerAccessLevel(audit.OperationAccessLevel);
                    break;
                case "POST":
                    audit.OperationType = OperationType.Create;
                    audit.OperationAccessLevel = "Write";
                    audit.AddCallerAccessLevel(audit.OperationAccessLevel);
                    break;
                case "PUT":
                case "PATCH":
                    audit.OperationType = OperationType.Update;
                    audit.OperationAccessLevel = "Write";
                    audit.AddCallerAccessLevel(audit.OperationAccessLevel);
                    break;
                case "DELETE":
                    audit.OperationType = OperationType.Delete;
                    audit.OperationAccessLevel = "Delete";
                    audit.AddCallerAccessLevel(audit.OperationAccessLevel);
                    break;
            }
            audit.AddOperationCategory(OperationCategory.ResourceManagement);
            audit.AddCallerAccessLevel(audit.OperationAccessLevel);
            audit.AddCallerIdentity(CallerIdentityType.Other, caller, "audit");
            this.logger.LogAudit(audit);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "AuditFailure: " + ex.Message);
        }
#pragma warning restore CA1031 // Do not catch general exception types
    }
}
