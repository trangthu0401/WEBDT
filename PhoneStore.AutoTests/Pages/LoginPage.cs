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

        public void ClickSubmitLogin()
        {
            driver.FindElement(btnSubmitLogin).Click();
        }

        // Hàm gộp thao tác đăng nhập cho Test Case ngắn gọn hơn
        public void Login(string username, string password)
        {
            EnterEmailOrPhone(username);
            EnterPassword(password);
            ClickSubmitLogin();
        }
    }
}