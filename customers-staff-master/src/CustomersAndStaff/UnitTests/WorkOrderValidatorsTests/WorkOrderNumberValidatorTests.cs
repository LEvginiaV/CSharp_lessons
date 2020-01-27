using FluentAssertions;

using Market.CustomersAndStaff.Models.Validations;
using Market.CustomersAndStaff.Models.WorkOrders;
using Market.CustomersAndStaff.ModelValidators;
using Market.CustomersAndStaff.ModelValidators.WorkOrders;

using NUnit.Framework;

namespace Market.CustomersAndStaff.UnitTests.WorkOrderValidatorsTests
{
    public class WorkOrderNumberValidatorTests
    {
        [TestCase("АА", 1)]
        [TestCase("АА", 999999)]
        [TestCase("ГЮ", 123456)]
        [TestCase("ЯЯ", 999999)]
        public void ValidTest(string series, int number)
        {
            validator.Validate(new WorkOrderNumber(series, number)).Should().BeEquivalentTo(ValidationResult.Success());
        }

        [TestCase(0)]
        [TestCase(1000000)]
        [TestCase(-123)]
        public void InvalidNumberTest(int number)
        {
            var result = validator.Validate(new WorkOrderNumber("АА", number));

            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be("workOrderNumber");
            result.ErrorDescription.Should().Be("number should be >= 1 and <= 999999");
        }

        [TestCase(null)]
        [TestCase("А")]
        [TestCase("ААА")]
        public void InvalidSeriesLengthTest(string series)
        {
            var result = validator.Validate(new WorkOrderNumber(series, 1));

            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be("workOrderSeries");
            result.ErrorDescription.Should().Be("series should have length 2");
        }

        [TestCase("АЁ")]
        [TestCase("АЗ")]
        [TestCase("АЙ")]
        [TestCase("АО")]
        [TestCase("АЧ")]
        [TestCase("АЬ")]
        [TestCase("АЫ")]
        [TestCase("АЪ")]
        [TestCase("А1")]
        [TestCase("АF")]
        public void InvalidSeriesCharacterTest(string series)
        {
            var result = validator.Validate(new WorkOrderNumber(series, 1));

            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be("workOrderSeries");
            result.ErrorDescription.Should().Be("series contains wrong characters");
        }

        private readonly IValidator<WorkOrderNumber> validator = new WorkOrderNumberValidator();
    }
}