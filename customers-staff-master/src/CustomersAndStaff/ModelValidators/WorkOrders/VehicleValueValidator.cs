using System;
using System.Text.RegularExpressions;

using Market.CustomersAndStaff.Models.Validations;
using Market.CustomersAndStaff.Models.WorkOrders;

namespace Market.CustomersAndStaff.ModelValidators.WorkOrders
{
    public class VehicleValueValidator : IValidator<VehicleCustomerValue>
    {
        public ValidationResult Validate(VehicleCustomerValue value)
        {
            if (!string.IsNullOrEmpty(value.Brand) && value.Brand.Length > 40)
            {
                return ValidationResult.Fail("vehicleValueBrand", "brand should be less or equals to 40");
            }

            if (!string.IsNullOrEmpty(value.Model) && value.Model.Length > 100)
            {
                return ValidationResult.Fail("vehicleValueModel", "model should be less or equals to 100");
            }

            if (!string.IsNullOrEmpty(value.RegisterSign))
            {
                if (value.RegisterSign.Length > 15)
                {
                    return ValidationResult.Fail("vehicleValueRegisterSign", "register sign should be less or equals to 15");
                }

                if (!registerSignRegex.IsMatch(value.RegisterSign))
                {
                    return ValidationResult.Fail("vehicleValueRegisterSign", "register sign contains wrong characters");
                }
            }

            if (value.Year != null && (value.Year < 1900 || value.Year > DateTime.UtcNow.AddDays(1).Year))
            {
                return ValidationResult.Fail("vehicleValueYear", "year should be more or equals to 1900 and less or equals to current year");
            }

            if (!string.IsNullOrEmpty(value.BodyNumber))
            {
                if (value.BodyNumber.Length > 17)
                {
                    return ValidationResult.Fail("vehicleValueBodyNumber", "body number should be less or equals to 17");
                }

                if (!vehicleNumbersRegex.IsMatch(value.BodyNumber))
                {
                    return ValidationResult.Fail("vehicleValueBodyNumber", "body number contains wrong characters");
                }
            }

            if (!string.IsNullOrEmpty(value.EngineNumber))
            {
                if (value.EngineNumber.Length > 17)
                {
                    return ValidationResult.Fail("vehicleValueEngineNumber", "engine number should be less or equals to 17");
                }

                if (!vehicleNumbersRegex.IsMatch(value.EngineNumber))
                {
                    return ValidationResult.Fail("vehicleValueEngineNumber", "engine number contains wrong characters");
                }
            }

            if (!string.IsNullOrEmpty(value.AdditionalInfo) && value.AdditionalInfo.Length > 500)
            {
                return ValidationResult.Fail("vehicleValueAdditionalInfo", "additional info should be less or equals to 500");
            }

            return ValidationResult.Success();
        }

        private static readonly Regex registerSignRegex = new Regex("^[0-9A-ZАВЕКМНОРСТУХ]+$", RegexOptions.Compiled);
        private static readonly Regex vehicleNumbersRegex = new Regex(@"^[0-9A-Za-z\-]+$", RegexOptions.Compiled);
    }
}