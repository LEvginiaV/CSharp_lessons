using Alko.Configuration.Serilog;
using Alko.Configuration.Settings;
using Alko.Graphite.Core;

using GroboContainer.Core;
using GroboContainer.Impl;

using Market.CustomersAndStaff.GroboContainer.Core;
using Market.CustomersAndStaff.Repositories.Configuration;

using Serilog;

using SkbKontur.Graphite.Client;

using Vostok.Logging.Abstractions;
using Vostok.Logging.Serilog;

namespace Market.CustomersAndStaff.ServiceApi.Houston
{
    public class HoustonSettingsConfigurator
    {
        public HoustonSettingsConfigurator(ServiceApiHoustonProperties properties)
        {
            this.properties = properties;
            Settings = ApplicationSettings.LoadDefault("serviceApi.csf");
            Logger = new SerilogLog(SerilogConfigurator.ConfigureLogger(Settings.GetString("LogDirectory")).WithConsole().CreateLogger());
            ConfigureContainer();
        }

        public HoustonSettingsConfigurator(ILog log, ServiceApiHoustonProperties properties)
        {
            this.properties = properties;
            Settings = ApplicationSettings.LoadFromClusterConfig("serviceApi.csf", "customersAndStaff");
            SerilogHoustonConfigurator.ConfigureDefault(log);
            Logger = log;
            ConfigureContainer();
        }

        public IApplicationSettings Settings { get; }
        public ILog Logger { get; }
        public IContainer Container { get; private set; }

        public void ConfigureContainer()
        {
            Container = new Container(new ContainerConfiguration(AssembliesLoader.Load()));
            Container.Configurator.ForAbstraction<IApplicationSettings>().UseInstances(Settings);
            Container.Configurator.ForAbstraction<IStatsDClient>().UseInstances(Container.Get<StatsDClientFactory>().Create());

            Container.Configurator.ForAbstraction<ILog>().UseInstances(Logger);
            Container.Configurator.ForAbstraction<ILogger>().UseInstances(Log.Logger);

            Container.ConfigureRepositories();
        }

        private readonly ServiceApiHoustonProperties properties;
    }
}