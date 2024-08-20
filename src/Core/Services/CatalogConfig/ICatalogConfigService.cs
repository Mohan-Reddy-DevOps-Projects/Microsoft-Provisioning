// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.Core;

using Microsoft.Purview.DataGovernance.Provisioning.Models;
using System.Threading.Tasks;

public interface ICatalogConfigService
{
    /// <summary>
    /// Create catalog config
    /// </summary>
    Task<CatalogConfigModel> CreateCatalogConfigAsync(string accountId, string tenantId, CancellationToken cancellationToken);

   /// <summary>
   /// Delete catalog config
   /// </summary>
   /// <param name="accountId"></param>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
    Task DeleteCatalogConfigAsync(string accountId, CancellationToken cancellationToken);

    /// <summary>
    /// Get catalog config
    /// </summary>
    /// <returns>CatalogConfigPayload</returns>
    Task<CatalogConfigModel> GetCatalogConfigAsync(string accountId, CancellationToken cancellationToken);

    /// <summary>
    /// Set catalog config
    /// </summary>
    /// <param name="catalogConfig">CatalogConfigModel</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>CatalogConfigPayload</returns>
    Task<CatalogConfigModel> SetCatalogConfigAsync(string accountId, CatalogConfigModel catalogConfig, CancellationToken cancellationToken);
}
