using Kontur.Selone.Properties;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Components
{
    public class Label : ComponentBase
    {
        public Label(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public IProp<string> Text => Prop.Create(() => Container.Text, "Label.Text");
    }
}