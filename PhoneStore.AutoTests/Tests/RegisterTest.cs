using NUnit.Framework;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using PhoneStore.AutoTests.Utilities;
using System.Threading;

namespace PhoneStore.AutoTests.Tests
{
    [TestFixture]
    public class RegisterTests : BaseTest
    {
        private HomePage homePage;
        private RegisterPage registerPage;

        [SetUp]
        public void Init()
        {
            homePage = new HomePage(driver);
            registerPage = new RegisterPage(driver);

            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl);
            Thread.Sleep(500);

            homePage.ClickMenuTaiKhoan();
            Thread.Sleep(500);

            homePage.ClickDangKy();
            Thread.Sleep(1000);
        }

        // ===== TC11: ĐĂNG KÝ THÀNH CÔNG =====
        [Test]
        public void Register_Valid()
        {
            registerPage.Register(
                "Nguyen Van A",
                "0901234567",
                "Nam",
                "01012000",
                "test" + System.DateTime.Now.Ticks + "@gmail.com",
                "123456",
                "123456"
            );

            Thread.Sleep(2000);

            Assert.IsTrue(
                driver.PageSource.Contains("thành công") ||
                driver.PageSource.Contains("Trang chủ")
            );
        }

        // ===== TC12: EMAIL ĐÃ TỒN TẠI =====
        [Test]
        public void Register_Email_Exist()
        {
            registerPage.Register(
                "Nguyen Van A",
                "0901234567",
                "Nam",
                "01012000",
                "admin@shop.com",
                "123456",
                "123456"
            );

            Thread.Sleep(2000);

            Assert.IsTrue(
                driver.PageSource.Contains("tồn tại") ||
                driver.PageSource.Contains("đã tồn tại")
            );
        }

        // ===== TC13: THIẾU EMAIL =====
        [Test]
        public void Register_Empty_Email()
        {
            registerPage.Register(
                "Nguyen Van A",
                "0901234567",
                "Nam",
                "01012000",
                "",
                "123456",
                "123456"
            );

            Thread.Sleep(2000);

            Assert.IsTrue(driver.PageSource.Contains("Email"));
        }

        // ===== TC14: PASSWORD KHÔNG KHỚP =====
        [Test]
        public void Register_Confirm_NotMatch()
        {
            registerPage.Register(
                "Nguyen Van A",
                "0901234567",
                "Nam",
                "01012000",
                "test123@gmail.com",
                "123456",
                "111111"
            );

            Thread.Sleep(2000);

            Assert.IsTrue(driver.PageSource.Contains("không khớp"));
        }
    }
}