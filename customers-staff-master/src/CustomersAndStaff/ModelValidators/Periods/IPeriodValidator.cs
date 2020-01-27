using Market.CustomersAndStaff.Models.Calendar;
using Market.CustomersAndStaff.Models.Validations;

namespace Market.CustomersAndStaff.ModelValidators.Periods
{
    public interface IPeriodValidator<T> where T : BaseCalendarRecord
    {
        ValidationResult Validate(WorkerCalendarDay<T> calendarDay);
        ValidationResult Validate(T[] model);
        ValidationResult Validate(T model);
    }
}