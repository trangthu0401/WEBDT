using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI; // Cần thư viện này để tương tác với Dropdown (thẻ <select>)

namespace PhoneStore.AutoTests.Pages
{
    public class CheckoutPage
    {
        private IWebDriver driver;

        public CheckoutPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        // ==========================================
        // 1. LOCATORS (Các phần tử mới từ file CSV)
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
        private By btnDongModal = By.CssSelector("button[data-bs-dismiss='modal']");

        // Các lựa chọn Thanh toán
        private By optBanking = By.Id("opt-banking"); // Chuyển khoản QR
        private By optCOD = By.Id("opt-cod");         // Tiền mặt

        private By btnDatHang = By.XPath("//button[contains(text(), 'Đặt hàng') or contains(text(), 'Thanh toán')]");

        // ==========================================
        // 2. ACTIONS (Các thao tác)
        // ==========================================
        public void EnterFullName(string fullName)
        {
            try { var e = driver.FindElement(txtFullName); e.Clear(); e.SendKeys(fullName); } catch { }
        }

        public void EnterPhone(string phone)
        {
            try { var e = driver.FindElement(txtPhone); e.Clear(); e.SendKeys(phone); } catch { }
        }

        // Bấm nút Thay đổi địa chỉ để mở Modal
        public void ClickThayDoiDiaChi()
        {
            try { driver.FindElement(btnThayDoiDiaChi).Click(); } catch { }
        }

        // Chọn Tỉnh/Thành Phố
        public void SelectProvince(string province)
        {
            try { new SelectElement(driver.FindElement(cboProvince)).SelectByText(province); } catch { }
        }

        // Chọn Quận/Huyện
        public void SelectDistrict(string district)
        {
            try { new SelectElement(driver.FindElement(cboDistrict)).SelectByText(district); } catch { }
        }

        // Chọn Phường/Xã
        public void SelectWard(string ward)
        {
            try { new SelectElement(driver.FindElement(cboWard)).SelectByText(ward); } catch { }
        }

        // Nhập số nhà
        public void EnterStreetDetail(string street)
        {
            try { var e = driver.FindElement(txtStreetDetail); e.Clear(); e.SendKeys(street); } catch { }
        }

        public void ClickLuuDiaChi()
        {
            try { driver.FindElement(btnLuuDiaChi).Click(); } catch { }
        }

        // Chọn phương thức thanh toán
        public void SelectPaymentMethod(string method)
        {
            try
            {
                if (method.Contains("QR") || method.Contains("VNPay") || method.Contains("Chuyển khoản"))
                    driver.FindElement(optBanking).Click();
                else
                    driver.FindElement(optCOD).Click();
            }
            catch { }
        }

        public void ClickDatHang()
        {
            driver.FindElement(btnDatHang).Click();
        }
    }
}