// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.DataAccess;

using Microsoft.Purview.DataGovernance.Provisioning.Configurations;
using Microsoft.Purview.DataGovernance.Provisioning.DataAccess.EntityModel;
using Microsoft.Purview.DataGovernance.Provisioning.Models;
using Microsoft.Extensions.Options;

internal sealed class ProcessingStorageRepository : StorageAccountRepository<ProcessingStorageModel, ProcessingStorageEntity>, IStorageAccountRepository<ProcessingStorageModel>
{
    private static readonly ProcessingStorageEntityAdapter converter = new();

    public ProcessingStorageRepository(
        ITableStorageClient<AccountStorageTableConfiguration> tableStorageClient,
        IOptions<AccountStorageTableConfiguration> tableConfiguration) : base(tableStorageClient, converter, tableConfiguration)
    {
    }
}
