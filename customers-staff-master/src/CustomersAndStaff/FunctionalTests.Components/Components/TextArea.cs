using Kontur.Selone.Extensions;
using Kontur.Selone.Properties;
using Kontur.Selone.Selectors.Css;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Components
{
    public class TextArea : ComponentBase
    {
        public TextArea(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
            textArea = Container.SearchElement(new CssBy("textarea"));
        }

        public IProp<string> Text => textArea.Text();
        public IProp<int?> MaxLength => Prop.Create(() => int.TryParse(textArea.GetAttribute("maxlength"), out var length) ? length : (int?)null, 
                                                    "Input.MaxLength");


        public void Click()
        {
            Container.ScrollIntoView();
            Container.Click();
        }

        public void SetRawValue(string value)
        {
            Click();
            Container.SendKeys(value);
        }

        public void Clear()
        {
            Click();
            Container.SendKeys(Keys.Control + "a" + Keys.Delete);
        }

        public void ResetRawValue(string value)
        {
            Clear();
            Container.SendKeys(value);
        }

        private readonly IWebElement textArea;
    }
}