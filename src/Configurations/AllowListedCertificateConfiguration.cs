﻿// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.Configurations;

using System.Collections.Generic;
using Microsoft.Purview.DataGovernance.Provisioning.Common;

/// <summary>
/// Client certificate configuration model.
/// </summary>
public class AllowListedCertificateConfiguration
{
    /// <summary>
    /// Allow listed issuer names.
    /// </summary>
    public List<string> AllowListedIssuerNames { get; set; }

    /// <summary>
    /// Allow listed data plane subject names.
    /// </summary>
    public List<string> AllowListedDataPlaneSubjectNames { get; set; }

    /// <summary>
    /// Allow listed control plane subject names.
    /// </summary>
    public List<string> AllowListedControlPlaneSubjectNames { get; set; }

    /// <summary>
    /// Get list of allowed subject names.
    /// </summary>
    /// <param name="certificateSet">Input certificate set.</param>
    /// <returns></returns>
    /// <exception cref="ServiceException"></exception>
    public List<string> GetAllowListedSubjectNames(CertificateSet certificateSet)
    {
        switch (certificateSet)
        {
            case CertificateSet.DataPlane:
                return this.AllowListedDataPlaneSubjectNames;
            case CertificateSet.ControlPlane:
                return this.AllowListedControlPlaneSubjectNames;
            default:
                throw new ServiceError(
                        ErrorCategory.ServiceError,
                        ErrorCode.Invalid_CertificateSet,
                        "Invalid certificate set value.")
                    .ToException();
        }
    }
}
