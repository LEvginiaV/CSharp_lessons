using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Common;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.CtorAttributes;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.WorkOrders
{
    public class ApplianceValue : ComponentBase
    {
        public ApplianceValue(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public Input Brand { get; set; }
        public Input Model { get; set; }
        public Input Name { get; set; }
        public Input Number { get; set; }
        [IntCtor]
        public Input<int?> Year { get; set; }
        public TextArea Comment { get; set; }

        public ErrorMessageTooltip NameValidation { get; set; }
    }
}