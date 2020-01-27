using System.ComponentModel;

namespace Market.CustomersAndStaff.Models.WorkOrders
{
    public enum WorkOrderStatus
    {
        [Description("Новый заказ")]
        New,
        [Description("В работе")]
        InProgress,
        [Description("Выполнен")]
        Completed,
        [Description("Выдан клиенту")]
        IssuedToClient,
    }
}