using Kontur.Selone.Extensions;
using Kontur.Selone.Properties;
using Kontur.Selone.Selectors.Css;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Components
{
    public abstract class ComponentBase
    {
        protected ComponentBase(ISearchContext searchContext, By by)
        {
            By = @by;
            Container = searchContext.SearchElement(@by);
        }

        public IWebElement Container { get; }
        public IProp<bool> IsPresent => Container.Present();

        private By By { get; }

        public string GetCssSelector()
        {
            var cssBy = By as CssBy;
            return cssBy != null ? cssBy.Selector : string.Empty;
        }
    }
}