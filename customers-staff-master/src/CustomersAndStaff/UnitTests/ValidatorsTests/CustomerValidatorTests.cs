using System.Collections.Generic;

using FluentAssertions;

using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.Models.Validations;
using Market.CustomersAndStaff.ModelValidators;
using Market.CustomersAndStaff.ModelValidators.Customers;
using Market.CustomersAndStaff.Tests.Core;

using NUnit.Framework;

namespace Market.CustomersAndStaff.UnitTests.ValidatorsTests
{
    public class CustomerValidatorTests
    {
        [TestCaseSource(nameof(GenerateValidCustomers))]
        public void ValidTest(Customer customer)
        {
            validator.Validate(customer).Should().BeEquivalentTo(new ValidationResult(true));
        }

        [TestCaseSource(nameof(GenerateInvalidCustomers))]
        public void InvalidTest(Customer customer, string fieldName, string errorDescription)
        {
            var result = validator.Validate(customer);
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(fieldName);
            result.ErrorDescription.Should().Be(errorDescription);
        }

        public static IEnumerable<TestCaseData> GenerateValidCustomers()
        {
            yield return new TestCaseData(new Customer
                    {
                        Name = "Василий Петухов из ZZ Top 1969",
                    })
                    {TestName = "Name only"};
            yield return new TestCaseData(new Customer
                    {
                        Name = RandomStringGenerator.GenerateRandomCyrillic(120),
                    })
                    {TestName = "Name max length"};
            yield return new TestCaseData(new Customer
                    {
                        Phone = "79112586914",
                    })
                    {TestName = "Phone only"};
            yield return new TestCaseData(new Customer
                    {
                        CustomId = "ded",
                    })
                    {TestName = "CustomId only"};
            yield return new TestCaseData(new Customer
                    {
                        CustomId = RandomStringGenerator.GenerateRandomLatin(100),
                    })
                    {TestName = "CustomId max length"};
            yield return new TestCaseData(new Customer
                    {
                        Email = RandomStringGenerator.GenerateRandomLatin(100),
                        CustomId = "ded",
                    })
                    {TestName = "Email max length"};
            yield return new TestCaseData(new Customer
                    {
                        CustomId = "ded",
                        Discount = 0,
                    })
                    {TestName = "Discount == 0"};
            yield return new TestCaseData(new Customer
                    {
                        CustomId = "ded",
                        Discount = 100,
                    })
                    {TestName = "Discount == 100"};
            yield return new TestCaseData(new Customer
                    {
                        CustomId = "ded",
                        Discount = 25.38m,
                    })
                    {TestName = "Discount == 25.38"};
            yield return new TestCaseData(new Customer
                    {
                        CustomId = "ded",
                        Gender = Gender.Male,
                    })
                    {TestName = "Gender"};
            yield return new TestCaseData(new Customer
                    {
                        CustomId = "ded",
                        Birthday = new Birthday(24, 4, 2005),
                    })
                    {TestName = "Birthday"};
        }

        public static IEnumerable<TestCaseData> GenerateInvalidCustomers()
        {
            yield return new TestCaseData(new Customer
                    {
                        Email = "f@g.com",
                        Discount = 20,
                        AdditionalInfo = "eidhwuibdh",
                    }, "Name, Phone, CustomId", "Expected at least one non-empty")
                    {TestName = "No required fields"};
            yield return new TestCaseData(new Customer
                    {
                        Name = "",
                        Phone = "",
                        CustomId = "",
                        Email = "f@g.com",
                        Discount = 20,
                        AdditionalInfo = "eidhwuibdh",
                        Gender = Gender.Male,
                        Birthday = new Birthday(1,1,2000)
                    }, "Name, Phone, CustomId", "Expected at least one non-empty")
                    {TestName = "Empty required fields"};
            yield return new TestCaseData(new Customer
                    {
                        Name = "  ",
                        Phone = "  ",
                        CustomId = "  ",
                        Email = "f@g.com",
                        Discount = 20,
                        AdditionalInfo = "eidhwuibdh",
                        Gender = Gender.Male,
                        Birthday = new Birthday(1,1,2000)
                    }, "Name, Phone, CustomId", "Expected at least one non-empty")
                    {TestName = "Empty fields with spaces"};
            yield return new TestCaseData(new Customer
                    {
                        Name = RandomStringGenerator.GenerateRandomCyrillic(121),
                    }, "Name", "Expected length <= 120")
                    {TestName = "Long name"};
            yield return new TestCaseData(new Customer
                    {
                        Email = RandomStringGenerator.GenerateRandomLatin(101),
                    }, "Email", "Expected length <= 100")
                    {TestName = "Long e-mail"};
            yield return new TestCaseData(new Customer
                    {
                        CustomId = RandomStringGenerator.GenerateRandomLatin(101),
                    }, "CustomId", "Expected length <= 100")
                    {TestName = "Long customId"};
            yield return new TestCaseData(new Customer
                    {
                        Discount = -1,
                    }, "Discount", "Expected value between [0, 100] and 2 decimals")
                    {TestName = "Discount < 0"};
            yield return new TestCaseData(new Customer
                    {
                        Discount = 101,
                    }, "Discount", "Expected value between [0, 100] and 2 decimals")
                    {TestName = "Discount > 100"};
            yield return new TestCaseData(new Customer
                    {
                        Discount = 25.384m,
                    }, "Discount", "Expected value between [0, 100] and 2 decimals")
                    {TestName = "Discount == 25.384"};
            yield return new TestCaseData(new Customer
                    {
                        Gender = (Gender?)-1,
                    }, "Gender", "Expected Male or Female")
                    {TestName = "Wrong gender"};
            yield return new TestCaseData(new Customer
                    {
                        AdditionalInfo = RandomStringGenerator.GenerateRandomLatin(501),
                    }, "AdditionalInfo", "Expected length <= 500")
                    {TestName = "Long additionalInfo"};
            yield return new TestCaseData(new Customer
                    {
                        Birthday = new Birthday(30, 2),
                    }, "Birthday.Day", "Expected 1 <= day <= 29")
                    { TestName = "Wrong birthday" };
        }

        private readonly IValidator<Customer> validator = new CustomerValidator(new BirthdayValidator());
    }
}