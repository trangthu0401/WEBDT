using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace PhoneStore.AutoTests.Pages
{
    public class OrderHistoryPage
    {
        private IWebDriver driver;
        public OrderHistoryPage(IWebDriver driver) => this.driver = driver;

        // --- LOCATORS (Cập nhật chuẩn theo file CSV mới nhất) ---
        private By drpStatus = By.Name("statusFilter");
        private By txtStartDate = By.Id("startDate");
        private By txtEndDate = By.Id("endDate");

        private By btnHuyDonHang = By.CssSelector(".btn-cancel-pop");
        // Locator cho lý do hủy (Tìm label chứa text lý do)
        private By btnXacNhanHuy = By.XPath("//button[contains(text(),'Xác nhận hủy')]");
        private By btnSweetAlertOK = By.CssSelector(".swal2-confirm");

        // --- ACTIONS ---
        public void LocDonHang(string status, string startDate, string endDate)
        {
            if (!string.IsNullOrEmpty(status))
                new SelectElement(driver.FindElement(drpStatus)).SelectByText(status);

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

        public void ClickHuyDon() => driver.FindElement(btnHuyDonHang).Click();

        public void ChonLyDoVaXacNhan(string lyDo)
        {
            // Tìm và click vào lý do cụ thể (vd: "Tìm thấy giá tốt hơn")
            try
            {
                driver.FindElement(By.XPath($"//label[contains(text(),'{lyDo}')]")).Click();
                System.Threading.Thread.Sleep(500);
                driver.FindElement(btnXacNhanHuy).Click();
            }
            catch { }
        }

        public void ClickOKPopup()
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                wait.Until(d => d.FindElement(btnSweetAlertOK)).Click();
            }
            catch { }
        }
    }
}