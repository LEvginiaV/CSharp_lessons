using System;
using System.Collections.Generic;

using FluentAssertions;

using Market.CustomersAndStaff.Models.WorkOrders;
using Market.CustomersAndStaff.ModelValidators;
using Market.CustomersAndStaff.ModelValidators.WorkOrders;
using Market.CustomersAndStaff.Tests.Core;

using NUnit.Framework;

namespace Market.CustomersAndStaff.UnitTests.WorkOrderValidatorsTests
{
    public class ApplianceValueValidatorTests
    {
        [TestCaseSource(nameof(GenerateValidApplianceValues))]
        public void ValidTest(ApplianceCustomerValue appliance)
        {
            validator.Validate(appliance).IsSuccess.Should().BeTrue();
        }

        [TestCaseSource(nameof(GenerateInvalidApplianceValues))]
        public void InvalidTest(ApplianceCustomerValue appliance, string fieldName, string errorDescription)
        {
            var result = validator.Validate(appliance);
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(fieldName);
            result.ErrorDescription.Should().Be(errorDescription);
        }

        public static IEnumerable<TestCaseData> GenerateValidApplianceValues()
        {
            yield return new TestCaseData(new ApplianceCustomerValue
                    {
                        Brand = RandomStringGenerator.GenerateRandomLatin(40),
                    })
                    {TestName = "Brand max length"};
            yield return new TestCaseData(new ApplianceCustomerValue
                    {
                        Brand = " ",
                    })
                    {TestName = "Brand with spaces"};
            
            yield return new TestCaseData(new ApplianceCustomerValue
                    {
                        Model = RandomStringGenerator.GenerateRandomLatin(100),
                    })
                    {TestName = "Model max length"};
            yield return new TestCaseData(new ApplianceCustomerValue
                    {
                        Model = " ",
                    })
                    {TestName = "Model with spaces"};
            
            yield return new TestCaseData(new ApplianceCustomerValue
                    {
                        Number = RandomStringGenerator.GenerateRandomLatin(100),
                    })
                    {TestName = "Number max length"};
            yield return new TestCaseData(new ApplianceCustomerValue
                    {
                        Number = " ",
                    })
                    {TestName = "Number with spaces"};
            
            yield return new TestCaseData(new ApplianceCustomerValue
                    {
                        Year = 1900,
                    })
                    {TestName = "Min year"};
            yield return new TestCaseData(new ApplianceCustomerValue
                    {
                        Year = DateTime.UtcNow.AddDays(1).Year,
                    })
                    {TestName = "Max year"};
            
            yield return new TestCaseData(new ApplianceCustomerValue
                    {
                        AdditionalInfo = RandomStringGenerator.GenerateRandomLatin(500),
                    })
                    {TestName = "AdditionalInfo max length"};
            yield return new TestCaseData(new ApplianceCustomerValue
                    {
                        AdditionalInfo = " ",
                    })
                    {TestName = "AdditionalInfo with spasces"};
        }

        public static IEnumerable<TestCaseData> GenerateInvalidApplianceValues()
        {
            yield return new TestCaseData(new ApplianceCustomerValue
                    {
                        Brand = RandomStringGenerator.GenerateRandomLatin(41),
                    }, "applianceValueBrand", "brand should be less or equals to 40")
                    {TestName = "Brand length"};
            yield return new TestCaseData(new ApplianceCustomerValue
                    {
                        Model = RandomStringGenerator.GenerateRandomLatin(101),
                    }, "applianceValueModel", "model should be less or equals to 100")
                    {TestName = "Model length"};
            yield return new TestCaseData(new ApplianceCustomerValue
                    {
                        Number = RandomStringGenerator.GenerateRandomLatin(101),
                    }, "applianceValueNumber", "number should be less or equals to 100")
                    {TestName = "Number length"};
            yield return new TestCaseData(new ApplianceCustomerValue
                    {
                        Year = 1899,
                    }, "applianceValueYear", "year should be more or equals to 1900 and less or equals to current year")
                    {TestName = "Min year"};
            yield return new TestCaseData(new ApplianceCustomerValue
                    {
                        Year = DateTime.UtcNow.AddDays(1).Year + 1,
                    }, "applianceValueYear", "year should be more or equals to 1900 and less or equals to current year")
                    {TestName = "Max year"};
            yield return new TestCaseData(new ApplianceCustomerValue
                    {
                        AdditionalInfo = RandomStringGenerator.GenerateRandomLatin(501),
                    }, "applianceValueAdditionalInfo", "additional info should be less or equals to 500")
                    {TestName = "AdditionalInfo length"};
        }

        private readonly IValidator<ApplianceCustomerValue> validator = new ApplianceValueValidator();
    }
}