// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.DataAccess;

using System.Net.Http;
using System.Threading.Tasks;

/// <summary>
/// Interface to create handler for babylon metadata client.
/// </summary>
internal interface ICertificateHandler
{
    /// <summary>
    /// Method to create http handler for babylon metadata client.
    /// </summary>
    /// <returns></returns>
    Task<SocketsHttpHandler> CreateHandlerAsync();
}
