using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.Models.Validations;

namespace Market.CustomersAndStaff.ModelValidators.Customers
{
    public class BirthdayValidator : IValidator<Birthday>
    {
        public ValidationResult Validate(Birthday model)
        {
            if (model.Year > 9999 || model.Year < 1000)
            {
                return ValidationResult.Fail(nameof(model.Year), "Expected 1000 <= year <= 9999");
            }

            if(model.Month < 1 || model.Month > 12)
            {
                return ValidationResult.Fail(nameof(model.Month), "Expected 1 <= month <= 12");
            }

            var numArray = IsLeapYear(model.Year) ? daysToMonthLeap : daysToMonth;
            if(model.Day < 1 || model.Day > numArray[model.Month])
            {
                return ValidationResult.Fail(nameof(model.Day), $"Expected 1 <= day <= {numArray[model.Month]}");
            }

            return ValidationResult.Success();
        }

        private static bool IsLeapYear(int? year)
        {
            if (year == null)
                return true;
            if (year % 4 != 0)
                return false;
            if (year % 100 == 0)
                return year % 400 == 0;
            return true;
        }

        private static readonly int[] daysToMonth = {
                0,
                31,
                28,
                31,
                30,
                31,
                30,
                31,
                31,
                30,
                31,
                30,
                31,
            };

        private static readonly int[] daysToMonthLeap = {
                0,
                31,
                29,
                31,
                30,
                31,
                30,
                31,
                31,
                30,
                31,
                30,
                31,
            };
    }
}