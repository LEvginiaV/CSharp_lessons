using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Common;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.CtorAttributes;
using Market.CustomersAndStaff.Models.Customers;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Customers
{
    public class CustomerItem : ComponentBase
    {
        public CustomerItem(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public void Click()
        {
            Container.Click();
        }

        [DefaultValueCtor("Имя не указано")]
        public Label<string> Name { get; set; }
        [DefaultValueCtor("не указана")]
        public Label<string> CustomId { get; set; }
        [PhoneCtor(true, "не указан")]
        public Label<string> Phone { get; set; }
        [PercentCtor(true, "нет")]
        public Label<decimal?> Discount { get; set; }

        public Customer ToActualData()
        {
            return new Customer
                {
                    Name = Name.Value.Get(),
                    CustomId = CustomId.IsPresent.Get() ? CustomId.Value.Get() : null,
                    Phone = Phone.Value.Get(),
                    Discount = Discount.Value.Get(),
                };
        }
    }
}