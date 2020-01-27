using System;
using System.Linq;

using Market.CustomersAndStaff.Models.Calendar;
using Market.CustomersAndStaff.Models.Validations;

namespace Market.CustomersAndStaff.ModelValidators.Periods
{
    public class PeriodValidator<T> : IPeriodValidator<T> where T : BaseCalendarRecord
    {
        public ValidationResult Validate(WorkerCalendarDay<T> calendarDay)
        {
            var validationResult = Validate(calendarDay.Records);
            if(!validationResult.IsSuccess)
            {
                return validationResult;
            }

            var date = calendarDay.Date;
            if(date != date.Date)
            {
                return ValidationResult.Fail("", "Expected date only");
            }

            return ValidationResult.Success();
        }

        public ValidationResult Validate(T[] array)
        {
            if(array.Length == 0)
            {
                return ValidationResult.Success();
            }

            var validationResult = array.Select(Validate).FirstOrDefault(x => !x.IsSuccess);
            if(validationResult != null)
            {
                return validationResult;
            }

            var sortedPeriods = array.OrderBy(x => x.Period.StartTime).ThenBy(x => x.Period.EndTime).ToArray();
            for(int i = 0; i < sortedPeriods.Length - 1; i++)
            {
                if(sortedPeriods[i].Period.EndTime > sortedPeriods[i + 1].Period.StartTime)
                {
                    return ValidationResult.Fail("timePeriod", "Expected non-intersected periods");
                }
            }

            return ValidationResult.Success();
        }

        public ValidationResult Validate(T model)
        {
            if (model.Period.StartTime < TimeSpan.Zero)
            {
                return ValidationResult.Fail("timePeriod", "Expected time >= 00:00:00");
            }

            if (model.Period.EndTime > TimeSpan.FromDays(1))
            {
                return ValidationResult.Fail("timePeriod", "Expected time <= 24:00:00");
            }

            if (model.Period.StartTime >= model.Period.EndTime)
            {
                return ValidationResult.Fail("timePeriod", "Expected StartTime < EndTime");
            }
            
            return ValidationResult.Success();
        }
    }
}