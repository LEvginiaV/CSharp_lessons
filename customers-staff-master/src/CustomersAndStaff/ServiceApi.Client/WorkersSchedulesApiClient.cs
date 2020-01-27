using System;
using System.Threading.Tasks;

using Kontur.Clusterclient.Core.Model;

using Market.CustomersAndStaff.Models.Calendar;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.ServiceApi.Client.Core;
using Market.CustomersAndStaff.ServiceApi.Client.Extensions;

namespace Market.CustomersAndStaff.ServiceApi.Client
{
    public class WorkersSchedulesApiClient : IWorkersSchedulesApiClient
    {
        public WorkersSchedulesApiClient(ISimpleClusterClient clusterClient)
        {
            this.clusterClient = clusterClient;
        }

        public async Task<ShopCalendarRange<WorkerScheduleRecord>> Get(Guid shopId, DateTime from, DateTime to)
        {
            var result = await clusterClient.SendAsync(
                             Request.Get(Url(shopId, c => c.AppendDateToPath(from).AppendDateToPath(to)))
                         );
            return result.DeserializeDto<ShopCalendarRange<WorkerScheduleRecord>>();
        }

        private static Uri Url(Guid shopId, Func<RequestUrlBuilder, RequestUrlBuilder> conf)
        {
            return conf(new RequestUrlBuilder()
                        .AppendToPath(shopId)
                        .AppendToPath("workersSchedules")).Build();
        }

        private readonly ISimpleClusterClient clusterClient;
    }
}