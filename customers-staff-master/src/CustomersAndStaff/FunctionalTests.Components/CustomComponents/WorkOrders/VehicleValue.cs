using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Common;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.CtorAttributes;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.WorkOrders
{
    public class VehicleValue : ComponentBase
    {
        public VehicleValue(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public Input Brand { get; set; }
        public Input Model { get; set; }
        public Input RegisterSign { get; set; }
        public Input Vin { get; set; }
        public Input BodyNumber { get; set; }
        public Input EngineNumber { get; set; }
        [IntCtor]
        public Input<int?> Year { get; set; }
        public TextArea Comment { get; set; }

        public ErrorMessageTooltip RegisterSignValidation { get; set; }
        public ErrorMessageTooltip VinValidation { get; set; }
    }
}