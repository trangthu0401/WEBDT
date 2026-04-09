using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace PhoneStore.AutoTests.Pages
{
    public class LoginPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public LoginPage(IWebDriver driver)
        {
            this.driver = driver;
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // --- 1. Locators ---
        private By txtEmail = By.Id("EmailOrPhone");
        private By txtPassword = By.Id("Password");
        private By btnLogin = By.XPath("//button[contains(text(),'Đăng nhập')]");

        // Selector cho checkbox Admin (Tối ưu để Robot dễ tìm thấy)
        private By chkAdminRole = By.XPath("//label[contains(.,'Admin')]//input | //input[@type='checkbox' and contains(@id, 'Admin')]");

        // --- 2. Actions ---

        public void EnterEmailOrPhone(string emailOrPhone)
        {
            var emailBox = wait.Until(ExpectedConditions.ElementIsVisible(txtEmail));
            emailBox.Clear();
            emailBox.SendKeys(emailOrPhone);
        }

        public void EnterPassword(string password)
        {
            var passBox = wait.Until(ExpectedConditions.ElementIsVisible(txtPassword));
            passBox.Clear();
            passBox.SendKeys(password);
        }

        public void SelectAdminRole(bool wantAdmin)
        {
            try
            {
                var checkbox = driver.FindElement(chkAdminRole);
                if (wantAdmin && !checkbox.Selected)
                {
                    checkbox.Click();
                }
                else if (!wantAdmin && checkbox.Selected)
                {
                    checkbox.Click(); // Bỏ chọn nếu là khách
                }
            }
            catch
            {
                // Nếu trang Login không có checkbox thì bỏ qua để tránh văng lỗi
            }
        }

        public void ClickSubmitLogin()
        {
            wait.Until(ExpectedConditions.ElementToBeClickable(btnLogin)).Click();
        }

        // --- 3. Hàm Login "Vạn Năng" ---
        // Có isAdmin = false là tham số tùy chọn. Giúp các file Test cũ không bị lỗi CS1739.
        public void Login(string username, string password, bool isAdmin = false)
        {
            EnterEmailOrPhone(username);
            EnterPassword(password);

            // Tự động thông minh: Nếu isAdmin là true HOẶC email có chữ "admin" thì tích ô Admin
            bool finalAdminDecision = isAdmin || username.ToLower().Contains("admin");
            SelectAdminRole(finalAdminDecision);

            ClickSubmitLogin();
        }
    }
}