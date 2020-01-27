using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Repositories.StoredModels;

using SKBKontur.Catalogue.CassandraUtils.Cassandra.SessionTableQueryExtending.PrimitiveStoring;

namespace Market.CustomersAndStaff.Repositories.Implementation
{
    public class UserSettingsRepository : IUserSettingsRepository
    {
        public UserSettingsRepository(CassandraStorage<UserSettingsStorageElement> cassandraStorage)
        {
            this.cassandraStorage = cassandraStorage;
        }

        public async Task UpdateAsync(Guid userId, string key, string value)
        {
            await cassandraStorage.WriteAsync(new UserSettingsStorageElement
                {
                    UserId = userId,
                    Key = key,
                    Value = value,
                });
        }
        
        public async Task<Dictionary<string, string>> ReadAsync(Guid userId)
        {
            return (await cassandraStorage.WhereAsync(x => x.UserId == userId)).ToDictionary(x => x.Key, x => x.Value);
        }

        private readonly CassandraStorage<UserSettingsStorageElement> cassandraStorage;
    }
}