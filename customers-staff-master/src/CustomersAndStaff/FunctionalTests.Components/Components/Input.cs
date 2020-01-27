using Kontur.Selone.Extensions;
using Kontur.Selone.Properties;
using Kontur.Selone.Selectors.Css;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Components
{
    public class Input : ComponentBase
    {
        public Input(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
            input = Container.SearchElement(new CssBy("input"));
        }

        public IProp<bool> Error => Prop.Create(() => Container.GetAttribute("data-prop-error") == "true", "Input.Error");
        public IProp<bool> Warning => Prop.Create(() => Container.GetAttribute("data-prop-warning") == "true", "Input.Warning");
        public IProp<string> Text => input.Value();
        public IProp<int?> MaxLength => Prop.Create(() => int.TryParse(input.GetAttribute("maxlength"), out var length) ? length : (int?)null, "Input.MaxLength");

        public void Click()
        {
            input.Click();
        }

        public void Unfocus()
        {
            input.SendKeys(Keys.Tab);
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

        public void MouseOver()
        {
            input.Mouseover();
        }

        private readonly IWebElement input;
    }
}