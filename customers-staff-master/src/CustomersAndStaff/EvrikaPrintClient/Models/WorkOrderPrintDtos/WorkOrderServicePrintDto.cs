namespace Market.CustomersAndStaff.EvrikaPrintClient.Models.WorkOrderPrintDtos
{
    public class WorkOrderServicePrintDto
    {
        public int  NaturalId { get; set; }
        public string Name { get; set; }
        public string Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Sum { get; set; }
        public string WorkerName { get; set; }
    }
}