using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Common
{
    public class ErrorMessageTooltip : PortalComponentBase
    {
        public ErrorMessageTooltip(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public Label ErrorMessage { get; set; }
    }
}