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
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

            // 1. Đăng nhập Admin trước khi thực hiện mỗi Test Case
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Account/Login");
            loginPage.Login("admin@shop.com", "admin123", isAdmin: true);
        }

        // --- HÀM PHỤ TRỢ KIỂM TRA SAU KHI THÊM ---
        private void VerifyAddProductSuccess(string productName)
        {
            Thread.Sleep(3000); // Chờ server xử lý lưu ảnh và data
            bool isNavigated = wait.Until(d => d.Url.ToLower().Contains("product"));

            Assert.Multiple(() =>
            {
                Assert.That(isNavigated, Is.True, $"Lỗi: Robot không quay về trang danh sách sau khi thêm {productName}!");
                Assert.That(driver.PageSource.Contains(productName), Is.True, $"Lỗi: Không tìm thấy tên {productName} trên trang danh sách!");
            });

            Console.WriteLine($"SUCCESS: Đã thêm thành công sản phẩm: {productName}");
        }

        // --- CÁC TEST CASE THÊM MỚI SẢN PHẨM ---

        [Test]
        public void TC_PRODUCT_ADMIN_01_AddSamsungA56()
        {
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
            var data = JsonReader.GetTestRow("AdminProductData.json", "TC_PRODUCT_ADMIN_01");

            adminPage.GoToCreatePage();
            adminPage.InputProductDetails(data);
            adminPage.Save();

            VerifyAddProductSuccess((string)data.ProductName);
        }

        //[Test]
        //public void TC_PRODUCT_ADMIN_01_1_AddStandardS24()
        //{
        //    driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
        //    var data = JsonReader.GetTestRow("AdminProductData.json", "TC_PRODUCT_ADMIN_01_1");

        //    adminPage.GoToCreatePage();
        //    adminPage.InputProductDetails(data);
        //    adminPage.Save();

        //    VerifyAddProductSuccess((string)data.ProductName);
        //}

        //[Test]
        //public void TC_PRODUCT_ADMIN_01_2_AddGalaxyA06()
        //{
        //    driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
        //    var data = JsonReader.GetTestRow("AdminProductData.json", "TC_PRODUCT_ADMIN_01_2");

        //    adminPage.GoToCreatePage();
        //    adminPage.InputProductDetails(data);
        //    adminPage.Save();

        //    VerifyAddProductSuccess((string)data.ProductName);
        //}

        //[Test]
        //public void TC_PRODUCT_ADMIN_01_3_AddIphone16e()
        //{
        //    driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
        //    var data = JsonReader.GetTestRow("AdminProductData.json", "TC_PRODUCT_ADMIN_01_3");

        //    adminPage.GoToCreatePage();
        //    adminPage.InputProductDetails(data);
        //    adminPage.Save();

        //    VerifyAddProductSuccess((string)data.ProductName);
        //}

        //[Test]
        //public void TC_PRODUCT_ADMIN_01_4_AddIphone13ProMax()
        //{
        //    driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
        //    var data = JsonReader.GetTestRow("AdminProductData.json", "TC_PRODUCT_ADMIN_01_4");

        //    adminPage.GoToCreatePage();
        //    adminPage.InputProductDetails(data);
        //    adminPage.Save();

        //    VerifyAddProductSuccess((string)data.ProductName);
        //}

        //[Test]
        //public void TC_PRODUCT_ADMIN_01_5_AddIphone16()
        //{
        //    driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
        //    var data = JsonReader.GetTestRow("AdminProductData.json", "TC_PRODUCT_ADMIN_01_5");

        //    adminPage.GoToCreatePage();
        //    adminPage.InputProductDetails(data);
        //    adminPage.Save();

        //    VerifyAddProductSuccess((string)data.ProductName);
        //}

        //[Test]
        //public void TC_PRODUCT_ADMIN_01_6_AddIphone16Plus()
        //{
        //    driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
        //    var data = JsonReader.GetTestRow("AdminProductData.json", "TC_PRODUCT_ADMIN_01_6");

        //    adminPage.GoToCreatePage();
        //    adminPage.InputProductDetails(data);
        //    adminPage.Save();

        //    VerifyAddProductSuccess((string)data.ProductName);
        //}

        //[Test]
        //public void TC_PRODUCT_ADMIN_01_7_AddIphone16Pro()
        //{
        //    driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
        //    var data = JsonReader.GetTestRow("AdminProductData.json", "TC_PRODUCT_ADMIN_01_7");

        //    adminPage.GoToCreatePage();
        //    adminPage.InputProductDetails(data);
        //    adminPage.Save();

        //    VerifyAddProductSuccess((string)data.ProductName);
        //}

        //[Test]

        //public void TC_PRODUCT_ADMIN_01_8_AddIphone15()
        //{
        //    driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
        //    var data = JsonReader.GetTestRow("AdminProductData.json", "TC_PRODUCT_ADMIN_01_8");

        //    adminPage.GoToCreatePage();
        //    adminPage.InputProductDetails(data);
        //    adminPage.Save();

        //    VerifyAddProductSuccess((string)data.ProductName);
        //}

        //[Test]
        //public void TC_PRODUCT_ADMIN_01_09_AddSamsungZFlip7()
        //{
        //    driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
        //    var data = JsonReader.GetTestRow("AdminProductData.json", "TC_PRODUCT_ADMIN_01_09");

        //    adminPage.GoToCreatePage();
        //    adminPage.InputProductDetails(data);
        //    adminPage.Save();

        //    VerifyAddProductSuccess((string)data.ProductName);
        //}

        //[Test]
        //public void TC_PRODUCT_ADMIN_01_10_AddZFold7Den()
        //{
        //    driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
        //    var data = JsonReader.GetTestRow("AdminProductData.json", "TC_PRODUCT_ADMIN_01_10");

        //    adminPage.GoToCreatePage();
        //    adminPage.InputProductDetails(data);
        //    adminPage.Save();

        //    VerifyAddProductSuccess((string)data.ProductName);
        //}

        [Test]
        [Property("TC_ID", "TC_PRODUCT_02")]
        public void TC_PRODUCT_02_AddProduct_MissingRequired()
        {
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
            var data = JsonReader.GetTestRow("AdminProductData.json", "TC_PRODUCT_02");

            adminPage.GoToCreatePage();
            adminPage.InputProductDetails(data);
            adminPage.Save();

            // Kiểm tra web không chuyển trang và hiển thị lỗi bắt buộc
            Assert.That(driver.Url.Contains("Create"), Is.True, "Lỗi: Hệ thống lẽ ra không được chuyển trang khi thiếu tên!");
            Assert.That(driver.PageSource.Contains("required") || driver.PageSource.Contains("bắt buộc"), Is.True, "Lỗi: Không thấy cảnh báo nhập liệu!");
        }

        [Test]
        [Property("TC_ID", "TC_PRODUCT_03")]
        public void TC_PRODUCT_03_AddProduct_InvalidNumber()
        {
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
            var data = JsonReader.GetTestRow("AdminProductData.json", "TC_PRODUCT_03");

            adminPage.GoToCreatePage();
            adminPage.InputProductDetails(data);
            adminPage.Save();

            // Kiểm tra thông báo lỗi định dạng số
            Assert.That(driver.PageSource.Contains("định dạng") || driver.PageSource.Contains("number"), Is.True, "Lỗi: Hệ thống không báo lỗi khi nhập chữ vào ô Giá!");
        }

        [Test]
        [Property("TC_ID", "TC_PRODUCT_04")]
        public void TC_PRODUCT_04_AddProduct_UploadImage()
        {
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
            var data = JsonReader.GetTestRow("AdminProductData.json", "TC_PRODUCT_04");

            adminPage.GoToCreatePage();
            adminPage.InputProductDetails(data);
            adminPage.Save();

            // Kiểm tra thành công (Giống TC 01)
            VerifyAddProductSuccess((string)data.ProductName);
        }

        [Test]
        [Property("TC_ID", "TC_PRODUCT_05")]
        public void TC_PRODUCT_05_CancelAddProduct()
        {
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
            var data = JsonReader.GetTestRow("AdminProductData.json", "TC_PRODUCT_05");

            adminPage.GoToCreatePage();
            // Nhập một số thông tin rồi nhấn Hủy
            adminPage.InputProductDetails(data);
            adminPage.Cancel();

            // Kiểm tra quay về trang danh sách và sản phẩm chưa được thêm
            wait.Until(d => d.Url.ToLower().Contains("product"));
            Assert.That(driver.PageSource.Contains((string)data.ProductName), Is.False, "Lỗi: Sản phẩm vẫn được thêm dù đã nhấn Hủy!");
        }
    }
}
