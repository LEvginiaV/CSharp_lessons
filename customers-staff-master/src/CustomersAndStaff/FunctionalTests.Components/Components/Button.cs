using Kontur.Selone.Properties;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Components
{
    public class Button : ComponentBase
    {
        public Button(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }
        
        public IProp<bool> Checked => Prop.Create(() => Container.GetAttribute("data-prop-checked") == "true", "Button.Checked");

        public IProp<string> Text => Prop.Create(() => Container.Text, "Button.Text");

        public void Click()
        {
            Container.Click();
        }
    }
}