// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.ApiService;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Purview.DataGovernance.Provisioning.Configurations;

/// <summary>
/// </summary>
[CertificateConfig(CertificateSet.ControlPlane)]
[Authorize(AuthenticationSchemes = "Certificate")]
public abstract class ControlPlaneController : Controller
{
}
