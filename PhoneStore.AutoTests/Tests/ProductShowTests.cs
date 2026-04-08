using NUnit.Framework;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using PhoneStore.AutoTests.Utilities;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using OpenQA.Selenium;

namespace PhoneStore.AutoTests.Tests
{
    [TestFixture]
    public class ProductShowTests : BaseTest
    {
        private LoginPage loginPage;
        private ProductShowPage showPage;
        private WebDriverWait wait;

        [SetUp]
        public void PreTest()
        {
            loginPage = new LoginPage(driver);
            showPage = new ProductShowPage(driver);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            // Đăng nhập Admin
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Account/Login");
            loginPage.Login("admin@shop.com", "admin123", isAdmin: true);
        }

        
        [Test]
        public void TC_PRODUCT_09_ShowVariant_AdminOnly()
        {
            var data = JsonReader.GetTestRow("ProductShowData.json", "TC_PRODUCT_09");
            string colorToShow = (string)data.TargetColor;

            // 1. Vào Admin
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");

            // 2. Vào trang biến thể
            showPage.GoToFirstProductVariant();

            // 3. Bấm nút "Đang bán" và Đóng popup (Hàm này Vy đã có nút Đóng rồi)
            showPage.ClickShowVariant(colorToShow);

            // 4. Thay vì sang User, mình kiểm tra ngay tại bảng Admin
            // Nếu nút "Đang bán" biến mất và thay bằng nút "Ẩn", nghĩa là đã thành công!
            System.Threading.Thread.Sleep(2000);

            // Kiểm tra xem dòng đó giờ đã hiện nút "Ẩn" chưa (nghĩa là nó đang ở trạng thái Đang bán)
            string xpathHideBtn = $"//td[contains(.,'{colorToShow}')]/following-sibling::td//button[contains(.,'Ẩn')]";
            bool isSuccess = driver.FindElements(By.XPath(xpathHideBtn)).Count > 0;

            Assert.That(isSuccess, Is.True, $"Lỗi: Đã bấm Đang bán cho màu {colorToShow} nhưng trạng thái trên bảng Admin không đổi!");

            Console.WriteLine($"PASS: Đã mở bán lại màu {colorToShow} thành công tại Admin!");
        }
    }
}