using System;
using System.Linq;

using GroboContainer.Core;

using Kontur.Configuration.Topology;
using Kontur.Logging;

using Portal.Auth;
using Portal.Common;
using Portal.Requisites;

namespace Market.CustomersAndStaff.Portal.Core
{
    public static class ContainerExtensions
    {
        public static void ConfigurePortal(this IContainer container)
        {
            var portalSettings = container.Get<IPortalSettings>();
            var provider = new ApiKeyAuthenticationProvider(portalSettings.ApiKey);

            SetPortalTopology("topology/api.sessions", new FakeLog(), portalSettings.AuthUrls);

            var authClient = new AuthClient(new ClientSettings(provider, new FakeLog(), TimeSpan.FromSeconds(15), false), new TopologyProvider(portalSettings.AuthUrls));
            var requisitesClient = new RequisitesClient(new ClientSettings(provider, new FakeLog(), TimeSpan.FromSeconds(15), false), new TopologyProvider(portalSettings.RequisitesUrls));
            container.Configurator.ForAbstraction<IRequisitesClient>().UseInstances(requisitesClient);
            container.Configurator.ForAbstraction<IAuthClient>().UseInstances(authClient);
        }

        private static void SetPortalTopology(string topologyName, ILog logger, Uri[] urls)
        {
            ClusterConfigTopologyResolver.SetManually(topologyName, logger, new Cluster(urls.Select(x => new Replica(x)).Cast<IReplica>().ToList()));
        }
    }
}