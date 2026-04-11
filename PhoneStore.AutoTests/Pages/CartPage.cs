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


        // Lấy số lượng của sản phẩm đầu tiên (giả định chỉ có 1 loại sản phẩm)
        public int GetSoLuongHienTai()
        {
            try
            {
                // Tìm input số lượng (có thể là input với id bắt đầu bằng "qty-" hoặc class "quantity")
                var qtyInput = wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id^='qty-'], input.quantity")));
                string val = qtyInput.GetAttribute("value");
                return int.TryParse(val, out int qty) ? qty : 0;
            }
            catch { return 0; }
        }

        // Đếm số lượng sản phẩm (dòng) trong giỏ
        public int GetSoLuongSanPham()
        {
            try
            {
                // Locator cho mỗi dòng sản phẩm (thường là <tr> hoặc div có class cart-item)
                var rows = driver.FindElements(By.CssSelector(".cart-item, .product-row, tr.cart-item"));
                return rows.Count;
            }
            catch { return 0; }
        }

        // Nếu có nhiều sản phẩm, lấy số lượng của sản phẩm theo tên
        public int GetSoLuongTheoTenSanPham(string productName)
        {
            var qtyInput = wait.Until(ExpectedConditions.ElementExists(
                By.XPath($"//tr[contains(.,'{productName}')]//input[starts-with(@id,'qty-')]")));
            string val = qtyInput.GetAttribute("value");
            return int.TryParse(val, out int qty) ? qty : 0;
        }

        public string GetTongTienHienTai()
        {
            try
            {
                // Dùng locator chính xác từ id grandTotalDisplay
                var totalElement = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("grandTotalDisplay")));
                return totalElement.Text.Trim();
            }
            catch
            {
                // Fallback: tìm element chứa ₫ hoặc VNĐ
                try
                {
                    var elements = driver.FindElements(By.XPath("//*[contains(text(), '₫') or contains(text(), 'VNĐ')]"));
                    if (elements.Count > 0)
                        return elements[elements.Count - 1].Text.Trim();
                }
                catch { }
                return "";
            }
        }
        public int GetSoLuongSanPhamTrongGio()
        {
            return driver.FindElements(By.CssSelector(".cart-item, .cart-row, tbody tr")).Count;
        }
    }
}