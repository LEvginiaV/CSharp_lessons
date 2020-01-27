using System.Linq;
using System.Threading.Tasks;

using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Workers;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.Models.Workers;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.WorkerTests
{
    public class StoringTest : TestBase
    {
        [Test, Description("Создаем сотрудника в одной торговой точке, смотрим, что он не появился в другой")]
        public async Task TwoSalesPointsOneOrganization()
        {
            var workerListPage =
                LoadMainPage()
                    .Do(page => page.NavigationLayout.WorkersLink.Click())
                    .GoToPage<WorkerListPage>();

            var workerEditorModal =
                workerListPage
                    .Do(page => page.EmptyPersonList.EmptyPersonListAddButton.Click())
                    .WaitModal<WorkerEditorModal>();

            workerEditorModal.Do(modal =>
                {
                    modal.Name.SetRawValue("Василий Петров-Водкин.");
                    modal.Position.SetRawValue("Vasily");
                    modal.AcceptButton.Click();
                });

            workerListPage.WaitModalClose<WorkerEditorModal>();

            workerListPage
                .Do(page => page.WorkerItem.Select(x => x.ToActualData()).Wait().ShouldBeEquivalentTo(new[]
                    {
                        new Worker {FullName = "Василий Петров-Водкин.", Position = "Vasily"},
                    }));

            var newShop = await CreateShop(shop.OrganizationId, shop.DepartmentId);
            LoadMainPage(newShop)
                .Do(page => page.NavigationLayout.WorkersLink.Click())
                .GoToPage<WorkerListPage>()
                .Do(page =>
                    {
                        page.EmptyPersonList.IsPresent.Wait().EqualTo(true);
                    });
        }
    }
}