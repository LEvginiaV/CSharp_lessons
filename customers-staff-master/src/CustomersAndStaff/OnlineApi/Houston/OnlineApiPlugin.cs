using System;

using Kontur.Houston.AspNetCore;
using Kontur.Houston.Plugin;
using Kontur.Houston.Plugin.Vostok;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Market.CustomersAndStaff.OnlineApi.Houston
{
    public class OnlineApiPlugin : AspNetCoreServiceBase<OnlineApiHoustonProperties>
    {
        public OnlineApiPlugin()
        {
        }

        public OnlineApiPlugin(HoustonSettingsConfigurator configurator)
        {
            this.configurator = configurator;
        }

        protected override HoustonAspNetCoreConfiguration Configure(IPluginContext<OnlineApiHoustonProperties> context, IHoustonWebHostBuilder configurationBuilder)
        {
            if(configurator == null)
                configurator = new HoustonSettingsConfigurator(context.GetVostok().Log, context.Properties);
            return configurationBuilder.ConfigureWebHost(x =>
                {
                    x.UseStartup<Startup>();
                    x.ConfigureServices(serviceCollection => serviceCollection.AddSingleton(configurator.Container));
                    x.UseShutdownTimeout(TimeSpan.FromSeconds(30));
                }).Build();
        }

        private HoustonSettingsConfigurator configurator;
    }
}