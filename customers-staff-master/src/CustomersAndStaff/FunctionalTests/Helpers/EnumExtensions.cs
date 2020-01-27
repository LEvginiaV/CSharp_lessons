using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Market.CustomersAndStaff.FunctionalTests.Helpers
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum val)
        {
            return val.GetType()
                      .GetMember(val.ToString())
                      .FirstOrDefault()?
                      .GetCustomAttribute<DescriptionAttribute>()?
                      .Description;
        }
    }
}