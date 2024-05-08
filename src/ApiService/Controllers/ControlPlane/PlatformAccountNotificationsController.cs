// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.ApiService;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.Azure.ProjectBabylon.Metadata.Models;
using System.Threading;
using Microsoft.Extensions.Options;
using Microsoft.Purview.DataGovernance.Provisioning.Configurations;
using Microsoft.Purview.DataGovernance.Provisioning.ProvisioningService;
using Microsoft.Purview.DataGovernance.Provisioning.ProvisioningService.Configurations;
using OperationType = DataTransferObjects.OperationType;
using System.Collections.Concurrent;
using Microsoft.Purview.DataGovernance.Provisioning.DataAccess;
using Microsoft.Purview.DataGovernance.Provisioning.Models;
using Microsoft.Purview.DataGovernance.Loggers;
using Microsoft.Purview.DataGovernance.EventHub;
using Microsoft.Purview.DataGovernance.EventHub.Models.Events;
using System.Globalization;
using Microsoft.Purview.DataGovernance.Common;
using Microsoft.Purview.DataGovernance.Provisioning.Common;

/// <summary>
/// Purview account notifications controller.
/// </summary>
[ApiController]
[Route("/controlplane/account/")]
[ApiVersionNeutral]
public class PlatformAccountNotificationsController : ControlPlaneController
{
    private readonly IServiceRequestLogger logger;
    private readonly IRequestContextAccessor requestContextAccessor;
    private readonly IPartnerService<AccountServiceModel, IPartnerDetails> partnerService;
    private readonly PartnerConfig<IPartnerDetails> partnerConfig;
    private readonly IAccountExposureControlConfigProvider exposureControl;
    private readonly IProcessingStorageManager processingStorageManager;
    private readonly IEventPublisher eventPublisher;

    /// <summary>
    /// Instantiate instance of PlatformAccountNotificationsController.
    /// </summary>
    public PlatformAccountNotificationsController(
        IPartnerService<AccountServiceModel, IPartnerDetails> partnerService,
        IAccountExposureControlConfigProvider exposureControl,
        IOptions<PartnerConfiguration> partnerConfiguration,
        IServiceRequestLogger logger,
        IRequestContextAccessor requestContextAccessor,
        IProcessingStorageManager processingStorageManager,
        IEventPublisher eventPublisher)
    {
        this.logger = logger;
        this.requestContextAccessor = requestContextAccessor;
        this.partnerService = partnerService;
        this.exposureControl = exposureControl;
        this.partnerConfig = new(partnerConfiguration);
        this.processingStorageManager = processingStorageManager;
        this.eventPublisher = eventPublisher;
    }

    /// <summary>
    /// Create or update account notification.
    /// </summary>
    /// <param name="account">The model of the platform account</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> CreateOrUpdateNotificationAsync(
        [FromBody] AccountServiceModel account,
        CancellationToken cancellationToken)
    {
        this.logger.LogInformation($"CreateOrUpdateNotificationAsync: ProvisioningState: {account.ProvisioningState}; Sku: {account.Sku?.Name}; Reconciled: {account.ReconciliationConfig?.ReconciliationStatus}");

        if (account.ProvisioningState != ProvisioningState.Creating || account.IsFreeTier() || !account.IsReconciled())
        {
            return this.Ok();
        }

        if (!this.exposureControl.IsDataGovProvisioningEnabled(account.Id, account.SubscriptionId, account.TenantId))
        {
            return this.Ok();
        }

        string responseStatus = await this.processingStorageManager.Provision(account, cancellationToken);
        Task partnerTask = PartnerNotifier.NotifyPartners(
                this.logger,
                this.partnerService,
                this.partnerConfig,
                account,
                ProvisioningService.OperationType.CreateOrUpdate,
                InitPartnerContext(this.partnerConfig.Partners));

        List<Task> tasks = new()
        {
            partnerTask
        };

        await Task.WhenAll(tasks);

        this.logger.LogInformation($"CreateOrUpdateNotificationAsync: ResponseCode: {responseStatus}");

        return responseStatus.Equals(ResponseStatus.Created) ? this.Created() : this.Ok();
    }

    /// <summary>
    /// Deletes the account dependencies.
    /// </summary>
    /// <param name="accountId">The accountId of the Service Account</param>
    /// <param name="operation">The type of operation to perform on the account.</param>
    /// <param name="account">The account model.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete]
    [ProducesResponseType(typeof(AccountServiceModel), 200)]
    [ProducesResponseType(204)]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("{accountId}/")]
    public async Task<IActionResult> DeleteOrSoftDeleteNotificationAsync(
        [FromRoute] Guid accountId,
        [FromQuery(Name = "operation")] OperationType operation,
        [FromBody] AccountServiceModel account,
        CancellationToken cancellationToken)
    {
        this.logger.LogInformation($"DeleteOrSoftDeleteNotificationAsync: ProvisioningState: {account.ProvisioningState}; Sku: {account.Sku?.Name}; Reconciled: {account.ReconciliationConfig?.ReconciliationStatus}");
        bool isInvalidReconcileState = !(account.IsReconciled() || account.IsInactiveReconcile());
        if (operation == OperationType.SoftDelete || account.IsFreeTier() || isInvalidReconcileState)
        {
            return this.Ok();
        }

        if (!this.exposureControl.IsDataGovProvisioningEnabled(account.Id, account.SubscriptionId, account.TenantId))
        {
            return this.Ok();
        }

        DataChangeEventPayload<AccountServiceModel> payload = new()
        {
            After = account
        };
        DataChangeEvent<AccountServiceModel> deleteEvent = this.AccountDeleteEvent(account, payload);
        await this.eventPublisher.PublishChangeEvent(deleteEvent);

        DeletionResult deletionResult = await this.processingStorageManager.Delete(account, cancellationToken);

        await PartnerNotifier.NotifyPartners(
                this.logger,
                this.partnerService,
                this.partnerConfig,
                account,
                ProvisioningService.OperationType.Delete,
                InitPartnerContext(this.partnerConfig.Partners)).ConfigureAwait(false);

        this.logger.LogInformation($"DeleteOrSoftDeleteNotificationAsync: StatusCode: {deletionResult.DeletionStatus}");

        return deletionResult.DeletionStatus switch
        {
            DeletionStatus.ResourceNotFound => this.NoContent(),
            DeletionStatus.Deleted => this.Ok(),
            _ => this.Ok(),
        };
    }

    private DataChangeEvent<T> AccountDeleteEvent<T>(AccountServiceModel account, DataChangeEventPayload<T> payload) where T : class
    {
        IRequestHeaderContext requestContext = this.requestContextAccessor.GetRequestContext();

        return new DataChangeEvent<T>()
        {
            EventId = Guid.NewGuid().ToString(),
            CorrelationId = requestContext.CorrelationId,
            AccountId = account.Id,
            TenantId = account.TenantId,
            EventSource = "Provision",
            PreciseTimestamp = DateTime.UtcNow.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
            OperationType = EventHub.Models.Events.OperationType.Delete,
            PayloadKind = PayloadKind.ResourceProviderAccount,
            Payload = payload,
            ChangedBy = requestContext.ClientObjectId,
        };
    }

    /// <summary>
    /// Initializes the Partner Context for Callbacks.
    /// </summary>
    /// <returns>The PartnerContext.</returns>
    private static ConcurrentDictionary<string, PartnerOptions> InitPartnerContext(IPartnerDetails[] partners)
    {
        ConcurrentDictionary<string, PartnerOptions> partnerContext = new();
        foreach (IPartnerDetails partner in partners)
        {
            partnerContext.TryAdd(partner.Name.ToLowerInvariant(), new PartnerOptions() { HasSucceeded = false });
        }

        return partnerContext;
    }
}
