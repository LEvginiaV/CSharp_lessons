namespace Market.CustomersAndStaff.Models.Validations
{
    public class ValidationResult
    {
        public ValidationResult(bool isSuccess, string errorType, string errorDescription)
        {
            IsSuccess = isSuccess;
            ErrorType = errorType;
            ErrorDescription = errorDescription;
        }

        public ValidationResult(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public ValidationResult()
        {
            
        }

        public bool IsSuccess { get; set; }
        public string ErrorType { get; set; }
        public string ErrorDescription { get; set; }

        public static ValidationResult Success()
        {
            return new ValidationResult(true);
        }

        public static ValidationResult Fail(string fieldName, string errorDescription)
        {
            return new ValidationResult(false, fieldName, errorDescription);
        }
    }
}