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
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        }

        private By btnViewVariant = By.XPath("(//a[contains(@href, 'ProductVariant')])[1]");

        // Locator lấy cái ô chứa tên Màu sắc ở dòng đầu tiên của bảng Admin
        private By firstColorName = By.XPath("//table//tbody/tr[1]/td[contains(@class,'color') or contains(.,'')]");
        // Locator nút Ẩn ở dòng đầu tiên
        private By btnHideFirst = By.XPath("//table//tbody/tr[1]//button[contains(.,'Ẩn')]");

        public void GoToFirstProductVariant() => wait.Until(ExpectedConditions.ElementToBeClickable(btnViewVariant)).Click();

        // HÀM MỚI: Robot tự lấy tên màu của dòng đầu tiên để tí nữa đi check
        public string GetFirstVariantColorName()
        {
            var element = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//table//tbody/tr[1]/td[3]"))); // Thường màu ở cột 3
            return element.Text.Trim();
        }

        public void ClickHideFirstVariant()
        {
            var btnHide = wait.Until(ExpectedConditions.ElementToBeClickable(btnHideFirst));

            // Cuộn tới và bấm
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", btnHide);
            System.Threading.Thread.Sleep(500);
            btnHide.Click();

            // Xác nhận SweetAlert nếu có
            try
            {
                var btnConfirm = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(".swal2-confirm")));
                btnConfirm.Click();
            }
            catch { }
        }

        public bool IsVariantVisibleOnUserPage(string colorName)
        {
            try
            {
                // Đợi một chút cho trang User load
                System.Threading.Thread.Sleep(2000);
                // Tìm xem có cái nút chọn màu nào chứa text đó không
                var elements = driver.FindElements(By.XPath($"//*[contains(@class,'color') or @type='radio']/..//*[contains(text(), '{colorName}')]"));
                return elements.Count > 0 && elements[0].Displayed;
            }
            catch
            {
                return false;
            }
        }
    }
}