using Kontur.Selone.Elements;
using Kontur.Selone.Extensions;
using Kontur.Selone.Properties;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Workers;
using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;
using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Workers.WorkingCalendarDayEditor;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Pages.Workers
{
    [TidPage("WorkerList")]
    public class WorkerListPage : PersonListPageBase
    {
        public WorkerListPage(IWebDriver searchContext, By @by, IComponentFactory componentFactory)
            : base(searchContext, @by, componentFactory)
        {
        }

        public ElementsCollection<WorkerItem> WorkerItem { get; set; }

        public Label MonthName { get; set; }
        public Link PrevMonth { get; set; }
        public Link NextMonth { get; set; }

        public Label LoadingHover { get; set; }
        public Link ListTab { get; set; }
        public Link ChartTab { get; set; }
        public DayEditor WorkersDayEditor { get; set; }
        public ElementsCollection<CalendarRow> CalendarRow { get; set; }

        public override IProp<bool> IsPresent => Prop.Create(() => Container.Present().Get() && !Loader.IsPresent.Get(), "WorkerList.IsPresent");
    }
}
