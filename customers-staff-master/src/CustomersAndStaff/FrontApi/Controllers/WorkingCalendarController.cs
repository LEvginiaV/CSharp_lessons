using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Kontur.Utilities.Convertions.Time;

using Market.Api.Models.Shops;
using Market.CustomersAndStaff.FrontApi.Converters.Mappers;
using Market.CustomersAndStaff.FrontApi.Converters.WorkerSchedule;
using Market.CustomersAndStaff.FrontApi.Dto.Common;
using Market.CustomersAndStaff.FrontApi.Dto.WorkingCalendar;
using Market.CustomersAndStaff.Models.Calendar;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.ModelValidators.Periods;
using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Utils.Extensions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using MoreLinq;

namespace Market.CustomersAndStaff.FrontApi.Controllers
{
    [ApiController]
    [Route("shops/{shopId:guid}/workers")]
    public class WorkingCalendarController : ControllerBase, IShopController
    {
        public WorkingCalendarController(
            ICalendarRepository<WorkerScheduleRecord> workerScheduleRepository,
            IWorkerRepository workerRepository,
            IPeriodValidator<WorkerScheduleRecord> periodValidator,
            IMapperWrapper mapperWrapper, IWorkerScheduleConverter workerScheduleConverter)
        {
            this.workerScheduleRepository = workerScheduleRepository;
            this.workerRepository = workerRepository;
            this.periodValidator = periodValidator;
            this.mapperWrapper = mapperWrapper;
            this.workerScheduleConverter = workerScheduleConverter;
        }

        [HttpGet("shopWorkingCalendar/days/{date:datetime}")]
        [ProducesResponseType(typeof(ShopWorkingDayDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetShopScheduleOneDay([FromRoute] DateTime date, [FromQuery] int version = -1)
        {
            var currentVersion = await workerScheduleRepository.GetVersionAsync(Shop.Id, date);
            if(currentVersion == version)
            {
                return Ok(new ShopWorkingDayDto{Version = currentVersion, Date = date});
            }
            
            var shopCalendarDay = await workerScheduleRepository.ReadShopCalendarDayAsync(Shop.Id, date);
            var workerIds = (await workerRepository.ReadByShopAsync(Shop.Id)).Select(x => x.Id);
            return Ok(workerScheduleConverter.Convert(shopCalendarDay, new HashSet<Guid>(workerIds), currentVersion));
        }
        
        [HttpGet("shopWorkingCalendar")]
        [ProducesResponseType(typeof(ShopWorkingCalendarDto), StatusCodes.Status200OK)]
        public Task<IActionResult> GetShopScheduleOnCurrentMonthObsolete()
        {
            return GetShopScheduleObsolete(DateHelper.GetFirstDayOfMonth(DateTime.UtcNow));
        }

        [HttpGet("shopWorkingCalendar/{month:datetime}")]
        [ProducesResponseType(typeof(ShopWorkingCalendarDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetShopScheduleObsolete([FromRoute] DateTime month, [FromQuery] int version = -1)
        {
            var currentVersion = await workerScheduleRepository.GetVersionAsync(Shop.Id, month);

            if(currentVersion == version)
            {
                return Ok(new ShopWorkingCalendarDto{Version = currentVersion, Month = month});
            }

            var shopCalendarMonth = await workerScheduleRepository.ReadShopCalendarMonthAsync(Shop.Id, month);
            var workerIds = (await workerRepository.ReadByShopAsync(Shop.Id)).Select(x => x.Id);
            return Ok(workerScheduleConverter.ConvertObsolete(shopCalendarMonth, new HashSet<Guid>(workerIds), currentVersion));
        }

        [HttpGet("v1/shopWorkingCalendar")]
        [ProducesResponseType(typeof(ShopWorkingCalendarDto), StatusCodes.Status200OK)]
        public Task<IActionResult> GetShopScheduleOnCurrentMonth()
        {
            return GetShopSchedule(DateHelper.GetFirstDayOfMonth(DateTime.UtcNow));
        }

        [HttpGet("v1/shopWorkingCalendar/{month:datetime}")]
        [ProducesResponseType(typeof(ShopWorkingCalendarDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetShopSchedule([FromRoute] DateTime month, [FromQuery] int version = -1)
        {
            var prevMonthVersion = await workerScheduleRepository.GetVersionAsync(Shop.Id, DateHelper.GetFirstDayOfPreviousMonth(month));
            var curMonthVersion = await workerScheduleRepository.GetVersionAsync(Shop.Id, month);
            var nextMonthVersion = await workerScheduleRepository.GetVersionAsync(Shop.Id, DateHelper.GetFirstDayOfNextMonth(month));

            var currentVersion = prevMonthVersion + curMonthVersion + nextMonthVersion;

            if(currentVersion == version)
            {
                return Ok(new ShopWorkingCalendarDto{Version = currentVersion, Month = month});
            }

            var workerIds = (await workerRepository.ReadByShopAsync(Shop.Id)).Select(x => x.Id).ToHashSet();

            var range = await workerScheduleRepository.ReadShopCalendarRangeAsync(
                            Shop.Id,
                            DateHelper.GetFirstDayOfMonth(month) - 1.Days(),
                            DateHelper.GetLastDayOfMonth(month) + 1.Days()
                        );

            return Ok(new ShopWorkingCalendarDto
                {
                    Version = currentVersion,
                    Month = month,
                    WorkingCalendarMap = workerScheduleConverter.Convert(range, workerIds)
                });
        }

        [HttpPut("{workerId:guid}/schedule")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationResultDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateWorkerSchedule([FromRoute] Guid workerId, [FromBody] WorkingCalendarDayInfoDto[] updates)
        {
            var worker = await workerRepository.ReadAsync(Shop.Id, workerId);
            if(worker == null)
            {
                return NotFound();
            }

            var modelUpdates = updates.Select(x => mapperWrapper.Map<WorkerCalendarDay<WorkerScheduleRecord>>(x)).ToArray();

            foreach(var update in modelUpdates)
            {
                var validationResult = periodValidator.Validate(update);
                if(!validationResult.IsSuccess)
                {
                    return BadRequest(mapperWrapper.Map<ValidationResultDto>(validationResult));
                }

                update.WorkerId = workerId;
            }

            await workerScheduleRepository.WriteAsync(Shop.Id, modelUpdates);

            return Ok();
        }

        public Shop Shop { get; set; }

        private readonly ICalendarRepository<WorkerScheduleRecord> workerScheduleRepository;
        private readonly IWorkerRepository workerRepository;
        private readonly IPeriodValidator<WorkerScheduleRecord> periodValidator;
        private readonly IMapperWrapper mapperWrapper;
        private readonly IWorkerScheduleConverter workerScheduleConverter;
    }
}