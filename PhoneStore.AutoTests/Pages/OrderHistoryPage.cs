using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace PhoneStore.AutoTests.Pages
{
    public class OrderHistoryPage
    {
        private IWebDriver driver;
        public OrderHistoryPage(IWebDriver driver) => this.driver = driver;

        // --- LOCATORS ---
        private By drpStatus = By.Name("statusFilter");
        private By drpDatePreset = By.Name("datePreset");
        private By txtStartDate = By.Id("startDate");
        private By txtEndDate = By.Id("endDate");

        // Lấy nút Hủy đơn hàng đầu tiên tìm thấy
        private By btnHuyDonHang = By.XPath("(//button[contains(@class,'btn-cancel-pop') or contains(text(),'Hủy')])[1]");
        private By btnXacNhanHuy = By.XPath("//button[contains(text(),'Xác nhận') or contains(@class,'confirm')]");
        private By btnSweetAlertOK = By.CssSelector(".swal2-confirm");

        // --- ACTIONS ---
        public void LocDonHang(string status, string datePreset, string startDate, string endDate)
        {
            if (!string.IsNullOrEmpty(status))
                new SelectElement(driver.FindElement(drpStatus)).SelectByText(status);

            if (!string.IsNullOrEmpty(datePreset))
                new SelectElement(driver.FindElement(drpDatePreset)).SelectByText(datePreset);

            if (!string.IsNullOrEmpty(startDate))
            {
                var start = driver.FindElement(txtStartDate);
                start.Clear();
                start.SendKeys(startDate);
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                var end = driver.FindElement(txtEndDate);
                end.Clear();
                end.SendKeys(endDate);
            }
        }

        public bool IsCancelButtonPresent()
        {
            try { return driver.FindElement(btnHuyDonHang).Displayed; }
            catch { return false; }
        }

        public void ClickHuyDon() => driver.FindElement(btnHuyDonHang).Click();

        public void ChonLyDoVaXacNhan(string lyDo, string detailReason = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(lyDo))
                {
                    driver.FindElement(By.XPath($"//label[contains(text(),'{lyDo}')]")).Click();
                    System.Threading.Thread.Sleep(500);
                }

                if (!string.IsNullOrEmpty(detailReason))
                {
                    try
                    {
                        var textarea = driver.FindElement(By.CssSelector("textarea"));
                        textarea.Clear();
                        textarea.SendKeys(detailReason);
                    }
                    catch { }
                }

                driver.FindElement(btnXacNhanHuy).Click();
            }
            catch { }
        }

        public void ClickOKPopup()
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
                wait.Until(d => d.FindElement(btnSweetAlertOK)).Click();
            }
            catch { }
        }

        public void CloseSweetAlert()
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(btnSweetAlertOK)).Click();
            }
            catch { }
        }
    }
}
