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

        private const int DEFAULT_PRODUCT_ID = 1;

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
                cartPage.ConfirmXoaSweetAlert();
                Thread.Sleep(1000);
            }

            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/OrderCustomer/Checkout");
            Thread.Sleep(2000);

            Assert.That(driver.Url.ToLower(), Does.Contain("cart"),
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

            checkoutPage.DongThongBao();
            checkoutPage.SelectAddressByStreet(data["Address"].ToString());

            checkoutPage.SelectPaymentMethod(isCOD: true);
            checkoutPage.ClickDatHang();
            checkoutPage.DongThongBao();

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.Url.ToLower().Contains("/home/index"));

            Assert.That(driver.Url.ToLower(), Does.Contain("/home/index"),
                "Lỗi: Đặt hàng không chuyển về trang chủ!");
        }

        [Test]
        [Property("TC_ID", "TC_CHK_11")]
        public void TC_CHK_11_BoTrongPhuongXa_KhongChoLuuDiaChi()
        {
            var data = JsonReader.GetTestRow("checkout.json", "TC_CHK_11");

            ThemDoVaoGioVaVaoCart();
            cartPage.ClickMuaNgay();
            Thread.Sleep(2000);

            checkoutPage.EnterFullName(data["FullName"].ToString());
            checkoutPage.EnterPhone(data["Phone"].ToString());

            checkoutPage.ClickThayDoiDiaChi();
            checkoutPage.ClickThemDiaChiMoi();

            checkoutPage.SelectProvince(data["Province"].ToString());
            checkoutPage.SelectDistrict(data["District"].ToString());
            checkoutPage.EnterStreetDetail(data["Street"].ToString());
            checkoutPage.ClickLuuDiaChi();
            Thread.Sleep(1500);

            bool isErrorDisplayed = checkoutPage.IsWardMissingErrorDisplayed();
            Assert.That(isErrorDisplayed, Is.True, "Hệ thống không cảnh báo khi thiếu Phường/Xã!");
        }

        [Test]
        [Property("TC_ID", "TC_CHK_24")]
        public void TC_CHK_24_ThongTinTuDongDien_TuProfile_Readonly()
        {
            ThemDoVaoGioVaVaoCart();
            cartPage.ClickMuaNgay();
            Thread.Sleep(2000);

            string expectedFullName = "Nguyễn Văn A";
            string expectedPhone = "0901234567";

            Assert.Multiple(() =>
            {
                Assert.That(checkoutPage.GetFullNameValue(), Is.EqualTo(expectedFullName), "Họ tên không khớp với profile!");
                Assert.That(checkoutPage.IsFullNameReadOnly(), Is.True, "Trường Họ tên không bị readonly!");
                Assert.That(checkoutPage.GetPhoneValue(), Is.EqualTo(expectedPhone), "SĐT không khớp với profile!");
                Assert.That(checkoutPage.IsPhoneReadOnly(), Is.True, "Trường SĐT không bị readonly!");
            });
        }

        [Test]
        [Property("TC_ID", "TC_CHK_26")]
        public void TC_CHK_26_LoadDanhSachQuanKhiChonTinh()
        {
            ThemDoVaoGioVaVaoCart();
            cartPage.ClickMuaNgay();
            Thread.Sleep(2000);

            checkoutPage.ClickThayDoiDiaChi();
            checkoutPage.ClickThemDiaChiMoi();
            checkoutPage.SelectProvince("Thành phố Hà Nội");

            Assert.That(checkoutPage.IsDistrictListLoaded(), Is.True, "Dropdown Quận/Huyện không được load dữ liệu!");
        }

        [Test]
        [Property("TC_ID", "TC_CHK_33")]
        public void TC_CHK_33_ThieuTinh_ChanDatHang()
        {
            ThemDoVaoGioVaVaoCart();
            cartPage.ClickMuaNgay();
            Thread.Sleep(2000);

            checkoutPage.ClickDatHang();
            Thread.Sleep(1500);

            Assert.That(checkoutPage.IsProvinceMissingErrorDisplayed(), Is.True,
                "Hệ thống không báo lỗi khi thiếu Tỉnh/Thành phố!");
        }

        [Test]
        [Property("TC_ID", "TC_SEC_06")]
        public void TC_SEC_06_ChanTruyCapCheckout_KhiGioTrong()
        {
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Cart");
            while (driver.FindElements(By.CssSelector(".btn-trash, .btn-remove")).Count > 0)
            {
                cartPage.ClickXoaSanPham();
                cartPage.ConfirmXoaSweetAlert();
                Thread.Sleep(1000);
            }

            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/OrderCustomer/Checkout");
            Thread.Sleep(2000);

            Assert.That(driver.Url.ToLower(), Does.Contain("cart").Or.Contains("home"),
                "Lỗi bảo mật: Vẫn vào được Checkout khi giỏ hàng trống!");
        }
    }
}