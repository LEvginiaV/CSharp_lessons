using Alko.Configuration.Serilog;
using Alko.Configuration.Settings;
using Alko.Graphite.Core;

using GroboContainer.Core;
using GroboContainer.Impl;

using Market.Api.Client;
using Market.CustomersAndStaff.FrontApi.Configuration;
using Market.CustomersAndStaff.FrontApi.Converters.Mappers;
using Market.CustomersAndStaff.GroboContainer.Core;
using Market.CustomersAndStaff.Portal.Core;
using Market.CustomersAndStaff.Repositories.Configuration;

using Serilog;

using SkbKontur.Graphite.Client;

using Vostok.Logging.Abstractions;
using Vostok.Logging.Serilog;

namespace Market.CustomersAndStaff.FrontApi.Houston
{
    public class HoustonSettingsConfigurator
    {
        private readonly FrontApiHoustonProperties properties;

        public IApplicationSettings Settings { get; }
        public ILog Logger { get; }
        public IContainer Container { get; private set; }

        public HoustonSettingsConfigurator(FrontApiHoustonProperties properties)
        {
            this.properties = properties;
            Settings = new ApplicationSettingsBuilder()
                       .LoadFromLocalSettings("frontApi.csf")
                       .Build();
            Logger = new SerilogLog(SerilogConfigurator.ConfigureLogger(Settings.GetString("LogDirectory")).WithConsole().CreateLogger());
            ConfigureContainer();
        }

        public HoustonSettingsConfigurator(ILog log, FrontApiHoustonProperties properties)
        {
            this.properties = properties;
            Settings = new ApplicationSettingsBuilder()
                       .LoadFromClusterConfig("customersAndStaff", "frontApi.csf")
                       .LoadFromHouston(() => properties)
                       .Build();
            SerilogHoustonConfigurator.ConfigureDefault(log);
            Logger = log;
            ConfigureContainer();
        }

        public void ConfigureContainer()
        {
            Container = new Container(new ContainerConfiguration(AssembliesLoader.Load()));
            Container.Configurator.ForAbstraction<IApplicationSettings>().UseInstances(Settings);
            Container.Configurator.ForAbstraction<IStatsDClient>().UseInstances(Container.Get<StatsDClientFactory>().Create());

            Container.Configurator.ForAbstraction<ILogger>().UseInstances(Log.Logger);
            Container.Configurator.ForAbstraction<ILog>().UseInstances(Logger);

            ConfigureSpecificSettings();

            Container.ConfigurePortal();
            Container.ConfigureRepositories();

            Container.Configurator.ForAbstraction<IMapperWrapper>().UseInstances(Container.Create<MapperWrapper>());
        }

        private void ConfigureSpecificSettings()
        {
            var marketApiSettings = Container.Get<IMarketApiSettings>();
            var marketApiClient = new MarketApiClient(marketApiSettings.Urls, marketApiSettings.ApiKey);
            Container.Configurator.ForAbstraction<IMarketApiClient>().UseInstances(marketApiClient);
        }
    }
}