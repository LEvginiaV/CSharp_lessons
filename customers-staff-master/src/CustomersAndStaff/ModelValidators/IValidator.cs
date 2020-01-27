using Market.CustomersAndStaff.Models.Validations;

namespace Market.CustomersAndStaff.ModelValidators
{
    public interface IValidator<in T>
    {
        ValidationResult Validate(T model);
    }
}