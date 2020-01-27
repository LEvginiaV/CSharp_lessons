using System;

namespace Market.CustomersAndStaff.FrontApi.Dto.WorkOrders
{
    public class WorkOrderDto
    {
        public WorkOrderNumberDto Number { get; set; }
        public ShopRequisitesDto ShopRequisites { get; set; }
        public DateTime ReceptionDate { get; set; }
        public DateTime CompletionDatePlanned { get; set; }
        public DateTime? CompletionDateFact { get; set; }
        public string WarrantyNumber { get; set; }
        public Guid? ReceptionWorkerId { get; set; }
        public WorkOrderStatusDto Status { get; set; }
        public Guid ClientId { get; set; }
        public CustomerValueListDto CustomerValues { get; set; }
        public CustomerProductDto[] CustomerProducts { get; set; }
        public ShopProductDto[] ShopProducts { get; set; }
        public ShopServiceDto[] ShopServices { get; set; }
        public DateTime WarrantyDate { get; set; }
        public string AdditionalText { get; set; }
    }
}