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
        public void TC_PRODUCT_08_HideFirstVariantAndCheck()
        {
            var data = JsonReader.GetTestRow("ProductHideData.json", "TC_PRODUCT_08");

            // BƯỚC 1: Vào Admin
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
            hidePage.GoToFirstProductVariant();

            // BƯỚC 2: Robot tự lấy tên màu ở dòng 1 để tí nữa đối chiếu
            string colorToHide = hidePage.GetFirstVariantColorName();
            Console.WriteLine($"Robot chọn ẩn màu: {colorToHide}");

            // BƯỚC 3: Bấm ẩn dòng đầu tiên
            hidePage.ClickHideFirstVariant();
            System.Threading.Thread.Sleep(2000);

            // BƯỚC 4: Sang trang User kiểm tra (Dùng URL từ JSON)
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + (string)data.ProductDetailUrl);

            // Kiểm tra: Hy vọng là KHÔNG tìm thấy màu đó nữa
            bool isStillThere = hidePage.IsVariantVisibleOnUserPage(colorToHide);

            Assert.That(isStillThere, Is.False, $"Lỗi: Biến thể '{colorToHide}' vẫn còn hiện ở trang User sau khi đã Ẩn!");
        }
    }
}