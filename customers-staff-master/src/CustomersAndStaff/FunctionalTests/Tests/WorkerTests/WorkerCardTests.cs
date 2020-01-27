using System.Linq;
using System.Threading.Tasks;

using GroboContainer.NUnitExtensions;

using Kontur.Selone.Extensions;

using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Workers;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.Repositories.Interface;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.WorkerTests
{
    public class WorkerCardTests : TestBase
    {
        [Test, Description("Редактирование уже созданного сотрудника")]
        public async Task EditWorkerTest()
        {
            await workerRepository.CreateAsync(shop.Id, new Worker
                {
                    FullName = "Василий",
                    Position = "Vasily",
                    Phone = "79112223344",
                    AdditionalInfo = "Vasily-Vasily",
                });

            var workerListPage = GoToWorkerListPage();
            workerListPage.WorkerItem.Count.Wait().EqualTo(1);
            var workerPage = GoToWorkerPage(workerListPage, 0);

            var workerEditorModal =
                workerPage
                    .Do(page =>
                        {
                            page.Code.Text.Wait().EqualTo("1");
                            page.CodeHelpCaption.HelpTooltip.Container.Mouseover();
                            page.CodeHelpCaption.HelpTooltip.IsOpened.Wait().EqualTo(true);
                            page.PersonHeader.PersonEditLink.Click();
                        })
                    .WaitModal<WorkerEditorModal>();

            workerEditorModal
                .Do(modal =>
                    {
                        modal.Name.ResetRawValue("Пётр");
                        modal.Position.ResetRawValue("Peter");
                        modal.Position.Unfocus();
                        modal.Phone.ResetRawValue("9443332211");
                        modal.Description.ResetRawValue("Peter-Peter");
                        modal.AcceptButton.Click();
                    });

            workerPage.WaitModalClose<WorkerEditorModal>();
            workerPage = workerPage.GoToPage<WorkerPage>();

            workerListPage =
                workerPage
                    .Do(page =>
                        {
                            page.PersonHeader.PersonName.Text.Wait().EqualTo("Пётр");
                            page.PersonHeader.PersonDescription.Text.Wait().EqualTo("Peter");
                            page.Phone.Text.Wait().EqualTo("+7 944 333-22-11");
                            page.Description.Text.Wait().EqualTo("Peter-Peter");
                            page.Code.Text.Wait().EqualTo("1");

                            page.PersonHeader.BackButton.Click();
                        })
                    .GoToPage<WorkerListPage>();
        }

        [Test, Description("Удаление сотрудника")]
        public async Task RemoveWorkerTest()
        {
            await workerRepository.CreateManyAsync(shop.Id, new[]
                {
                    new Worker {FullName = "Василий", Position = "Vasily"},
                    new Worker {FullName = "Пётр", Position = "Peter"},
                });

            var workerListPage = GoToWorkerListPage();

            var workerPage =
                workerListPage
                    .Do(page =>
                        {
                            page.WorkerItem.Select(x => x.ToActualData()).Wait().ShouldBeEquivalentTo(new[]
                                {
                                    new Worker {FullName = "Василий", Position = "Vasily"},
                                    new Worker {FullName = "Пётр", Position = "Peter"},
                                });
                            page.WorkerItem.First(x => x.Name.Text.Get().Equals("Василий")).Click();
                        })
                    .GoToPage<WorkerPage>();

            workerPage
                .Do(page => page.PersonHeader.PersonRemoveLink.Click())
                .WaitModal<RemoveWorkerModal>()
                .Do(modal => modal.Accept.Click());
            workerPage.WaitModalClose<RemoveWorkerModal>();

            workerListPage = workerPage.GoToPage<WorkerListPage>();

            workerListPage
                .Do(page =>
                    {
                        page.WorkerItem.Select(x => x.ToActualData()).Wait().ShouldBeEquivalentTo(new[]
                            {
                                new Worker {FullName = "Пётр", Position = "Peter"},
                            });
                    });
        }

        [Test, Description("Нажимаем удалить сотрудника, но потом отменяем")]
        public async Task CancelRemoveWorkerTest()
        {
            await workerRepository.CreateManyAsync(shop.Id, new[]
                {
                    new Worker {FullName = "Василий", Position = "Vasily"},
                });

            var workerListPage = GoToWorkerListPage();

            var workerPage =
                workerListPage
                    .Do(page =>
                        {
                            page.WorkerItem.Select(x => x.ToActualData()).Wait().ShouldBeEquivalentTo(new[]
                                {
                                    new Worker {FullName = "Василий", Position = "Vasily"},
                                });
                            page.WorkerItem.First().Click();
                        })
                    .GoToPage<WorkerPage>();

            workerPage
                .Do(page => page.PersonHeader.PersonRemoveLink.Click())
                .WaitModal<RemoveWorkerModal>()
                .Do(modal => modal.Cancel.Click());
            workerPage.WaitModalClose<RemoveWorkerModal>();

            workerPage
                .GoToPage<WorkerPage>()
                .Do(page => page.PersonHeader.PersonName.Text.Wait().EqualTo("Василий"));
        }

        [Test, Description("Если телефон не заполнен, отображается 'не указан'")]
        public async Task EmptyPhoneLabelTest()
        {
            await workerRepository.CreateAsync(shop.Id, new Worker { FullName = "Василий" });
            var workerListPage = GoToWorkerListPage();
            var workerPage = GoToWorkerPage(workerListPage, 0);

            workerPage.Phone.Text.Wait().EqualTo("не указан");
        }

        private WorkerListPage GoToWorkerListPage()
        {
            var workerListPage =
                LoadMainPage()
                    .Do(page => page.NavigationLayout.WorkersLink.Click())
                    .GoToPage<WorkerListPage>();
            return workerListPage;
        }

        private static WorkerPage GoToWorkerPage(WorkerListPage workerListPage, int workerIndex)
        {
            var workerPage =
                workerListPage
                    .Do(page =>
                        {
                            page.WorkerItem.Count.Wait().MoreOrEqual(workerIndex + 1);
                            page.WorkerItem.ElementAt(workerIndex).Click();
                        })
                    .GoToPage<WorkerPage>();
            return workerPage;
        }

        [Injected]
        private readonly IWorkerRepository workerRepository;
    }
}