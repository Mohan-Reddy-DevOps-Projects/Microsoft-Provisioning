// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.ApiService.Adapters;

using System.ComponentModel;
using Microsoft.Purview.DataGovernance.Provisioning.Models;

/// <summary>
/// Adapter to convert CatalogSkuName between model and DTO.
/// </summary>
internal static class CatalogSkuNameAdapter
{
    /// <summary>
    /// Convert to DTO.
    /// </summary>
    /// <param name="model">Input model.</param>
    /// <returns></returns>
    public static DataTransferObjects.CatalogSkuName ToDto(this CatalogSkuName model)
    {
        switch (model)
        {
            case CatalogSkuName.Advanced:
                return DataTransferObjects.CatalogSkuName.Advanced;
            case CatalogSkuName.Basic:
                return DataTransferObjects.CatalogSkuName.Basic;
            case CatalogSkuName.Standard:
                return DataTransferObjects.CatalogSkuName.Standard;
            default:
                throw new InvalidEnumArgumentException(nameof(model), (int)model, typeof(CatalogSkuName));
        }
    }

    /// <summary>
    /// Convert to model.
    /// </summary>
    /// <param name="dto">Input DTO.</param>
    /// <returns></returns>
    public static CatalogSkuName ToModel(this DataTransferObjects.CatalogSkuName dto)
    {
        switch (dto)
        {
            case DataTransferObjects.CatalogSkuName.Advanced:
                return CatalogSkuName.Advanced;
            case DataTransferObjects.CatalogSkuName.Basic:
                return CatalogSkuName.Basic;
            case DataTransferObjects.CatalogSkuName.Standard:
                return CatalogSkuName.Standard;
            default:
                throw new InvalidEnumArgumentException(
                    nameof(dto),
                    (int)dto,
                    typeof(DataTransferObjects.CatalogSkuName));
        }
    }
}
