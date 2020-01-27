using System;

namespace Market.CustomersAndStaff.FunctionalTests.Infrastructure
{
    public static class FlowExtensions
    {
        public static T Do<T>(this T value, Action<T> action)
        {
            action(value);
            return value;
        }
    }
}