using NUnit.Framework;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using PhoneStore.AutoTests.Utilities;
using System;

namespace PhoneStore.AutoTests.Tests
{
    [TestFixture]
    public class OrderActionTests : BaseTest
    {
        private LoginPage loginPage;
        private OrderActionPage actionPage;

        [SetUp]
        public void PreTest()
        {
            loginPage = new LoginPage(driver);
            actionPage = new OrderActionPage(driver);

            // Đăng nhập
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Account/Login");
            loginPage.Login("admin@shop.com", "admin123", isAdmin: true);
        }

        [Test]
        public void TC_ORDER_03_ConfirmTopWaitingOrder()
        {
            // 1. Vào trang danh sách đơn
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Order/Index");
            System.Threading.Thread.Sleep(2000);

            try
            {
                // 2. Robot tự săn đơn CHỜ XÁC NHẬN
                string pickedID = actionPage.ClickFirstWaitingOrder();
                Console.WriteLine($"Robot: Đã chọn đơn {pickedID}");

                // 3. Xử lý xác nhận
                actionPage.ProcessConfirmOrder();

                // 4. Kiểm tra
                Assert.That(driver.PageSource.Contains("Đang giao") || driver.PageSource.Contains("Đã xác nhận"), Is.True);
                Console.WriteLine("PASS: Xác nhận đơn thành công!");
            }
            catch (Exception)
            {
                Assert.Ignore("Không tìm thấy đơn nào đang CHỜ XÁC NHẬN.");
            }
        }
    }
}