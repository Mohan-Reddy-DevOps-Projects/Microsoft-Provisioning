﻿// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.Configurations;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

/// <summary>
///Defines certificate configuration set to use
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum CertificateSet
{
    /// <summary>
    /// Data Plane
    /// </summary>
    DataPlane = 1,

    /// <summary>
    /// Control Plane
    /// </summary>
    ControlPlane,
}
