using Cassandra.Mapping.Attributes;

using SKBKontur.Catalogue.CassandraUtils.Cassandra.Commons.Mapping.Declarative.AttributesDefinition;

namespace Market.CustomersAndStaff.Repositories.StoredModels
{
    [CassandraTable(Keyspace = RepositoryConstants.KeyspaceName, Table = "counters")]
    public class CounterStorageElement
    {
        [PartitionKey]
        public string Key { get; set; }

        [Column]
        public int Counter { get; set; }
    }
}