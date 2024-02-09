namespace Microsoft.Purview.DataGovernance.Provisioning.Common;

using System.Runtime.Serialization;

[Serializable]
public class ServiceException : Exception
{
    /// <inheritdoc />
    public ServiceException()
    {
        this.Details = string.Empty;
        this.Errors = Array.Empty<ErrorModel>();
    }

    /// <inheritdoc />
    [Obsolete]
    protected ServiceException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
        this.Details = string.Empty;
        this.Errors = Array.Empty<ErrorModel>();
    }

    /// <inheritdoc />
    public ServiceException(string message) : base(message)
    {
        this.Details = string.Empty;
        this.Errors = Array.Empty<ErrorModel>();
    }

    /// <inheritdoc />
    public ServiceException(string message, Exception innerException) : base(message, innerException)
    {
        this.Details = string.Empty;
        this.Errors = Array.Empty<ErrorModel>();
    }

    /// <inheritdoc />
    public ServiceException(
        ErrorCategory category,
        ServiceErrorCode code,
        string message,
        string details = null,
        Exception innerException = null,
        ErrorModel[] errors = null)
        : base(message, innerException)
    {
        this.Category = category;
        this.Code = code;
        this.Details = details ?? string.Empty;
        this.Errors = errors ?? Array.Empty<ErrorModel>();
    }

    /// <inheritdoc />
    public ErrorCategory Category { get; }

    /// <inheritdoc />
    public ServiceErrorCode Code { get; }

    /// <inheritdoc />
    public string Details { get; }

    /// <inheritdoc />
    public ErrorModel[] Errors { get; }
}