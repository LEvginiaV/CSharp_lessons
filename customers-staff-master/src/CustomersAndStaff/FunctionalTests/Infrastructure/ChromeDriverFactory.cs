using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

using Kontur.Selone.Extensions;
using Kontur.Selone.WebDrivers;

using Microsoft.Win32;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;

namespace Market.CustomersAndStaff.FunctionalTests.Infrastructure
{
    public class ChromeDriverFactory : IWebDriverFactory
    {
        public static string DownloadPath => downloadPath;

        public IWebDriver Create()
        {
            SetChromeVersionToRegistry(chromeExePath);
            for(var i = 0; i < 3; i++)
            {
                try
                {
                    var chromeDriverService = ChromeDriverService.CreateDefaultService(chromeDriverPath);
                    var options = CreateChromeOptions(chromeExePath);
                    var result = CreateChromeDriver(chromeDriverService, options);
                    ProcessExtensions.MinimizeMainWindow(() => chromeDriverService.ProcessId, TimeSpan.FromSeconds(15));
                    return result;
                }
                catch(InvalidOperationException e) when(e.Message.Contains("session not created exception"))
                {
                }
            }

            return null;
        }

        private static void SetChromeVersionToRegistry(string chromeExe)
        {
            if(chromeExe == null)
            {
                return;
            }

            var key = Registry.CurrentUser.CreateSubKey(registryKey);
            if(key == null)
            {
                throw new Exception($"Не удалось создать ключ {Registry.CurrentUser}\\{registryKey}");
            }

            key.SetValue("pv", FileVersionInfo.GetVersionInfo(chromeExe).ProductVersion);
            key.Close();
        }

        private static ChromeOptions CreateChromeOptions(string chromeExe)
        {
            var options = new ChromeOptions();
            if(chromeExe != null)
            {
                options.BinaryLocation = chromeExe;
            }

            options.AddArguments("--no-sandbox", "--disable-extensions");
            options.AddUserProfilePreference("profile.default_content_setting_values.automatic_downloads", 1);
            options.AddUserProfilePreference("download.default_directory", downloadPath);
            return options;
        }

        private static IWebDriver CreateChromeDriver(ChromeDriverService chromeDriverService, ChromeOptions options)
        {
            var chromeDriver = new WebDriverCloseOnDisposeDecorator(new ChromeDriver(chromeDriverService, options, TimeSpan.FromSeconds(180)));
            chromeDriver.Manage().Window.SetSize(1144, 768);
            return chromeDriver;
        }

        private const string registryKey = @"Software\Google\Update\Clients\{8A69D345-D564-463c-AFF1-A69D9E530F96}";
        private const string path = @"..\..\..\..\..\alco.global-services\chrome-driver-2.44";

        private static readonly string chromeDriverPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
        private static readonly string chromeExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path, "chrome.exe");
        private static readonly string downloadPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "downloads");

        private class WebDriverCloseOnDisposeDecorator : IWebDriver, IJavaScriptExecutor, ITakesScreenshot, IHasInputDevices, IActionExecutor
        {
            public WebDriverCloseOnDisposeDecorator(IWebDriver inner)
            {
                this.inner = inner;
            }

            public IWebElement FindElement(By @by)
            {
                return inner.FindElement(@by);
            }

            public ReadOnlyCollection<IWebElement> FindElements(By @by)
            {
                return inner.FindElements(@by);
            }

            public void Dispose()
            {
                inner.Close();
                inner.Dispose();
            }

            public void Close()
            {
                inner.Close();
            }

            public void Quit()
            {
                inner.Quit();
            }

            public IOptions Manage()
            {
                return inner.Manage();
            }

            public INavigation Navigate()
            {
                return inner.Navigate();
            }

            public ITargetLocator SwitchTo()
            {
                return inner.SwitchTo();
            }

            public Screenshot GetScreenshot()
            {
                return ((ITakesScreenshot)inner).GetScreenshot();
            }

            public string Url { get => inner.Url; set => inner.Url = value; }
            public string Title => inner.Title;
            public string PageSource => inner.PageSource;
            public string CurrentWindowHandle => inner.CurrentWindowHandle;
            public ReadOnlyCollection<string> WindowHandles => inner.WindowHandles;

            public object ExecuteScript(string script, params object[] args)
            {
                return ((IJavaScriptExecutor)inner).ExecuteScript(script, args);
            }

            public object ExecuteAsyncScript(string script, params object[] args)
            {
                return ((IJavaScriptExecutor)inner).ExecuteAsyncScript(script, args);
            }

            public IKeyboard Keyboard => ((IHasInputDevices)inner).Keyboard;
            public IMouse Mouse => ((IHasInputDevices)inner).Mouse;

            public void PerformActions(IList<ActionSequence> actionSequenceList)
            {
                ((IActionExecutor)inner).PerformActions(actionSequenceList);
            }

            public void ResetInputState()
            {
                ((IActionExecutor)inner).ResetInputState();
            }

            public bool IsActionExecutor => ((IActionExecutor)inner).IsActionExecutor;

            private readonly IWebDriver inner;
        }
    }
}