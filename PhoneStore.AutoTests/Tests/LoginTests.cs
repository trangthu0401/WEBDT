using NUnit.Framework;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using PhoneStore.AutoTests.Utilities; // Để dùng được ConfigHelper

namespace PhoneStore.AutoTests.Tests
{
    [TestFixture]
    public class LoginTests : BaseTest
    {
        private LoginPage loginPage;

        [SetUp]
        public void Init()
        {
            loginPage = new LoginPage(driver);
            // Robot đi đến trang Login trước mỗi test case
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Account/Login");
        }

        [Test]
        public void TC_LOGIN_01_AdminLoginSuccess()
        {
            loginPage.Login("admin@shop.com", "admin123", isAdmin: true);

            // Thay Assert.IsTrue bằng Assert.That
            Assert.That(driver.Url.Contains("Dashboard"), Is.True, "Lỗi: Không thấy trang Dashboard!");
        }

        [Test]
        public void TC_LOGIN_02_CustomerLoginSuccess()
        {
            loginPage.Login(ConfigHelper.TestUserPhone, ConfigHelper.TestUserPassword, isAdmin: false);

            // Thay Assert.IsFalse bằng Assert.That(..., Is.False)
            Assert.That(driver.Url.Contains("Dashboard"), Is.False, "Lỗi: Khách thường lại vào được trang Admin!");
        }
    }
}