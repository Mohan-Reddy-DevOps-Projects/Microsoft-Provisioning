namespace Microsoft.Purview.DataGovernance.Provisioning.Common;

/// <summary>
/// Defines broad error categories the service may run into.
/// </summary>
public enum ErrorCategory
{
    /// <summary>
    /// An input error.
    /// </summary>
    InputError,

    /// <summary>
    /// A problem in the service code which is unrelated to caller's inputs.
    /// </summary>
    ServiceError,

    /// <summary>
    /// A dependency's problem.
    /// </summary>
    DownStreamError,

    /// <summary>
    /// The requested resource was not found.
    /// </summary>
    ResourceNotFound,

    /// <summary>
    /// The requested operation is not allowed.
    /// </summary>
    Forbidden,

    /// <summary>
    /// An operation is being called too frequently. Callers need to back off for sometime.
    /// </summary>
    TooManyRequests,

    /// <summary>
    /// The operation cannot be completed because it results in a conflict with already existing rule/resource.
    /// </summary>
    Conflict,

    /// <summary>
    /// Deployment setup is incorrect.
    /// </summary>
    DeploymentSetupError,

    /// <summary>
    /// Authentication problem.
    /// </summary>
    AuthenticationError,

    /// <summary>
    /// Optimistic Concurrency Error.
    /// </summary>
    ConcurrencyMismatch,

    /// <summary>
    /// If the state is transient and can possibly lead to a successful execution eventually.
    /// </summary>
    TransientStateError
}
