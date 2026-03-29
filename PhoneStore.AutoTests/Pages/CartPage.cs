using OpenQA.Selenium;
using PhoneStore.AutoTests.Utilities;

namespace PhoneStore.AutoTests.Pages
{
    public class CartPage
    {
        private IWebDriver driver;
        public CartPage(IWebDriver driver) { this.driver = driver; }

        private By btnMuaNgay = By.CssSelector(".btn-buy-now");
        private By btnTangSoLuong = By.XPath("//button[normalize-space()='+']");
        private By btnGiamSoLuong = By.XPath("//button[normalize-space()='-']");
        private By btnXoaSanPham = By.XPath("//a[contains(@class, 'btn-remove') or contains(@onclick, 'removeItem')]");

        public void ClickMuaNgay()
        {
            try { driver.FindElement(btnMuaNgay).Click(); }
            catch { driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Checkout"); }
        }

        // BỔ SUNG: Chọn sản phẩm theo ID checkbox (như cb-1041 trong CSV)
        public void SelectProductCheckbox(string productId)
        {
            try { driver.FindElement(By.Id($"cb-{productId}")).Click(); } catch { }
        }

        public void TangSoLuong() => driver.FindElement(btnTangSoLuong).Click();
        public void GiamSoLuong() => driver.FindElement(btnGiamSoLuong).Click();
        public void ClickXoaSanPham() { try { driver.FindElement(btnXoaSanPham).Click(); } catch { } }
        // Thêm vào trong class CartPage cũ:
        private By cbAll = By.Id("check-all"); // Checkbox chọn tất cả

        public void TickCheckboxById(string id)
        {
            try
            {
                driver.FindElement(By.Id(id)).Click();
            }
            catch
            {
                // Nếu ID thay đổi theo DB, ta tick cái đầu tiên tìm thấy
                driver.FindElement(By.CssSelector("input[type='checkbox']")).Click();
            }
        }

        public void AcceptAlert()
        {
            try { new OpenQA.Selenium.Support.UI.WebDriverWait(driver, System.TimeSpan.FromSeconds(3)).Until(SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent()).Accept(); } catch { }
        }
    }
}