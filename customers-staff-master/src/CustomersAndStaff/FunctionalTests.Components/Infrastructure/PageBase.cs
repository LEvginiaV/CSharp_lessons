using System;
using System.Linq;

using Kontur.Selone.Properties;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure
{
    public abstract class PageBase : ComponentBase
    {
        protected PageBase(IWebDriver searchContext, By @by, IComponentFactory componentFactory)
            : base(searchContext, @by)
        {
            this.ComponentFactory = componentFactory;
            WebDriver = searchContext;
        }

        public new abstract IProp<bool> IsPresent { get; }

        public readonly IWebDriver WebDriver;

        public readonly IComponentFactory ComponentFactory;
    }
}