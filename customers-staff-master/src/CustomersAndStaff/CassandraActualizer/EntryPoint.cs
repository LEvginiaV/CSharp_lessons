using System;
using System.Threading;

using Cassandra;

using GroboContainer.Core;
using GroboContainer.Impl;

using Market.CustomersAndStaff.CassandraActualizer.Configuration;
using Market.CustomersAndStaff.GroboContainer.Core;
using Market.CustomersAndStaff.Repositories;

using Serilog;

namespace Market.CustomersAndStaff.CassandraActualizer
{
    class EntryPoint
    {
        static void Main(string[] args)
        {
            var container = new Container(new ContainerConfiguration(AssembliesLoader.Load()));
            container.Configure();
            logger = Log.Logger.ForContext<EntryPoint>();
            for(var i = 0; i <= 10; i++)
            {
                try
                {
                    container.Get<CustomerAndStaffCassandraSchemaActualizer>().Actualize();
                    Log.CloseAndFlush();
                    break;
                }
                catch(NoHostAvailableException)
                {
                    logger.Error("Cassandra host not available");
                    if(i == 10)
                    {
                        logger.Error("Impossible to aclualize schema: cassandra host not available");
                        Log.CloseAndFlush();
                        Environment.Exit(1);
                    }

                    Thread.Sleep(1000);
                }
                catch(Exception e)
                {
                    logger.Error(e, "unexpected exception while actualize cassandra");
                    Log.CloseAndFlush();
                    Environment.Exit(1);
                }
            }
        }

        private static ILogger logger;
    }
}