namespace Market.CustomersAndStaff.FrontApi.Dto.WorkOrders
{
    public class ApplianceCustomerValueDto : BaseCustomerValueDto
    {
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Number { get; set; }
        public int? Year { get; set; }
    }
}