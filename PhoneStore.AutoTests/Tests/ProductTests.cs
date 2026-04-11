using NUnit.Framework;
using OpenQA.Selenium.Support.UI;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using PhoneStore.AutoTests.Utilities;
using SeleniumExtras.WaitHelpers;
using System.Collections.Generic;
using System.Threading;

namespace PhoneStore.AutoTests.Tests
{
    [TestFixture]
    public class ProductTests : BaseTest
    {
        private LoginPage loginPage;
        private ProductPage productPage;

        [SetUp]
        public void InitPages()
        {
            loginPage = new LoginPage(driver);
            productPage = new ProductPage(driver);
        }

        [Test]
        [Property("TC_ID", "TC_PROD_01")]
        public void TC_PROD_01_KhachAnDanh_NhanMuaNgay_PhaiChuyenHuongLogin()
        {
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Home/ProductDetail/2");
            Thread.Sleep(2000);
            productPage.ClickMuaNgay();
            Thread.Sleep(2000);
            Assert.That(driver.Url.ToLower(), Does.Contain("login").Or.Contains("dang-nhap"),
                "Lỗi: Khách vãng lai bấm Mua Ngay không bị ép đăng nhập!");
        }

       

        public static IEnumerable<TestCaseData> GetVariantData()
        {
            var allData = JsonReader.GetTestData("product_details.json");
            foreach (var testCase in allData)
            {
                var jObj = testCase.Arguments[0] as Newtonsoft.Json.Linq.JObject;
                if (jObj != null)
                {
                    string tcId = jObj["TestCaseID"]?.ToString();
                    if (tcId == "TC_PROD_03" || tcId == "TC_PROD_04")
                    {
                        testCase.SetProperty("TC_ID", tcId);
                        yield return testCase;
                    }
                }
            }
        }

        [Test, TestCaseSource(nameof(GetVariantData))]
        public void TC_PROD_ThemVaoGio_VoiCacBienTheKhacNhau(Newtonsoft.Json.Linq.JObject testData)
        {
            string color = testData["Color"]?.ToString();
            string storage = testData["Storage"]?.ToString();

            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Account/Login");
            loginPage.Login(ConfigHelper.TestUserPhone, ConfigHelper.TestUserPassword);
            Thread.Sleep(2000);

            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Home/ProductDetail/3");
            Thread.Sleep(2000);

            productPage.ChonCauHinh(color, storage);
            Thread.Sleep(1000);
            productPage.ClickThemVaoGio();
            Thread.Sleep(1500);
            productPage.AcceptAlert();

            Assert.Pass($"Đã thêm thành công sản phẩm màu {color}, dung lượng {storage}");
        }

        [Test]
        [Property("TC_ID", "TC_PROD_05")]
        public void TC_PROD_05_GiamSoLuong_KhongDuocPhepAm()
        {
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Home/ProductDetail/2");
            Thread.Sleep(2000);

            for (int i = 0; i < 3; i++)
            {
                productPage.GiamSoLuong(); // Nếu nút bị mờ, sẽ throw exception -> test fail đúng
                Thread.Sleep(500);
            }

            string actualQuantity = productPage.GetSoLuongHienTai();
            Assert.AreEqual("1", actualQuantity, "Lỗi: Số lượng bị giảm xuống dưới 1!");
        }

        [Test]
        [Property("TC_ID", "TC_PROD_07")]
        public void TC_PROD_07_SanPhamHetHang_ButtonBiDisabled()
        {
            // Giả sử product ID 10 có stock = 0
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Home/ProductDetail/2");
            Thread.Sleep(2000);

            Assert.That(productPage.IsAddToCartDisabled(), Is.True, "Nút 'Thêm vào giỏ' vẫn có thể bấm dù hết hàng!");
            Assert.That(productPage.IsBuyNowDisabled(), Is.True, "Nút 'Mua ngay' vẫn có thể bấm dù hết hàng!");
        }

        [Test]
        [Property("TC_ID", "TC_PROD_10")]
        public void TC_PROD_10_GioiHanMaxKhiTangSoLuong()
        {
            // Sản phẩm ID 5 có stock = 5
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Home/ProductDetail/5");
            Thread.Sleep(2000);

            for (int i = 0; i < 5; i++)
            {
                productPage.TangSoLuong();
                Thread.Sleep(300);
            }

            Assert.That(productPage.IsPlusButtonDisabled(), Is.True, "Nút (+) vẫn hoạt động dù đã đạt giới hạn stock!");
            string qty = productPage.GetSoLuongHienTai();
            Assert.AreEqual("5", qty, $"Số lượng hiển thị không đúng (kỳ vọng 5, thực tế {qty})");
        }

        [Test]
        [Property("TC_ID", "TC_PROD_16")]
        public void TC_PROD_16_SauKhiThemVaoGio_UrlKhongDoi()
        {
            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Account/Login");
            loginPage.Login(ConfigHelper.TestUserPhone, ConfigHelper.TestUserPassword);
            Thread.Sleep(2000);

            driver.Navigate().GoToUrl($"{ConfigHelper.BaseUrl}/Home/ProductDetail/6");
            Thread.Sleep(2000);
            string beforeUrl = driver.Url;

            productPage.ClickThemVaoGio();
            Thread.Sleep(2000);
            productPage.AcceptAlert();

            Assert.AreEqual(beforeUrl, driver.Url, "Sau khi thêm vào giỏ, URL bị chuyển hướng bất thường!");
        }
    }
}