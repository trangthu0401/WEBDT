using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.IO; // Thư viện để xử lý đường dẫn file

namespace PhoneStore.AutoTests.Pages
{
    public class AdminProductPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public AdminProductPage(IWebDriver driver)
        {
            this.driver = driver;
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        }

        // Locators
        private By txtName = By.Id("Product_Name");
        private By ddBrand = By.Id("Product_BrandId");
        private By txtDesc = By.Id("Product_Description");
        private By fileImg = By.CssSelector("input[type='file']");
        private By btnSubmit = By.XPath("//button[contains(.,'Lưu')]");

        // Thông số kỹ thuật
        private By txtChipset = By.XPath("//label[contains(.,'Chipset')]/following-sibling::input");
        private By txtOS = By.XPath("//label[contains(.,'Hệ điều hành')]/following-sibling::input");
        private By txtBattery = By.XPath("//label[contains(.,'Pin')]/following-sibling::input");

        // Biến thể
        private By txtColor = By.XPath("//label[contains(.,'Màu sắc')]/following-sibling::input");
        private By txtStorage = By.XPath("//label[contains(.,'Bộ nhớ trong')]/following-sibling::input");
        private By txtRAM = By.XPath("//label[contains(.,'RAM')]/following-sibling::input");
        private By txtPrice = By.XPath("//label[contains(.,'Giá bán')]/following-sibling::input");
        private By txtStock = By.XPath("//label[contains(.,'Tồn kho')]/following-sibling::input");

        public void GoToCreatePage()
        {
            // Thêm 1-2 giây đợi cho trang web ổn định hẳn (vì chạy Localhost hay bị lag nhẹ)
            System.Threading.Thread.Sleep(2000);

            // XPath này sẽ tìm mọi thẻ <a> có chứa chữ "Thêm" hoặc có link chứa chữ "Create"
            By btnCreate = By.XPath("//a[contains(@href,'Create')] | //a[contains(.,'Thêm')] | //button[contains(.,'Thêm')]");

            try
            {
                var element = wait.Until(ExpectedConditions.ElementToBeClickable(btnCreate));
                // Cuộn màn hình tới cái nút đó để đảm bảo nó nằm trong tầm mắt Robot
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
                element.Click();
            }
            catch (Exception ex)
            {
                // Nếu lỗi, Robot sẽ chụp ảnh và báo cho Vy biết nó đang đứng ở đâu
                Console.WriteLine("Robot đang đứng ở URL: " + driver.Url);
                throw new Exception("Vy ơi, không thấy nút Thêm mới đâu hết! Coi chừng bị văng Login rồi.");
            }
        }

        public void InputProductDetails(dynamic data)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            // 1. Nhập thông tin cơ bản
            wait.Until(ExpectedConditions.ElementIsVisible(txtName)).SendKeys((string)data.ProductName);
            new SelectElement(driver.FindElement(ddBrand)).SelectByText((string)data.Brand);
            driver.FindElement(txtDesc).SendKeys((string)data.Description);

            // 2. XỬ LÝ UP ẢNH TỪ PROJECT (Vy nhìn kỹ chỗ này nhé)
            try
            {
                string fileName = (string)data.ImageFileName;
                // Tự động lấy đường dẫn folder Images trong project
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName);

                driver.FindElement(fileImg).SendKeys(fullPath);
                Console.WriteLine("Robot up ảnh từ Project: " + fullPath);
            }
            catch (Exception ex) { Console.WriteLine("Lỗi up ảnh: " + ex.Message); }

            // 3. Nhập Thông số kỹ thuật
            driver.FindElement(txtChipset).SendKeys((string)data.Chipset);
            driver.FindElement(txtOS).SendKeys((string)data.OS);
            driver.FindElement(txtBattery).SendKeys((string)data.Battery);

            js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");

            // 4. Nhập Biến thể
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
            element.SendKeys(Keys.Tab);
            System.Threading.Thread.Sleep(500);
        }

        public void Save()
        {
            var btn = driver.FindElement(btnSubmit);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", btn);
            System.Threading.Thread.Sleep(1000);
            btn.Click();
        }
        // Thêm locator cho nút Hủy (thường là thẻ a hoặc button cạnh nút Lưu)
        private By btnCancel = By.XPath("//a[contains(.,'Hủy')] | //a[contains(@href,'Index')]");

        public void Cancel()
        {
            driver.FindElement(btnCancel).Click();
        }

        public string GetPageSource() => driver.PageSource;
    }
}