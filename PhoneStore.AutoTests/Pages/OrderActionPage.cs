using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace PhoneStore.AutoTests.Pages
{
    public class OrderActionPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public OrderActionPage(IWebDriver driver)
        {
            this.driver = driver;
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        }

        // --- ACTIONS ---

        // 1. Tìm đơn "CHỜ XÁC NHẬN" đầu tiên và bấm vào
        public string ClickFirstWaitingOrder()
        {
            // 1. Chờ bảng hiện ra
            wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("tbody")));

            // 2. Tìm tất cả các dòng TR trong bảng
            var rows = driver.FindElements(By.XPath("//table/tbody/tr"));

            foreach (var row in rows)
            {
                // Lấy text của dòng, chuyển về chữ thường và xóa khoảng trắng thừa 2 đầu
                string rowText = row.Text.ToLower().Trim();

                // Kiểm tra xem dòng này có chứa chữ "chờ xác nhận" không
                if (rowText.Contains("chờ xác nhận"))
                {
                    // Lấy ID đơn hàng ở cột đầu tiên (td[1]) để in ra log
                    string orderID = row.FindElement(By.XPath("./td[1]")).Text;

                    // CUỘC CHƠI THAY ĐỔI Ở ĐÂY:
                    // Vì dòng <tr> có thuộc tính onclick, nên mình bấm thẳng vào cái dòng đó luôn!
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", row);
                    System.Threading.Thread.Sleep(500);

                    // Bấm vào chính cái dòng đó
                    row.Click();

                    Console.WriteLine($"Robot: Đã tìm thấy và bấm vào đơn hàng {orderID}");
                    return orderID;
                }
            }

            throw new NoSuchElementException("Không tìm thấy dòng nào có trạng thái 'Chờ xác nhận'.");
        }

        // 2. Quy trình bấm xác nhận trên Popup
        public void ProcessConfirmOrder()
        {
            // Bấm nút mở bảng
            var btnOpen = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(.,'Xác nhận & Giao hàng')]")));
            btnOpen.Click();
            System.Threading.Thread.Sleep(2000);

            // Chọn ngẫu nhiên shipper
            try
            {
                var drp = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//select | //*[contains(text(),'Chọn')]//parent::select")));
                SelectElement select = new SelectElement(drp);
                if (select.Options.Count > 1)
                {
                    select.SelectByIndex(new Random().Next(1, select.Options.Count));
                }
            }
            catch { }

            // Bấm xác nhận cuối cùng
            var btnFinal = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//div[contains(@class,'modal')]//button[contains(.,'Xác nhận & Giao')]")));
            btnFinal.Click();
            System.Threading.Thread.Sleep(3000);
        }
    }
}