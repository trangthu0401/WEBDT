using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using PhoneStore.AutoTests.Utilities;
using System;
using System.IO;

namespace PhoneStore.AutoTests.Core
{
    public class BaseTest
    {
        protected IWebDriver driver;

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            // Tên file Excel thiết kế test case của bạn (để ở thư mục gốc Project)
            ExcelHelper.InitExistingExcel("ST-FunctionalTestCase-BDCLPM.xlsx");
        }

        [OneTimeTearDown]
        public void GlobalTearDown()
        {
            // Bắt buộc phải có hàm này thì Excel mới được lưu
            ExcelHelper.SaveAndClose();
        }

        [SetUp]
        public void Setup()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddUserProfilePreference("credentials_enable_service", false);
            options.AddUserProfilePreference("profile.password_manager_enabled", false);
            options.AddUserProfilePreference("profile.password_manager_leak_detection", false);
            options.AddUserProfilePreference("safebrowsing.enabled", false);
            options.AddArgument("--disable-notifications");
            options.AddExcludedArgument("enable-automation");

            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();

            // Giới hạn thời gian tránh treo web
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);
        }

        [TearDown]
        public void Teardown()
        {
            var status = TestContext.CurrentContext.Result.Outcome.Status;
            var error = TestContext.CurrentContext.Result.Message ?? "";

            // Lấy ra TC_ID để truyền qua hàm chụp ảnh và hàm ghi Excel
            var tcId = TestContext.CurrentContext.Test.Properties.Get("TC_ID")?.ToString();
            var rawTestName = TestContext.CurrentContext.Test.Name;

            try
            {
                if (status == TestStatus.Failed)
                {
                    // Truyền tcId vào để cắt ngắn tên file ảnh
                    ScreenshotHelper.TakeScreenshot(driver, rawTestName, tcId);
                }

                if (!string.IsNullOrEmpty(tcId))
                {
                    string statusStr = (status == TestStatus.Passed) ? "Passed" : "Failed";
                    ExcelHelper.UpdateTestResult(tcId, statusStr, error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi Teardown: {ex.Message}");
            }
            finally
            {
                if (driver != null)
                {
                    driver.Quit();
                    driver.Dispose();
                }
            }
        }
    }
}