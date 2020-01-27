namespace Market.CustomersAndStaff.Models.WorkOrders
{
    public class ApplianceCustomerValue : BaseCustomerValue
    {
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Number { get; set; }
        public int? Year { get; set; }
    }
}