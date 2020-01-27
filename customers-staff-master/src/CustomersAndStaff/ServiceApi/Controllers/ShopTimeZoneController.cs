using System;
using System.Linq;
using System.Threading.Tasks;

using Market.CustomersAndStaff.Models.TimeZones;
using Market.CustomersAndStaff.Repositories.Interface;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Market.CustomersAndStaff.ServiceApi.Controllers
{
    [ApiController]
    [Route("{shopId}/timeZone")]
    public class ShopTimeZoneController : ControllerBase
    {
        public ShopTimeZoneController(ITimeZoneSettingsRepository timeZoneSettingsRepository)
        {
            this.timeZoneSettingsRepository = timeZoneSettingsRepository;
        }

        [HttpGet]
        [ProducesResponseType(typeof(TimeSpan?), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute] Guid shopId)
        {
            var timeZoneId = await timeZoneSettingsRepository.GetAsync(shopId);
            var timeZone = TimeZoneList.TimeZones.FirstOrDefault(x => x.Id == timeZoneId);
            return Ok(timeZone?.Offset);
        }

        private readonly ITimeZoneSettingsRepository timeZoneSettingsRepository;
    }
}