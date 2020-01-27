using Kontur.Houston.Plugin.Local;

using Market.CustomersAndStaff.AspNetCore.Core.Configuration;
using Market.CustomersAndStaff.OnlineApi.Houston;

using Vostok.Logging.Kontur.Legacy;

namespace Market.CustomersAndStaff.OnlineApi
{
    class EntryPoint
    {
        static void Main(string[] args)
        {
            var configurator = new HoustonSettingsConfigurator(new OnlineApiHoustonProperties());
            var settings = configurator.Container.Get<IWebApiSettings>();

            PluginRunner.Create(new OnlineApiPlugin(configurator))
                        .Listen(settings.Port)
                        .WithProperties()
                        .WithInfo()
                        .WithLog(new VostokLogAdapter(configurator.Logger))
                        .Start();
        }
    }
}
