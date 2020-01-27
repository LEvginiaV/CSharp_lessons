using System;

using Cassandra;
using Cassandra.Mapping.Attributes;

using SKBKontur.Catalogue.CassandraUtils.Cassandra.Commons.Mapping.Declarative.AttributesDefinition;

namespace Market.CustomersAndStaff.Repositories.StoredModels
{
    [CassandraTable(Keyspace = RepositoryConstants.KeyspaceName, Table = RepositoryConstants.ServiceCalendar)]
    [CassandraTable(Keyspace = RepositoryConstants.KeyspaceName, Table = RepositoryConstants.ServiceCalendarRemoved)]
    [CassandraTable(Keyspace = RepositoryConstants.KeyspaceName, Table = RepositoryConstants.WorkerSchedule)]
    public class CalendarDayStorageElement
    {
        [PartitionKey(0)]
        public Guid ShopId { get; set; }
        [PartitionKey(1)]
        public LocalDate Month { get; set; }
        [ClusteringKey(0)]
        public LocalDate Date { get; set; }
        [ClusteringKey(1)]
        public Guid WorkerId { get; set; }
        [Column]
        public byte[] Records { get; set; }
    }
}