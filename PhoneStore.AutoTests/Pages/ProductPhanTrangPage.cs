using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace PhoneStore.AutoTests.Pages
{
    public class ProductPhanTrangPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public ProductPhanTrangPage(IWebDriver driver)
        {
            this.driver = driver;
            // Đợi tối đa 15 giây để đảm bảo bảng load xong trên localhost
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        }

        // --- LOCATORS ---

        // Tìm nút số trang (1, 2, 3...) dựa theo nội dung text bên trong thẻ <a>
        private By PageNode(string pageNum) => By.XPath($"//ul[contains(@class,'pagination')]//a[text()='{pageNum}']");

        // Tìm thẻ <li> nào đang chứa class 'active' (để biết Robot đang ở trang nào)
        private By activePage = By.XPath("//ul[contains(@class,'pagination')]//li[contains(@class,'active')]");

        // --- ACTIONS ---

        /// <summary>
        /// Click chuyển sang một số trang cụ thể
        /// </summary>
        public void ClickToPage(string pageNum)
        {
            // 1. Đợi nút số trang có thể click được
            var btn = wait.Until(ExpectedConditions.ElementToBeClickable(PageNode(pageNum)));

            // 2. Cuộn màn hình xuống dưới cùng để nút hiện ra (tránh bị các element khác che)
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'});", btn);

            // Nghỉ một chút để trình duyệt cuộn xong
            System.Threading.Thread.Sleep(1000);

            // 3. Thực hiện Click
            btn.Click();

            // 4. Nghỉ 2 giây để bảng đổ dữ liệu mới (Ajax load)
            System.Threading.Thread.Sleep(2000);
        }

        /// <summary>
        /// Lấy số trang hiện tại đang hiển thị màu đỏ (Active)
        /// </summary>
        public string GetCurrentActivePage()
        {
            try
            {
                // Đợi thẻ li active hiện ra và lấy text của nó
                var element = wait.Until(ExpectedConditions.ElementIsVisible(activePage));
                return element.Text.Trim();
            }
            catch
            {
                // Nếu không thấy (lỗi load), mặc định coi như đang ở trang 1
                return "1";
            }
        }

        /// <summary>
        /// Lấy URL hiện tại để kiểm tra tham số ?page=
        /// </summary>
        public string GetCurrentUrl()
        {
            return driver.Url;
        }

        /// <summary>
        /// Lấy ID của sản phẩm đầu tiên trong bảng (dùng để so sánh xem dữ liệu có thực sự đổi khi qua trang không)
        /// </summary>
        public string GetFirstProductID()
        {
            try
            {
                return driver.FindElement(By.XPath("//table//tbody/tr[1]/td[1]")).Text.Trim();
            }
            catch { return ""; }
        }
    }
}