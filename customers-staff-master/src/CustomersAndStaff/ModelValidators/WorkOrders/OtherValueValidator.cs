using Market.CustomersAndStaff.Models.Validations;
using Market.CustomersAndStaff.Models.WorkOrders;

namespace Market.CustomersAndStaff.ModelValidators.WorkOrders
{
    public class OtherValueValidator : IValidator<OtherCustomerValue>
    {
        public ValidationResult Validate(OtherCustomerValue value)
        {
            if (!string.IsNullOrEmpty(value.Description) && value.Description.Length > 500)
            {
                return ValidationResult.Fail("otherValueDescription", "description should be less or equals to 500");
            }

            if (!string.IsNullOrEmpty(value.AdditionalInfo) && value.AdditionalInfo.Length > 500)
            {
                return ValidationResult.Fail("otherValueAdditionalInfo", "additional info should be less or equals to 500");
            }

            return ValidationResult.Success();
        }
    }
}