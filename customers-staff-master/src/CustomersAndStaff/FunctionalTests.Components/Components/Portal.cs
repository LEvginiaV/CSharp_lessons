using Kontur.Selone.Extensions;
using Kontur.Selone.Selectors.Css;

using OpenQA.Selenium;
using OpenQA.Selenium.Internal;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Components
{
    public class Portal : ComponentBase
    {
        public Portal(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public IWebElement GetPortalElement()
        {
            if(!IsPresent.Get())
            {
                return null;
            }
            var renderContentId = Container.GetAttribute("data-render-container-id");
            try
            {
                return UnWrapElement().WebDriver().SearchElement(new CssBy($"[data-rendered-container-id='{renderContentId}']"));
            }
            catch(NoSuchElementException)
            {
                return null;
            }
        }

        private IWebElement UnWrapElement()
        {
            var element = Container;
            while(element is IWrapsElement wrapper)
            {
                element = wrapper.WrappedElement;
            }

            return element;
        }
    }
}