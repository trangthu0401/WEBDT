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
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // --- LOCATORS ---
        // Ô nhập từ khóa tìm kiếm
        private By txtSearch = By.XPath("//input[contains(@placeholder, 'Theo ID hoặc tên')]");
        // Nút "Tìm" màu đỏ cạnh ô input
        private By btnSearch = By.XPath("//button[contains(.,'Tìm') or contains(@class,'btn-search')]");
        // Cột ID trong bảng kết quả (để check xem Robot tìm đúng ID đó không)
        private By firstRowID = By.XPath("//table//tbody/tr[1]/td[contains(., 'ID')]");

        // --- ACTIONS ---
        public void SearchByID(string id)
        {
            var input = wait.Until(ExpectedConditions.ElementIsVisible(txtSearch));
            input.Clear();
            input.SendKeys(id);

            var btn = driver.FindElement(btnSearch);
            btn.Click();

            // Đợi một chút cho bảng load lại kết quả
            System.Threading.Thread.Sleep(1500);
        }

        public string GetFirstResultID()
        {
            try
            {
                return driver.FindElement(firstRowID).Text;
            }
            catch
            {
                return "Không tìm thấy";
            }
        }
    }
}