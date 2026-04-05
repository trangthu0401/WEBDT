using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace PhoneStore.AutoTests.Pages
{
    public class AdminProductPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public AdminProductPage(IWebDriver driver)
        {
            this.driver = driver;
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // Locators dùng XPath cho "bất tử"
        private By txtName = By.Id("Product_Name");
        private By ddBrand = By.Id("Product_BrandId");
        private By txtDesc = By.Id("Product_Description");
        private By fileImg = By.CssSelector("input[type='file']");

        private By txtChipset = By.XPath("//label[contains(.,'Chipset')]/following-sibling::input");
        private By txtOS = By.XPath("//label[contains(.,'Hệ điều hành')]/following-sibling::input");
        private By txtBattery = By.XPath("//label[contains(.,'Pin')]/following-sibling::input");

        private By txtColor = By.XPath("//label[contains(.,'Màu sắc')]/following-sibling::input");
        private By txtStorage = By.XPath("//label[contains(.,'Bộ nhớ trong')]/following-sibling::input");
        private By txtRAM = By.XPath("//label[contains(.,'RAM')]/following-sibling::input");
        private By txtPrice = By.XPath("//label[contains(.,'Giá bán')]/following-sibling::input");
        private By txtStock = By.XPath("//label[contains(.,'Tồn kho')]/following-sibling::input");

        private By btnSubmit = By.XPath("//button[contains(.,'Lưu')]");

        public void GoToCreatePage() => wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("a[href*='Create']"))).Click();

        public void InputProductDetails(dynamic data)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            // 1. Nhập Tên, Hãng, Mô tả
            wait.Until(ExpectedConditions.ElementIsVisible(txtName)).SendKeys((string)data.ProductName);
            new SelectElement(driver.FindElement(ddBrand)).SelectByText((string)data.Brand);
            driver.FindElement(txtDesc).SendKeys((string)data.Description);

            // 2. Up ảnh (Vy check kỹ đuôi file .jpg hay .webp nhé)
            try
            {
                driver.FindElement(fileImg).SendKeys((string)data.ImagePath);
            }
            catch (Exception ex) { Console.WriteLine("Lỗi ảnh: " + ex.Message); }

            // 3. Nhập Thông số kỹ thuật
            driver.FindElement(txtChipset).SendKeys((string)data.Chipset);
            driver.FindElement(txtOS).SendKeys((string)data.OS);
            driver.FindElement(txtBattery).SendKeys((string)data.Battery);

            js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");

            // 4. Nhập Biến thể (Dùng hàm Smart để trình duyệt không báo lỗi "Please fill out")
            FillInputSmart(txtColor, (string)data.Color);
            FillInputSmart(txtStorage, (string)data.Storage);
            FillInputSmart(txtRAM, (string)data.RAM);
            FillInputSmart(txtPrice, (string)data.Price);
            FillInputSmart(txtStock, (string)data.Stock);
        }

        private void FillInputSmart(By locator, string value)
        {
            var element = wait.Until(ExpectedConditions.ElementIsVisible(locator));
            element.Click();
            element.SendKeys(Keys.Control + "a");
            element.SendKeys(Keys.Backspace);
            element.SendKeys(value);
            // Quan trọng: Nhấn Tab để web xác nhận dữ liệu đã nhập
            element.SendKeys(Keys.Tab);
            System.Threading.Thread.Sleep(500); // Nghỉ một chút cho web nhận
        }

        public void Save()
        {
            var btn = driver.FindElement(btnSubmit);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", btn);
            System.Threading.Thread.Sleep(1000);
            btn.Click();
        }
    }
}