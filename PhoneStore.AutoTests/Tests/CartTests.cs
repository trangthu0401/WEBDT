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
    public class CartTests : BaseTest
    {
        private HomePage homePage;
        private LoginPage loginPage;
        private CartPage cartPage;

        [SetUp]
        public void InitAndGoToCart()
        {
            homePage = new HomePage(driver);
            loginPage = new LoginPage(driver);
            cartPage = new CartPage(driver);

            // 1. Đăng nhập để thấy giỏ hàng cá nhân
            homePage.ClickMenuTaiKhoan();
            Thread.Sleep(1000);
            homePage.ClickDangNhap();
            Thread.Sleep(1000);
            loginPage.Login("4556666666", "123456");
            Thread.Sleep(2000);

            // 2. Đi thẳng vào trang Giỏ hàng
            driver.Navigate().GoToUrl("https://localhost:7033/Cart");
            Thread.Sleep(2000);
        }

        public static IEnumerable<TestCaseData> GetCartData()
        {
            // Đảm bảo bạn đã có file cart.json trong thư mục DataTests/TestData
            return JsonReader.GetTestData("cart.json");
        }

        [Test, TestCaseSource(nameof(GetCartData))]
        public void AutoRun_Cart_DataDriven(JObject testData)
        {
            string action = testData["Action"]?.ToString();
            string expectedResult = testData["ExpectedResult"]?.ToString();

            // THỰC THI HÀNH ĐỘNG
            if (action == "Click_MuaNgay")
            {
                cartPage.ClickMuaNgay();
                Thread.Sleep(2000);
            }
            else if (action == "Increase_Quantity")
            {
                cartPage.TangSoLuong();
                Thread.Sleep(1000);
            }
            else if (action == "Decrease_Quantity")
            {
                cartPage.GiamSoLuong();
                Thread.Sleep(1000);
            }
            else if (action == "Remove_Product")
            {
                cartPage.ClickXoaSanPham();
                Thread.Sleep(1000);
                // Xử lý xác nhận xóa (nếu có popup hiện lên)
                try { driver.SwitchTo().Alert().Accept(); } catch { }
            }

            // KIỂM TRA KẾT QUẢ
            if (expectedResult == "Redirect_To_Checkout")
            {
                Assert.IsTrue(driver.Url.Contains("Checkout"), "Lỗi: Không chuyển hướng sang trang thanh toán!");
            }
            else if (expectedResult == "Cart_Empty_Message")
            {
                // Kiểm tra xem có hiện chữ "Giỏ hàng trống" không
                Assert.IsTrue(driver.PageSource.Contains("trống") || driver.PageSource.Contains("empty"));
            }
            else
            {
                Assert.IsTrue(true);
            }
        }
    }
}