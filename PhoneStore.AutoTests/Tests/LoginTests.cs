using NUnit.Framework;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using System.Threading;

namespace PhoneStore.AutoTests.Tests
{
    [TestFixture]
    public class LoginTests : BaseTest // Kế thừa BaseTest để tự mở Chrome
    {
        private HomePage homePage;
        private LoginPage loginPage;

        [SetUp]
        public void Init()
        {
            // Khởi tạo các trang
            homePage = new HomePage(driver);
            loginPage = new LoginPage(driver);
        }

      
    }
}