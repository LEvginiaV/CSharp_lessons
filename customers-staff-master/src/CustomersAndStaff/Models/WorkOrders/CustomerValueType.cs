using System.ComponentModel;

namespace Market.CustomersAndStaff.Models.WorkOrders
{
    public enum CustomerValueType
    {
        [Description("Автомобиль")]
        Vehicle,
        [Description("Техника")]
        Appliances,
        [Description("Другое")]
        Other,
    }
}