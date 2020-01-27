using Market.CustomersAndStaff.FunctionalTests.Components.Components;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Common
{
    public class EmptyPersonList : ComponentBase
    {
        public EmptyPersonList(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public Button EmptyPersonListAddButton { get; set; }
    }
}