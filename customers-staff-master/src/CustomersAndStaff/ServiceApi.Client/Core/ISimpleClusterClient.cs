using System;
using System.Threading;
using System.Threading.Tasks;

using Kontur.Clusterclient.Core.Model;

namespace Market.CustomersAndStaff.ServiceApi.Client.Core
{
    public interface ISimpleClusterClient
    {
        Task<ClusterResult> SendAsync(Request request, CancellationToken? cancellationToken = null, TimeSpan? timeout = null);
    }
}