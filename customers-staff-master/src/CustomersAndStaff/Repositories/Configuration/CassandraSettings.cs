using Alko.Configuration.Settings;

namespace Market.CustomersAndStaff.Repositories.Configuration
{
    public class CassandraSettings : ICassandraSettings
    {
        public CassandraSettings(IApplicationSettings applicationSettings)
        {
            Keyspace = applicationSettings.GetString("CustomersAndStaff.Cassandra.Keyspace");
            LockKeyspace = applicationSettings.GetString("CustomersAndStaff.Cassandra.LockKeyspace");
            LockTtlSeconds = applicationSettings.GetInt("CustomersAndStaff.Cassandra.LockTtlSeconds");
        }

        public string Keyspace { get; }
        public string LockKeyspace { get; }
        public int LockTtlSeconds { get; }
    }
}