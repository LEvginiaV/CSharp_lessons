using System;
using System.Threading.Tasks;

using Market.CustomersAndStaff.Models.Calendar;
using Market.CustomersAndStaff.Models.ServiceCalendar;
using Market.CustomersAndStaff.Models.Validations;

namespace Market.CustomersAndStaff.Services.ServiceCalendar
{
    public interface ICalendarService
    {
        Task<(Guid recordId, ValidationResult validationResult)> CreateAsync(Guid shopId, DateTime date, Guid workerId, ServiceCalendarRecord record);
        Task<ValidationResult> UpdateAsync(Guid shopId, DateTime date, Guid workerId, ServiceCalendarRecord record, DateTime? updateDate = null, Guid? updateWorkerId = null);
        Task<ValidationResult> RemoveAsync(Guid shopId, DateTime date, Guid workerId, Guid recordId);
        Task<ShopCalendarDay<ServiceCalendarRecord>> ReadShopCalendarDayAsync(Guid shopId, DateTime date, RecordStatus? status = null);
        Task<ValidationResult> UpdateCustomerStatusAsync(Guid shopId, DateTime date, Guid workerId, Guid recordId, CustomerStatus newStatus);
    }
}