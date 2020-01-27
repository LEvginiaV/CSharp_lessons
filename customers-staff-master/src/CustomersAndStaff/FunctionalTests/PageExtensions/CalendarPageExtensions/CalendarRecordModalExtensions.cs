using System;
using System.Linq;

using FluentAssertions;

using Kontur.RetryableAssertions.Extensions;

using Market.CustomersAndStaff.FunctionalTests.ComponentExtensions;
using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.Pages;
using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Calendar;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.PageExtensions.CalendarPageExtensions
{

    public class Dto
    {
        public string Name { get; set; }
        public string Age { get; set; }
        public string Color { get; set; }
        public string Race { get; set; }
        public string ParentId { get; set; }
        public string CarNumber { get; set; }
    }
    public static class CalendarRecordModalExtensions
    {
        public static CalendarRecordModal CheckServiceTokens(this CalendarRecordModal modal, params (string name, decimal? price)[] tokens)
        {
            modal.ServiceSelector.Token.WaitCount(tokens.Length, "коллекцая токенов в инпуте услуг");

            modal.ServiceSelector.Token
                           .Wait()
                           .Select(x => (
                                            x.ServiceName.Text.Get(),
                                            x.ServicePrice.IsPresent.Get() ? x.ServicePrice.Value.Get() : (decimal?)null
                                        )
                           )
                           .ShouldBeEquivalentTo(tokens);
            return modal;
        }

        public static CalendarRecordModal CheckFooterTotal(this CalendarRecordModal modal, decimal? val)
        {
            if(val == null)
            {
                modal.FooterTotal.WaitAbsence(componentDescription: "сумма в подвале редактора записи");
            }
            else
            {
                modal.FooterTotal.Value.Wait("ожидаем корректности суммы в подвале редактора записи").EqualTo(val);
            }
            return modal;
        }

        public static CalendarRecordModal CheckCustomerDiscountErrorValidation(this CalendarRecordModal modal)
        {
            modal.DiscountInput.Error.Wait().EqualTo(true);
            return modal;
        }
        
        public static CalendarRecordModal CheckCustomerPhoneErrorValidation(this CalendarRecordModal modal)
        {
            modal.AddCustomerFormPhoneInput.Error.Wait().EqualTo(true);
            return modal;
        }

        public static CalendarRecordModal CheckCustomerNameErrorValidation(this CalendarRecordModal modal)
        {
            modal.AddCustomerFormNameInput.Error.Wait().EqualTo(true);
            return modal;
        }

        public static CalendarRecordModal SetCustomerDiscount(this CalendarRecordModal modal, decimal discount)
        {
            modal.DiscountInput.ResetValue(discount);
            return modal;
        }

        public static CalendarRecordModal SelectServicesByNames(this CalendarRecordModal modal, params string[] names)
        {
            modal.ServiceSelector.SetRawValue(string.Join(Keys.Enter, names) + Keys.Enter);
            modal.ServiceSelector.Unfocus();
            return modal;
        }

        public static CalendarRecordModal RemoveLastService(this CalendarRecordModal modal)
        {
            modal.ServiceSelector.SetRawValue(Keys.Backspace + Keys.Backspace);
            modal.ServiceSelector.Unfocus();
            return modal;
        }

        public static CalendarRecordModal SelectCustomerByName(this CalendarRecordModal modal, string name)
        {
            modal.CustomerComboBox.TypeAndSelect(name);
            return modal;
        }
        
        public static CalendarRecordModal SetCustomerPhone(this CalendarRecordModal modal, string phone)
        {
            modal.AddCustomerFormPhoneInput.ResetRawValue(phone);
            return modal;
        }
        
        public static CalendarRecordModal SetCustomerNameInEditor(this CalendarRecordModal modal, string name)
        {
            modal.AddCustomerFormNameInput.ResetRawValue(name);
            return modal;
        }

        public static CalendarRecordModal CheckCustomerNameInEditor(this CalendarRecordModal modal, string name)
        {
            modal.AddCustomerFormNameInput.WaitText(name);
            return modal;
        }

        public static CalendarRecordModal CheckCustomerName(this CalendarRecordModal modal, string name)
        {
            modal.CustomerViewNameLabel.WaitText(name);
            return modal;
        }
        
        public static CalendarRecordModal CheckCustomerPhone(this CalendarRecordModal modal, string phone)
        {
            modal.CustomerViewPhoneLabel.WaitText(phone);
            return modal;
        }

        public static CalendarRecordModal CheckSuggestedCustomers(this CalendarRecordModal modal, params string[] customers)
        {
            modal.CustomerComboBox.GetMenuItemList<Label>()
                            .Count
                            .Wait($"ждем {customers.Length} элементов в выпадающем списке выбора клиента")
                            .EqualTo(customers.Length);

            modal.CustomerComboBox.GetMenuItemList<Label>()
                            .Select(x => x.Text.Get())
                            .Should().BeEquivalentTo(customers);
            return modal;
        }

        public static CalendarRecordModal SearchCustomer(this CalendarRecordModal modal, string name)
        {
            modal.CustomerComboBox.ResetRawValue(name);
            return modal;
        } 

        public static CalendarRecordModal AddNewCustomer(this CalendarRecordModal modal, string name)
        {
            modal.CustomerComboBox.ResetRawValue(name);
            modal.CustomerComboBox.GetMenuHeaderList<Label>().Count.Wait("ждем как минимум 1 пунтка в комбобоксе").MoreOrEqual(1);
            modal.CustomerComboBox.GetMenuHeaderList<Label>().First().Text.Wait().EqualTo($"+ Добавить \"{name}\"");
            modal.CustomerComboBox.GetMenuHeaderList<Label>().First().Container.Click();
            return modal;
        }

        public static CalendarRecordModal EraseWorker(this CalendarRecordModal modal)
        {
            modal.WorkerKebab.Click();
            modal.WorkerKebab.GetMenuItemList<Label>().Count.Wait().EqualTo(1);
            modal.WorkerKebab.GetMenuItemList<Label>().ElementAt(0).Container.Click();
            return modal;
        }

        public static CalendarRecordModal CancelAddingNewCustomer(this CalendarRecordModal modal)
        {
            modal.CustomerEditorKebab.Click();
            modal.CustomerEditorKebab.GetMenuItemList<Label>().Count.Wait().EqualTo(2);
            modal.CustomerEditorKebab.GetMenuItemList<Label>().ElementAt(1).Container.Click();
            return modal;
        }

        public static CalendarRecordModal EraseCustomer(this CalendarRecordModal modal)
        {
            modal.CustomerKebab.Click();
            modal.CustomerKebab.GetMenuItemList<Label>().Count.Wait().EqualTo(1);
            modal.CustomerKebab.GetMenuItemList<Label>().ElementAt(0).Container.Click();
            return modal;
        }

        public static CalendarRecordModal SetTimeRange(this CalendarRecordModal modal, string start, string end)
        {
            modal.StartTime.ResetRawValue(start);
            modal.EndTime.ResetRawValue(end);
            return modal;
        }

        public static CalendarRecordModal SetComment(this CalendarRecordModal modal, string text)
        {
            modal.Comment.ResetRawValue(text);
            return modal;
        }

        public static CalendarRecordModal CheckTimeRange(this CalendarRecordModal modal, string start, string end)
        {
            modal.StartTime.WaitText(start);
            modal.EndTime.WaitText(end);
            return modal;
        }

        public static CalendarRecordModal SetDate(this CalendarRecordModal modal, DateTime date) => modal.SetDate($"{date:dd.MM.yyyy}");

        public static CalendarRecordModal SetDate(this CalendarRecordModal modal, string date)
        {
            modal.Date.ResetRawValue(date);
            return modal;
        }

        public static CalendarRecordModal CheckDate(this CalendarRecordModal modal, DateTime date)
        {
            modal.Date.WaitText($"{date:dd.MM.yyyy}");
            return modal;
        }

        public static CalendarRecordModal CheckTimeValidation(this CalendarRecordModal modal, string message, Level level, ErrorSide side = ErrorSide.LeftAndRight)
        {
            var checkLeft = side == ErrorSide.Left || side == ErrorSide.LeftAndRight;
            var leftIsError = level == Level.Error && checkLeft;
            var leftIsWarn = level == Level.Warning && checkLeft;
            modal.StartTime.Error.Wait($"ожидаем валидацию ошибки на времени начала (наличие={leftIsError})").EqualTo(leftIsError);
            modal.StartTime.Warning.Wait($"ожидаем валидацию предупреждения на времени начала (наличие={leftIsWarn})").EqualTo(leftIsWarn);
            
            var checkRight = side == ErrorSide.Right || side == ErrorSide.LeftAndRight;
            var rightIsError = level == Level.Error && checkRight;
            var rightIsWarn = level == Level.Warning && checkRight;
            modal.EndTime.Error.Wait($"ожидаем валидацию ошибки на времени начала (наличие={rightIsError})").EqualTo(rightIsError);
            modal.EndTime.Warning.Wait($"ожидаем валидацию предупреждения на времени начала (наличие={rightIsWarn})").EqualTo(rightIsWarn);

            if(string.IsNullOrEmpty(message))
            {
                modal.TimeErrorMessage.WaitAbsence();
            }
            else
            {
                modal.TimeErrorMessage.WaitText(message, errorMessage: "ожидаем ошибку валидации времени");
            }
            return modal;
        }

        public static CalendarRecordModal CheckDateValidation(this CalendarRecordModal modal, string message, Level level)
        {
            if(level == Level.Error) modal.Date.WaitError(true, "ожидаем валидацию ошибки на выборе даты");
            if(level == Level.Warning) modal.Date.WaitWarning(true, "ожидаем валидацию предупреждения на выборе даты");

            modal.DateErrorMessage.WaitText(message, errorMessage: "ожидаем ошибку валидации даты");
            return modal;
        }

        public static CalendarRecordModal CheckWorkerInputInvalid(this CalendarRecordModal modal)
        {
            modal.WorkerComboBox.Error.Wait("ожидаем ошибку валидации комбобокса выбора сотрудника").EqualTo(true);
            return modal;
        }

        public static CalendarRecordModal CheckSelectedWorkerName(this CalendarRecordModal modal, string name)
        {
            modal.WorkerNameLabel.WaitText(name);
            return modal;
        }

        public static CalendarRecordModal SelectWorkerByName(this CalendarRecordModal modal, string name)
        {
            modal.WorkerComboBox.ResetRawValue(name);
            modal.WorkerComboBox.GetMenuItemList<Label>().Count.Wait("ожидаем выпадающий список с нужным сотрудником").EqualTo(1);
            modal.WorkerComboBox.SelectByIndex(0);
            return modal;
        }

        public static CalendarRecordModal ClickSave(this CalendarRecordModal modal)
        {
            modal.Accept.Click();
            return modal;
        }

        public static void SaveAndClose(this CalendarRecordModal modal)
        {
            modal.Accept.Click();
            modal.WaitAbsence();
        }

        public static void Close(this CalendarRecordModal modal)
        {
            modal.Cancel.Click();
            modal.WaitAbsence();
        }
    }
}