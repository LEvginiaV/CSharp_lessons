using System;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TidModalAttribute : Attribute
    {
        public TidModalAttribute(string tid)
        {
            Tid = tid;
        }

        public string Tid { get; }
    }
}