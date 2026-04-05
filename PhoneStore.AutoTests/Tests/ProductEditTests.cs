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
    public class ProductEditTests : BaseTest
    {
        private LoginPage loginPage;
        private ProductEditPage editPage;
        private WebDriverWait wait;

        [SetUp]
        public void PreTest()
        {
            // Đã xóa bỏ LicenseContext ở đây
            loginPage = new LoginPage(driver);
            editPage = new ProductEditPage(driver);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Account/Login");
            loginPage.Login("admin@shop.com", "admin123", isAdmin: true);
        }

        [Test]
        public void TC_PRODUCT_07_EditFirstProduct()
        {
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
            var data = JsonReader.GetTestRow("ProductEditData.json", "TC_PRODUCT_07");

            // 1. Vào trang sửa
            editPage.GoToEditFirstProduct();

            // 2. Điền form
            editPage.FillEditForm(data);

            // 3. Bấm Lưu
            editPage.Save();

            // 4. Nghỉ 3 giây cho server xử lý
            System.Threading.Thread.Sleep(3000);

            // 5. Đợi quay về trang danh sách (Sửa thành đợi chữ "Product")
            // Vì web của Vy hiện tại là localhost:7033/Product
            bool isNavigated = wait.Until(d => d.Url.ToLower().Contains("product"));

            Assert.That(isNavigated, Is.True, "Lỗi: Robot không thấy quay về trang danh sách sản phẩm!");

            // 6. Kiểm tra tên mới đã xuất hiện chưa
            Assert.That(driver.PageSource.Contains((string)data.ProductName), Is.True, "Lỗi: Không tìm thấy tên sản phẩm mới trên web!");
        }
    }
}