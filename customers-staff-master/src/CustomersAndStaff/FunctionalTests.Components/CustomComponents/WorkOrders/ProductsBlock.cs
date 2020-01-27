using Kontur.Selone.Elements;

using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Common;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.CtorAttributes;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.WorkOrders
{
    public class ProductsBlock : SpoilerBlockBase
    {
        public ProductsBlock(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public ElementsCollection<ProductsItem> RowItem { get; set; }
        [MoneyCtor(useRubleSign: false)]
        public Label<decimal?> TotalSum { get; set; }
        [MoneyCtor(useRubleSign: false)]
        public Label<decimal?> SumForServicesAndProducts { get; set; }
    }
}