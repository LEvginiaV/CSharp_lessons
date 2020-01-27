using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Pages.Calendar
{
    [TidModal("CalendarRecordCancelModal")]
    public class CalendarRecordCancelModal : SimpleLightBoxModal
    {
        public CalendarRecordCancelModal(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public Button CancelRecord { get; set; }
        public Button DoNotCancelRecord { get; set; }
        public Label ErrorMessage { get; set; }
        
        public Label NotCome { get; set; }
        public Label CanceledBeforeEvent { get; set; }
    }
}
