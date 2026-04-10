using NUnit.Framework;
using OpenQA.Selenium;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using PhoneStore.AutoTests.Utilities;
using System.Threading;

namespace PhoneStore.AutoTests.Tests
{
    [TestFixture]
    public class CheckoutTests : BaseTest
    {
        private LoginPage loginPage;
        private CheckoutPage checkoutPage;
        private CartPage cartPage;
        private ProductPage productPage;

        [SetUp]
        public void SetupCheckoutEnv()
        {
            loginPage = new LoginPage(driver);
            checkoutPage = new CheckoutPage(driver);
            cartPage = new CartPage(driver);
            productPage = new ProductPage(driver);

            // 1. Đăng nhập
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Account/Login");
            loginPage.Login(ConfigHelper.TestUserPhone, ConfigHelper.TestUserPassword);
            Thread.Sleep(2000);
        }

        // Hàm bổ trợ để đảm bảo giỏ hàng có đồ trước khi bấm Thanh toán
        private void ThemDoVaoGioVaVaoCart()
        {
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Home/ProductDetail/5");
            Thread.Sleep(1000);
            productPage.ClickThemVaoGio();
            Thread.Sleep(1000);
            productPage.AcceptAlert();
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Cart");
            Thread.Sleep(1500);
        }

        [Test]
        [Property("TC_ID", "TC_CHK_02")]
        public void TC_CHK_02_QuyTrinhDatHangChuan_TuGioHang()
        {
            // Bước 1: Vào giỏ hàng
            ThemDoVaoGioVaVaoCart();

            // Bước 2: Từ giỏ hàng bấm nút tiến tới Thanh toán (Mua ngay)
            cartPage.ClickMuaNgay();
            Thread.Sleep(2000);

            // Bước 3: Điền thông tin
            checkoutPage.EnterFullName("Nguyễn Thị Thu Trang");
            checkoutPage.EnterPhone("0901234567");

            // Bước 4: Chọn địa chỉ modal
            checkoutPage.ClickThayDoiDiaChi();
            checkoutPage.ChonDiaChiFull("Hồ Chí Minh", "Quận 1", "Phường Tân Định", "Số 1 Lê Duẩn");
            checkoutPage.ConfirmSweetAlert();
            Thread.Sleep(1000);

            // Bước 5: Đặt hàng
            checkoutPage.SelectPaymentMethod(isCOD: true);
            checkoutPage.ClickDatHang();
            Thread.Sleep(2000);
            checkoutPage.ConfirmSweetAlert(); // Bấm OK ở popup thành công

            Assert.IsTrue(driver.Url.ToLower().Contains("success") || driver.PageSource.Contains("thành công"),
                "Lỗi: Luồng đặt hàng từ Giỏ hàng không thành công!");
        }

        [Test]
        [Property("TC_ID", "TC_CHK_01")]
        public void TC_CHK_01_ChanVaoCheckoutKhiGioHangTrong()
        {
            // Đảm bảo giỏ hàng trống (vào trang giỏ hàng và xóa nếu có đồ)
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Cart");
            while (driver.PageSource.Contains("btn-trash"))
            {
                cartPage.ClickXoaSanPham();
                cartPage.ConfirmXoaSweetAlert();
                Thread.Sleep(1000);
            }

            // Thử truy cập thẳng link Checkout
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Checkout");
            Thread.Sleep(2000);

            // Kỳ vọng bị đá về lại trang Cart vì không có đồ
            Assert.IsTrue(driver.Url.ToLower().Contains("cart"), "Lỗi: Giỏ hàng trống nhưng vẫn cho vào trang Thanh toán!");
        }
        [Test]
        [Property("TC_ID", "TC_CHK_13")]
        public void TC_CHK_13_ValidateForm_HoTenChuaKyTuDacBiet()
        {
            // Bước 1: Mồi đồ vào giỏ và tiến hành thanh toán
            ThemDoVaoGioVaVaoCart();
            cartPage.ClickMuaNgay(); // (Hoặc ClickThanhToan() tùy theo tên hàm hiện tại của bạn)
            Thread.Sleep(2000);

            // Bước 2: Cố tình nhập Họ tên chứa ký tự lạ (dựa theo kịch bản Excel)
            checkoutPage.EnterFullName("Trang @#$% Nguyễn");
            checkoutPage.EnterPhone("0901234567");

            // Xóa sạch các bước chọn địa chỉ / phương thức thanh toán rườm rà
            // Chỉ cần điền tên, sđt rồi bấm Đặt hàng luôn để check Validate
            checkoutPage.ClickDatHang();
            Thread.Sleep(1500);

            // Bước 3: Kiểm tra xem web có văng ra câu chửi/báo lỗi hay không
            string pageSource = driver.PageSource.ToLower();
            bool hasError = pageSource.Contains("ký tự") ||
                            pageSource.Contains("hợp lệ") ||
                            pageSource.Contains("không được chứa") ||
                            pageSource.Contains("định dạng");

            Assert.IsTrue(hasError, "Lỗi: Nhập Họ tên chứa ký tự đặc biệt (@#$%) nhưng hệ thống KHÔNG chặn báo lỗi!");
        }
    }
}