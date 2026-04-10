using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace PhoneStore.AutoTests.Pages
{
    public class ProductSearchPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public ProductSearchPage(IWebDriver driver)
        {
            this.driver = driver;
            // Tăng thời gian chờ lên 15s cho chắc ăn khi chạy Run All
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        }

        // --- LOCATORS ---
        // Ô nhập từ khóa (ID hoặc Tên)
        private By txtSearch = By.XPath("//input[contains(@placeholder, 'Theo ID hoặc tên')]");

        // Nút Tìm kiếm
        private By btnSearch = By.XPath("//button[contains(.,'Tìm') or contains(@class,'btn-search')]");

        // Cột ID trong bảng kết quả (Dòng 1, Cột 1)
        private By firstRowID = By.XPath("//table//tbody/tr[1]/td[1]");

        // Cột Tên sản phẩm trong bảng kết quả (Dòng 1, Cột 2)
        // Lưu ý: Nếu giao diện thực tế của Vy tên ở cột khác thì sửa số [2] thành số cột tương ứng
        private By firstRowName = By.XPath("//table//tbody/tr[1]/td[2]");

        // --- ACTIONS ---

        /// <summary>
        /// Thực hiện tìm kiếm sản phẩm theo từ khóa (ID hoặc Tên)
        /// </summary>
        public void SearchKeyword(string keyword)
        {
            // 1. Đợi ô search hiển thị và nhập liệu
            var input = wait.Until(ExpectedConditions.ElementIsVisible(txtSearch));
            input.Clear();
            input.SendKeys(keyword);

            // 2. Cuộn tới nút Tìm kiếm và Click
            var btn = driver.FindElement(btnSearch);
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", btn);
            System.Threading.Thread.Sleep(500); // Nghỉ một chút sau khi cuộn
            btn.Click();

            // 3. Đợi bảng dữ liệu load lại (Ajax). 
            // 1.5 giây là khoảng thời gian an toàn cho localhost.
            System.Threading.Thread.Sleep(1500);
        }

        /// <summary>
        /// Lấy giá trị ID của sản phẩm đầu tiên trong bảng kết quả
        /// </summary>
        public string GetFirstResultID()
        {
            try
            {
                var element = wait.Until(ExpectedConditions.ElementIsVisible(firstRowID));
                return element.Text.Trim();
            }
            catch
            {
                return "Không tìm thấy";
            }
        }

        /// <summary>
        /// Lấy giá trị Tên của sản phẩm đầu tiên trong bảng kết quả
        /// </summary>
        public string GetFirstResultName()
        {
            try
            {
                var element = wait.Until(ExpectedConditions.ElementIsVisible(firstRowName));
                return element.Text.Trim();
            }
            catch
            {
                return "Không tìm thấy";
            }
        }

       
        public bool IsTableEmpty()
        {
            try
            {
                return driver.PageSource.Contains("Không tìm thấy") ||
                       driver.FindElements(By.XPath("//table//tbody/tr")).Count == 0;
            }
            catch { return true; }
        }
    }
}