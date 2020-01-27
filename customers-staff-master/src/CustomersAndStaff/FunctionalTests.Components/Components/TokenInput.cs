using Kontur.Selone.Elements;

using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Calendar;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Components
{
    public class TokenInput : ComboBox
    {
        public TokenInput(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public ElementsCollection<ServiceItem> Token { get; set; }
    }
}