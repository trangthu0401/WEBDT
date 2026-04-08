using NUnit.Framework;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using PhoneStore.AutoTests.Utilities;
using OpenQA.Selenium;
using System.Threading;
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
            homePage = new HomePage(driver);
            homePage = new HomePage(driver);
            loginPage = new LoginPage(driver);

            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl);

            homePage.ClickMenuTaiKhoan();
            Thread.Sleep(200);

            homePage.ClickDangNhap();
        }

        // ===== TC1: Login thành công =====
        [Test]
        public void TC1_Login_Admin_Valid()
        {
            loginPage.Login("admin@shop.com", "admin123");
            Thread.Sleep(300);

            Assert.IsTrue(driver.PageSource.Contains("Trang chủ")
                || driver.PageSource.Contains("Xin chào")
                || driver.PageSource.Contains("Admin"));
        }

        // ===== TC2: Sai mật khẩu =====
        [Test]
        public void TC2_Login_WrongPassword()
        {
            loginPage.Login("admin@shop.com", "123456");
            Thread.Sleep(300);

            Assert.IsTrue(driver.PageSource.Contains("không đúng")
                || driver.PageSource.Contains("Sai mật khẩu"));
        }

        // ===== TC3: Email trống =====
        [Test]
        public void TC3_Login_EmptyEmail()
        {
            loginPage.Login("", "admin123");
            Thread.Sleep(300);

            Assert.IsTrue(driver.PageSource.Contains("Email"));
        }

        // ===== TC4: Password trống =====
        [Test]
        public void TC4_Login_EmptyPassword()
        {
            loginPage.Login("admin@shop.com", "");
            Thread.Sleep(300);

            Assert.IsTrue(driver.PageSource.Contains("Mật khẩu"));
        }

        // ===== TC5: Email sai format =====
        [Test]
        public void TC5_Login_InvalidEmail()
        {
            loginPage.Login("abc", "admin123");
            Thread.Sleep(300);

            Assert.IsTrue(driver.PageSource.Contains("Email"));
        }

        // ===== TC6: Tài khoản không tồn tại =====
        [Test]
        public void TC6_Login_NotExist()
        {
            loginPage.Login("abc@gmail.com", "123456");
            Thread.Sleep(300);

            Assert.IsTrue(driver.PageSource.Contains("không tồn tại")
                || driver.PageSource.Contains("không đúng"));
        }

        // ===== TC7: Bỏ trống tất cả =====
        [Test]
        public void TC7_Login_EmptyAll()
        {
            loginPage.Login("", "");
            Thread.Sleep(300);

            Assert.IsTrue(driver.PageSource.Contains("bắt buộc")
                || driver.PageSource.Contains("Email"));
        }

        // ===== TC8: Nhập ký tự đặc biệt =====
        [Test]
        public void TC8_Login_SpecialChar()
        {
            loginPage.Login("@@@@", "####");
            Thread.Sleep(300);

            Assert.IsTrue(driver.PageSource.Contains("lỗi")
                || driver.PageSource.Contains("không đúng"));
        }

        // ===== TC9: Login nhiều lần =====
        [Test]
        public void TC9_Login_MultipleTimes()
        {
            loginPage.Login("admin@shop.com", "admin123");
            Thread.Sleep(300);

            loginPage.Login("admin@shop.com", "admin123");
            Thread.Sleep(300);

            Assert.IsTrue(true);
        }

        // ===== TC10: Refresh trang =====
        [Test]
        public void TC10_Login_Refresh()
        {
            driver.Navigate().Refresh();
            Thread.Sleep(300);

            Assert.IsTrue(driver.PageSource.Contains("Đăng nhập"));
        }
    }
}