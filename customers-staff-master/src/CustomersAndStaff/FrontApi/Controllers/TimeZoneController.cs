using System;
using System.Linq;
using System.Threading.Tasks;

using Market.Api.Models.Shops;
using Market.CustomersAndStaff.FrontApi.Converters.Mappers;
using Market.CustomersAndStaff.FrontApi.Dto.TimeZones;
using Market.CustomersAndStaff.Models.TimeZones;
using Market.CustomersAndStaff.Repositories.Interface;

using Microsoft.AspNetCore.Mvc;

namespace Market.CustomersAndStaff.FrontApi.Controllers
{
    [Route("shops/{shopId:guid}/timezone")]
    public class TimeZoneController : ControllerBase, IShopController
    {
        public TimeZoneController(ITimeZoneSettingsRepository timeZoneSettingsRepository, IMapperWrapper mapperWrapper)
        {
            this.timeZoneSettingsRepository = timeZoneSettingsRepository;
            this.mapperWrapper = mapperWrapper;
        }

        [HttpPost]
        public async Task<IActionResult> Set([FromQuery] Guid timeZoneId)
        {
            if(TimeZoneList.TimeZones.All(x => x.Id != timeZoneId))
            {
                return NotFound();
            }
            await timeZoneSettingsRepository.SetAsync(Shop.Id, timeZoneId);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var timeZoneId = await timeZoneSettingsRepository.GetAsync(Shop.Id);
            var timeZone = TimeZoneList.TimeZones.FirstOrDefault(x => x.Id == timeZoneId);
            return Ok(timeZone);
        }

        [HttpGet("list")]
        public IActionResult GetList()
        {
            return Ok(mapperWrapper.Map<TimeZoneDto[]>(TimeZoneList.TimeZones));
        }

        public Shop Shop { get; set; }

        private readonly ITimeZoneSettingsRepository timeZoneSettingsRepository;
        private readonly IMapperWrapper mapperWrapper;
    }
}