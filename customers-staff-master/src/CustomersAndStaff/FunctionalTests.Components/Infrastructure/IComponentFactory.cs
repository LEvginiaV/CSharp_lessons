using Kontur.Selone.Elements;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure
{
    public interface IComponentFactory
    {
        T CreatePage<T>(IWebDriver searchContext) where T : PageBase;
        T CreateComponent<T>(ISearchContext globalContext, ISearchContext searchContext, string tid) where T : ComponentBase;
        ElementsCollection<T> CreateCollection<T>(ISearchContext globalContext, ISearchContext searchContext, string tid) where T : ComponentBase;
    }
}