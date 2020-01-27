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
    public class VehicleValueValidatorTests
    {
        [TestCase("АВЕКМНОРСТУХ")]
        [TestCase("0123456789")]
        [TestCase("ABCDEFGHIJKLMNO")]
        [TestCase("PQRSTUVWXYZ")]
        public void ValidRegisterSignTest(string registerSign)
        {
            validator.Validate(new VehicleCustomerValue
                {
                    RegisterSign = registerSign,
                }).IsSuccess.Should().BeTrue();
        }

        [TestCase("Б")]
        [TestCase("в")]
        [TestCase("q")]
        [TestCase("-")]
        public void InvalidRegisterSignTest(string registerSign)
        {
            var result = validator.Validate(new VehicleCustomerValue
                {
                    RegisterSign = registerSign,
                });
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be("vehicleValueRegisterSign");
            result.ErrorDescription.Should().Be("register sign contains wrong characters");
        }

        [TestCase("0123456789")]
        [TestCase("ABCDEFGHIJKLMNOPQ")]
        [TestCase("RSTUVWXYZ")]
        [TestCase("abcdefghijklmnopq")]
        [TestCase("rstuvwxyz-")]
        public void ValidBodyNumberTest(string bodyNumber)
        {
            validator.Validate(new VehicleCustomerValue
                {
                    RegisterSign = "X",
                    BodyNumber = bodyNumber,
                }).IsSuccess.Should().BeTrue();
        }

        [TestCase("Б")]
        [TestCase("в")]
        [TestCase("~")]
        public void InvalidBodyNumberTest(string bodyNumber)
        {
            var result = validator.Validate(new VehicleCustomerValue
                {
                    RegisterSign = "X",
                    BodyNumber = bodyNumber,
                });
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be("vehicleValueBodyNumber");
            result.ErrorDescription.Should().Be("body number contains wrong characters");
        }

        [TestCase("0123456789")]
        [TestCase("ABCDEFGHIJKLMNOPQ")]
        [TestCase("RSTUVWXYZ")]
        [TestCase("abcdefghijklmnopq")]
        [TestCase("rstuvwxyz-")]
        public void ValidEngineNumberTest(string engineNumber)
        {
            validator.Validate(new VehicleCustomerValue
                {
                    RegisterSign = "X",
                    EngineNumber = engineNumber,
                }).IsSuccess.Should().BeTrue();
        }

        [TestCase("Б")]
        [TestCase("в")]
        [TestCase("~")]
        public void InvalidEngineNumberTest(string engineNumber)
        {
            var result = validator.Validate(new VehicleCustomerValue
                {
                    RegisterSign = "X",
                    EngineNumber = engineNumber,
                });
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be("vehicleValueEngineNumber");
            result.ErrorDescription.Should().Be("engine number contains wrong characters");
        }

        [TestCaseSource(nameof(GenerateValidVehicleValues))]
        public void ValidTest(VehicleCustomerValue vehicle)
        {
            validator.Validate(vehicle).IsSuccess.Should().BeTrue();
        }

        [TestCaseSource(nameof(GenerateInvalidVehicleValues))]
        public void InvalidTest(VehicleCustomerValue vehicle, string fieldName, string errorDescription)
        {
            var result = validator.Validate(vehicle);
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(fieldName);
            result.ErrorDescription.Should().Be(errorDescription);
        }

        public static IEnumerable<TestCaseData> GenerateValidVehicleValues()
        {
            yield return new TestCaseData(new VehicleCustomerValue
                    {
                        RegisterSign = "X",
                        Brand = RandomStringGenerator.GenerateRandomLatin(40),
                    })
                    {TestName = "Brand max length"};
            yield return new TestCaseData(new VehicleCustomerValue
                    {
                        RegisterSign = "X",
                        Brand = " ",
                    })
                    {TestName = "Brand with spaces"};
            
            yield return new TestCaseData(new VehicleCustomerValue
                    {
                        RegisterSign = "X",
                        Model = RandomStringGenerator.GenerateRandomLatin(100),
                    })
                    { TestName = "Model max length" };
            yield return new TestCaseData(new VehicleCustomerValue
                    {
                        RegisterSign = "X",
                        Model = " "
                    })
                    { TestName = "Model with spaces" };
            
            yield return new TestCaseData(new VehicleCustomerValue
                    {
                        RegisterSign = "X",
                        Year = 1900,
                    })
                    { TestName = "Min year" };
            yield return new TestCaseData(new VehicleCustomerValue
                    {
                        RegisterSign = "X",
                        Year = DateTime.UtcNow.AddDays(1).Year,
                    })
                    { TestName = "Max year" };
            
            yield return new TestCaseData(new VehicleCustomerValue
                    {
                        RegisterSign = "X",
                        AdditionalInfo = RandomStringGenerator.GenerateRandomLatin(500),
                    })
                    { TestName = "AdditionalInfo max length" };
            yield return new TestCaseData(new VehicleCustomerValue
                    {
                        RegisterSign = "X",
                        AdditionalInfo = " ",
                    })
                    { TestName = "AdditionalInfo with spaces" };
        }

        public static IEnumerable<TestCaseData> GenerateInvalidVehicleValues()
        {
            yield return new TestCaseData(new VehicleCustomerValue
                    {
                        RegisterSign = "X",
                        Brand = RandomStringGenerator.GenerateRandomLatin(41),
                    }, "vehicleValueBrand", "brand should be less or equals to 40")
                    { TestName = "Brand length" };
            yield return new TestCaseData(new VehicleCustomerValue
                    {
                        RegisterSign = "X",
                        Model = RandomStringGenerator.GenerateRandomLatin(101),
                    }, "vehicleValueModel", "model should be less or equals to 100")
                    { TestName = "Model length" };
            yield return new TestCaseData(new VehicleCustomerValue
                    {
                        RegisterSign = RandomStringGenerator.GenerateRandomLatin(16).ToUpperInvariant(),
                    }, "vehicleValueRegisterSign", "register sign should be less or equals to 15")
                    { TestName = "Register sign length length" };
            yield return new TestCaseData(new VehicleCustomerValue
                    {
                        RegisterSign = "X",
                        Year = 1899,
                    }, "vehicleValueYear", "year should be more or equals to 1900 and less or equals to current year")
                    { TestName = "Min year" };
            yield return new TestCaseData(new VehicleCustomerValue
                    {
                        RegisterSign = "X",
                        Year = DateTime.UtcNow.AddDays(1).Year + 1,
                    }, "vehicleValueYear", "year should be more or equals to 1900 and less or equals to current year")
                    { TestName = "Max year" };
            yield return new TestCaseData(new VehicleCustomerValue
                    {
                        BodyNumber = RandomStringGenerator.GenerateRandomLatin(18).ToUpperInvariant(),
                    }, "vehicleValueBodyNumber", "body number should be less or equals to 17")
                    { TestName = "Body number length" };
            yield return new TestCaseData(new VehicleCustomerValue
                    {
                        EngineNumber = RandomStringGenerator.GenerateRandomLatin(18).ToUpperInvariant(),
                    }, "vehicleValueEngineNumber", "engine number should be less or equals to 17")
                    { TestName = "Engine number length" };
            yield return new TestCaseData(new VehicleCustomerValue
                    {
                        RegisterSign = "X",
                        AdditionalInfo = RandomStringGenerator.GenerateRandomLatin(501),
                    }, "vehicleValueAdditionalInfo", "additional info should be less or equals to 500")
                    { TestName = "AdditionalInfo length" };
        }

        private readonly IValidator<VehicleCustomerValue> validator = new VehicleValueValidator();
    }
}