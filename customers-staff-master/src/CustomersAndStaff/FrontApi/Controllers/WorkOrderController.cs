using System;
using System.Threading.Tasks;

using Market.Api.Client;
using Market.Api.Models.Shops;
using Market.CustomersAndStaff.EvrikaPrintClient.Client;
using Market.CustomersAndStaff.EvrikaPrintClient.Models;
using Market.CustomersAndStaff.FrontApi.Converters.Mappers;
using Market.CustomersAndStaff.FrontApi.Converters.WorkOrders;
using Market.CustomersAndStaff.FrontApi.Dto.Common;
using Market.CustomersAndStaff.FrontApi.Dto.Print;
using Market.CustomersAndStaff.FrontApi.Dto.WorkOrders;
using Market.CustomersAndStaff.Models.WorkOrders;
using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Services.WorkOrders;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Market.CustomersAndStaff.FrontApi.Controllers
{
    [Route("shops/{shopId:guid}/workOrders")]
    public class WorkOrderController : ControllerBase, IShopController
    {
        public WorkOrderController(IWorkOrderService workOrderService, IMapperWrapper mapperWrapper, IWorkOrderPrintConverter workOrderPrintConverter, IMarketApiClient marketApiClient, ICustomerRepository customerRepository, IWorkerRepository workerRepository, IEvrikaPrinterClient evrikaPrinterClient)
        {
            this.workOrderService = workOrderService;
            this.mapperWrapper = mapperWrapper;
            this.workOrderPrintConverter = workOrderPrintConverter;
            this.marketApiClient = marketApiClient;
            this.customerRepository = customerRepository;
            this.workerRepository = workerRepository;
            this.evrikaPrinterClient = evrikaPrinterClient;
        }

        public Shop Shop { get; set; }

        [HttpPost("generateNumber")]
        [ProducesResponseType(typeof(WorkOrderCreateInfoDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GenerateWorkOrderNumber()
        {
            var createInfo = await workOrderService.GetCreateInfoAsync(Shop.Id);
            var createInfoDto = new WorkOrderCreateInfoDto
                {
                    OrderNumber = mapperWrapper.Map<WorkOrderNumberDto>(createInfo.OrderNumber),
                    AdditionalText = createInfo.AdditionalText,
                };
            return Ok(createInfoDto);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationResultDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrder([FromBody] WorkOrderDto workOrder)
        {
            var (id, validationResult) = await workOrderService.CreateNewOrderAsync(Shop.Id, mapperWrapper.Map<WorkOrder>(workOrder));
            if(validationResult.IsSuccess)
            {
                return Ok(id);
            }

            return BadRequest(mapperWrapper.Map<ValidationResultDto>(validationResult));
        }

        [HttpPut("{orderId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationResultDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateOrder([FromRoute] Guid orderId, [FromBody] WorkOrderDto workOrder)
        {
            var validationResult = await workOrderService.SaveOrderAsync(Shop.Id, orderId, mapperWrapper.Map<WorkOrder>(workOrder));

            if(validationResult.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(mapperWrapper.Map<ValidationResultDto>(validationResult));
        }

        [HttpPut("{orderId:guid}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationResultDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateOrderStatus([FromRoute] Guid orderId, [FromQuery] WorkOrderStatusDto workOrderStatus)
        {
            var validationResult = await workOrderService.UpdateStatus(Shop.Id, orderId, mapperWrapper.Map<WorkOrderStatus>(workOrderStatus));

            if(validationResult.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(mapperWrapper.Map<ValidationResultDto>(validationResult));
        }

        [HttpGet("{orderId:guid}")]
        [ProducesResponseType(typeof(WorkOrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReadOrder([FromRoute] Guid orderId)
        {
            var order = await workOrderService.ReadOrderAsync(Shop.Id, orderId);
            if(order == null)
            {
                return NotFound();
            }

            return Ok(mapperWrapper.Map<WorkOrderDto>(order));
        }

        [HttpDelete("{orderId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveOrder([FromRoute] Guid orderId)
        {
            var res = await workOrderService.RemoveOrderAsync(Shop.Id, orderId);

            if(res)
                return Ok();
            return NotFound();
        }

        [HttpGet]
        [ProducesResponseType(typeof(WorkOrderItemDto[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> ReadOrders()
        {
            return Ok(mapperWrapper.Map<WorkOrderItemDto[]>(await workOrderService.ReadOrderInfosAsync(Shop.Id)));
        }

        [HttpPost("{orderId:guid}/print")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreatePrintTask([FromRoute] Guid orderId, [FromQuery] bool invoice)
        {
            var order = await workOrderService.ReadOrderAsync(Shop.Id, orderId);
            if(order == null)
            {
                return NotFound();
            }

            var productsTask = marketApiClient.Products.GetAll(Shop.Id);
            var organizationTask = marketApiClient.Organizations.Get(Shop.OrganizationId);
            var customerTask = customerRepository.ReadAsync(Shop.OrganizationId, order.ClientId);
            var workersTask = workerRepository.ReadByShopAsync(Shop.Id);

            await Task.WhenAll(productsTask, organizationTask, customerTask, workersTask);

            var printOrder = workOrderPrintConverter.Convert(order, Shop, organizationTask.Result, customerTask.Result, productsTask.Result, workersTask.Result, invoice);
            var taskId = await evrikaPrinterClient.CreatePrintTaskAsync(new PrintTask
                {
                    OutputFormat = PrintOutputFormat.Word,
                    TemplateId = PrinterTemplateIds.WorkOrder,
                    Data = printOrder,
                });

            return Ok(taskId);
        }

        [HttpGet("printTasks/status/{taskId}")]
        [ProducesResponseType(typeof(PrintTaskInfoDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTaskStatus([FromRoute] string taskId)
        {
            return Ok(await evrikaPrinterClient.GetTaskInfoAsync(taskId));
        }

        [HttpGet("printTasks/file/{taskId}")]
        [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTaskFile(string taskId)
        {
            var result = await evrikaPrinterClient.GetTaskResultAsync(taskId);
            return Ok(result);
        }

        private readonly IWorkOrderService workOrderService;
        private readonly IMapperWrapper mapperWrapper;
        private readonly IWorkOrderPrintConverter workOrderPrintConverter;
        private readonly IMarketApiClient marketApiClient;
        private readonly ICustomerRepository customerRepository;
        private readonly IWorkerRepository workerRepository;
        private readonly IEvrikaPrinterClient evrikaPrinterClient;
    }
}