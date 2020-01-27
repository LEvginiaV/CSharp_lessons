using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.WorkOrders;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;

namespace Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkOrderExtensions
{
    public static class ProductsItemExtensions
    {
        public static void CheckRow(this ProductsItem productsItem, string name, decimal? price, string quantity, decimal? totalPrice)
        {
            productsItem.CardName.Text.Wait().EqualTo(name);
            productsItem.Quantity.Text.Wait().EqualTo(quantity);
            productsItem.Price.Value.Wait().EqualTo(price);
            productsItem.TotalPrice.Value.Wait().EqualTo(totalPrice);
        }
    }
}