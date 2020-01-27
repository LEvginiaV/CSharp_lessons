using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.PropTransformations;
using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.CtorAttributes
{
    public class DateCtorAttribute : AdditionalConstructorArgsAttribute
    {
        public DateCtorAttribute()
        {
            Args = new object[] {new DatePropTransformation()};
        }
    }
}