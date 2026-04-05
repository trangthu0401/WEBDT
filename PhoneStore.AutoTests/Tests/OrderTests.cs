using NUnit.Framework;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using PhoneStore.AutoTests.Utilities;
using System;

namespace PhoneStore.AutoTests.Tests
{
    [TestFixture]
    public class OrderTests : BaseTest
    {
        private LoginPage loginPage;
        private OrderDetailPage orderPage;

        [SetUp]
        public void PreTest()
        {
            loginPage = new LoginPage(driver);
            orderPage = new OrderDetailPage(driver);

            // Đăng nhập
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Account/Login");
            loginPage.Login("admin@shop.com", "admin123", isAdmin: true);
        }

        [Test]
        public void TC_ORDER_02_ViewDetail_ByCode()
        {
            var data = JsonReader.GetTestRow("OrderDetailData.json", "TC_ORDER_02");
            string targetCode = (string)data.OrderCode;

            // 1. Vào đúng trang Đơn hàng (Theo ảnh bạn gửi)
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Order/Index");

            // 2. Bấm vào đúng mã đơn HD01036
            orderPage.ClickOnOrderCode(targetCode);

            // 3. Kiểm tra nội dung bên trong trang chi tiết
            System.Threading.Thread.Sleep(2000);

            bool isNameOk = orderPage.VerifyContentExists((string)data.CustomerName);
            bool isStatusOk = orderPage.VerifyContentExists((string)data.Status);

            Assert.Multiple(() => {
                Assert.That(isNameOk, Is.True, $"Lỗi: Vào chi tiết đơn {targetCode} nhưng không thấy tên khách!");
                Assert.That(isStatusOk, Is.True, $"Lỗi: Trạng thái không phải {(string)data.Status}!");
            });

            Console.WriteLine($"PASS: Đã xem đúng chi tiết đơn hàng {targetCode}!");
        }
    }
}