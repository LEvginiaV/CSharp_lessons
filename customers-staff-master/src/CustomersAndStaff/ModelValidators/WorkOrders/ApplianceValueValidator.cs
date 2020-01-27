using System;

using Market.CustomersAndStaff.Models.Validations;
using Market.CustomersAndStaff.Models.WorkOrders;

namespace Market.CustomersAndStaff.ModelValidators.WorkOrders
{
    public class ApplianceValueValidator : IValidator<ApplianceCustomerValue>
    {
        public ValidationResult Validate(ApplianceCustomerValue value)
        {
            if (!string.IsNullOrEmpty(value.Brand) && value.Brand.Length > 40)
            {
                return ValidationResult.Fail("applianceValueBrand", "brand should be less or equals to 40");
            }

            if (!string.IsNullOrEmpty(value.Model) && value.Model.Length > 100)
            {
                return ValidationResult.Fail("applianceValueModel", "model should be less or equals to 100");
            }

            if (!string.IsNullOrEmpty(value.Number) && value.Number.Length > 100)
            {
                return ValidationResult.Fail("applianceValueNumber", "number should be less or equals to 100");
            }

            if (value.Year != null && (value.Year < 1900 || value.Year > DateTime.UtcNow.AddDays(1).Year))
            {
                return ValidationResult.Fail("applianceValueYear", "year should be more or equals to 1900 and less or equals to current year");
            }

            if (!string.IsNullOrEmpty(value.AdditionalInfo) && value.AdditionalInfo.Length > 500)
            {
                return ValidationResult.Fail("applianceValueAdditionalInfo", "additional info should be less or equals to 500");
            }

            return ValidationResult.Success();
        }
    }
}