using System;

namespace Market.CustomersAndStaff.FrontApi.Dto.WorkOrders
{
    public class ShopServiceDto
    {
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public TimeSpan ServiceTime { get; set; }
        public decimal? Price { get; set; }
        public Guid? WorkerId { get; set; }
    }
}