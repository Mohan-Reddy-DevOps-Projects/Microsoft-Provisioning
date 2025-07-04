﻿// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.Configurations;
/// <summary>
/// Configurations related to metadata service
/// </summary>
public class MetadataServiceConfiguration : BaseCertificateConfiguration
{
    /// <summary>
    /// Api version to use on the endpoint
    /// </summary>
    public string ApiVersion { get; set; }

    /// <summary>
    /// Metadata service endpoint
    /// </summary>
    public string Endpoint { get; set; }
}
