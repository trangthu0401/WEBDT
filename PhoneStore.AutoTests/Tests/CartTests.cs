using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using PhoneStore.AutoTests.Utilities;
using System;
using System.Threading;

namespace PhoneStore.AutoTests.Tests
{
    [TestFixture]
    public class CartTests : BaseTest
    {
        private LoginPage loginPage;
        private CartPage cartPage;
        private ProductPage productPage;
        private WebDriverWait wait;

        private string inStockProductId = "5";

        [SetUp]
        public void InitAndGoToCart()
        {
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            loginPage = new LoginPage(driver);
            cartPage = new CartPage(driver);
            productPage = new ProductPage(driver);

            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Account/Login");
            loginPage.Login(ConfigHelper.TestUserPhone, ConfigHelper.TestUserPassword);
            Thread.Sleep(1000);

            // Xóa hết sản phẩm trong giỏ
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Cart");
            Thread.Sleep(1000);
            while (driver.PageSource.Contains("btn-trash"))
            {
                cartPage.ClickXoaSanPham();
                cartPage.ConfirmXoaSweetAlert();
                Thread.Sleep(1000);
            }

            // Thêm sản phẩm mới
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Home/ProductDetail/{inStockProductId}");
            Thread.Sleep(1000);
            productPage.ClickThemVaoGio();
            Thread.Sleep(1000);
            productPage.AcceptAlert();

            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Cart");
            Thread.Sleep(1000);
        }

        [Test]
        [Property("TC_ID", "TC_CART_01")]
        public void TC_CART_01_ThemTrungSanPham_PhaiCongDonSoLuong()
        {
            int initialQty = cartPage.GetSoLuongHienTai();

            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Home/ProductDetail/{inStockProductId}");
            Thread.Sleep(1500);
            productPage.ClickThemVaoGio();
            Thread.Sleep(1500);
            productPage.AcceptAlert();

            Thread.Sleep(3000);
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Cart");
            Thread.Sleep(2000);

            int newQty = cartPage.GetSoLuongHienTai();

            Assert.That(newQty, Is.GreaterThan(initialQty),
                $"Lỗi: Thêm trùng sản phẩm nhưng số lượng không cộng dồn! (Trước: {initialQty}, Sau: {newQty})");
        }

        [Test]
        [Property("TC_ID", "TC_CART_02")]
        public void TC_CART_02_NhanNutTangSoLuong_TongTienPhaiTangLen()
        {
            string priceBefore = cartPage.GetTongTienHienTai();
            cartPage.TangSoLuong();
            Thread.Sleep(2000);
            string priceAfter = cartPage.GetTongTienHienTai();

            Assert.That(priceAfter, Is.Not.EqualTo(priceBefore),
                "Lỗi: Đã bấm tăng (+) số lượng nhưng Tổng tiền không nhúc nhích!");
        }

        [Test]
        [Property("TC_ID", "TC_CART_03")]
        public void TC_CART_03_NhanNutGiamSoLuong_TongTienPhaiGiamXuong()
        {
            cartPage.TangSoLuong();
            Thread.Sleep(2000);
            string priceBefore = cartPage.GetTongTienHienTai();
            cartPage.GiamSoLuong();
            Thread.Sleep(2000);
            string priceAfter = cartPage.GetTongTienHienTai();

            Assert.That(priceAfter, Is.Not.EqualTo(priceBefore),
                "Lỗi: Đã bấm giảm (-) số lượng nhưng Tổng tiền không thay đổi!");
        }

        [Test]
        [Property("TC_ID", "TC_CART_05")]
        public void TC_CART_05_XoaSanPham_HienThiThongBaoGioHangTrong()
        {
            cartPage.ClickXoaSanPham();
            Thread.Sleep(1000);
            cartPage.ConfirmXoaSweetAlert();
            Thread.Sleep(2000);

            string pageText = driver.PageSource.ToLower();
            bool isEmpty = pageText.Contains("trống") || pageText.Contains("empty") || pageText.Contains("chưa có sản phẩm");

            Assert.That(isEmpty, Is.True, "Lỗi: Đã xóa thành công nhưng không thấy thông báo Giỏ hàng trống!");
        }

        [Test]
        [Property("TC_ID", "TC_CART_07")]
        public void TC_CART_07_TangSoLuong_VuotQuaTonKho_PhaiBaoLoi()
        {
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Home/ProductDetail/{inStockProductId}");
            int stock = productPage.GetStockQuantity();
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Cart");
            Thread.Sleep(500);

            int currentQty = cartPage.GetSoLuongHienTai();
            int maxClick = stock - currentQty + 1;
            if (maxClick <= 0) maxClick = 5;

            bool hasError = false;
            for (int i = 0; i < maxClick; i++)
            {
                cartPage.TangSoLuong();
                try
                {
                    var sweetAlert = driver.FindElement(By.CssSelector(".swal2-container"));
                    if (sweetAlert.Displayed)
                    {
                        hasError = true;
                        break;
                    }
                }
                catch { }

                try
                {
                    var alert = driver.SwitchTo().Alert();
                    hasError = true;
                    alert.Accept();
                    break;
                }
                catch { }
            }

            int finalQty = cartPage.GetSoLuongHienTai();
            if (!hasError && finalQty > stock)
                hasError = false;
            else if (!hasError && finalQty <= stock)
                Assert.Inconclusive($"Chưa vượt quá tồn kho (stock={stock}, finalQty={finalQty}). Cần kiểm tra lại dữ liệu.");

            Assert.That(hasError, Is.True, "Lỗi: Bấm tăng số lượng quá tồn kho nhưng Web không báo lỗi (không alert, không sweetalert)!");
        }

        [Test]
        [Property("TC_ID", "TC_CHK_25")]
        public void TC_CHK_25_GioiHanSoLuongTrongGio_KhongVuotStock()
        {
            // Giả sử sản phẩm ID = 2 có stock = 2 (cần đảm bảo stock thực tế)
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Home/ProductDetail/2");
            productPage.ClickThemVaoGio();
            productPage.AcceptAlert();
            productPage.ClickThemVaoGio();
            productPage.AcceptAlert();

            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Cart");
            Thread.Sleep(1000);

            var qtyInput = driver.FindElement(By.CssSelector(".qty-input, input[name*='quantity']"));
            int oldQty = int.Parse(qtyInput.GetAttribute("value"));

            cartPage.TangSoLuong();
            Thread.Sleep(500);

            int newQty = int.Parse(qtyInput.GetAttribute("value"));
            bool hasAlert = false;
            try
            {
                driver.SwitchTo().Alert().Accept();
                hasAlert = true;
            }
            catch { }

            bool condition = (newQty == oldQty) || hasAlert;
            Assert.That(condition, Is.True, "Vượt quá stock nhưng vẫn tăng số lượng được!");
        }
        // ==================== BỔ SUNG 2 TEST CASE ====================

        [Test]
        [Property("TC_ID", "TC_CART_04")]
        public void TC_CART_04_XoaSanPham_HuyBo_SanPhamVanConTrongGio()
        {
            // Chuẩn bị: giỏ hàng có 2 sản phẩm (trong SetUp đã có 1, cần thêm 1 sản phẩm khác)
            // Thêm sản phẩm thứ 2 (ví dụ productId=6)
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Home/ProductDetail/6");
            Thread.Sleep(1000);
            productPage.ClickThemVaoGio();
            Thread.Sleep(1000);
            productPage.AcceptAlert();
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Cart");
            Thread.Sleep(1000);

            int initialCount = cartPage.GetSoLuongSanPhamTrongGio(); // cần method mới: đếm số dòng sản phẩm

            // Click nút xóa của sản phẩm đầu tiên
            cartPage.ClickXoaSanPham(); // mở popup xác nhận
            Thread.Sleep(500);
            // Nhấn nút "Cancel" hoặc "Không" (giả sử có button Cancel)
            driver.FindElement(By.XPath("//button[contains(text(),'Hủy')] | //button[contains(text(),'Cancel')]")).Click();
            Thread.Sleep(1000);

            int afterCount = cartPage.GetSoLuongSanPhamTrongGio();
            Assert.That(afterCount, Is.EqualTo(initialCount), "Số lượng sản phẩm trong giỏ thay đổi sau khi hủy xóa!");
        }

        [Test]
        [Property("TC_ID", "TC_CART_06")]
        public void TC_CART_06_GioHangTrong_HienThiUIRong()
        {
            // Xóa hết sản phẩm trong giỏ (SetUp đã có sẵn 1 SP, cần xóa nó)
            cartPage.ClickXoaSanPham();
            cartPage.ConfirmXoaSweetAlert();
            Thread.Sleep(2000);

            string pageSource = driver.PageSource.ToLower();
            bool hasEmptyMessage = pageSource.Contains("giỏ hàng trống")
                                    || pageSource.Contains("chưa có sản phẩm")
                                    || pageSource.Contains("empty cart")
                                    || driver.FindElements(By.CssSelector(".empty-cart, .cart-empty")).Count > 0;
            Assert.That(hasEmptyMessage, Is.True, "Không hiển thị thông báo hoặc hình ảnh giỏ hàng trống!");
        }
    }
}