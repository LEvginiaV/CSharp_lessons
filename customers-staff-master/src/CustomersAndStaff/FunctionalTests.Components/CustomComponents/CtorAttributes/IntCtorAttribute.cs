using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.PropTransformations;
using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.CtorAttributes
{
    public class IntCtorAttribute : AdditionalConstructorArgsAttribute
    {
        public IntCtorAttribute()
        {
            Args = new object[]
                {
                    new IntPropTransformation(),
                };
        }
    }
}