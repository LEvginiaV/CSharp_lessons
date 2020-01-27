using Kontur.Selone.Properties;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Common
{
    public class Avatar : ComponentBase
    {
        public Avatar(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public IProp<string> Text => Prop.Create(() => Container.Text, "Avatar.Text");
    }
}