﻿// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.Configurations;

using Microsoft.Purview.DataGovernance.Common;

/// <summary>
/// Configuration for processing storage account table
/// </summary>
public class AccountStorageTableConfiguration : StorageTableConfiguration
{
}

/// <summary>
/// Configuration for Data Governance Account storage table
/// </summary>
public abstract class StorageTableConfiguration : AuthConfiguration
{
    /// <summary>
    /// Table service uri
    /// </summary>
    public string TableServiceUri { get; set; }

    /// <summary>
    /// Table name
    /// </summary>
    public string TableName { get; set; }
}
