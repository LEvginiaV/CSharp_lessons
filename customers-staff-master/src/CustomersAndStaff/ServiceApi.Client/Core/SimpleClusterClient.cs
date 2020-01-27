using System;
using System.Threading;
using System.Threading.Tasks;

using Kontur.Clusterclient.Core;
using Kontur.Clusterclient.Core.Model;
using Kontur.Clusterclient.Core.Ordering.Weighed;
using Kontur.Clusterclient.Core.Ordering.Weighed.Adaptive;
using Kontur.Clusterclient.Core.Strategies;
using Kontur.Clusterclient.Core.Strategies.DelayProviders;
using Kontur.Clusterclient.Transport.Webrequest;
using Kontur.Logging;
using Kontur.Singular.Client;
using Kontur.Utilities.Convertions.Time;

namespace Market.CustomersAndStaff.ServiceApi.Client.Core
{
    public class SimpleClusterClient : ISimpleClusterClient
    {
        public SimpleClusterClient(Uri[] addressList, TimeSpan? timeout = null, ILog log = null)
        {
            replicasCount = addressList.Length;
            globalTimeout = timeout ?? TimeSpan.FromSeconds(15);
            clusterClient = new ClusterClient(log, config =>
                {
                    config.ClusterProvider = new ConstantTopologyClusterProvider(addressList);
                    config.SetupWebRequestTransport();
                    config.SetupWeighedReplicaOrdering(builder => { builder.AddAdaptiveHealthModifierWithLinearDecay(TuningPolicies.ByResponseVerdict, 10.Minutes()); });
                });
            forkingRequestStrategy = new ForkingRequestStrategy(new EqualDelaysProvider(addressList.Length), addressList.Length);
            parallelRequestStrategy = new ParallelRequestStrategy(addressList.Length);
        }
        
        public SimpleClusterClient(string serviceName, TimeSpan timeout, ILog log, string zone = null)
        {
            globalTimeout = timeout;
            clusterClient = new SingularClient(log ?? new FakeLog(), serviceName, zone);
        }

        public async Task<ClusterResult> SendAsync(Request request, CancellationToken? cancellationToken = null, TimeSpan? timeout = null)
        {
            switch(request.Method)
            {
            case "GET":
                return await clusterClient
                             .SendAsync(request, timeout ?? globalTimeout, parallelRequestStrategy, cancellationToken ?? default(CancellationToken))
                             .ConfigureAwait(false);
            case "POST":
            case "DELETE":
            case "PUT":
                return await clusterClient
                             .SendAsync(request, TimeSpan.FromMilliseconds((timeout ?? globalTimeout).TotalMilliseconds * (replicasCount > 0 ? replicasCount : 1)), 
                                        forkingRequestStrategy, cancellationToken ?? default(CancellationToken))
                             .ConfigureAwait(false);
            default:
                throw new NotSupportedException($"Unsupporded method {request.Method}");
            }
        }

        private readonly IClusterClient clusterClient;
        private readonly ForkingRequestStrategy forkingRequestStrategy;
        private readonly ParallelRequestStrategy parallelRequestStrategy;
        private readonly TimeSpan globalTimeout;
        private readonly int replicasCount;
    }
}