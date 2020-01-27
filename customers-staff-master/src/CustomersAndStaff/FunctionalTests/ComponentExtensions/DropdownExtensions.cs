using System;
using System.Linq;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Helpers;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;

namespace Market.CustomersAndStaff.FunctionalTests.ComponentExtensions
{
    public static class DropdownExtensions
    {
        public static void SelectByValue(this Dropdown dropdown, string value)
        {
            dropdown.Click();
            dropdown.ItemListPresent.Wait().EqualTo(true);
            dropdown.GetMenuItemList<Button>().First(x => x.Text.Get() == value).Click();
        }

        public static void SelectByValue(this Dropdown dropdown, Enum value)
        {
            dropdown.SelectByValue(value.GetDescription());
        }

        public static void WaitValue(this Dropdown dropdown, Enum value)
        {
            dropdown.Text.Wait().EqualTo(value.GetDescription());
        }
    }
}