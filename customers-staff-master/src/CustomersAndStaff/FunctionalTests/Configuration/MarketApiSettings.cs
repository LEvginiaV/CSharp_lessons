using System;
using System.Linq;

using Alko.Configuration.Settings;

namespace Market.CustomersAndStaff.FunctionalTests.Configuration
{
    public class MarketApiSettings : IMarketApiSettings
    {
        public MarketApiSettings(IApplicationSettings applicationSettings)
        {
            ApiKey = applicationSettings.GetString("MarketApi.ApiKey");
            Urls = applicationSettings.GetStringArray("MarketApi.Urls").Select(x => new Uri(x)).ToArray();
        }

        public string ApiKey { get; }
        public Uri[] Urls { get; }
    }
}