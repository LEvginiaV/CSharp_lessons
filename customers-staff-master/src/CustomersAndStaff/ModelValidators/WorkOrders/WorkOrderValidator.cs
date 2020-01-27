using System;
using System.Linq;

using Market.CustomersAndStaff.Models.Validations;
using Market.CustomersAndStaff.Models.WorkOrders;

namespace Market.CustomersAndStaff.ModelValidators.WorkOrders
{
    public class WorkOrderValidator : IValidator<WorkOrder>
    {
        public WorkOrderValidator(
            IValidator<WorkOrderNumber> workOrderNumberValidator,
            IValidator<VehicleCustomerValue> vehicleValidator,
            IValidator<ApplianceCustomerValue> applianceValidator,
            IValidator<OtherCustomerValue> otherValidator)
        {
            this.workOrderNumberValidator = workOrderNumberValidator;
            this.vehicleValidator = vehicleValidator;
            this.applianceValidator = applianceValidator;
            this.otherValidator = otherValidator;
        }

        public ValidationResult Validate(WorkOrder model)
        {
            var endDate = DateTime.UtcNow.Date.AddDays(2);

            var result = workOrderNumberValidator.Validate(model.Number);

            if(!result.IsSuccess)
            {
                return result;
            }

            if(!string.IsNullOrEmpty(model.ShopRequisites?.Phone) && (model.ShopRequisites.Phone.Length != 11 || !model.ShopRequisites.Phone.All(char.IsDigit)))
            {
                return ValidationResult.Fail("shopPhone", "phone should be 11 digits string");
            }

            if(model.ReceptionDate < startDate || model.ReceptionDate > endDate)
            {
                return ValidationResult.Fail("receptionDate", "reception date should be more then 01.07.2016 and less then 2 days after now");
            }

            if(model.CompletionDatePlanned < model.ReceptionDate || model.CompletionDatePlanned > model.ReceptionDate.AddYears(1))
            {
                return ValidationResult.Fail("completionDatePlanned", "completion date should be more then reception date and less then 1 year after reception date");
            }

            if(model.CompletionDateFact != null && (model.CompletionDateFact < model.ReceptionDate || model.CompletionDateFact > model.ReceptionDate.AddYears(1)))
            {
                return ValidationResult.Fail("completionDateFact", "completion date should be more then reception date and less then 1 year after reception date");
            }

            if(!string.IsNullOrEmpty(model.WarrantyNumber) && model.WarrantyNumber.Length > 20)
            {
                return ValidationResult.Fail("warrantyNumber", "warranty number length should be less then 20");
            }

            if(model.CustomerValues?.CustomerValues != null)
            {
                var customerValues = model.CustomerValues;
                switch(customerValues.CustomerValueType)
                {
                case CustomerValueType.Vehicle:
                    var vehicleCustomerValues = customerValues.CustomerValues.OfType<VehicleCustomerValue>().ToArray();
                    if(vehicleCustomerValues.Length != model.CustomerValues.CustomerValues.Length)
                    {
                        return ValidationResult.Fail("customerValueType", "values should be of type Vehicle");
                    }

                    result = vehicleCustomerValues.Select(x => vehicleValidator.Validate(x)).FirstOrDefault(x => !x.IsSuccess);
                    break;
                case CustomerValueType.Appliances:
                    var applianceCustomerValues = model.CustomerValues.CustomerValues.OfType<ApplianceCustomerValue>().ToArray();
                    if(applianceCustomerValues.Length != model.CustomerValues.CustomerValues.Length)
                    {
                        return ValidationResult.Fail("customerValueType", "values should be of type Appliance");
                    }

                    result = applianceCustomerValues.Select(x => applianceValidator.Validate(x)).FirstOrDefault(x => !x.IsSuccess);
                    break;
                case CustomerValueType.Other:
                    var otherCustomerValues = model.CustomerValues.CustomerValues.OfType<OtherCustomerValue>().ToArray();
                    if(otherCustomerValues.Length != model.CustomerValues.CustomerValues.Length)
                    {
                        return ValidationResult.Fail("customerValueType", "values should be of type Other");
                    }

                    result = otherCustomerValues.Select(x => otherValidator.Validate(x)).FirstOrDefault(x => !x.IsSuccess);
                    break;
                default:
                    return ValidationResult.Fail("customerValueType", "unknown value type");
                }

                if(result != null)
                {
                    return result;
                }
            }

            if(!string.IsNullOrEmpty(model.AdditionalText) && model.AdditionalText.Length > 2000)
            {
                return ValidationResult.Fail("additionalText", "additional text should be less or equals to 2000");
            }

            return ValidationResult.Success();
        }

        private static readonly DateTime startDate = new DateTime(2016, 7, 1, 0, 0, 0, DateTimeKind.Utc);
        private readonly IValidator<WorkOrderNumber> workOrderNumberValidator;
        private readonly IValidator<VehicleCustomerValue> vehicleValidator;
        private readonly IValidator<ApplianceCustomerValue> applianceValidator;
        private readonly IValidator<OtherCustomerValue> otherValidator;
    }
}