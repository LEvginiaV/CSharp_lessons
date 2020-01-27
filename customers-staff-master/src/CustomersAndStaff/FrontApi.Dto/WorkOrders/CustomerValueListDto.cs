namespace Market.CustomersAndStaff.FrontApi.Dto.WorkOrders
{
    public class CustomerValueListDto
    {
        public CustomerValueTypeDto CustomerValueType { get; set; }
        public BaseCustomerValueDto[] CustomerValues { get; set; }
    }
}