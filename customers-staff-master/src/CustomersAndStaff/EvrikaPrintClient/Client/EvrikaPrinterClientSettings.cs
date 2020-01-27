using System;
using System.Linq;

using Alko.Configuration.Settings;

namespace Market.CustomersAndStaff.EvrikaPrintClient.Client
{
    public class EvrikaPrinterClientSettings : IEvrikaPrinterClientSettings
    {
        public EvrikaPrinterClientSettings(IApplicationSettings applicationSettings)
        {
            this.applicationSettings = applicationSettings;
            PortalApiKey = applicationSettings.GetString("Printer.ApiKey");
        }

        public Uri Host => new Uri(applicationSettings.GetString("Printer.Host"));
        public int RepeatReplicasCount => applicationSettings.GetInt("Printer.RepeatReplicasCount");
        public TimeSpan Timeout => applicationSettings.GetTimeSpan("Printer.Timeout");
        public Uri[] TestAuthUrls => applicationSettings.GetStringArray("Printer.TestAuthUrls").Select(x => new Uri(x)).ToArray();
        public string PortalApiKey { get; }

        private readonly IApplicationSettings applicationSettings;
    }
}