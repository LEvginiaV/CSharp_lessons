using Kontur.Selone.Elements;
using Kontur.Selone.Extensions;
using Kontur.Selone.Properties;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Pages.Calendar
{
    [TidPage("Calendar")]
    public class CalendarPage : PersonListPageBase
    {
        public CalendarPage(IWebDriver searchContext, By @by, IComponentFactory componentFactory)
            : base(searchContext, @by, componentFactory)
        {
        }

        public Link NextDay { get; set; }
        public Link PrevDay { get; set; }
        public Link TimeZoneLink { get; set; }
        public Label DataLoaded { get; set; }
        public Label EmptyCalendarMessage { get; set; }
        public ElementsCollection<WorkerColumn> WorkerColumn { get; set; }
        public CalendarRecordTooltip RecordTooltip { get; set; }
        public Switcher WorkingFilter { get; set; }
        public NowLine NowLine { get; set; }

        public override IProp<bool> IsPresent => Prop.Create(() => Container.Present().Get() && !Loader.IsPresent.Get(), "Calendar.IsPresent");
    }
}