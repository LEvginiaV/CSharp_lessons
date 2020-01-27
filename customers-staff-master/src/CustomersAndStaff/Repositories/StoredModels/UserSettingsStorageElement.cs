using System;

using Cassandra.Mapping.Attributes;

using SKBKontur.Catalogue.CassandraUtils.Cassandra.Commons.Mapping.Declarative.AttributesDefinition;

namespace Market.CustomersAndStaff.Repositories.StoredModels
{
    [CassandraTable(Keyspace = RepositoryConstants.KeyspaceName, Table = "user_settings")]
    public class UserSettingsStorageElement
    {
        [PartitionKey]
        public Guid UserId { get; set; }
        [ClusteringKey]
        public string Key { get; set; }
        [Column]
        public string Value { get; set; }
    }
}