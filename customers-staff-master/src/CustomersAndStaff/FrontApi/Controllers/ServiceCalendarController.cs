using System;
using System.Threading.Tasks;

using Market.Api.Models.Shops;
using Market.CustomersAndStaff.FrontApi.Converters.Mappers;
using Market.CustomersAndStaff.FrontApi.Dto.Calendars;
using Market.CustomersAndStaff.FrontApi.Dto.Common;
using Market.CustomersAndStaff.Models.ServiceCalendar;
using Market.CustomersAndStaff.ModelValidators;
using Market.CustomersAndStaff.ModelValidators.Periods;
using Market.CustomersAndStaff.Services.ServiceCalendar;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Market.CustomersAndStaff.FrontApi.Controllers
{
    [ApiController]
    [Route("shops/{shopId:guid}/calendar")]
    public class ServiceCalendarController : ControllerBase, IShopController
    {
        public ServiceCalendarController(
            ICalendarService calendarService,
            IMapperWrapper mapperWrapper
            )
        {
            this.calendarService = calendarService;
            this.mapperWrapper = mapperWrapper;
        }

        [HttpPost("{date:datetime}/workers/{workerId:guid}/records")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationResultDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateRecord([FromRoute] DateTime date, [FromRoute] Guid workerId, [FromBody] ServiceCalendarRecordInfoDto recordDto)
        {
            var record = mapperWrapper.Map<ServiceCalendarRecord>(recordDto);

            var (recordId, validationResult) = await calendarService.CreateAsync(Shop.Id, date, workerId, record);

            if(!validationResult.IsSuccess)
                return BadRequest(mapperWrapper.Map<ValidationResultDto>(validationResult));

            return Ok(recordId);
        }

        [HttpPut("{date:datetime}/workers/{workerId:guid}/records/{recordId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationResultDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateRecord([FromRoute] DateTime date, [FromRoute] Guid workerId, [FromRoute] Guid recordId, [FromBody] ServiceCalendarRecordUpdateDto recordDto)
        {
            var record = mapperWrapper.Map<ServiceCalendarRecord>(recordDto);

            record.Id = recordId;
            var validationResult = await calendarService.UpdateAsync(Shop.Id, date, workerId, record, recordDto.UpdatedDate, recordDto.UpdatedWorkerId);
            if(!validationResult.IsSuccess)
                return BadRequest(mapperWrapper.Map<ValidationResultDto>(validationResult));

            return Ok();
        }

        [HttpDelete("{date:datetime}/workers/{workerId:guid}/records/{recordId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationResultDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteRecord([FromRoute] DateTime date, [FromRoute] Guid workerId, [FromRoute] Guid recordId)
        {
            var validationResult = await calendarService.RemoveAsync(Shop.Id, date, workerId, recordId);
            if(validationResult.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(mapperWrapper.Map<ValidationResultDto>(validationResult));
        }

        [HttpPut("{date:datetime}/workers/{workerId:guid}/records/{recordId:guid}/updateCustomerStatus")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationResultDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateCustomerStatusRecord([FromRoute] DateTime date, [FromRoute] Guid workerId, [FromRoute] Guid recordId, [FromQuery] CustomerStatusDto newStatus)
        {
            var validationResult = await calendarService.UpdateCustomerStatusAsync(Shop.Id, date, workerId, recordId, mapperWrapper.Map<CustomerStatus>(newStatus));
            if(validationResult.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(mapperWrapper.Map<ValidationResultDto>(validationResult));
        }

        [HttpGet("{date:datetime}")]
        [ProducesResponseType(typeof(ShopServiceCalendarDayDto), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetShopDay([FromRoute] DateTime date, [FromQuery] RecordStatusDto? status = null)
        {
            var shopCalendarDay = await calendarService.ReadShopCalendarDayAsync(Shop.Id, date, mapperWrapper.Map<RecordStatus>(status));
            return Ok(mapperWrapper.Map<ShopServiceCalendarDayDto>(shopCalendarDay));
        }
        
        public Shop Shop { get; set; }

        private readonly ICalendarService calendarService;
        private readonly IMapperWrapper mapperWrapper;
    }
}