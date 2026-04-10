using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using PhoneStore.AutoTests.Utilities;
using SeleniumExtras.WaitHelpers;
using System;

namespace PhoneStore.AutoTests.Pages
{
    public class OrderHistoryPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public OrderHistoryPage(IWebDriver driver)
        {
            this.driver = driver;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        public void GoToOrderHistory()
        {
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/OrderCustomer/History");
            wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
            System.Threading.Thread.Sleep(500);
        }

        // ========== LOCATORS (dựa trên file record) ==========
        // Ô tìm kiếm ID
        private By searchInput = By.XPath("//input[@placeholder='Nhập id đơn hàng']");

        // Dropdown lọc
        private By statusFilter = By.XPath("//select[@name='statusFilter']");
        private By paymentFilter = By.XPath("//select[@name='paymentMethod']");
        private By datePreset = By.XPath("//select[@name='datePreset']");

        // Nút lọc (submit)
        private By filterButton = By.XPath("//input[@type='submit']");

        // Bảng đơn hàng
        private By tableRows = By.XPath("//table//tbody/tr");
        private By emptyMessage = By.XPath("//*[contains(text(),'Không tìm thấy đơn hàng') or contains(text(),'Chưa có đơn hàng')]");

        // Nút hủy trên dòng đầu tiên
        private By firstRowCancelButton = By.XPath("(//button[@class='btn-cancel-pop'])[1]");

        // Popup hủy đơn
        private By cancelReasonLabel = By.XPath("//label[normalize-space()='{0}']");
        private By otherReasonTextarea = By.XPath("//textarea[@id='otherReason']");
        private By confirmCancelButton = By.XPath("//button[contains(text(),'Xác nhận hủy')]");
        private By okSweetAlert = By.XPath("//button[normalize-space()='OK']");

        // Modal chi tiết đơn hàng (khi click vào mã đơn)
        private By modalCloseButton = By.XPath("//button[@aria-label='Close']");
        private By modalSubtotal = By.XPath("//*[contains(text(),'Tạm tính')]/following-sibling::*");
        private By modalShipping = By.XPath("//*[contains(text(),'Phí vận chuyển')]/following-sibling::*");
        private By modalTotal = By.XPath("//*[contains(text(),'Tổng cộng')]/following-sibling::*");

        // Phân trang (nếu có)
        private By page2Link = By.XPath("//a[contains(text(),'2')]");

        // ========== ACTIONS ==========
        public void SearchByOrderId(string orderId)
        {
            var input = wait.Until(ExpectedConditions.ElementIsVisible(searchInput));
            input.Clear();
            input.SendKeys(orderId);
            input.SendKeys(Keys.Enter);
            System.Threading.Thread.Sleep(500);
        }

        public void SelectStatus(string statusText)
        {
            var select = new SelectElement(wait.Until(ExpectedConditions.ElementIsVisible(statusFilter)));
            select.SelectByText(statusText);
            System.Threading.Thread.Sleep(500);
        }

        public void SelectPaymentMethod(string paymentText)
        {
            var select = new SelectElement(wait.Until(ExpectedConditions.ElementIsVisible(paymentFilter)));
            select.SelectByText(paymentText);
            System.Threading.Thread.Sleep(500);
        }

        public void SelectDatePreset(string preset)
        {
            var select = new SelectElement(wait.Until(ExpectedConditions.ElementIsVisible(datePreset)));
            select.SelectByText(preset);
            System.Threading.Thread.Sleep(500);
        }

        public void ClickFilter()
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(filterButton)).Click();
            System.Threading.Thread.Sleep(500);
        }

        public int GetOrderCount()
        {
            try { wait.Until(d => d.FindElements(tableRows).Count >= 0); } catch { }
            return driver.FindElements(tableRows).Count;
        }

        public bool IsEmptyMessageDisplayed() => driver.FindElements(emptyMessage).Count > 0;

        public string GetFirstOrderStatus()
        {
            try
            {
                var statusCell = driver.FindElement(By.XPath("//tbody/tr[1]/td[6]"));
                return statusCell.Text.Trim();
            }
            catch { return string.Empty; }
        }

        public string GetFirstOrderId()
        {
            try
            {
                var idCell = driver.FindElement(By.XPath("//tbody/tr[1]/td[1]"));
                return idCell.Text.Trim().Replace("#", "");
            }
            catch { return string.Empty; }
        }

        public void ClickFirstCancelButton()
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(firstRowCancelButton)).Click();
            System.Threading.Thread.Sleep(500);
        }

        public void SelectCancelReason(string reason)
        {
            var reasonXpath = By.XPath($"//label[normalize-space()='{reason}']");
            wait.Until(ExpectedConditions.ElementToBeClickable(reasonXpath)).Click();
        }

        public void EnterCancelDetail(string detail)
        {
            var textarea = wait.Until(ExpectedConditions.ElementIsVisible(otherReasonTextarea));
            textarea.Clear();
            textarea.SendKeys(detail);
        }

        public void ConfirmCancel()
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(confirmCancelButton)).Click();
        }

        public void ClickOkSweetAlert()
        {
            try { wait.Until(ExpectedConditions.ElementToBeClickable(okSweetAlert)).Click(); }
            catch { }
        }

        // Mở modal chi tiết bằng cách click vào mã đơn hàng đầu tiên
        public void ClickFirstOrderId()
        {
            var idCell = driver.FindElement(By.XPath("//tbody/tr[1]/td[1]"));
            idCell.Click();
            System.Threading.Thread.Sleep(500);
        }

        public void CloseModal()
        {
            try { wait.Until(ExpectedConditions.ElementToBeClickable(modalCloseButton)).Click(); }
            catch { }
            System.Threading.Thread.Sleep(300);
        }

        public decimal GetModalSubtotal() => ParseMoney(modalSubtotal);
        public decimal GetModalShipping() => ParseMoney(modalShipping);
        public decimal GetModalTotal() => ParseMoney(modalTotal);

        public void ClickPage2()
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(page2Link)).Click();
            System.Threading.Thread.Sleep(500);
        }

        private decimal ParseMoney(By locator)
        {
            string text = wait.Until(ExpectedConditions.ElementExists(locator)).Text;
            text = text.Replace(",", "").Replace("₫", "").Replace("đ", "").Trim();
            return decimal.Parse(text);
        }
    }
}