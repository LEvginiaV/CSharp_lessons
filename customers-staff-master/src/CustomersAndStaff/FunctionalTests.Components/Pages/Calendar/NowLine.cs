using Kontur.Selone.Properties;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Pages.Calendar
{
    public class NowLine : ComponentBase
    {
        public NowLine(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }
        
        public IProp<int> Top => Prop.Create(() => int.Parse(Container.GetAttribute("data-top")), "NowLine.Top");
    }
}