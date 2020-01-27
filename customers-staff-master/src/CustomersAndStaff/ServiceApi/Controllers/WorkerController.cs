using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Market.CustomersAndStaff.Models.Validations;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.ModelValidators;
using Market.CustomersAndStaff.Repositories.Interface;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Market.CustomersAndStaff.ServiceApi.Controllers
{
    [ApiController]
    [Route("{organizationId}/worker")]
    public class WorkerController : ControllerBase
    {
        public WorkerController(IWorkerRepository workerRepository, IMapper mapper, IValidator<Worker> validator)
        {
            repository = workerRepository;
            this.mapper = mapper;
            this.validator = validator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Worker), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateAsync([FromRoute] Guid organizationId, [FromBody] Worker worker)
        {
            var validation = validator.Validate(worker);
            if(!validation.IsSuccess)
            {
                return BadRequest(validation);
            }

            return Ok(await repository.CreateAsync(organizationId, worker));
        }

        [HttpPost("batch")]
        [ProducesResponseType(typeof(Worker[]), StatusCodes.Status200OK)]
        public async Task<ActionResult> CreateManyAsync([FromRoute] Guid organizationId, [FromBody] Worker[] workers)
        {
            var validation = workers.Select(x => validator.Validate(x)).FirstOrDefault(x => !x.IsSuccess);
            if(validation != null)
            {
                return BadRequest(validation);
            }

            return Ok(await repository.CreateManyAsync(organizationId, workers));
        }

        [HttpPost("{workerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateAsync([FromRoute] Guid organizationId, [FromRoute] Guid workerId, [FromBody] Worker worker)
        {
            var validation = validator.Validate(worker);
            if(!validation.IsSuccess)
            {
                return BadRequest(validation);
            }

            try
            {
                await repository.UpdateAsync(organizationId, workerId, w => mapper.Map(worker, w));
            }
            catch(KeyNotFoundException)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpGet("{workerId}")]
        [ProducesResponseType(typeof(Worker), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ReadAsync([FromRoute] Guid organizationId, [FromRoute] Guid workerId)
        {
            var result = await repository.ReadAsync(organizationId, workerId);
            if(result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(Worker[]), StatusCodes.Status200OK)]
        public async Task<ActionResult> ReadByOrganizationAsync([FromRoute] Guid organizationId, [FromQuery] bool includeDeleted = false)
        {
            var result = await repository.ReadByShopAsync(organizationId, includeDeleted);
            return Ok(result);
        }

        [HttpGet("version")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetVersionAsync([FromRoute] Guid organizationId)
        {
            var result = await repository.GetVersionAsync(organizationId);
            return Ok(result);
        }

        private readonly IWorkerRepository repository;
        private readonly IMapper mapper;
        private readonly IValidator<Worker> validator;
    }
}