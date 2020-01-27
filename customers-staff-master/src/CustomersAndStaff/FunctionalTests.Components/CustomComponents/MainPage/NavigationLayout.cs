using Market.CustomersAndStaff.FunctionalTests.Components.Components;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.MainPage
{
    public class NavigationLayout : ComponentBase
    {
        public NavigationLayout(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public Link WorkersLink { get; set; }
        public Link CustomersLink { get; set; }
        public Link CalendarLink { get; set; }
        public Link OrdersLink { get; set; }
    }
}