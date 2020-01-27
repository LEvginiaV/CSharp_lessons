using System.Collections.Generic;

using FluentAssertions;

using Market.CustomersAndStaff.Models.WorkOrders;
using Market.CustomersAndStaff.ModelValidators;
using Market.CustomersAndStaff.ModelValidators.WorkOrders;
using Market.CustomersAndStaff.Tests.Core;

using NUnit.Framework;

namespace Market.CustomersAndStaff.UnitTests.WorkOrderValidatorsTests
{
    public class OtherValueValidatorTests
    {
        [TestCaseSource(nameof(GenerateValidOtherValues))]
        public void ValidTest(OtherCustomerValue other)
        {
            validator.Validate(other).IsSuccess.Should().BeTrue();
        }

        [TestCaseSource(nameof(GenerateInvalidOtherValues))]
        public void InvalidTest(OtherCustomerValue other, string fieldName, string errorDescription)
        {
            var result = validator.Validate(other);
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(fieldName);
            result.ErrorDescription.Should().Be(errorDescription);
        }

        public static IEnumerable<TestCaseData> GenerateValidOtherValues()
        {
            yield return new TestCaseData(new OtherCustomerValue
                    {
                        Description = RandomStringGenerator.GenerateRandomLatin(500),
                    })
                    {TestName = "Description max length"};
            yield return new TestCaseData(new OtherCustomerValue
                    {
                        Description = " ",
                    })
                    {TestName = "Description with spaces"};
            
            yield return new TestCaseData(new OtherCustomerValue
                    {
                        AdditionalInfo = RandomStringGenerator.GenerateRandomLatin(500),
                    })
                    {TestName = "AdditionalInfo max length"};
            yield return new TestCaseData(new OtherCustomerValue
                    {
                        AdditionalInfo = " "
                    })
                    {TestName = "AdditionalInfo with spaces"};
        }

        public static IEnumerable<TestCaseData> GenerateInvalidOtherValues()
        {
            yield return new TestCaseData(new OtherCustomerValue
                    {
                        Description = RandomStringGenerator.GenerateRandomLatin(501),
                    }, "otherValueDescription", "description should be less or equals to 500")
                    {TestName = "AdditionalInfo length"};
            yield return new TestCaseData(new OtherCustomerValue
                    {
                        AdditionalInfo = RandomStringGenerator.GenerateRandomLatin(501),
                    }, "otherValueAdditionalInfo", "additional info should be less or equals to 500")
                    {TestName = "AdditionalInfo length"};
        }

        private readonly IValidator<OtherCustomerValue> validator = new OtherValueValidator();
    }
}