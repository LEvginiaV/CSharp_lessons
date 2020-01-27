namespace Market.CustomersAndStaff.EvrikaPrintClient.Models.WorkOrderPrintDtos
{
    public class WorkOrderVehiclePrintDto
    {
        public string Vin { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string RegisterSign { get; set; }
        public int? Year { get; set; }
        public string BodyNumber { get; set; }
        public string EngineNumber { get; set; }
        public string Comment { get; set; }
    }
}