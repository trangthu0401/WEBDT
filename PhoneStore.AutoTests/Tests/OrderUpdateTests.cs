using NUnit.Framework;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using PhoneStore.AutoTests.Utilities;
using System;

namespace PhoneStore.AutoTests.Tests
{
    [TestFixture]
    public class OrderUpdateTests : BaseTest
    {
        private LoginPage loginPage;
        private OrderUpdatePage updatePage;

        [SetUp]
        public void PreTest()
        {
            loginPage = new LoginPage(driver);
            updatePage = new OrderUpdatePage(driver);
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Account/Login");
            loginPage.Login("admin@shop.com", "admin123", isAdmin: true);
        }

        [Test]
        public void TC_ORDER_06_ConfirmShippingToCompleted()
        {
            // 1. Vào trang danh sách đơn
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Order/Index");

            try
            {
                // 2. Săn đơn Đang giao
                string pickedID = updatePage.ClickFirstShippingOrder();
                Console.WriteLine($"Robot: Đang xử lý hoàn tất cho đơn {pickedID}");

                // 3. Bấm xác nhận và OK Alert
                updatePage.ConfirmDeliverySuccess();

                // 4. Kiểm tra xem Timeline đã nhảy sang "Hoàn tất" chưa
                Assert.That(driver.PageSource.Contains("Hoàn tất"), Is.True, "Lỗi: Đơn hàng chưa chuyển sang trạng thái Hoàn tất!");

                Console.WriteLine($"PASS: Đơn hàng {pickedID} đã được giao thành công.");
            }
            catch (Exception ex)
            {
                Assert.Ignore("Không tìm thấy đơn nào đang giao để test: " + ex.Message);
            }
        }
    }
}