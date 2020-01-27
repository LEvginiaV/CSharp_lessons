using SKBKontur.Catalogue.CassandraUtils.Cassandra.Commons.Mapping.Declarative.ExtendedDefinition;
using SKBKontur.Catalogue.CassandraUtils.Cassandra.SessionTableQueryExtending.TimestampStoring;

namespace Market.CustomersAndStaff.Repositories.Configuration
{
    public class LogsTimestampCellCoordinates : ExtendedTableDefinition<TimestampCell>
    {
        public override string Keyspace => RepositoryConstants.KeyspaceName;
        public override string Table => "timestamp";
    }
}