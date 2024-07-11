// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.Models;

public interface ICatalogFeatureSettingsModel
{
    /// <summary>
    /// mode of the catalog feature settings.
    /// </summary>
    CatalogSkuMode Mode { get; set; }
}