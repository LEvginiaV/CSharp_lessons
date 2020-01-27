using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Kontur.Clusterclient.Core.Model;

using Market.CustomersAndStaff.ServiceApi.Client.Core;

namespace Market.CustomersAndStaff.ServiceApi.Client
{
    public class UserSettingsApiClient : IUserSettingsApiClient
    {
        public UserSettingsApiClient(ISimpleClusterClient clusterClient)
        {
            this.clusterClient = clusterClient;
        }

        public async Task<Dictionary<string, string>> ReadAsync(Guid userId)
        {
            var result = await clusterClient.SendAsync(Request.Get(Url(userId)));
            return result.DeserializeDto<Dictionary<string, string>>();
        }

        public async Task UpdateAsync(Guid userId, string key, string value)
        {
            var result = await clusterClient.SendAsync(Request.Put(Url(userId, c => c.AppendToPath(key).AppendToQuery("value", value)))); 
            result.CheckResponseSuccessful();
        }

        private Uri Url(Guid userId) => Url(userId, c => c);

        private Uri Url(Guid userId, Func<RequestUrlBuilder, RequestUrlBuilder> conf)
        {
            return conf(
                    new RequestUrlBuilder()
                        .AppendToPath("users")
                        .AppendToPath(userId)
                        .AppendToPath("settings")
                )
                .Build();
        }

        private readonly ISimpleClusterClient clusterClient;
    }
}