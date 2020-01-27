using System;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;
using FluentAssertions.Extensions;

using GroboContainer.NUnitExtensions;

using Market.Api.Models.Products;
using Market.CustomersAndStaff.FunctionalTests.ComponentExtensions;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.WorkOrders;
using Market.CustomersAndStaff.FunctionalTests.Components.Pages.WorkOrders;
using Market.CustomersAndStaff.FunctionalTests.Helpers.Customers;
using Market.CustomersAndStaff.FunctionalTests.Helpers.Products;
using Market.CustomersAndStaff.FunctionalTests.Helpers.Workers;
using Market.CustomersAndStaff.FunctionalTests.Helpers.WorkOrders;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkOrderExtensions;
using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.Models.WorkOrders;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.WorkOrderTests
{
    public class WorkOrderCreationAndEditionTests : WorkOrderTestBase
    {
        [Test, Description("Полное создание ЗН с указанием и проверкой всех полей")]
        public async Task CreationTest()
        {
            var today = DateTime.Now.Date;

            var customer = await customerHelper.CreateAsync("Машенька", "71234567890", "Комментарий из клиента");
            var worker1 = await workerHelper.CreateAsync("Приёмщик по телефону");
            var worker2 = await workerHelper.CreateAsync("Пьяный механик");
            var service1 = await productHelper.CreateServiceAsync("Накачать шины азотом", 150);
            var service2 = await productHelper.CreateServiceAsync("Прокатило", 999);
            var product1 = await productHelper.CreateProductAsync("Азот", 25, ProductUnit.Liter);
            var product2 = await productHelper.CreateProductAsync("Ничего", 250, ProductUnit.Piece);

            var workOrderListPage = LoadWorkOrderList();
            var workOrderViewPage = workOrderListPage.CreateNewOrder();

            workOrderViewPage.HeaderBlock.ReceptionDate.SetValue(today);
            workOrderViewPage.ClientBlock.NameSelector.TypeAndSelect("Машенька");

            workOrderViewPage.InfoBlock.Phone.SetValue("78007006050");
            workOrderViewPage.InfoBlock.Worker.SelectByValue("Приёмщик по телефону");
            workOrderViewPage.InfoBlock.WarrantyNumber.SetRawValue("123ыюя456asd");
            workOrderViewPage.InfoBlock.CompletionDatePlanned.SetValue(today + 1.Days());
            workOrderViewPage.InfoBlock.CompletionDateFact.SetValue(today + 2.Days());

            workOrderViewPage.ServicesBlock.SpoilerCaption.Click();
            workOrderViewPage.ServicesBlock.RowItem.First().CardNameSelector.TypeAndSelect("Накачать шины азотом");
            workOrderViewPage.ServicesBlock.RowItem.First().Quantity.SetRawValue("4");
            workOrderViewPage.ServicesBlock.RowItem.First().Worker.SelectByValue("Пьяный механик");
            workOrderViewPage.ServicesBlock.RowItem.ElementAt(1).CardNameSelector.TypeAndSelect("Прокатило");
            workOrderViewPage.ServicesBlock.RowItem.ElementAt(1).Quantity.SetRawValue("1");

            workOrderViewPage.ProductsBlock.SpoilerCaption.Click();
            workOrderViewPage.ProductsBlock.RowItem.First().CardNameSelector.TypeAndSelect("Азот");
            workOrderViewPage.ProductsBlock.RowItem.First().Quantity.SetRawValue("0,75");
            workOrderViewPage.ProductsBlock.RowItem.ElementAt(1).CardNameSelector.TypeAndSelect("Ничего");
            workOrderViewPage.ProductsBlock.RowItem.ElementAt(1).Quantity.SetRawValue("1");

            workOrderViewPage.CustomerValuesBlock.SpoilerCaption.Click();
            workOrderViewPage.CustomerValuesBlock.TypeSelector.SelectByValue(CustomerValueType.Vehicle);
            workOrderViewPage.CustomerValuesBlock.VehicleValue.WaitPresence();
            workOrderViewPage.CustomerValuesBlock.VehicleValue.Brand.SetRawValue("Маленькая");
            workOrderViewPage.CustomerValuesBlock.VehicleValue.Model.SetRawValue("Красненькая");
            workOrderViewPage.CustomerValuesBlock.VehicleValue.Comment.SetRawValue("Машинка");
            workOrderViewPage.CustomerValuesBlock.VehicleValue.RegisterSign.SetRawValue("123ВУASD");
            workOrderViewPage.CustomerValuesBlock.VehicleValue.Vin.SetRawValue("JT2EL46D0P0308478");
            workOrderViewPage.CustomerValuesBlock.VehicleValue.BodyNumber.SetRawValue("Number1");
            workOrderViewPage.CustomerValuesBlock.VehicleValue.EngineNumber.SetRawValue("Number2");
            workOrderViewPage.CustomerValuesBlock.VehicleValue.Year.SetRawValue("1901");

            workOrderViewPage.CustomerProductsBlock.SpoilerCaption.Click();
            workOrderViewPage.CustomerProductsBlock.RowItem.First().Name.SetRawValue("Ёлочка-вонючка");
            workOrderViewPage.CustomerProductsBlock.RowItem.First().Quantity.SetRawValue("ящик");

            workOrderViewPage.AdditionalText.SetRawValue("Машина норм, а девочка на любителя.");

            workOrderViewPage.ClientBlock.SelectedName.Text.Wait().EqualTo("Машенька");
            workOrderViewPage.ClientBlock.SelectedPhone.Value.Wait().EqualTo("71234567890");
            workOrderViewPage.ClientBlock.SelectedInfo.Text.Wait().EqualTo("Комментарий из клиента");

            workOrderViewPage.ServicesBlock.RowItem.First().TotalPrice.Value.Wait().EqualTo(600);
            workOrderViewPage.ServicesBlock.RowItem.ElementAt(1).TotalPrice.Value.Wait().EqualTo(999);
            workOrderViewPage.ServicesBlock.TotalSum.Value.Wait().EqualTo(1599);

            workOrderViewPage.ProductsBlock.RowItem.First().TotalPrice.Value.Wait().EqualTo(18.75m);
            workOrderViewPage.ProductsBlock.RowItem.ElementAt(1).TotalPrice.Value.Wait().EqualTo(250);
            workOrderViewPage.ProductsBlock.TotalSum.Value.Wait().EqualTo(268.75m);
            workOrderViewPage.ProductsBlock.SumForServicesAndProducts.Value.Wait().EqualTo(1867.75m);

            workOrderViewPage.SaveButton.Click();
            workOrderListPage = workOrderViewPage.GoToPage<WorkOrderListPage>();

            var workOrder = await workOrderHelper.ReadSingleAsync();
            
            var expectedWorkOrder = WorkOrderBuilder
                                    .CreateWithCustomer(customer)
                                    .WithNumber(new WorkOrderNumber("АА", 1))
                                    .WithStatus(WorkOrderStatus.New)
                                    .WithReceptionDate(today)
                                    .WithShopPhone("78007006050")
                                    .WithReceptionWorker(worker1)
                                    .WithWarrantyNumber("123ыюя456asd")
                                    .WithCompletionDatePlanned(today + 1.Days())
                                    .WithCompletionDateFact(today + 2.Days())
                                    .AddShopServiceFromMarketProduct(service1, worker2, 4)
                                    .AddShopServiceFromMarketProduct(service2, quantity : 1)
                                    .AddShopProductFromMarketProduct(product1, 0.75m)
                                    .AddShopProductFromMarketProduct(product2, 1)
                                    .AddCustomerProduct("Ёлочка-вонючка", "ящик")
                                    .WithVehicleCustomerValue(builder => builder
                                                                  .WithBrand("Маленькая")
                                                                  .WithModel("Красненькая")
                                                                  .WithAdditionalInfo("Машинка")
                                                                  .WithRegisterSign("123ВУASD")
                                                                  .WithVin("JT2EL46D0P0308478")
                                                                  .WithBodyNumber("Number1")
                                                                  .WithEngineNumber("Number2")
                                                                  .WithYear(1901))
                                    .WithAdditionalText("Машина норм, а девочка на любителя.")
                                    .WithTotalSum(1867.75m)
                                    .WithFirstProduct(service1)
                                    .Build();

            workOrder.Should().BeEquivalentTo(expectedWorkOrder, opt => opt.Excluding(x => x.Id));

            workOrderViewPage = workOrderListPage.GoToNotIssuedOrder();

            workOrderViewPage.HeaderBlock.SeriesInput.Text.Wait().EqualTo("АА");
            workOrderViewPage.HeaderBlock.NumberInput.Text.Wait().EqualTo("000001");
            workOrderViewPage.HeaderBlock.ReceptionDate.Value.Wait().EqualTo(today);
            workOrderViewPage.HeaderBlock.WorkOrderStatus.Text.Wait().EqualTo("Новый заказ");

            workOrderViewPage.ClientBlock.SelectedName.Text.Wait().EqualTo("Машенька");
            workOrderViewPage.ClientBlock.SelectedPhone.Value.Wait().EqualTo("71234567890");
            workOrderViewPage.ClientBlock.SelectedInfo.Text.Wait().EqualTo("Комментарий из клиента");

            workOrderViewPage.InfoBlock.Phone.Value.Wait().EqualTo("78007006050");
            workOrderViewPage.InfoBlock.Worker.Text.Wait().EqualTo("Приёмщик по телефону");
            workOrderViewPage.InfoBlock.WarrantyNumber.Text.Wait().EqualTo("123ыюя456asd");
            workOrderViewPage.InfoBlock.CompletionDatePlanned.Value.Wait().EqualTo(today + 1.Days());
            workOrderViewPage.InfoBlock.CompletionDateFact.Value.Wait().EqualTo(today + 2.Days());

            workOrderViewPage.ServicesBlock.SpoilerCaption.Click();
            workOrderViewPage.ServicesBlock.RowItem.Count.Wait().EqualTo(3);
            workOrderViewPage.ServicesBlock.RowItem.First().CheckRow("Накачать шины азотом", 150, "4", 600, "Пьяный механик");
            workOrderViewPage.ServicesBlock.RowItem.Skip(1).First().CheckRow("Прокатило", 999, "1", 999);
            workOrderViewPage.ServicesBlock.TotalSum.Value.Wait().EqualTo(1599);

            workOrderViewPage.ProductsBlock.SpoilerCaption.Click();
            workOrderViewPage.ProductsBlock.RowItem.Count.Wait().EqualTo(3);
            workOrderViewPage.ProductsBlock.RowItem.First().CheckRow("Азот", 25, "0,750", 18.75m);
            workOrderViewPage.ProductsBlock.RowItem.Skip(1).First().CheckRow("Ничего", 250, "1", 250);
            workOrderViewPage.ProductsBlock.TotalSum.Value.Wait().EqualTo(268.75m);
            workOrderViewPage.ProductsBlock.SumForServicesAndProducts.Value.Wait().EqualTo(1867.75m);

            workOrderViewPage.CustomerProductsBlock.SpoilerCaption.Click();
            workOrderViewPage.CustomerProductsBlock.RowItem.Count.Wait().EqualTo(2);
            workOrderViewPage.CustomerProductsBlock.RowItem.First().Name.Text.Wait().EqualTo("Ёлочка-вонючка");
            workOrderViewPage.CustomerProductsBlock.RowItem.First().Quantity.Text.Wait().EqualTo("ящик");

            workOrderViewPage.CustomerValuesBlock.SpoilerCaption.Click();
            workOrderViewPage.CustomerValuesBlock.TypeSelector.Text.Wait().EqualTo("Автомобиль");
            workOrderViewPage.CustomerValuesBlock.VehicleValue.Brand.Text.Wait().EqualTo("Маленькая");
            workOrderViewPage.CustomerValuesBlock.VehicleValue.Model.Text.Wait().EqualTo("Красненькая");
            workOrderViewPage.CustomerValuesBlock.VehicleValue.Comment.Text.Wait().EqualTo("Машинка");
            workOrderViewPage.CustomerValuesBlock.VehicleValue.RegisterSign.Text.Wait().EqualTo("123ВУASD");
            workOrderViewPage.CustomerValuesBlock.VehicleValue.Vin.Text.Wait().EqualTo("JT2EL46D0P0308478");
            workOrderViewPage.CustomerValuesBlock.VehicleValue.BodyNumber.Text.Wait().EqualTo("Number1");
            workOrderViewPage.CustomerValuesBlock.VehicleValue.EngineNumber.Text.Wait().EqualTo("Number2");
            workOrderViewPage.CustomerValuesBlock.VehicleValue.Year.Value.Wait().EqualTo(1901);

            workOrderViewPage.AdditionalText.Text.Wait().EqualTo("Машина норм, а девочка на любителя.");
        }

        [Test, Description("Поиск клиента по номеру телефона")]
        public async Task FindCustomerByPhoneNumberTest()
        {
            var customer = await customerRepository.CreateAsync(shop.OrganizationId, new Customer {Name = "Машенька", Phone = "71234567890"});
            await customerRepository.CreateAsync(shop.OrganizationId, new Customer {Name = "Петруха"});

            var workOrderListPage = LoadWorkOrderList();
            workOrderListPage.AddButton.Click();
            var workOrderViewPage = workOrderListPage.GoToPage<WorkOrderViewPage>();

            workOrderViewPage.InfoBlock.CompletionDatePlanned.SetValue(DateTime.Today + 1.Days());

            workOrderViewPage.ClientBlock.NameSelector.Click();
            workOrderViewPage.ClientBlock.NameSelector.GetMenuItemList<WorkOrderClientItem>().Select(x => (x.Name.Text.Get(), x.Phone.Value.Get()))
                             .Wait().ShouldBeEquivalentTo(new[] {("Машенька", "71234567890"), ("Петруха", null)});
            workOrderViewPage.ClientBlock.NameSelector.SetRawValue("123");
            workOrderViewPage.ClientBlock.NameSelector.GetMenuItemList<WorkOrderClientItem>().Select(x => (x.Name.Text.Get(), x.Phone.Value.Get()))
                             .Wait().ShouldBeEquivalentTo(new[] {("Машенька", "71234567890")});
            workOrderViewPage.ClientBlock.NameSelector.GetMenuItemList<WorkOrderClientItem>().First().Click();
            workOrderViewPage.SaveAndGoToList();

            var workOrder = await workOrderHelper.ReadSingleAsync();
            workOrder.ClientId.Should().Be(customer.Id);
        }

        [Test, Description("Смена клиента новым клиентом при помощи кнопки 'Выбрать другого'")]
        public async Task ChangeCustomerTest()
        {
            await CreateDefaultWorkOrder();

            var workOrderListPage = LoadWorkOrderList();
            workOrderListPage.NotIssuedOrders.WorkOrderItem.First().Click();
            var workOrderViewPage = workOrderListPage.GoToPage<WorkOrderViewPage>();

            workOrderViewPage.ClientBlock.SelectAnother.Click();
            workOrderViewPage.ClientBlock.NameSelector.SetRawValue("Иванов Super Star 9000");
            workOrderViewPage.ClientBlock.ClickEmptySpace();
            workOrderViewPage.ClientBlock.PhoneInput.SetValue("70987654321");
            workOrderViewPage.ClientBlock.Comment.SetRawValue("Клиент из заказ-наряда");
            workOrderViewPage.SaveButton.Click();
            workOrderViewPage.GoToPage<WorkOrderListPage>();

            var customers = await customerRepository.ReadByOrganizationAsync(shop.OrganizationId);
            customers.Length.Should().Be(2);
            var customer = customers.Single(x => x.Phone == "70987654321");
            customer.Should().BeEquivalentTo(new Customer {Name = "Иванов Super Star 9000", Phone = "70987654321", AdditionalInfo = "Клиент из заказ-наряда"},
                                             cfg => cfg.Excluding(x => x.Id).Excluding(x => x.OrganizationId));

            var order = await workOrderHelper.ReadSingleAsync();
            order.ClientId.Should().Be(customer.Id);
        }

        [Test, Description("Удаление сохранённых позиций внутри ЗН")]
        public async Task RemovePositionsTest()
        {
            var service = await productHelper.CreateServiceAsync("Услуга", 1);
            var product = await productHelper.CreateProductAsync("Товар", 1);

            var customer = await CreateDefaultCustomer();
            var order = WorkOrderBuilder.CreateWithCustomer(customer)
                                        .AddShopServiceFromMarketProduct(service)
                                        .AddShopProductFromMarketProduct(product)
                                        .AddCustomerProduct("Материалы", "1шт")
                                        .Build();
            await workOrderHelper.CreateOrderAsync(order);

            var workOrderListPage = LoadWorkOrderList();
            var workOrderViewPage = workOrderListPage.GoToNotIssuedOrder();

            workOrderViewPage.ServicesBlock.SpoilerCaption.Click();
            workOrderViewPage.ServicesBlock.RowItem.First().RemoveLink.Click();
            workOrderViewPage.ProductsBlock.SpoilerCaption.Click();
            workOrderViewPage.ProductsBlock.RowItem.First().RemoveLink.Click();
            workOrderViewPage.CustomerProductsBlock.SpoilerCaption.Click();
            workOrderViewPage.CustomerProductsBlock.RowItem.First().RemoveLink.Click();
            workOrderViewPage.SaveAndGoToList();

            order = await workOrderHelper.ReadSingleAsync();
            order.ShopServices.Should().BeEmpty();
            order.ShopProducts.Should().BeEmpty();
            order.CustomerProducts.Should().BeEmpty();
        }

        [Test, Description("Кнопка 'Назад' не сохраняет ЗН")]
        public async Task BackButtonTest()
        {
            var workOrderListPage = LoadWorkOrderList();
            var workOrderViewPage = workOrderListPage.CreateNewOrder();
            workOrderViewPage.GoBackToList();

            await workOrderHelper.CheckRepositoryIsEmpty();
        }

        [Injected]
        private IWorkOrderHelper workOrderHelper;

        [Injected]
        private ICustomerHelper customerHelper;

        [Injected]
        private IWorkerHelper workerHelper;

        [Injected]
        private IProductHelper productHelper;
    }
}