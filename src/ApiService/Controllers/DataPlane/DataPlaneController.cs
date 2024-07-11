// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.ApiService;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Purview.DataGovernance.Provisioning.Configurations;

/// <summary>
/// Base controller for data plane certificate authentication.
/// </summary>
[CertificateConfig(CertificateSet.DataPlane)]
[Authorize(AuthenticationSchemes = "Certificate")]
public abstract class DataPlaneController : Controller
{
}
