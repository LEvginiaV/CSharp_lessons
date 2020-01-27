using Market.CustomersAndStaff.Models.ServiceCalendar;
using Market.CustomersAndStaff.Models.Validations;
using Market.CustomersAndStaff.ModelValidators.Periods;

namespace Market.CustomersAndStaff.ModelValidators.Calendar
{
    public class ServiceCalendarRecordValidator : IValidator<ServiceCalendarRecord>
    {
        private readonly IPeriodValidator<ServiceCalendarRecord> periodValidator;

        public ServiceCalendarRecordValidator(IPeriodValidator<ServiceCalendarRecord> periodValidator)
        {
            this.periodValidator = periodValidator;
        }

        public ValidationResult Validate(ServiceCalendarRecord model)
        {
            if(!string.IsNullOrWhiteSpace(model.Comment) && model.Comment.Length > 500)
            {
                return ValidationResult.Fail(nameof(model.Comment), "Expected length <= 500");
            }

            return periodValidator.Validate(model);
        }
    }
}