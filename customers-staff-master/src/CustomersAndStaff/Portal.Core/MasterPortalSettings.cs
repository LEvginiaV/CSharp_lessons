using Alko.Configuration.Settings;

using GroboContainer.Infection;

namespace Market.CustomersAndStaff.Portal.Core
{
    public class MasterPortalSettings : IMasterPortalSettings
    {
        [ContainerConstructor]
        public MasterPortalSettings(IApplicationSettings applicationSettings)
        {
            MasterLogin = applicationSettings.GetString("Portal.MasterLogin");
            MasterPassword = applicationSettings.GetString("Portal.MasterPassword");
        }

        public string MasterLogin { get; }
        public string MasterPassword { get; }
    }
}