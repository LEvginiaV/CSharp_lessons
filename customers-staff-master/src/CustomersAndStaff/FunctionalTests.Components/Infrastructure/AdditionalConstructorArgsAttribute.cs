using System;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AdditionalConstructorArgsAttribute : Attribute
    {
        public AdditionalConstructorArgsAttribute(params object[] args)
        {
            Args = args;
        }

        public object[] Args { get; set; }
    }
}