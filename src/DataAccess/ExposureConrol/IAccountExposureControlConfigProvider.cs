// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.DataAccess;

using Microsoft.Purview.DataGovernance.Common;

/// <summary>
/// Expose Exposure Control APIs
/// </summary>
public interface IAccountExposureControlConfigProvider
{
    /// <summary>
    /// Determines if provisioning for Data Governance is enabled. By default this is false.
    /// </summary>
    /// <param name="accountId">The accountId</param>
    /// <param name="subscriptionId">The subscription id</param>
    /// <param name="tenantId">The tenant id</param>
    /// <returns></returns>
    public bool IsDataGovProvisioningEnabled(string accountId, string subscriptionId, string tenantId);

    /// <summary>
    /// Determines if geo replication for Data Governance is enabled. By default this is false.
    /// </summary>
    /// <param name="requestContext">The request context</param>
    /// <returns></returns>
    public bool IsGeoReplicationEnabled(IRequestHeaderContext requestContext);
}
