using NUnit.Framework;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using PhoneStore.AutoTests.Utilities;
using System;

namespace PhoneStore.AutoTests.Tests
{
    [TestFixture]
    public class ProductVariantTests : BaseTest
    {
        private LoginPage loginPage;
        private ProductVariantPage variantPage;

        [SetUp]
        public void PreTest()
        {
            loginPage = new LoginPage(driver);
            variantPage = new ProductVariantPage(driver);

            // Đăng nhập Admin
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Account/Login");
            loginPage.Login("admin@shop.com", "admin123", isAdmin: true);
        }

        [Test]
        public void TC_PRODUCT_15_AddVariant_Iphone16e()
        {
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product/Index");
            var data = JsonReader.GetTestRow("ProductVariantData.json", "TC_PRODUCT_15");

            variantPage.GoToFirstProductVariant();
            variantPage.OpenAddModal();
            variantPage.InputVariantDetails(data);
            variantPage.Save();

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

        [Test]
        public void TC_PRODUCT_15_3_AddVariant_SamsungFlip7()
        {
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product/Index");
            var data = JsonReader.GetTestRow("ProductVariantData.json", "TC_PRODUCT_15_3");

            variantPage.GoToFirstProductVariant();
            variantPage.OpenAddModal();
            variantPage.InputVariantDetails(data);
            variantPage.Save();

            System.Threading.Thread.Sleep(3000);
            Assert.That(driver.PageSource.Contains((string)data.Color), Is.True, "Lỗi: Không tìm thấy màu sắc biến thể mới!");
        }
    }
}