using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Market.Api.Models.Shops;
using Market.CustomersAndStaff.FrontApi.Converters.Mappers;
using Market.CustomersAndStaff.FrontApi.Dto.Common;
using Market.CustomersAndStaff.FrontApi.Dto.Customers;
using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.ModelValidators;
using Market.CustomersAndStaff.Repositories.Interface;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Market.CustomersAndStaff.FrontApi.Controllers
{
    [ApiController]
    [Route("shops/{shopId:guid}/customers")]
    public class CustomersController : ControllerBase, IShopController
    {
        public CustomersController(
            ICustomerRepository customerRepository,
            IMapper mapper,
            IValidator<Customer> validator,
            IMapperWrapper mapperWrapper)
        {
            this.customerRepository = customerRepository;
            this.mapper = mapper;
            this.validator = validator;
            this.mapperWrapper = mapperWrapper;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationResultDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateAsync([FromBody] CustomerInfoDto customerInfoDto)
        {
            var customer = mapperWrapper.Map<Customer>(customerInfoDto);
            var validation = validator.Validate(customer);
            if(!validation.IsSuccess)
            {
                return BadRequest(mapperWrapper.Map<ValidationResultDto>(validation));
            }

            var createdCustomer = await customerRepository.CreateAsync(Shop.OrganizationId, customer);
            var result = mapperWrapper.Map<CustomerDto>(createdCustomer);
            return Ok(result);
        }

        [HttpPut("{customerId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationResultDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateAsync([FromRoute] Guid customerId, [FromBody] CustomerInfoDto customerInfoDto)
        {
            try
            {
                var customer = mapperWrapper.Map<Customer>(customerInfoDto);
                var validation = validator.Validate(customer);
                if(!validation.IsSuccess)
                {
                    return BadRequest(mapperWrapper.Map<ValidationResultDto>(validation));
                }

                await customerRepository.UpdateAsync(Shop.OrganizationId, customerId, c => mapper.Map(customer, c));
            }
            catch(KeyNotFoundException)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPut("{customerId:guid}/comment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationResultDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateCommentAsync([FromRoute] Guid customerId, [FromBody] CommentDto commentDto)
        {
            var comment = commentDto.Comment;
            try
            {
                if(comment != null && comment.Length > 500)
                {
                    return BadRequest(new ValidationResultDto {ErrorType = "comment", ErrorDescription = "Expected length <= 500"});
                }

                await customerRepository.UpdateAsync(Shop.OrganizationId, customerId, c => c.AdditionalInfo = comment);
            }
            catch(KeyNotFoundException)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpGet("{customerId:guid}")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ReadAsync([FromRoute] Guid customerId)
        {
            var customer = await customerRepository.ReadAsync(Shop.OrganizationId, customerId);
            if(customer == null)
                return NotFound();
            var result = mapperWrapper.Map<CustomerDto>(customer);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(CustomerListDto), StatusCodes.Status200OK)]
        public async Task<ActionResult> ReadAllAsync([FromQuery] int? version = null)
        {
            var currentVersion = await customerRepository.GetVersionAsync(Shop.OrganizationId);

            if(version != null && currentVersion == version)
            {
                return Ok(new CustomerListDto {Version = currentVersion, Customers = null});
            }

            var customers = await customerRepository.ReadByOrganizationAsync(Shop.OrganizationId);
            var result = customers.Select(x => mapperWrapper.Map<CustomerDto>(x)).ToArray();
            return Ok(new CustomerListDto
                {
                    Customers = result,
                    Version = currentVersion,
                });
        }

        public Shop Shop { get; set; }

        private readonly ICustomerRepository customerRepository;
        private readonly IMapper mapper;
        private readonly IValidator<Customer> validator;
        private readonly IMapperWrapper mapperWrapper;
    }
}