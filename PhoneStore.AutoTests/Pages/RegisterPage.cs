using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace PhoneStore.AutoTests.Pages
{
    public class RegisterPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public RegisterPage(IWebDriver driver)
        {
            this.driver = driver;
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // ===== LOCATORS =====
        private By txtFullName = By.Id("FullName");
        private By txtPhone = By.Id("Phone");
        private By txtGender = By.Id("Gender");
        private By txtBirthDate = By.Id("BirthDate");
        private By txtEmail = By.Id("Email");
        private By txtPassword = By.Id("Password");
        private By txtConfirm = By.Id("ConfirmPassword");

        private By btnRegister = By.XPath("//button[contains(text(),'Đăng ký')]");

        // ===== ACTIONS =====
        public void EnterFullName(string value)
        {
            try { var e = driver.FindElement(txtFullName); e.Clear(); e.SendKeys(value); } catch { }
        }

        public void EnterPhone(string value)
        {
            try { var e = driver.FindElement(txtPhone); e.Clear(); e.SendKeys(value); } catch { }
        }

        public void EnterGender(string value)
        {
            try { var e = driver.FindElement(txtGender); e.Clear(); e.SendKeys(value); } catch { }
        }

        public void EnterBirthDate(string value)
        {
            try { var e = driver.FindElement(txtBirthDate); e.Clear(); e.SendKeys(value); } catch { }
        }

        public void EnterEmail(string value)
        {
            try { var e = driver.FindElement(txtEmail); e.Clear(); e.SendKeys(value); } catch { }
        }

        public void EnterPassword(string value)
        {
            try { var e = driver.FindElement(txtPassword); e.Clear(); e.SendKeys(value); } catch { }
        }

        public void EnterConfirmPassword(string value)
        {
            try { var e = driver.FindElement(txtConfirm); e.Clear(); e.SendKeys(value); } catch { }
        }

        public void ClickRegister()
        {
            try { driver.FindElement(btnRegister).Click(); } catch { }
        }

        // 👉 Hàm gộp
        public void Register(string name, string phone, string gender, string dob, string email, string pass, string confirm)
        {
            EnterFullName(name);
            EnterPhone(phone);
            EnterGender(gender);
            EnterBirthDate(dob);
            EnterEmail(email);
            EnterPassword(pass);
            EnterConfirmPassword(confirm);
            ClickRegister();
        }
    }
}