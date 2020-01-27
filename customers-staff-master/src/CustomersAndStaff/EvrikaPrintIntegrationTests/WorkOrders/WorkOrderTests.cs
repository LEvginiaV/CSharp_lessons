using System;
using System.IO;
using System.Threading.Tasks;

using GroboContainer.NUnitExtensions;

using Market.Api.Models.Organizations;
using Market.Api.Models.Products;
using Market.Api.Models.Shops;
using Market.CustomersAndStaff.EvrikaPrintClient.Client;
using Market.CustomersAndStaff.EvrikaPrintClient.Models;
using Market.CustomersAndStaff.FrontApi.Converters.WorkOrders;
using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.Models.WorkOrders;
using Market.CustomersAndStaff.Tests.Core.Configuration;

using NUnit.Framework;

namespace Market.CustomersAndStaff.EvrikaPrintIntegrationTests.WorkOrders
{
    [WithPortal]
    public class WorkOrderTests : IMainSuite
    {
        [Test, Explicit]
        public async Task SimpleTest()
        {
            var clientId = Guid.NewGuid();
            var workerId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var serviceId = Guid.NewGuid();
            var date = DateTime.UtcNow.Date;

            var workOrder = new WorkOrder
                {
                    Number = new WorkOrderNumber("ХХ", 123456),
                    ClientId = clientId,
                    ReceptionDate = date,
                    CompletionDatePlanned = date,
                    WarrantyNumber = "abc",
                    ReceptionWorkerId = workerId,
                    ShopRequisites = new ShopRequisites
                        {
                            Phone = "79112223344",
                        },
                    CustomerValues = new CustomerValueList
                        {
                            CustomerValueType = CustomerValueType.Vehicle,
                            CustomerValues = new BaseCustomerValue[]
                                {
                                    new VehicleCustomerValue
                                        {
                                            Brand = "abc",
                                            Model = "def",
                                            Year = 2007,
                                            RegisterSign = "GHI123",
                                            BodyNumber = "jkl",
                                            EngineNumber = "mno",
                                            Vin = "pqrstuvw123456789",
                                            AdditionalInfo = "Тачка просто в хлам",
                                        }
                                }
                        },
                    ShopServices = new[]
                        {
                            new ShopService
                                {
                                    ProductId = serviceId,
                                    WorkerId = workerId,
                                    Price = 101m,
                                    Quantity = 2m,
                                },
                        },
                    ShopProducts = new[]
                        {
                            new ShopProduct
                                {
                                    ProductId = productId,
                                    Price = 20m,
                                    Quantity = 31m,
                                },
                        },
                    CustomerProducts = new[]
                        {
                            new CustomerProduct
                                {
                                    Name = "Какая-то хрень",
                                    Quantity = "2 шт",
                                },
                        },
                    AdditionalText = "Без гарантии",
                };

            var products = new[]
                {
                    new Product
                        {
                            Id = serviceId,
                            Name = "Чо-то подкрасить",
                            Nomenclature = 2,
                        },
                    new Product
                        {
                            Id = productId,
                            Name = "Краска",
                            Nomenclature = 3,
                            ProductUnit = ProductUnit.RunningMeter,
                        },
                };
            var client = new Customer
                {
                    Id = clientId,
                    Name = "Вася",
                    Phone = "79998887766",
                    AdditionalInfo = "ул. Пушкина",
                };

            var worker = new Worker
                {
                    Id = workerId,
                    FullName = "Рабочий",
                };

            var shop = new Shop
                {
                    Name = "Мега-мастерская",
                    Address = "В гаражах",
                };

            var organization = new Organization
                {
                    Inn = "1234567890",
                };

            var printOrder = workOrderPrintConverter.Convert(workOrder, shop, organization, client, products, new[] {worker}, false);
            var taskId = await evrikaPrinterClient.CreatePrintTaskAsync(new PrintTask
                {
                    TemplateId = PrinterTemplateIds.WorkOrder,
                    OutputFormat = PrintOutputFormat.Word,
                    Data = printOrder,
                });

            Console.WriteLine($"taskId: {taskId}");

            for(int i = 0; i < 10; i++)
            {
                await Task.Delay(1000);
                var info = await evrikaPrinterClient.GetTaskInfoAsync(taskId);
                if(info.Status == PrintTaskStatus.Failed)
                {
                    Console.WriteLine($"Fail: {info.ErrorMessage}");
                    Assert.Fail();
                }
                else if(info.Status == PrintTaskStatus.Complete)
                {
                    var result = await evrikaPrinterClient.GetTaskResultAsync(taskId);
                    File.WriteAllBytes(@"C:\temp\order.docx", result);
                    return;
                }
            }

            Assert.Fail();
        }

        [Injected]
        private IWorkOrderPrintConverter workOrderPrintConverter;

        [Injected]
        private IEvrikaPrinterClient evrikaPrinterClient;
    }
}