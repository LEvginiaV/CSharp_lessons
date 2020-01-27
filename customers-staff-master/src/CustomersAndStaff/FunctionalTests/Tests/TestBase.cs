using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

using GroboContainer.NUnitExtensions;

using Kontur.Market.FakeMarkerApi.ConfigurationClient;

using Market.Api.Client;
using Market.Api.Models.ResourceGroups;
using Market.Api.Models.Shops;
using Market.CustomersAndStaff.FunctionalTests.Components.Pages;
using Market.CustomersAndStaff.FunctionalTests.Configuration;
using Market.CustomersAndStaff.FunctionalTests.Helpers;
using Market.CustomersAndStaff.FunctionalTests.Infrastructure;
using Market.CustomersAndStaff.FunctionalTests.PageExtensions;
using Market.CustomersAndStaff.Models.Users;
using Market.CustomersAndStaff.Portal.Core;
using Market.CustomersAndStaff.Repositories.Interface;
using Market.CustomersAndStaff.Tests.Core.Configuration;

using NUnit.Framework;
using NUnit.Framework.Interfaces;

using OpenQA.Selenium;

using Portal.Auth;
using Portal.Requisites;

namespace Market.CustomersAndStaff.FunctionalTests.Tests
{
    [WithPortal, WithMarketApiClients]
    public abstract class TestBase : IMainSuite
    {
        private string TestWorkerId => TestContext.CurrentContext.WorkerId;

        [GroboSetUp]
        public virtual void SetUp()
        {
            shop = CreateShop().Result;
            ContextHelper.SetCurrentShop(shop);
            (userSid, userId) = CreateUserForOrganization(shop.OrganizationId).Result;
            userSettingsRepository.UpdateAsync(userId, UserSettingsKeys.HideWorkOrderOnBoard, "true").Wait();
            userSettingsRepository.UpdateAsync(userId, UserSettingsKeys.HideCalendarOnBoard, "true").Wait();
            userSettingsRepository.UpdateAsync(userId, UserSettingsKeys.HideWorkersOnBoard, "true").Wait();
            userSettingsRepository.UpdateAsync(userId, UserSettingsKeys.HideCustomersOnBoard, "true").Wait();
            Console.WriteLine($"Login link: {url}?shopId={shop.Id}&authSid={userSid}");
        }

        [GroboTearDown]
        public virtual void TearDown()
        {
            try
            {
                ContextHelper.Clear();
                if(TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
                {
                    WebDriver.SaveScreenshot();
                }
            }
            finally
            {
                if(acquiredWebDrivers.TryRemove(TestWorkerId, out var webDriver))
                {
                    webDriver.Manage().Cookies.DeleteAllCookies();
                    AssemblySetUpFixture.WebDriverPool.Release(webDriver);
                }
            }
        }

        protected IWebDriver WebDriver => acquiredWebDrivers.GetOrAdd(TestWorkerId, x => AssemblySetUpFixture.WebDriverPool.Acquire());

        protected CustomersAndStaffMainPage LoadMainPage(Shop newShop = null)
        {
            if(newShop != null)
            {
                WebDriver.Manage().Cookies.DeleteAllCookies();
            }

            var authPage = WebDriver.GoToUrl<AuthPage>(url);

            WebDriver.Manage().Cookies.AddCookie(new Cookie("testingMode", "1"));

            var mainPage = authPage.Do(page =>
                        {
                            page.IsPresent.Wait().EqualTo(true);
                            page.AuthSidInput.SetRawValue(userSid);
                            page.ShopIdInput.SetRawValue((newShop?.Id ?? shop.Id).ToString());
                            page.SendButton.Click();
                            page.IsPresent.Wait().EqualTo(false);
                        })
                    .GoToPage<CustomersAndStaffMainPage>();

            return mainPage;
        }

        protected async Task<Shop> CreateShop(Guid? organizationId = null, Guid? departmentId = null)
        {
            var newShop = new Shop
                {
                    OrganizationId = organizationId ?? Guid.NewGuid(),
                    DepartmentId = departmentId ?? Guid.NewGuid(),
                    Id = Guid.NewGuid(),
                    Address = "a",
                    Name = "B",
                };

            await fakeMarketApiClient.SetShop(newShop);
            Console.WriteLine($"New ShopId: {newShop.Id}");

            return newShop;
        }

        protected void RefreshPage()
        {
            WebDriver.Navigate().Refresh();
        }

        private async Task<(string UserSid, Guid UserId)> CreateUserForOrganization(Guid organizationId, ResourceGroups resourceGroups = ResourceGroups.DeveloperResourse)
        {
            var login = Guid.NewGuid().ToString().Replace("-", "").Substring(16);
            var password = "psw";

            var properties = new Dictionary<string, string>
                {
                    {BaseRequisites.Login, login},
                    {BaseRequisites.FirstName, "Vasya"},
                    {BaseRequisites.LastName, "Petukhov"},
                    {"scope/egais/partyids", organizationId.ToString()}
                };

            var masterResult = await authClient.AuthByPassAsync2(new AuthByPassRequest(masterPortalSettings.MasterLogin, masterPortalSettings.MasterPassword));
            masterResult.EnsureSuccess();
            var masterSid = masterResult.Response.Sid;

            var localUserId = Guid.NewGuid();
            var result = await requisitesClient.Users.CreateAsync(new User(localUserId, new Requisites(properties)), sessionId : masterSid);
            result.EnsureSuccess();

            var result2 = await authClient.SetPassword(localUserId, password, masterSid);
            result2.EnsureSuccess();

            var loginResult = await authClient.AuthByPassAsync2(new AuthByPassRequest(login, password));
            var sid = loginResult.Response.Sid;

            await fakeMarketApiClient.SetUserGroup(localUserId.ToString(), resourceGroups);
            Console.WriteLine($"User sid: {sid}");

            return (sid, localUserId);
        }

        [Injected]
        private IRequisitesClient requisitesClient;

        [Injected]
        private IAuthClient authClient;

        [Injected]
        protected IFakeMarketApiClient fakeMarketApiClient;

        [Injected]
        protected IMarketApiClient marketApiClient;

        [Injected]
        private IMasterPortalSettings masterPortalSettings;

        [Injected]
        private IUserSettingsRepository userSettingsRepository;

        protected Shop shop;

        private Guid userId;
        private string userSid;
        private static readonly ConcurrentDictionary<string, IWebDriver> acquiredWebDrivers = new ConcurrentDictionary<string, IWebDriver>();
        private const string url = "http://localhost:3000";
    }
}