using Market.CustomersAndStaff.ServiceApi.Client.Core;

namespace Market.CustomersAndStaff.ServiceApi.Client
{
    public class ServiceApiClient : IServiceApiClient
    {
        public ServiceApiClient(ServiceApiClientSettings settings)
        {
            var simpleClusterClient = !string.IsNullOrWhiteSpace(settings.ServiceName) 
                              ? new SimpleClusterClient(settings.ServiceName, settings.Timeout, settings.Log, settings.Zone) 
                              : new SimpleClusterClient(settings.Replicas, settings.Timeout, settings.Log);
            
            Customers = new CustomerApiClient(simpleClusterClient);
            Workers = new WorkerApiClient(simpleClusterClient);
            WorkersSchedules = new WorkersSchedulesApiClient(simpleClusterClient);
            ShopTimeZone = new ShopTimeZoneApiClient(simpleClusterClient);
            UserSettingsApiClient = new UserSettingsApiClient(simpleClusterClient);
            ServiceCalendar = new ServiceCalendarApiClient(simpleClusterClient);
        }

        public ICustomerApiClient Customers { get; }
        public IWorkerApiClient Workers { get; }
        public IWorkersSchedulesApiClient WorkersSchedules { get; }
        public IShopTimeZoneApiClient ShopTimeZone { get; }
        public IUserSettingsApiClient UserSettingsApiClient { get; }
        public IServiceCalendarApiClient ServiceCalendar { get; }
    }
}