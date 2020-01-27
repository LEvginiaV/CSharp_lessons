using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.WorkOrders;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;

namespace Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkOrderExtensions
{
    public static class ServicesItemExtensions
    {

        public static void CheckRow(this ServicesItem servicesItem, string name, decimal? price, string quantity, decimal? totalPrice, string workerName = null)
        {
            servicesItem.CardName.Text.Wait().EqualTo(name);
            servicesItem.Quantity.Text.Wait().EqualTo(quantity);
            servicesItem.Price.Value.Wait().EqualTo(price);
            servicesItem.TotalPrice.Value.Wait().EqualTo(totalPrice);
            if(workerName != null)
            {
                servicesItem.Worker.Text.Wait().EqualTo(workerName);
            }
        }
    }
}