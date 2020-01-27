using System.Threading.Tasks;

using FluentAssertions;

using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Calendar;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions.CalendarPageExtensions;
using Market.CustomersAndStaff.Models.Common;
using Market.CustomersAndStaff.Models.ServiceCalendar;
using Market.CustomersAndStaff.Utils.Extensions;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.CalendarTests
{
    public class CalendarRecordStateTests : CalendarTestBase
    {
        [Test]
        [Description("Смена статусов. 'Напомнили', 'Выполнили', 'Записали'")]
        public async Task ChangeRecordStatus()
        {
            var workerId = await CreateWorker();
            await CreateOneWorkingDay(workerId, DateHelper.Tomorrow(), TimePeriod.CreateByHours(10, 16));
            await CreateSingleRecord(workerId, DateHelper.Tomorrow(), TimePeriod.CreateByHours(13, 14));
            
            var page = GoToTomorrowCalendarPage();
            
            page.ShowRecordTooltip(0, 0)
                .CheckIsActiveState()
                .SetMessageState();
            
            page.ShowRecordTooltip(0, 0)
                .CheckIsMessageState()
                .SetCompletedState();
            
            page.ShowRecordTooltip(0, 0)
                .CheckIsCompletedState()
                .SetActiveState();

            page.ShowRecordTooltip(0, 0)
                .CheckIsActiveState();
        }

        [Test]
        [Description("Отмена записи")]
        public async Task RecordCancellation()
        {
            var workerId = await CreateWorker();
            await CreateOneWorkingDay(workerId, DateHelper.Tomorrow(), TimePeriod.CreateByHours(10, 16));
            await CreateSingleRecord(workerId, DateHelper.Tomorrow(), TimePeriod.CreateByHours(13, 14));
            
            var page = GoToTomorrowCalendarPage();
            
            page.ShowRecordTooltip(0, 0)
                .ClickCancel();

            var cancelModal = page.WaitModal<CalendarRecordCancelModal>();

            cancelModal.ClickCancelRecord(false)
                       .CheckValidationMessage("Укажите причину отмены")
                       .SelectNotComeReason()
                       .ClickCancelRecord();

            page.CheckRecordsCount(0, 0);

            var record = await ReadSingleRecordTomorrow(workerId);

            record.RecordStatus.Should().Be(RecordStatus.Canceled);
            record.CustomerStatus.Should().Be(CustomerStatus.NotCome);
        }

        [Test]
        [Description("Нажимаем 'Не отменять' в лайтбоксе отмены записи")]
        public async Task CheckDoNotCancel()
        {
            var workerId = await CreateWorker();
            await CreateOneWorkingDay(workerId, DateHelper.Tomorrow(), TimePeriod.CreateByHours(10, 16));
            await CreateSingleRecord(workerId, DateHelper.Tomorrow(), TimePeriod.CreateByHours(13, 14));
            
            var page = GoToTomorrowCalendarPage();
            
            page.ShowRecordTooltip(0, 0)
                .ClickCancel();

            page.WaitModal<CalendarRecordCancelModal>().ClickDoNotCancelRecord();
            
            var record = await ReadSingleRecordTomorrow(workerId);

            record.RecordStatus.Should().Be(RecordStatus.Active);
        }
    }
}
