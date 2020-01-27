using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework;

using OpenQA.Selenium;

namespace Market.CustomersAndStaff.FunctionalTests.Infrastructure
{
    public static class ScreenshotSaverExtensions
    {
        public static void SaveImage(this Stream image, string name)
        {
            var now = DateTime.Now;

            var fileName = $"{workDirPath}\\{now:HH-mm-ss}_{EscapePath(TestContext.CurrentContext.Test.FullName, 128)}_{name}.png";
            SaveImageBase(fileName, x =>
                {
                    using(var fileStream = File.Create(x))
                    {
                        image.CopyTo(fileStream);
                    }
                });
        }

        public static void SaveScreenshot(this IWebDriver webDriver)
        {
            var now = DateTime.Now;

            var fileName = $"{workDirPath}\\{now:HH-mm-ss}_{EscapePath(TestContext.CurrentContext.Test.FullName, 128)}.png";
            SaveImageBase(fileName, x => ((ITakesScreenshot)webDriver).GetScreenshot().SaveAsFile(x));
        }

        private static void SaveImageBase(string fileName, Action<string> saveAction)
        {
            try
            {
                CreateWorkDirectoryIfNotExists();
                saveAction(fileName);
                Console.WriteLine($"##teamcity[publishArtifacts '{fileName} => .screenshots']");
                Console.WriteLine("Screenshot saved to:\r\n{0}", ConvertToFileUri(fileName));
            }
            catch(Exception exception)
            {
                Console.WriteLine("Cannot save screenshot to '{0}'", fileName);
                Console.WriteLine(exception);
            }
        }

        private static string EscapePath(string testName, int maxLength)
        {
            var testNameLocal = GetLocalTestName(testName);
            var resultLength = Math.Min(maxLength, testNameLocal.Length);
            var result = new StringBuilder(resultLength);

            foreach(var symbol in testNameLocal.Take(resultLength))
            {
                var value = !IsUrlSafeChar(symbol) || invalidPathChars.Contains(symbol) ? '_' : symbol;
                result.Append(value);
            }

            return result.ToString();
        }

        private static string GetLocalTestName(string testName)
        {
            return string.Join(".", testName.Split('.').Reverse().Take(2).Reverse());
        }

        private static bool IsUrlSafeChar(char ch)
        {
            if((ch >= 97 && ch <= 122) || (ch >= 65 && ch <= 90) || (ch >= 48 && ch <= 57) || ch == 33)
                return true;
            switch(ch)
            {
            case '(':
            case ')':
            case '*':
            case '-':
            case '.':
            case '_':
                return true;
            default:
                return false;
            }
        }

        private static string ConvertToFileUri(string localPath)
        {
            return new Uri(localPath).AbsoluteUri;
        }
        private static void CreateWorkDirectoryIfNotExists()
        {
            if (!Directory.Exists(workDirPath))
            {
                Directory.CreateDirectory(workDirPath);
            }
        }

        private static readonly HashSet<char> invalidPathChars = new HashSet<char>(Path.GetInvalidPathChars());
        private static readonly string workDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData");
    }
}