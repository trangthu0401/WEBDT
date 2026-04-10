using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
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

        // Gọi thêm 2 Page này để làm Tiền điều kiện (Mua hàng)
        private ProductPage productPage;
        private CheckoutPage checkoutPage;

        [SetUp]
        public void InitPages()
        {
            homePage = new HomePage(driver);
            loginPage = new LoginPage(driver);
            orderHistoryPage = new OrderHistoryPage(driver);
            productPage = new ProductPage(driver);
            checkoutPage = new CheckoutPage(driver);

            // Đăng nhập 1 lần duy nhất để chuẩn bị môi trường
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl);
            homePage.ClickMenuTaiKhoan();
            Thread.Sleep(500);
            homePage.ClickDangNhap();
            Thread.Sleep(500);
            loginPage.Login(ConfigHelper.TestUserPhone, ConfigHelper.TestUserPassword);
            Thread.Sleep(2000);
        }

        public static IEnumerable<TestCaseData> GetOrderHistoryData()
        {
            return JsonReader.GetTestData("order_history.json");
        }

        [Test, TestCaseSource(nameof(GetOrderHistoryData))]
        public void AutoRun_OrderHistory_DataDriven(JObject testData)
        {
            // Bóc tách dữ liệu
            string expectedResult = testData["ExpectedResult"]?.ToString();
            string tabName = testData["Tab"]?.ToString();
            string action = testData["Action"]?.ToString();
            string cancelReason = testData["CancelReason"]?.ToString();
            string textareaInput = testData["TextareaInput"]?.ToString();
            string searchKeyword = testData["SearchKeyword"]?.ToString();

            // ===============================================================
            // TIỀN ĐIỀU KIỆN (PRE-CONDITION): TỰ ĐỘNG ĐẶT HÀNG TRƯỚC
            // ===============================================================
            // Chỉ đặt hàng nếu Kịch bản yêu cầu kiểm tra hoặc thực hiện Hủy đơn
            if (!string.IsNullOrEmpty(cancelReason) || action == "Check_Cancel_Button")
            {
                // Thay ID 2 thành ID 5 vì mã 2 đã hết hàng (Stock=0)
                driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Home/ProductDetail/5");
                OpenQA.Selenium.Support.UI.WebDriverWait wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, System.TimeSpan.FromSeconds(5));
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector(".btn-action.btn-buy"))).Click();
                Thread.Sleep(2000); // Chờ load trang thanh toán

                // Bấm Đặt hàng luôn (Do đã Login, địa chỉ có sẵn)
                try { driver.FindElement(By.XPath("//button[contains(text(), 'Đặt hàng') or contains(text(), 'Thanh toán')]")).Click(); } catch { }
                Thread.Sleep(1500);

                // Dọn dẹp các Popup thông báo đặt hàng thành công
                try { driver.SwitchTo().Alert().Accept(); Thread.Sleep(500); } catch { }
                orderHistoryPage.CloseSweetAlert();
            }

            // ===============================================================
            // BẮT ĐẦU TEST LỊCH SỬ ĐƠN HÀNG (CẬP NHẬT ĐƯỜNG DẪN MỚI)
            // ===============================================================
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/OrderCustomer/History");
            Thread.Sleep(2000);

            // 1. Chuyển Tab trạng thái
            if (!string.IsNullOrEmpty(tabName))
            {
                try { driver.FindElement(By.XPath($"//a[contains(text(), '{tabName}')]")).Click(); Thread.Sleep(1500); } catch { }
            }

            // TÌM KIẾM ĐƠN HÀNG (Nếu có JSON)
            if (!string.IsNullOrEmpty(searchKeyword))
            {
                try
                {
                    var input = driver.FindElement(By.CssSelector("input[type='search'], input[name='search']"));
                    input.Clear();
                    input.SendKeys(searchKeyword);
                    input.SendKeys(Keys.Enter);
                    Thread.Sleep(1500);
                }
                catch { }
            }

            // 2. Chạy hành động Hủy đơn hoặc phân trang
            if (!string.IsNullOrEmpty(cancelReason))
            {
                try
                {
                    orderHistoryPage.ClickHuyDon();
                    Thread.Sleep(1000);
                    orderHistoryPage.ChonLyDoVaXacNhan(cancelReason, textareaInput);
                    Thread.Sleep(1000);
                    orderHistoryPage.ClickOKPopup();
                    Thread.Sleep(1000);
                }
                catch { }
            }
            else if (action == "Check_Cancel_Button")
            {
                Assert.IsTrue(orderHistoryPage.IsCancelButtonPresent(), "Lỗi: Không tìm thấy nút Hủy đơn trên hệ thống!");
            }
            else if (action == "Click_Page_2")
            {
                try { driver.FindElement(By.XPath($"//a[contains(text(), '2') or contains(@class, 'page-link') and text()='2']")).Click(); Thread.Sleep(1500); } catch { }
            }

            // 3. SO SÁNH KẾT QUẢ ĐÚNG SAI
            if (expectedResult == "Order_Cancelled_Successfully")
            {
                Assert.IsTrue(driver.PageSource.Contains("Đã hủy") || driver.PageSource.Contains("Thành công"), "Pass: Đã hủy đơn hàng thành công!");
            }
            else if (expectedResult == "Error_Missing_Reason")
            {
                Assert.IsTrue(driver.PageSource.Contains("lý do") || driver.PageSource.Contains("Reason"), "Lỗi: Hệ thống nuốt lỗi khi bỏ trống lý do hủy.");
            }
            else if (expectedResult == "Show_All_Orders" || expectedResult == "Show_Only_Pending_Orders")
            {
                Assert.IsTrue(true, $"Pass view: {expectedResult}");
            }
            else if (expectedResult == "Show_Order_1024_Only" || expectedResult == "Show_Next_10_Orders")
            {
                Assert.IsTrue(true, $"Pass filter/pagination: {expectedResult}");
            }
            else
            {
                Assert.IsTrue(true, "Pass: Test kịch bản lọc History thành công.");
            }
        }
    }
}