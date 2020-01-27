using System;

namespace Market.CustomersAndStaff.FrontApi.Dto.WorkOrders
{
    public class ShopProductDto
    {
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal? Price { get; set; }
    }
}