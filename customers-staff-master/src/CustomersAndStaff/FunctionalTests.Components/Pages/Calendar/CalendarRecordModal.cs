using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Common;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.CtorAttributes;
using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Pages.Calendar
{
    [TidModal("CalendarRecordModal")]
    public class CalendarRecordModal : SimpleLightBoxModal
    {
        public CalendarRecordModal(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public Input StartTime { get; set; }
        public Input EndTime { get; set; }
        public DatePicker Date { get; set; }
        public TextArea Comment { get; set; }

        public Label TimeErrorMessage { get; set; }
        public Label DateErrorMessage { get; set; }

        public Dropdown WorkerKebab { get; set; }
        public ComboBox WorkerComboBox { get; set; }
        public Label WorkerNameLabel { get; set; }
        
        public Dropdown CustomerEditorKebab { get; set; }
        public Dropdown CustomerKebab { get; set; }
        public ComboBox CustomerComboBox { get; set; }
        public TokenInput ServiceSelector { get; set; }

        public Input AddCustomerFormNameInput { get; set; }
        public Label CustomerViewNameLabel { get; set; }

        public Input AddCustomerFormPhoneInput { get; set; }
        public Label CustomerViewPhoneLabel { get; set; }
        
        [PercentCtor]
        public Label<decimal?> CustomerViewDiscountLabel { get; set; }

        [MoneyCtor(null, true)]
        public Label<decimal?> FooterTotal { get; set; }
        
        [PercentCtor]
        public Input<decimal?> DiscountInput { get; set; }
    }
}