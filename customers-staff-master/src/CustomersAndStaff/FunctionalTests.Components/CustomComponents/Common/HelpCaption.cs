using Market.CustomersAndStaff.FunctionalTests.Components.Components;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Common
{
    public class HelpCaption : ComponentBase
    {
        public HelpCaption(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }
        
        public Tooltip HelpTooltip { get; set; }
    }
}