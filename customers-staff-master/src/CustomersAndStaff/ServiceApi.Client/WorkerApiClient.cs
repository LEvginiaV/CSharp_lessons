using System;
using System.Threading.Tasks;

using Kontur.Clusterclient.Core.Model;

using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.ServiceApi.Client.Core;

namespace Market.CustomersAndStaff.ServiceApi.Client
{
    public class WorkerApiClient : IWorkerApiClient
    {
        public WorkerApiClient(ISimpleClusterClient clusterClient)
        {
            this.clusterClient = clusterClient;
        }

        public async Task<Worker> CreateAsync(Guid shopId, Worker worker)
        {
            var result = await clusterClient.SendAsync(
                             Request.Post(Url(shopId, c => c))
                                    .WithContentTypeApplicationJsonHeader()
                                    .WithContent(worker.SerializeDto())
                         );
            return result.DeserializeDto<Worker>();
        }

        public async Task<Worker[]> CreateManyAsync(Guid shopId, Worker[] workers)
        {
            var result = await clusterClient.SendAsync(
                             Request.Post(Url(shopId, c => c.AppendToPath("batch")))
                                    .WithContentTypeApplicationJsonHeader()
                                    .WithContent(workers.SerializeDto())
                         );
            return result.DeserializeDto<Worker[]>();
        }

        public async Task UpdateAsync(Guid shopId, Guid workerId, Worker worker)
        {
            var result = await clusterClient.SendAsync(
                             Request.Post(Url(shopId, c => c.AppendToPath(workerId)))
                                    .WithContentTypeApplicationJsonHeader()
                                    .WithContent(worker.SerializeDto())
                         );
            result.CheckResponseSuccessful();
        }

        public async Task<Worker> ReadAsync(Guid shopId, Guid workerId)
        {
            var result = await clusterClient.SendAsync(
                             Request.Get(Url(shopId, c => c.AppendToPath(workerId)))
                         );
            return result.DeserializeDto<Worker>();
        }

        public async Task<Worker[]> ReadByOrganizationAsync(Guid shopId, bool includeDeleted = false)
        {
            var result = await clusterClient.SendAsync(
                             Request.Get(Url(shopId, c => c.AppendToQuery("includeDeleted", includeDeleted)))
                         );
            return result.DeserializeDto<Worker[]>();
        }

        public async Task<int> GetVersionAsync(Guid shopId)
        {
            var result = await clusterClient.SendAsync(
                             Request.Get(Url(shopId, c => c.AppendToPath("version")))
                         );
            return result.DeserializeDto<int>();
        }

        private Uri Url(Guid shopId, Func<RequestUrlBuilder, RequestUrlBuilder> conf)
        {
            return conf(
                    new RequestUrlBuilder()
                        .AppendToPath(shopId)
                        .AppendToPath("worker")
                )
                .Build();
        }

        private readonly ISimpleClusterClient clusterClient;
    }
}