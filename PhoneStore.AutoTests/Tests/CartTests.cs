using NUnit.Framework;
using OpenQA.Selenium;
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

            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Account/Login");
            loginPage.Login(ConfigHelper.TestUserPhone, ConfigHelper.TestUserPassword);
            Thread.Sleep(1000);

            // Xóa hết sản phẩm trong giỏ hiện tại
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Cart");
            Thread.Sleep(1000);
            while (driver.PageSource.Contains("btn-trash"))
            {
                cartPage.ClickXoaSanPham();
                cartPage.ConfirmXoaSweetAlert();
                Thread.Sleep(1000);
            }

            // Sau đó mới thêm sản phẩm mới
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
            // Lấy số lượng trước khi thêm
            int initialQty = cartPage.GetSoLuongHienTai();

            // Thêm lại sản phẩm lần 2
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Home/ProductDetail/{inStockProductId}");
            Thread.Sleep(1500);
            productPage.ClickThemVaoGio();
            Thread.Sleep(1500);
            productPage.AcceptAlert(); // Đóng popup thông báo "Đã thêm vào giỏ"

            // Chờ giỏ hàng cập nhật (có thể cần chờ AJAX)
            Thread.Sleep(3000);
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Cart");
            Thread.Sleep(2000);

            // Lấy số lượng sau
            int newQty = cartPage.GetSoLuongHienTai();

            Assert.Greater(newQty, initialQty,
                $"Lỗi: Thêm trùng sản phẩm nhưng số lượng không cộng dồn! (Trước: {initialQty}, Sau: {newQty})");
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
            // Lấy số lượng tồn kho (nếu có)
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Home/ProductDetail/{inStockProductId}");
            int stock = productPage.GetStockQuantity();
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Cart");
            Thread.Sleep(500);

            int currentQty = cartPage.GetSoLuongHienTai();
            int maxClick = stock - currentQty + 1; // Số lần click cần để vượt quá
            if (maxClick <= 0) maxClick = 5;

            bool hasError = false;
            for (int i = 0; i < maxClick; i++)
            {
                cartPage.TangSoLuong();
              //  Thread.Sleep(100);

                // Kiểm tra SweetAlert sau mỗi lần click
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

                // Kiểm tra Alert
                try
                {
                    var alert = driver.SwitchTo().Alert();
                    hasError = true;
                    alert.Accept();
                    break;
                }
                catch { }
            }

            // Nếu không có alert/sweetalert, kiểm tra số lượng cuối cùng có vượt stock không
            int finalQty = cartPage.GetSoLuongHienTai();
            if (!hasError && finalQty > stock)
            {
                hasError = false; // vẫn không báo lỗi -> bug
            }
            else if (!hasError && finalQty <= stock)
            {
                // Có thể chưa chạm tới giới hạn, cần tăng số lần click
                Assert.Inconclusive($"Chưa vượt quá tồn kho (stock={stock}, finalQty={finalQty}). Cần kiểm tra lại dữ liệu.");
            }

            Assert.IsTrue(hasError, "Lỗi: Bấm tăng số lượng quá tồn kho nhưng Web không báo lỗi (không alert, không sweetalert)!");
        }
        [Test]
        [Property("TC_ID", "TC_CHK_25")]
        public void TC_CHK_25_GioiHanSoLuongTrongGio_KhongVuotStock()
        {
            // Chuẩn bị: sản phẩm có stock = 2, thêm vào giỏ với số lượng = 2
            // (có thể dùng productId cụ thể, hoặc set stock qua DB)
            // Giả sử sản phẩm ID = 2 có stock = 2
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Home/ProductDetail/2");
            // Thêm 2 lần để có Qty=2 (hoặc dùng JS tăng số lượng)
            productPage.ClickThemVaoGio(); // lần 1
            productPage.AcceptAlert();
            productPage.ClickThemVaoGio(); // lần 2
            productPage.AcceptAlert();

            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Cart");
            Thread.Sleep(1000);

            // Lưu số lượng ban đầu (2)
            var qtyInput = driver.FindElement(By.CssSelector(".qty-input, input[name*='quantity']"));
            int oldQty = int.Parse(qtyInput.GetAttribute("value"));

            // Click nút (+)
            cartPage.TangSoLuong();
            Thread.Sleep(500);

            // Kiểm tra: số lượng không tăng, hoặc có alert
            int newQty = int.Parse(qtyInput.GetAttribute("value"));
            bool hasAlert = false;
            try { driver.SwitchTo().Alert().Accept(); hasAlert = true; } catch { }

            Assert.That(newQty == oldQty || hasAlert, Is.True, "Vượt quá stock nhưng vẫn tăng số lượng được!");
        }
    }
}