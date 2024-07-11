// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------
namespace Microsoft.Purview.DataGovernance.Provisioning.DataAccess;

using Microsoft.Purview.DataGovernance.Provisioning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ICatalogConfigRepository
{
    /// <summary>
    /// Creates config
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CatalogConfigModel> Create(string accountId, CatalogConfigModel model, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a single entity.
    /// </summary>
    /// <param name="accountId">The id used to retrieve the entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that resolves to the entity information.</returns>
    Task<CatalogConfigModel> GetSingle(string accountId, CancellationToken cancellationToken);

    /// <summary>
    /// Updates config
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CatalogConfigModel> Update(string accountId, CatalogConfigModel model, CancellationToken cancellationToken);

    /// <summary>
    /// Deltes config
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task Delete(string accountId, CancellationToken cancellationToken);
}
