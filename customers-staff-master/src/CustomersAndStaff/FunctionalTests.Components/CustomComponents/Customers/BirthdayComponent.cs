using Kontur.Selone.Properties;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Common;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.CtorAttributes;
using Market.CustomersAndStaff.Models.Customers;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Customers
{
    public class BirthdayComponent : ComponentBase
    {
        public BirthdayComponent(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public IProp<bool> Error => Prop.Create(() => BirthdayDay.Error.Get() && BirthdayMonth.Error.Get() && BirthdayYear.Error.Get(), "Birthday.Error");

        [IntCtor]
        public Input<int?> BirthdayDay { get; set; }
        public ComboBox BirthdayMonth { get; set; }
        [IntCtor]
        public Input<int?> BirthdayYear { get; set; }

        public void SetValue(Birthday birthday)
        {
            BirthdayDay.ResetValue(birthday.Day);
            BirthdayMonth.SelectByIndex(birthday.Month - 1);
            BirthdayYear.ResetValue(birthday.Year);
        }
    }
}