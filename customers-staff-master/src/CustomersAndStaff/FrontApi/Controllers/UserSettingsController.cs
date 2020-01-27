using System.Collections.Generic;
using System.Threading.Tasks;

using Market.CustomersAndStaff.Repositories.Interface;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Portal.Requisites;

namespace Market.CustomersAndStaff.FrontApi.Controllers
{
    [ApiController]
    [Route("user/settings")]
    public class UserSettingsController : ControllerBase, IUserController
    {
        public UserSettingsController(IUserSettingsRepository userSettingsRepository)
        {
            this.userSettingsRepository = userSettingsRepository;
        }

        [HttpGet]
        [ProducesResponseType(typeof(Dictionary<string, string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            return Ok(await userSettingsRepository.ReadAsync(AuthUser.Id));
        }

        [HttpPut("{key}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromRoute] string key, [FromQuery] string value)
        {
            await userSettingsRepository.UpdateAsync(AuthUser.Id, key, value);
            return Ok();
        }

        public User AuthUser { get; set; }

        private readonly IUserSettingsRepository userSettingsRepository;
    }
}