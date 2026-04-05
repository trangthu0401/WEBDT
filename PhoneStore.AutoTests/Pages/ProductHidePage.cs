using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace PhoneStore.AutoTests.Pages
{
    public class ProductHidePage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public ProductHidePage(IWebDriver driver)
        {
            this.driver = driver;
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // --- LOCATORS ---
        private By btnViewVariant = By.XPath("(//a[contains(@href, 'ProductVariant')])[1]");

        // --- ACTIONS ---
        public void GoToFirstProductVariant() => wait.Until(ExpectedConditions.ElementToBeClickable(btnViewVariant)).Click();

        public void ClickHideVariant(string colorName)
        {
            // Robot tìm dòng nào có chứa tên màu sắc Vy truyền vào, rồi bấm nút "Ẩn" ở cuối dòng đó
            string xpathHideBtn = $"//td[contains(.,'{colorName}')]/following-sibling::td//button[contains(.,'Ẩn')]";
            var btnHide = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(xpathHideBtn)));

            // Cuộn tới nút và bấm
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", btnHide);
            System.Threading.Thread.Sleep(500);
            btnHide.Click();

            // Nếu có popup xác nhận (SweetAlert), bấm OK
            try
            {
                var btnConfirm = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(".swal2-confirm")));
                btnConfirm.Click();
            }
            catch { }
        }

        public bool IsVariantVisibleOnUserPage(string colorName)
        {
            // Kiểm tra xem trên trang User có còn cái nút hoặc label tên màu đó không
            try
            {
                var elements = driver.FindElements(By.XPath($"//*[contains(text(), '{colorName}')]"));
                return elements.Count > 0 && elements[0].Displayed;
            }
            catch
            {
                return false;
            }
        }
    }
}