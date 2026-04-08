using NUnit.Framework;
using OpenQA.Selenium;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using PhoneStore.AutoTests.Utilities;
using System.Threading;

namespace PhoneStore.AutoTests.Tests
{
    [TestFixture]
    public class HomeTests : BaseTest
    {
        private HomePage homePage;

        [SetUp]
        public void Init()
        {
            homePage = new HomePage(driver);

            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl);
            Thread.Sleep(1000);
        }

        // ===== TC40: LOAD TRANG CHỦ =====
        [Test]
        public void TC40_LoadHome()
        {
            Assert.IsTrue(driver.PageSource.Contains("Tài khoản"));
        }

        // ===== TC41: CLICK MENU TÀI KHOẢN =====
        [Test]
        public void TC41_ClickMenuTaiKhoan()
        {
            homePage.ClickMenuTaiKhoan();
            Thread.Sleep(1000);

            Assert.IsTrue(driver.PageSource.Contains("Đăng nhập"));
        }

        // ===== TC42: CLICK ĐĂNG NHẬP =====
        [Test]
        public void TC42_ClickDangNhap()
        {
            homePage.ClickMenuTaiKhoan();
            Thread.Sleep(500);

            homePage.ClickDangNhap();
            Thread.Sleep(1000);

            Assert.IsTrue(driver.Url.Contains("Login"));
        }

        // ===== TC43: CLICK ĐĂNG KÝ =====
        [Test]
        public void TC43_ClickDangKy()
        {
            homePage.ClickMenuTaiKhoan();
            Thread.Sleep(500);

            homePage.ClickDangKy();
            Thread.Sleep(1000);

            Assert.IsTrue(driver.Url.Contains("Register"));
        }

        // ===== TC44: CLICK GIỎ HÀNG =====
        [Test]
        public void TC44_ClickGioHang()
        {
            homePage.ClickGioHang();
            Thread.Sleep(1000);

            Assert.IsTrue(driver.PageSource.Contains("Giỏ hàng"));
        }

        // ===== TC45: CLICK SẢN PHẨM =====
        [Test]
        public void TC45_ClickProduct()
        {
            homePage.ClickFirstProduct();
            Thread.Sleep(2000);

            Assert.IsTrue(
                driver.Url.Contains("Product") ||
                driver.PageSource.Contains("Chi tiết")
            );
        }

        // ===== TC46: REFRESH =====
        [Test]
        public void TC46_Refresh()
        {
            driver.Navigate().Refresh();
            Thread.Sleep(1000);

            Assert.IsTrue(driver.PageSource.Contains("Tài khoản"));
        }

        // ===== TC47: BACK =====
        [Test]
        public void TC47_Back()
        {
            homePage.ClickFirstProduct();
            Thread.Sleep(1500);

            driver.Navigate().Back();
            Thread.Sleep(1000);

            Assert.IsTrue(driver.PageSource.Contains("Tài khoản"));
        }

        // ===== TC48: SCROLL =====
        [Test]
        public void TC48_Scroll()
        {
            ((OpenQA.Selenium.IJavaScriptExecutor)driver)
                .ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");

            Thread.Sleep(1000);

            Assert.IsTrue(true);
        }

        // ===== TC49: LOAD PRODUCT IMAGE =====
        [Test]
        public void TC49_ProductImage()
        {
            Assert.IsTrue(
                driver.FindElements(By.CssSelector("img")).Count > 0
            );
        }

        // ===== TC50: MULTI CLICK =====
        [Test]
        public void TC50_ClickMultiple()
        {
            homePage.ClickMenuTaiKhoan();
            homePage.ClickMenuTaiKhoan();
            homePage.ClickMenuTaiKhoan();

            Thread.Sleep(1000);

            Assert.IsTrue(true);
        }
    }
}