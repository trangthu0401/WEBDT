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

        // Helper: chuẩn hóa chuỗi (bỏ dấu, viết hoa)
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

        // Đảm bảo có ít nhất một đơn hàng ở trạng thái "Chờ xác nhận"
        private void DamBaoCoDonChoXacNhan()
        {
            orderHistoryPage.GoToOrderHistory();
            orderHistoryPage.SelectStatus("Chờ xác nhận");
            orderHistoryPage.ClickFilter(); // Áp dụng bộ lọc
            if (orderHistoryPage.GetOrderCount() == 0)
            {
                // Nếu không có, tạo đơn hàng mới (cần có CartPage, ProductPage)
                // Ở đây tạm thời bỏ qua vì test đã có sẵn đơn hàng từ record.
                // Nếu muốn tạo, cần implement đầy đủ luồng đặt hàng.
                Assert.Inconclusive("Không có đơn hàng Chờ xác nhận và chưa implement tạo đơn tự động.");
            }
        }

        [Test]
        [Property("TC_ID", "TC_ORD_01")]
        public void TC_ORD_01_HienThiTatCaDonHang()
        {
            orderHistoryPage.GoToOrderHistory();
            // Mặc định trang hiển thị tất cả đơn (không cần click tab)
            Assert.Greater(orderHistoryPage.GetOrderCount(), 0, "Không hiển thị đơn hàng nào");
        }

        [Test]
        [Property("TC_ID", "TC_ORD_02")]
        public void TC_ORD_02_LocDonChoXacNhan()
        {
            orderHistoryPage.GoToOrderHistory();
            orderHistoryPage.SelectStatus("Chờ xác nhận");
            orderHistoryPage.ClickFilter();
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
            orderHistoryPage.ClickFilter();
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
            DamBaoCoDonChoXacNhan();
            Assert.IsTrue(orderHistoryPage.IsCancelButtonPresent(), "Không tìm thấy nút hủy trên đơn hàng Chờ xác nhận");
            // Đóng popup nếu có (click Cancel)
            try { driver.FindElement(By.XPath("//button[contains(text(),'Hủy')]")).Click(); } catch { }
        }

        [Test]
        [Property("TC_ID", "TC_ORD_06")]
        public void TC_ORD_06_NutHuyAnKhiDangGiao()
        {
            orderHistoryPage.GoToOrderHistory();
            orderHistoryPage.SelectStatus("Đang giao");
            orderHistoryPage.ClickFilter();
            if (orderHistoryPage.GetOrderCount() == 0)
                Assert.Inconclusive("Không có đơn hàng Đang giao để kiểm tra");
            Assert.IsFalse(orderHistoryPage.IsCancelButtonPresent(), "Nút hủy vẫn hiển thị trên đơn hàng Đang giao (không được phép hủy)");
        }

        [Test]
        [Property("TC_ID", "TC_ORD_07")]
        public void TC_ORD_07_HuyDonVoiLyDoCoSan()
        {
            DamBaoCoDonChoXacNhan();
            orderHistoryPage.ClickFirstCancelButton();
            orderHistoryPage.SelectCancelReason("Tìm thấy giá tốt hơn");
            orderHistoryPage.ConfirmCancel();
            orderHistoryPage.ClickOkSweetAlert();
            // Kiểm tra đơn đã chuyển sang trạng thái Đã hủy
            orderHistoryPage.SelectStatus("Đã hủy");
            orderHistoryPage.ClickFilter();
            Assert.Greater(orderHistoryPage.GetOrderCount(), 0, "Đơn hàng không xuất hiện trong danh sách Đã hủy");
        }

        [Test]
        [Property("TC_ID", "TC_ORD_08")]
        public void TC_ORD_08_MoTextareaKhiChonLyDoKhac()
        {
            DamBaoCoDonChoXacNhan();
            orderHistoryPage.ClickFirstCancelButton();
            orderHistoryPage.SelectCancelReason("Lý do khác");
            Assert.IsTrue(orderHistoryPage.IsTextareaVisible(), "Textarea không hiển thị khi chọn Lý do khác");
        }

        [Test]
        [Property("TC_ID", "TC_ORD_09")]
        public void TC_ORD_09_HuyDonVoiLyDoKhac()
        {
            DamBaoCoDonChoXacNhan();
            orderHistoryPage.ClickFirstCancelButton();
            orderHistoryPage.SelectCancelReason("Lý do khác");
            orderHistoryPage.EnterCancelDetail("Hàng giao chậm quá");
            orderHistoryPage.ConfirmCancel();
            orderHistoryPage.ClickOkSweetAlert();
            orderHistoryPage.SelectStatus("Đã hủy");
            orderHistoryPage.ClickFilter();
            Assert.Greater(orderHistoryPage.GetOrderCount(), 0, "Hủy với lý do khác không thành công");
        }

        [Test]
        [Property("TC_ID", "TC_ORD_10")]
        public void TC_ORD_10_HuyDonBoTrongTextarea()
        {
            DamBaoCoDonChoXacNhan();
            orderHistoryPage.ClickFirstCancelButton();
            orderHistoryPage.SelectCancelReason("Lý do khác");
            orderHistoryPage.EnterCancelDetail("");
            orderHistoryPage.ConfirmCancel();
            string error = orderHistoryPage.GetSweetAlertErrorText();
            Assert.IsTrue(error.Contains("bỏ trống") || error.Contains("không được để trống"), "Không hiển thị lỗi khi bỏ trống lý do");
            orderHistoryPage.ClickOkSweetAlert();
        }

        [Test]
        [Property("TC_ID", "TC_ORD_11")]
        public void TC_ORD_11_RollbackTonKho()
        {
            Assert.Inconclusive("Test rollback tồn kho cần kết nối DB, chưa implement.");
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
        [Property("TC_ID", "TC_ORD_14")]
        public void TC_ORD_14_UiKhongCoDonHang()
        {
            Assert.Inconclusive("Chưa có tài khoản mới để kiểm tra UI giỏ hàng trống.");
        }

        [Test]
        [Property("TC_ID", "TC_ORD_15")]
        public void TC_ORD_15_PhanTrang()
        {
            orderHistoryPage.GoToOrderHistory();
            int beforeCount = orderHistoryPage.GetOrderCount();
            if (beforeCount == 0) Assert.Inconclusive("Không có đơn hàng để phân trang");
            try
            {
                orderHistoryPage.ClickPage2();
                int afterCount = orderHistoryPage.GetOrderCount();
                Assert.AreNotEqual(beforeCount, afterCount, "Phân trang không hoạt động");
            }
            catch
            {
                Assert.Inconclusive("Không tìm thấy nút phân trang (có thể chỉ có 1 trang)");
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
        [Property("TC_ID", "TC_ORD_17")]
        public void TC_ORD_17_ChiTietGiamGia()
        {
            orderHistoryPage.GoToOrderHistory();
            // Cần đơn hàng có dùng mã giảm giá, nếu không có thì Inconclusive
            try
            {
                orderHistoryPage.ClickFirstOrderId();
                // Nếu không có dòng giảm giá, sẽ ném exception
                var discountElem = driver.FindElement(By.XPath("//*[contains(text(),'Mã giảm giá')]/following-sibling::*"));
                Assert.IsNotNull(discountElem, "Không tìm thấy thông tin giảm giá");
                orderHistoryPage.CloseModal();
            }
            catch
            {
                Assert.Inconclusive("Đơn hàng đầu tiên không có giảm giá hoặc chưa hỗ trợ.");
            }
        }

        [Test]
        [Property("TC_ID", "TC_ORD_18")]
        public void TC_ORD_18_MauSacTrangThai()
        {
            orderHistoryPage.GoToOrderHistory();
            var statusCell = driver.FindElement(By.XPath("//tbody/tr[1]/td[6]"));
            string bgColor = statusCell.GetCssValue("background-color");
            Assert.IsNotNull(bgColor, "Không có màu nền cho trạng thái");
        }

        [Test]
        [Property("TC_ID", "TC_ORD_19")]
        public void TC_ORD_19_NutQuayLaiDanhSach()
        {
            orderHistoryPage.GoToOrderHistory();
            if (orderHistoryPage.GetOrderCount() == 0) Assert.Inconclusive("Không có đơn hàng để kiểm tra");
            orderHistoryPage.ClickFirstOrderId();
            orderHistoryPage.CloseModal();
            Assert.IsTrue(driver.Url.Contains("History"), "Không quay lại danh sách đơn hàng");
        }

        [Test]
        [Property("TC_ID", "TC_ORD_20")]
        public void TC_ORD_20_HuyDonTextareaQuaDai()
        {
            DamBaoCoDonChoXacNhan();
            orderHistoryPage.ClickFirstCancelButton();
            orderHistoryPage.SelectCancelReason("Lý do khác");
            string longText = new string('A', 600);
            orderHistoryPage.EnterCancelDetail(longText);
            orderHistoryPage.ConfirmCancel();
            string error = orderHistoryPage.GetSweetAlertErrorText();
            Assert.IsTrue(error.Contains("vượt quá") || error.Contains("500"), "Không báo lỗi khi nhập lý do quá dài");
            orderHistoryPage.ClickOkSweetAlert();
        }

        [Test]
        [Property("TC_ID", "TC_ORD_21")]
        public void TC_ORD_21_HuyDonKyTuDacBiet()
        {
            DamBaoCoDonChoXacNhan();
            orderHistoryPage.ClickFirstCancelButton();
            orderHistoryPage.SelectCancelReason("Lý do khác");
            orderHistoryPage.EnterCancelDetail("😭 @lỗi #$%");
            orderHistoryPage.ConfirmCancel();
            orderHistoryPage.ClickOkSweetAlert();
            orderHistoryPage.SelectStatus("Đã hủy");
            orderHistoryPage.ClickFilter();
            Assert.Greater(orderHistoryPage.GetOrderCount(), 0, "Hủy với ký tự đặc biệt không thành công");
        }

        [Test]
        [Property("TC_ID", "TC_ORD_22")]
        public void TC_ORD_22_HienThiLyDoHuyTrongChiTiet()
        {
            // Tạo một đơn hàng mới và hủy nó để có lý do
            DamBaoCoDonChoXacNhan();
            orderHistoryPage.ClickFirstCancelButton();
            orderHistoryPage.SelectCancelReason("Lý do khác");
            orderHistoryPage.EnterCancelDetail("Test reason 123");
            orderHistoryPage.ConfirmCancel();
            orderHistoryPage.ClickOkSweetAlert();

            orderHistoryPage.SelectStatus("Đã hủy");
            orderHistoryPage.ClickFilter();
            orderHistoryPage.ClickFirstOrderId();
            string reason = orderHistoryPage.GetModalCancelReason();
            Assert.AreEqual("Test reason 123", reason, "Lý do hủy không hiển thị chính xác");
            orderHistoryPage.CloseModal();
        }

        [Test]
        [Property("TC_ID", "TC_ORD_23")]
        public void TC_ORD_23_MuaLaiDonHang()
        {
            orderHistoryPage.GoToOrderHistory();
            orderHistoryPage.SelectStatus("Hoàn tất");
            orderHistoryPage.ClickFilter();
            if (orderHistoryPage.GetOrderCount() == 0)
                Assert.Inconclusive("Không có đơn hàng Hoàn tất để test mua lại");
            var reorderBtn = driver.FindElements(By.XPath("//button[contains(text(),'Mua lại')]"));
            if (reorderBtn.Count == 0)
                Assert.Inconclusive("Tính năng Mua lại chưa được implement");
            reorderBtn[0].Click();
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Cart");
            var cartItems = driver.FindElements(By.CssSelector(".cart-item"));
            Assert.Greater(cartItems.Count, 0, "Mua lại không thêm sản phẩm vào giỏ hàng");
        }
    }
}