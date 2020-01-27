using System;
using System.Linq;

using Kontur.Configuration.Topology;

namespace Market.CustomersAndStaff.FrontApi.Configuration
{
    public class TopologyProvider : ITopologyProvider
    {
        public TopologyProvider(Uri[] uris)
        {
            this.uris = uris;
        }

        public bool TryResolve(out Cluster cluster)
        {
            cluster = new Cluster(uris.Select(x => new Replica(x)).Cast<IReplica>().ToList());
            return true;
        }

        private readonly Uri[] uris;
    }
}