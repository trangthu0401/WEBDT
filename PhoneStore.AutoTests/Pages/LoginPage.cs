using OpenQA.Selenium;

namespace PhoneStore.AutoTests.Pages
{
    public class LoginPage
    {
        private IWebDriver driver;

        public LoginPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        // --- 1. Locators ---
        private By txtEmailOrPhone = By.Id("EmailOrPhone");
        private By txtPassword = By.Id("Password");
        private By btnSubmitLogin = By.XPath("//button[contains(text(),'Đăng nhập')]");

        // Selector cho checkbox Admin (Dựa trên hình ảnh giao diện của Vy)
        private By chkAdminRole = By.XPath("//label[contains(text(),'Admin')]/preceding-sibling::input | //input[following-sibling::label[contains(text(),'Admin')]] | //label[contains(.,'Admin')]//input");

        // --- 2. Actions ---
        public void EnterEmailOrPhone(string emailOrPhone)
        {
            var element = driver.FindElement(txtEmailOrPhone);
            element.Clear();
            element.SendKeys(emailOrPhone);
        }

        public void EnterPassword(string password)
        {
            var element = driver.FindElement(txtPassword);
            element.Clear();
            element.SendKeys(password);
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
            driver.FindElement(btnSubmitLogin).Click();
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