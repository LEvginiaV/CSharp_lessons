using System;
using System.Diagnostics;

using Alko.Configuration.Process;
using Alko.Configuration.Settings;

using GroboContainer.Core;
using GroboContainer.Impl;

using Market.CustomersAndStaff.GroboContainer.Core;

using Serilog;

using Topshelf;

using Vostok.Logging.Serilog;

namespace Market.CustomersAndStaff.Topshelf.Core
{
    public abstract class ServiceBase : ServiceControl
    {
        protected ServiceBase(IApplicationSettings settings)
        {
            this.settings = settings;
            log = Log.Logger;
        }

        protected abstract void StartChildServices();
        protected abstract void StopChildServices();
        protected abstract void ConfigureContainerForChildServices();

        public bool Start(HostControl hostControl)
        {
            try
            {
                log.Information("Configuring container");
                ConfigureContainer();
                log.Information("Starting service");
                ProcessPriorityHelper.SetMemoryPriority(ProcessMemoryPriority.Normal, new SerilogLog(log));
                ProcessPriorityHelper.SetProcessPriorityClass(ProcessPriorityClass.Normal, new SerilogLog(log));
                if(!settings.TryGetInt("ThreadMultiplier", out var threadMultiplier))
                {
                    threadMultiplier = 16;
                    log.Warning("`ThreadMultiplier` setting not found. Use multiplier {multiplier}", threadMultiplier);
                }

                ThreadPoolUtility.SetUp(new SerilogLog(log), threadMultiplier);

                StartChildServices();

                log.Information("Service is started");
                Console.WriteLine("Service is started");
                return true;
            }
            catch(Exception e)
            {
                log.Error(e, "Unexpected error while starting service");
                (log as IDisposable)?.Dispose();
                throw;
            }
        }

        public bool Stop(HostControl hostControl)
        {
            log.Information("Stopping service");
            StopChildServices();
            container.Dispose();
            log.Information("Service is stopped");
            (log as IDisposable)?.Dispose();
            return true;
        }

        private void ConfigureContainer()
        {
            container = new Container(new ContainerConfiguration(AssembliesLoader.Load()));
            container.Configurator.ForAbstraction<IApplicationSettings>().UseInstances(settings);
            ConfigureContainerForChildServices();
        }

        protected IContainer container;
        private readonly ILogger log;
        private readonly IApplicationSettings settings;
    }
}