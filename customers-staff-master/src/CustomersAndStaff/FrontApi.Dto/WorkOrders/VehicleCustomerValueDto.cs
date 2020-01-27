namespace Market.CustomersAndStaff.FrontApi.Dto.WorkOrders
{
    public class VehicleCustomerValueDto : BaseCustomerValueDto
    {
        public string Vin { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string RegisterSign { get; set; }
        public int? Year { get; set; }
        public string BodyNumber { get; set; }
        public string EngineNumber { get; set; }
    }
}