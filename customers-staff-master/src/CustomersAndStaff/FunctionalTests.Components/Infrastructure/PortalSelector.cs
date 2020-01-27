using System.Collections.ObjectModel;
using System.Linq;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure
{
    public class PortalSelector : By
    {
        public PortalSelector(ISearchContext globalContext, By renderInfoSelector)
        {
            this.renderInfoSelector = renderInfoSelector;
            this.globalContext = globalContext;
        }

        public override IWebElement FindElement(ISearchContext context)
        {
            var element = context.FindElement(renderInfoSelector);
            return GetPortalElement(element);
        }

        public override ReadOnlyCollection<IWebElement> FindElements(ISearchContext context)
        {
            var elements = context.FindElements(renderInfoSelector);
            return new ReadOnlyCollection<IWebElement>(
                elements.Select(GetPortalElement).ToList()
            );
        }

        private IWebElement GetPortalElement(IWebElement renderedInfo)
        {
            var containerId = renderedInfo.GetAttribute("data-render-container-id");
            return globalContext.FindElement(CssSelector($"[data-rendered-container-id='{containerId}']"));
        }

        private readonly ISearchContext globalContext;
        private readonly By renderInfoSelector;
    }
}