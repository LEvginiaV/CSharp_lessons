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
    public class ComboBox : ComponentBase
    {
        public ComboBox(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
            input = Container.SearchElement(new CssBy("input"));
            portal = new Portal(Container, new CssBy("noscript"));
        }
        public IProp<string> Text => Prop.Create(() => Container.GetAttribute("data-prop-textvalue"), "ComboBox.Text");

        public IProp<int?> MaxLength => Prop.Create(() => int.TryParse(input.GetAttribute("maxlength"), out var length) ? length : (int?)null,
                                                    "Input.MaxLength");

        public IProp<bool> ItemListPresent => Prop.Create(() => portal.IsPresent.Get(), "ComboBox.ItemListPresent");

        public IProp<bool> Error => Prop.Create(() => Container.GetAttribute("data-prop-error") == "true", "ComboBox.Error");

        public void Click()
        {
            Container.Click();
        }

        public void SetRawValue(string value)
        {
            Click();
            input.SendKeys(value);
        }

        public void Clear()
        {
            Click();
            input.SendKeys(Keys.Control + "a" + Keys.Delete);
        }

        public void ResetRawValue(string value)
        {
            Clear();
            input.SendKeys(value);
        }

        public void Unfocus()
        {
            input.SendKeys(Keys.Tab);
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
        
        public IElementsCollection<T> GetMenuHeaderList<T>() where T : ComponentBase
        {
            return new ElementsCollection<T>(portal.GetPortalElement(),
                                             x => x.XPath(".//").AnyTag().WithAttribute("data-comp-name", "MenuHeader").FixedByKey(),
                                             (sc, by, we) => (T)Activator.CreateInstance(typeof(T), sc, by));
        }

        private IElementsCollection<IWebElement> GetMenuList()
        {
            return portal.GetPortalElement()?
                .SearchElements(new CssBy("[data-comp-name='MenuItem']").FixedByIndex());
        }

        protected readonly IWebElement input;
        private readonly Portal portal;
    }
}