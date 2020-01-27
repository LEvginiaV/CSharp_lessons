using System;
using System.Linq;
using System.Reflection;

using Alko.Configuration.Serilog;
using Alko.Configuration.Settings;

using GroboContainer.Core;
using GroboContainer.Impl;

using Market.CustomersAndStaff.GroboContainer.Core;
using Market.CustomersAndStaff.Repositories.Configuration;

using Vostok.Logging.Abstractions;
using Vostok.Logging.Serilog;

namespace Market.CustomersAndStaff.GodLikeTools
{
    public class EntryPoint
    {
        public static void Main(string[] args)
        {
            var container = new Container(new ContainerConfiguration(AssembliesLoader.Load()));
            var settings = ApplicationSettings.LoadDefault("godLikeTools.csf");
            container.Configurator.ForAbstraction<IApplicationSettings>().UseInstances(settings);
            container.ConfigureRepositories();

            var logger = new SerilogLog(SerilogConfigurator.ConfigureLogger(settings.GetString("LogDirectory")).WithConsole().CreateLogger());
            container.Configurator.ForAbstraction<ILog>().UseInstances(logger);

            var commandLine = new CommandLine(args);
            var command = commandLine.GetCommandLineSetting("-mode");

            var processorTypes = Assembly.GetExecutingAssembly()
                                         .GetTypes()
                                         .Where(x => typeof(ICommandProcessor).IsAssignableFrom(x))
                                         .ToArray();

            var processorsGroup = processorTypes.GroupBy(x => x.Name).FirstOrDefault(group => group.Count() > 1);
            if(processorsGroup != null)
                throw new Exception("Some processors have same command descriptions: " + processorsGroup.First().Name);

            var processorType = processorTypes.SingleOrDefault(x => x.Name == command);

            if(processorType == null)
            {
                throw new BadCommandLineException("The list of available commands: " +
                                                  string.Join(", ", processorTypes.Select(p => p.Name)));
            }

            var processor = (ICommandProcessor)container.Get(processorType);

            processor.Run(commandLine);
            container.Dispose();
            logger.Info($"GodLikeTool Processor in mode '{command}' completed");
        }
    }
}