using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace PhoneStore.AutoTests.Pages
{
    public class OrderUpdatePage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public OrderUpdatePage(IWebDriver driver)
        {
            this.driver = driver;
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        }

        public string ClickFirstShippingOrder()
        {
            // 1. Chờ bảng hiện ra
            wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("tbody")));

            // 2. Lấy danh sách tất cả các dòng
            var rows = driver.FindElements(By.XPath("//table/tbody/tr"));

            foreach (var row in rows)
            {
                // Chuyển text dòng về chữ thường để so sánh cho chuẩn
                string rowText = row.Text.ToLower().Trim();

                // Nếu thấy chữ "đang giao"
                if (rowText.Contains("đang giao"))
                {
                    // Lấy ID ở cột đầu tiên (Cột ID)
                    string orderID = row.FindElement(By.XPath("./td[1]")).Text;

                    // Cuộn tới và bấm thẳng vào dòng (vì dòng có lệnh onclick chuyển trang)
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", row);
                    System.Threading.Thread.Sleep(500);

                    row.Click();

                    Console.WriteLine($"Robot: Đã tìm thấy và mở đơn Đang giao: {orderID}");
                    return orderID;
                }
            }
            throw new NoSuchElementException("Robot không tìm thấy đơn nào có trạng thái 'Đang giao' trên trang này.");
        }

        // 2. Bấm Xác nhận đã giao và xử lý Alert OK
        public void ConfirmDeliverySuccess()
        {
            // Bấm cái nút "Xác nhận Đã giao" màu xanh lá
            var btnConfirm = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(.,'Xác nhận Đã giao')]")));
            btnConfirm.Click();

            // CHỖ NÀY QUAN TRỌNG: Đợi cái bảng thông báo hiện lên và bấm OK
            wait.Until(ExpectedConditions.AlertIsPresent());
            IAlert alert = driver.SwitchTo().Alert();
            Console.WriteLine("Robot thấy thông báo: " + alert.Text);
            alert.Accept(); // Bấm nút OK

            System.Threading.Thread.Sleep(2000); // Đợi web cập nhật
        }
    }
}