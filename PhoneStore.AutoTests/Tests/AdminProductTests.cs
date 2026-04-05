using NUnit.Framework;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using PhoneStore.AutoTests.Utilities;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Threading;

namespace PhoneStore.AutoTests.Tests
{
    [TestFixture]
    public class AdminProductTests : BaseTest
    {
        private LoginPage loginPage;
        private AdminProductPage adminPage;
        private WebDriverWait wait;

        [SetUp]
        public void PreTest()
        {
            loginPage = new LoginPage(driver);
            adminPage = new AdminProductPage(driver);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30)); // Tăng lên 30s cho thoải mái

            // 1. Đăng nhập Admin trước mỗi case
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Account/Login");
            loginPage.Login("admin@shop.com", "admin123", isAdmin: true);
        }

        // Hàm phụ trợ để tránh lặp code, xử lý đợi URL thông minh hơn
        private void VerifyAddProductSuccess(string productName)
        {
            // Nghỉ 3 giây để Server kịp lưu Data và Ảnh
            Thread.Sleep(3000);

            // Đợi URL chứa chữ "Product" (Vì web Vy quay về /Product chứ không phải /Index)
            bool isNavigated = wait.Until(d => d.Url.ToLower().Contains("product"));

            Assert.That(isNavigated, Is.True, $"Lỗi: Robot không thấy quay về trang danh sách sau khi thêm {productName}!");
            Assert.That(driver.PageSource.Contains(productName), Is.True, $"Lỗi: Không tìm thấy tên {productName} trên trang danh sách!");
        }

        [Test]
        public void TC_PRODUCT_ADMIN_01_AddFullProduct()
        {
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
            var data = JsonReader.GetTestRow("AdminProductData.json", "TC_PRODUCT_ADMIN_01");

            adminPage.GoToCreatePage();
            adminPage.InputProductDetails(data);
            adminPage.Save();

            VerifyAddProductSuccess((string)data.ProductName);
        }

        [Test]
        public void TC_PRODUCT_ADMIN_01_1_AddStandardS24()
        {
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
            var data = JsonReader.GetTestRow("AdminProductData.json", "TC_PRODUCT_ADMIN_01_1");

            adminPage.GoToCreatePage();
            adminPage.InputProductDetails(data);
            adminPage.Save();

            VerifyAddProductSuccess((string)data.ProductName);
        }

        [Test]
        public void TC_PRODUCT_ADMIN_01_2_AddGalaxyA06()
        {
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
            var data = JsonReader.GetTestRow("AdminProductData.json", "TC_PRODUCT_ADMIN_01_2");

            adminPage.GoToCreatePage();
            adminPage.InputProductDetails(data);
            adminPage.Save();

            VerifyAddProductSuccess("Samsung Galaxy A06");
        }

        [Test]
        public void TC_PRODUCT_ADMIN_01_3_Addiphone16e()
        {
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
            var data = JsonReader.GetTestRow("AdminProductData.json", "TC_PRODUCT_ADMIN_01_3");

            adminPage.GoToCreatePage();
            adminPage.InputProductDetails(data);
            adminPage.Save();

            VerifyAddProductSuccess((string)data.ProductName);
        }

        [Test]
        public void TC_PRODUCT_ADMIN_01_4_Addiphone13promax()
        {
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
            var data = JsonReader.GetTestRow("AdminProductData.json", "TC_PRODUCT_ADMIN_01_4");

            adminPage.GoToCreatePage();
            adminPage.InputProductDetails(data);
            adminPage.Save();

            VerifyAddProductSuccess((string)data.ProductName);
        }

        [Test]
        public void TC_PRODUCT_ADMIN_01_5_Addiphone16()
        {
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
            var data = JsonReader.GetTestRow("AdminProductData.json", "TC_PRODUCT_ADMIN_01_5");

            adminPage.GoToCreatePage();
            adminPage.InputProductDetails(data);
            adminPage.Save();

            VerifyAddProductSuccess((string)data.ProductName);
        }

        [Test]
        public void TC_PRODUCT_ADMIN_01_6_Addiphone16plus()
        {
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
            var data = JsonReader.GetTestRow("AdminProductData.json", "TC_PRODUCT_ADMIN_01_6");

            adminPage.GoToCreatePage();
            adminPage.InputProductDetails(data);
            adminPage.Save();

            VerifyAddProductSuccess((string)data.ProductName);
        }

        [Test]
        public void TC_PRODUCT_ADMIN_01_7_Addiphone16pro()
        {
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
            var data = JsonReader.GetTestRow("AdminProductData.json", "TC_PRODUCT_ADMIN_01_7");

            adminPage.GoToCreatePage();
            adminPage.InputProductDetails(data);
            adminPage.Save();

            VerifyAddProductSuccess((string)data.ProductName);
        }

        [Test]
        public void TC_PRODUCT_ADMIN_01_8_Addiphone15()
        {
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
            var data = JsonReader.GetTestRow("AdminProductData.json", "TC_PRODUCT_ADMIN_01_8");

            adminPage.GoToCreatePage();
            adminPage.InputProductDetails(data);
            adminPage.Save();

            VerifyAddProductSuccess((string)data.ProductName);
        }
    }
}