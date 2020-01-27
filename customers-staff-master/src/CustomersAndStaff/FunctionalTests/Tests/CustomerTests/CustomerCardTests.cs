using System.Linq;
using System.Threading.Tasks;

using GroboContainer.NUnitExtensions;

using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Customers;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.Repositories.Interface;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.CustomerTests
{
    public class CustomerCardTests : TestBase
    {
        [Test, Description("Редактирование уже созданного клиента с поиском по новой информации")]
        public async Task EditCustomerTest()
        {
            await customerRepository.CreateAsync(shop.OrganizationId, new Customer
                {
                    Name = "Василий",
                    CustomId = "Vasily",
                    Phone = "79112223344",
                    AdditionalInfo = "Vasily-Vasily",
                    Discount = 20.3m,
                    Birthday = new Birthday(2, 3, 2008),
                    Email = "vasya@gmail.com",
                });

            var customerListPage =
                LoadMainPage()
                    .Do(page => page.NavigationLayout.CustomersLink.Click())
                    .GoToPage<CustomerListPage>();

            var customerPage =
                customerListPage
                    .Do(page =>
                        {
                            page.CustomerItem.Count.Wait().EqualTo(1);
                            page.CustomerItem.First().Click();
                        })
                    .GoToPage<CustomerPage>();

            var customerEditorModal =
                customerPage
                    .Do(page => page.PersonHeader.PersonEditLink.Click())
                    .WaitModal<CustomerEditorModal>();

            customerEditorModal
                .Do(modal =>
                    {
                        modal.Name.ResetRawValue("Пётр");
                        modal.CustomId.ResetRawValue("Peter");
                        modal.Phone.ResetValue("79443332211");
                        modal.Discount.ResetValue(0);
                        modal.GenderSelector.GetButton(0).Click();
                        modal.AdditionalInfo.ResetRawValue("Peter-Peter");
                        modal.Birthday.SetValue(new Birthday(14, 5));
                        modal.Email.ResetRawValue("peter@gmail.com");
                        modal.AcceptButton.Click();
                    });

            customerPage.WaitModalClose<CustomerEditorModal>();
            customerPage = customerPage.GoToPage<CustomerPage>();

            customerListPage =
                customerPage
                    .Do(page =>
                        {
                            page.PersonHeader.PersonName.Text.Wait().EqualTo("Пётр");
                            page.PersonHeader.PersonDescription.Text.Wait().EqualTo("+7 944 333-22-11");
                            page.AdditionalInfo.Text.Wait().EqualTo("Peter-Peter");
                            page.CustomId.Text.Wait().EqualTo("Peter");
                            page.Discount.Value.Wait().EqualTo(0);
                            page.Birthday.Value.Wait().EqualTo(new Birthday(14, 5));
                            page.Email.Text.Wait().EqualTo("peter@gmail.com");
                            page.PersonHeader.Gender.Wait().EqualTo(Gender.Male);

                            page.PersonHeader.BackButton.Click();
                        })
                    .GoToPage<CustomerListPage>();

            customerListPage
                .Do(page =>
                    {
                        page.CustomerItem.Select(x => x.ToActualData()).Wait().ShouldBeEquivalentTo(new[]
                            {
                                new Customer
                                    {
                                        Name = "Пётр",
                                        CustomId = "Peter",
                                        Phone = "79443332211",
                                        Discount = 0
                                    }
                            });
                        page.SearchInput.SetRawValue("Пётр");
                        page.CustomerItem.Select(x => x.ToActualData()).Wait().ShouldBeEquivalentTo(new[]
                            {
                                new Customer
                                    {
                                        Name = "Пётр",
                                        CustomId = "Peter",
                                        Phone = "79443332211",
                                        Discount = 0
                                    }
                            });
                        page.SearchInput.ResetRawValue("Peter");
                        page.CustomerItem.Select(x => x.ToActualData()).Wait().ShouldBeEquivalentTo(new[]
                            {
                                new Customer
                                    {
                                        Name = "Пётр",
                                        CustomId = "Peter",
                                        Phone = "79443332211",
                                        Discount = 0
                                    }
                            });
                    });
        }

        [Injected]
        private readonly ICustomerRepository customerRepository;
    }
}