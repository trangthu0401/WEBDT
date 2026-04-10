using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace PhoneStore.AutoTests.Pages
{
    public class OrderFilterPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public OrderFilterPage(IWebDriver driver)
        {
            this.driver = driver;
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        }

        // --- LOCATORS ---
        // Menu "Đơn hàng" ở Sidebar bên trái (Dựa theo ảnh nút đỏ của Vy)
        private By menuOrder = By.XPath("//a[contains(@href, 'Order')] | //span[contains(.,'Đơn hàng')]/parent::a | //a[contains(.,'Đơn hàng')]");

        // Dropdown trạng thái đơn hàng
        private By ddStatus = By.XPath("//select[contains(@class, 'form-select')] | //select");

        // Nút Lọc màu xanh
        private By btnFilter = By.XPath("//button[contains(.,'Lọc')]");

        // Sửa td[6] thành td[5] vì Trạng thái nằm ở cột thứ 5 Vy nhé!
        private By firstRowStatusLabel = By.XPath("//table//tbody/tr[1]/td[5]");

        // --- ACTIONS ---

        // Hàm click vào Menu Đơn hàng thay vì nhập URL trực tiếp
        public void GoToOrderMenu()
        {
            var menu = wait.Until(ExpectedConditions.ElementToBeClickable(menuOrder));
            menu.Click();
            // Đợi Dropdown hiện ra để chắc chắn trang đã load xong
            wait.Until(ExpectedConditions.ElementIsVisible(ddStatus));
        }

        public void SelectStatus(string statusText)
        {
            var selectElement = wait.Until(ExpectedConditions.ElementIsVisible(ddStatus));

            // Cuộn tới Dropdown để tránh bị che khuất
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", selectElement);

            var select = new SelectElement(selectElement);
            select.SelectByText(statusText);
        }

        public void ClickFilter()
        {
            driver.FindElement(btnFilter).Click();
            // Đợi Ajax load bảng (tầm 2 giây là ổn cho localhost)
            System.Threading.Thread.Sleep(2000);
        }

        public string GetFirstOrderStatus()
        {
            try
            {
                // Robot sẽ đợi cho đến khi cái ô ở cột 5 có chữ hiện lên
                var element = wait.Until(d => {
                    var el = d.FindElement(By.XPath("//table//tbody/tr[1]/td[5]"));
                    return !string.IsNullOrEmpty(el.Text.Trim()) ? el : null;
                });

                string result = element.Text.Trim();
                Console.WriteLine("Robot đã nhìn thấy: " + result);
                return result;
            }
            catch
            {
                return "TRỐNG";
            }
        }
    }
}