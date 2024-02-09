// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.ApiService;

using Microsoft.Purview.DataGovernance.Provisioning.Configurations;

/// <summary>
/// Controller allows gateway certificates
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class CertificateConfigAttribute : Attribute
{
    /// <summary>
    /// Which certificate configuration set to use
    /// </summary>
    public CertificateSet CertificateSet { get; }

    /// <summary>
    /// </summary>
    /// <param name="certificateSet">Which certificate configuration set to use</param>
    public CertificateConfigAttribute(CertificateSet certificateSet)
    {
        this.CertificateSet = certificateSet;
    }
}
