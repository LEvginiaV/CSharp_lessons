namespace Market.CustomersAndStaff.FrontApi.Dto.Customers
{
    public class CustomerInfoDto
    {
        public string Name { get; set; }
        public BirthdayDto Birthday { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string CustomId { get; set; }
        public decimal? Discount { get; set; }
        public GenderDto? Gender { get; set; }
        public string AdditionalInfo { get; set; }
    }
}