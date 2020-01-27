using System;

namespace Market.CustomersAndStaff.EvrikaPrintClient.Client
{
    public interface IEvrikaPrinterClientSettings
    {
        Uri Host { get; }
        int RepeatReplicasCount { get; }
        TimeSpan Timeout { get; }
        Uri[] TestAuthUrls { get; }
        string PortalApiKey { get; }
    }
}