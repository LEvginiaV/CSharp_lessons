using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Common;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.CtorAttributes;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Pages.Calendar
{
    public class ServiceItemWithRubleSign : ComponentBase
    {
        public ServiceItemWithRubleSign(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public Label ServiceName { get; set; }
        [MoneyCtor]
        public Label<decimal?> ServicePrice { get; set; }
    }
    
    public class ServiceItem : ComponentBase
    {
        public ServiceItem(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public Label ServiceName { get; set; }
        [MoneyCtor(null, false)]
        public Label<decimal?> ServicePrice { get; set; }
    }
}