using System;

namespace Market.CustomersAndStaff.Models.Customers
{
    public class Customer
    {
        public Guid OrganizationId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Birthday Birthday { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string CustomId { get; set; }
        public decimal? Discount { get; set; }
        public Gender? Gender { get; set; }
        public bool IsDeleted { get; set; }
        public string AdditionalInfo { get; set; }
    }
}