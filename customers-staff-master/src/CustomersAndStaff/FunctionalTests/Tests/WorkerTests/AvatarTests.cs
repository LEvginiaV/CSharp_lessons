using System.Linq;

using GroboContainer.NUnitExtensions;

using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Workers;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.Repositories.Interface;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.WorkerTests
{
    public class AvatarTests : TestBase
    {
        [Test, Description("Создаем сотрудника с именем 'Лучший сотрудник', проверяем инициалы 'ЛС'")]
        public void CreateWorkerWithTwoWords()
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
                    modal.Name.SetRawValue("Лучший сотрудник");
                    modal.AcceptButton.Click();
                });

            workerListPage.WaitModalClose<WorkerEditorModal>();

            var workerPage =
                workerListPage
                    .Do(page =>
                        {
                            page.WorkerItem.Count.Wait().EqualTo(1);
                            page.WorkerItem.First().Click();
                        })
                    .GoToPage<WorkerPage>();

            workerPage.Do(page => page.PersonHeader.Avatar.Text.Wait().EqualTo("ЛС"));
        }

        [Test, Description("Проверяем, что у сотрудника с именем 'Сотрудник' будут инициалы 'С'")]
        public void CheckWorkerWithOneWord()
        {
            workerRepository.CreateAsync(shop.Id, new Worker
                {
                    FullName = "Сотрудник",
                }).Wait();

            var workerListPage =
                LoadMainPage()
                    .Do(page => page.NavigationLayout.WorkersLink.Click())
                    .GoToPage<WorkerListPage>();

            var workerPage =
                workerListPage
                    .Do(page =>
                        {
                            page.WorkerItem.Count.Wait().EqualTo(1);
                            page.WorkerItem.First().Click();
                        })
                    .GoToPage<WorkerPage>();

            workerPage.Do(page => page.PersonHeader.Avatar.Text.Wait().EqualTo("С"));
        }

        [Test, Description("Проверяем, что у сотрудника с именем 'Лучший сотрудник навсегда' будут инициалы 'ЛС'")]
        public void CheckWorkerWithThreeWords()
        {
            workerRepository.CreateAsync(shop.Id, new Worker
                {
                    FullName = "Лучший сотрудник навсегда",
                }).Wait();

            var workerListPage =
                LoadMainPage()
                    .Do(page => page.NavigationLayout.WorkersLink.Click())
                    .GoToPage<WorkerListPage>();

            var workerPage =
                workerListPage
                    .Do(page =>
                        {
                            page.WorkerItem.Count.Wait().EqualTo(1);
                            page.WorkerItem.First().Click();
                        })
                    .GoToPage<WorkerPage>();

            workerPage.Do(page => page.PersonHeader.Avatar.Text.Wait().EqualTo("ЛС"));
        }

        [Injected]
        private readonly IWorkerRepository workerRepository;
    }
}