using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.IO; // Quan trọng để xử lý đường dẫn ảnh

namespace PhoneStore.AutoTests.Pages
{
    public class ProductVariantPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public ProductVariantPage(IWebDriver driver)
        {
            this.driver = driver;
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        }

        // --- LOCATORS ---
        private By btnViewVariant = By.XPath("(//a[contains(@href, 'ProductVariant')])[1]");
        private By btnOpenAddModal = By.XPath("//button[contains(., 'Thêm Biến Thể')]");

        private By txtColor = By.XPath("//label[contains(.,'Màu sắc')]/following-sibling::input");
        private By txtStorage = By.XPath("//label[contains(.,'Dung lượng')]/following-sibling::input");
        private By txtRAM = By.XPath("//label[contains(.,'RAM')]/following-sibling::input");
        private By txtPrice = By.XPath("//label[contains(.,'Giá bán gốc')]/following-sibling::input");
        private By txtPromoPrice = By.XPath("//label[contains(.,'Giá khuyến mãi')]/following-sibling::input");
        private By txtStock = By.XPath("//label[contains(.,'Số lượng tồn kho')]/following-sibling::input");
        private By fileImg = By.XPath("//input[@type='file']");
        private By btnSave = By.XPath("//button[contains(., 'Lưu Biến Thể')]");




        // --- ACTIONS ---
        public void GoToFirstProductVariant()
        {
            var btn = wait.Until(ExpectedConditions.ElementToBeClickable(btnViewVariant));
            // Cuộn tới cho chắc ăn rồi mới click
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", btn);
            btn.Click();
        }

        public bool IsAtVariantListPage()
        {
            // Kiểm tra URL xem đã nhảy sang trang biến thể chưa
            return wait.Until(d => d.Url.ToLower().Contains("productvariant"));
        }

        public void OpenAddModal()
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(btnOpenAddModal)).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(txtColor));
        }

        public void InputVariantDetails(dynamic data)
        {
            // Nhập các ô text
            driver.FindElement(txtColor).SendKeys((string)data.Color);
            driver.FindElement(txtStorage).SendKeys((string)data.Storage);
            driver.FindElement(txtRAM).SendKeys((string)data.RAM);

            FillInputSmart(txtPrice, (string)data.Price);

            if (!string.IsNullOrEmpty((string)data.PromoPrice))
                FillInputSmart(txtPromoPrice, (string)data.PromoPrice);

            FillInputSmart(txtStock, (string)data.Stock);

            // XỬ LÝ ẢNH BIẾN THỂ TỪ PROJECT
            try
            {
                string fileName = (string)data.ImageFileName;
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName);
                driver.FindElement(fileImg).SendKeys(fullPath);
                Console.WriteLine("Robot up ảnh biến thể: " + fullPath);
            }
            catch (Exception ex) { Console.WriteLine("Lỗi up ảnh biến thể: " + ex.Message); }
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
            var btn = wait.Until(ExpectedConditions.ElementToBeClickable(btnSave));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", btn);
            System.Threading.Thread.Sleep(1000);
            btn.Click();
        }
        // Locator cho nút Xem chi tiết ở dòng đầu tiên (Icon mắt hoặc chữ Xem)
        private By btnViewDetail = By.XPath("(//a[contains(@href, 'Details')])[1]");

        public void ViewFirstProductDetail()
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(btnViewDetail)).Click();
        }

        public bool IsErrorMessageDisplayed()
        {
            try
            {
                // Tìm các span validation hoặc message lỗi của hệ thống
                var error = driver.FindElements(By.XPath("//span[contains(@class,'text-danger')] | //div[contains(@class,'validation-summary-errors')]"));
                return error.Count > 0 && error[0].Displayed;
            }
            catch { return false; }
        }

        public string GetUploadedFileName()
        {
            // Lấy giá trị từ ô input file (thường nó sẽ lưu đường dẫn file)
            return driver.FindElement(fileImg).GetAttribute("value");
        }

        public string GetInputValue(By locator)
        {
            return driver.FindElement(locator).GetAttribute("value");
        }

        // Tạo thuộc tính để Test gọi cho dễ
        public By ColorInput => txtColor;
        public By StockInput => txtStock;

    }
}