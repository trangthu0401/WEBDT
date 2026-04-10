using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace PhoneStore.AutoTests.Pages
{
    public class CartPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public CartPage(IWebDriver driver)
        {
            this.driver = driver;
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // =========================================================
        // LOCATORS CỰC CHUẨN (BÓC TỪ FILE SELENIUM IDE)
        // =========================================================

        // Nút Mua ngay (Dòng 25)
        private By btnMuaNgay = By.CssSelector(".btn-buy-now");

        // Nút (+) và (-) trong Giỏ hàng (Dòng 14 & 15: dùng hàm changeQty)
        private By btnTangSoLuong = By.XPath("//button[contains(@class, 'qty-btn') and contains(@onclick, '1)') and not(contains(@onclick, '-1)'))]");
        private By btnGiamSoLuong = By.XPath("//button[contains(@class, 'qty-btn') and contains(@onclick, '-1)')]");

        // Nút Thùng rác (Xóa sản phẩm) (Dòng 17)
        private By btnXoaSanPham = By.CssSelector(".btn-trash, .fa-trash-alt");

        // Nút Xác nhận Xóa của Popup SweetAlert2 (Dòng 18)
        private By btnXacNhanXoa = By.CssSelector(".swal2-confirm");

        // Ô input số lượng bắt đầu bằng chữ "qty-" (Dòng 16)
        private By txtSoLuong = By.CssSelector("input[id^='qty-']");

        // =========================================================
        // ACTIONS
        // =========================================================
        public void ClickMuaNgay() => wait.Until(ExpectedConditions.ElementToBeClickable(btnMuaNgay)).Click();
        public void TangSoLuong() => wait.Until(ExpectedConditions.ElementToBeClickable(btnTangSoLuong)).Click();
        public void GiamSoLuong() => wait.Until(ExpectedConditions.ElementToBeClickable(btnGiamSoLuong)).Click();
        public void ClickXoaSanPham() => wait.Until(ExpectedConditions.ElementToBeClickable(btnXoaSanPham)).Click();

        public void ConfirmXoaSweetAlert()
        {
            try
            {
                // Chờ cái Popup SweetAlert hiện lên rồi bấm nút Xóa/OK
                wait.Until(ExpectedConditions.ElementToBeClickable(btnXacNhanXoa)).Click();
            }
            catch { }
        }

        public int GetSoLuongHienTai()
        {
            try
            {
                string val = wait.Until(ExpectedConditions.ElementIsVisible(txtSoLuong)).GetAttribute("value");
                return int.TryParse(val, out int qty) ? qty : 0;
            }
            catch { return 0; }
        }

        public string GetTongTienHienTai()
        {
            try
            {
                // Quét tìm tất cả các chữ chứa ký hiệu tiền tệ (₫) trên màn hình. 
                // Dựa vào file CSV dòng 34, giá tiền hiển thị dạng 36,000,000₫
                var elements = driver.FindElements(By.XPath("//*[contains(text(), '₫') or contains(text(), 'VNĐ')]"));
                if (elements.Count > 0)
                {
                    // Lấy cục tiền ở dưới cùng (thường là Tổng tiền thanh toán)
                    return elements[elements.Count - 1].Text;
                }
                return "0";
            }
            catch { return "0"; }
        }
    }
}