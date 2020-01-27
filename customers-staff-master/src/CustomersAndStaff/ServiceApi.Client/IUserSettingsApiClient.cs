using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Market.CustomersAndStaff.ServiceApi.Client
{
    public interface IUserSettingsApiClient
    {
        Task<Dictionary<string, string>> ReadAsync(Guid userId);
        Task UpdateAsync(Guid userId, string key, string value);
    }
}