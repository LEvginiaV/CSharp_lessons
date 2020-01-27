using System;

namespace Market.CustomersAndStaff.Models.Workers
{
    public class Worker
    {
        public Guid ShopId { get; set; }
        public Guid Id { get; set; }
        public int Code { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Position { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsAvailableOnline { get; set; }
        public string AdditionalInfo { get; set; }
    }
}