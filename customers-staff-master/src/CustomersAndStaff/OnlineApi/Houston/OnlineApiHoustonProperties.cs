using Alko.Configuration.Settings.Houston;

using Kontur.Houston.Plugin;

namespace Market.CustomersAndStaff.OnlineApi.Houston
{
    public class OnlineApiHoustonProperties
    {
        [ApplicationSettingsAlias("MarketApi.ApiKey")]
        [HoustonSecured(Level = SecurityLevel.Total)]
        public string MarketApiApiKey { get; set; }
    }
}