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
    public class ProductVariantTests : BaseTest
    {
        private LoginPage loginPage;
        private ProductVariantPage variantPage;
        private WebDriverWait wait;

        [SetUp]
        public void PreTest()
        {
            loginPage = new LoginPage(driver);
            variantPage = new ProductVariantPage(driver);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(25));

            // 1. Đăng nhập Admin
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Account/Login");
            loginPage.Login("admin@shop.com", "admin123", isAdmin: true);
        }

        [Test]
        public void TC_PRODUCT_15_AddVariant_Iphone16e()
        {
            // Đi tới trang danh sách sản phẩm
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product/Index");

            // Lấy dữ liệu biến thể từ JSON
            var data = JsonReader.GetTestRow("ProductVariantData.json", "TC_PRODUCT_15");

            // 1. Vào trang quản lý biến thể của sản phẩm đầu tiên (iPhone 16e)
            variantPage.GoToFirstProductVariant();

            // 2. Mở Modal thêm mới
            variantPage.OpenAddModal();

            // 3. Nhập liệu
            variantPage.InputVariantDetails(data);

            // 4. Lưu lại
            variantPage.Save();

            // 5. Kiểm tra xem có về lại trang danh sách biến thể và thấy màu mới không
            System.Threading.Thread.Sleep(3000);
            Assert.That(driver.PageSource.Contains((string)data.Color), Is.True, "Lỗi: Không tìm thấy màu sắc biến thể mới!");
        }


        [Test]
        public void TC_PRODUCT_15_1_AddVariant_Iphone13prmax()
        {
            
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product/Index");
            var data = JsonReader.GetTestRow("ProductVariantData.json", "TC_PRODUCT_15_1");
            variantPage.GoToFirstProductVariant();
            variantPage.OpenAddModal();
            variantPage.InputVariantDetails(data);
            variantPage.Save();
            System.Threading.Thread.Sleep(3000);
            Assert.That(driver.PageSource.Contains((string)data.Color), Is.True, "Lỗi: Không tìm thấy màu sắc biến thể mới!");
        }

        [Test]
        public void TC_PRODUCT_15_2_AddVariant_Iphone15()
        {

            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product/Index");
            var data = JsonReader.GetTestRow("ProductVariantData.json", "TC_PRODUCT_15_2");
            variantPage.GoToFirstProductVariant();
            variantPage.OpenAddModal();
            variantPage.InputVariantDetails(data);
            variantPage.Save();
            System.Threading.Thread.Sleep(3000);
            Assert.That(driver.PageSource.Contains((string)data.Color), Is.True, "Lỗi: Không tìm thấy màu sắc biến thể mới!");
        }

    }
}