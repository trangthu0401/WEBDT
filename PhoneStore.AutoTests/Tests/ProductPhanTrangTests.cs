using NUnit.Framework;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using PhoneStore.AutoTests.Utilities;
using System;

namespace PhoneStore.AutoTests.Tests
{
    [TestFixture]
    public class ProductPhanTrangTests : BaseTest
    {
        private LoginPage loginPage;
        private ProductPhanTrangPage phanTrangPage;

        [SetUp]
        public void PreTest()
        {
            loginPage = new LoginPage(driver);
            phanTrangPage = new ProductPhanTrangPage(driver);

            // 1. Đăng nhập Admin
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Account/Login");
            loginPage.Login("admin@shop.com", "admin123", isAdmin: true);

            // 2. Vào trang Sản phẩm
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
        }

        [Test, Order(1)]
        [Property("TC_ID", "TC_PRODUCT_28")]
        public void TC_PRODUCT_28_NextToPage2()
        {
            // 1. Click sang trang 2
            phanTrangPage.ClickToPage("2");

            // 2. Kiểm tra trang hiện tại có phải là số 2 không
            string currentPage = phanTrangPage.GetCurrentActivePage();
            Assert.That(currentPage, Is.EqualTo("2"), "Lỗi: Robot không chuyển sang được trang 2!");

            Console.WriteLine("SUCCESS: Đã chuyển sang trang 2 thành công.");
        }

        [Test, Order(2)]
        [Property("TC_ID", "TC_PRODUCT_29")]
        public void TC_PRODUCT_29_BackToPage1()
        {
            // 1. Đảm bảo đang ở trang 2 trước đã
            phanTrangPage.ClickToPage("2");
            Console.WriteLine("Đang ở trang 2, bắt đầu quay về trang 1...");

            // 2. Click vào số 1 để quay lại trang đầu
            phanTrangPage.ClickToPage("1");

            // 3. Kiểm tra số trang active phải là 1
            Assert.That(phanTrangPage.GetCurrentActivePage(), Is.EqualTo("1"), "Lỗi: Không về được trang 1!");

            // 4. Kiểm tra URL phải chứa page=1 (hoặc quay về mặc định)
            Assert.That(phanTrangPage.GetCurrentUrl().ToLower(), Does.Contain("page=1").Or.Not.Contain("page="),
                "Lỗi: URL không hiển thị đúng trang 1!");

            Console.WriteLine("SUCCESS: Đã quay về trang 1 và kiểm tra URL thành công.");
        }
    }
}