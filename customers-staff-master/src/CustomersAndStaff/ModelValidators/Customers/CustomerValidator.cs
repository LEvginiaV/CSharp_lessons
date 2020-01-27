using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.Models.Validations;
using Market.CustomersAndStaff.ModelValidators.Commons;

namespace Market.CustomersAndStaff.ModelValidators.Customers
{
    public class CustomerValidator : IValidator<Customer>
    {
        public CustomerValidator(IValidator<Birthday> birthdayValidator)
        {
            this.birthdayValidator = birthdayValidator;
        }

        public ValidationResult Validate(Customer model)
        {
            if(!string.IsNullOrEmpty(model.Name))
            {
                var result = ValidateLength(model.Name, nameof(model.Name), 120);
                if(!result.IsSuccess)
                {
                    return result;
                }

                if (!StringValidators.ValidateLettersAndNumbers(model.Name))
                {
                    return ValidationResult.Fail(nameof(model.Name), "Expected numbers or letters");
                }
            }

            if(!string.IsNullOrEmpty(model.Email))
            {
                var result = ValidateLength(model.Email, nameof(model.Email), 100);
                if (!result.IsSuccess)
                {
                    return result;
                }
            }

            if(!string.IsNullOrEmpty(model.CustomId))
            {
                var result = ValidateLength(model.CustomId, nameof(model.CustomId), 100);
                if (!result.IsSuccess)
                {
                    return result;
                }
            }

            if(model.Birthday != null)
            {
                var birthdayValidation = birthdayValidator.Validate(model.Birthday);
                if(!birthdayValidation.IsSuccess)
                {
                    birthdayValidation.ErrorType = $"{nameof(model.Birthday)}.{birthdayValidation.ErrorType}";
                    return birthdayValidation;
                }
            }

            if(model.Discount != null && (decimal.Round(model.Discount.Value, 2) != model.Discount || model.Discount > 100 || model.Discount < 0))
            {
                return ValidationResult.Fail(nameof(model.Discount), "Expected value between [0, 100] and 2 decimals");
            }

            if(model.Gender != null && model.Gender != Gender.Male && model.Gender != Gender.Female)
            {
                return ValidationResult.Fail(nameof(Gender), "Expected Male or Female");
            }

            if(model.AdditionalInfo != null && model.AdditionalInfo.Length > 500)
            {
                return ValidationResult.Fail(nameof(model.AdditionalInfo), "Expected length <= 500");
            }

            if(string.IsNullOrWhiteSpace(model.Name) && string.IsNullOrWhiteSpace(model.Phone) && string.IsNullOrWhiteSpace(model.CustomId))
            {
                return ValidationResult.Fail($"{nameof(model.Name)}, {nameof(model.Phone)}, {nameof(model.CustomId)}", "Expected at least one non-empty");
            }

            return ValidationResult.Success();
        }

        private ValidationResult ValidateLength(string str, string fieldName, int length)
        {
            if(str.Length > length)
            {
                return ValidationResult.Fail(fieldName, $"Expected length <= {length}");
            }

            return ValidationResult.Success();
        }

        private readonly IValidator<Birthday> birthdayValidator;
    }
}