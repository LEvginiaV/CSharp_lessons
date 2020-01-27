using System;
using System.Threading.Tasks;

using Market.CustomersAndStaff.Models.Calendar;
using Market.CustomersAndStaff.Models.ServiceCalendar;
using Market.CustomersAndStaff.ModelValidators.Periods;
using Market.CustomersAndStaff.Services.ServiceCalendar;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Market.CustomersAndStaff.ServiceApi.Controllers
{
    [ApiController]
    [Route("shops/{shopId:guid}/calendar")]
    public class ServiceCalendarController : ControllerBase
    {
        public ServiceCalendarController(
            ICalendarService calendarService,
            IPeriodValidator<ServiceCalendarRecord> periodValidator)
        {
            this.calendarService = calendarService;
            this.periodValidator = periodValidator;
        }
        
        [HttpPost("{date:datetime}/workers/{workerId:guid}/records")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateRecord([FromRoute] Guid shopId, [FromRoute] DateTime date, [FromRoute] Guid workerId, [FromBody] ServiceCalendarRecord record)
        {
            var periodValidation = periodValidator.Validate(record);

            if(!periodValidation.IsSuccess)
                return BadRequest($"{periodValidation.ErrorType} - {periodValidation.ErrorDescription}");

            var (recordId, validationResult) = await calendarService.CreateAsync(shopId, date, workerId, record);

            if(!validationResult.IsSuccess)
                return BadRequest($"{validationResult.ErrorType} - {validationResult.ErrorDescription}");

            return Ok(recordId);
        }
        
        [HttpGet("{date:datetime}")]
        [ProducesResponseType(typeof(ShopCalendarDay<ServiceCalendarRecord>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetShopDay([FromRoute] Guid shopId, [FromRoute] DateTime date)
        {
            var shopCalendarDay = await calendarService.ReadShopCalendarDayAsync(shopId, date);
            return Ok(shopCalendarDay);
        }

        private readonly ICalendarService calendarService;
        private readonly IPeriodValidator<ServiceCalendarRecord> periodValidator;
    }
}