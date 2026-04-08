using Newtonsoft.Json.Linq;
using NUnit.Framework;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using PhoneStore.AutoTests.Utilities;
using System.Collections.Generic;

namespace PhoneStore.AutoTests.Tests
{
    [TestFixture]
    public class ProductSearchEmptyTests : BaseTest
    {
        private LoginPage loginPage;
        private ProductSearchEmptyPage emptyPage;

        [SetUp]
        public void PreTest()
        {
            loginPage = new LoginPage(driver);
            emptyPage = new ProductSearchEmptyPage(driver);

            // Đăng nhập Admin
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Account/Login");
            loginPage.Login("admin@shop.com", "admin123", isAdmin: true);
        }

        public static IEnumerable<TestCaseData> GetEmptySearchData()
        {
            return JsonReader.GetTestData("ProductSearchEmptyData.json");
        }

        [Test, TestCaseSource(nameof(GetEmptySearchData))]
        public void TC_PRODUCT_23_SearchEmptyResult(JObject testData)
        {
            string keyword = testData["Keyword"]?.ToString();

            // 1. Vào trang Sản phẩm
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");

            // 2. Tìm kiếm từ khóa tào lao
            emptyPage.Search(keyword);

            // 3. Kiểm tra: Mong đợi là PHẢI thấy thông báo không tìm thấy
            bool isMessageShowed = emptyPage.IsEmptyMessageDisplayed();

            Assert.That(isMessageShowed, Is.True,
                $"Lỗi: Tìm '{keyword}' mà Web không hiện thông báo 'không tìm thấy sản phẩm'!");

            Console.WriteLine($"PASS: Web đã xử lý đúng kịch bản rỗng cho từ khóa: {keyword}");
        }
    }
}