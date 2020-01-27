using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Market.Api.Models.Shops;
using Market.CustomersAndStaff.FrontApi.Converters.Mappers;
using Market.CustomersAndStaff.FrontApi.Dto.Common;
using Market.CustomersAndStaff.FrontApi.Dto.Workers;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.ModelValidators;
using Market.CustomersAndStaff.Repositories.Interface;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Market.CustomersAndStaff.FrontApi.Controllers
{
    [ApiController]
    [Route("shops/{shopId:guid}/workers")]
    public class WorkersController : ControllerBase, IShopController
    {
        public WorkersController(
            IWorkerRepository workerRepository,
            IMapper mapper,
            IValidator<Worker> validator,
            IMapperWrapper mapperWrapper)
        {
            this.workerRepository = workerRepository;
            this.mapper = mapper;
            this.validator = validator;
            this.mapperWrapper = mapperWrapper;
        }

        [HttpPost]
        [ProducesResponseType(typeof(WorkerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationResultDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateAsync([FromBody] WorkerInfoDto workerInfoDto)
        {
            var worker = mapperWrapper.Map<Worker>(workerInfoDto);
            var validation = validator.Validate(worker);
            if(!validation.IsSuccess)
            {
                return BadRequest(mapperWrapper.Map<ValidationResultDto>(validation));
            }

            var createdWorker = await workerRepository.CreateAsync(Shop.Id, worker);
            var result = mapperWrapper.Map<WorkerDto>(createdWorker);
            return Ok(result);
        }

        [HttpPut("{workerId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationResultDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateAsync([FromRoute] Guid workerId, [FromBody] WorkerInfoDto workerInfoDto)
        {
            try
            {
                var worker = mapperWrapper.Map<Worker>(workerInfoDto);
                var validation = validator.Validate(worker);
                if(!validation.IsSuccess)
                {
                    return BadRequest(mapperWrapper.Map<ValidationResultDto>(validation));
                }

                await workerRepository.UpdateAsync(Shop.Id, workerId, w => mapper.Map(worker, w));
            }
            catch(KeyNotFoundException)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPut("{workerId:guid}/comment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationResultDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateCommentAsync([FromRoute] Guid workerId, [FromBody] CommentDto commentDto)
        {
            var comment = commentDto.Comment;
            try
            {
                if(comment != null && comment.Length > 500)
                {
                    return BadRequest(new ValidationResultDto {ErrorType = "comment", ErrorDescription = "Expected length <= 500"});
                }

                await workerRepository.UpdateAsync(Shop.Id, workerId, w => w.AdditionalInfo = comment);
            }
            catch(KeyNotFoundException)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpGet("{workerId:guid}")]
        [ProducesResponseType(typeof(WorkerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ReadAsync([FromRoute] Guid workerId)
        {
            var worker = await workerRepository.ReadAsync(Shop.Id, workerId);
            if(worker == null)
                return NotFound();
            var result = mapperWrapper.Map<WorkerDto>(worker);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(WorkerListDto), StatusCodes.Status200OK)]
        public async Task<ActionResult> ReadAllAsync([FromQuery] int? version = null)
        {
            var currentVersion = await workerRepository.GetVersionAsync(Shop.Id);

            if(version != null && currentVersion == version)
            {
                return Ok(new WorkerListDto {Version = currentVersion, Workers = null});
            }

            var workers = await workerRepository.ReadByShopAsync(Shop.Id);
            var result = workers.Select(x => mapperWrapper.Map<WorkerDto>(x)).ToArray();
            return Ok(new WorkerListDto
                {
                    Workers = result,
                    Version = currentVersion,
                });
        }

        [HttpDelete("{workerId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteAsync([FromRoute] Guid workerId)
        {
            try
            {
                await workerRepository.UpdateAsync(Shop.Id, workerId, c => c.IsDeleted = true);
            }
            catch(KeyNotFoundException)
            {
                return NotFound();
            }

            return Ok();
        }

        public Shop Shop { get; set; }

        private readonly IWorkerRepository workerRepository;
        private readonly IMapper mapper;
        private readonly IValidator<Worker> validator;
        private readonly IMapperWrapper mapperWrapper;
    }
}