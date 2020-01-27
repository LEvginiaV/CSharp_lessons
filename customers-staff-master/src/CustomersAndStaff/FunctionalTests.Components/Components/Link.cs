using Kontur.Selone.Properties;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Components
{
    public class Link : ComponentBase
    {
        public Link(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public void Click()
        {
            Container.Click();
        }

        public IProp<string> Text => Prop.Create(() => Container.Text, "Link.Text");

        public IProp<bool> Disabled => Prop.Create(() => Container.GetAttribute("data-prop-disabled") == "true", "Link.Disabled");
    }
}
