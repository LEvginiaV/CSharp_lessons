using System;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;
using FluentAssertions.Extensions;

using GroboContainer.NUnitExtensions;

using Market.CustomersAndStaff.FunctionalTests.ComponentExtensions;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.WorkOrders;
using Market.CustomersAndStaff.FunctionalTests.Components.Pages.WorkOrders;
using Market.CustomersAndStaff.FunctionalTests.Helpers.Products;
using Market.CustomersAndStaff.FunctionalTests.Helpers.WorkOrders;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkOrderExtensions;
using Market.CustomersAndStaff.Models.WorkOrders;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.WorkOrderTests
{
    public class WorkOrderValidationTests : WorkOrderTestBase
    {
        [Test, Description("Минимальный набор полей")]
        public async Task MinimalFieldsTest()
        {
            var today = DateTime.Now.Date;
            await CreateDefaultCustomer();

            var workOrderListPage = LoadWorkOrderList();
            workOrderListPage.AddButton.Click();

            var workOrderViewPage = workOrderListPage.GoToPage<WorkOrderViewPage>();
            workOrderViewPage.HeaderBlock.ReceptionDate.Clear();
            workOrderViewPage.SaveButton.Click();

            workOrderViewPage.HeaderBlock.ReceptionDate.Error.Wait().EqualTo(true);
            workOrderViewPage.HeaderBlock.ReceptionDateValidation.ErrorMessage.Text.Wait().EqualTo("Необходимо ввести дату приемки");

            workOrderViewPage.HeaderBlock.ReceptionDate.SetValue(today);
            workOrderViewPage.SaveButton.Click();

            workOrderViewPage.ClientBlock.NameSelector.Error.Wait().EqualTo(true);
            workOrderViewPage.ClientBlock.NameValidation.ErrorMessage.Text.Wait().EqualTo("Имя должно быть заполнено");

            workOrderViewPage.ClientBlock.NameSelector.SelectByIndex(0);
            workOrderViewPage.SaveButton.Click();

            workOrderViewPage.InfoBlock.CompletionDatePlanned.Error.Wait().EqualTo(true);
            workOrderViewPage.InfoBlock.CompletionDatePlannedValidation.ErrorMessage.Text.Wait().EqualTo("Необходимо ввести дату завершения");

            workOrderViewPage.InfoBlock.CompletionDatePlanned.SetValue(today + 1.Days());
            workOrderViewPage.SaveButton.Click();

            workOrderViewPage.WaitAbsence();
            workOrderViewPage.GoToPage<WorkOrderListPage>();
        }

        [Test, Description("Валидация дат")]
        public async Task DateValidationTest()
        {
            await CreateDefaultWorkOrder();
            var today = DateTime.Now.Date;

            var workOrderListPage = LoadWorkOrderList();
            workOrderListPage.NotIssuedOrders.WorkOrderItem.First().Click();
            var workOrderViewPage = workOrderListPage.GoToPage<WorkOrderViewPage>();

            workOrderViewPage.HeaderBlock.ReceptionDate.ResetRawValue("30.06.2016");
            workOrderViewPage.HeaderBlock.ReceptionDate.Error.Wait().EqualTo(true);
            workOrderViewPage.HeaderBlock.ReceptionDateValidation.ErrorMessage.Text.Wait().EqualTo("Дата приемки слишком далеко в прошлом");

            workOrderViewPage.HeaderBlock.ReceptionDate.ResetValue(today + 2.Days());
            workOrderViewPage.HeaderBlock.ReceptionDate.Error.Wait().EqualTo(true);
            workOrderViewPage.HeaderBlock.ReceptionDateValidation.ErrorMessage.Text.Wait().EqualTo("Дата приемки слишком далеко в будущем");

            workOrderViewPage.HeaderBlock.ReceptionDate.ResetValue(today - 1.Days());
            workOrderViewPage.HeaderBlock.ReceptionDate.Error.Wait().EqualTo(false);

            workOrderViewPage.InfoBlock.CompletionDatePlanned.ResetValue(today - 2.Days());
            workOrderViewPage.InfoBlock.CompletionDatePlanned.Error.Wait().EqualTo(true);
            workOrderViewPage.InfoBlock.CompletionDatePlannedValidation.ErrorMessage.Text.Wait().EqualTo("Дата завершения не может быть раньше даты приемки");

            workOrderViewPage.InfoBlock.CompletionDatePlanned.ResetValue(today.AddYears(1));
            workOrderViewPage.InfoBlock.CompletionDatePlanned.Error.Wait().EqualTo(true);
            workOrderViewPage.InfoBlock.CompletionDatePlannedValidation.ErrorMessage.Text.Wait().EqualTo("Дата завершения слишком далеко в будущем");

            workOrderViewPage.InfoBlock.CompletionDatePlanned.ResetValue(today);
            workOrderViewPage.InfoBlock.CompletionDatePlanned.Error.Wait().EqualTo(false);

            workOrderViewPage.InfoBlock.CompletionDateFact.ResetValue(today - 2.Days());
            workOrderViewPage.InfoBlock.CompletionDateFact.Error.Wait().EqualTo(true);
            workOrderViewPage.InfoBlock.CompletionDateFactValidation.ErrorMessage.Text.Wait().EqualTo("Дата завершения не может быть раньше даты приемки");

            workOrderViewPage.InfoBlock.CompletionDateFact.ResetValue(today.AddYears(1));
            workOrderViewPage.InfoBlock.CompletionDateFact.Error.Wait().EqualTo(true);
            workOrderViewPage.InfoBlock.CompletionDateFactValidation.ErrorMessage.Text.Wait().EqualTo("Дата завершения слишком далеко в будущем");

            workOrderViewPage.InfoBlock.CompletionDateFact.ResetValue(today);
            workOrderViewPage.InfoBlock.CompletionDateFact.Error.Wait().EqualTo(false);

            workOrderViewPage.SaveButton.Click();
            workOrderViewPage.WaitAbsence();

            workOrderViewPage.GoToPage<WorkOrderListPage>();
        }

        [Test, Description("Максимальные длины полей")]
        public async Task MaxLengthTest()
        {
            const string wholeProductName = "Свеча RX-228";
            const string fractionalProductName = "Краска для волос";

            await CreateDefaultWorkOrder();

            var workOrderListPage = LoadWorkOrderList();
            workOrderListPage.NotIssuedOrders.WorkOrderItem.First().Click();
            var workOrderViewPage = workOrderListPage.GoToPage<WorkOrderViewPage>();

            workOrderViewPage.HeaderBlock.SeriesInput.SetRawValue("АБВ");
            workOrderViewPage.HeaderBlock.SeriesInput.Text.Wait().EqualTo("АБ");
            workOrderViewPage.HeaderBlock.NumberInput.SetRawValue("1234567");
            workOrderViewPage.HeaderBlock.NumberInput.Text.Wait().EqualTo("123456");

            workOrderViewPage.InfoBlock.Phone.SetValue("712345678901");
            workOrderViewPage.InfoBlock.Phone.Value.Wait().EqualTo("71234567890");
            workOrderViewPage.InfoBlock.WarrantyNumber.MaxLength.Wait().EqualTo(20);

            workOrderViewPage.ClientBlock.ChangeData.Click();
            workOrderViewPage.ClientBlock.NameInput.MaxLength.Wait().EqualTo(120);
            workOrderViewPage.ClientBlock.PhoneInput.SetValue("712345678901");
            workOrderViewPage.ClientBlock.PhoneInput.Value.Wait().EqualTo("71234567890");
            workOrderViewPage.ClientBlock.Comment.MaxLength.Wait().EqualTo(500);

            workOrderViewPage.ClientBlock.CancelEdit.Click();
            workOrderViewPage.ClientBlock.SelectAnother.Click();
            workOrderViewPage.ClientBlock.NameSelector.Click();
            workOrderViewPage.ClientBlock.NameSelector.MaxLength.Wait().EqualTo(120);
            workOrderViewPage.ClientBlock.ClickEmptySpace();
            workOrderViewPage.ClientBlock.PhoneInput.SetValue("712345678901");
            workOrderViewPage.ClientBlock.PhoneInput.Value.Wait().EqualTo("71234567890");
            workOrderViewPage.ClientBlock.Comment.MaxLength.Wait().EqualTo(500);

            workOrderViewPage.ServicesBlock.SpoilerCaption.Click();
            workOrderViewPage.ServicesBlock.RowItem.First().CardNameSelector.SelectByIndex(0);
            workOrderViewPage.ServicesBlock.RowItem.First().Price.MaxLength.Wait().EqualTo(16);
            workOrderViewPage.ServicesBlock.RowItem.First().Quantity.MaxLength.Wait().EqualTo(13);

            workOrderViewPage.ProductsBlock.SpoilerCaption.Click();
            workOrderViewPage.ProductsBlock.RowItem.First().CardNameSelector.TypeAndSelect(wholeProductName);
            workOrderViewPage.ProductsBlock.RowItem.First().Price.MaxLength.Wait().EqualTo(16);
            workOrderViewPage.ProductsBlock.RowItem.First().Quantity.MaxLength.Wait().EqualTo(13);
            workOrderViewPage.ProductsBlock.RowItem.ElementAt(1).CardNameSelector.TypeAndSelect(fractionalProductName);
            workOrderViewPage.ProductsBlock.RowItem.ElementAt(1).Price.MaxLength.Wait().EqualTo(16);
            workOrderViewPage.ProductsBlock.RowItem.ElementAt(1).Quantity.MaxLength.Wait().EqualTo(17);

            workOrderViewPage.CustomerProductsBlock.SpoilerCaption.Click();
            workOrderViewPage.CustomerProductsBlock.RowItem.First().Name.SetRawValue("1");
            workOrderViewPage.CustomerProductsBlock.RowItem.First().Name.MaxLength.Wait().EqualTo(100);
            workOrderViewPage.CustomerProductsBlock.RowItem.First().Quantity.MaxLength.Wait().EqualTo(14);

            workOrderViewPage.CustomerValuesBlock.SpoilerCaption.Click();

            workOrderViewPage.CustomerValuesBlock.TypeSelector.SelectByValue(CustomerValueType.Appliances);
            workOrderViewPage.CustomerValuesBlock.ApplianceValue.WaitPresence();
            workOrderViewPage.CustomerValuesBlock.ApplianceValue.Name.MaxLength.Wait().EqualTo(100);
            workOrderViewPage.CustomerValuesBlock.ApplianceValue.Number.MaxLength.Wait().EqualTo(100);
            workOrderViewPage.CustomerValuesBlock.ApplianceValue.Brand.MaxLength.Wait().EqualTo(40);
            workOrderViewPage.CustomerValuesBlock.ApplianceValue.Model.MaxLength.Wait().EqualTo(100);
            workOrderViewPage.CustomerValuesBlock.ApplianceValue.Comment.MaxLength.Wait().EqualTo(500);

            workOrderViewPage.CustomerValuesBlock.TypeSelector.SelectByValue(CustomerValueType.Vehicle);
            workOrderViewPage.CustomerValuesBlock.VehicleValue.WaitPresence();
            workOrderViewPage.CustomerValuesBlock.VehicleValue.Brand.MaxLength.Wait().EqualTo(40);
            workOrderViewPage.CustomerValuesBlock.VehicleValue.Model.MaxLength.Wait().EqualTo(100);
            workOrderViewPage.CustomerValuesBlock.VehicleValue.RegisterSign.MaxLength.Wait().EqualTo(15);

            workOrderViewPage.CustomerValuesBlock.VehicleValue.Vin.MaxLength.Wait().EqualTo(17);
            workOrderViewPage.CustomerValuesBlock.VehicleValue.Vin.SetRawValue("1234567890123456");
            workOrderViewPage.CustomerValuesBlock.VehicleValue.BodyNumber.Click();
            workOrderViewPage.CustomerValuesBlock.VehicleValue.Vin.Error.Wait().EqualTo(true);
            workOrderViewPage.CustomerValuesBlock.VehicleValue.Vin.MouseOver();
            workOrderViewPage.CustomerValuesBlock.VehicleValue.VinValidation.ErrorMessage.Text.Wait().EqualTo("В строке должно быть 17 символов");

            workOrderViewPage.CustomerValuesBlock.VehicleValue.BodyNumber.MaxLength.Wait().EqualTo(17);
            workOrderViewPage.CustomerValuesBlock.VehicleValue.EngineNumber.MaxLength.Wait().EqualTo(17);
            workOrderViewPage.CustomerValuesBlock.VehicleValue.Comment.MaxLength.Wait().EqualTo(500);

            workOrderViewPage.CustomerValuesBlock.TypeSelector.SelectByValue(CustomerValueType.Other);
            workOrderViewPage.CustomerValuesBlock.OtherValue.WaitPresence();
            workOrderViewPage.CustomerValuesBlock.OtherValue.Comment.MaxLength.Wait().EqualTo(500);

            workOrderViewPage.AdditionalText.MaxLength.Wait().EqualTo(2000);
        }

        [Test, Description("Транспортное средство: VIN и госномер обязательны")]
        public async Task VehicleValueTest()
        {
            await CreateDefaultWorkOrder();

            var workOrderListPage = LoadWorkOrderList();
            workOrderListPage.NotIssuedOrders.WorkOrderItem.First().Click();
            var workOrderViewPage = workOrderListPage.GoToPage<WorkOrderViewPage>();

            workOrderViewPage.CustomerValuesBlock.SpoilerCaption.Click();
            workOrderViewPage.CustomerValuesBlock.TypeSelector.SelectByValue(CustomerValueType.Vehicle);
            workOrderViewPage.CustomerValuesBlock.VehicleValue.WaitPresence();

            workOrderViewPage.CustomerValuesBlock.VehicleValue.Brand.SetRawValue("1");
            workOrderViewPage.SaveButton.Click();
            CheckVehicleValueErrorsInFields(workOrderViewPage.CustomerValuesBlock.VehicleValue);
            workOrderViewPage.CustomerValuesBlock.VehicleValue.Brand.Clear();

            workOrderViewPage.CustomerValuesBlock.VehicleValue.Model.SetRawValue("1");
            workOrderViewPage.SaveButton.Click();
            CheckVehicleValueErrorsInFields(workOrderViewPage.CustomerValuesBlock.VehicleValue);
            workOrderViewPage.CustomerValuesBlock.VehicleValue.Model.Clear();

            workOrderViewPage.CustomerValuesBlock.VehicleValue.Comment.SetRawValue("1");
            workOrderViewPage.SaveButton.Click();
            CheckVehicleValueErrorsInFields(workOrderViewPage.CustomerValuesBlock.VehicleValue);
            workOrderViewPage.CustomerValuesBlock.VehicleValue.Comment.Clear();

            workOrderViewPage.CustomerValuesBlock.VehicleValue.BodyNumber.SetRawValue("1");
            workOrderViewPage.SaveButton.Click();
            CheckVehicleValueErrorsInFields(workOrderViewPage.CustomerValuesBlock.VehicleValue);
            workOrderViewPage.CustomerValuesBlock.VehicleValue.BodyNumber.Clear();

            workOrderViewPage.CustomerValuesBlock.VehicleValue.EngineNumber.SetRawValue("1");
            workOrderViewPage.SaveButton.Click();
            CheckVehicleValueErrorsInFields(workOrderViewPage.CustomerValuesBlock.VehicleValue);
            workOrderViewPage.CustomerValuesBlock.VehicleValue.EngineNumber.Clear();

            workOrderViewPage.CustomerValuesBlock.VehicleValue.Year.SetRawValue("1");
            workOrderViewPage.SaveButton.Click();
            CheckVehicleValueErrorsInFields(workOrderViewPage.CustomerValuesBlock.VehicleValue);
            workOrderViewPage.CustomerValuesBlock.VehicleValue.Year.Clear();

            workOrderViewPage.CustomerValuesBlock.VehicleValue.RegisterSign.SetRawValue("1");
            workOrderViewPage.SaveButton.Click();
            workOrderViewPage.CustomerValuesBlock.VehicleValue.Vin.Error.Wait().EqualTo(true);
            workOrderViewPage.CustomerValuesBlock.VehicleValue.VinValidation.ErrorMessage.Text.Wait().EqualTo("В строке должно быть 17 символов");

            workOrderViewPage.CustomerValuesBlock.VehicleValue.Vin.SetRawValue("12345678901234567");
            workOrderViewPage.SaveAndGoToList();

            var order = await workOrderHelper.ReadSingleAsync();
            order.CustomerValues.CustomerValueType.Should().Be(CustomerValueType.Vehicle);
            var vehicleCustomerValue = order.CustomerValues.CustomerValues.Cast<VehicleCustomerValue>().Single();
            vehicleCustomerValue.RegisterSign.Should().Be("1");
            vehicleCustomerValue.Vin.Should().Be("12345678901234567");
        }

        private void CheckVehicleValueErrorsInFields(VehicleValue vehicleValue)
        {
            vehicleValue.RegisterSign.Error.Wait().EqualTo(true);
            vehicleValue.RegisterSignValidation.ErrorMessage.Text.Wait().EqualTo("Строка не должна быть пустая");
            vehicleValue.Vin.MouseOver();
            vehicleValue.Vin.Error.Wait().EqualTo(true);
            vehicleValue.VinValidation.ErrorMessage.Text.Wait().EqualTo("В строке должно быть 17 символов");
        }

        [Test, Description("Техника: наименование обязательно")]
        public async Task ApplianceValueTest()
        {
            await CreateDefaultWorkOrder();

            var workOrderListPage = LoadWorkOrderList();
            workOrderListPage.NotIssuedOrders.WorkOrderItem.First().Click();
            var workOrderViewPage = workOrderListPage.GoToPage<WorkOrderViewPage>();

            workOrderViewPage.CustomerValuesBlock.SpoilerCaption.Click();
            workOrderViewPage.CustomerValuesBlock.TypeSelector.SelectByValue(CustomerValueType.Appliances);
            workOrderViewPage.CustomerValuesBlock.ApplianceValue.WaitPresence();

            workOrderViewPage.CustomerValuesBlock.ApplianceValue.Number.SetRawValue("1");
            workOrderViewPage.SaveButton.Click();
            CheckApplianceValueErrorsInFields(workOrderViewPage.CustomerValuesBlock.ApplianceValue);
            workOrderViewPage.CustomerValuesBlock.ApplianceValue.Number.Clear();

            workOrderViewPage.CustomerValuesBlock.ApplianceValue.Comment.SetRawValue("1");
            workOrderViewPage.SaveButton.Click();
            CheckApplianceValueErrorsInFields(workOrderViewPage.CustomerValuesBlock.ApplianceValue);
            workOrderViewPage.CustomerValuesBlock.ApplianceValue.Comment.Clear();

            workOrderViewPage.CustomerValuesBlock.ApplianceValue.Brand.SetRawValue("1");
            workOrderViewPage.SaveButton.Click();
            CheckApplianceValueErrorsInFields(workOrderViewPage.CustomerValuesBlock.ApplianceValue);
            workOrderViewPage.CustomerValuesBlock.ApplianceValue.Brand.Clear();

            workOrderViewPage.CustomerValuesBlock.ApplianceValue.Model.SetRawValue("1");
            workOrderViewPage.SaveButton.Click();
            CheckApplianceValueErrorsInFields(workOrderViewPage.CustomerValuesBlock.ApplianceValue);
            workOrderViewPage.CustomerValuesBlock.ApplianceValue.Model.Clear();

            workOrderViewPage.CustomerValuesBlock.ApplianceValue.Year.SetRawValue("1");
            workOrderViewPage.SaveButton.Click();
            CheckApplianceValueErrorsInFields(workOrderViewPage.CustomerValuesBlock.ApplianceValue);
            workOrderViewPage.CustomerValuesBlock.ApplianceValue.Year.Clear();

            workOrderViewPage.CustomerValuesBlock.ApplianceValue.Name.SetRawValue("1");
            workOrderViewPage.SaveButton.Click();
            workOrderViewPage.GoToPage<WorkOrderListPage>();

            var order = await workOrderHelper.ReadSingleAsync();
            order.CustomerValues.CustomerValueType.Should().Be(CustomerValueType.Appliances);
            var applianceCustomerValue = order.CustomerValues.CustomerValues.Cast<ApplianceCustomerValue>().Single();
            applianceCustomerValue.Name.Should().Be("1");
        }

        private void CheckApplianceValueErrorsInFields(ApplianceValue applianceValue)
        {
            applianceValue.Name.Error.Wait().EqualTo(true);
            applianceValue.NameValidation.ErrorMessage.Text.Wait().EqualTo("Строка не должна быть пустая");
        }

        [Test, Description("Поля 'цена' и 'количество' обязательны для работ, товаров и материалов клиента")]
        public async Task ProductsTest()
        {
            await CreateDefaultWorkOrder();


            await productHelper.CreateServiceAsync("Услуга");
            await productHelper.CreateProductAsync("Товар");
            
            var workOrderListPage = LoadWorkOrderList();
            workOrderListPage.NotIssuedOrders.WorkOrderItem.First().Click();
            var workOrderViewPage = workOrderListPage.GoToPage<WorkOrderViewPage>();

            workOrderViewPage.ServicesBlock.SpoilerCaption.Click();
            workOrderViewPage.ServicesBlock.RowItem.First().CardNameSelector.TypeAndSelect("Услуга");

            workOrderViewPage.SaveButton.Click();
            workOrderViewPage.ServicesBlock.RowItem.First().Quantity.Error.Wait().EqualTo(true);
            workOrderViewPage.ServicesBlock.RowItem.First().QuantityValidation.ErrorMessage.Text.Wait().EqualTo("Необходимо ввести значение");
            workOrderViewPage.ServicesBlock.RowItem.First().Quantity.SetRawValue("1");
            workOrderViewPage.ServicesBlock.RowItem.First().Price.MouseOver();
            workOrderViewPage.ServicesBlock.RowItem.First().Price.Error.Wait().EqualTo(true);
            workOrderViewPage.ServicesBlock.RowItem.First().PriceValidation.ErrorMessage.Text.Wait().EqualTo("Необходимо ввести значение");

            workOrderViewPage.ServicesBlock.RowItem.First().Quantity.ResetRawValue("1");
            workOrderViewPage.SaveButton.Click();
            workOrderViewPage.ServicesBlock.RowItem.First().Price.Error.Wait().EqualTo(true);
            workOrderViewPage.ServicesBlock.RowItem.First().PriceValidation.ErrorMessage.Text.Wait().EqualTo("Необходимо ввести значение");
            workOrderViewPage.ServicesBlock.RowItem.First().Quantity.Clear();

            workOrderViewPage.ServicesBlock.RowItem.First().Price.ResetValue(1);
            workOrderViewPage.SaveButton.Click();
            workOrderViewPage.ServicesBlock.RowItem.First().Quantity.Error.Wait().EqualTo(true);
            workOrderViewPage.ServicesBlock.RowItem.First().QuantityValidation.ErrorMessage.Text.Wait().EqualTo("Необходимо ввести значение");
            workOrderViewPage.ServicesBlock.RowItem.First().Quantity.ResetRawValue("1");

            workOrderViewPage.ProductsBlock.SpoilerCaption.Click();
            workOrderViewPage.ProductsBlock.RowItem.First().CardNameSelector.TypeAndSelect("Товар");

            workOrderViewPage.SaveButton.Click();
            workOrderViewPage.ProductsBlock.RowItem.First().Quantity.Error.Wait().EqualTo(true);
            workOrderViewPage.ProductsBlock.RowItem.First().QuantityValidation.ErrorMessage.Text.Wait().EqualTo("Необходимо ввести значение");
            workOrderViewPage.ProductsBlock.RowItem.First().Quantity.SetRawValue("1");
            workOrderViewPage.ProductsBlock.RowItem.First().Price.MouseOver();
            workOrderViewPage.ProductsBlock.RowItem.First().Price.Error.Wait().EqualTo(true);
            workOrderViewPage.ProductsBlock.RowItem.First().PriceValidation.ErrorMessage.Text.Wait().EqualTo("Необходимо ввести значение");

            workOrderViewPage.ProductsBlock.RowItem.First().Quantity.ResetRawValue("1");
            workOrderViewPage.SaveButton.Click();
            workOrderViewPage.ProductsBlock.RowItem.First().Price.Error.Wait().EqualTo(true);
            workOrderViewPage.ProductsBlock.RowItem.First().PriceValidation.ErrorMessage.Text.Wait().EqualTo("Необходимо ввести значение");
            workOrderViewPage.ProductsBlock.RowItem.First().Quantity.Clear();

            workOrderViewPage.ProductsBlock.RowItem.First().Price.ResetValue(1);
            workOrderViewPage.SaveButton.Click();
            workOrderViewPage.ProductsBlock.RowItem.First().Quantity.Error.Wait().EqualTo(true);
            workOrderViewPage.ProductsBlock.RowItem.First().QuantityValidation.ErrorMessage.Text.Wait().EqualTo("Необходимо ввести значение");
            workOrderViewPage.ProductsBlock.RowItem.First().Quantity.ResetRawValue("1");

            workOrderViewPage.CustomerProductsBlock.SpoilerCaption.Click();
            workOrderViewPage.CustomerProductsBlock.RowItem.First().Name.SetRawValue("1");

            workOrderViewPage.SaveButton.Click();
            workOrderViewPage.CustomerProductsBlock.RowItem.First().Quantity.Error.Wait().EqualTo(true);
            workOrderViewPage.CustomerProductsBlock.RowItem.First().QuantityValidation.ErrorMessage.Text.Wait().EqualTo("Строка не должна быть пустая");
        }
        
        [Injected]
        private IWorkOrderHelper workOrderHelper;

        [Injected]
        private IProductHelper productHelper;
    }
}