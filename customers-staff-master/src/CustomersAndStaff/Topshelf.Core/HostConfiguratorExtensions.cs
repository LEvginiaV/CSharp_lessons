using Alko.Configuration.Serilog;
using Alko.Configuration.Settings;

using Topshelf;
using Topshelf.HostConfigurators;

namespace Market.CustomersAndStaff.Topshelf.Core
{
    public static class HostConfiguratorExtensions
    {
        public static HostConfigurator UseLogging(this HostConfigurator hostConfigurator, IApplicationSettings applicationSettings)
        {
            var settings = new BaseServiceSettings(applicationSettings);
            SerilogConfigurator.ConfigureDefault(settings.LogDirectory);
            hostConfigurator.UseSerilog();
            return hostConfigurator;
        }
    }
}