using System;

using Kontur.Houston.AspNetCore;
using Kontur.Houston.Plugin;
using Kontur.Houston.Plugin.Vostok;

using Market.CustomersAndStaff.AspNetCore.Core.AspNetCoreServiceConfiguration;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Market.CustomersAndStaff.ServiceApi.Houston
{
    public class ServiceApiPlugin : AspNetCoreServiceBase<ServiceApiHoustonProperties>
    {
        public ServiceApiPlugin()
        {
        }

        public ServiceApiPlugin(HoustonSettingsConfigurator configurator)
        {
            this.configurator = configurator;
        }

        protected override HoustonAspNetCoreConfiguration Configure(IPluginContext<ServiceApiHoustonProperties> context, IHoustonWebHostBuilder configurationBuilder)
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