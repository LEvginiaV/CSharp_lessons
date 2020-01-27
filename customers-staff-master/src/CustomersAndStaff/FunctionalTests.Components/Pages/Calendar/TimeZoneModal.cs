using System.Linq;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Pages.Calendar
{
    [TidModal("TimeZoneModal")]
    public class TimeZoneModal : SimpleLightBoxModal
    {
        public TimeZoneModal(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }
        public Dropdown TimeZoneSelect { get; set; }
        public Label TimeZoneLoader { get; set; }
    }
}