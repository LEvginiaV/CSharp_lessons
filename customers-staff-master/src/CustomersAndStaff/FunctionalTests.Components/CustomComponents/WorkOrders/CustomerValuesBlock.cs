using Market.CustomersAndStaff.FunctionalTests.Components.Components;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.WorkOrders
{
    public class CustomerValuesBlock : SpoilerBlockBase
    {
        public CustomerValuesBlock(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public Dropdown TypeSelector { get; set; }
        public VehicleValue VehicleValue { get; set; }
        public ApplianceValue ApplianceValue { get; set; }
        public OtherValue OtherValue { get; set; }
    }
}