using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.Models.Workers;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Workers
{
    public class WorkerItem : ComponentBase
    {
        public WorkerItem(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public void Click()
        {
            Container.Click();
        }

        public Label Name { get; set; }
        public Label Position { get; set; }
        public Label DaysCounter { get; set; }
        public Label HoursCounter { get; set; }

        public Worker ToActualData()
        {
            var position = Position.Text.Get();
            return new Worker
                {
                    FullName = Name.Text.Get(),
                    Position = position.Equals("Должность не указана") ? null : position,
                };
        }
    }
}