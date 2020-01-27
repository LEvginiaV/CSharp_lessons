using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Market.CustomersAndStaff.Repositories.Interface;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Market.CustomersAndStaff.ServiceApi.Controllers
{
    [ApiController]
    [Route("users/{userId}/settings")]
    public class UserSettingsController : ControllerBase
    {
        public UserSettingsController(IUserSettingsRepository userSettingsRepository)
        {
            this.userSettingsRepository = userSettingsRepository;
        }

        [HttpGet]
        [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute] Guid userId)
        {
            return Ok(await userSettingsRepository.ReadAsync(userId));
        }

        [HttpPut("{key}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromRoute] Guid userId, [FromRoute] string key, [FromQuery] string value)
        {
            await userSettingsRepository.UpdateAsync(userId, key, value);
            return Ok();
        }

        private readonly IUserSettingsRepository userSettingsRepository;
    }
}