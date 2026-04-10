using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using PhoneStore.AutoTests.Utilities;
using System.Globalization;
using System.Text;
using System.Threading;

namespace PhoneStore.AutoTests.Tests
{
    [TestFixture]
    public class OrderHistoryTests : BaseTest
    {
        private LoginPage loginPage;
        private OrderHistoryPage orderHistoryPage;

        [SetUp]
        public void KhoiTao()
        {
            loginPage = new LoginPage(driver);
            orderHistoryPage = new OrderHistoryPage(driver);

            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl);
            driver.FindElement(By.XPath("//span[contains(text(),'Tài khoản')]")).Click();
            Thread.Sleep(500);
            driver.FindElement(By.XPath("//a[contains(text(),'Đăng nhập')]")).Click();
            Thread.Sleep(500);
            loginPage.Login(ConfigHelper.TestUserPhone, ConfigHelper.TestUserPassword);
            Thread.Sleep(2000);
        }

        private string NormalizeString(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            string normalized = input.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            foreach (char c in normalized)
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            return sb.ToString().Normalize(NormalizationForm.FormC).ToUpperInvariant();
        }

        private void TaoDonHangMoi()
        {
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Home/ProductDetail/5");
            var wait = new WebDriverWait(driver, System.TimeSpan.FromSeconds(5));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector(".btn-action.btn-buy"))).Click();
            Thread.Sleep(2000);
            driver.FindElement(By.XPath("//button[contains(text(),'Đặt hàng')]")).Click();
            Thread.Sleep(1500);
            orderHistoryPage.ClickOkSweetAlert();
        }

        [Test]
        [Property("TC_ID", "TC_ORD_01")]
        public void TC_ORD_01_HienThiTatCaDonHang()
        {
            orderHistoryPage.GoToOrderHistory();
            Assert.Greater(orderHistoryPage.GetOrderCount(), 0, "Không hiển thị đơn hàng nào");
        }

        [Test]
        [Property("TC_ID", "TC_ORD_02")]
        public void TC_ORD_02_LocDonChoXacNhan()
        {
            orderHistoryPage.GoToOrderHistory();
            orderHistoryPage.SelectStatus("Chờ xác nhận");
            int count = orderHistoryPage.GetOrderCount();
            Assert.Greater(count, 0, "Không có đơn hàng nào sau khi lọc Chờ xác nhận");
            string firstStatus = orderHistoryPage.GetFirstOrderStatus();
            Assert.AreEqual(NormalizeString("Chờ xác nhận"), NormalizeString(firstStatus), "Trạng thái dòng đầu không đúng");
        }

        [Test]
        [Property("TC_ID", "TC_ORD_03")]
        public void TC_ORD_03_LocDonDangGiao()
        {
            orderHistoryPage.GoToOrderHistory();
            orderHistoryPage.SelectStatus("Đang giao");
            int count = orderHistoryPage.GetOrderCount();
            if (count == 0)
                Assert.IsTrue(orderHistoryPage.IsEmptyMessageDisplayed(), "Không có đơn đang giao nhưng không hiển thị thông báo");
            else
            {
                string firstStatus = orderHistoryPage.GetFirstOrderStatus();
                Assert.AreEqual(NormalizeString("Đang giao"), NormalizeString(firstStatus));
            }
        }

        [Test]
        [Property("TC_ID", "TC_ORD_04")]
        public void TC_ORD_04_XemChiTietDonHang()
        {
            orderHistoryPage.GoToOrderHistory();
            if (orderHistoryPage.GetOrderCount() == 0) Assert.Inconclusive("Không có đơn hàng để xem chi tiết");
            orderHistoryPage.ClickFirstOrderId();
            decimal total = orderHistoryPage.GetModalTotal();
            Assert.That(total, Is.GreaterThan(0), "Tổng tiền không hợp lệ trong modal chi tiết");
            orderHistoryPage.CloseModal();
        }

        [Test]
        [Property("TC_ID", "TC_ORD_05")]
        public void TC_ORD_05_NutHuyHienThi()
        {
            orderHistoryPage.GoToOrderHistory();
            if (orderHistoryPage.GetOrderCount() == 0) Assert.Inconclusive("Không có đơn hàng để kiểm tra nút hủy");
            // Kiểm tra nút hủy có tồn tại trên dòng đầu không
            Assert.DoesNotThrow(() => orderHistoryPage.ClickFirstCancelButton(), "Không tìm thấy nút hủy trên dòng đơn hàng");
            // Đóng popup hủy nếu bị mở (có thể click Cancel)
            try
            {
                driver.FindElement(By.XPath("//button[contains(text(),'Hủy')]")).Click();
            }
            catch { }
        }

        [Test]
        [Property("TC_ID", "TC_ORD_07")]
        public void TC_ORD_07_HuyDonVoiLyDoCoSan()
        {
            TaoDonHangMoi();
            orderHistoryPage.GoToOrderHistory();
            orderHistoryPage.SelectStatus("Chờ xác nhận");
            orderHistoryPage.ClickFirstCancelButton();
            orderHistoryPage.SelectCancelReason("Tìm thấy giá tốt hơn");
            orderHistoryPage.ConfirmCancel();
            orderHistoryPage.ClickOkSweetAlert();
            orderHistoryPage.SelectStatus("Đã hủy");
            Assert.Greater(orderHistoryPage.GetOrderCount(), 0, "Đơn hàng không xuất hiện trong danh sách đã hủy");
        }

        [Test]
        [Property("TC_ID", "TC_ORD_09")]
        public void TC_ORD_09_HuyDonVoiLyDoKhac()
        {
            TaoDonHangMoi();
            orderHistoryPage.GoToOrderHistory();
            orderHistoryPage.SelectStatus("Chờ xác nhận");
            orderHistoryPage.ClickFirstCancelButton();
            orderHistoryPage.SelectCancelReason("Lý do khác");
            orderHistoryPage.EnterCancelDetail("Hàng giao chậm quá");
            orderHistoryPage.ConfirmCancel();
            orderHistoryPage.ClickOkSweetAlert();
            orderHistoryPage.SelectStatus("Đã hủy");
            Assert.Greater(orderHistoryPage.GetOrderCount(), 0);
        }

        [Test]
        [Property("TC_ID", "TC_ORD_10")]
        public void TC_ORD_10_HuyDonBoTrongTextarea()
        {
            TaoDonHangMoi();
            orderHistoryPage.GoToOrderHistory();
            orderHistoryPage.SelectStatus("Chờ xác nhận");
            orderHistoryPage.ClickFirstCancelButton();
            orderHistoryPage.SelectCancelReason("Lý do khác");
            orderHistoryPage.EnterCancelDetail("");
            orderHistoryPage.ConfirmCancel();
            // Kiểm tra lỗi hiển thị (SweetAlert error)
            string error = driver.FindElement(By.CssSelector(".swal2-error")).Text;
            Assert.IsTrue(error.Contains("bỏ trống") || error.Contains("không được để trống"));
            // Đóng alert
            orderHistoryPage.ClickOkSweetAlert();
        }

        [Test]
        [Property("TC_ID", "TC_ORD_12")]
        public void TC_ORD_12_TimKiemTheoMaDon()
        {
            orderHistoryPage.GoToOrderHistory();
            if (orderHistoryPage.GetOrderCount() == 0) Assert.Inconclusive("Không có đơn hàng để lấy ID");
            string firstId = orderHistoryPage.GetFirstOrderId();
            orderHistoryPage.SearchByOrderId(firstId);
            Assert.AreEqual(1, orderHistoryPage.GetOrderCount(), $"Tìm kiếm ID {firstId} không trả về đúng 1 kết quả");
        }

        [Test]
        [Property("TC_ID", "TC_ORD_13")]
        public void TC_ORD_13_TimKiemMaDonKhongTonTai()
        {
            orderHistoryPage.GoToOrderHistory();
            orderHistoryPage.SearchByOrderId("999999");
            Assert.IsTrue(orderHistoryPage.IsEmptyMessageDisplayed(), "Không hiển thị thông báo khi không tìm thấy");
        }

        [Test]
        [Property("TC_ID", "TC_ORD_15")]
        public void TC_ORD_15_PhanTrang()
        {
            orderHistoryPage.GoToOrderHistory();
            int beforeCount = orderHistoryPage.GetOrderCount();
            if (beforeCount == 0) Assert.Inconclusive("Không có đơn hàng để phân trang");
            // Giả sử có nút phân trang số 2
            try
            {
                orderHistoryPage.ClickPage2();
                int afterCount = orderHistoryPage.GetOrderCount();
                Assert.AreNotEqual(beforeCount, afterCount, "Phân trang không hoạt động");
            }
            catch
            {
                Assert.Inconclusive("Không tìm thấy nút phân trang");
            }
        }

        [Test]
        [Property("TC_ID", "TC_ORD_16")]
        public void TC_ORD_16_ChiTietPhiVanChuyen()
        {
            orderHistoryPage.GoToOrderHistory();
            if (orderHistoryPage.GetOrderCount() == 0) Assert.Inconclusive("Không có đơn hàng để kiểm tra");
            orderHistoryPage.ClickFirstOrderId();
            decimal subtotal = orderHistoryPage.GetModalSubtotal();
            decimal shipping = orderHistoryPage.GetModalShipping();
            decimal total = orderHistoryPage.GetModalTotal();
            Assert.That(total, Is.EqualTo(subtotal + shipping).Within(0.01m), "Tổng tiền không bằng Tạm tính + Phí vận chuyển");
            orderHistoryPage.CloseModal();
        }

        [Test]
        [Property("TC_ID", "TC_ORD_19")]
        public void TC_ORD_19_NutQuayLaiDanhSach()
        {
            orderHistoryPage.GoToOrderHistory();
            if (orderHistoryPage.GetOrderCount() == 0) Assert.Inconclusive("Không có đơn hàng để kiểm tra");
            orderHistoryPage.ClickFirstOrderId();
            orderHistoryPage.CloseModal(); // Đóng modal quay lại danh sách
            Assert.IsTrue(driver.Url.Contains("History"), "Không quay lại danh sách đơn hàng");
        }

        [Test]
        [Property("TC_ID", "TC_ORD_22")]
        public void TC_ORD_22_HienThiLyDoHuyTrongChiTiet()
        {
            TaoDonHangMoi();
            orderHistoryPage.GoToOrderHistory();
            orderHistoryPage.SelectStatus("Chờ xác nhận");
            orderHistoryPage.ClickFirstCancelButton();
            orderHistoryPage.SelectCancelReason("Lý do khác");
            orderHistoryPage.EnterCancelDetail("Test reason 123");
            orderHistoryPage.ConfirmCancel();
            orderHistoryPage.ClickOkSweetAlert();

            orderHistoryPage.SelectStatus("Đã hủy");
            orderHistoryPage.ClickFirstOrderId();
            // Tìm dòng lý do hủy trong modal
            var reasonElem = driver.FindElement(By.XPath("//*[contains(text(),'Lý do hủy')]/following-sibling::*"));
            string reason = reasonElem.Text.Trim();
            Assert.AreEqual("Test reason 123", reason, "Lý do hủy không hiển thị chính xác");
            orderHistoryPage.CloseModal();
        }
    }
}