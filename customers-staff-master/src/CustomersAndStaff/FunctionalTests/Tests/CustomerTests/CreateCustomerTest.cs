using System;
using System.Linq;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Common;
using Market.CustomersAndStaff.FunctionalTests.Components.Pages;
using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Customers;
using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Workers;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.Utils.Extensions;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.CustomerTests
{
    public class CreateCustomerTest : TestBase
    {
        [Test, Description("Валидации полей")]
        public void FieldValidationTest()
        {
            var customerListPage =
                LoadMainPage()
                    .Do(page => page.NavigationLayout.CustomersLink.Click())
                    .GoToPage<CustomerListPage>();

            var customerEditorModal =
                customerListPage
                    .Do(page =>
                        {
                            page.EmptyPersonList.IsPresent.Wait().EqualTo(true);
                            page.EmptyPersonList.EmptyPersonListAddButton.Click();
                        })
                    .WaitModal<CustomerEditorModal>();

            customerEditorModal.Do(modal =>
                {
                    modal.Name.MaxLength.Wait().EqualTo(120);
                    var name = string.Join("ё", Enumerable.Range(0, 5).Select(x => Guid.NewGuid()));
                    modal.Name.SetRawValue(name);
                    modal.Name.WaitText(name.Substring(0, 120));
                });

            customerEditorModal.Do(modal =>
                {
                    modal.Phone.SetRawValue("+");
                    modal.Phone.WaitText(string.Empty);
                    modal.Phone.SetRawValue("abc");
                    modal.Phone.WaitText(string.Empty);
                    
                    modal.Phone.ResetRawValue("9999");
                    modal.Phone.Unfocus();
                    modal.Phone.Error.WaitBool(true);
                    
                    modal.Phone.ResetRawValue("9112223344");
                    modal.Phone.Value.Wait().EqualTo("79112223344");
                });

            customerEditorModal.Do(modal =>
                {
                    modal.Discount.SetRawValue("-");
                    modal.Discount.Value.Wait().EqualTo(null);
                    modal.Discount.SetRawValue("abc");
                    modal.Discount.Value.Wait().EqualTo(null);
                    modal.Discount.SetRawValue("121");
                    modal.Discount.Value.Wait().EqualTo(12m);
                    modal.Discount.ResetValue(20);
                    modal.Discount.Value.Wait().EqualTo(20m);
                    modal.Discount.ResetValue(20.22m);
                    modal.Discount.Value.Wait().EqualTo(20.22m);
                    modal.Discount.ResetValue(20.228m);
                    modal.Discount.Value.Wait().EqualTo(20.22m);
                });

            customerEditorModal.Do(modal =>
                {
                    modal.CustomId.MaxLength.Wait().EqualTo(16);
                    modal.CustomId.SetRawValue("Vasya");
                    modal.CustomId.Text.Wait().EqualTo("Vasya");
                });

            customerEditorModal.Do(modal =>
                {
                    modal.AdditionalInfo.MaxLength.Wait().EqualTo(500);
                    modal.AdditionalInfo.SetRawValue("bla-bla-bla");
                    modal.AdditionalInfo.Text.Wait().EqualTo("bla-bla-bla");
                });

            customerEditorModal.Do(modal =>
                {
                    modal.Email.MaxLength.Wait().EqualTo(100);

                    modal.Email.SetRawValue("Vasya");
                    modal.Email.Unfocus();
                    modal.Email.Error.WaitBool(true);
                    modal.Email.WaitText("Vasya");

                    modal.Email.ResetRawValue("Vasya@mail");
                    modal.Email.Unfocus();
                    modal.Email.Error.WaitBool(true);
                    modal.Email.WaitText("Vasya@mail");

                    modal.Email.ResetRawValue("Vasya@mail.ru");
                    modal.Email.Unfocus();
                    modal.Email.Error.WaitBool(false);
                });

            customerEditorModal.Do(modal =>
                {
                    CheckInput(modal.Birthday.BirthdayDay, value: "-", expectedValue: string.Empty, waitError: false);
                    CheckInput(modal.Birthday.BirthdayDay, value: "fgd", expectedValue: string.Empty, waitError: false);
                    
                    CheckIntInput(modal.Birthday.BirthdayDay, value: 0, expectedValue: 0, waitError: true);
                    CheckIntInput(modal.Birthday.BirthdayDay, value: 404, expectedValue: 40, waitError: true);
                    
                    CheckIntInput(modal.Birthday.BirthdayDay, value: 31, expectedValue: 31, waitError: false);
                    
                    modal.Birthday.BirthdayDay.Clear();
                    
                    CheckCombobox(modal.Birthday.BirthdayMonth, value: "-", expectedValue: "-", waitError: true);
                    CheckCombobox(modal.Birthday.BirthdayMonth, value: "0", expectedValue: "0", waitError: true);
                    CheckCombobox(modal.Birthday.BirthdayMonth, value: "13", expectedValue: "13", waitError: true);
                    CheckCombobox(modal.Birthday.BirthdayMonth, value: "asd", expectedValue: "asd", waitError: true);
                    
                    CheckCombobox(modal.Birthday.BirthdayMonth, value: "3", expectedValue: "Март", waitError: false);
                    CheckCombobox(modal.Birthday.BirthdayMonth, value: "апрель", expectedValue: "Апрель", waitError: false);
                    CheckCombobox(modal.Birthday.BirthdayMonth, value: "Май", expectedValue: "Май", waitError: false);
                    
                    modal.Birthday.BirthdayMonth.Clear();
                    modal.Birthday.BirthdayMonth.ItemListPresent.Wait().EqualTo(true);
                    modal.Birthday.BirthdayMonth.GetMenuItemList<Label>().Count.Wait().EqualTo(12);
                    modal.Birthday.BirthdayMonth.SelectByIndex(3);
                    modal.Birthday.BirthdayMonth.Text.Wait().EqualTo("Апрель");
                    
                    modal.Birthday.BirthdayMonth.Clear();
                    
                    CheckInput(modal.Birthday.BirthdayYear, value: "-", expectedValue: string.Empty, waitError: false);
                    CheckInput(modal.Birthday.BirthdayYear, value: "fgd", expectedValue: string.Empty, waitError: false);
                    
                    CheckIntInput(modal.Birthday.BirthdayYear, value: 0, expectedValue: 0, waitError: true);
                    CheckIntInput(modal.Birthday.BirthdayYear, value: 12345, expectedValue: 1234, waitError: true);
                    
                    CheckIntInput(modal.Birthday.BirthdayYear, value: 1899, expectedValue: 1899, waitError: true);
                    var currentYear = DateTime.Now.Year;
                    CheckIntInput(modal.Birthday.BirthdayYear, value: currentYear + 1, expectedValue: currentYear + 1, waitError: true);
                    
                    CheckIntInput(modal.Birthday.BirthdayYear, value: currentYear, expectedValue: currentYear, waitError: false);
                    CheckIntInput(modal.Birthday.BirthdayYear, value: 1900, expectedValue: 1900, waitError: false);
                    
                    modal.Birthday.BirthdayYear.Clear();
                    
                    CheckDayAndMonthError(modal, "31", "", ErrorSide.Right);
                    CheckDayAndMonthError(modal, "", "1", ErrorSide.Left);
                    CheckDayAndMonthError(modal, "31", "2", ErrorSide.Left);

                    CheckFullDate(modal, DateHelper.Tomorrow(), true);
                    CheckFullDate(modal, DateTime.Now, false);
                });

            customerEditorModal.AcceptButton.Click();

            customerListPage.WaitModalClose<CustomerEditorModal>();
        }

        private void CheckInput(Input field, string value, string expectedValue, bool waitError)
        {
            field.ResetRawValue(value);
            field.Unfocus();
            field.WaitText(expectedValue);
            field.Error.WaitBool(waitError);
        }
        
        private void CheckIntInput(Input<int?> field, int? value, int? expectedValue, bool waitError)
        {
            field.ResetValue(value);
            field.Unfocus();
            field.Value.Wait().EqualTo(expectedValue);
            field.Error.WaitBool(waitError);
        }

        private void CheckCombobox(ComboBox comboBox, string value, string expectedValue, bool waitError)
        {
            comboBox.ResetRawValue(value);
            comboBox.Unfocus();
            comboBox.Text.Wait().EqualTo(expectedValue);
            comboBox.Error.WaitBool(waitError);
        }

        private void CheckFullDate(CustomerEditorModal modal, DateTime date, bool waitError)
        {
            modal.Birthday.BirthdayDay.ResetRawValue(date.Day.ToString());
            modal.Birthday.BirthdayMonth.ResetRawValue(date.Month.ToString());
            modal.Birthday.BirthdayYear.ResetRawValue(date.Year.ToString());
            modal.Birthday.BirthdayYear.Unfocus();
                    
            modal.Birthday.BirthdayDay.Error.WaitBool(waitError);
            modal.Birthday.BirthdayMonth.Error.WaitBool(waitError);
            modal.Birthday.BirthdayYear.Error.WaitBool(waitError);
        }
        private void CheckDayAndMonthError(CustomerEditorModal modal, string rawDay, string rawMonth, ErrorSide errorSide)
        {
            modal.Birthday.BirthdayDay.ResetRawValue(rawDay);
            modal.Birthday.BirthdayMonth.ResetRawValue(rawMonth);
            modal.AcceptButton.Click();
            
            if(errorSide == ErrorSide.Left || errorSide == ErrorSide.LeftAndRight)
                modal.Birthday.BirthdayDay.Error.WaitBool(true);
            
            if(errorSide == ErrorSide.Right || errorSide == ErrorSide.LeftAndRight)
                modal.Birthday.BirthdayMonth.Error.WaitBool(true);
        }

        [Test, Description("У пользователя должно быть заполнено одно из трех полей: имя, телефон или номер карты")]
        public void TryCreateWithEmptyNameTest()
        {
            var customerListPage =
                LoadMainPage()
                    .Do(page => page.NavigationLayout.CustomersLink.Click())
                    .GoToPage<CustomerListPage>();

            var customerEditorModal =
                customerListPage
                    .Do(page =>
                        {
                            page.EmptyPersonList.IsPresent.Wait().EqualTo(true);
                            page.EmptyPersonList.EmptyPersonListAddButton.Click();
                        })
                    .WaitModal<CustomerEditorModal>();

            customerEditorModal.Do(modal =>
                {
                    modal.AcceptButton.Click();
                    modal.Name.Error.Wait().EqualTo(true);
                    modal.Phone.Error.Wait().EqualTo(true);
                    modal.CustomId.Error.Wait().EqualTo(true);

                    modal.Name.SetRawValue("Василий");
                    modal.AcceptButton.Click();
                });

            customerListPage.WaitModalClose<CustomerEditorModal>();

            var customerPage =
                customerListPage
                    .Do(page =>
                        {
                            page.CustomerItem.Count.Wait().EqualTo(1);
                            page.CustomerItem.First().Click();
                        })
                    .GoToPage<CustomerPage>();

            customerEditorModal =
                customerPage
                    .Do(page => page.PersonHeader.PersonEditLink.Click())
                    .WaitModal<CustomerEditorModal>();

            customerEditorModal.Do(modal =>
                {
                    modal.Name.Clear();
                    modal.Phone.SetValue("79112223344");
                    modal.AcceptButton.Click();
                });

            customerPage.WaitModalClose<CustomerEditorModal>();

            customerEditorModal =
                customerPage
                    .Do(page => page.PersonHeader.PersonEditLink.Click())
                    .WaitModal<CustomerEditorModal>();

            customerEditorModal.Do(modal =>
                {
                    modal.Phone.Clear();
                    modal.CustomId.SetRawValue("Vasya");
                    modal.AcceptButton.Click();
                });

            customerPage.WaitModalClose<WorkerEditorModal>();
        }
    }
}