using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Common;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.CtorAttributes;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.WorkOrders
{
    public class ProductsItem : ComponentBase
    {
        public ProductsItem(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public Label CardName { get; set; }

        public ComboBox CardNameSelector { get; set; }

        [MoneyCtor(useRubleSign: false)]
        public Input<decimal?> Price { get; set; }

        public Input Quantity { get; set; }

        [MoneyCtor(useRubleSign: false)]
        public Label<decimal?> TotalPrice { get; set; }
        
        public Link RemoveLink { get; set; }

        public ErrorMessageTooltip PriceValidation { get; set; }
        public ErrorMessageTooltip QuantityValidation { get; set; }
    }
}