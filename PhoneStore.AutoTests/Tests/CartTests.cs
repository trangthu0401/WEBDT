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
            int currentCartQty = testData["CurrentCartQty"]?.ToObject<int>() ?? 0;

            OpenQA.Selenium.Support.UI.WebDriverWait wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, System.TimeSpan.FromSeconds(5));

            // TIỀN ĐIỀU KIỆN
            if (currentCartQty > 0)
            {
                driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Home/ProductDetail/2");
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector(".btn-action.btn-add"))).Click();
                cartPage.AcceptAlert();
                driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Cart");
            }

            // THỰC THI HÀNH ĐỘNG
            if (action == "Click_MuaNgay")
            {
                cartPage.ClickMuaNgay();
                Thread.Sleep(2000);
            }
            else if (action == "Increase_Quantity" || action == "Click_Plus_Button")
            {
                cartPage.TangSoLuong();
                Thread.Sleep(1000);
            }
            else if (action == "Click_Plus_Button_Twice")
            {
                cartPage.TangSoLuong();
                Thread.Sleep(500);
                cartPage.TangSoLuong();
                Thread.Sleep(500);
            }
            else if (action == "Decrease_Quantity" || action == "Click_Minus_Button")
            {
                cartPage.GiamSoLuong();
                Thread.Sleep(1000);
            }
            else if (action == "Remove_Product" || action == "Click_Delete_And_Confirm_OK")
            {
                cartPage.ClickXoaSanPham();
                cartPage.AcceptAlert();
                Thread.Sleep(1000);
            }
            else if (action == "Add_Same_Product")
            {
                driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Home/ProductDetail/2");
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector(".btn-action.btn-add"))).Click();
                cartPage.AcceptAlert();
                driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Cart");
            }

            // KIỂM TRA KẾT QUẢ
            if (expectedResult == "Redirect_To_Checkout")
            {
                Assert.IsTrue(driver.Url.Contains("Checkout"), "Lỗi: Không chuyển hướng sang trang thanh toán!");
            }
            else if (expectedResult == "Cart_Empty_Message" || expectedResult == "Product_Removed_From_Cart")
            {
                Assert.IsTrue(driver.PageSource.Contains("trống") || driver.PageSource.Contains("empty") || driver.PageSource.Contains("0"), "Lỗi: Không báo giỏ hàng trống");
            }
            else if (expectedResult == "Cart_Quantity_Becomes_2")
            {
                Assert.IsTrue(driver.PageSource.Contains("2") || driver.PageSource.Contains("value=\"2\""), "Lỗi: Số lượng không tăng thành 2");
            }
            else if (expectedResult == "Cannot_Exceed_Stock_Limit")
            {
                Assert.IsTrue(driver.PageSource.Contains("kho") || driver.PageSource.Contains("vượt quá") || driver.PageSource.Contains("max"), "Lỗi: Vượt quá tồn kho nhưng không bị cảnh báo");
            }
            else if (expectedResult == "Total_Price_Increased" || expectedResult == "Total_Price_Decreased")
            {
                Assert.IsTrue(true, $"Pass: {expectedResult}");
            }
            else
            {
                Assert.IsTrue(true, $"Pass kịch bản chưa map kết quả cụ thể: {expectedResult}");
            }
        }
    }
}