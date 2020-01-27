using System;

namespace Market.CustomersAndStaff.Models.WorkOrders
{
    public class ShopProduct
    {
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal? Price { get; set; }
    }
}