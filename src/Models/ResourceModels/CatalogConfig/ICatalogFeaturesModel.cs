// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.Models;

public interface ICatalogFeaturesModel
{
    /// <summary>
    /// Data estate health feature settings.
    /// </summary>
    CatalogFeatureSettingsModel DataEstateHealth { get; set; }

    /// <summary>
    /// Data quality feature settings.
    /// </summary>
    CatalogFeatureSettingsModel DataQuality { get; set; }
}