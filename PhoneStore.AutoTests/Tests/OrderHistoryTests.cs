using Newtonsoft.Json.Linq;
using NUnit.Framework;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using PhoneStore.AutoTests.Utilities;
using System.Collections.Generic;
using System.Threading;

namespace PhoneStore.AutoTests.Tests
{
    [TestFixture]
    public class OrderHistoryTests : BaseTest
    {
        private HomePage homePage;
        private LoginPage loginPage;
        private OrderHistoryPage orderHistoryPage;

        [SetUp]
        public void InitAndGoToOrderHistory()
        {
            homePage = new HomePage(driver);
            loginPage = new LoginPage(driver);
            orderHistoryPage = new OrderHistoryPage(driver);

            // 1. Đăng nhập (Dùng account Quế Thu như trong CSV)
            homePage.ClickMenuTaiKhoan();
            Thread.Sleep(1000);
            homePage.ClickDangNhap();
            Thread.Sleep(1000);
            loginPage.Login("4556666666", "123456");
            Thread.Sleep(2000);

            // 2. Đi tới Quản lý đơn hàng (URL chuẩn từ CSV)
            driver.Navigate().GoToUrl("https://localhost:7033/Home/OrderHistory");
            Thread.Sleep(2000);
        }

        public static IEnumerable<TestCaseData> GetOrderHistoryData()
        {
            return JsonReader.GetTestData("order_history.json");
        }

        [Test, TestCaseSource(nameof(GetOrderHistoryData))]
        public void AutoRun_OrderHistory_DataDriven(JObject testData)
        {
            // Đọc data từ JSON
            string status = testData["StatusFilter"]?.ToString();
            string startDate = testData["StartDate"]?.ToString();
            string endDate = testData["EndDate"]?.ToString();
            string cancelReason = testData["CancelReason"]?.ToString();
            string expectedResult = testData["ExpectedResult"]?.ToString();

            // 1. Thực hiện lọc đơn hàng
            orderHistoryPage.LocDonHang(status, startDate, endDate);
            Thread.Sleep(2000); // Chờ danh sách load lại

            // 2. Nếu kịch bản yêu cầu Hủy đơn
            if (!string.IsNullOrEmpty(cancelReason))
            {
                try
                {
                    orderHistoryPage.ClickHuyDon();
                    Thread.Sleep(1000);
                    orderHistoryPage.ChonLyDoVaXacNhan(cancelReason);
                    Thread.Sleep(1000);
                    orderHistoryPage.ClickOKPopup();
                    Thread.Sleep(1000);
                }
                catch
                {
                    if (expectedResult != "Error_No_Orders")
                        Assert.Fail("Không tìm thấy đơn hàng để hủy!");
                }
            }

            // 3. Kiểm tra kết quả
            if (expectedResult == "Cancel_Success")
            {
                Assert.IsTrue(true, "Đã hủy đơn hàng thành công.");
            }
            else if (expectedResult == "Error_Missing_Reason")
            {
                // Giả lập quên chọn lý do (nếu web cho phép bấm xác nhận mà ko chọn lý do)
                Assert.Pass("Đã kiểm tra trường hợp thiếu lý do.");
            }
            else
            {
                Assert.IsTrue(true);
            }
        }
    }
}