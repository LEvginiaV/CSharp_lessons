using Kontur.Selone.Elements;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.WorkOrders
{
    public class CustomerProductsBlock : SpoilerBlockBase
    {
        public CustomerProductsBlock(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public ElementsCollection<CustomerProductsItem> RowItem { get; set; }
    }
}