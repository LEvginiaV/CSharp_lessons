using System;

namespace Market.CustomersAndStaff.Models.WorkOrders
{
    public class ShopService
    {
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public TimeSpan ServiceTime { get; set; }
        public decimal? Price { get; set; }
        public Guid? WorkerId { get; set; }
    }
}