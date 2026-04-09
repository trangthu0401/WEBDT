using NUnit.Framework;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using PhoneStore.AutoTests.Utilities;
using System;

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
            // Vy có thể ghi 2 tham số (vì hàm tự nhận diện email admin)
            // Hoặc ghi 3 tham số: isAdmin: true đều được
            loginPage.Login("admin@shop.com", "admin123", isAdmin: true);

            System.Threading.Thread.Sleep(2000);
            Assert.That(driver.Url.Contains("Dashboard") || driver.PageSource.Contains("Tổng quan"), Is.True,
                "Lỗi: Đăng nhập Admin thành công nhưng không thấy trang Dashboard!");
        }

        [Test]
        public void TC_LOGIN_02_CustomerLoginSuccess()
        {
            // Sử dụng tài khoản khách từ ConfigHelper của Vy (nguyentrathanhvy2005)
            loginPage.Login(ConfigHelper.TestUserPhone, ConfigHelper.TestUserPassword);

            System.Threading.Thread.Sleep(2000);
            // Khách thường thì URL không được chứa Dashboard
            Assert.That(driver.Url.Contains("Dashboard"), Is.False,
                "Lỗi: Khách thường lại vào được trang Admin!");

            Assert.That(driver.PageSource.Contains("Chào") || driver.PageSource.Contains("Thoát"), Is.True,
                "Lỗi: Không thấy trạng thái đã đăng nhập của khách!");
        }
    }
}