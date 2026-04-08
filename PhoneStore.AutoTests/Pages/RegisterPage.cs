using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace PhoneStore.AutoTests.Pages
{
    public class RegisterPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public RegisterPage(IWebDriver driver)
        {
            this.driver = driver;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // ===== LOCATORS =====
        private By txtFullName = By.Id("FullName");
        private By txtPhone = By.Id("Phone");
        private By txtGender = By.Id("Gender"); // nếu dropdown thì đổi lại Select
        private By txtBirthDate = By.Id("BirthDate");

        private By txtEmail = By.Id("Email");
        private By txtPassword = By.Id("Password");
        private By txtConfirm = By.Id("ConfirmPassword");

        // FIX NÚT REGISTER (KHÔNG DÙNG TEXT NỮA)
        private By btnRegister = By.XPath("//button[@type='submit']");

        // ===== ACTIONS =====
        public void EnterFullName(string name)
        {
            var e = wait.Until(ExpectedConditions.ElementIsVisible(txtFullName));
            e.Clear();
            e.SendKeys(name);
        }

        public void EnterPhone(string phone)
        {
            var e = wait.Until(ExpectedConditions.ElementIsVisible(txtPhone));
            e.Clear();
            e.SendKeys(phone);
        }

        public void EnterGender(string gender)
        {
            try
            {
                var e = wait.Until(ExpectedConditions.ElementIsVisible(txtGender));
                e.Clear();
                e.SendKeys(gender);
            }
            catch { }
        }

        public void EnterBirthDate(string date)
        {
            var e = wait.Until(ExpectedConditions.ElementIsVisible(txtBirthDate));
            e.Clear();
            e.SendKeys(date); // vd: 01012000
        }

        public void EnterEmail(string email)
        {
            var e = wait.Until(ExpectedConditions.ElementIsVisible(txtEmail));
            e.Clear();
            e.SendKeys(email);
        }

        public void EnterPassword(string password)
        {
            var e = wait.Until(ExpectedConditions.ElementIsVisible(txtPassword));
            e.Clear();
            e.SendKeys(password);
        }

        public void EnterConfirm(string confirm)
        {
            var e = wait.Until(ExpectedConditions.ElementIsVisible(txtConfirm));
            e.Clear();
            e.SendKeys(confirm);
        }

        // 🔥 FIX CLICK 100%
        public void ClickRegister()
        {
            IWebElement btn = wait.Until(ExpectedConditions.ElementExists(btnRegister));

            // scroll xuống
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", btn);
            System.Threading.Thread.Sleep(300);

            try
            {
                wait.Until(ExpectedConditions.ElementToBeClickable(btnRegister));
                btn.Click();
            }
            catch
            {
                // fallback JS click
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", btn);
            }
        }

        // ===== GỘP =====
        public void Register(string name, string phone, string gender,
                             string birth, string email, string pass, string confirm)
        {
            EnterFullName(name);
            EnterPhone(phone);
            EnterGender(gender);
            EnterBirthDate(birth);
            EnterEmail(email);
            EnterPassword(pass);
            EnterConfirm(confirm);
            ClickRegister();
        }
    }
}