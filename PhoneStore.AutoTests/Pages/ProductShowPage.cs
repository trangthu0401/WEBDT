using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace PhoneStore.AutoTests.Pages
{
    public class ProductShowPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public ProductShowPage(IWebDriver driver)
        {
            this.driver = driver;
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        private By btnViewVariant = By.XPath("(//a[contains(@href, 'ProductVariant')])[1]");
        private By btnCloseSuccess = By.XPath("//button[contains(.,'Đóng')]");

        public void GoToFirstProductVariant() => wait.Until(ExpectedConditions.ElementToBeClickable(btnViewVariant)).Click();

        public void ClickShowVariant(string colorName)
        {
            // Tìm dòng có màu sắc và bấm nút "Đang bán"
            string xpathShowBtn = $"//td[contains(.,'{colorName}')]/following-sibling::td//button[contains(.,'Đang bán')]";
            var btnShow = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(xpathShowBtn)));

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", btnShow);
            System.Threading.Thread.Sleep(500);
            btnShow.Click();

            // Bấm Đóng popup thành công
            try
            {
                var btnClose = wait.Until(ExpectedConditions.ElementToBeClickable(btnCloseSuccess));
                btnClose.Click();
                System.Threading.Thread.Sleep(1000);
            }
            catch { }
        }

        
        public bool IsVariantVisibleOnUserPage(string productName, string colorName)
        {
            try
            {
                // Robot sẽ đợi một chút cho trang User load xong hẳn
                System.Threading.Thread.Sleep(2000);

                // Cách 1: Tìm xem có cái cụm từ (Tên máy + Màu) xuất hiện trong mã nguồn không
                string fullText = productName + " " + colorName;
                bool existsInSource = driver.PageSource.Contains(productName) && driver.PageSource.Contains(colorName);

                // Cách 2: Tìm theo Element cụ thể (Phòng hờ trường hợp text nằm trong thuộc tính title hoặc alt)
                var elements = driver.FindElements(By.XPath($"//*[contains(text(), '{colorName}')]"));

                return existsInSource || (elements.Count > 0);
            }
            catch { return false; }
        }
    }
}