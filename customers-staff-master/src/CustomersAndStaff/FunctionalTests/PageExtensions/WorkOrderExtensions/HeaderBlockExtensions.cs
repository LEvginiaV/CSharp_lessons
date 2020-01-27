using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.WorkOrders;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;

namespace Market.CustomersAndStaff.FunctionalTests.PageExtensions.WorkOrderExtensions
{
    public static class HeaderBlockExtensions
    {
        public static void SetOrderNumber(this HeaderBlock headerBlock, string series, string number)
        {
            headerBlock.SeriesInput.SetRawValue(series);
            headerBlock.NumberInput.SetRawValue(number);
        }

        public static void CheckOrderNumber(this HeaderBlock headerBlock, string series, string number)
        {
            headerBlock.SeriesInput.Text.Wait().EqualTo(series);
            headerBlock.NumberInput.Text.Wait().EqualTo(number);
        }
    }
}