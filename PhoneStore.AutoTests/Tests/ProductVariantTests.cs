using NUnit.Framework;
using PhoneStore.AutoTests.Core;
using PhoneStore.AutoTests.Pages;
using PhoneStore.AutoTests.Utilities;
using System;

namespace PhoneStore.AutoTests.Tests
{
    [TestFixture]
    public class ProductVariantTests : BaseTest
    {
        private LoginPage loginPage;
        private ProductVariantPage variantPage;

        [SetUp]
        public void PreTest()
        {
            loginPage = new LoginPage(driver);
            variantPage = new ProductVariantPage(driver);

            // Đăng nhập Admin
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Account/Login");
            loginPage.Login("admin@shop.com", "admin123", isAdmin: true);
        }

        [Test]
        [Property("TC_ID", "TC_PRODUCT_06")]
        public void TC_PRODUCT_06_ViewVariantList()
        {
            // 1. Vào trang danh sách sản phẩm
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");

            // 2. Click vào nút "Xem biến thể" của ông đầu bảng
            variantPage.GoToFirstProductVariant();

            // 3. Kiểm tra xem đã vào đúng trang danh sách biến thể chưa
            Assert.That(variantPage.IsAtVariantListPage(), Is.True, "Lỗi: Robot không vào được trang danh sách biến thể!");

            Console.WriteLine("SUCCESS: Đã vào trang danh sách biến thể thành công.");
        }


        [Test, Order(2)]
        [Property("TC_ID", "TC_PRODUCT_14")]
        public void TC_PRODUCT_14_AddVariant_InputNoSave()
        {
            // 1. Vào trang danh sách sản phẩm
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product/Index");
            var data = JsonReader.GetTestRow("ProductVariantData.json", "TC_PRODUCT_14");

            // 2. Mở Modal thêm biến thể của sản phẩm đầu tiên
            variantPage.GoToFirstProductVariant();
            variantPage.OpenAddModal();

            // 3. Nhập đầy đủ thông tin từ JSON
            variantPage.InputVariantDetails(data);

            // KHÔNG GỌI variantPage.Save() theo yêu cầu của Vy!

            // 4. Kiểm tra xem dữ liệu đã được điền vào các field chưa
            string actualColor = variantPage.GetInputValue(variantPage.ColorInput);
            string actualStock = variantPage.GetInputValue(variantPage.StockInput);

            Assert.Multiple(() =>
            {
                Assert.That(actualColor, Is.EqualTo((string)data.Color), "Lỗi: Field Màu sắc chưa nhập đúng!");
                Assert.That(actualStock, Is.EqualTo((string)data.Stock), "Lỗi: Field Tồn kho chưa nhập đúng!");
            });

            Console.WriteLine("SUCCESS: Đã nhập đầy đủ thông tin biến thể nhưng không bấm Lưu.");
        }


        [Test, Order(2)]
        [Property("TC_ID", "TC_PRODUCT_15")]
        public void TC_PRODUCT_15_AddVariant_Success()
        {
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product/Index");
            var data = JsonReader.GetTestRow("ProductVariantData.json", "TC_PRODUCT_15");

            variantPage.GoToFirstProductVariant();
            variantPage.OpenAddModal();
            variantPage.InputVariantDetails(data);
            variantPage.Save();

            System.Threading.Thread.Sleep(3000);
            Assert.That(driver.PageSource.Contains((string)data.Color), Is.True, "Lỗi: Không tìm thấy màu sắc biến thể mới!");
        }

        [Test, Order(3)]
        [Property("TC_ID", "TC_PRODUCT_16")]
        public void TC_PRODUCT_16_AddVariant_MissingData()
        {
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
            var data = JsonReader.GetTestRow("ProductVariantData.json", "TC_PRODUCT_16");

            variantPage.GoToFirstProductVariant();
            variantPage.OpenAddModal();
            variantPage.InputVariantDetails(data);
            variantPage.Save();

            Assert.That(variantPage.IsErrorMessageDisplayed(), Is.True, "Lỗi: Không hiển thị lỗi required khi bỏ trống màu!");
        }

        [Test, Order(4)]
        [Property("TC_ID", "TC_PRODUCT_17")]
        public void TC_PRODUCT_17_AddVariant_NegativeStock()
        {
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
            var data = JsonReader.GetTestRow("ProductVariantData.json", "TC_PRODUCT_17");

            variantPage.GoToFirstProductVariant();
            variantPage.OpenAddModal();
            variantPage.InputVariantDetails(data);
            variantPage.Save();

            Assert.That(variantPage.IsErrorMessageDisplayed(), Is.True, "Lỗi: Hệ thống không báo lỗi khi nhập tồn kho âm!");
        }

        [Test, Order(5)]
        [Property("TC_ID", "TC_PRODUCT_18")]
        public void TC_PRODUCT_18_AddVariant_UploadImageOnly()
        {
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
            var data = JsonReader.GetTestRow("ProductVariantData.json", "TC_PRODUCT_18");

            variantPage.GoToFirstProductVariant();
            variantPage.OpenAddModal();

            // Bước quan trọng: Robot điền thông tin và nạp ảnh
            variantPage.InputVariantDetails(data);

            // Không gọi variantPage.Save() theo ý Vy nè!

            // Kiểm tra: Xem ô input file có chứa tên file mình vừa nạp không
            string uploadedFile = variantPage.GetUploadedFileName();
            string expectedFileName = (string)data.ImageFileName;

            Assert.That(uploadedFile.Contains(expectedFileName), Is.True,
                $"Lỗi: Robot chưa nạp được file {expectedFileName} vào ô upload!");

            Console.WriteLine($"SUCCESS: Robot đã nạp ảnh {expectedFileName} thành công (chưa bấm Lưu).");
        }

        [Test, Order(6)]
        [Property("TC_ID", "TC_PRODUCT_19")]
        public void TC_PRODUCT_19_AddVariant_InvalidImageFile()
        {
            driver.Navigate().GoToUrl(ConfigHelper.BaseUrl + "/Product");
            var data = JsonReader.GetTestRow("ProductVariantData.json", "TC_PRODUCT_19");

            variantPage.GoToFirstProductVariant();
            variantPage.OpenAddModal();
            variantPage.InputVariantDetails(data);
            variantPage.Save();

            Assert.That(variantPage.IsErrorMessageDisplayed(), Is.True, "Lỗi: Hệ thống không chặn file ảnh sai định dạng (.exe)!");
        }


    }
}
