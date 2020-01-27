using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using AutoFixture;
using AutoFixture.Dsl;

using GroboContainer.NUnitExtensions;
using GroboContainer.NUnitExtensions.Impl.TestContext;

using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.Models.WorkOrders;

namespace Market.CustomersAndStaff.Tests.Core.Configuration
{
    public class WithCustomizedFixture : GroboTestSuiteWrapperAttribute
    {
        public override void SetUp(string suiteName, Assembly testAssembly, IEditableGroboTestContext suiteContext)
        {
            var random = new Random();
            var fixture = new Fixture();
            fixture.Customize((ICustomizationComposer<Customer> comp) =>
                                  comp.With(x => x.Birthday,
                                            () => new Birthday(random.Next(1, 28), random.Next(1, 12)))
                                      .With(x => x.Discount, () => random.Next(10000) / 100m)
                                      .With(x => x.Name, () => RandomStringGenerator.GenerateRandomCyrillic(30)));
            fixture.Customize((ICustomizationComposer<Worker> comp) =>
                                  comp.With(x => x.FullName, () => RandomStringGenerator.GenerateRandomCyrillic(30))
                                      .With(x => x.Phone, () => random.Next(100, 100000000).ToString()));

            fixture.Customize((ICustomizationComposer<WorkOrder> comp) =>
                                  comp.With(x => x.Number, () => new WorkOrderNumber("ВГ", random.Next(1, 1000000)))
                                      .With(x => x.CustomerValues, () =>
                                          {
                                              var valueType = fixture.Create<CustomerValueType>();
                                              IEnumerable<BaseCustomerValue> values;
                                              switch(valueType)
                                              {
                                              case CustomerValueType.Appliances:
                                                  values = fixture.CreateMany<ApplianceCustomerValue>();
                                                  break;
                                              case CustomerValueType.Vehicle:
                                                  values = fixture.CreateMany<VehicleCustomerValue>();
                                                  break;
                                              case CustomerValueType.Other:
                                                  values = fixture.CreateMany<OtherCustomerValue>();
                                                  break;
                                              default:
                                                  throw new ArgumentOutOfRangeException();
                                              }

                                              return new CustomerValueList
                                                  {
                                                      CustomerValueType = valueType,
                                                      CustomerValues = values.ToArray(),
                                                  };
                                          }));
            suiteContext.Container.Configurator.ForAbstraction<Fixture>().UseInstances(fixture);
            suiteContext.Container.Configurator.ForAbstraction<IFixture>().UseInstances(fixture);
        }
    }
}