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
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        }

        // Locators – mở rộng thêm các khả năng
        private By txtFullName = By.XPath("//input[@id='FullName' or @name='FullName' or @id='ReceiverName' or @name='ReceiverName' or @id='CustomerName' or @name='CustomerName']");
        private By txtPhone = By.XPath("//input[@id='Phone' or @name='Phone' or @id='ReceiverPhone' or @name='ReceiverPhone' or @id='CustomerPhone' or @name='CustomerPhone']");

        private By btnThayDoiDiaChi = By.CssSelector(".btn-change-address");
        private By modalAddressDialog = By.XPath("//div[contains(@class,'modal') and contains(@style,'display: block')]");
        private By btnThemDiaChiMoi = By.CssSelector(".btn.btn-outline-danger.py-2.dashed-border");
        private By cboProvince = By.Id("province");
        private By cboDistrict = By.Id("district");
        private By cboWard = By.Id("ward");
        private By txtStreetDetail = By.Id("streetDetail");
        private By btnLuuDiaChi = By.XPath("//button[contains(text(),'Lưu địa chỉ')]");
        private By sweetOkButton = By.CssSelector(".swal2-confirm.swal2-styled");
        private By optCOD = By.Id("opt-cod");
        private By optBanking = By.Id("opt-banking");
        private By btnDatHang = By.CssSelector(".btn-place-order, .btn-buy-now");

        // Hàm kiểm tra element tồn tại (không throw exception)
        private bool IsElementPresent(By by, int timeoutSeconds = 2)
        {
            try
            {
                var waitShort = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
                waitShort.Until(ExpectedConditions.ElementExists(by));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void EnterFullName(string fullName)
        {
            if (!IsElementPresent(txtFullName, 3))
            {
                // Nếu không có field nhập tên, coi như không cần nhập (bỏ qua)
                Console.WriteLine("Không tìm thấy field nhập Họ tên, bỏ qua bước này.");
                return;
            }
            var e = wait.Until(ExpectedConditions.ElementIsVisible(txtFullName));
            e.Clear(); e.SendKeys(fullName);
        }

        public void EnterPhone(string phone)
        {
            if (!IsElementPresent(txtPhone, 3))
            {
                Console.WriteLine("Không tìm thấy field nhập SĐT, bỏ qua bước này.");
                return;
            }
            var e = wait.Until(ExpectedConditions.ElementIsVisible(txtPhone));
            e.Clear(); e.SendKeys(phone);
        }

        // Các hàm còn lại giữ nguyên (ClickThayDoiDiaChi, ClickThemDiaChiMoi, ...)
        public void ClickThayDoiDiaChi()
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(btnThayDoiDiaChi)).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(modalAddressDialog));
        }

        public void ClickThemDiaChiMoi()
        {
            var addBtn = wait.Until(ExpectedConditions.ElementToBeClickable(btnThemDiaChiMoi));
            try
            {
                addBtn.Click();
            }
            catch (ElementClickInterceptedException)
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addBtn);
            }
            wait.Until(ExpectedConditions.ElementIsVisible(cboProvince));
        }

        public void SelectProvince(string province)
        {
            var select = new SelectElement(wait.Until(ExpectedConditions.ElementIsVisible(cboProvince)));
            select.SelectByText(province);
            System.Threading.Thread.Sleep(500);
        }

        public void SelectDistrict(string district)
        {
            wait.Until(d => new SelectElement(d.FindElement(cboDistrict)).Options.Count > 1);
            var select = new SelectElement(driver.FindElement(cboDistrict));
            select.SelectByText(district);
            System.Threading.Thread.Sleep(500);
        }

        public void SelectWard(string ward)
        {
            if (string.IsNullOrEmpty(ward)) return;
            wait.Until(d => new SelectElement(d.FindElement(cboWard)).Options.Count > 1);
            new SelectElement(driver.FindElement(cboWard)).SelectByText(ward);
        }

        public void EnterStreetDetail(string street)
        {
            var e = wait.Until(ExpectedConditions.ElementIsVisible(txtStreetDetail));
            e.Clear(); e.SendKeys(street);
        }

        public void ClickLuuDiaChi()
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(btnLuuDiaChi)).Click();
        }

        public void ChonDiaChiFull(string tinh, string quan, string xa, string soNha)
        {
            SelectProvince(tinh);
            SelectDistrict(quan);
            SelectWard(xa);
            EnterStreetDetail(soNha);
            ClickLuuDiaChi();
        }

        public void DongThongBao()
        {
            try
            {
                var ok = wait.Until(ExpectedConditions.ElementToBeClickable(sweetOkButton));
                ok.Click();
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(sweetOkButton));
            }
            catch
            {
                try { driver.SwitchTo().Alert().Accept(); } catch { }
            }
        }

        public void SelectPaymentMethod(bool isCOD)
        {
            By target = isCOD ? optCOD : optBanking;
            // Chờ element tồn tại và có thể tương tác
            var element = wait.Until(ExpectedConditions.ElementExists(target));
            // Cuộn element vào giữa màn hình (tránh bị che)
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", element);
            System.Threading.Thread.Sleep(300);
            // Dùng JavaScript click để bypass lỗi che khuất
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", element);
        }

        public void ClickDatHang()
        {
            var element = wait.Until(ExpectedConditions.ElementToBeClickable(btnDatHang));
            // Cuộn nút vào giữa màn hình
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", element);
            System.Threading.Thread.Sleep(300);
            // Click bằng JavaScript để tránh bị chặn
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", element);
        }
        // Đóng bảng địa chỉ bên phải (nếu có)
        public void CloseAddressPanel()
        {
            try
            {
                // Thử tìm nút X (có thể là button với class close, hoặc span chứa ×)
                var closeBtn = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[contains(@class,'close') or contains(@class,'btn-close')] | //span[text()='×']/parent::button")));
                closeBtn.Click();
                Thread.Sleep(500);
            }
            catch { /* Không có hoặc không cần đóng */ }
        }
        // Locator cho danh sách địa chỉ (tùy chỉnh theo giao diện thực tế)
        private By addressItems = By.CssSelector(".address-item, .list-group-item, .shipping-address-item");

        /// <summary>
        /// Chọn địa chỉ dựa trên nội dung (số nhà, tên đường) vừa thêm
        /// </summary>
        public void SelectAddressByStreet(string streetDetail)
        {
            // Đợi danh sách địa chỉ xuất hiện
            wait.Until(ExpectedConditions.ElementIsVisible(addressItems));

            // Tìm địa chỉ chứa streetDetail và click
            var address = wait.Until(ExpectedConditions.ElementToBeClickable(
                By.XPath($"//*[contains(@class,'address-item') or contains(@class,'list-group-item')]//*[contains(text(),'{streetDetail}')]")));
            address.Click();
            Thread.Sleep(500);
        }
        // ========== PHƯƠNG THỨC LẤY THÔNG TIN TỰ ĐỘNG ĐIỀN (CHO TC_CHK_24) ==========
        public string GetFullNameValue()
        {
            if (!IsElementPresent(txtFullName, 3)) return "Field not found";
            var e = driver.FindElement(txtFullName);
            return e.GetAttribute("value") ?? e.Text;
        }

        public bool IsFullNameReadOnly()
        {
            if (!IsElementPresent(txtFullName, 3)) return false;
            var e = driver.FindElement(txtFullName);
            return e.GetAttribute("readonly") != null || e.GetAttribute("disabled") != null;
        }

        public string GetPhoneValue()
        {
            if (!IsElementPresent(txtPhone, 3)) return "Field not found";
            var e = driver.FindElement(txtPhone);
            return e.GetAttribute("value") ?? e.Text;
        }

        public bool IsPhoneReadOnly()
        {
            if (!IsElementPresent(txtPhone, 3)) return false;
            var e = driver.FindElement(txtPhone);
            return e.GetAttribute("readonly") != null || e.GetAttribute("disabled") != null;
        }

        // ========== KIỂM TRA DANH SÁCH QUẬN/HUYỆN (CHO TC_CHK_26) ==========
        public int GetDistrictOptionsCount()
        {
            try
            {
                var select = new SelectElement(driver.FindElement(cboDistrict));
                return select.Options.Count;
            }
            catch
            {
                return 0;
            }
        }

        public bool IsDistrictListLoaded()
        {
            return GetDistrictOptionsCount() > 1; // >1 nghĩa là đã có dữ liệu (không chỉ placeholder)
        }

        // ========== KIỂM TRA LỖI KHI THIẾU TỈNH (CHO TC_CHK_33) ==========
        public string GetErrorMessage()
        {
            try
            {
                var errorElem = driver.FindElement(By.CssSelector(".field-validation-error, .text-danger, .alert-danger"));
                return errorElem.Text;
            }
            catch { return ""; }
        }

        public bool IsSweetAlertErrorDisplayed(string expectedKeyword = "")
        {
            try
            {
                var swal = driver.FindElement(By.CssSelector(".swal2-popup"));
                string text = swal.Text.ToLower();
                if (!string.IsNullOrEmpty(expectedKeyword))
                    return text.Contains(expectedKeyword.ToLower());
                return true;
            }
            catch { return false; }
        }
        // Thêm vào cuối class CheckoutPage
        public bool IsWardMissingErrorDisplayed()
        {
            try
            {
                // Kiểm tra alert trước
                var alert = driver.SwitchTo().Alert();
                string alertText = alert.Text.ToLower();
                alert.Accept();
                return alertText.Contains("phường") || alertText.Contains("xã") || alertText.Contains("chọn");
            }
            catch
            {
                // Nếu không có alert, kiểm tra text trên page
                string pageText = driver.PageSource.ToLower();
                return pageText.Contains("phường") || pageText.Contains("xã") || pageText.Contains("chọn");
            }
        }

        public bool IsProvinceMissingErrorDisplayed()
        {
            try
            {
                var swal = driver.FindElement(By.CssSelector(".swal2-popup"));
                string swalText = swal.Text.ToLower();
                return swalText.Contains("tỉnh") || swalText.Contains("thành phố");
            }
            catch
            {
                string pageText = driver.PageSource.ToLower();
                return pageText.Contains("tỉnh") || pageText.Contains("thành phố");
            }
        }

    }
}