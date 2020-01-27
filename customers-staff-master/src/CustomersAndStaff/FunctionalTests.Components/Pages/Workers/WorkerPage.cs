using Kontur.Selone.Extensions;
using Kontur.Selone.Properties;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Common;
using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Pages.Workers
{
    [TidPage("WorkerPage")]
    public class WorkerPage : PageBase
    {
        public WorkerPage(IWebDriver searchContext, By @by, IComponentFactory componentFactory)
            : base(searchContext, @by, componentFactory)
        {
        }

        public PersonHeader PersonHeader { get; set; }
        public Label Code { get; set; }
        public Label Phone { get; set; }
        public TextArea Description { get; set; }
        public HelpCaption CodeHelpCaption { get; set; }
        public Link BackButton { get; set; }

        public override IProp<bool> IsPresent => Prop.Create(() => Container.Present().Get(), "WorkerPage.IsPresent");
    }
}