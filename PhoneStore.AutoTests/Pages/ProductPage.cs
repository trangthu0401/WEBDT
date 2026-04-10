using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace PhoneStore.AutoTests.Pages
{
    public class ProductPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public ProductPage(IWebDriver driver)
        {
            this.driver = driver;
            // Tăng thời gian chờ lên 10s để đảm bảo web load xong nút
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // =========================================================
        // LOCATORS BẤT TỬ (KẾT HỢP XPATH & CLASS)
        // =========================================================

        // Tìm nút có class chứa 'buy' HOẶC text chứa 'Mua ngay'
        private By btnMuaNgay = By.XPath("//button[contains(@class, 'buy') or contains(translate(text(), 'MUA NGAY', 'mua ngay'), 'mua ngay')]");

        // Tìm nút có class chứa 'add' HOẶC text chứa 'Thêm'
        private By btnThemVaoGio = By.XPath("//button[contains(@class, 'add') or contains(text(), 'Thêm')]");

        // Nút Tăng / Giảm số lượng
        private By btnTangSoLuong = By.CssSelector(".qty-btn[onclick*='1']");
        private By btnGiamSoLuong = By.CssSelector(".qty-btn[onclick*='-1']");

        // Ô Input
        private By txtSoLuong = By.XPath("//input[@type='number' or contains(@class, 'qty') or @id='quantity' or @name='quantity']");

        // =========================================================
        // ACTIONS
        // =========================================================

        public void ClickThemVaoGio()
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(btnThemVaoGio)).Click();
        }

        public void ClickMuaNgay()
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(btnMuaNgay)).Click();
        }

        public void TangSoLuong()
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(btnTangSoLuong)).Click();
        }

        public void GiamSoLuong()
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(btnGiamSoLuong)).Click();
        }

        public string GetSoLuongHienTai()
        {
            return wait.Until(ExpectedConditions.ElementIsVisible(txtSoLuong)).GetAttribute("value");
        }

        public void ChonCauHinh(string color, string storage)
        {
            if (!string.IsNullOrEmpty(color))
            {
                var eleColor = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath($"//*[contains(text(), '{color}')]")));
                eleColor.Click();
            }

            if (!string.IsNullOrEmpty(storage))
            {
                var eleStorage = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath($"//a[contains(text(), '{storage}')]")));
                eleStorage.Click();
            }
        }

        public void AcceptAlert()
        {
            try { wait.Until(ExpectedConditions.AlertIsPresent()).Accept(); }
            catch { }
        }
        public int GetStockQuantity()
        {
            try
            {
                var stockElem = driver.FindElement(By.XPath("//span[contains(@class,'stock') or contains(@id,'stock')]"));
                string stockText = stockElem.Text;
                // Giả sử text "Còn lại: 50 sản phẩm" -> lấy số
                var match = System.Text.RegularExpressions.Regex.Match(stockText, @"\d+");
                return match.Success ? int.Parse(match.Value) : int.MaxValue;
            }
            catch { return int.MaxValue; }
        }
        // Locator cho nút Thêm vào giỏ (có thể bị disabled)
        private By btnAddToCartDisabled = By.XPath("//button[contains(@class, 'add') and @disabled]");
        private By btnBuyNowDisabled = By.XPath("//button[contains(@class, 'buy') and @disabled]");

        // Kiểm tra nút Thêm vào giỏ có bị disabled không
        public bool IsAddToCartDisabled()
        {
            try
            {
                var btn = driver.FindElement(btnThemVaoGio);
                return btn.GetAttribute("disabled") != null || btn.GetAttribute("class").Contains("disabled");
            }
            catch { return true; }
        }

        // Kiểm tra nút Mua ngay có bị disabled không
        public bool IsBuyNowDisabled()
        {
            try
            {
                var btn = driver.FindElement(btnMuaNgay);
                return btn.GetAttribute("disabled") != null || btn.GetAttribute("class").Contains("disabled");
            }
            catch { return true; }
        }

        // Lấy số lượng tối đa có thể chọn (stock) - đã có GetStockQuantity, nhưng có thể cải thiện
        // Thêm method chờ nút (+) bị disabled
        public bool IsPlusButtonDisabled()
        {
            try
            {
                var btn = driver.FindElement(btnTangSoLuong);
                return btn.GetAttribute("disabled") != null || btn.GetAttribute("class").Contains("disabled");
            }
            catch { return true; }
        }
    }
}