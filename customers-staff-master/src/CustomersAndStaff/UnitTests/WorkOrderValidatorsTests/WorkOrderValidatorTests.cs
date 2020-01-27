using System;
using System.Collections.Generic;

using FluentAssertions;

using Market.CustomersAndStaff.Models.Validations;
using Market.CustomersAndStaff.Models.WorkOrders;
using Market.CustomersAndStaff.ModelValidators;
using Market.CustomersAndStaff.ModelValidators.WorkOrders;
using Market.CustomersAndStaff.Tests.Core;

using NUnit.Framework;

namespace Market.CustomersAndStaff.UnitTests.WorkOrderValidatorsTests
{
    public class WorkOrderValidatorTests
    {
        [TestCaseSource(nameof(GenerateValidWorkOrders))]
        public void ValidTest(WorkOrder workerOrder)
        {
            validator.Validate(workerOrder).Should().BeEquivalentTo(new ValidationResult(true));
        }

        [TestCaseSource(nameof(GenerateInvalidWorkOrders))]
        public void InvalidTest(WorkOrder workOrder, string fieldName, string errorDescription)
        {
            var result = validator.Validate(workOrder);
            result.IsSuccess.Should().BeFalse();
            result.ErrorType.Should().Be(fieldName);
            result.ErrorDescription.Should().Be(errorDescription);
        }

        public static IEnumerable<TestCaseData> GenerateValidWorkOrders()
        {
            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ReceptionDate = new DateTime(2016, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                        CompletionDatePlanned = new DateTime(2016, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                    })
                    {TestName = "Min reception date"};
            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ReceptionDate = DateTime.UtcNow.Date.AddDays(2),
                        CompletionDatePlanned = DateTime.UtcNow.Date.AddMonths(2),
                    })
                    {TestName = "Max reception date"};

            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ReceptionDate = DateTime.UtcNow.Date,
                        CompletionDatePlanned = DateTime.UtcNow.Date,
                    })
                    {TestName = "Min completion date planned"};
            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ReceptionDate = DateTime.UtcNow.Date,
                        CompletionDatePlanned = DateTime.UtcNow.Date.AddYears(1),
                    })
                    {TestName = "Max completion date planned"};

            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ReceptionDate = DateTime.UtcNow.Date,
                        CompletionDatePlanned = DateTime.UtcNow.Date.AddMonths(2),
                        CompletionDateFact = DateTime.UtcNow.Date,
                    })
                    {TestName = "Min completion date fact"};
            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ReceptionDate = DateTime.UtcNow.Date,
                        CompletionDatePlanned = DateTime.UtcNow.Date.AddMonths(2),
                        CompletionDateFact = DateTime.UtcNow.Date.AddYears(1),
                    })
                    {TestName = "Max completion date fact"};

            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ReceptionDate = DateTime.UtcNow.Date,
                        CompletionDatePlanned = DateTime.UtcNow.Date.AddMonths(2),
                        ShopRequisites = new ShopRequisites {Phone = "79112223344"}
                    })
                    {TestName = "Shop phone"};
            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ReceptionDate = DateTime.UtcNow.Date,
                        CompletionDatePlanned = DateTime.UtcNow.Date.AddMonths(2),
                        WarrantyNumber = "1234567890asdfghjkqw",
                    })
                    {TestName = "Warranty number"};
            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ReceptionDate = DateTime.UtcNow.Date,
                        CompletionDatePlanned = DateTime.UtcNow.Date.AddMonths(2),
                        CustomerValues = new CustomerValueList
                            {
                                CustomerValueType = CustomerValueType.Vehicle,
                                CustomerValues = new BaseCustomerValue[0],
                            }
                    })
                    {TestName = "No customer values"};
            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ReceptionDate = DateTime.UtcNow.Date,
                        CompletionDatePlanned = DateTime.UtcNow.Date.AddMonths(2),
                        CustomerValues = new CustomerValueList
                            {
                                CustomerValueType = CustomerValueType.Vehicle,
                                CustomerValues = new BaseCustomerValue[]
                                    {
                                        new VehicleCustomerValue
                                            {
                                                Brand = RandomStringGenerator.GenerateRandomLatin(20),
                                                Model = RandomStringGenerator.GenerateRandomLatin(50),
                                                RegisterSign = "А123ВЕ198",
                                                Year = DateTime.UtcNow.Year,
                                                BodyNumber = RandomStringGenerator.GenerateRandomLatin(17).ToUpper(),
                                                EngineNumber = RandomStringGenerator.GenerateRandomLatin(17).ToUpper(),
                                                AdditionalInfo = RandomStringGenerator.GenerateRandomLatin(200),
                                            },
                                    }
                            }
                    })
                    {TestName = "Vehicle"};
            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ReceptionDate = DateTime.UtcNow.Date,
                        CompletionDatePlanned = DateTime.UtcNow.Date.AddMonths(2),
                        CustomerValues = new CustomerValueList
                            {
                                CustomerValueType = CustomerValueType.Appliances,
                                CustomerValues = new BaseCustomerValue[]
                                    {
                                        new ApplianceCustomerValue()
                                            {
                                                Brand = RandomStringGenerator.GenerateRandomLatin(20),
                                                Model = RandomStringGenerator.GenerateRandomLatin(50),
                                                Number = RandomStringGenerator.GenerateRandomLatin(50),
                                                Year = DateTime.UtcNow.Year,
                                                AdditionalInfo = RandomStringGenerator.GenerateRandomLatin(200),
                                            },
                                    }
                            }
                    })
                    {TestName = "Appliance"};
            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ReceptionDate = DateTime.UtcNow.Date,
                        CompletionDatePlanned = DateTime.UtcNow.Date.AddMonths(2),
                        CustomerValues = new CustomerValueList
                            {
                                CustomerValueType = CustomerValueType.Other,
                                CustomerValues = new BaseCustomerValue[]
                                    {
                                        new OtherCustomerValue()
                                            {
                                                Description = RandomStringGenerator.GenerateRandomLatin(200),
                                                AdditionalInfo = RandomStringGenerator.GenerateRandomLatin(200),
                                            },
                                    }
                            }
                    })
                    {TestName = "Other"};
            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ReceptionDate = DateTime.UtcNow.Date,
                        CompletionDatePlanned = DateTime.UtcNow.Date.AddMonths(2),
                        AdditionalText = RandomStringGenerator.GenerateRandomLatin(2000),
                    })
                    {TestName = "Additional text"};
        }

        public static IEnumerable<TestCaseData> GenerateInvalidWorkOrders()
        {
            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 0),
                    }, "workOrderNumber", "number should be >= 1 and <= 999999")
                    {TestName = "Wrong order number"};

            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ShopRequisites = new ShopRequisites {Phone = "0123456789"},
                    }, "shopPhone", "phone should be 11 digits string")
                    {TestName = "Short phone"};
            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ShopRequisites = new ShopRequisites {Phone = "012345678901"},
                    }, "shopPhone", "phone should be 11 digits string")
                    {TestName = "Long phone"};
            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ShopRequisites = new ShopRequisites {Phone = "0123456789a"},
                    }, "shopPhone", "phone should be 11 digits string")
                    {TestName = "Phone with letter"};

            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ReceptionDate = new DateTime(2016, 6, 30),
                    }, "receptionDate", "reception date should be more then 01.07.2016 and less then 2 days after now")
                    {TestName = "Min reception date"};
            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ReceptionDate = DateTime.UtcNow.Date.AddDays(3),
                    }, "receptionDate", "reception date should be more then 01.07.2016 and less then 2 days after now")
                    {TestName = "Max reception date"};

            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ReceptionDate = DateTime.UtcNow.Date,
                        CompletionDatePlanned = DateTime.UtcNow.Date.AddDays(-1),
                    }, "completionDatePlanned", "completion date should be more then reception date and less then 1 year after reception date")
                    {TestName = "Min completion date planned"};
            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ReceptionDate = DateTime.UtcNow.Date,
                        CompletionDatePlanned = DateTime.UtcNow.Date.AddYears(1).AddDays(1),
                    }, "completionDatePlanned", "completion date should be more then reception date and less then 1 year after reception date")
                    {TestName = "Max completion date planned"};

            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ReceptionDate = DateTime.UtcNow.Date,
                        CompletionDatePlanned = DateTime.UtcNow.Date,
                        CompletionDateFact = DateTime.UtcNow.Date.AddDays(-1),
                    }, "completionDateFact", "completion date should be more then reception date and less then 1 year after reception date")
                    {TestName = "Min completion date fact"};
            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ReceptionDate = DateTime.UtcNow.Date,
                        CompletionDatePlanned = DateTime.UtcNow.Date,
                        CompletionDateFact = DateTime.UtcNow.Date.AddYears(1).AddDays(1),
                    }, "completionDateFact", "completion date should be more then reception date and less then 1 year after reception date")
                    {TestName = "Max completion date fact"};

            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ReceptionDate = DateTime.UtcNow.Date,
                        CompletionDatePlanned = DateTime.UtcNow.Date,
                        WarrantyNumber = RandomStringGenerator.GenerateRandomLatin(21)
                    }, "warrantyNumber", "warranty number length should be less then 20")
                    {TestName = "Warranty number length"};

            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ReceptionDate = DateTime.UtcNow.Date,
                        CompletionDatePlanned = DateTime.UtcNow.Date,
                        CustomerValues = new CustomerValueList
                            {
                                CustomerValueType = CustomerValueType.Vehicle,
                                CustomerValues = new BaseCustomerValue[] {new ApplianceCustomerValue()},
                            },
                    }, "customerValueType", "values should be of type Vehicle")
                    {TestName = "Vehicle value type"};
            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ReceptionDate = DateTime.UtcNow.Date,
                        CompletionDatePlanned = DateTime.UtcNow.Date,
                        CustomerValues = new CustomerValueList
                            {
                                CustomerValueType = CustomerValueType.Vehicle,
                                CustomerValues = new BaseCustomerValue[] {new VehicleCustomerValue {RegisterSign = "a"}},
                            },
                    }, "vehicleValueRegisterSign", "register sign contains wrong characters")
                    {TestName = "Vehicle value error"};

            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ReceptionDate = DateTime.UtcNow.Date,
                        CompletionDatePlanned = DateTime.UtcNow.Date,
                        CustomerValues = new CustomerValueList
                            {
                                CustomerValueType = CustomerValueType.Appliances,
                                CustomerValues = new BaseCustomerValue[] {new OtherCustomerValue()},
                            },
                    }, "customerValueType", "values should be of type Appliance")
                    {TestName = "Appliance value type"};
            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ReceptionDate = DateTime.UtcNow.Date,
                        CompletionDatePlanned = DateTime.UtcNow.Date,
                        CustomerValues = new CustomerValueList
                            {
                                CustomerValueType = CustomerValueType.Appliances,
                                CustomerValues = new BaseCustomerValue[] {new ApplianceCustomerValue {Brand = RandomStringGenerator.GenerateRandomLatin(41)}},
                            },
                    }, "applianceValueBrand", "brand should be less or equals to 40")
                    {TestName = "Appliance value error"};

            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ReceptionDate = DateTime.UtcNow.Date,
                        CompletionDatePlanned = DateTime.UtcNow.Date,
                        CustomerValues = new CustomerValueList
                            {
                                CustomerValueType = CustomerValueType.Other,
                                CustomerValues = new BaseCustomerValue[] {new VehicleCustomerValue()},
                            },
                    }, "customerValueType", "values should be of type Other")
                    {TestName = "Other value type"};
            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ReceptionDate = DateTime.UtcNow.Date,
                        CompletionDatePlanned = DateTime.UtcNow.Date,
                        CustomerValues = new CustomerValueList
                            {
                                CustomerValueType = CustomerValueType.Other,
                                CustomerValues = new BaseCustomerValue[] {new OtherCustomerValue {Description = RandomStringGenerator.GenerateRandomLatin(501)}},
                            },
                    }, "otherValueDescription", "description should be less or equals to 500")
                    {TestName = "Other value error"};

            yield return new TestCaseData(new WorkOrder
                    {
                        Number = new WorkOrderNumber("АА", 1),
                        ReceptionDate = DateTime.UtcNow.Date,
                        CompletionDatePlanned = DateTime.UtcNow.Date,
                        AdditionalText = RandomStringGenerator.GenerateRandomLatin(2001)
                    }, "additionalText", "additional text should be less or equals to 2000")
                    {TestName = "Additional text length"};
        }

        private readonly IValidator<WorkOrder> validator = new WorkOrderValidator(new WorkOrderNumberValidator(),
                                                                                  new VehicleValueValidator(),
                                                                                  new ApplianceValueValidator(),
                                                                                  new OtherValueValidator());
    }
}