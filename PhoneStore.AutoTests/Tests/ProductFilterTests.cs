using Newtonsoft.Json.Linq;
using NUnit.Framework;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using PhoneStore.AutoTests.Utilities;
using System;
using System.Collections.Generic;

namespace PhoneStore.AutoTests.Tests
{
    [TestFixture]
    public class ProductFilterTests : BaseTest
    {
        private LoginPage loginPage;
        private ProductFilterPage filterPage;

        [SetUp]
        public void PreTest()
        {
            loginPage = new LoginPage(driver);
            filterPage = new ProductFilterPage(driver);

            // 1. Đăng nhập Admin trước khi test lọc
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Account/Login");
            loginPage.Login("admin@shop.com", "admin123", isAdmin: true);
        }

        // Đọc dữ liệu từ file JSON ProductFilterData.json
        public static IEnumerable<TestCaseData> GetFilterData()
        {
            return JsonReader.GetTestData("ProductFilterData.json");
        }

        [Test, TestCaseSource(nameof(GetFilterData))]
        public void TC_PRODUCT_24_FilterByBrand(JObject testData)
        {
            // Lấy tên hãng từ JSON Vy mới sửa (Apple, Samsung, Google, Xiaomi, OPPO)
            string brand = testData["BrandName"]?.ToString();

            // 1. Điều hướng tới trang Sản phẩm
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");

            // 2. Thực hiện bấm nút lọc hãng
            filterPage.FilterByBrand(brand);

            // 3. Kiểm tra tính chính xác của bảng (Gọi đúng hàm AreAllResultsMatchBrand)
            bool isCorrect = filterPage.AreAllResultsMatchBrand(brand);

            Assert.That(isCorrect, Is.True, $"Lỗi: Sau khi chọn hãng '{brand}', bảng vẫn xuất hiện sản phẩm của hãng khác!");

            Console.WriteLine($"PASS: Đã kiểm tra xong bộ lọc hãng {brand}");
        }
    }
}