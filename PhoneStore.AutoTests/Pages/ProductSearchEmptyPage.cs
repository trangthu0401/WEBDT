using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace PhoneStore.AutoTests.Pages
{
    public class ProductSearchEmptyPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public ProductSearchEmptyPage(IWebDriver driver)
        {
            this.driver = driver;
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // --- LOCATORS ---
        private By txtSearch = By.XPath("//input[contains(@placeholder, 'Theo ID hoặc tên')]");
        private By btnSearch = By.XPath("//button[contains(.,'Tìm') or contains(@class,'btn-search')]");

        // Locator cho thông báo không tìm thấy (Vy check xem Web Vy hiện chữ gì nhé)
        private By lblEmptyMessage = By.XPath("//td[contains(.,'không tìm thấy')] | //div[contains(.,'không tìm thấy')] | //p[contains(.,'không tìm thấy')]");

        // --- ACTIONS ---
        public void Search(string keyword)
        {
            var input = wait.Until(ExpectedConditions.ElementIsVisible(txtSearch));
            input.Clear();
            input.SendKeys(keyword);
            driver.FindElement(btnSearch).Click();
            System.Threading.Thread.Sleep(2000); // Đợi bảng load lại
        }

        public bool IsEmptyMessageDisplayed()
        {
            try
            {
                // Kiểm tra xem có dòng chữ "không tìm thấy" xuất hiện không
                return driver.FindElement(lblEmptyMessage).Displayed;
            }
            catch
            {
                // Nếu không thấy dòng chữ đó, có thể là do Web vẫn hiện bảng trắng hoặc lỗi khác
                return false;
            }
        }
    }
}