using System;

using FluentAssertions.Extensions;

using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Infrastructure
{
    public static class WebDriverExtensions
    {
        public static T GoToUrl<T>(this IWebDriver webDriver, string url) where T : PageBase
        {
            webDriver.Navigate().GoToUrl(url);
            var page = factory.CreatePage<T>(webDriver);
            page.IsPresent.Wait($"Не дождались перехода на страницу '{typeof(T).Name}' по адресу {url}").EqualTo(true, 20.Seconds());
            Console.WriteLine($"Переход по адресу {url}");
            return page;
        }

        public static T GoToPage<T>(this IWebDriver webDriver) where T : PageBase
        {
            var page = factory.CreatePage<T>(webDriver);
            page.IsPresent.Wait($"Не дождались перехода на страницу '{typeof(T).Name}'").EqualTo(true, 20.Seconds());
            return page;
        }

        private static readonly IComponentFactory factory = new ComponentFactory();
    }
}