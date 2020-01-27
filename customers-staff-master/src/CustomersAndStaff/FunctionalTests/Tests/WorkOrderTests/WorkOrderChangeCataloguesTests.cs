using System.Linq;
using System.Threading.Tasks;

using GroboContainer.NUnitExtensions;

using Market.CustomersAndStaff.FunctionalTests.Helpers.Customers;
using Market.CustomersAndStaff.FunctionalTests.Helpers.Products;
using Market.CustomersAndStaff.FunctionalTests.Helpers.Workers;
using Market.CustomersAndStaff.FunctionalTests.Helpers.WorkOrders;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkOrderExtensions;
using Market.CustomersAndStaff.Models.Customers;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.WorkOrderTests
{
    public class WorkOrderChangeCataloguesTests : WorkOrderTestBase
    {
        [Test, Description("Изменение данных пользователя")]
        public async Task ChangeCustomerTest()
        {
            await CreateDefaultWorkOrder();

            var workOrderListPage = LoadWorkOrderList();
            var workOrderViewPage = workOrderListPage.GoToNotIssuedOrder();

            workOrderViewPage.ClientBlock.ChangeData.Click();
            workOrderViewPage.ClientBlock.NameInput.ResetRawValue("Новый клиент");
            workOrderViewPage.ClientBlock.PhoneInput.ResetValue("79998887766");
            workOrderViewPage.ClientBlock.Comment.ResetRawValue("Новый комментарий из ЗН");
            workOrderViewPage.SaveAndGoToList();

            await customerHelper.CheckSingleCustomer(new Customer {Name = "Новый клиент", Phone = "79998887766", AdditionalInfo = "Новый комментарий из ЗН"});
        }

        [Test, Description("Удаление данных")]
        public async Task RemovingData()
        {
            var customer = await CreateDefaultCustomer();
            var worker = await workerHelper.CreateAsync("Сотрудник для удаления");
            var service = await productHelper.CreateServiceAsync("Услуга для удаления", 100);
            var product = await productHelper.CreateProductAsync("Товар для удаления", 50);

            var order = WorkOrderBuilder.CreateWithCustomer(customer)
                                        .WithReceptionWorker(worker)
                                        .AddShopServiceFromMarketProduct(service, worker)
                                        .AddShopProductFromMarketProduct(product)
                                        .Build();

            await workOrderHelper.CreateOrderAsync(order);

            var workOrderListPage = LoadWorkOrderList();
            var workOrderViewPage = workOrderListPage.GoToNotIssuedOrder();

            workOrderViewPage.InfoBlock.Worker.Text.Wait().EqualTo("Сотрудник для удаления");
            workOrderViewPage.ServicesBlock.SpoilerCaption.Click();
            workOrderViewPage.ServicesBlock.RowItem.First().CheckRow("Услуга для удаления", 100, "1", 100, "Сотрудник для удаления");
            workOrderViewPage.ProductsBlock.SpoilerCaption.Click();
            workOrderViewPage.ProductsBlock.RowItem.First().CheckRow("Товар для удаления", 50, "1", 50);

            await productHelper.RemoveAsync(service);
            await productHelper.RemoveAsync(product);
            await workerHelper.RemoveAsync(worker.Id);

            RefreshPage();

            workOrderViewPage.WaitPresence();
            workOrderViewPage.InfoBlock.Worker.WaitAbsence();
            workOrderViewPage.ServicesBlock.SpoilerCaption.Click();
            workOrderViewPage.ServicesBlock.RowItem.First().CheckRow("?", 100, "1", 100);
            workOrderViewPage.ServicesBlock.RowItem.First().Worker.WaitAbsence();
            workOrderViewPage.ProductsBlock.SpoilerCaption.Click();
            workOrderViewPage.ProductsBlock.RowItem.First().CheckRow("?", 50, "1", 50);
        }
        
        [Injected]
        private readonly IWorkOrderHelper workOrderHelper;

        [Injected]
        private readonly IProductHelper productHelper;

        [Injected]
        private readonly IWorkerHelper workerHelper;

        [Injected]
        private readonly ICustomerHelper customerHelper;
    }
}