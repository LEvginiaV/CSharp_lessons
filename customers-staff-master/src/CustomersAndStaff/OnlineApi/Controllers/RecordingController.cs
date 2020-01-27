using System;
using System.Linq;
using System.Threading.Tasks;

using Kontur.Utilities.Convertions.Time;

using Market.Api.Client;
using Market.Api.Models.Shops;
using Market.CustomersAndStaff.Models.Common;
using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.Models.ServiceCalendar;
using Market.CustomersAndStaff.Models.TimeZones;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.OnlineApi.Converters.Mappers;
using Market.CustomersAndStaff.OnlineApi.Converters.RecordingInfo;
using Market.CustomersAndStaff.OnlineApi.Dto.Common;
using Market.CustomersAndStaff.OnlineApi.Dto.CreateRecord;
using Market.CustomersAndStaff.OnlineApi.Dto.RecordingInfo;
using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Services.ServiceCalendar;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Market.CustomersAndStaff.OnlineApi.Controllers
{
    [ApiController]
    [Route("{link}/recording")]
    public class RecordingController : Controller, IShopController
    {
        public RecordingController(
            ICalendarService calendarService,
            ICustomerRepository customerRepository,
            IMapperWrapper mapper, IMarketApiClient marketApiClient,
            IWorkerRepository workerRepository,
            ICalendarRepository<WorkerScheduleRecord> workerCalendarRepository,
            ICalendarRepository<ServiceCalendarRecord> serviceCalendarRepository,
            IOnlineServiceListRepository onlineServiceListRepository,
            IRecordingInfoConverter recordingInfoConverter,
            ITimeZoneSettingsRepository timeZoneSettingsRepository)
        {
            this.calendarService = calendarService;
            this.mapper = mapper;
            this.marketApiClient = marketApiClient;
            this.workerRepository = workerRepository;
            this.workerCalendarRepository = workerCalendarRepository;
            this.serviceCalendarRepository = serviceCalendarRepository;
            this.onlineServiceListRepository = onlineServiceListRepository;
            this.recordingInfoConverter = recordingInfoConverter;
            this.timeZoneSettingsRepository = timeZoneSettingsRepository;
            this.customerRepository = customerRepository;
        }

        [HttpGet]
        [ProducesResponseType(typeof(RecordingInfoDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRecordingInfo()
        {
            var serviceIdsTask = onlineServiceListRepository.ReadAsync(Shop.Id);
            var productsTask = marketApiClient.Products.GetAll(Shop.Id);
            var workersTask = workerRepository.ReadByShopAsync(Shop.Id);

            var timezoneId = await timeZoneSettingsRepository.GetAsync(Shop.Id);
            var timezone = TimeZoneList.TimeZones.FirstOrDefault(x => x.Id == timezoneId);

            var today = (DateTime.UtcNow + (timezone?.Offset ?? TimeSpan.Zero)).Date;

            var workersCalendarTask = workerCalendarRepository.ReadShopCalendarRangeAsync(Shop.Id, today, today + 30.Days());
            var serviceCalendarTask = serviceCalendarRepository.ReadShopCalendarRangeAsync(Shop.Id, today, today + 30.Days());

            await Task.WhenAll(serviceIdsTask, productsTask, workersTask, workersCalendarTask, serviceCalendarTask);

            var recordingInfo = recordingInfoConverter.CreateInfo(
                Shop,
                serviceIdsTask.Result,
                productsTask.Result,
                workersTask.Result,
                workersCalendarTask.Result,
                serviceCalendarTask.Result,
                today);

            return Ok(recordingInfo);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationResultDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRecordDto([FromBody] CreateRecordDto createRecord)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customersByPhone = await customerRepository.FindByPhoneAsync(Shop.OrganizationId, createRecord.CustomerPhone);

            Customer customer;
            if(customersByPhone.Length > 0)
            {
                customer = customersByPhone.First();
            }
            else
            {
                customer = await customerRepository.CreateAsync(Shop.OrganizationId, new Customer
                    {
                        Name = createRecord.CustomerName,
                        Phone = createRecord.CustomerPhone,
                    });
            }
            
            var (_, validationResult) = await calendarService.CreateAsync(Shop.Id, createRecord.Date, createRecord.WorkerId, new ServiceCalendarRecord
                {
                    IsOnlineRecord = true,
                    Period = mapper.Map<TimePeriod>(createRecord.Period),
                    ProductIds = new[] {createRecord.ServiceId},
                    CustomerId = customer.Id,
                });

            if(!validationResult.IsSuccess)
            {
                return BadRequest(mapper.Map<ValidationResultDto>(validationResult));
            }

            return Ok();
        }

        public Shop Shop { get; set; }

        private readonly ICalendarService calendarService;
        private readonly ICustomerRepository customerRepository;
        private readonly IMapperWrapper mapper;
        private readonly IMarketApiClient marketApiClient;
        private readonly IWorkerRepository workerRepository;
        private readonly ICalendarRepository<WorkerScheduleRecord> workerCalendarRepository;
        private readonly ICalendarRepository<ServiceCalendarRecord> serviceCalendarRepository;
        private readonly IOnlineServiceListRepository onlineServiceListRepository;
        private readonly IRecordingInfoConverter recordingInfoConverter;
        private readonly ITimeZoneSettingsRepository timeZoneSettingsRepository;
    }
}