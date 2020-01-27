using System;

namespace Market.CustomersAndStaff.EvrikaPrintClient.Models.WorkOrderPrintDtos
{
    public class WorkOrderPrintDto
    {
        public string Title { get; set; }
        public DateTime ReceptionDate { get; set; }
        public DateTime CompletionDatePlanned { get; set; }
        public string WarrantyNumber { get; set; }
        public string ReceptionWorkerName { get; set; }

        public WorkOrderShopPrintDto Shop { get; set; }
        public WorkOrderClientPrintDto Client { get; set; }
        public bool HasVehicleInfo { get; set; }
        public WorkOrderVehiclePrintDto VehicleDescription { get; set; }
        public bool HasApplianceInfo { get; set; }
        public WorkOrderAppliancePrintDto ApplianceDescription { get; set; }
        public bool HasCustomerValueDescription { get; set; }
        public string CustomerValueDescription { get; set; }
        public bool HasServices { get; set; }
        public WorkOrderServicePrintDto[] Services { get; set; }
        public decimal ServicesTotalSum { get; set; }
        public bool HasProducts { get; set; }
        public WorkOrderProductPrintDto[] Products { get; set; }
        public decimal ProductsTotalSum { get; set; }
        public decimal ServicesProductsTotalSum { get; set; }
        public bool HasCustomerProducts { get; set; }
        public CustomerProductPrintDto[] CustomerProducts { get; set; }
        public bool HasComment { get; set; }
        public string Comment { get; set; }
    }
}