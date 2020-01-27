namespace Market.CustomersAndStaff.Models.WorkOrders
{
    public class CustomerValueList
    {
        public CustomerValueType CustomerValueType { get; set; }
        public BaseCustomerValue[] CustomerValues { get; set; }
    }
}