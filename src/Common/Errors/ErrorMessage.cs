// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.Common;

/// <summary>
/// Error messages shared across the service.
/// </summary>
public class ErrorMessage
{
    /// <summary>
    /// An unknown error (internal server error).
    /// </summary>
    public const string Unknown = "Internal server error. Please retry later or contact customer support.";

    /// <summary>
    /// Service exception
    /// </summary>
    public const string ServiceErrorMessage = "Service Exception, please retry later or contact customer support.";

    /// <summary>
    /// Service exception details
    /// </summary>
    public const string ServiceErrorDetailsMessage = "Service Error Occured";

    /// <summary>
    /// Downstream dependency error
    /// </summary>
    public const string DownstreamDependency =
        "Service is currently facing technical issues. Please retry later or contact customer support.";
}
