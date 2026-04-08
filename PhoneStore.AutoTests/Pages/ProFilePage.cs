using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace PhoneStore.AutoTests.Pages
{
    public class ProfilePage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public ProfilePage(IWebDriver driver)
        {
            this.driver = driver;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // ===== LOCATORS =====
        private By txtFullName = By.Id("FullName");
        private By txtPhone = By.Id("Phone");
        private By txtGender = By.Id("Gender");
        private By txtBirthDate = By.Id("BirthDate");

        private By btnSave = By.XPath("//button[contains(text(),'Lưu') or @type='submit']");

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
            e.SendKeys(date);
        }

        public void ClickSave()
        {
            IWebElement btn = wait.Until(ExpectedConditions.ElementExists(btnSave));

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", btn);
            System.Threading.Thread.Sleep(300);

            try
            {
                wait.Until(ExpectedConditions.ElementToBeClickable(btnSave));
                btn.Click();
            }
            catch
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", btn);
            }
        }

        // ===== GỘP =====
        public void UpdateProfile(string name, string phone, string gender, string birth)
        {
            EnterFullName(name);
            EnterPhone(phone);
            EnterGender(gender);
            EnterBirthDate(birth);
            ClickSave();
        }
    }
}