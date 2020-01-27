using System.Linq;

using Cassandra.Mapping;

using GroboContainer.Core;

using Market.CustomersAndStaff.Repositories.Configuration;

using SKBKontur.Catalogue.CassandraUtils.Cassandra.Commons.Mapping;
using SKBKontur.Catalogue.CassandraUtils.Cassandra.Commons.SessionsManager;
using SKBKontur.Catalogue.CassandraUtils.DistributedLock;

using ICassandraSchemaActualizer = SKBKontur.Catalogue.CassandraUtils.Cassandra.Commons.Actualization.CassandraSchemaActualizer;

namespace Market.CustomersAndStaff.Repositories
{
    public class CustomerAndStaffCassandraSchemaActualizer
    {
        public CustomerAndStaffCassandraSchemaActualizer(
            IContainer container,
            ICassandraSchemaActualizer cassandraSchemaActualizer,
            DistributedLockSchemaActualizer distributedLockSchemaActualizer,
            ICassandraSettings cassandraSettings)
        {
            this.distributedLockSchemaActualizer = distributedLockSchemaActualizer;
            this.cassandraSettings = cassandraSettings;
            this.cassandraSchemaActualizer = cassandraSchemaActualizer;
            this.container = container;
        }

        public void Actualize()
        {
            cassandraSchemaActualizer.Actualize(CreateMaps(container), false);
            distributedLockSchemaActualizer.CreateSchemaIfNotExist(new[] {cassandraSettings.LockKeyspace});
        }

        public static void ConfigureMappings(IContainer container)
        {
            CassandraSessionsManager.ConfigureMappings(CreateMaps(container).Select(x => x.Mappings).ToArray());
            CassandraSessionsManager.ConfigureMappings(container.GetAll<Mappings>());
        }

        private static ICassandraMap[] CreateMaps(IContainer container)
        {
            var cassandraMapsFromMapCreators = container.GetAll<IMapCreator>().Select(x => x.CreateMap()).Where(x => x != null).ToArray();
            var cassandraMapsFromAttributes = container.Get<IMappingsRetriever>().GetAttributeMappings();
            return cassandraMapsFromMapCreators.Concat(cassandraMapsFromAttributes).ToArray();
        }

        private readonly ICassandraSchemaActualizer cassandraSchemaActualizer;
        private readonly IContainer container;
        private readonly DistributedLockSchemaActualizer distributedLockSchemaActualizer;
        private readonly ICassandraSettings cassandraSettings;
    }
}