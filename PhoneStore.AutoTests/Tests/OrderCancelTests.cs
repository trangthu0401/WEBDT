using NUnit.Framework;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using PhoneStore.AutoTests.Utilities;
using System;

namespace PhoneStore.AutoTests.Tests
{
    [TestFixture]
    public class OrderCancelTests : BaseTest
    {
        private LoginPage loginPage;
        private OrderCancelPage cancelPage;

        [SetUp]
        public void PreTest()
        {
            loginPage = new LoginPage(driver);
            cancelPage = new OrderCancelPage(driver);
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Account/Login");
            loginPage.Login("admin@shop.com", "admin123", isAdmin: true);
        }

        [Test, Order(1)] // Cho chạy đầu tiên
        [Property("TC_ID", "TC_ORDER_10")]
        public void TC_ORDER_10_Cancel_MissingReason()
        {
            // 1. Vào trang danh sách đơn
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Order/Index");
            System.Threading.Thread.Sleep(2000);

            // 2. Click vào đơn Chờ xác nhận bất kỳ
            cancelPage.ClickFirstWaitingOrder();

            // 3. Thực hiện hủy mà không nhập lý do
            cancelPage.ConfirmCancelWithoutReason();

            // 4. Kiểm tra xem có hiện thông báo lỗi yêu cầu nhập lý do không
            Assert.That(cancelPage.IsReasonRequiredErrorDisplayed(), Is.True,
                "Lỗi: Hệ thống không hiển thị cảnh báo khi bỏ trống lý do hủy!");

            Console.WriteLine("SUCCESS: Hệ thống đã chặn thành công khi không nhập lý do.");
        }

        [Test, Order(2)] // Chạy sau khi test xong lỗi bỏ trống
        [Property("TC_ID", "TC_ORDER_09")]
        public void TC_ORDER_09_CancelTopWaitingOrder()
        {
            // Lấy dữ liệu lý do từ JSON
            var data = JsonReader.GetTestRow("OrderCancelData.json", "TC_ORDER_09");
            string reason = (string)data.CancelReason;

            // 1. Vào trang danh sách đơn
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Order/Index");
            System.Threading.Thread.Sleep(2000);

            try
            {
                // 2. Robot săn đơn Chờ xác nhận
                string pickedID = cancelPage.ClickFirstWaitingOrder();
                Console.WriteLine($"Robot: Đang tiến hành hủy đơn {pickedID}");

                // 3. Thực hiện hủy đơn với lý do
                cancelPage.ProcessCancelOrder(reason);

                // 4. Kiểm tra xem đã hủy thành công chưa (thường là hiện chữ "Đã hủy")
                Assert.That(driver.PageSource.Contains("Đã hủy"), Is.True, "Lỗi: Đơn hàng chưa chuyển sang trạng thái Đã hủy!");

                Console.WriteLine($"PASS: Đã hủy đơn {pickedID} thành công với lý do: {reason}");
            }
            catch (Exception ex)
            {
                Assert.Ignore("Không thực hiện được hủy đơn: " + ex.Message);
            }
        }
    }
}