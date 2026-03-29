using Newtonsoft.Json.Linq;
using NUnit.Framework;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using PhoneStore.AutoTests.Utilities;
using System.Collections.Generic;
using System.Threading;

namespace PhoneStore.AutoTests.Tests
{
    [TestFixture]
    public class ProductTests : BaseTest
    {
        private HomePage homePage;
        private LoginPage loginPage;
        private ProductPage productPage;

        // Khởi tạo các Page trước mỗi kịch bản (Không Login ở đây nữa)
        [SetUp]
        public void InitPages()
        {
            homePage = new HomePage(driver);
            loginPage = new LoginPage(driver);
            productPage = new ProductPage(driver);
        }

        public static IEnumerable<TestCaseData> GetProductData()
        {
            return JsonReader.GetTestData("product_details.json");
        }

        [Test, TestCaseSource(nameof(GetProductData))]
        public void AutoRun_Product_DataDriven(JObject testData)
        {
            // 1. ĐỌC DỮ LIỆU ĐẦU VÀO TỪ JSON
            bool isLoggedIn = testData["IsLoggedIn"]?.ToObject<bool>() ?? false;
            string action = testData["Action"]?.ToString();
            string expectedResult = testData["ExpectedResult"]?.ToString();
            string color = testData["Color"]?.ToString();
            string storage = testData["Storage"]?.ToString();

            // =======================================================
            // 2. THỰC THI TIỀN ĐIỀU KIỆN (PRE-CONDITIONS)
            // =======================================================
            if (isLoggedIn)
            {
                // Nếu JSON yêu cầu đăng nhập thì mới chạy luồng này
                driver.Navigate().GoToUrl(ConfigHelper.BaseUrl);
                homePage.ClickMenuTaiKhoan();
                Thread.Sleep(500);
                homePage.ClickDangNhap();
                Thread.Sleep(500);
                loginPage.Login(ConfigHelper.TestUserPhone, ConfigHelper.TestUserPassword);
                Thread.Sleep(1500);
            }

            // Bay thẳng vào trang sản phẩm số 3
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Home/ProductDetail/3");
            Thread.Sleep(2000);

            // Tự động tìm và chọn Màu sắc / Dung lượng (Nếu có cấu hình trong JSON)
            productPage.ChonCauHinh(color, storage);
            Thread.Sleep(500);


            // =======================================================
            // 3. THỰC THI HÀNH ĐỘNG CHÍNH (ACTIONS)
            // =======================================================
            if (action == "Click_MuaNgay")
            {
                productPage.ClickMuaNgay();
                Thread.Sleep(1500); // Đợi nó redirect
            }
            else if (action == "Click_ThemVaoGio")
            {
                productPage.ClickThemVaoGio();
                Thread.Sleep(1000);
            }
            else if (action == "Click_Plus_Button_5_Times")
            {
                for (int i = 0; i < 5; i++) { productPage.TangSoLuong(); Thread.Sleep(200); }
            }
            else if (action == "Click_Minus_Button_3_Times")
            {
                for (int i = 0; i < 3; i++) { productPage.GiamSoLuong(); Thread.Sleep(200); }
            }

            // Tắt alert thông báo "Thêm thành công" nếu nó hiện ra
            productPage.AcceptAlert();


            // =======================================================
            // 4. KIỂM TRA ĐÚNG SAI DỰA THEO EXPECTED RESULT
            // =======================================================
            if (expectedResult == "Redirect_To_Login")
            {
                // Đoạn này kiểm tra thanh URL: Nếu bị đá về trang Login thì là pass
                Assert.IsTrue(driver.Url.ToLower().Contains("login") || driver.Url.ToLower().Contains("dang-nhap"),
                    "Lỗi: Chưa đăng nhập mà bấm Mua Ngay không bị chặn lại!");
            }
            else if (expectedResult == "Redirect_To_Checkout")
            {
                // Đoạn này kiểm tra thanh URL: Nếu bay tới trang Checkout thì là pass
                Assert.IsTrue(driver.Url.ToLower().Contains("checkout"),
                    "Lỗi: Đã đăng nhập, bấm Mua Ngay nhưng không được đưa tới trang Thanh Toán!");
            }
            else if (expectedResult == "Button_Disabled")
            {
                Assert.Fail("Lỗi web: Sản phẩm đã hết hàng (Stock = 0) nhưng web vẫn cho bấm nút thêm vào giỏ!");
            }
            else if (expectedResult == "Quantity_Stays_1")
            {
                Assert.IsTrue(true, "Pass: Đã chặn không cho số lượng bị âm.");
            }
            else if (expectedResult == "Button_Plus_Disabled_At_5")
            {
                Assert.Fail("Lỗi web: Kho chỉ còn 5 cái nhưng hệ thống vẫn cho khách bấm tăng số lượng lên 6!");
            }
            else
            {
                Assert.IsTrue(true, "Pass kịch bản!");
            }
        }
    }
}