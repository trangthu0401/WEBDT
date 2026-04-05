using NUnit.Framework;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using PhoneStore.AutoTests.Utilities;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace PhoneStore.AutoTests.Tests
{
    [TestFixture]
    public class ProductHideTests : BaseTest
    {
        private LoginPage loginPage;
        private ProductHidePage hidePage;
        private WebDriverWait wait;

        [SetUp]
        public void PreTest()
        {
            loginPage = new LoginPage(driver);
            hidePage = new ProductHidePage(driver);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            // BƯỚC 1: Đăng nhập Admin để thực hiện Ẩn
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Account/Login");
            loginPage.Login("admin@shop.com", "admin123", isAdmin: true);
        }

        [Test]
        public void TC_PRODUCT_08_HideVariantAndCheckOnUserPage()
        {
            var data = JsonReader.GetTestRow("ProductHideData.json", "TC_PRODUCT_08");
            string colorToHide = (string)data.TargetColor;

            // BƯỚC 2: Vào Admin ẩn biến thể
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
            hidePage.GoToFirstProductVariant();
            hidePage.ClickHideVariant(colorToHide);

            System.Threading.Thread.Sleep(2000); // Đợi hệ thống cập nhật DB

            // BƯỚC 3: Sang trang User kiểm tra
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + (string)data.ProductDetailUrl);
            System.Threading.Thread.Sleep(2000);

            // Kiểm tra: Hy vọng là KHÔNG tìm thấy (IsVisible == false)
            bool isStillThere = hidePage.IsVariantVisibleOnUserPage(colorToHide);

            Assert.That(isStillThere, Is.False, $"Lỗi: Biến thể màu '{colorToHide}' vẫn còn hiện ở trang User sau khi đã Ẩn!");
        }
    }
}