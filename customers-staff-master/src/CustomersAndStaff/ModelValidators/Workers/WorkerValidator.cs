using Market.CustomersAndStaff.Models.Validations;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.ModelValidators.Commons;

namespace Market.CustomersAndStaff.ModelValidators.Workers
{
    public class WorkerValidator : IValidator<Worker>
    {
        public ValidationResult Validate(Worker model)
        {
            if(string.IsNullOrWhiteSpace(model.FullName))
            {
                return ValidationResult.Fail(nameof(model.FullName), "Expected non-empty");
            }

            var result = ValidateLength(model.FullName, nameof(model.FullName), 120);
            if(!result.IsSuccess)
            {
                return result;
            }

            if(!StringValidators.ValidateLettersAndNumbers(model.FullName))
            {
                return ValidationResult.Fail(nameof(model.FullName), "Expected numbers or letters");
            }

            if(!string.IsNullOrEmpty(model.Phone))
            {
                if(!StringValidators.ValidatePhone(model.Phone))
                {
                    return ValidationResult.Fail(nameof(model.Phone), "Expected numeric");
                }
            }

            if(!string.IsNullOrEmpty(model.Position))
            {
                result = ValidateLength(model.Position, nameof(model.Position), 120);
                if (!result.IsSuccess)
                {
                    return result;
                }
            }

            if(model.AdditionalInfo != null && model.AdditionalInfo.Length > 500)
            {
                return ValidationResult.Fail(nameof(model.AdditionalInfo), "Expected length <= 500");
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
    }
}