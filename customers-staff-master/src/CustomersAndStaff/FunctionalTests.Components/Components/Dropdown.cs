using System;
using System.Linq;

using Kontur.Selone.Elements;
using Kontur.Selone.Extensions;
using Kontur.Selone.Properties;
using Kontur.Selone.Selectors.Css;
using Kontur.Selone.Selectors.XPath;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Components
{
    public class Dropdown : ComponentBase
    {
        public Dropdown(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
            button = new Button(searchContext, by);
            portal = new Portal(Container, new CssBy("noscript"));
        }

        private Button button;

        public IProp<bool> ItemListPresent => Prop.Create(() => portal.IsPresent.Get(), "ComboBox.ItemListPresent");

        public IProp<string> Text => Prop.Create(() => Container.Text, "Dropdown.Text");

        public void Click()
        {
            button.Click();
        }

        public void SelectByIndex(int index)
        {
            Click();
            GetMenuList()?.Skip(index).FirstOrDefault()?.Click();
        }

        public IElementsCollection<T> GetMenuItemList<T>() where T : ComponentBase
        {
            return new ElementsCollection<T>(portal.GetPortalElement(),
                                             x => x.XPath(".//").AnyTag().WithAttribute("data-comp-name", "MenuItem").FixedByKey(),
                                             (sc, by, we) => (T)Activator.CreateInstance(typeof(T), sc, by));
        }

        private IElementsCollection<IWebElement> GetMenuList()
        {
            return portal.GetPortalElement()?
                .SearchElements(x => x.XPath(".//").AnyTag().WithAttribute("data-comp-name", "MenuItem").FixedByKey());
        }

        private readonly Portal portal;
    }
}