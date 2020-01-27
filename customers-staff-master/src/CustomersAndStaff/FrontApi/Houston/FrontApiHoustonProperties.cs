using Alko.Configuration.Settings.Houston;

using Kontur.Houston.Plugin;

namespace Market.CustomersAndStaff.FrontApi.Houston
{
    public class FrontApiHoustonProperties
    {
        [ApplicationSettingsAlias("MarketApi.ApiKey")]
        [HoustonSecured(Level = SecurityLevel.Total)]
        public string MarketApiApiKey { get; set; }
        
        [ApplicationSettingsAlias("Portal.ApiKey")]
        [HoustonSecured(Level = SecurityLevel.Total)]
        public string PortalApiKey { get; set; }
        
        [ApplicationSettingsAlias("Portal.MasterLogin")]
        [HoustonSecured(Level = SecurityLevel.Total)]
        public string PortalMasterLogin { get; set; }
        
        [ApplicationSettingsAlias("Portal.MasterPassword")]
        [HoustonSecured(Level = SecurityLevel.Total)]
        public string PortalMasterPassword { get; set; }

        [ApplicationSettingsAlias("Printer.ApiKey")]
        [HoustonSecured(Level = SecurityLevel.Total)]
        public string PrinterApiKey { get; set; }
    }
}