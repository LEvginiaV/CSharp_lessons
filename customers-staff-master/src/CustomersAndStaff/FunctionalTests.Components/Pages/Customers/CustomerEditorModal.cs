using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Common;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.CtorAttributes;
using Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Customers;
using Market.CustomersAndStaff.FunctionalTests.Components.Infrastructure;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.Pages.Customers
{
    [TidModal("CustomerEditor")]
    public class CustomerEditorModal : ModalBase
    {
        public CustomerEditorModal(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public Input Name { get; set; }
        [PhoneCtor]
        public Input<string> Phone { get; set; }
        [PercentCtor]
        public Input<decimal?> Discount { get; set; }
        public Input CustomId { get; set; }
        public Input Email { get; set; }
        public Switcher GenderSelector { get; set; }
        public BirthdayComponent Birthday { get; set; }
        //todo gender
        public TextArea AdditionalInfo { get; set; }

        public Button AcceptButton { get; set; }
        public Button CancelButton { get; set; }
    }
}