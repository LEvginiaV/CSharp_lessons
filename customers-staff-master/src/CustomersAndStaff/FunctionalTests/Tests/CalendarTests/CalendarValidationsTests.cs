using System;
using System.Threading.Tasks;

using FluentAssertions;

using Market.CustomersAndStaff.FunctionalTests.Components.Pages;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions.CalendarPageExtensions;
using Market.CustomersAndStaff.Models.Common;
using Market.CustomersAndStaff.Utils.Extensions;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.CalendarTests
{
    public class CalendarValidationsTests : CalendarTestBase
    {
        [Test]
        [Description("Обязательность полей Дата записи, Время записи")]
        public async Task CheckRequiredFields()
        {
             var workerId = await CreateWorker();

            var modal = GoToTomorrowCalendarPage()
                 .OpenCalendarModalAtHour(0, DateTime.Now.Hour);

            modal.SetTimeRange("", "")
                 .SetDate("")
                 .ClickSave()
                 .CheckTimeValidation("Укажите время записи", Level.Error)
                 .CheckDateValidation("Укажите дату", Level.Error);
                 
            modal.SetTimeRange("10:00", "11:00")
                 .SetDate(DateHelper.Tomorrow())
                 .SaveAndClose();

            var record = await ReadSingleRecordTomorrow(workerId);
            record.Period.Should().BeEquivalentTo(TimePeriod.CreateByHours(10, 11));
        }

        [Test]
        [Description("Валидация времени записи")]
        public async Task TimeSelectorValidations()
        {
             var workerId = await CreateWorker();
             await CreateOneWorkingDay(workerId, DateHelper.Tomorrow(), TimePeriod.CreateByHours(10, 24));

             var modal = GoToTomorrowCalendarPage()
                  .OpenCalendarModalAtHour(0, 12);

             modal.CheckTimeRange("12:00", "13:00");

             modal.SetTimeRange("13:00", "24:01")
                  .ClickSave()
                  .CheckTimeValidation("В земных сутках нет такого времени", Level.Error, ErrorSide.Right);

             modal.SetTimeRange("14:00", "13:00")
                  .ClickSave()
                  .CheckTimeValidation("Начало периода не должно быть позже конца", Level.Error);

             modal.SetTimeRange("13:00", "13:14")
                  .ClickSave()
                  .CheckTimeValidation("Нельзя сделать запись короче 15 минут", Level.Error);

             modal.SetTimeRange("", "")
                  .ClickSave()
                  .CheckTimeValidation("Укажите время записи", Level.Error);

             modal.SetTimeRange("14:00", "")
                  .ClickSave()
                  .CheckTimeValidation("Укажите время записи", Level.Error, ErrorSide.Right);

             modal.SetTimeRange("", "13")
                  .ClickSave()
                  .CheckTimeValidation("Укажите время записи", Level.Error, ErrorSide.Left);

             modal.SetTimeRange("11:00", "24:00")
                  .SetComment("")
                  .CheckTimeValidation(null, Level.Ok, ErrorSide.LeftAndRight);
        }

        [Test]
        [Description("Валидация даты записи, запись в прошлом, в будущем, за границей будущего")]
        public async Task DateSelectorValidations()
        {
            var workerId = await CreateWorker();
            await CreateOneWorkingDay(workerId, DateTime.Now.AddDays(-1), TimePeriod.CreateByHours(10, 16));
            await CreateOneWorkingDay(workerId, DateHelper.Tomorrow(), TimePeriod.CreateByHours(10, 16));
            
            var page = GoToTomorrowCalendarPage();
            var modal = page.OpenCalendarModalAtHour(0, 12);

            modal.CheckDate(DateHelper.Tomorrow());

            modal.SetDate(DateTime.Now.AddDays(366))
                 .ClickSave()
                 .CheckDateValidation("Дата слишком далеко в будущем", Level.Error);

            modal.SetDate("31.02.2019")
                 .ClickSave()
                 .CheckDateValidation("", Level.Error);
                 
            modal.SetDate(DateTime.Now.AddDays(-1))
                 .CheckDateValidation("Дата в прошлом", Level.Warning)
                 .SaveAndClose();

            page = page.GoToPrevDay().GoToPrevDay();

            modal = page.CheckRecordsCount(0, 1)
                        .ChangeRecord(0, 0);
            
            modal.SetDate(DateTime.Now.AddDays(365))
                 .CheckTimeValidation("Попадет на нерабочее время сотрудника", Level.Warning)
                 .SaveAndClose();

            await CheckSingleRecord(workerId, DateTime.Now.AddDays(365), TimePeriod.CreateByHours(12, 13));
        }

        [Test]
        [Description("Валидация на пустое поле при выборе другого сотрудника при создании записи")]
        public async Task ChangeWorkerInputValidation()
        {
            var workerId = await CreateWorker();
            await CreateOneWorkingDay(workerId, DateHelper.Tomorrow(), TimePeriod.CreateByHours(10, 16));
            await CreateSingleRecord(workerId, DateHelper.Tomorrow(), TimePeriod.CreateByHours(12, 13));

            var modal = GoToTomorrowCalendarPage()
                 .ChangeRecord(0, 0);
            
            modal.EraseWorker()
                 .ClickSave()
                 .CheckWorkerInputInvalid();
        }

        [Test]
        [Description("Валидация при создании нового клиента")]
        public async Task CreatingNewCustomersValidations()
        {
            var workerId = await CreateWorker();
            await CreateOneWorkingDay(workerId, DateHelper.Tomorrow(), TimePeriod.CreateByHours(10, 16));

            var modal = GoToTomorrowCalendarPage()
                 .OpenCalendarModalAtHour(0, 12);

            modal.AddNewCustomer("Перчатка бесконечности")
                 .CheckCustomerNameInEditor("Перчатка бесконечности")
                 .SetCustomerNameInEditor("")
                 .ClickSave();

            modal.CheckCustomerNameErrorValidation()
                 .CheckCustomerPhoneErrorValidation()
                 .SetCustomerDiscount(10.1m)
                 .ClickSave();
            
            modal.CheckCustomerNameErrorValidation()
                 .CheckCustomerPhoneErrorValidation();

            var workerCalendarDay = await ReadWorkerCalendarDay(workerId, DateHelper.Tomorrow());
            workerCalendarDay.Records.Length.Should().Be(0);
        }
    }
}