using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace PhoneStore.AutoTests.Pages
{
    public class OrderCancelPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public OrderCancelPage(IWebDriver driver)
        {
            this.driver = driver;
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        }

        // 1. Tìm đơn "CHỜ XÁC NHẬN" trên cùng và bấm vào
        public string ClickFirstWaitingOrder()
        {
            wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("tbody")));
            var rows = driver.FindElements(By.XPath("//table/tbody/tr"));

            foreach (var row in rows)
            {
                if (row.Text.ToLower().Trim().Contains("chờ xác nhận"))
                {
                    string orderID = row.FindElement(By.XPath("./td[1]")).Text;
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", row);
                    System.Threading.Thread.Sleep(500);
                    row.Click();
                    return orderID;
                }
            }
            throw new NoSuchElementException("Không tìm thấy đơn nào đang chờ xác nhận để hủy.");
        }

        public void ProcessCancelOrder(string reason)
        {
            // 1. Bấm nút "Hủy đơn" (nút màu đỏ ngoài trang chi tiết)
            var btnOpenCancel = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(.,'Hủy đơn')]")));
            btnOpenCancel.Click();

            // Đợi 2 giây cho cái bảng "Hủy Đơn Hàng" hiện lên hẳn
            System.Threading.Thread.Sleep(2000);

            // 2. Tìm chính xác ô nhập lý do (thẻ textarea bên trong modal)
            // Tui dùng XPath đi từ tiêu đề "Hủy Đơn Hàng" xuống cho chắc chắn
            By txtReasonPath = By.XPath("//div[contains(@class,'modal')]//textarea");

            var txtReason = wait.Until(ExpectedConditions.ElementIsVisible(txtReasonPath));

            // Cuộn tới ô nhập để tránh bị các thành phần khác che
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", txtReason);
            System.Threading.Thread.Sleep(500);

            // Click vào trước khi nhập để kích hoạt con trỏ chuột
            txtReason.Click();
            txtReason.Clear();
            txtReason.SendKeys(reason);
            Console.WriteLine($"Robot: Đã nhập lý do hủy: {reason}");

            // 3. Bấm nút "Xác nhận Hủy" màu đỏ TRÊN CÁI BẢNG
            By btnConfirmCancelPath = By.XPath("//div[contains(@class,'modal')]//button[contains(.,'Xác nhận Hủy')]");
            var btnConfirmCancel = wait.Until(ExpectedConditions.ElementToBeClickable(btnConfirmCancelPath));
            btnConfirmCancel.Click();

            System.Threading.Thread.Sleep(3000); // Đợi hệ thống xử lý hủy đơn
        }
        // Hàm này nhấn xác nhận hủy mà không nhập lý do
        public void ConfirmCancelWithoutReason()
        {
            // 1. Bấm nút "Hủy đơn" để mở Modal
            var btnOpenCancel = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(.,'Hủy đơn')]")));
            btnOpenCancel.Click();

            System.Threading.Thread.Sleep(2000);

            // 2. Không nhập gì cả, bấm thẳng nút "Xác nhận Hủy"
            By btnConfirmCancelPath = By.XPath("//div[contains(@class,'modal')]//button[contains(.,'Xác nhận Hủy')]");
            var btnConfirmCancel = wait.Until(ExpectedConditions.ElementToBeClickable(btnConfirmCancelPath));
            btnConfirmCancel.Click();
        }

        // Hàm kiểm tra lỗi validation (thường là thẻ span màu đỏ dưới ô textarea)
        public bool IsReasonRequiredErrorDisplayed()
        {
            try
            {
                // Tìm thông báo lỗi dạng: "Vui lòng nhập lý do" hoặc class text-danger
                var error = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[contains(@class,'modal')]//span[contains(@class,'text-danger')] | //div[contains(@class,'modal')]//*[contains(text(),'lý do')]")));
                return error.Displayed;
            }
            catch { return false; }
        }
    }
}