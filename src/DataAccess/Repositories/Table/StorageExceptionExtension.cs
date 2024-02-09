// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.DataAccess;

using global::Azure;
using Microsoft.Purview.DataGovernance.Provisioning.Common;

/// <summary>
/// Storage exception extension
/// </summary>
internal static class StorageExceptionExtension
{
    /// <summary>
    /// Converts <see cref="RequestFailedException"/> to <see cref="ServiceException"/>
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static ServiceException ToServiceException(this RequestFailedException exception)
    {
        ArgumentNullException.ThrowIfNull(exception, nameof(exception));

        return exception.ErrorCode switch
        {
            _ when string.Equals(StorageErrorCode.EntityNotFound, exception.ErrorCode, StringComparison.Ordinal) =>
                new ServiceError(
                    ErrorCategory.ResourceNotFound,
                    ErrorCode.StorageException,
                    exception.Message).ToException(),
            _ when string.Equals(StorageErrorCode.EntityAlreadyExists, exception.ErrorCode, StringComparison.Ordinal) =>
                new ServiceError(
                    ErrorCategory.Conflict,
                    ErrorCode.StorageException,
                    exception.Message).ToException(),
            _ when string.Equals(StorageErrorCode.InvalidInput, exception.ErrorCode, StringComparison.Ordinal) =>
                new ServiceError(
                    ErrorCategory.InputError,
                    ErrorCode.StorageException,
                    exception.Message).ToException(),
            _ when string.Equals(StorageErrorCode.UpdateConditionNotSatisfied, exception.ErrorCode, StringComparison.Ordinal) =>
                new ServiceError(
                    ErrorCategory.Conflict,
                    ErrorCode.StorageException,
                    exception.Message).ToException(),
            _ when string.Equals(StorageErrorCode.QueueNotFound, exception.ErrorCode, StringComparison.Ordinal) =>
                new ServiceError(
                    ErrorCategory.ResourceNotFound,
                    ErrorCode.StorageException,
                    exception.Message).ToException(),
            _ => new ServiceError(
                ErrorCategory.ServiceError,
                ErrorCode.StorageException,
                "Failed to execute operation in storage!").ToException(),
        };
    }
}
