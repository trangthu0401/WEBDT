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

        public void Login(string email, string password)
        {
            // 🔥 CHỜ ELEMENT HIỆN RA RỒI NHẬP NGAY
            var emailBox = wait.Until(ExpectedConditions.ElementIsVisible(txtEmail));
            emailBox.Clear();
            emailBox.SendKeys(email);

            var passBox = wait.Until(ExpectedConditions.ElementIsVisible(txtPassword));
            passBox.Clear();
            passBox.SendKeys(password);

            wait.Until(ExpectedConditions.ElementToBeClickable(btnLogin)).Click();
        }
    }
}