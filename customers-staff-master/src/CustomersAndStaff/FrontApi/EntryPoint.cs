using Kontur.Houston.Plugin.Local;

using Market.CustomersAndStaff.AspNetCore.Core.Configuration;
using Market.CustomersAndStaff.FrontApi.Houston;

using Vostok.Logging.Kontur.Legacy;

namespace Market.CustomersAndStaff.FrontApi
{
    class EntryPoint
    {
        static void Main()
        {
            var configurator = new HoustonSettingsConfigurator(new FrontApiHoustonProperties());
            var settings = configurator.Container.Get<IWebApiSettings>();
                
            PluginRunner.Create(new FrontApiPlugin(configurator))
                        .Listen(settings.Port)
                        .WithProperties()
                        .WithInfo()
                        .WithLog(new VostokLogAdapter(configurator.Logger))
                        .Start();
        }
    }
}