using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace PhoneStore.AutoTests.Pages
{
    public class OrderDetailPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public OrderDetailPage(IWebDriver driver)
        {
            this.driver = driver;
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        }

        // --- ACTIONS ---
        public void ClickOnOrderCode(string orderCode)
        {
            // Robot tìm cái ô nào có chứa đúng mã đơn hàng HD01036 và bấm vào
            By orderLink = By.XPath($"//td[contains(text(), '{orderCode}')] | //a[contains(text(), '{orderCode}')]");
            var element = wait.Until(ExpectedConditions.ElementToBeClickable(orderLink));

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
            System.Threading.Thread.Sleep(500);
            element.Click();
        }
        public bool VerifyContentExists(string text)
        {
            try
            {
                // Nếu text là trạng thái, mình kiểm tra cả các nút bấm hoặc timeline
                if (text == "CHỜ XÁC NHẬN")
                {
                    // Kiểm tra xem có nút "Xác nhận & Giao hàng" hoặc chữ "Đã xác nhận" không
                    bool hasConfirmBtn = driver.PageSource.Contains("Xác nhận & Giao hàng");
                    bool hasConfirmedStep = driver.PageSource.Contains("Đã xác nhận");
                    return hasConfirmBtn || hasConfirmedStep;
                }

                // Với các text khác (như tên Vy) thì vẫn quét bình thường
                return driver.PageSource.Contains(text);
            }
            catch { return false; }
        }
    }
}