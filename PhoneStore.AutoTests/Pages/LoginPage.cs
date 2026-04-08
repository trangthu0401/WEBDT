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
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        }

        private By txtEmail = By.Id("EmailOrPhone"); // nhớ đúng id nha
        private By txtPassword = By.Id("Password");
        private By btnLogin = By.XPath("//button[contains(text(),'Đăng nhập')]");

        // Selector cho checkbox Admin (Dựa trên hình ảnh giao diện của Vy)
        private By chkAdminRole = By.XPath("//label[contains(text(),'Admin')]/preceding-sibling::input | //input[following-sibling::label[contains(text(),'Admin')]] | //label[contains(.,'Admin')]//input");

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

        public void Login(string email, string password)
        {
            EnterEmailOrPhone(email);
            EnterPassword(password);
            ClickSubmitLogin();
        }

        // Hàm mới: Tick vào ô đăng nhập quyền Admin
        public void SelectAdminRole(bool wantAdmin)
        {
            var checkbox = driver.FindElement(chkAdminRole);
            if (wantAdmin && !checkbox.Selected)
            {
                checkbox.Click();
            }
            else if (!wantAdmin && checkbox.Selected)
            {
                checkbox.Click(); // Bỏ tick nếu là khách thường
            }
        }

        public void ClickSubmitLogin()
        {
            driver.FindElement(btnLogin).Click();
        }

        // Hàm Login cập nhật: có thêm biến isAdmin
        public void Login(string username, string password, bool isAdmin = false)
        {
            EnterEmailOrPhone(username);
            EnterPassword(password);
            SelectAdminRole(isAdmin); // Xử lý yêu cầu click checkbox Admin
            ClickSubmitLogin();
        }
    }
}