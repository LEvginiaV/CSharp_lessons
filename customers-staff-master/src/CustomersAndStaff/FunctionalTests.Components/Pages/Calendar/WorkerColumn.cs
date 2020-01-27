using System.Linq;

using Kontur.Selone.Elements;
using Kontur.Selone.Extensions;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Pages.Calendar
{
    public class WorkerColumn : ComponentBase
    {
        public WorkerColumn(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public ElementsCollection<Label> HourDelimiter { get; set; }
        public ElementsCollection<Label> Record { get; set; }
    }
}