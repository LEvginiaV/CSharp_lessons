namespace Market.CustomersAndStaff.FrontApi.Dto.Print
{
    public class PrintTaskInfoDto
    {
        public string Id { get; set; }
        public string ErrorMessage { get; set; }
        public PrintTaskStatusDto Status { get; set; }
    }
}