// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.DataAccess.EntityModel;

using global::Microsoft.Purview.DataGovernance.Provisioning.Models;

internal class CatalogConfigStorageEntity : TableEntity
{
    public string Features { get; set; }

    public string Sku { get; set; }

    public Guid TenantId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public override string ResourceId() => ResourceId(ResourceIds.CatalogConfig, [this.RowKey.ToString()]);
}
