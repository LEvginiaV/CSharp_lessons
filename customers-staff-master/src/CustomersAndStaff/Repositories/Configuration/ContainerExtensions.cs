using System;

using Alko.Configuration.Settings;

using AutoMapper;

using Cassandra;

using GroboContainer.Core;

using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.Models.OnlineRecording;
using Market.CustomersAndStaff.Models.ServiceCalendar;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.Repositories.StoredModels;
using Market.CustomersAndStaff.Repositories.StoredModels.OnlineRecording;

using MoreLinq;

using SKBKontur.Catalogue.CassandraUtils.Cassandra.Commons.SessionsManager;
using SKBKontur.Catalogue.CassandraUtils.Cassandra.Commons.Settings;
using SKBKontur.Catalogue.CassandraUtils.Cassandra.SessionTableQueryExtending.PrimitiveStoring;
using SKBKontur.Catalogue.CassandraUtils.DistributedLock.Locker;
using SKBKontur.Catalogue.CassandraUtils.DistributedLock.Metrics;
using SKBKontur.Catalogue.CassandraUtils.DistributedLock.Settings;

namespace Market.CustomersAndStaff.Repositories.Configuration
{
    public static class ContainerExtensions
    {
        public static void ConfigureRepositories(this IContainer container)
        {
            var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMissingTypeMaps = false;
                    cfg.CreateMap<DateTime, LocalDate>().ConvertUsing(x => new LocalDate(x.Year, x.Month, x.Day));
                    cfg.CreateMap<LocalDate, DateTime>().ConvertUsing(x => new DateTime(x.Year, x.Month, x.Day, 0, 0, 0, DateTimeKind.Utc));
                    cfg.CreateMap<Birthday, string>().ConvertUsing(x => x == null ? null : x.ToString());
                    cfg.CreateMap<string, Birthday>().ConvertUsing(x => x == null ? null : Birthday.Parse(x));
                    cfg.CreateMap<Gender, string>().ConvertUsing(x => x.ToString());
                    cfg.CreateMap<string, Gender>().ConvertUsing(x => (Gender)Enum.Parse(typeof(Gender), x));
                    cfg.CreateMap<CustomerStorageElement, Customer>().ReverseMap();
                    cfg.CreateMap<CustomerStorageElement, CustomerIndexByPhoneStorageElement>().ForMember(x => x.CustomerId, 
                                                                                            expression => expression.MapFrom(x => x.Id));
                    cfg.CreateMap<WorkerStorageElement, Worker>().ReverseMap();
                    cfg.CreateMap<Birthday, Birthday>();
                    cfg.CreateMap<Customer, Customer>();
                    cfg.CreateMap<Worker, Worker>();
                    cfg.CreateMap<ServiceCalendarRecord, ServiceCalendarRemovedRecord>().ReverseMap();
                    cfg.CreateMap<PublicLinkToShopIdStorageElement, PublicLink>().ReverseMap();
                    cfg.CreateMap<PublicLinkToShopIdStorageElement, ShopIdToPublicLinkStorageElement>();
                });

            config.AssertConfigurationIsValid();
            config.CompileMappings();

            container.Configurator.ForAbstraction<IMapper>().UseInstances(new Mapper(config));
            var cassandraSettings = container.Get<ICassandraSettings>();

            container.ConfigureContainerForWorkingWithCassandra();
            container.ConfigureCassandraStorages();
            container.Configurator.ForAbstraction<ILocker>().UseInstances(
                new Locker(container.Get<ICassandraSessionsManager>(),
                           new LockSettings(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(cassandraSettings.LockTtlSeconds), cassandraSettings.LockKeyspace),
                           new LockMetrics(cassandraSettings.LockKeyspace))
            );
        }

        private static void ConfigureCassandraStorages(this IContainer container)
        {
            var simpleCassandraStorages = container.Get<ICassandraStorageFactory>().GetSimpleCassandraStorages();
            simpleCassandraStorages.ForEach(x => container.Configurator.ForAbstraction(x.GetType()).UseInstances(x));
        }

        private static void ConfigureContainerForWorkingWithCassandra(this IContainer container)
        {
            var connectionString = container.Get<IApplicationSettings>().GetString("CassandraConnectionString");
            container.Configurator.ForAbstraction<ICassandraSessionsManager>()
                     .UseInstances(new MultipleCassandraSessionsManager(MultipleCassandraSettingsBuilder.CreateFromConnectionString(connectionString)));
            CustomerAndStaffCassandraSchemaActualizer.ConfigureMappings(container);
        }
    }
}