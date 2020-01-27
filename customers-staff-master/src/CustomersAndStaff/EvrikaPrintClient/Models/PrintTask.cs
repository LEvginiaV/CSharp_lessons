namespace Market.CustomersAndStaff.EvrikaPrintClient.Models
{
    public class PrintTask
    {
        public PrintOutputFormat OutputFormat { get; set; }
        public string TemplateId { get; set; }
        public object Data { get; set; }
    }
}