using Alko.Configuration.Serilog;
using Alko.Configuration.Settings;

using GroboContainer.Core;

using Market.CustomersAndStaff.Repositories.Configuration;

using Serilog;

namespace Market.CustomersAndStaff.CassandraActualizer.Configuration
{
    public static class ContainerExtensions
    {
        public static void Configure(this IContainer container)
        {
            var applicationSettings = ApplicationSettings.LoadDefault("actualizer.csf");
            var settings = new BaseServiceSettings(applicationSettings);
            
            Log.Logger = SerilogConfigurator
                .ConfigureLogger(settings.LogDirectory)
                .WithConsole()
                .CreateLogger();
            
            container.Configurator.ForAbstraction<IApplicationSettings>().UseInstances(applicationSettings);
            
            Log.Logger.Information("Start actualizing cassandra");
            container.ConfigureRepositories();
        }
    }
}