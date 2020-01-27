using System.Collections.Generic;

using FluentAssertions;

using Market.CustomersAndStaff.Models.Validations;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.ModelValidators;
using Market.CustomersAndStaff.ModelValidators.Workers;
using Market.CustomersAndStaff.Tests.Core;

using NUnit.Framework;

namespace Market.CustomersAndStaff.UnitTests.ValidatorsTests
{
    public class WorkerValidatorTests
    {
        [TestCaseSource(nameof(GenerateValidWorkers))]
        public void ValidTest(Worker worker)
        {
            validator.Validate(worker).Should().BeEquivalentTo(new ValidationResult(true));
        }

        [TestCaseSource(nameof(GenerateInvalidWorkers))]
        public void InvalidTest(Worker worker, string fieldName, string errorDescription)
        {
            var result = validator.Validate(worker);
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(fieldName);
            result.ErrorDescription.Should().Be(errorDescription);
        }

        public static IEnumerable<TestCaseData> GenerateValidWorkers()
        {
            yield return new TestCaseData(new Worker
                    {
                        FullName = "Василий Петухов из ZZ Top 1969",
                    })
                    {TestName = "Name only"};
            yield return new TestCaseData(new Worker
                    {
                        FullName = RandomStringGenerator.GenerateRandomCyrillic(120),
                    })
                    {TestName = "Name max length"};
            yield return new TestCaseData(new Worker
                    {
                        FullName = "Василий Петухов",
                        Phone = "79112586914",
                    })
                    {TestName = "Phone"};
            yield return new TestCaseData(new Worker
                    {
                        FullName = "Василий Петухов",
                        Position = "ded",
                    })
                    {TestName = "Position"};
            yield return new TestCaseData(new Worker
                    {
                        FullName = "Василий Петухов",
                        Position = RandomStringGenerator.GenerateRandomLatin(120),
                    })
                    {TestName = "Position max length"};
            yield return new TestCaseData(new Worker
                    {
                        FullName = "Василий Петухов",
                        AdditionalInfo = "eidhwuibdh",
                    })
                    {TestName = "AdditionalInfo"};
        }

        public static IEnumerable<TestCaseData> GenerateInvalidWorkers()
        {
            yield return new TestCaseData(new Worker
                    {
                        FullName = "",
                    }, "FullName", "Expected non-empty")
                    {TestName = "No name"};
            yield return new TestCaseData(new Worker(), "FullName", "Expected non-empty")
                    {TestName = "Name is null"};
            yield return new TestCaseData(new Worker
                    {
                        FullName = "  ",
                    }, "FullName", "Expected non-empty")
                    {TestName = "Name with spaces"};
            yield return new TestCaseData(new Worker
                    {
                        FullName = RandomStringGenerator.GenerateRandomCyrillic(121),
                    }, "FullName", "Expected length <= 120")
                    {TestName = "Long name"};
            yield return new TestCaseData(new Worker
                    {
                        FullName = "Василий Петухов",
                        Phone = "+7(999)38727833"
                    }, "Phone", "Expected numeric")
                    {TestName = "Wrong phone field"};
            yield return new TestCaseData(new Worker
                    {
                        FullName = "Василий Петухов",
                        Position = RandomStringGenerator.GenerateRandomLatin(121),
                    }, "Position", "Expected length <= 120")
                    {TestName = "Long position"};
            yield return new TestCaseData(new Worker
                    {
                        FullName = "Василий Петухов",
                        AdditionalInfo = RandomStringGenerator.GenerateRandomLatin(501),
                    }, "AdditionalInfo", "Expected length <= 500")
                    { TestName = "Long additional info" };
        }

        private readonly IValidator<Worker> validator = new WorkerValidator();
    }
}