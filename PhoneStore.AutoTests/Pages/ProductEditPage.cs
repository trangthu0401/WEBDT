using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.IO; // Thêm thư viện này để xử lý đường dẫn

namespace PhoneStore.AutoTests.Pages
{
    public class ProductEditPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public ProductEditPage(IWebDriver driver)
        {
            this.driver = driver;
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        }

        // --- LOCATORS ---
        private By btnEditFirst = By.XPath("(//a[contains(@href, 'Edit')])[1]");
        private By txtName = By.Id("Product_Name");
        private By ddBrand = By.Id("Product_BrandId");
        private By txtDesc = By.Id("Product_Description");
        private By dtpDate = By.Id("Product_ReleaseDate");

        private By txtChipset = By.XPath("//label[contains(.,'Chipset')]/following-sibling::input");
        private By txtOS = By.XPath("//label[contains(.,'Hệ điều hành')]/following-sibling::input");
        private By txtBattery = By.XPath("//label[contains(.,'Pin')]/following-sibling::input");

        private By fileImg = By.CssSelector("input[type='file']");
        private By btnSave = By.XPath("//button[contains(.,'Lưu')]");

        // --- ACTIONS ---
        public void GoToEditFirstProduct() => wait.Until(ExpectedConditions.ElementToBeClickable(btnEditFirst)).Click();

        public void FillEditForm(dynamic data)
        {
            // 1. Thông tin chung
            FillInputSmart(txtName, (string)data.ProductName);
            new SelectElement(driver.FindElement(ddBrand)).SelectByText((string)data.Brand);
            FillInputSmart(txtDesc, (string)data.Description);

            // 2. Thông số kỹ thuật
            try
            {
                var dateElem = driver.FindElement(dtpDate);
                dateElem.Clear();
                dateElem.SendKeys((string)data.ReleaseDate);
            }
            catch { }

            FillInputSmart(txtChipset, (string)data.Chipset);
            FillInputSmart(txtOS, (string)data.OS);
            FillInputSmart(txtBattery, (string)data.Battery);

            // 3. XỬ LÝ ẢNH TỪ PROJECT
            try
            {
                string fileName = (string)data.ImageFileName;
                if (!string.IsNullOrEmpty(fileName))
                {
                    // Tự động tìm đường dẫn trong thư mục bin của Project
                    string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName);

                    driver.FindElement(fileImg).SendKeys(fullPath);
                    Console.WriteLine("Robot: Đã tải ảnh từ project: " + fullPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi up ảnh: " + ex.Message);
            }
        }

        private void FillInputSmart(By locator, string value)
        {
            var element = wait.Until(ExpectedConditions.ElementIsVisible(locator));
            element.Click();
            element.SendKeys(Keys.Control + "a");
            element.SendKeys(Keys.Backspace);
            element.SendKeys(value);
            element.SendKeys(Keys.Tab);
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