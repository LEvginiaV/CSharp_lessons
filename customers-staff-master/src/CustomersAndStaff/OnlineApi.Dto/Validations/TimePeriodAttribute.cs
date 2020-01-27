using System;
using System.ComponentModel.DataAnnotations;

using Kontur.Utilities.Convertions.Time;

using Market.CustomersAndStaff.OnlineApi.Dto.Common;

namespace Market.CustomersAndStaff.OnlineApi.Dto.Validations
{
    public class TimePeriodAttribute : ValidationAttribute
    {
        public TimePeriodAttribute(string minPeriod, string maxPeriod)
        {
            this.minPeriod = TimeSpan.Parse(minPeriod);
            this.maxPeriod = TimeSpan.Parse(maxPeriod);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var timePeriod = (TimePeriodDto)value;

            if(timePeriod.StartTime < 0.Hours() || timePeriod.StartTime > 24.Hours())
            {
                return new ValidationResult($"Start time must be >= 00:00:00 and <= 24:00:00");
            }

            if (timePeriod.EndTime < 0.Hours() || timePeriod.EndTime > 24.Hours())
            {
                return new ValidationResult($"End time must be >= 00:00:00 and <= 24:00:00");
            }

            if (timePeriod.StartTime + minPeriod >= timePeriod.EndTime)
            {
                return new ValidationResult($"Period must be >= {minPeriod}");
            }

            if (timePeriod.StartTime + maxPeriod < timePeriod.EndTime)
            {
                return new ValidationResult($"Period must be <= {maxPeriod}");
            }

            return ValidationResult.Success;
        }

        private readonly TimeSpan minPeriod;
        private readonly TimeSpan maxPeriod;
    }
}