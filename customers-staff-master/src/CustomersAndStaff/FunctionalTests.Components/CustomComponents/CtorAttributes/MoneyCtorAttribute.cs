using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.PropTransformations;
using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.CtorAttributes
{
    public class MoneyCtorAttribute : AdditionalConstructorArgsAttribute
    {
        public MoneyCtorAttribute(string defaultValue = null, bool useRubleSign = true)
        {
            Args = new object[]
                {
                    new MoneyPropTransformation(defaultValue, useRubleSign), 
                };
        }
    }
}