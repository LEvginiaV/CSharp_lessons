using System;

namespace Market.CustomersAndStaff.FrontApi.Dto.WorkOrders
{
    public class WorkOrderItemDto
    {
        public Guid Id { get; set; }
        public WorkOrderNumberDto Number { get; set; }
        public WorkOrderStatusDto Status { get; set; }
        public DateTime ReceptionDate { get; set; }
        public decimal TotalSum { get; set; }
        public Guid? FirstProductId { get; set; }
        public Guid ClientId { get; set; }
    }
}