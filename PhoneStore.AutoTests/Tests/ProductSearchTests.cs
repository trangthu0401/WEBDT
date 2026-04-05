using Newtonsoft.Json.Linq;
using NUnit.Framework;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using PhoneStore.AutoTests.Utilities;
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

            // Đăng nhập Admin
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Account/Login");
            loginPage.Login("admin@shop.com", "admin123", isAdmin: true);
        }

        // Hàm lấy dữ liệu từ JSON lên
        public static IEnumerable<TestCaseData> GetSearchData()
        {
            return JsonReader.GetTestData("ProductSearchData.json");
        }

        [Test, TestCaseSource(nameof(GetSearchData))]
        public void TC_PRODUCT_22_SearchProductByID(JObject testData)
        {
            string idToSearch = testData["SearchID"]?.ToString();

            // 1. Vào trang Sản phẩm
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");

            // 2. Thực hiện tìm kiếm
            searchPage.SearchByID(idToSearch);

            // 3. Kiểm tra kết quả: ID dòng đầu tiên phải chứa cái ID mình vừa tìm
            string actualID = searchPage.GetFirstResultID();

            Assert.That(actualID.Contains(idToSearch), Is.True,
                $"Lỗi: Tìm ID {idToSearch} nhưng kết quả lại ra {actualID}!");

            Console.WriteLine($"PASS: Đã tìm thấy sản phẩm có ID {idToSearch}");
        }
    }
}