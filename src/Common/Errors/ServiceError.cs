namespace Microsoft.Purview.DataGovernance.Provisioning.Common;

/// <summary>
/// Represents a service error.
/// </summary>
public class ServiceError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceError" /> class.
    /// </summary>
    /// <param name="category">The error category.</param>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    public ServiceError(ErrorCategory category, ErrorCode code, string message)
    {
        this.Category = category;
        this.ErrorCode = code;
        this.Message = message;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceError" /> class with
    /// the default service error category.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    public ServiceError(ErrorCode code, string message)
        : this(ErrorCategory.ServiceError, code, message)
    {
    }

    /// <summary>
    /// Gets the error category.
    /// </summary>
    public ErrorCategory Category { get; }

    /// <summary>
    /// Gets the error code which identifies the semantics of the error.
    /// </summary>
    public ErrorCode ErrorCode { get; }

    /// <summary>
    /// Gets the error message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Creates a general service error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A configured service error.</returns>
    public static ServiceError GeneralError(ErrorCode code, string message = "")
    {
        return new ServiceError(ErrorCategory.ServiceError, code, message);
    }

    /// <summary>
    /// Creates an input error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A configured input error.</returns>
    public static ServiceError InputError(ErrorCode code, string message = "")
    {
        return new ServiceError(ErrorCategory.InputError, code, message);
    }

    /// <summary>
    /// Creates a down stream error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A configured down stream error.</returns>
    public static ServiceError DownStreamError(ErrorCode code, string message = "")
    {
        return new ServiceError(ErrorCategory.DownStreamError, code, message);
    }

    /// <summary>
    /// Creates a resource not found error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A configured resource not found error.</returns>
    public static ServiceError ResourceNotFound(ErrorCode code, string message = "")
    {
        return new ServiceError(ErrorCategory.ResourceNotFound, code, message);
    }

    /// <summary>
    /// Creates a forbidden error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A configured forbidden error.</returns>
    public static ServiceError Forbidden(ErrorCode code, string message = "")
    {
        return new ServiceError(ErrorCategory.Forbidden, code, message);
    }

    /// <summary>
    /// Creates a too many requests error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A configured too many requests error.</returns>
    public static ServiceError TooManyRequests(ErrorCode code, string message = "")
    {
        return new ServiceError(ErrorCategory.TooManyRequests, code, message);
    }

    /// <summary>
    /// Creates a conflict error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A configured conflict error.</returns>
    public static ServiceError Conflict(ErrorCode code, string message = "")
    {
        return new ServiceError(ErrorCategory.Conflict, code, message);
    }

    /// <summary>
    /// Creates a deployment setup error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A configured deployment setup error.</returns>
    public static ServiceError DeploymentSetupError(ErrorCode code, string message = "")
    {
        return new ServiceError(ErrorCategory.DeploymentSetupError, code, message);
    }

    /// <summary>
    /// Creates an authentication error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A configured authentication error.</returns>
    public static ServiceError AuthenticationError(ErrorCode code, string message = "")
    {
        return new ServiceError(ErrorCategory.AuthenticationError, code, message);
    }

    /// <summary>
    /// Creates a concurrency mismatch error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A configured concurrency mismatch error.</returns>
    public static ServiceError ConcurrencyMismatch(ErrorCode code, string message = "")
    {
        return new ServiceError(ErrorCategory.ConcurrencyMismatch, code, message);
    }

    /// <summary>
    /// Creates a transient state error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A configured transient state error.</returns>
    public static ServiceError TransientStateError(ErrorCode code, string message = "")
    {
        return new ServiceError(ErrorCategory.TransientStateError, code, message);
    }

    /// <summary>
    /// Creates a service exception using the current service error object.
    /// </summary>
    /// <returns></returns>
    public ServiceException ToException()
    {
        return new ServiceException(this.Category, this.ErrorCode, this.Message);
    }

    /// <summary>
    /// Throws a service exception using the current service error object.
    /// </summary>
    /// <returns></returns>
    public void Throw()
    {
        throw new ServiceException(this.Category, this.ErrorCode, this.Message);
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        if (obj is ServiceError otherServiceError)
        {
            return this.Category == otherServiceError.Category &&
                this.ErrorCode == otherServiceError.ErrorCode &&
                this.Message.Equals(otherServiceError.Message, StringComparison.Ordinal);
        }

        return false;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return this.Category.GetHashCode() ^ this.ErrorCode.GetHashCode() ^ this.Message.GetHashCode();
    }
}