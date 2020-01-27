using System.Diagnostics;

using AutoMapper;

using Market.CustomersAndStaff.EvrikaPrintClient.Models;
using Market.CustomersAndStaff.EvrikaPrintClient.Models.WorkOrderPrintDtos;
using Market.CustomersAndStaff.FrontApi.Dto.Calendars;
using Market.CustomersAndStaff.FrontApi.Dto.Common;
using Market.CustomersAndStaff.FrontApi.Dto.Customers;
using Market.CustomersAndStaff.FrontApi.Dto.Print;
using Market.CustomersAndStaff.FrontApi.Dto.TimeZones;
using Market.CustomersAndStaff.FrontApi.Dto.Workers;
using Market.CustomersAndStaff.FrontApi.Dto.WorkingCalendar;
using Market.CustomersAndStaff.FrontApi.Dto.WorkOrders;
using Market.CustomersAndStaff.Models.Calendar;
using Market.CustomersAndStaff.Models.Common;
using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.Models.ServiceCalendar;
using Market.CustomersAndStaff.Models.TimeZones;
using Market.CustomersAndStaff.Models.Validations;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.Models.WorkOrders;

using Serilog;

namespace Market.CustomersAndStaff.FrontApi.Converters.Mappers
{
    public class MapperWrapper : IMapperWrapper
    {
        public MapperWrapper(ILogger logger)
        {
            var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMissingTypeMaps = false;

                    cfg.CreateMap<Birthday, BirthdayDto>().ReverseMap();
                    cfg.CreateMap<CustomerInfoDto, Customer>()
                       .ForMember(x => x.Id, opt => opt.Ignore())
                       .ForMember(x => x.OrganizationId, opt => opt.Ignore())
                       .ForMember(x => x.IsDeleted, opt => opt.Ignore());
                    cfg.CreateMap<Customer, CustomerDto>();
                    
                    cfg.CreateMap<WorkerInfoDto, Worker>()
                       .ForMember(x => x.Id, opt => opt.Ignore())
                       .ForMember(x => x.ShopId, opt => opt.Ignore())
                       .ForMember(x => x.Code, opt => opt.Ignore())
                       .ForMember(x => x.IsDeleted, opt => opt.Ignore());
                    cfg.CreateMap<Worker, WorkerDto>();

                    cfg.CreateMap<TimePeriod, TimePeriodDto>().ReverseMap();

                    cfg.CreateMap<WorkerScheduleRecord, WorkingCalendarRecordDto>().ReverseMap();
                    cfg.CreateMap<WorkingCalendarDayInfoDto, WorkerCalendarDay<WorkerScheduleRecord>>()
                       .ForMember(x => x.WorkerId, opt => opt.Ignore())
                       .ReverseMap();

                    cfg.CreateMap<CustomerStatus, CustomerStatusDto>().ReverseMap();
                    cfg.CreateMap<RecordStatus, RecordStatusDto>().ReverseMap();
                    cfg.CreateMap<ServiceCalendarRecord, ServiceCalendarRecordDto>();
                    cfg.CreateMap<ServiceCalendarRecordInfoDto, ServiceCalendarRecord>()
                       .ForMember(x => x.Id, opt => opt.Ignore())
                       .ForMember(x => x.CustomerStatus, opt => opt.Ignore())
                       .ForMember(x => x.RecordStatus, opt => opt.Ignore());
                    cfg.CreateMap<ServiceCalendarRecordUpdateDto, ServiceCalendarRecord>()
                       .ForMember(x => x.Id, opt => opt.Ignore())
                       .ForMember(x => x.CustomerStatus, opt => opt.Ignore())
                       .ForMember(x => x.RecordStatus, opt => opt.Ignore());
                    cfg.CreateMap<WorkerCalendarDay<ServiceCalendarRecord>, WorkerServiceCalendarDayDto>();
                    cfg.CreateMap<ShopCalendarDay<ServiceCalendarRecord>, ShopServiceCalendarDayDto>();

                    cfg.CreateMap<ValidationResult, ValidationResultDto>();

                    cfg.CreateMap<TimeZone, TimeZoneDto>();

                    cfg.CreateMap<WorkOrderNumber, WorkOrderNumberDto>().ReverseMap();
                    cfg.CreateMap<WorkOrderStatus, WorkOrderStatusDto>().ReverseMap();
                    cfg.CreateMap<ShopRequisites, ShopRequisitesDto>().ReverseMap();
                    cfg.CreateMap<CustomerValueType, CustomerValueTypeDto>().ReverseMap();
                    cfg.CreateMap<ApplianceCustomerValue, ApplianceCustomerValueDto>().ReverseMap();
                    cfg.CreateMap<VehicleCustomerValue, VehicleCustomerValueDto>().ReverseMap();
                    cfg.CreateMap<OtherCustomerValue, OtherCustomerValueDto>().ReverseMap();
                    cfg.CreateMap<BaseCustomerValue, BaseCustomerValueDto>()
                       .Include<ApplianceCustomerValue, ApplianceCustomerValueDto>()
                       .Include<VehicleCustomerValue, VehicleCustomerValueDto>()
                       .Include<OtherCustomerValue, OtherCustomerValueDto>()
                       .ReverseMap();
                    cfg.CreateMap<CustomerValueList, CustomerValueListDto>().ReverseMap();
                    cfg.CreateMap<CustomerProduct, CustomerProductDto>().ReverseMap();
                    cfg.CreateMap<ShopProduct, ShopProductDto>().ReverseMap();
                    cfg.CreateMap<ShopService, ShopServiceDto>().ReverseMap();
                    cfg.CreateMap<WorkOrder, WorkOrderDto>().ReverseMap();
                    cfg.CreateMap<WorkOrder, WorkOrderItemDto>();

                    cfg.CreateMap<VehicleCustomerValue, WorkOrderVehiclePrintDto>()
                       .ForMember(x => x.Comment, opt => opt.MapFrom(x => x.AdditionalInfo));
                    cfg.CreateMap<ApplianceCustomerValue, WorkOrderAppliancePrintDto>()
                       .ForMember(x => x.Comment, opt => opt.MapFrom(x => x.AdditionalInfo));
                    cfg.CreateMap<PrintTaskStatus, PrintTaskStatusDto>();
                    cfg.CreateMap<PrintTaskInfo, PrintTaskInfoDto>();
                });

            config.AssertConfigurationIsValid();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            config.CompileMappings();
            stopwatch.Stop();
            logger.Information($"Mappings compile time: {stopwatch.Elapsed:mm\\:ss\\.fff}");
            mapper = new Mapper(config);
        }

        public TDest Map<TDest>(object source)
        {
            return mapper.Map<TDest>(source);
        }

        private readonly IMapper mapper;
    }
}