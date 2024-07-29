// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.ApiService.Adapters;

using System.ComponentModel;
using Microsoft.Purview.DataGovernance.Provisioning.Models;

/// <summary>
/// Adapter to convert CatalogSkuMode between model and DTO.
/// </summary>
internal static class CatalogSkuModeAdapter
{
    /// <summary>
    /// Convert to DTO.
    /// </summary>
    /// <param name="model">Input model.</param>
    /// <returns></returns>
    public static DataTransferObjects.CatalogSkuMode ToDto(this CatalogSkuMode model)
    {
        switch (model)
        {
            case CatalogSkuMode.On:
                return DataTransferObjects.CatalogSkuMode.On;
            case CatalogSkuMode.Off:
                return DataTransferObjects.CatalogSkuMode.Off;
            default:
                throw new InvalidEnumArgumentException(nameof(model), (int)model, typeof(CatalogSkuMode));
        }
    }

    /// <summary>
    /// Convert to model.
    /// </summary>
    /// <param name="dto">Input DTO.</param>
    /// <returns></returns>
    public static CatalogSkuMode ToModel(this DataTransferObjects.CatalogSkuMode dto)
    {
        switch (dto)
        {
            case DataTransferObjects.CatalogSkuMode.On:
                return CatalogSkuMode.On;
            case DataTransferObjects.CatalogSkuMode.Off:
                return CatalogSkuMode.Off;
            default:
                throw new InvalidEnumArgumentException(
                    nameof(dto),
                    (int)dto,
                    typeof(DataTransferObjects.CatalogSkuMode));
        }
    }
}
