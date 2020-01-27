namespace Market.CustomersAndStaff.EvrikaPrintClient.Models
{
    public class PrintTaskInfo
    {
        public string Id { get; set; }
        public string ErrorMessage { get; set; }
        public PrintTaskType Type { get; set; }
        public PrintTaskStatus Status { get; set; }
    }
}