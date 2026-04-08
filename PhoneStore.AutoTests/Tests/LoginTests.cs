using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using PhoneStore.AutoTests.Utilities;
using SeleniumExtras.WaitHelpers;
using System.Collections.Generic;

namespace PhoneStore.AutoTests.Tests
{
    [TestFixture]
    public class LoginTests : BaseTest
    {
        private HomePage homePage;
        private LoginPage loginPage;
        private WebDriverWait wait;

        [SetUp]
        public void Init()
        {
            homePage = new HomePage(driver);
            loginPage = new LoginPage(driver);

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            // 👉 mở web
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl);

            // 👉 mở form login (KHÔNG dùng sleep nữa)
            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//span[contains(text(),'Tài khoản')]"))).Click();
            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[contains(text(),'Đăng nhập')]"))).Click();
        }

        // 👉 đọc JSON
        public static IEnumerable<TestCaseData> GetLoginData()
        {
            return JsonReader.GetTestData("login.json");
        }

        // 👉 TEST CHÍNH
        [Test, TestCaseSource(nameof(GetLoginData))]
        public void Auto_Login_Test(JObject data)
        {
            string email = data["Email"]?.ToString();
            string password = data["Password"]?.ToString();
            string expected = data["Expected"]?.ToString();

            // 👉 WAIT input hiện ra rồi mới nhập
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("EmailOrPhone")));

            // 👉 ACTION
            loginPage.Login(email, password);

            // 👉 WAIT sau khi login
            System.Threading.Thread.Sleep(800);

            // 👉 ASSERT
            if (expected == "success")
            {
                Assert.IsTrue(
                    driver.PageSource.Contains("Trang chủ") ||
                    driver.PageSource.Contains("Xin chào") ||
                    driver.PageSource.Contains("Admin"),
                    "❌ Expected SUCCESS nhưng FAIL"
                );
            }
            else
            {
                Assert.IsTrue(
                    driver.PageSource.Contains("không đúng") ||
                    driver.PageSource.Contains("lỗi") ||
                    driver.PageSource.Contains("bắt buộc"),
                    "❌ Expected FAIL nhưng PASS"
                );
            }
        }
    }
}