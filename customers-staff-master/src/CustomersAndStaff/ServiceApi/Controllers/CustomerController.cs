using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.Models.Validations;
using Market.CustomersAndStaff.ModelValidators;
using Market.CustomersAndStaff.Repositories.Interface;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Market.CustomersAndStaff.ServiceApi.Controllers
{
    [ApiController]
    [Route("{organizationId}/customer")]
    public class CustomerController : ControllerBase
    {
        public CustomerController(ICustomerRepository customerRepository, IMapper mapper, IValidator<Customer> validator)
        {
            repository = customerRepository;
            this.mapper = mapper;
            this.validator = validator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Customer), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateAsync([FromRoute] Guid organizationId, [FromBody] Customer customer)
        {
            var validation = validator.Validate(customer);
            if(!validation.IsSuccess)
            {
                return BadRequest(validation);
            }

            return Ok(await repository.CreateAsync(organizationId, customer));
        }

        [HttpPost("batch")]
        [ProducesResponseType(typeof(Customer[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateManyAsync([FromRoute] Guid organizationId, [FromBody] Customer[] customers)
        {
            var validation = customers.Select(x => validator.Validate(x)).FirstOrDefault(x => !x.IsSuccess);
            if(validation != null)
            {
                return BadRequest(validation);
            }

            return Ok(await repository.CreateManyAsync(organizationId, customers));
        }

        [HttpPost("{customerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationResult), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateAsync([FromRoute] Guid organizationId, [FromRoute] Guid customerId, [FromBody] Customer customer)
        {
            var validation = validator.Validate(customer);
            if(!validation.IsSuccess)
            {
                return BadRequest(validation);
            }

            try
            {
                await repository.UpdateAsync(organizationId, customerId, c => mapper.Map(customer, c));
            }
            catch(KeyNotFoundException)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpGet("{customerId}")]
        [ProducesResponseType(typeof(Customer), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ReadAsync([FromRoute] Guid organizationId, [FromRoute] Guid customerId)
        {
            var result = await repository.ReadAsync(organizationId, customerId);
            if(result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(Customer[]), StatusCodes.Status200OK)]
        public async Task<ActionResult> ReadByOrganizationAsync([FromRoute] Guid organizationId, [FromQuery] bool includeDeleted = false)
        {
            var result = await repository.ReadByOrganizationAsync(organizationId, includeDeleted);
            return Ok(result);
        }

        [HttpGet("version")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetVersionAsync([FromRoute] Guid organizationId)
        {
            var result = await repository.GetVersionAsync(organizationId);
            return Ok(result);
        }

        private readonly ICustomerRepository repository;
        private readonly IMapper mapper;
        private readonly IValidator<Customer> validator;
    }
}