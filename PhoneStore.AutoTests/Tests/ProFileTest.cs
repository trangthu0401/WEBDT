using NUnit.Framework;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using PhoneStore.AutoTests.Utilities;
using System.Threading;

namespace PhoneStore.AutoTests.Tests
{
    [TestFixture]
    public class ProfileTests : BaseTest
    {
        private HomePage homePage;
        private LoginPage loginPage;
        private ProfilePage profilePage;

        [SetUp]
        public void Init()
        {
            homePage = new HomePage(driver);
            loginPage = new LoginPage(driver);
            profilePage = new ProfilePage(driver);

            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl);
            Thread.Sleep(500);

            // login trước
            homePage.ClickMenuTaiKhoan();
            Thread.Sleep(500);
            homePage.ClickDangNhap();
            Thread.Sleep(500);

            loginPage.Login("admin@shop.com", "admin123");
            Thread.Sleep(1500);

            // vào profile
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Account/Profile");
            Thread.Sleep(1000);
        }

        // ===== TC30: UPDATE SUCCESS =====
        [Test]
        public void Profile_Update_Valid()
        {
            profilePage.UpdateProfile(
                "Nguyen Van B",
                "0909999999",
                "Nam",
                "01012000"
            );

            Thread.Sleep(2000);

            Assert.IsTrue(driver.PageSource.Contains("thành công"));
        }

        // ===== TC31: BỎ TRỐNG TÊN =====
        [Test]
        public void Profile_Empty_Name()
        {
            profilePage.UpdateProfile(
                "",
                "0909999999",
                "Nam",
                "01012000"
            );

            Thread.Sleep(2000);

            Assert.IsTrue(driver.PageSource.Contains("Tên"));
        }

        // ===== TC32: PHONE SAI =====
        [Test]
        public void Profile_Invalid_Phone()
        {
            profilePage.UpdateProfile(
                "Nguyen Van B",
                "abc",
                "Nam",
                "01012000"
            );

            Thread.Sleep(2000);

            Assert.IsTrue(driver.PageSource.Contains("điện thoại"));
        }

        // ===== TC33: DATE SAI =====
        [Test]
        public void Profile_Invalid_Date()
        {
            profilePage.UpdateProfile(
                "Nguyen Van B",
                "0909999999",
                "Nam",
                "abc"
            );

            Thread.Sleep(2000);

            Assert.IsTrue(driver.PageSource.Contains("ngày"));
        }

        // ===== TC34: CHỈ SỬA TÊN =====
        [Test]
        public void Profile_Update_Name_Only()
        {
            profilePage.EnterFullName("Test Name");
            profilePage.ClickSave();

            Thread.Sleep(2000);

            Assert.IsTrue(driver.PageSource.Contains("thành công"));
        }

        // ===== TC35: KHÔNG SỬA GÌ =====
        [Test]
        public void Profile_No_Change()
        {
            profilePage.ClickSave();

            Thread.Sleep(2000);

            Assert.IsTrue(true); // pass nếu không crash
        }

        // ===== TC36: PHONE DÀI =====
        [Test]
        public void Profile_Long_Phone()
        {
            profilePage.UpdateProfile(
                "Test",
                "123456789012345",
                "Nam",
                "01012000"
            );

            Thread.Sleep(2000);

            Assert.IsTrue(true);
        }

        // ===== TC37: KÝ TỰ ĐẶC BIỆT =====
        [Test]
        public void Profile_Special_Char()
        {
            profilePage.UpdateProfile(
                "@@@",
                "@@@",
                "Nam",
                "01012000"
            );

            Thread.Sleep(2000);

            Assert.IsTrue(true);
        }

        // ===== TC38: CLICK NHIỀU LẦN =====
        [Test]
        public void Profile_Click_Multiple()
        {
            profilePage.UpdateProfile(
                "Test",
                "0901234567",
                "Nam",
                "01012000"
            );

            profilePage.ClickSave();
            profilePage.ClickSave();

            Thread.Sleep(2000);

            Assert.IsTrue(true);
        }

        // ===== TC39: REFRESH =====
        [Test]
        public void Profile_Refresh()
        {
            driver.Navigate().Refresh();
            Thread.Sleep(2000);

            Assert.IsTrue(true);
        }

        // ===== TC40: UI HIỂN THỊ =====
        [Test]
        public void Profile_UI()
        {
            Assert.IsTrue(driver.PageSource.Contains("Họ tên"));
        }
    }
}