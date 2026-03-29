using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using PhoneStore.AutoTests.Utilities;
using System;

namespace PhoneStore.AutoTests.Core
{
    public class BaseTest
    {
        protected IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            // ================================================================
            // TẠO CẤU HÌNH TRỊ MỌI LOẠI THÔNG BÁO CỦA TRÌNH DUYỆT CHROME
            // ================================================================
            ChromeOptions options = new ChromeOptions();

            // 1. Tắt bảng hỏi "Lưu mật khẩu không?"
            options.AddUserProfilePreference("credentials_enable_service", false);
            options.AddUserProfilePreference("profile.password_manager_enabled", false);

            // 2. TẮT BẢNG CẢNH BÁO "LỘ MẬT KHẨU" (CHANGE YOUR PASSWORD) NÀY NHÉ!
            options.AddUserProfilePreference("profile.password_manager_leak_detection", false);
            options.AddUserProfilePreference("safebrowsing.enabled", false);

            // 3. Tắt luôn các thông báo xin quyền (Vị trí, Notifications...)
            options.AddArgument("--disable-notifications");

            // 4. Ẩn dòng chữ "Chrome is being controlled by automated test software"
            options.AddExcludedArgument("enable-automation");

            // Truyền cấu hình vào "Tài xế" Chrome
            driver = new ChromeDriver(options);

            // Các thiết lập cơ bản khác
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            // Bay vào trang chủ
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl);
        }

        [TearDown]
        public void Teardown()
        {
            string screenshotPath = "";
            var status = TestContext.CurrentContext.Result.Outcome.Status;
            var testName = TestContext.CurrentContext.Test.Name;
            var error = TestContext.CurrentContext.Result.Message ?? "";

            try
            {
                if (status == NUnit.Framework.Interfaces.TestStatus.Failed)
                {
                    // Chụp ảnh và lưu vào folder Screenshots trong Project
                    screenshotPath = ScreenshotHelper.TakeScreenshot(driver, testName);
                }
                // Ghi vào file Excel .xlsx
                ExcelHelper.LogTestResult(testName, status.ToString(), error, screenshotPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong quá trình Teardown: {ex.Message}");
            }
            finally
            {
                // ĐẢM BẢO ĐÓNG TRÌNH DUYỆT 100%
                if (driver != null)
                {
                    driver.Quit();
                    driver.Dispose();
                }
            }
        }
    }
}