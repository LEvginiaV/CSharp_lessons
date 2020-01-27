namespace Market.CustomersAndStaff.ServiceApi.Client
{
    public interface IServiceApiClient
    {
        ICustomerApiClient Customers { get; }
        IWorkerApiClient Workers { get; }
        IWorkersSchedulesApiClient WorkersSchedules { get; }
        IShopTimeZoneApiClient ShopTimeZone { get; }
        IUserSettingsApiClient UserSettingsApiClient { get; }
        IServiceCalendarApiClient ServiceCalendar { get; }
    }
}