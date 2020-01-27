using System;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions.Extensions;

using GroboContainer.NUnitExtensions;

using Market.Api.Models.Products;
using Market.CustomersAndStaff.FunctionalTests.ComponentExtensions;
using Market.CustomersAndStaff.FunctionalTests.Helpers.Products;
using Market.CustomersAndStaff.FunctionalTests.Helpers.WorkOrders;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkOrderExtensions;
using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.Models.WorkOrders;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.WorkOrderTests
{
    public class WorkOrderListTests : WorkOrderTestBase
    {
        [Test, Description("Смена статусов и отображение завершённых ЗН")]
        public async Task ChangeStatusTest()
        {
            await CreateDefaultWorkOrder();
            var customer = await CreateDefaultCustomer();
            var order1 = WorkOrderBuilder.CreateWithCustomer(customer).Build();
            var order2 = WorkOrderBuilder.CreateWithCustomer(customer).WithNumber(new WorkOrderNumber("ЯЯ", 999999)).Build();
            await workOrderHelper.CreateOrderAsync(order1);
            await workOrderHelper.CreateOrderAsync(order2);

            var workOrderListPage = LoadWorkOrderList();
            workOrderListPage.NotIssuedOrders.WaitItemNamesEquivalentTo("Заказ-наряд АА-000001", "Заказ-наряд ЯЯ-999999");
            workOrderListPage.IssuedOrders.WaitEmpty();

            workOrderListPage.NotIssuedOrders.WorkOrderItem.ElementAt(0).Status.SelectByValue(WorkOrderStatus.InProgress);
            var workOrderViewPage = workOrderListPage.GoToNotIssuedOrder(1);
            workOrderViewPage.HeaderBlock.WorkOrderStatus.SelectByValue(WorkOrderStatus.Completed);
            workOrderListPage = workOrderViewPage.SaveAndGoToList();
            workOrderListPage.NotIssuedOrders.WorkOrderItem.ElementAt(1).Status.WaitValue(WorkOrderStatus.Completed);
            workOrderListPage.NotIssuedOrders.WaitItemNamesEquivalentTo("Заказ-наряд АА-000001", "Заказ-наряд ЯЯ-999999");
            workOrderListPage.IssuedOrders.WaitEmpty();

            workOrderListPage.NotIssuedOrders.WorkOrderItem.ElementAt(0).Status.SelectByValue(WorkOrderStatus.IssuedToClient);
            workOrderListPage.NotIssuedOrders.WaitItemNamesEquivalentTo("Заказ-наряд АА-000001");
            workOrderListPage.IssuedOrders.WaitItemNamesEquivalentTo("Заказ-наряд ЯЯ-999999");

            workOrderListPage.NotIssuedOrders.WorkOrderItem.ElementAt(0).Status.SelectByValue(WorkOrderStatus.IssuedToClient);
            workOrderListPage.NotIssuedOrders.WaitEmpty();
            workOrderListPage.IssuedOrders.WaitItemNamesEquivalentTo("Заказ-наряд АА-000001", "Заказ-наряд ЯЯ-999999");

            workOrderListPage.IssuedOrders.WorkOrderItem.ElementAt(0).Status.SelectByValue(WorkOrderStatus.New);
            workOrderListPage.NotIssuedOrders.WaitItemNamesEquivalentTo("Заказ-наряд ЯЯ-999999");
            workOrderListPage.IssuedOrders.WaitItemNamesEquivalentTo("Заказ-наряд АА-000001");
        }

        [Test, Description("Отображение полей ЗН в списке")]
        public async Task FieldsInListTest()
        {
            var today = DateTime.Today;
            var customer = await customerRepository.CreateAsync(shop.OrganizationId, new Customer {Name = "Дмитрий", Phone = "71234567890"});
            var service1 = await productHelper.CreateServiceAsync("Услуга 1", 100);
            var service2 = await productHelper.CreateServiceAsync("Услуга 2", 50);
            var product1 = await productHelper.CreateProductAsync("Товар 1", 25, ProductUnit.Liter);
            var product2 = await productHelper.CreateProductAsync("Товар 2", 12, ProductUnit.Piece);

            var order = WorkOrderBuilder.CreateWithCustomer(customer)
                                        .WithNumber(new WorkOrderNumber("БЮ", 123))
                                        .WithReceptionDate(today - 1.Days())
                                        .WithCompletionDatePlanned(today + 1.Days())
                                        .WithStatus(WorkOrderStatus.New)
                                        .AddShopServiceFromMarketProduct(service1)
                                        .AddShopServiceFromMarketProduct(service2)
                                        .AddShopProductFromMarketProduct(product1)
                                        .AddShopProductFromMarketProduct(product2)
                                        .Build();

            await workOrderHelper.CreateOrderAsync(order);

            var workOrderListPage = LoadWorkOrderList();

            var workOrderItem = workOrderListPage.NotIssuedOrders.WorkOrderItem.First();
            workOrderItem.Name.Text.Wait().EqualTo("Заказ-наряд БЮ-000123");
            workOrderItem.Customer.Text.Wait().EqualTo("Дмитрий, +7 123 456-78-90");
            workOrderItem.Description.Text.Wait().EqualTo("Услуга 1");
            workOrderItem.ReceptionDate.Value.Wait().EqualTo(today - 1.Days());
            workOrderItem.TotalSum.Value.Wait().EqualTo(187);

            var workOrderViewPage = workOrderListPage.GoToNotIssuedOrder();
            workOrderViewPage.ServicesBlock.SpoilerCaption.Click();
            workOrderViewPage.ServicesBlock.RowItem.First().RemoveLink.Click();
            workOrderViewPage.ServicesBlock.RowItem.First().RemoveLink.Click();
            workOrderListPage = workOrderViewPage.SaveAndGoToList();

            workOrderItem = workOrderListPage.NotIssuedOrders.WorkOrderItem.First();
            workOrderItem.Description.Text.Wait().EqualTo("Товар 1");
            workOrderItem.TotalSum.Value.Wait().EqualTo(37);

            workOrderViewPage = workOrderListPage.GoToNotIssuedOrder();
            workOrderViewPage.ProductsBlock.SpoilerCaption.Click();
            workOrderViewPage.ProductsBlock.RowItem.First().RemoveLink.Click();
            workOrderViewPage.ProductsBlock.RowItem.First().RemoveLink.Click();
            workOrderViewPage.SaveAndGoToList();

            workOrderItem = workOrderListPage.NotIssuedOrders.WorkOrderItem.First();
            workOrderItem.Description.Text.Wait().EqualTo("-");
            workOrderItem.TotalSum.Value.Wait().EqualTo(0);
        }

        [Injected]
        private readonly IWorkOrderHelper workOrderHelper;

        [Injected]
        private readonly IProductHelper productHelper;
    }
}