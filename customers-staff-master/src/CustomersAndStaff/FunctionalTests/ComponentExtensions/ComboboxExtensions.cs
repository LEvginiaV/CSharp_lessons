using System.Linq;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;

namespace Market.CustomersAndStaff.FunctionalTests.ComponentExtensions
{
    public static class ComboboxExtensions
    {
        public static void TypeAndSelect(this ComboBox comboBox, string value)
        {
            comboBox.SetRawValue(value);
            comboBox.GetMenuItemList<Button>().Count.Wait().EqualTo(1);
            comboBox.GetMenuItemList<Button>().First().Click();
        }
    }
}