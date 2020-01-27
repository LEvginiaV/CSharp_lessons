using Kontur.Selone.Properties;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.Models.Customers;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Components.CustomComponents.Common
{
    public class PersonHeader : ComponentBase
    {
        public PersonHeader(ISearchContext searchContext, By @by)
            : base(searchContext, @by)
        {
        }

        public Avatar Avatar { get; set; }
        public Label PersonName { get; set; }
        public Link PersonEditLink { get; set; }
        public Label PersonDescription { get; set; }
        public Link PersonRemoveLink { get; set; }

        public BackButton BackButton { get; set; }


        public IProp<Gender?> Gender => Prop.Create(() => ToGender(Container.GetAttribute("data-prop-gendertype")), "Avatar.Gender");

        private static Gender? ToGender(string str)
        {
            switch (str)
            {
            case "male":
                return Models.Customers.Gender.Male;
            case "female":
                return Models.Customers.Gender.Female;
            default:
                return null;
            }
        }
    }
}