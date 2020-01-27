using System;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Common;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.CtorAttributes;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.WorkOrders
{
    public class WorkOrderItem : ComponentBase
    {
        public WorkOrderItem(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public void Click()
        {
            Container.Click();
        }

        public Label Name { get; set; }
        public Label Customer { get; set; }
        public Label Description { get; set; }
        [DateCtor]
        public Label<DateTime?> ReceptionDate { get; set; }
        [MoneyCtor]
        public Label<decimal?> TotalSum { get; set; }
        public OrderStatusSelector Status { get; set; }
    }
}