namespace Market.CustomersAndStaff.FrontApi.Dto.Workers
{
    public class WorkerInfoDto
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Position { get; set; }
        public string AdditionalInfo { get; set; }
        public bool IsAvailableOnline { get; set; }
    }
}