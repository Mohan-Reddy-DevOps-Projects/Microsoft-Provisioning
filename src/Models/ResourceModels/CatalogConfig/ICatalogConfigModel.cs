// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.Models;

using System;

public interface ICatalogConfigModel
{
    /// <summary>
    /// Id of the catalog configuration
    /// </summary>
    /// <remarks>
    /// This is a unique identifier for the catalog configuration. It can't be changed after creation
    /// </remarks>
    Guid Id { get; init; }

    /// <summary>
    /// Tenant id of the catalog configuration
    /// </summary>
    Guid TenantId { get; set; }

    /// <summary>
    /// Sku of the catalog configuration
    /// </summary>
    CatalogSkuName Sku { get; set; }

    /// <summary>
    /// Features of the catalog configuration
    /// </summary>
    CatalogFeaturesModel Features { get; set; }

    /// <summary>
    /// Date/Time this entity was created.
    /// </summary>
    DateTime CreatedAt { get; init; }

    /// <summary>
    /// Date/Time this entity was modified.
    /// </summary>
    DateTime ModifiedAt { get; set; }
}
