using Kontur.Houston.Plugin.Local;

using Market.CustomersAndStaff.AspNetCore.Core.Configuration;
using Market.CustomersAndStaff.ServiceApi.Houston;

using Vostok.Logging.Kontur.Legacy;

namespace Market.CustomersAndStaff.ServiceApi
{
    class EntryPoint
    {
        static void Main()
        {
            var configurator = new HoustonSettingsConfigurator(new ServiceApiHoustonProperties());
            var settings = configurator.Container.Get<IWebApiSettings>();

            PluginRunner.Create(new ServiceApiPlugin(configurator))
                        .Listen(settings.Port)
                        .WithProperties()
                        .WithInfo()
                        .WithLog(new VostokLogAdapter(configurator.Logger))
                        .Start();
        }
    }
}