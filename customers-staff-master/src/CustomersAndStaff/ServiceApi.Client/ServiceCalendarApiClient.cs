using System;
using System.Threading.Tasks;

using Kontur.Clusterclient.Core.Model;

using Market.CustomersAndStaff.Models.Calendar;
using Market.CustomersAndStaff.Models.ServiceCalendar;
using Market.CustomersAndStaff.ServiceApi.Client.Core;

namespace Market.CustomersAndStaff.ServiceApi.Client
{
    public class ServiceCalendarApiClient : IServiceCalendarApiClient
    {
        private readonly ISimpleClusterClient clusterClient;

        public ServiceCalendarApiClient(ISimpleClusterClient clusterClient)
        {
            this.clusterClient = clusterClient;
        }

        public async Task<Guid> CreateRecord(Guid shopId, DateTime date, Guid workerId, ServiceCalendarRecord record)
        {
            var request = Request.Post(Url(shopId, date, c => c.AppendToPath("workers").AppendToPath(workerId).AppendToPath("records")))
                                     .WithContentTypeApplicationJsonHeader()
                                     .WithContent(record.SerializeDto());
            var result = await clusterClient.SendAsync(
                             request
                         );
            return result.DeserializeDto<Guid>();
        }

        public async Task<ShopCalendarDay<ServiceCalendarRecord>> GetShopDay(Guid shopId, DateTime date)
        {
            var result = await clusterClient.SendAsync(Request.Get(Url(shopId, date, c => c)));
            return result.DeserializeDto<ShopCalendarDay<ServiceCalendarRecord>>();
        }
        
        private Uri Url(Guid shopId, DateTime date, Func<RequestUrlBuilder, RequestUrlBuilder> conf)
        {
            return conf(
                    new RequestUrlBuilder()
                        .AppendToPath("shops")
                        .AppendToPath(shopId)
                        .AppendToPath("calendar")
                        .AppendToPath(date.ToString("yyyy-MM-dd"))
                )
                .Build();
        }
    }
}
