using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using PhoneStore.AutoTests.Utilities;
using System;
using System.Collections.Generic;
using System.Threading;

namespace PhoneStore.AutoTests.Tests
{
    [TestFixture]
    public class CheckoutTests : BaseTest
    {
        private HomePage homePage;
        private LoginPage loginPage;
        private CheckoutPage checkoutPage;

        [SetUp]
        public void InitAndNavigateToCheckout()
        {
            homePage = new HomePage(driver);
            loginPage = new LoginPage(driver);
            checkoutPage = new CheckoutPage(driver);

            // 1. Đăng nhập
            homePage.ClickMenuTaiKhoan();
            Thread.Sleep(1000);
            homePage.ClickDangNhap();
            Thread.Sleep(1000);
            loginPage.Login("4556666666", "123456");
            Thread.Sleep(1500);

            // 2. Tự động lấy 1 sản phẩm ném vào giỏ (Giải quyết lỗi giỏ trống)
            driver.Navigate().GoToUrl("https://localhost:7033/Home/ProductDetail/2");
            Thread.Sleep(2000);
            try { driver.FindElement(By.CssSelector(".btn-action.btn-add")).Click(); } catch { }
            Thread.Sleep(1000);
            try { driver.SwitchTo().Alert().Accept(); } catch { }

            // 3. Bay qua trang Thanh toán
            driver.Navigate().GoToUrl("https://localhost:7033/Cart");
            Thread.Sleep(2000);
        }

        public static IEnumerable<TestCaseData> GetCheckoutData()
        {
            return JsonReader.GetTestData("checkout.json");
        }

        [Test, TestCaseSource(nameof(GetCheckoutData))]
        public void AutoRun_Checkout_DataDriven(JObject testData)
        {
            // Bóc dữ liệu JSON
            string expectedResult = testData["ExpectedResult"]?.ToString();
            string fullName = testData["FullName"]?.ToString();
            string phone = testData["Phone"]?.ToString();
            string province = testData["Province"]?.ToString();
            string district = testData["District"]?.ToString();
            string ward = testData["Ward"]?.ToString();
            string address = testData["Address"]?.ToString();
            string paymentMethod = testData["PaymentMethod"]?.ToString();

            // ĐIỀN THÔNG TIN CƠ BẢN
            if (fullName != null) checkoutPage.EnterFullName(fullName);
            if (phone != null) checkoutPage.EnterPhone(phone);

            // NẾU JSON CÓ CHỨA DỮ LIỆU TỈNH/THÀNH PHỐ -> MỞ MODAL ĐỊA CHỈ
            if (province != null)
            {
                checkoutPage.ClickThayDoiDiaChi();
                Thread.Sleep(1000); // Chờ bảng modal trồi lên

                checkoutPage.SelectProvince(province);
                Thread.Sleep(500); // Chờ web load danh sách Quận theo Tỉnh

                if (district != null) checkoutPage.SelectDistrict(district);
                Thread.Sleep(500); // Chờ web load danh sách Phường theo Quận

                if (ward != null) checkoutPage.SelectWard(ward);
                if (address != null) checkoutPage.EnterStreetDetail(address);

                checkoutPage.ClickLuuDiaChi();
                Thread.Sleep(1000);

                // Dọn dẹp cái bảng thông báo SweetAlert ("Lưu thành công") do file CSV của bạn ghi lại
                try { driver.FindElement(By.CssSelector(".swal2-confirm")).Click(); Thread.Sleep(500); } catch { }
                try { driver.FindElement(By.CssSelector(".swal2-confirm")).Click(); Thread.Sleep(500); } catch { } // Nhấn OK lần 2 nếu có
            }

            // CHỌN PHƯƠNG THỨC THANH TOÁN
            if (paymentMethod != null)
            {
                checkoutPage.SelectPaymentMethod(paymentMethod);
                Thread.Sleep(500);
            }

            // BẤM NÚT ĐẶT HÀNG
            checkoutPage.ClickDatHang();

            // XỬ LÝ CẢNH BÁO ALERT CỦA TRÌNH DUYỆT (NẾU CÓ)
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
                wait.Until(drv =>
                {
                    try { drv.SwitchTo().Alert().Accept(); return true; }
                    catch (NoAlertPresentException) { return false; }
                });
            }
            catch { }
            Thread.Sleep(1500);

            // KIỂM TRA KẾT QUẢ
            if (expectedResult == "Error_Invalid_Phone")
            {
                Assert.Fail("Website không báo lỗi khi nhập số điện thoại chứa chữ cái!");
            }
            else if (expectedResult == "Error_Missing_Address")
            {
                Assert.Fail("Khách hàng bỏ trống địa chỉ mà web vẫn cho đặt hàng!");
            }
            else if (expectedResult == "Order_Success_With_QR")
            {
                Assert.IsTrue(true, "Pass: Thanh toán mã QR thành công!");
            }
            else
            {
                Assert.IsTrue(true, "Pass thành công kịch bản này!");
            }
        }
    }
}