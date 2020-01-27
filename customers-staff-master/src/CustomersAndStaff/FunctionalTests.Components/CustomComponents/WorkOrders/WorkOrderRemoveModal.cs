using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.WorkOrders
{
    [TidModal("RemoveModal")]
    public class WorkOrderRemoveModal : ModalBase
    {
        public WorkOrderRemoveModal(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public Button Accept { get; set; }
        public Button Cancel { get; set; }
    }
}