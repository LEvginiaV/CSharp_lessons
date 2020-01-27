using System;

namespace Market.CustomersAndStaff.Models.OnlineRecording
{
    public class PublicLink
    {
        public Guid ShopId { get; set; }
        public string Link { get; set; }
        public bool IsActive { get; set; }
    }
}