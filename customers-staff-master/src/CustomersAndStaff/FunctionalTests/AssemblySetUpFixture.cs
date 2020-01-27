using System.IO;

using Kontur.Selone.Extensions;
using Kontur.Selone.WebDrivers;

using Market.CustomersAndStaff.FunctionalTests.Infrastructure;

using NUnit.Framework;

namespace Market.CustomersAndStaff.FunctionalTests
{
    [SetUpFixture]
    public class AssemblySetUpFixture
    {
        public static WebDriverPool WebDriverPool { get; private set; }

        [OneTimeSetUp]
        public void SetUp()
        {
            if (Directory.Exists(ChromeDriverFactory.DownloadPath))
            {
                Directory.Delete(ChromeDriverFactory.DownloadPath, true);
            }
            var factory = new ChromeDriverFactory();
            var cleaner = new DelegateWebDriverCleaner(x =>
                {
                    x.ResetWindows();
                });
            WebDriverPool = new WebDriverPool(factory, cleaner);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            WebDriverPool.Clear();
            if (Directory.Exists(ChromeDriverFactory.DownloadPath))
            {
                Directory.Delete(ChromeDriverFactory.DownloadPath, true);
            }
        }
    }
}