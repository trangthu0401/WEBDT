using OpenQA.Selenium;

namespace PhoneStore.AutoTests.Pages
{
    public class ProductPage
    {
        private IWebDriver driver;
        public ProductPage(IWebDriver driver) { this.driver = driver; }

        // --- LOCATORS ---
        private By btnThemVaoGio = By.CssSelector(".btn-action.btn-add");
        // Cập nhật theo CSV: Nút Mua ngay có class là btn-buy
        private By btnMuaNgay = By.CssSelector(".btn-action.btn-buy");
        private By btnTangSoLuong = By.XPath("//button[normalize-space()='+']");
        private By btnGiamSoLuong = By.XPath("//button[normalize-space()='-']");

        // --- ACTIONS ---
        public void ClickThemVaoGio() => driver.FindElement(btnThemVaoGio).Click();
        public void ClickMuaNgay() => driver.FindElement(btnMuaNgay).Click();
        public void TangSoLuong() => driver.FindElement(btnTangSoLuong).Click();
        public void GiamSoLuong() => driver.FindElement(btnGiamSoLuong).Click();

        public void ChonCauHinh(string color, string storage)
        {
            if (!string.IsNullOrEmpty(color))
            {
                try { driver.FindElement(By.XPath($"//span[contains(text(), '{color}')]")).Click(); } catch { }
            }
            if (!string.IsNullOrEmpty(storage))
            {
                try { driver.FindElement(By.XPath($"//a[contains(text(), '{storage}')]")).Click(); } catch { }
            }
        }

        public void AcceptAlert()
        {
            try { new OpenQA.Selenium.Support.UI.WebDriverWait(driver, System.TimeSpan.FromSeconds(3)).Until(SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent()).Accept(); } catch { }
        }
    }
}