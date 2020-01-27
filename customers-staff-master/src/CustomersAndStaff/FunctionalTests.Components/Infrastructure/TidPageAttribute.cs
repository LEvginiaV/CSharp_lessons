using System;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TidPageAttribute : Attribute
    {
        public TidPageAttribute(string tid)
        {
            Tid = tid;
        }

        public string Tid { get; }
    }
}