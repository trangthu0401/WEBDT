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
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        }

        public void GoToOrderHistory()
        {
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/OrderCustomer/History");
            wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
            System.Threading.Thread.Sleep(500);
        }

        // ========== LOCATORS (CHUẨN TỪ CSV RECORD) ==========
        // Bộ lọc
        private By searchInput = By.XPath("//input[@placeholder='Nhập id đơn hàng']");
        private By statusFilter = By.XPath("//select[@name='statusFilter']");
        private By paymentFilter = By.XPath("//select[@name='paymentMethod']");
        private By datePreset = By.XPath("//select[@name='datePreset']");
        private By filterSubmit = By.XPath("//input[@type='submit']");

        // Bảng đơn hàng
        private By tableRows = By.XPath("//table//tbody/tr");
        private By firstRowIdCell = By.XPath("//tbody/tr[1]/td[1]");
        private By firstRowStatusCell = By.XPath("//tbody/tr[1]/td[6]");
        private By firstRowCancelBtn = By.XPath("(//button[@class='btn-cancel-pop'])[1]");
        private By emptyMessage = By.XPath("//*[contains(text(),'Không tìm thấy đơn hàng') or contains(text(),'Chưa có đơn hàng')]");

        // Popup hủy đơn
        private By cancelReasonLabel = By.XPath("//label[normalize-space()='{0}']");
        private By otherReasonTextarea = By.XPath("//textarea[@id='otherReason']");
        private By confirmCancelBtn = By.XPath("//button[contains(text(),'Xác nhận hủy')]");
        private By okSweetAlert = By.XPath("//button[normalize-space()='OK']");
        private By sweetAlertError = By.CssSelector(".swal2-error");

        // Modal chi tiết đơn hàng
        private By modalCloseBtn = By.XPath("//button[@aria-label='Close']");
        private By modalSubtotal = By.XPath("//*[contains(text(),'Tạm tính')]/following-sibling::*");
        private By modalShipping = By.XPath("//*[contains(text(),'Phí vận chuyển')]/following-sibling::*");
        private By modalTotal = By.XPath("//*[contains(text(),'Tổng cộng')]/following-sibling::*");
        private By modalCancelReason = By.XPath("//*[contains(text(),'Lý do hủy')]/following-sibling::*");

        // Phân trang
        private By page2Link = By.XPath("//a[contains(text(),'2')]");

        // ========== HÀNH ĐỘNG ==========
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
            System.Threading.Thread.Sleep(300);
        }

        public void SelectPaymentMethod(string paymentText)
        {
            var select = new SelectElement(wait.Until(ExpectedConditions.ElementIsVisible(paymentFilter)));
            select.SelectByText(paymentText);
            System.Threading.Thread.Sleep(300);
        }

        public void SelectDatePreset(string preset)
        {
            var select = new SelectElement(wait.Until(ExpectedConditions.ElementIsVisible(datePreset)));
            select.SelectByText(preset);
            System.Threading.Thread.Sleep(300);
        }

        public void ClickFilter()
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(filterSubmit)).Click();
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
            try { return driver.FindElement(firstRowStatusCell).Text.Trim(); }
            catch { return string.Empty; }
        }

        public string GetFirstOrderId()
        {
            try { return driver.FindElement(firstRowIdCell).Text.Replace("#", "").Trim(); }
            catch { return string.Empty; }
        }

        public bool IsCancelButtonPresent()
        {
            try { return driver.FindElement(firstRowCancelBtn).Displayed; }
            catch { return false; }
        }

        public void ClickFirstCancelButton()
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(firstRowCancelBtn)).Click();
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
            wait.Until(ExpectedConditions.ElementToBeClickable(confirmCancelBtn)).Click();
        }

        public void ClickOkSweetAlert()
        {
            try { wait.Until(ExpectedConditions.ElementToBeClickable(okSweetAlert)).Click(); }
            catch { }
        }

        public string GetSweetAlertErrorText()
        {
            try { return wait.Until(ExpectedConditions.ElementIsVisible(sweetAlertError)).Text; }
            catch { return string.Empty; }
        }

        public void ClickFirstOrderId()
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(firstRowIdCell)).Click();
            System.Threading.Thread.Sleep(500);
        }

        public void CloseModal()
        {
            try { wait.Until(ExpectedConditions.ElementToBeClickable(modalCloseBtn)).Click(); }
            catch { }
            System.Threading.Thread.Sleep(300);
        }

        public decimal GetModalSubtotal() => ParseMoney(modalSubtotal);
        public decimal GetModalShipping() => ParseMoney(modalShipping);
        public decimal GetModalTotal() => ParseMoney(modalTotal);
        public string GetModalCancelReason() => driver.FindElement(modalCancelReason).Text;

        public void ClickPage2()
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(page2Link)).Click();
            System.Threading.Thread.Sleep(500);
        }

        public bool IsTextareaVisible()
        {
            try { return driver.FindElement(otherReasonTextarea).Displayed; }
            catch { return false; }
        }

        private decimal ParseMoney(By locator)
        {
            string text = wait.Until(ExpectedConditions.ElementExists(locator)).Text;
            text = text.Replace(",", "").Replace("₫", "").Replace("đ", "").Trim();
            return decimal.Parse(text);
        }
    }
}