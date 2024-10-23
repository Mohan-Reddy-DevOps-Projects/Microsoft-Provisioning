// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.ApiService.Controllers.DataManageConfig;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Purview.DataGovernance.Common;
using Microsoft.Purview.DataGovernance.Loggers;
using Microsoft.Purview.DataGovernance.Provisioning.Core;
using Microsoft.Purview.DataGovernance.Provisioning.DataAccess;

[ApiController]
[Route("/config/")]
public class CatalogConfigControlller : DataPlaneController
{
    private readonly ICatalogConfigService catalogService;

    private readonly IRequestHeaderContext requestHeaderContext;

    private readonly IServiceRequestLogger logger;

    private IAccountExposureControlConfigProvider exposureControl;

    private readonly CatalogConfigAdapter catalogConfigAdapter = new CatalogConfigAdapter();

    public CatalogConfigControlller(
        ICatalogConfigService catalogService,
        IRequestHeaderContext requestHeaderContext,
        IServiceRequestLogger logger,
        IAccountExposureControlConfigProvider exposureControl
    )
    {
        this.catalogService = catalogService;
        this.requestHeaderContext = requestHeaderContext;
        this.logger = logger;   
        this.exposureControl = exposureControl;
    }

    [HttpGet]
    public async Task<ActionResult<CatalogConfig>> GetCatalogConfig(
        CancellationToken cancellationToken)
    {
        var accountId = this.requestHeaderContext.AccountObjectId.ToString();

        var catalogConfigModel = await this.catalogService.GetCatalogConfigAsync(accountId, cancellationToken);
                
        this.logger.LogInformation($"GetCatalogConfig:{catalogConfigModel} ");
        
        var catalogConfig = this.catalogConfigAdapter.FromModel(catalogConfigModel);

        return this.Ok(catalogConfig);
    }

    [HttpPut]
    public async Task<ActionResult<CatalogConfig>> UpdateCatalogConfig(
        [FromBody] CatalogConfig catalogConfigPayload,
        CancellationToken cancellationToken)
    {
        var accountId = this.requestHeaderContext.AccountObjectId.ToString();

        var updatedcatalogConfig = await this.catalogService.SetCatalogConfigAsync(this.requestHeaderContext.AccountObjectId.ToString(), this.catalogConfigAdapter.ToModel(catalogConfigPayload), cancellationToken);

        this.logger.LogInformation($"UpdateCatalogConfig:{updatedcatalogConfig} ");
        
        var catalogConfig = this.catalogConfigAdapter.FromModel(updatedcatalogConfig);

        return this.Ok(catalogConfig);
    }
}
