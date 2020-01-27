using System;

namespace Market.CustomersAndStaff.Models.WorkOrders
{
    public class WorkOrder
    {
        public Guid Id { get; set; }
        public WorkOrderDocumentStatus DocumentStatus { get; set; }
        public WorkOrderNumber Number { get; set; }
        public ShopRequisites ShopRequisites { get; set; }
        public DateTime ReceptionDate { get; set; }
        public DateTime CompletionDatePlanned { get; set; }
        public DateTime? CompletionDateFact { get; set; }
        public string WarrantyNumber { get; set; }
        public Guid? ReceptionWorkerId { get; set; }
        public WorkOrderStatus Status { get; set; }
        public Guid ClientId { get; set; }
        public CustomerValueList CustomerValues { get; set; }
        public CustomerProduct[] CustomerProducts { get; set; }
        public ShopProduct[] ShopProducts { get; set; }
        public ShopService[] ShopServices { get; set; }
        public Guid? DeliveryWorkerId { get; set; }
        public DateTime WarrantyDate { get; set; }
        public decimal TotalSum { get; set; }
        public Guid? FirstProductId { get; set; }
        public string AdditionalText { get; set; }
    }
}