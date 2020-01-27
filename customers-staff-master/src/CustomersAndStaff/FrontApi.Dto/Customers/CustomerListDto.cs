namespace Market.CustomersAndStaff.FrontApi.Dto.Customers
{
    public class CustomerListDto
    {
        public CustomerDto[] Customers { get; set; }
        public int Version { get; set; }
    }
}