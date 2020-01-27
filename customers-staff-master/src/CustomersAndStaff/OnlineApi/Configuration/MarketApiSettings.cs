using System;
using System.Linq;

using Alko.Configuration.Settings;

namespace Market.CustomersAndStaff.OnlineApi.Configuration
{
    public class MarketApiSettings : IMarketApiSettings
    {
        public MarketApiSettings(IApplicationSettings applicationSettings)
        {
            Urls = applicationSettings.GetStringArray("MarketApi.Urls").Select(x => new Uri(x)).ToArray();
            ApiKey = applicationSettings.GetString("MarketApi.ApiKey");
        }
        
        public string ApiKey { get; }
        public Uri[] Urls { get; }
    }
}