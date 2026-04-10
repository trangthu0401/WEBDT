using NUnit.Framework;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using PhoneStore.AutoTests.Utilities;
using System.Threading;

namespace PhoneStore.AutoTests.Tests
{
    [TestFixture]
    public class CartTests : BaseTest
    {
        private LoginPage loginPage;
        private CartPage cartPage;
        private ProductPage productPage;

        // Dùng đúng con Samsung S24 Ultra (ID=5) trong file record CSV của bạn
        private string inStockProductId = "5";

        [SetUp]
        public void InitAndGoToCart()
        {
            loginPage = new LoginPage(driver);
            cartPage = new CartPage(driver);
            productPage = new ProductPage(driver);

            // 1. Đăng nhập
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Account/Login");
            loginPage.Login(ConfigHelper.TestUserPhone, ConfigHelper.TestUserPassword);
            Thread.Sleep(1500);

            // 2. MỒI DỮ LIỆU: Thêm S24 Ultra vào giỏ trước khi test
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Home/ProductDetail/{inStockProductId}");
            Thread.Sleep(1500);

            productPage.ClickThemVaoGio();
            Thread.Sleep(1500);
            productPage.AcceptAlert(); // Đóng popup thêm thành công (nếu có)

            // 3. Bay thẳng vào trang Giỏ hàng
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Cart");
            Thread.Sleep(1500);
        }

        [Test]
        [Property("TC_ID", "TC_CART_01")]
        public void TC_CART_01_ThemTrungSanPham_PhaiCongDonSoLuong()
        {
            int initialQty = cartPage.GetSoLuongHienTai();

            // Về lại trang S24 Ultra bấm Thêm lần nữa
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Home/ProductDetail/{inStockProductId}");
            Thread.Sleep(1500);

            productPage.ClickThemVaoGio();
            Thread.Sleep(1500);
            productPage.AcceptAlert();

            // Vào lại giỏ hàng kiểm tra
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Cart");
            Thread.Sleep(1500);

            int newQty = cartPage.GetSoLuongHienTai();
            Assert.IsTrue(newQty > initialQty, $"Lỗi: Thêm trùng sản phẩm nhưng số lượng không cộng dồn! (Hiện tại: {newQty})");
        }

        [Test]
        [Property("TC_ID", "TC_CART_02")]
        public void TC_CART_02_NhanNutTangSoLuong_TongTienPhaiTangLen()
        {
            string priceBefore = cartPage.GetTongTienHienTai();

            cartPage.TangSoLuong();
            Thread.Sleep(2000); // Chờ web tính lại giá tiền

            string priceAfter = cartPage.GetTongTienHienTai();

            Assert.AreNotEqual(priceBefore, priceAfter, "Lỗi: Đã bấm tăng (+) số lượng nhưng Tổng tiền không nhúc nhích!");
        }

        [Test]
        [Property("TC_ID", "TC_CART_03")]
        public void TC_CART_03_NhanNutGiamSoLuong_TongTienPhaiGiamXuong()
        {
            // Tăng lên trước để lát có cái mà giảm
            cartPage.TangSoLuong();
            Thread.Sleep(2000);

            string priceBefore = cartPage.GetTongTienHienTai();

            cartPage.GiamSoLuong();
            Thread.Sleep(2000);

            string priceAfter = cartPage.GetTongTienHienTai();

            Assert.AreNotEqual(priceBefore, priceAfter, "Lỗi: Đã bấm giảm (-) số lượng nhưng Tổng tiền không thay đổi!");
        }

        [Test]
        [Property("TC_ID", "TC_CART_05")]
        public void TC_CART_05_XoaSanPham_HienThiThongBaoGioHangTrong()
        {
            cartPage.ClickXoaSanPham();
            Thread.Sleep(1000);

            // XỬ LÝ SWEET ALERT TỪ FILE CSV
            cartPage.ConfirmXoaSweetAlert();
            Thread.Sleep(2000);

            string pageText = driver.PageSource.ToLower();
            bool isEmpty = pageText.Contains("trống") || pageText.Contains("empty") || pageText.Contains("chưa có sản phẩm");

            Assert.IsTrue(isEmpty, "Lỗi: Đã xóa thành công nhưng không thấy thông báo Giỏ hàng trống!");
        }

        [Test]
        [Property("TC_ID", "TC_CART_07")]
        public void TC_CART_07_TangSoLuong_VuotQuaTonKho_PhaiBaoLoi()
        {
            // Cố tình spam nút (+) 10 lần
            for (int i = 0; i < 10; i++)
            {
                cartPage.TangSoLuong();
                Thread.Sleep(300);
            }

            string pageText = driver.PageSource.ToLower();
            bool hasError = pageText.Contains("kho") || pageText.Contains("vượt quá") || pageText.Contains("không đủ") || pageText.Contains("tối đa");

            Assert.IsTrue(hasError, "Lỗi: Bấm tăng số lượng quá tồn kho nhưng Web đứng im không báo lỗi!");
        }
    }
}