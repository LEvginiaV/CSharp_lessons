using System;
using System.Linq;

using Alko.Configuration.Settings;

using GroboContainer.Infection;

namespace Market.CustomersAndStaff.Portal.Core
{
    public class PortalSettings : IPortalSettings
    {
        [ContainerConstructor]
        public PortalSettings(IApplicationSettings applicationSettings)
        {
            ApiKey = applicationSettings.GetString("Portal.ApiKey");
            AuthUrls = applicationSettings.GetStringArray("Portal.AuthUrls").Select(x => new Uri(x)).ToArray();
            RequisitesUrls = applicationSettings.GetStringArray("Portal.RequisitesUrls").Select(x => new Uri(x)).ToArray();
        }

        public string ApiKey { get; }
        public Uri[] AuthUrls { get; }
        public Uri[] RequisitesUrls { get; }
    }
}