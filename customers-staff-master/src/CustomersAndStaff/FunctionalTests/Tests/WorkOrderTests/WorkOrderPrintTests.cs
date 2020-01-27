using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;
using FluentAssertions.Extensions;

using GroboContainer.NUnitExtensions;

using Market.Api.Models.Products;
using Market.CustomersAndStaff.FunctionalTests.ComponentExtensions;
using Market.CustomersAndStaff.FunctionalTests.Components.Pages.WorkOrders;
using Market.CustomersAndStaff.FunctionalTests.Helpers;
using Market.CustomersAndStaff.FunctionalTests.Helpers.Products;
using Market.CustomersAndStaff.FunctionalTests.Helpers.Workers;
using Market.CustomersAndStaff.FunctionalTests.Helpers.WorkOrders;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.Models.WorkOrders;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.WorkOrderTests
{
    public class WorkOrderPrintTests : WorkOrderTestBase
    {
        [Test, Description("Проверка содержимого docx ЗН и Квитанции")]
        public async Task ExcelDownloadTest()
        {
            var customer = await CreateDefaultCustomer();
            var worker1 = await workerHelper.CreateAsync("Приёмщик по телефону");
            var worker2 = await workerHelper.CreateAsync("Пьяный механик");
            var service1 = await productHelper.CreateServiceAsync("Накачать шины азотом", 150);
            var service2 = await productHelper.CreateServiceAsync("Прокатило", 999);
            var product1 = await productHelper.CreateProductAsync("Азот", 25, ProductUnit.Liter);
            var product2 = await productHelper.CreateProductAsync("Ничего", 250, ProductUnit.Piece);

            var orderDate = new DateTime(2019, 06, 11, 0, 0, 0, DateTimeKind.Utc);
            await workOrderHelper.CreateOrderAsync(customer,
                                                   builder => builder
                                                              .WithNumber(new WorkOrderNumber("АА", 1))
                                                              .WithStatus(WorkOrderStatus.New)
                                                              .WithReceptionDate(orderDate)
                                                              .WithShopPhone("78007006050")
                                                              .WithReceptionWorker(worker1)
                                                              .WithWarrantyNumber("123ыюя456asd")
                                                              .WithCompletionDatePlanned(orderDate + 1.Days())
                                                              .AddShopServiceFromMarketProduct(service1, worker2, 4)
                                                              .AddShopServiceFromMarketProduct(service2, quantity : 1)
                                                              .AddShopProductFromMarketProduct(product1, 0.75m)
                                                              .AddShopProductFromMarketProduct(product2, 1)
                                                              .AddCustomerProduct("Ёлочка-вонючка", "ящик")
                                                              .WithVehicleCustomerValue(builder2 => builder2
                                                                                                    .WithBrand("Маленькая")
                                                                                                    .WithModel("Красненькая")
                                                                                                    .WithAdditionalInfo("Машинка")
                                                                                                    .WithRegisterSign("123ВУASD")
                                                                                                    .WithVin("JT2EL46D0P0308478")
                                                                                                    .WithBodyNumber("Number1")
                                                                                                    .WithEngineNumber("Number2")
                                                                                                    .WithYear(1901))
                                                              .WithAdditionalText("Машина норм, а девочка на любителя."));

            var workOrderListPage = LoadWorkOrderList();
            workOrderListPage.NotIssuedOrders.WorkOrderItem.First().Container.Click();

            var workOrderViewPage = workOrderListPage.GoToPage<WorkOrderViewPage>();
            workOrderViewPage.PrintLink.Click();
            workOrderViewPage.PrintLink.Disabled.Wait().EqualTo(false);

            var expectedFilesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tests", "WorkOrderTests", "Files");

            DocxComparer.CompareDocxFiles(
                Path.Combine(ChromeDriverFactory.DownloadPath, $"Квитанция к заказ-наряду_№АА-000001_от_{orderDate:dd.MM.yyyy}.docx"),
                Path.Combine(expectedFilesPath, "Квитанция к заказ-наряду.docx"));
            DocxComparer.CompareDocxFiles(
                Path.Combine(ChromeDriverFactory.DownloadPath, $"Заказ-наряд_№АА-000001_от_{orderDate:dd.MM.yyyy}.docx"),
                Path.Combine(expectedFilesPath, "Заказ-наряд.docx"));
        }

        [Test, Description("Сохранение при печати")]
        public async Task SaveOnPrintTest()
        {
            await CreateDefaultWorkOrder();

            await productHelper.CreateServiceAsync("Услуга", 1);
            await productHelper.CreateProductAsync("Товар", 1);

            var workOrderListPage = LoadWorkOrderList();
            workOrderListPage.NotIssuedOrders.WorkOrderItem.First().Container.Click();
            var workOrderViewPage = workOrderListPage.GoToPage<WorkOrderViewPage>();

            workOrderViewPage.ServicesBlock.SpoilerCaption.Click();
            workOrderViewPage.ServicesBlock.RowItem.First().CardNameSelector.TypeAndSelect("Услуга");
            workOrderViewPage.ServicesBlock.RowItem.First().Quantity.SetRawValue("1");

            workOrderViewPage.ProductsBlock.SpoilerCaption.Click();
            workOrderViewPage.ProductsBlock.RowItem.First().CardNameSelector.TypeAndSelect("Товар");
            workOrderViewPage.ProductsBlock.RowItem.First().Quantity.SetRawValue("1");

            workOrderViewPage.PrintLink.Click();
            workOrderViewPage.PrintLink.Disabled.Wait().EqualTo(false);

            var workOrder = await workOrderHelper.ReadSingleAsync();
            workOrder.ShopServices.Length.Should().Be(1);
            workOrder.ShopProducts.Length.Should().Be(1);
        }

        [Injected]
        private IWorkOrderHelper workOrderHelper;

        [Injected]
        private IProductHelper productHelper;

        [Injected]
        private IWorkerHelper workerHelper;
    }
}