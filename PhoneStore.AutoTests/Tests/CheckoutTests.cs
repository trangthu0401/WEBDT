using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
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

        // Sản phẩm mặc định dùng để thêm vào giỏ (chọn ID chắc chắn tồn tại)
        private const int DEFAULT_PRODUCT_ID = 1; // 👈 Thay bằng ID sản phẩm thật của bạn

        [SetUp]
        public void SetupCheckoutEnv()
        {
            loginPage = new LoginPage(driver);
            checkoutPage = new CheckoutPage(driver);
            cartPage = new CartPage(driver);
            productPage = new ProductPage(driver);

            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Account/Login");
            loginPage.Login(ConfigHelper.TestUserPhone, ConfigHelper.TestUserPassword);
            Thread.Sleep(2000);
        }

        /// <summary>
        /// Thêm sản phẩm vào giỏ và chuyển đến trang Cart
        /// </summary>
        private void ThemDoVaoGioVaVaoCart(int productId = DEFAULT_PRODUCT_ID)
        {
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Home/ProductDetail/{productId}");
            Thread.Sleep(1000);
            productPage.ClickThemVaoGio();
            Thread.Sleep(1000);
            productPage.AcceptAlert();
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Cart");
            Thread.Sleep(1500);
        }

        [Test]
        [Property("TC_ID", "TC_CHK_01")]
        public void TC_CHK_01_ChanVaoCheckoutKhiGioHangTrong()
        {
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Cart");
            while (driver.PageSource.Contains("btn-trash"))
            {
                cartPage.ClickXoaSanPham();
                try { cartPage.ConfirmXoaSweetAlert(); } catch { }
                Thread.Sleep(1000);
            }

            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/OrderCustomer/Checkout");
            Thread.Sleep(2000);

            Assert.IsTrue(driver.Url.ToLower().Contains("cart"),
                "Lỗi: Giỏ hàng trống nhưng vẫn vào trang thanh toán!");
        }
        [Test]
        [Property("TC_ID", "TC_CHK_02")]
        public void TC_CHK_02_QuyTrinhDatHangChuan_TuGioHang()
        {
            var data = JsonReader.GetTestRow("checkout.json", "TC_CHK_02");

            ThemDoVaoGioVaVaoCart();
            cartPage.ClickMuaNgay();
            Thread.Sleep(2000);

            checkoutPage.EnterFullName(data["FullName"].ToString());
            checkoutPage.EnterPhone(data["Phone"].ToString());

            checkoutPage.ClickThayDoiDiaChi();
            checkoutPage.ClickThemDiaChiMoi();

            checkoutPage.ChonDiaChiFull(
                data["Province"].ToString(),
                data["District"].ToString(),
                data["Ward"].ToString(),
                data["Address"].ToString()
            );

            checkoutPage.DongThongBao();        // đóng sweetalert lưu địa chỉ
            checkoutPage.SelectAddressByStreet(data["Address"].ToString());

            // 6. Chọn COD và đặt hàng
            checkoutPage.SelectPaymentMethod(isCOD: true);
            checkoutPage.ClickDatHang();
            checkoutPage.DongThongBao(); // đóng thông báo đặt hàng thành công

            // 7. Chờ chuyển về trang chủ
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.Url.ToLower().Contains("/home/index"));

            Assert.IsTrue(driver.Url.ToLower().Contains("/home/index"),
                "Lỗi: Đặt hàng không chuyển về trang chủ!");

        }

        [Test]
        [Property("TC_ID", "TC_CHK_11")]
        public void TC_CHK_11_BoTrongPhuongXa_KhongChoLuuDiaChi()
        {
            var data = JsonReader.GetTestRow("checkout.json", "TC_CHK_11");

            // 1. Thêm sản phẩm vào giỏ và vào checkout
            ThemDoVaoGioVaVaoCart();
            cartPage.ClickMuaNgay();
            Thread.Sleep(2000);

            // 2. Nhập họ tên, SĐT (lấy từ JSON hoặc default)
            string fullName = data.ContainsKey("FullName") ? data["FullName"].ToString() : "Nguyễn Văn A";
            string phone = data.ContainsKey("Phone") ? data["Phone"].ToString() : "0901234567";
            checkoutPage.EnterFullName(fullName);
            checkoutPage.EnterPhone(phone);

            // 3. Mở modal địa chỉ và thêm mới
            checkoutPage.ClickThayDoiDiaChi();
            checkoutPage.ClickThemDiaChiMoi();

            // 4. Chọn tỉnh, huyện, điền số nhà nhưng bỏ qua phường/xã
            checkoutPage.SelectProvince(data["Province"].ToString());
            checkoutPage.SelectDistrict(data["District"].ToString());
            checkoutPage.EnterStreetDetail(data["Street"].ToString());
            checkoutPage.ClickLuuDiaChi();
            Thread.Sleep(1500);

            // 5. Kiểm tra cảnh báo lỗi
            bool isErrorDisplayed = false;
            try
            {
                string alertText = driver.SwitchTo().Alert().Text.ToLower();
                isErrorDisplayed = alertText.Contains("phường") || alertText.Contains("xã") || alertText.Contains("chọn");
                driver.SwitchTo().Alert().Accept();
            }
            catch
            {
                string pageText = driver.PageSource.ToLower();
                isErrorDisplayed = pageText.Contains("phường") || pageText.Contains("xã") || pageText.Contains("chọn");
            }

            Assert.IsTrue(isErrorDisplayed, "Lỗi: Hệ thống không cảnh báo khi thiếu Phường/Xã!");
        }
    }
}