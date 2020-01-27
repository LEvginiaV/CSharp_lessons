using System.Collections.Generic;

using FluentAssertions;

using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.Models.Validations;
using Market.CustomersAndStaff.ModelValidators;
using Market.CustomersAndStaff.ModelValidators.Customers;

using NUnit.Framework;

namespace Market.CustomersAndStaff.UnitTests.ValidatorsTests
{
    public class BirthdayValidatorTests
    {
        [TestCaseSource(nameof(GenerateValidBirthdays))]
        public void ValidTest(Birthday birthday)
        {
            validator.Validate(birthday).Should().BeEquivalentTo(new ValidationResult(true));
        }

        [TestCaseSource(nameof(GenerateInvalidBirthdays))]
        public void InvalidTest(Birthday birthday)
        {
            var result = validator.Validate(birthday);
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().NotBeEmpty();
            result.ErrorDescription.Should().NotBeEmpty();
        }

        public static IEnumerable<TestCaseData> GenerateValidBirthdays()
        {
            yield return new TestCaseData(new Birthday(22, 4, 1995))
                    {TestName = "Simple"};
            yield return new TestCaseData(new Birthday(22, 4, 1000))
                    { TestName = "Year == 1000" };
            yield return new TestCaseData(new Birthday(22, 4, 9999))
                    { TestName = "Year == 9999" };
            yield return new TestCaseData(new Birthday(22, 1, 1995))
                    { TestName = "Month == 1" };
            yield return new TestCaseData(new Birthday(22, 12, 1995))
                    { TestName = "Month == 12" };
            yield return new TestCaseData(new Birthday(1, 4, 1995))
                    { TestName = "Day == 1" };
            yield return new TestCaseData(new Birthday(28, 2, 1995))
                    { TestName = "28.02.1995" };
            yield return new TestCaseData(new Birthday(29, 2, 1996))
                    { TestName = "29.02.1996" };
            yield return new TestCaseData(new Birthday(30, 4, 1996))
                    { TestName = "30.04.1996" };
            yield return new TestCaseData(new Birthday(31, 5, 1996))
                    { TestName = "31.05.1996" };
            yield return new TestCaseData(new Birthday(22, 4))
                    { TestName = "No year" };
            yield return new TestCaseData(new Birthday(29, 2))
                    { TestName = "29.02" };
            yield return new TestCaseData(new Birthday(30, 4))
                    { TestName = "30.04" };
            yield return new TestCaseData(new Birthday(31, 5))
                    { TestName = "31.05" };
        }

        public static IEnumerable<TestCaseData> GenerateInvalidBirthdays()
        {
            yield return new TestCaseData(new Birthday(22, 4, 999))
                    { TestName = "Year < 1000" };
            yield return new TestCaseData(new Birthday(22, 4, 10000))
                    { TestName = "Year > 9999" };
            yield return new TestCaseData(new Birthday(22, 0, 1995))
                    { TestName = "Month < 1" };
            yield return new TestCaseData(new Birthday(22, 13, 1995))
                    { TestName = "Month > 12" };
            yield return new TestCaseData(new Birthday(0, 4, 1995))
                    { TestName = "Day < 1" };
            yield return new TestCaseData(new Birthday(29, 2, 1995))
                    { TestName = "29.02.1995" };
            yield return new TestCaseData(new Birthday(30, 2, 1996))
                    { TestName = "30.02.1996" };
            yield return new TestCaseData(new Birthday(31, 4, 1996))
                    { TestName = "31.04.1996" };
            yield return new TestCaseData(new Birthday(32, 5, 1996))
                    { TestName = "32.05.1996" };
            yield return new TestCaseData(new Birthday(30, 2))
                    { TestName = "30.02" };
            yield return new TestCaseData(new Birthday(31, 4))
                    { TestName = "31.04" };
            yield return new TestCaseData(new Birthday(32, 5))
                    { TestName = "32.05" };
        }

        private readonly IValidator<Birthday> validator = new BirthdayValidator();
    }
}