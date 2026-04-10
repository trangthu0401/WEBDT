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
    public class ProductSearchTests : BaseTest
    {
        private LoginPage loginPage;
        private ProductSearchPage searchPage;

        [SetUp]
        public void PreTest()
        {
            loginPage = new LoginPage(driver);
            searchPage = new ProductSearchPage(driver);

            // 1. Đăng nhập Admin
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Account/Login");
            loginPage.Login("admin@shop.com", "admin123", isAdmin: true);
        }

        // Hàm đọc data từ JSON
        public static IEnumerable<TestCaseData> GetSearchData()
        {
            return JsonReader.GetTestData("ProductSearchData.json");
        }

        // --- TEST CASE 22: TÌM THEO ID ---
        [Test, TestCaseSource(nameof(GetSearchData))]
        [Property("TC_ID", "TC_PRODUCT_22")]
        public void TC_PRODUCT_22_SearchProductByID(JObject testData)
        {
            string idToSearch = testData["SearchID"]?.ToString();

            // Nếu dòng dữ liệu này không có SearchID thì bỏ qua để hàm dưới chạy
            if (string.IsNullOrEmpty(idToSearch)) return;

            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
            searchPage.SearchKeyword(idToSearch);

            string actualID = searchPage.GetFirstResultID();

            Assert.That(actualID.Contains(idToSearch), Is.True,
                $"Lỗi: Tìm ID '{idToSearch}' nhưng dòng đầu tiên lại ra '{actualID}'!");

            Console.WriteLine($"PASS: Đã tìm thấy sản phẩm có ID {idToSearch}");
        }

        // --- TEST CASE 10: TÌM THEO TÊN ---
        [Test, TestCaseSource(nameof(GetSearchData))]
        [Property("TC_ID", "TC_PRODUCT_10")]
        public void TC_PRODUCT_10_SearchProductName(JObject testData)
        {
            string nameToSearch = testData["SearchName"]?.ToString();

            // Nếu dòng dữ liệu này không có SearchName thì bỏ qua để hàm trên chạy
            if (string.IsNullOrEmpty(nameToSearch)) return;

            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
            searchPage.SearchKeyword(nameToSearch);

            string actualName = searchPage.GetFirstResultName().ToLower();

            Assert.That(actualName.Contains(nameToSearch.ToLower()), Is.True,
                $"Lỗi: Tìm sản phẩm tên '{nameToSearch}' nhưng kết quả là '{actualName}'!");

            Console.WriteLine($"PASS: Đã tìm thấy sản phẩm có tên chứa '{nameToSearch}'");
        }
    }
}