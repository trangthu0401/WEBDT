using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace PhoneStore.AutoTests.Pages
{
    public class ProductVariantPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public ProductVariantPage(IWebDriver driver)
        {
            this.driver = driver;
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // --- LOCATORS ---
        // Nút xem biến thể ở dòng đầu tiên của danh sách sản phẩm
        private By btnViewVariant = By.XPath("(//a[contains(@href, 'ProductVariant')])[1]");
        // Nút mở Modal thêm biến thể
        private By btnOpenAddModal = By.XPath("//button[contains(., 'Thêm Biến Thể')]");

        // Các ô nhập liệu trong Modal (Dùng XPath cho chuẩn)
        private By txtColor = By.XPath("//label[contains(.,'Màu sắc')]/following-sibling::input");
        private By txtStorage = By.XPath("//label[contains(.,'Dung lượng')]/following-sibling::input");
        private By txtRAM = By.XPath("//label[contains(.,'RAM')]/following-sibling::input");
        private By txtPrice = By.XPath("//label[contains(.,'Giá bán gốc')]/following-sibling::input");
        private By txtPromoPrice = By.XPath("//label[contains(.,'Giá khuyến mãi')]/following-sibling::input");
        private By txtStock = By.XPath("//label[contains(.,'Số lượng tồn kho')]/following-sibling::input");
        private By fileImg = By.XPath("//input[@type='file']");
        private By btnSave = By.XPath("//button[contains(., 'Lưu Biến Thể')]");

        // --- ACTIONS ---
        public void GoToFirstProductVariant() => wait.Until(ExpectedConditions.ElementToBeClickable(btnViewVariant)).Click();

        public void OpenAddModal()
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(btnOpenAddModal)).Click();
            // Đợi Modal hiện lên bằng cách đợi ô Màu sắc hiển thị
            wait.Until(ExpectedConditions.ElementIsVisible(txtColor));
        }

        public void InputVariantDetails(dynamic data)
        {
            // Nhập các ô text cơ bản
            driver.FindElement(txtColor).SendKeys((string)data.Color);
            driver.FindElement(txtStorage).SendKeys((string)data.Storage);
            driver.FindElement(txtRAM).SendKeys((string)data.RAM);

            // Nhập Giá và Tồn kho dùng hàm Smart (Xóa số 0 mặc định)
            FillInputSmart(txtPrice, (string)data.Price);

            if (!string.IsNullOrEmpty((string)data.PromoPrice))
                FillInputSmart(txtPromoPrice, (string)data.PromoPrice);

            FillInputSmart(txtStock, (string)data.Stock);

            // Up ảnh biến thể
            try
            {
                driver.FindElement(fileImg).SendKeys((string)data.ImagePath);
            }
            catch (Exception ex) { Console.WriteLine("Lỗi ảnh biến thể: " + ex.Message); }
        }

        private void FillInputSmart(By locator, string value)
        {
            var element = wait.Until(ExpectedConditions.ElementIsVisible(locator));
            element.Click();
            element.SendKeys(Keys.Control + "a");
            element.SendKeys(Keys.Backspace);
            element.SendKeys(value);
            element.SendKeys(Keys.Tab);
            System.Threading.Thread.Sleep(500);
        }

        public void Save()
        {
            var btn = driver.FindElement(btnSave);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", btn);
            System.Threading.Thread.Sleep(1000);
            btn.Click();
        }
    }
}