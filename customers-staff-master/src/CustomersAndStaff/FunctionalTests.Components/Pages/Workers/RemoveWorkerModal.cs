using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Pages.Workers
{
    [TidModal("RemoveWorkerModal")]
    public class RemoveWorkerModal : SimpleLightBoxModal
    {
        public RemoveWorkerModal(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }
    }
}