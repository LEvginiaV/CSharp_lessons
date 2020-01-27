using System;
using System.Threading.Tasks;

using Market.CustomersAndStaff.Models.Calendar;
using Market.CustomersAndStaff.Models.Validations;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Utils.Extensions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Market.CustomersAndStaff.ServiceApi.Controllers
{
    [ApiController]
    [Route("{shopId}/workersSchedules")]
    public class WorkersSchedulesController : ControllerBase
    {
        public WorkersSchedulesController(ICalendarRepository<WorkerScheduleRecord> workerScheduleRepository)
        {
            this.workerScheduleRepository = workerScheduleRepository;
        }

        [HttpGet("{from:datetime}/{to:datetime}")]
        [ProducesResponseType(typeof(ShopCalendarRange<WorkerScheduleRecord>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Get([FromRoute] Guid shopId, [FromRoute] DateTime from, [FromRoute] DateTime to)
        {
            if(from > to)
            {
                return BadRequest(new ValidationResult(false, "wrong to date", 
                                                       "To date must be greater than from date"));
            }

            if(DateHelper.GetMonthDifference(from, to) > maxMonthRange)
            {
                return BadRequest(new ValidationResult(false, "unsupported period",
                                                       $"Period should be less than or equal to {maxMonthRange} months"));
            }

            var calendarRange = await workerScheduleRepository.ReadShopCalendarRangeAsync(shopId, from, to);
            return Ok(calendarRange);
        }

        private readonly ICalendarRepository<WorkerScheduleRecord> workerScheduleRepository;
        private const int maxMonthRange = 3;
    }
}