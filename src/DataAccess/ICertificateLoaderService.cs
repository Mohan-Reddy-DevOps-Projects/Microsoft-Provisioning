// -----------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------

namespace Microsoft.Purview.DataGovernance.Provisioning.DataAccess;

using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

public interface ICertificateLoaderService
{
    Task BindAsync(HttpMessageHandler httpMessageHandler, string certName, CancellationToken cancellationToken);
    void Dispose();
    Task InitializeAsync();
    Task<X509Certificate2> LoadAsync(string secretName, CancellationToken cancellationToken);
}