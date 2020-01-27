namespace Market.CustomersAndStaff.Repositories.Configuration
{
    public interface ICassandraSettings
    {
        string Keyspace { get; }
        string LockKeyspace { get; }
        int LockTtlSeconds { get; }
    }
}