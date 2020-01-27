using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.PropTransformations;
using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.CtorAttributes
{
    public class BirthdayCtorAttribute : AdditionalConstructorArgsAttribute
    {
        public BirthdayCtorAttribute()
        {
            Args = new object[]
                {
                    new BirthdayPropTransformation(), 
                };
        }
    }
}