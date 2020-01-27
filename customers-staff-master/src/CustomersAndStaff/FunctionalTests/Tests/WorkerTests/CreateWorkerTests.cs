using System;
using System.Linq;
using System.Threading.Tasks;

using GroboContainer.NUnitExtensions;

using Market.CustomersAndStaff.FunctionalTests.Components.Components;
using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Workers;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.Models.Workers;
using Market.CustomersAndStaff.Repositories.Interface;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.WorkerTests
{
    public class CreateWorkerTests : TestBase
    {
        [Test, Description("Валидации полей")]
        public void FieldValidationTest()
        {
            var workerListPage =
                LoadMainPage()
                    .Do(page => page.NavigationLayout.WorkersLink.Click())
                    .GoToPage<WorkerListPage>();

            var workerEditorModal =
                workerListPage
                    .Do(page =>
                        {
                            page.EmptyPersonList.EmptyPersonListAddButton.Click();
                        })
                    .WaitModal<WorkerEditorModal>();

            workerEditorModal.Do(modal =>
                {
                    modal.Name.MaxLength.Wait().EqualTo(120);
                    var name = string.Join("ё", Enumerable.Range(0, 5).Select(x => Guid.NewGuid()));
                    modal.Name.SetRawValue(name);
                    modal.Name.WaitText(name.Substring(0, 120));
                });

            workerEditorModal.Do(modal =>
                {
                    modal.Phone.SetRawValue("+");
                    modal.Phone.WaitText(string.Empty);
                    modal.Phone.SetRawValue("abc");
                    modal.Phone.WaitText(string.Empty);
                    
                    modal.Phone.ResetRawValue("9999");
                    modal.Phone.Unfocus();
                    modal.Phone.Error.WaitBool(true);
                    
                    modal.Phone.ResetRawValue("9112223344");
                    modal.Phone.WaitText("911 222-33-44");
                });

            workerEditorModal.Do(modal =>
                {
                    modal.Position.Click();
                    modal.Position.MaxLength.Wait().EqualTo(120);
                    modal.Position.SetRawValue("Vasya");
                    modal.Position.Text.Wait().EqualTo("Vasya");
                });

            workerEditorModal.Do(modal =>
                {
                    modal.Description.MaxLength.Wait().EqualTo(500);
                    modal.Description.SetRawValue("bla-bla-bla");
                    modal.Description.Text.Wait().EqualTo("bla-bla-bla");
                });
        }

        [Test, Description("Создаем сотрудника с пустым именем, получаем ошибку. Вводим имя - все ОК")]
        public void TryCreateWithEmptyNameTest()
        {
            var workerListPage =
                LoadMainPage()
                    .Do(page => page.NavigationLayout.WorkersLink.Click())
                    .GoToPage<WorkerListPage>();

            var workerEditorModal =
                workerListPage
                    .Do(page =>
                        {
                            page.EmptyPersonList.EmptyPersonListAddButton.Click();
                        })
                    .WaitModal<WorkerEditorModal>();

            workerEditorModal.Do(modal =>
                {
                    modal.AcceptButton.Click();
                    modal.Name.Error.Wait().EqualTo(true);
                    modal.Name.SetRawValue("Василий");
                    modal.AcceptButton.Click();
                });

            workerListPage.WaitModalClose<WorkerEditorModal>();
        }

        [Test, Description("Поле 'должность' запоминает вводимые значения")]
        public void RememberPositions()
        {
            var workerListPage =
                LoadMainPage()
                    .Do(page => page.NavigationLayout.WorkersLink.Click())
                    .GoToPage<WorkerListPage>();

            var workerEditorModal =
                workerListPage
                    .Do(page =>
                        {
                            page.EmptyPersonList.EmptyPersonListAddButton.Click();
                        })
                    .WaitModal<WorkerEditorModal>();

            workerEditorModal.Do(modal =>
                {
                    modal.Name.SetRawValue("Василий");
                    modal.Position.SetRawValue("Vasily");
                    modal.AcceptButton.Click();
                });

            workerListPage.WaitModalClose<WorkerEditorModal>();

            workerEditorModal =
                workerListPage
                    .Do(page =>
                        {
                            page.AddButton.IsPresent.Wait().EqualTo(true);
                            page.AddButton.Click();
                        })
                    .WaitModal<WorkerEditorModal>();

            workerEditorModal.Do(modal =>
                {
                    modal.Position.Click();
                    var menuItems = modal.Position.GetMenuItemList<Label>();
                    menuItems.Count.Wait().EqualTo(1);
                    menuItems.First().Text.Wait().EqualTo("Vasily");
                });
        }

        [Test, Description("Популярные должности идут первыми")]
        public async Task SearchPositionsByPopularity()
        {
            await workerRepository.CreateManyAsync(shop.Id, new[]
                {
                    new Worker {FullName = "Сотрудник 1", Position = "Должность 2"},
                    new Worker {FullName = "Сотрудник 2", Position = "Должность 1"},
                    new Worker {FullName = "Сотрудник 3", Position = "Должность 2"},
                });

            var workerListPage =
                LoadMainPage()
                    .Do(page => page.NavigationLayout.WorkersLink.Click())
                    .GoToPage<WorkerListPage>();

            var workerEditorModal =
                workerListPage
                    .Do(page =>
                        {
                            page.AddButton.Click();
                        })
                    .WaitModal<WorkerEditorModal>();

            workerEditorModal.Do(modal =>
                {
                    modal.Position.Click();
                    var menuItems = modal.Position.GetMenuItemList<Label>();

                    menuItems.Select(x => x.Text.Get()).Wait().EquivalentTo(new[] {"Должность 2", "Должность 1"});
                });
        }

        [Injected]
        private readonly IWorkerRepository workerRepository;
    }
}