using System;

namespace Market.CustomersAndStaff.FrontApi.Dto.Customers
{
    public class CustomerDto : CustomerInfoDto
    {
        public Guid Id { get; set; }
    }
}