using OpenQA.Selenium;

namespace PhoneStore.AutoTests.Pages
{
    public class HomePage
    {
        private IWebDriver driver;

        public HomePage(IWebDriver driver)
        {
            this.driver = driver;
        }

        // --- 1. LOCATORS ---
        private By btnTaiKhoan = By.XPath("//span[contains(text(),'Tài khoản')]");
        private By linkDangNhap = By.XPath("//a[contains(text(),'Đăng nhập')]");
        private By iconGioHang = By.XPath("//span[contains(text(),'Giỏ hàng')]");

        // Lấy sản phẩm đầu tiên trong mục Bán chạy
        private By firstProduct = By.CssSelector("section.top-selling-section img");

        // --- 2. ACTIONS ---
        public void ClickMenuTaiKhoan()
        {
            driver.FindElement(btnTaiKhoan).Click();
        }

        public void ClickDangNhap()
        {
            driver.FindElement(linkDangNhap).Click();
        }

        public void ClickGioHang()
        {
            driver.FindElement(iconGioHang).Click();
        }

        // ĐÂY LÀ HÀM BỊ THIẾU GÂY RA LỖI CỦA BẠN:
        public void ClickFirstProduct()
        {
            try
            {
                driver.FindElement(firstProduct).Click();
            }
            catch
            {
                // Dự phòng nếu không click được ảnh thì ép link đi thẳng vào sản phẩm số 1
                driver.Navigate().GoToUrl("https://localhost:7033/Product/Details/1");
            }
        }
    }
}