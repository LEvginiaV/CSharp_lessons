using System.Linq;
using System.Threading.Tasks;

using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Customers;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.Models.Customers;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.CustomerTests
{
    public class StoringTest : TestBase
    {
        [Test, Description("Создаем клиента в одной торговой точке, смотрим, что он появился в другой")]
        public async Task TwoSalesPointsOneOrganization()
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
                    modal.Name.SetRawValue("Василий Петров-Водкин.");
                    modal.CustomId.SetRawValue("Vasily");
                    modal.Phone.SetValue("79112223344");
                    modal.Discount.SetValue(20.35m);
                    modal.AcceptButton.Click();
                });

            customerListPage.WaitModalClose<CustomerEditorModal>();

            customerListPage
                .Do(page => page.CustomerItem.Select(x => x.ToActualData()).Wait().ShouldBeEquivalentTo(new[]
                    {
                        new Customer {Name = "Василий Петров-Водкин.", CustomId = "Vasily", Phone = "79112223344", Discount = 20.35m},
                    }));

            var newShop = await CreateShop(shop.OrganizationId, shop.DepartmentId);
            LoadMainPage(newShop)
                .Do(page => page.NavigationLayout.CustomersLink.Click())
                .GoToPage<CustomerListPage>()
                .Do(page => page.CustomerItem.Select(x => x.ToActualData()).Wait().ShouldBeEquivalentTo(new[]
                    {
                        new Customer {Name = "Василий Петров-Водкин.", CustomId = "Vasily", Phone = "79112223344", Discount = 20.35m},
                    }));
        }
    }
}