using Market.CustomersAndStaff.Repositories.Configuration;

using SKBKontur.Catalogue.CassandraUtils.Cassandra.Commons.Mapping;
using SKBKontur.Catalogue.CassandraUtils.Cassandra.Commons.Mapping.Declarative;

namespace Market.CustomersAndStaff.Repositories
{
    public class CassandraCoordinatesSubstitution : ICassandraCoordinatesSubstitution
    {
        public CassandraCoordinatesSubstitution(ICassandraSettings cassandraSettings)
        {
            this.cassandraSettings = cassandraSettings;
        }

        public KeyspaceTableNamePair Substitute(string keyspace, string table)
        {
            return new KeyspaceTableNamePair(SubstituteKeyspace(keyspace), SubstituteTable(table));
        }

        public string SubstituteKeyspace(string keyspace)
        {
            return keyspace == RepositoryConstants.KeyspaceName ? cassandraSettings.Keyspace : keyspace;
        }

        public string SubstituteTable(string table)
        {
            return table;
        }

        private readonly ICassandraSettings cassandraSettings;
    }
}