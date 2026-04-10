using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace PhoneStore.AutoTests.Pages
{
    public class CheckoutPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public CheckoutPage(IWebDriver driver)
        {
            this.driver = driver;
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // ==========================================
        // LOCATORS (Khớp với cấu trúc HTML của bạn)
        // ==========================================
        private By txtFullName = By.XPath("//input[@id='FullName' or @name='FullName' or @id='ReceiverName']");
        private By txtPhone = By.XPath("//input[@id='Phone' or @name='Phone' or @id='ReceiverPhone']");

        // Modal Địa chỉ
        private By btnThayDoiDiaChi = By.CssSelector(".btn-change-address");
        private By cboProvince = By.Id("province");
        private By cboDistrict = By.Id("district");
        private By cboWard = By.Id("ward");
        private By txtStreetDetail = By.Id("streetDetail");
        private By btnLuuDiaChi = By.XPath("//button[contains(text(),'Lưu địa chỉ') or contains(text(),'Thêm địa chỉ')]");

        // Phương thức thanh toán & Đặt hàng
        private By optBanking = By.Id("opt-banking");
        private By optCOD = By.Id("opt-cod");
        private By btnDatHang = By.CssSelector(".btn-place-order, .btn-buy-now"); // Nút 'ĐANG XỬ LÝ...' trong CSV

        // ==========================================
        // ACTIONS
        // ==========================================
        public void EnterFullName(string fullName)
        {
            var e = wait.Until(ExpectedConditions.ElementIsVisible(txtFullName));
            e.Clear(); e.SendKeys(fullName);
        }

        public void EnterPhone(string phone)
        {
            var e = wait.Until(ExpectedConditions.ElementIsVisible(txtPhone));
            e.Clear(); e.SendKeys(phone);
        }

        public void ClickThayDoiDiaChi() => wait.Until(ExpectedConditions.ElementToBeClickable(btnThayDoiDiaChi)).Click();

        public void ChonDiaChiFull(string tinh, string quan, string xa, string soNha)
        {
            new SelectElement(wait.Until(ExpectedConditions.ElementIsVisible(cboProvince))).SelectByText(tinh);

            wait.Until(d => new SelectElement(d.FindElement(cboDistrict)).Options.Count > 1);
            new SelectElement(driver.FindElement(cboDistrict)).SelectByText(quan);

            wait.Until(d => new SelectElement(d.FindElement(cboWard)).Options.Count > 1);
            new SelectElement(driver.FindElement(cboWard)).SelectByText(xa);

            driver.FindElement(txtStreetDetail).SendKeys(soNha);
            driver.FindElement(btnLuuDiaChi).Click();
        }

        public void SelectPaymentMethod(bool isCOD)
        {
            By target = isCOD ? optCOD : optBanking;
            wait.Until(ExpectedConditions.ElementToBeClickable(target)).Click();
        }

        public void ClickDatHang() => wait.Until(ExpectedConditions.ElementToBeClickable(btnDatHang)).Click();

        public void ConfirmSweetAlert()
        {
            try
            {
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(".swal2-confirm"))).Click();
            }
            catch { }
        }
    }
}