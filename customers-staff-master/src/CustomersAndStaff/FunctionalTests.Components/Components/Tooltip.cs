using Kontur.Selone.Properties;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Components
{
    public class Tooltip : ComponentBase
    {
        public Tooltip(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public IProp<bool> IsOpened => Prop.Create(() => Container.GetAttribute("data-prop-opened") == "true", "Tooltip.IsOpened");
    }
}