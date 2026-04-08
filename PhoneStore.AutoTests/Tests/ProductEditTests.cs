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

            // 1. Vào trang sửa sản phẩm đầu tiên
            editPage.GoToEditFirstProduct();

            // 2. Điền form chỉnh sửa (Lấy ảnh từ project)
            editPage.FillEditForm(data);

            // 3. Bấm Lưu
            editPage.Save();

            // 4. Đợi server xử lý
            System.Threading.Thread.Sleep(3000);

            // 5. Kiểm tra kết quả
            bool isNavigated = wait.Until(d => d.Url.ToLower().Contains("product"));
            Assert.That(isNavigated, Is.True, "Lỗi: Không quay về trang danh sách!");
            Assert.That(driver.PageSource.Contains((string)data.ProductName), Is.True, "Lỗi: Tên sản phẩm mới không hiển thị!");

            Console.WriteLine("PASS: Chỉnh sửa sản phẩm thành công!");
        }
    }
}