using System.Linq;
using System.Threading.Tasks;

using AutoFixture;

using GroboContainer.NUnitExtensions;

using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Workers;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Tests.Core.Configuration;

using MoreLinq;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.WorkerTests
{
    [WithCustomizedFixture]
    public class WorkerListTests : TestBase
    {
        [Test, Description("Пустой список, создаем нового сотрудника")]
        public void CreateNewWorkerTest()
        {
            var workerListPage =
                LoadMainPage()
                    .Do(page => page.NavigationLayout.WorkersLink.Click())
                    .GoToPage<WorkerListPage>();

            var workerEditorModal =
                workerListPage
                    .Do(page =>
                        {
                            page.EmptyPersonList.EmptyPersonListAddButton.IsPresent.Wait().EqualTo(true);
                            page.EmptyPersonList.EmptyPersonListAddButton.Click();
                        })
                    .WaitModal<WorkerEditorModal>();

            workerEditorModal
                .Do(modal =>
                    {
                        modal.Name.SetRawValue("Вася");
                        modal.Position.SetRawValue("Vasya");
                        modal.AcceptButton.Click();
                    });

            workerListPage.WaitModalClose<WorkerEditorModal>();

            workerListPage
                .Do(page =>
                    {
                        page.EmptyPersonList.IsPresent.Wait().EqualTo(false);
                        page.WorkerItem.Select(x => x.ToActualData()).Wait().ShouldBeEquivalentTo(new[]
                            {
                                new Worker {FullName = "Вася", Position = "Vasya"},
                            });
                    });
        }

        [Test, Description("В базе есть один сотрудник, добавляем еще одного")]
        public async Task CreateAnotherWorker()
        {
            await workerRepository.CreateAsync(shop.Id, new Worker {FullName = "Сотрудник один", Position = "Должность один"});

            var workerListPage =
                LoadMainPage()
                    .Do(page => page.NavigationLayout.WorkersLink.Click())
                    .GoToPage<WorkerListPage>();

            var workerEditorModal =
                workerListPage
                    .Do(page =>
                        {
                            page.EmptyPersonList.IsPresent.Wait().EqualTo(false);
                            page.WorkerItem.Select(x => x.ToActualData()).Wait().ShouldBeEquivalentTo(new[]
                                {
                                    new Worker {FullName = "Сотрудник один", Position = "Должность один"},
                                });
                            page.AddButton.Click();
                        })
                    .WaitModal<WorkerEditorModal>();

            workerEditorModal
                .Do(modal =>
                    {
                        modal.Name.SetRawValue("Сотрудник два");
                        modal.Position.SetRawValue("Должность два");
                        modal.AcceptButton.Click();
                    });

            workerListPage.WaitModalClose<WorkerEditorModal>();

            workerListPage
                .Do(page =>
                    {
                        page.WorkerItem.Select(x => x.ToActualData()).Wait().ShouldBeEquivalentTo(new[]
                            {
                                new Worker {FullName = "Сотрудник один", Position = "Должность один"},
                                new Worker {FullName = "Сотрудник два", Position = "Должность два"},
                            });
                    });
        }

        [Test, Description("Создаем список из 31 сотрудника, проверяем, что появляется пейджинг")]
        public async Task PagingTest()
        {
            var workers = fixture.CreateMany<Worker>(31).ToArray();
            workers.ForEach(x => x.IsDeleted = false);
            await workerRepository.CreateManyAsync(shop.Id, workers);

            var workerListPage =
                LoadMainPage()
                    .Do(page => page.NavigationLayout.WorkersLink.Click())
                    .GoToPage<WorkerListPage>();

            workerListPage
                .Do(page =>
                    {
                        page.Paging.IsPresent.Wait().EqualTo(true);
                        page.Paging.PagesCount.Wait().EqualTo(2);

                        page.WorkerItem.Count.Wait().EqualTo(30);
                        page.Paging.LinkTo(2).Click();
                        page.WorkerItem.Count.Wait().EqualTo(1);
                    });
        }

        [Test, Description("Сортировка по имени")]
        public async Task SortingTest()
        {
            var workers = new[]
                {
                    new Worker {FullName = "а сотрудник", Position = "а должность"},
                    new Worker {FullName = "Ё сотрудник", Position = "а должность"},
                    new Worker {FullName = "я сотрудник", Position = "а должность"},
                    
                    new Worker {FullName = "АБ сотрудник", Position = "Я должность"},
                    new Worker {FullName = "Ек сотрудник", Position = "Я должность"},
                    new Worker {FullName = "ЯЯ сотрудник", Position = "Я должность"},
                    
                    new Worker {FullName = "АВ сотрудник", Position = null},
                    new Worker {FullName = "Ел сотрудник", Position = null},
                    new Worker {FullName = "ыЯ сотрудник", Position = null},
                };

            await workerRepository.CreateManyAsync(shop.Id, workers);

            var workerListPage =
                LoadMainPage()
                    .Do(page => page.NavigationLayout.WorkersLink.Click())
                    .GoToPage<WorkerListPage>();

            workerListPage
                .Do(page =>
                    {
                        page.WorkerItem.Select(x => x.ToActualData()).Wait().ShouldBeEquivalentTo(workers, opt =>
                            {
                                return opt.Including(x => x.FullName)
                                          .Including(x => x.Position)
                                          .WithStrictOrdering();
                            });
                    });
        }

        [Injected]
        private readonly IWorkerRepository workerRepository;

        [Injected]
        private readonly IFixture fixture;
    }
}