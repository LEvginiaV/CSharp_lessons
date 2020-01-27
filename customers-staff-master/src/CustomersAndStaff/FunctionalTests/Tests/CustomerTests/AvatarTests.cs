using System.Linq;

using GroboContainer.NUnitExtensions;

using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Customers;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.Repositories.Interface;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.CustomerTests
{
    public class AvatarTests : TestBase
    {
        [Test, Description("Создаем клиента с именем 'Лучший клиент', проверяем инициалы 'ЛК'")]
        public void CreateCustomerWithTwoWords()
        {
            var customerListPage =
                LoadMainPage()
                    .Do(page => page.NavigationLayout.CustomersLink.Click())
                    .GoToPage<CustomerListPage>();

            var customerEditorModal =
                customerListPage
                    .Do(page => page.EmptyPersonList.EmptyPersonListAddButton.Click())
                    .WaitModal<CustomerEditorModal>();

            customerEditorModal.Do(modal =>
                {
                    modal.Name.SetRawValue("Лучший клиент");
                    modal.AcceptButton.Click();
                });

            customerListPage.WaitModalClose<CustomerEditorModal>();

            var customerPage =
                customerListPage
                    .Do(page =>
                        {
                            page.CustomerItem.Count.Wait().EqualTo(1);
                            page.CustomerItem.First().Click();
                        })
                    .GoToPage<CustomerPage>();

            customerPage.Do(page => page.PersonHeader.Avatar.Text.Wait().EqualTo("ЛК"));
        }

        [Test, Description("Проверяем, что у клиента с именем 'Клиент' будут инициалы 'К'")]
        public void CheckCustomerWithOneWord()
        {
            customerRepository.CreateAsync(shop.OrganizationId, new Customer
                {
                    Name = "клиент",
                }).Wait();

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

            customerPage.Do(page => page.PersonHeader.Avatar.Text.Wait().EqualTo("К"));
        }

        [Test, Description("Проверяем, что у клиента с именем 'Лучший клиент навсегда' будут инициалы 'ЛК'")]
        public void CheckCustomerWithThreeWords()
        {
            customerRepository.CreateAsync(shop.OrganizationId, new Customer
                {
                    Name = "Лучший клиент навсегда",
                }).Wait();

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

            customerPage.Do(page =>
                {
                    page.PersonHeader.Avatar.Text.Wait().EqualTo("ЛК");
                    page.PersonHeader.Gender.Wait().EqualTo(null);
                });
        }

        [Test, Description("Проверяем, что у клиента-девочки будет картинка девочки")]
        public void CheckCustomerFemale()
        {
            customerRepository.CreateAsync(shop.OrganizationId, new Customer
                {
                    Name = "Девочка",
                    Gender = Gender.Female,
                }).Wait();

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

            customerPage.Do(page =>
                {
                    page.PersonHeader.Avatar.Text.Wait().EqualTo("");
                    page.PersonHeader.Gender.Wait().EqualTo(Gender.Female);
                });
        }

        [Test, Description("Проверяем, что у клиента-мальчика без имени будет картинка мальчика")]
        public void CheckCustomerMale()
        {
            customerRepository.CreateAsync(shop.OrganizationId, new Customer
                {
                    CustomId = "123",
                    Gender = Gender.Male,
                }).Wait();

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

            customerPage.Do(page =>
                {
                    page.PersonHeader.Avatar.Text.Wait().EqualTo("");
                    page.PersonHeader.Gender.Wait().EqualTo(Gender.Male);
                });
        }

        [Test, Description("Проверяем, что у клиента без имени и пола будет нарисован ?")]
        public void CheckCustomerWithoutNameAndGender()
        {
            customerRepository.CreateAsync(shop.OrganizationId, new Customer
                {
                    Phone = "79112223344",
                }).Wait();

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

            customerPage.Do(page =>
                {
                    page.PersonHeader.Avatar.Text.Wait().EqualTo("?");
                    page.PersonHeader.Gender.Wait().EqualTo(null);
                });
        }

        [Injected]
        private readonly ICustomerRepository customerRepository;
    }
}