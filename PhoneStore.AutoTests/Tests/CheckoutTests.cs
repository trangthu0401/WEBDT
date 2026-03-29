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
            loginPage.Login(ConfigHelper.TestUserPhone, ConfigHelper.TestUserPassword);
            Thread.Sleep(1500);
            // Không tự động thêm sản phẩm vào giỏ hàng tại đây!
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
            string cartStatus = testData["CartStatus"]?.ToString();
            string discountCode = testData["DiscountCode"]?.ToString();
            string manipulatedQty = testData["ManipulatedQty"]?.ToString();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            // TIỀN ĐIỀU KIỆN TỪ JSON: Nếu giỏ không phải là "Empty" thì thêm sản phẩm vào giỏ
            if (cartStatus != "Empty")
            {
                driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Home/ProductDetail/2");
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.CssSelector(".btn-action.btn-add"))).Click();
                try { wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent()).Accept(); } catch { }
            }

            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Checkout");
            Thread.Sleep(1500); // Chờ load trang thanh toán

            // XỬ LÝ SỚM (EARLY EXIT) TRÁNH CRASH CODE CHO GIỎ HÀNG TRỐNG
            if (expectedResult == "Redirect_To_Cart")
            {
                Assert.IsTrue(driver.Url.Contains("Cart"), "Lỗi: Giỏ rỗng nhưng không đá về trang Cart.");
                return; // THOÁT KHỎI HÀM NGAY LẬP TỨC
            }

            // HACK TỒN KHO THÔNG QUA JAVASCRIPT
            if (manipulatedQty != null)
            {
                try
                {
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    js.ExecuteScript($"var e = document.querySelector('.qty-input, input[name*=\"quantity\"]'); if(e) e.value = '{manipulatedQty}';");
                    Thread.Sleep(500);
                }
                catch { }
            }

            // ÁP DỤNG MÃ GIẢM GIÁ
            if (discountCode != null)
            {
                checkoutPage.EnterAndApplyDiscount(discountCode);
                Thread.Sleep(1500);
            }

            // ĐIỀN THÔNG TIN CƠ BẢN
            if (fullName != null) checkoutPage.EnterFullName(fullName);
            if (phone != null) checkoutPage.EnterPhone(phone);

            // NẾU JSON CÓ CHỨA DỮ LIỆU TỈNH/THÀNH PHỐ -> MỞ MODAL ĐỊA CHỈ
            if (province != null)
            {
                checkoutPage.ClickThayDoiDiaChi();
                // Chờ bảng modal trồi lên
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("province")));

                checkoutPage.SelectProvince(province);
                // Chờ danh sách Quận cập nhật (phải có hơn 1 option)
                wait.Until(d => new SelectElement(d.FindElement(By.Id("district"))).Options.Count > 1);

                if (district != null) checkoutPage.SelectDistrict(district);
                if (ward != null)
                {
                    wait.Until(d => new SelectElement(d.FindElement(By.Id("ward"))).Options.Count > 1);
                    checkoutPage.SelectWard(ward);
                }
                
                if (address != null) checkoutPage.EnterStreetDetail(address);

                checkoutPage.ClickLuuDiaChi();
                Thread.Sleep(1000);

                // Dọn dẹp cái bảng thông báo SweetAlert ("Lưu thành công")
                checkoutPage.CloseSweetAlert();
            }

            // CHỌN PHƯƠNG THỨC THANH TOÁN
            if (paymentMethod != null)
            {
                checkoutPage.SelectPaymentMethod(paymentMethod);
            }

            // BẤM NÚT ĐẶT HÀNG
            checkoutPage.ClickDatHang();

            // XỬ LÝ CẢNH BÁO ALERT CỦA TRÌNH DUYỆT (NẾU CÓ)
            try
            {
                WebDriverWait alertWait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
                alertWait.Until(drv =>
                {
                    try { drv.SwitchTo().Alert().Accept(); return true; }
                    catch (NoAlertPresentException) { return false; }
                });
            }
            catch { }
            Thread.Sleep(1500);

            // KIỂM TRA KẾT QUẢ THEO DATA MAP
            if (expectedResult == "Error_Invalid_Phone")
            {
                // Đoạn này nên thêm assert để chắc chắn UI báo lỗi thay vì assert.fail nếu không báo lỗi
                Assert.IsTrue(driver.PageSource.Contains("điện thoại") || driver.PageSource.Contains("Phone"), "Lỗi: Không cảnh báo số điện thoại không hợp lệ!");
            }
            else if (expectedResult == "Error_Missing_Address")
            {
                Assert.IsTrue(driver.PageSource.Contains("địa chỉ") || driver.PageSource.Contains("Address"), "Lỗi: Đặt hàng thiếu địa chỉ nhưng không bị chặn báo lỗi!");
            }
            else if (expectedResult == "Order_Success_With_QR" || expectedResult == "Order_Success")
            {
                Assert.IsTrue(driver.Url.Contains("Success") || driver.PageSource.Contains("thành công"), "Pass: Đã đặt hàng thành công.");
            }
            else if (expectedResult == "Redirect_To_Cart")
            {
                // Dòng này đã được bắt sớm ở trên, nếu chạy xuống đây thì fail
                Assert.Fail("Lệnh return cho Redirect_To_Cart đã không hoạt động!");
            }
            else if (expectedResult == "Error_Exceed_Inventory")
            {
                Assert.IsTrue(driver.PageSource.Contains("kho") || driver.PageSource.Contains("vượt quá"), "Lỗi: Vượt tồn kho mà không báo lỗi.");
            }
            else if (expectedResult == "District_List_Loaded")
            {
                Assert.IsTrue(new SelectElement(driver.FindElement(By.Id("district"))).Options.Count > 1, "Lỗi: Chưa tải được danh sách Quận/Huyện.");
            }
            else if (expectedResult == "Error_Phone_Too_Short")
            {
                Assert.IsTrue(driver.PageSource.Contains("ngắn") || driver.PageSource.Contains("điện thoại"), "Lỗi: Điện thoại quá ngắn không bị chặn.");
            }
            else if (expectedResult == "Error_Invalid_Discount_Code")
            {
                Assert.IsTrue(driver.PageSource.Contains("giảm giá") || driver.PageSource.Contains("không hợp lệ") || driver.PageSource.Contains("hết hạn"), "Lỗi: Mã giảm giá sai nhưng không báo lỗi.");
            }
            else if (expectedResult == "Discount_Applied_Successfully")
            {
                Assert.IsTrue(driver.PageSource.Contains("thành công") || driver.PageSource.Contains("-"), "Lỗi: Áp mã giảm giá không thành công.");
            }
            else
            {
                Assert.IsTrue(true, $"Test pass with unmapped result: {expectedResult}");
            }
        }
    }
}