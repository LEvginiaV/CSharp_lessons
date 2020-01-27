namespace Market.CustomersAndStaff.EvrikaPrintClient.Models.WorkOrderPrintDtos
{
    public class WorkOrderProductPrintDto
    {
        public int NaturalId { get; set; }
        public string Name { get; set; }
        public string Quantity { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public decimal Sum { get; set; }
    }
}