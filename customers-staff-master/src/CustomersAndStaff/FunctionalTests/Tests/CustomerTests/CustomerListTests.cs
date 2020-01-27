using System.Linq;
using System.Threading.Tasks;

using AutoFixture;

using FluentAssertions.Extensions;

using GroboContainer.NUnitExtensions;

using Market.CustomersAndStaff.FunctionalTests.Components.Pages.Customers;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.Models.Customers;
using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Tests.Core.Configuration;

using MoreLinq;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests.Tests.CustomerTests
{
    [WithCustomizedFixture]
    public class CustomerListTests : TestBase
    {
        [Test, Description("Пустой список, создаем нового клиента")]
        public void CreateNewCustomerTest()
        {
            var customerListPage =
                LoadMainPage()
                    .Do(page => page.NavigationLayout.CustomersLink.Click())
                    .GoToPage<CustomerListPage>();

            var customerEditorModal =
                customerListPage
                    .Do(page =>
                        {
                            page.EmptyPersonList.IsPresent.Wait().EqualTo(true);
                            page.EmptyPersonList.EmptyPersonListAddButton.IsPresent.Wait().EqualTo(true);
                            page.EmptyPersonList.EmptyPersonListAddButton.Click();
                        })
                    .WaitModal<CustomerEditorModal>();

            customerEditorModal
                .Do(modal =>
                    {
                        modal.Name.SetRawValue("Вася");
                        modal.CustomId.SetRawValue("Vasya");
                        modal.Phone.SetValue("79112223344");
                        modal.Discount.SetValue(20.28m);
                        modal.AcceptButton.Click();
                    });

            customerListPage.WaitModalClose<CustomerEditorModal>();

            customerListPage
                .Do(page =>
                    {
                        page.EmptyPersonList.IsPresent.Wait().EqualTo(false);
                        page.CustomerItem.Select(x => x.ToActualData()).Wait().ShouldBeEquivalentTo(new[]
                            {
                                new Customer {Name = "Вася", CustomId = "Vasya", Phone = "79112223344", Discount = 20.28m},
                            });
                    });
        }

        [Test, Description("В базе есть один клиент, добавляем еще одного")]
        public async Task CreateAnotherCustomer()
        {
            await customerRepository.CreateAsync(shop.OrganizationId, new Customer {Name = "Клиент один", CustomId = "111", Phone = "79112223344", Discount = 3.14m});

            var customerListPage =
                LoadMainPage()
                    .Do(page => page.NavigationLayout.CustomersLink.Click())
                    .GoToPage<CustomerListPage>();

            var customerEditorModal =
                customerListPage
                    .Do(page =>
                        {
                            page.EmptyPersonList.IsPresent.Wait().EqualTo(false);
                            page.CustomerItem.Select(x => x.ToActualData()).Wait().ShouldBeEquivalentTo(new[]
                                {
                                    new Customer {Name = "Клиент один", CustomId = "111", Phone = "79112223344", Discount = 3.14m},
                                });
                            page.AddButton.Click();
                        })
                    .WaitModal<CustomerEditorModal>();

            customerEditorModal
                .Do(modal =>
                    {
                        modal.Name.SetRawValue("Клиент два");
                        modal.CustomId.SetRawValue("222");
                        modal.Phone.SetValue("79443332211");
                        modal.Discount.SetValue(22);
                        modal.AcceptButton.Click();
                    });

            customerListPage.WaitModalClose<CustomerEditorModal>();

            customerListPage
                .Do(page =>
                    {
                        page.CustomerItem.Select(x => x.ToActualData()).Wait().ShouldBeEquivalentTo(new[]
                            {
                                new Customer {Name = "Клиент один", CustomId = "111", Phone = "79112223344", Discount = 3.14m},
                                new Customer {Name = "Клиент два", CustomId = "222", Phone = "79443332211", Discount = 22m},
                            });
                    });
        }

        [Test, Description("Создаем список из 51 клиента, проверяем, что появляется пейджинг")]
        public async Task PagingTest()
        {
            var customers = fixture.CreateMany<Customer>(51).ToArray();
            customers.ForEach(x => x.IsDeleted = false);
            await customerRepository.CreateManyAsync(shop.OrganizationId, customers);

            var customerListPage =
                LoadMainPage()
                    .Do(page => page.NavigationLayout.CustomersLink.Click())
                    .GoToPage<CustomerListPage>();

            customerListPage
                .Do(page =>
                    {
                        page.Paging.IsPresent.Wait().EqualTo(true);
                        page.Paging.PagesCount.Wait().EqualTo(2);

                        page.CustomerItem.Count.Wait().EqualTo(50);
                        page.Paging.LinkTo(2).Click();
                        page.CustomerItem.Count.Wait().EqualTo(1);
                    });
        }

        [Test, Description("Отображение полей, комбинированные")]
        public async Task FieldsView()
        {
            var customers = new[]
                {
                    new Customer{Name = "Клиент один", CustomId = "111", Phone = "79091112233", Discount = 5m},
                    new Customer{Name = "Клиент два", CustomId = null, Phone = null, Discount = null},
                    new Customer{Name = null, CustomId = "222222", Phone = null, Discount = 10m},
                    new Customer{Name = null, CustomId = null, Phone = "79092223344", Discount = 15m},
                };

            await customerRepository.CreateManyAsync(shop.OrganizationId, customers);

            var customerListPage =
                LoadMainPage()
                    .Do(page => page.NavigationLayout.CustomersLink.Click())
                    .GoToPage<CustomerListPage>();

            customerListPage
                .Do(page =>
                    {
                        page.CustomerItem.Select(x => x.ToActualData()).Wait().ShouldBeEquivalentTo(customers, opt =>
                            {
                                return opt.Including(x => x.Name)
                                          .Including(x => x.CustomId)
                                          .Including(x => x.Phone)
                                          .Including(x => x.Discount);
                            });
                    });
        }

        [Test, Description("Проверяем, что если у пользователей нет поля карты, то столбец не отображается. Если хотя бы у одного есть, то показываем")]
        public async Task CardFieldInList()
        {
            var customers = new[]
                {
                    new Customer{Name = "Клиент один", CustomId = null, Phone = "79091112233", Discount = 5m},
                    new Customer{Name = "Клиент два", CustomId = null, Phone = "79092223344", Discount = 10m},
                };

            customers = await customerRepository.CreateManyAsync(shop.OrganizationId, customers);

            var customerListPage =
                LoadMainPage()
                    .Do(page => page.NavigationLayout.CustomersLink.Click())
                    .GoToPage<CustomerListPage>();

            customerListPage.Do(page =>
                {
                    page.CustomerItem.Count.Wait().EqualTo(2);
                    page.CustomerItem.First().CustomId.IsPresent.Wait().EqualTo(false);
                });

            await customerRepository.UpdateAsync(shop.OrganizationId, customers[0].Id, c => c.CustomId = "111");

            customerListPage.Do(page => page.CustomerItem.First().CustomId.IsPresent.Wait().EqualTo(true, 20.Seconds()));
        }

        [Test, Description("Сортировка по имени, при равенстве маленька буква идёт первой")]
        public async Task SortingTest()
        {
            var customers = new[]
                {
                    new Customer {Name = "а клиент"},
                    new Customer {Name = "А клиент"},
                    new Customer {Name = "е клиент"},
                    new Customer {Name = "Е клиент"},
                    new Customer {Name = "ё клиент"}, // ё - особенная буква
                    new Customer {Name = "Ё клиент"},
                    new Customer {Name = "м клиент"},
                    new Customer {Name = "Я клиент"},
                    new Customer {Phone = "79112223344"},
                };

            await customerRepository.CreateManyAsync(shop.OrganizationId, customers);

            var customerListPage =
                LoadMainPage()
                    .Do(page => page.NavigationLayout.CustomersLink.Click())
                    .GoToPage<CustomerListPage>();

            customerListPage
                .Do(page =>
                    {
                        page.CustomerItem.Select(x => x.ToActualData()).Wait().ShouldBeEquivalentTo(customers, opt =>
                            {
                                return opt.Including(x => x.Name)
                                          .Including(x => x.Phone)
                                          .WithStrictOrdering();
                            });
                    });
        }

        [Test, Description("Поиск")]
        public async Task SearchTest()
        {
            var customers = new[]
                {
                    new Customer {Name = "Клиент", CustomId = "111", Phone = "79092223344"},
                    new Customer {Name = "Чувак", CustomId = "222222", Phone = "79091112233"},
                    new Customer {Name = "Левый", CustomId = "333", Phone = "79121115577"},
                };

            await customerRepository.CreateManyAsync(shop.OrganizationId, customers);

            var customerListPage =
                LoadMainPage()
                    .Do(page => page.NavigationLayout.CustomersLink.Click())
                    .GoToPage<CustomerListPage>();

            customerListPage
                .Do(page =>
                    {
                        page.SearchInput.SetRawValue("Клиент");
                        page.CustomerItem.Select(x => x.ToActualData()).Wait().ShouldBeEquivalentTo(new[]
                            {
                                new Customer {Name = "Клиент", CustomId = "111", Phone = "79092223344"},
                            });
                        page.SearchInput.Clear();
                    });

            customerListPage
                .Do(page =>
                    {
                        page.SearchInput.SetRawValue("222222");
                        page.CustomerItem.Select(x => x.ToActualData()).Wait().ShouldBeEquivalentTo(new[]
                            {
                                new Customer {Name = "Чувак", CustomId = "222222", Phone = "79091112233"},
                            });
                        page.SearchInput.Clear();
                    });

            customerListPage
                .Do(page =>
                    {
                        page.SearchInput.SetRawValue("+ 7 909222");
                        page.CustomerItem.Select(x => x.ToActualData()).Wait().ShouldBeEquivalentTo(new[]
                            {
                                new Customer {Name = "Клиент", CustomId = "111", Phone = "79092223344"},
                            });
                        page.SearchInput.Clear();
                    });

            customerListPage
                .Do(page =>
                    {
                        page.SearchInput.SetRawValue("8909111");
                        page.CustomerItem.Select(x => x.ToActualData()).Wait().ShouldBeEquivalentTo(new[]
                            {
                                new Customer {Name = "Чувак", CustomId = "222222", Phone = "79091112233"},
                            });
                        page.SearchInput.Clear();
                    });

            customerListPage
                .Do(page =>
                    {
                        page.SearchInput.SetRawValue("222");
                        page.CustomerItem.Select(x => x.ToActualData()).Wait().ShouldBeEquivalentTo(new[]
                            {
                                new Customer {Name = "Клиент", CustomId = "111", Phone = "79092223344"},
                                new Customer {Name = "Чувак", CustomId = "222222", Phone = "79091112233"},
                            });
                        page.SearchInput.Clear();
                    });

            customerListPage
                .Do(page =>
                    {
                        page.SearchInput.SetRawValue("Нечто");
                        page.CustomerItem.Count.Wait().EqualTo(0);
                        page.SearchInput.Clear();
                    });
        }

        [Injected]
        private readonly ICustomerRepository customerRepository;

        [Injected]
        private readonly IFixture fixture;
    }
}