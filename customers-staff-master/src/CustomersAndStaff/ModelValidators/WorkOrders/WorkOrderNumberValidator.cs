using System.Collections.Generic;
using System.Linq;

using Market.CustomersAndStaff.Models.Validations;
using Market.CustomersAndStaff.Models.WorkOrders;

namespace Market.CustomersAndStaff.ModelValidators.WorkOrders
{
    public class WorkOrderNumberValidator : IValidator<WorkOrderNumber>
    {
        public ValidationResult Validate(WorkOrderNumber workOrderNumber)
        {
            if (workOrderNumber.Number < 1 || workOrderNumber.Number > 999999)
            {
                return ValidationResult.Fail("workOrderNumber", "number should be >= 1 and <= 999999");
            }

            if (workOrderNumber.Series == null || workOrderNumber.Series.Length != 2)
            {
                return ValidationResult.Fail("workOrderSeries", "series should have length 2");
            }

            if (CheckSeries(workOrderNumber.Series))
            {
                return ValidationResult.Fail("workOrderSeries", "series contains wrong characters");
            }

            return ValidationResult.Success();
        }

        private static bool CheckSeries(string series)
        {
            return series.Any(ch => ch < 'А' || ch > 'Я' || deniedChars.Contains(ch));
        }

        private static readonly HashSet<char> deniedChars = new HashSet<char>(new[] { 'Ё', 'З', 'Й', 'О', 'Ч', 'Ь', 'Ы', 'Ъ' });
    }
}