namespace Microsoft.Purview.DataGovernance.Provisioning.Common;

/// <summary>
/// Groups service error codes. Extend and add your own service errors.
/// </summary>
public class ServiceErrorCode
{
    internal const int UnknownErrorCode = 1000;

    /// <summary>
    /// ServiceErrorCode for unknown errors.
    /// </summary>
    public static ServiceErrorCode Unknown = new(UnknownErrorCode);

    /// <summary>
    /// ServiceErrorCode for Missing Field.
    /// </summary>
    public static ServiceErrorCode MissingField = new(1001, "{0} is required.");

    /// <summary>
    /// ServiceErrorCode for InvalidField.
    /// </summary>
    public static ServiceErrorCode InvalidField = new(1002);

    /// <summary>
    /// Numeric error code.
    /// </summary>
    public int Code { get; }

    /// <summary>
    /// Message related to the error.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Constructor for ServiceErrorCode.
    /// </summary>
    /// <param name="code">Numeric code of the error.</param>
    /// <param name="message">Optional error message associated with the code.</param>
    protected ServiceErrorCode(int code, string message = null)
    {
        this.Code = code;
        this.Message = message;
    }

    /// <summary>
    /// Formats the Message using the provided arguments.
    /// </summary>
    /// <param name="args">Arguments to provide to the Message.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Thrown when Message is null.</exception>
    public string FormatMessage(params string[] args)
    {
        if (string.IsNullOrWhiteSpace(this.Message))
        {
            throw new InvalidOperationException("ServiceErrorCode without a message cannot be formatted.");
        }

        return string.Format(this.Message, args);
    }
}
