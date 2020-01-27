using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Market.CustomersAndStaff.Repositories.Interface
{
    public interface IUserSettingsRepository
    {
        Task UpdateAsync(Guid userId, string key, string value);
        Task<Dictionary<string, string>> ReadAsync(Guid userId);
    }
}