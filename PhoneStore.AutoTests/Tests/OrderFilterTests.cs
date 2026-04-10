using NUnit.Framework;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using PhoneStore.AutoTests.Utilities;
using System;

namespace PhoneStore.AutoTests.Tests
{
    [TestFixture]
    public class OrderFilterTests : BaseTest
    {
        private LoginPage loginPage;
        private OrderFilterPage filterPage;

        [SetUp]
        public void PreTest()
        {
            loginPage = new LoginPage(driver);
            filterPage = new OrderFilterPage(driver);

            // 1. Đăng nhập Admin
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Account/Login");
            loginPage.Login("admin@shop.com", "admin123", isAdmin: true);

            // 2. Click Menu Đơn hàng trên Sidebar
            filterPage.GoToOrderMenu();
        }

        [Test, Order(1)]
        [Property("TC_ID", "TC_ORDER_13")]
        public void TC_ORDER_13_Filter_Shipping()
        {
            string status = "Đang giao";
            filterPage.SelectStatus(status);
            filterPage.ClickFilter();
            Assert.That(filterPage.GetFirstOrderStatus().ToUpper(), Contains.Substring(status.ToUpper()));
        }

        [Test, Order(2)]
        [Property("TC_ID", "TC_ORDER_14")]
        public void TC_ORDER_14_Filter_Pending()
        {
            string status = "Chờ xác nhận";
            filterPage.SelectStatus(status);
            filterPage.ClickFilter();
            Assert.That(filterPage.GetFirstOrderStatus().ToUpper(), Contains.Substring(status.ToUpper()));
        }

        [Test, Order(3)]
        [Property("TC_ID", "TC_ORDER_15")]
        public void TC_ORDER_15_Filter_Delivered()
        {
            string status = "Đã giao";
            filterPage.SelectStatus(status);
            filterPage.ClickFilter();
            Assert.That(filterPage.GetFirstOrderStatus().ToUpper(), Contains.Substring(status.ToUpper()));
        }

        [Test, Order(4)]
        [Property("TC_ID", "TC_ORDER_16")]
        public void TC_ORDER_16_Filter_Cancelled()
        {
            string status = "Đã hủy";
            filterPage.SelectStatus(status);
            filterPage.ClickFilter();
            Assert.That(filterPage.GetFirstOrderStatus().ToUpper(), Contains.Substring(status.ToUpper()));
        }
    }
}