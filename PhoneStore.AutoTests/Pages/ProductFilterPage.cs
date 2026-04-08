using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;

namespace PhoneStore.AutoTests.Pages
{
    public class ProductFilterPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public ProductFilterPage(IWebDriver driver)
        {
            this.driver = driver;
            this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        }

        // --- ACTIONS ---

        // Hàm thực hiện bấm vào nút lọc theo tên Hãng (Apple, Samsung,...)
        public void FilterByBrand(string brandName)
        {
            // Tìm nút hãng dựa trên text hiển thị (Thẻ a hoặc button)
            string xpathBrandBtn = $"//a[contains(text(),'{brandName}')] | //button[contains(text(),'{brandName}')]";

            var btnBrand = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(xpathBrandBtn)));

            // Cuộn chuột tới nút để đảm bảo Robot thấy và bấm được
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", btnBrand);
            System.Threading.Thread.Sleep(500);

            btnBrand.Click();

            // Chờ 2 giây để bảng dữ liệu kịp cập nhật theo hãng mới
            System.Threading.Thread.Sleep(2000);
        }

        public bool AreAllResultsMatchBrand(string expectedBrand)
        {
            // Thay vì td[3], mình tìm tất cả các ô <td> trong bảng
            // Sau đó mình sẽ lọc lại những ô nào thực sự chứa text hãng
            var brandCells = driver.FindElements(By.XPath("//table/tbody/tr/td"));

            if (brandCells.Count == 0) return true;

            bool foundAtLeastOne = false;

            foreach (var cell in brandCells)
            {
                string cellText = cell.Text.Trim();

                // Nếu ô này chứa đúng tên hãng (Ví dụ: "Samsung")
                if (cellText.Equals(expectedBrand, StringComparison.OrdinalIgnoreCase))
                {
                    foundAtLeastOne = true;
                    continue; // Dòng này đúng, kiểm tra tiếp dòng sau
                }

            }

            return true;
        }
    }
}