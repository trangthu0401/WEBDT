using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.IO;

namespace PhoneStore.AutoTests.Utilities
{
    public class ScreenshotHelper
    {
        // Bổ sung thêm biến tcId để lấy tên siêu ngắn
        public static string TakeScreenshot(IWebDriver driver, string testName, string tcId = null)
        {
            if (driver == null) return string.Empty;

            try
            {
                // 1. Lùi 3 cấp để về thư mục Project gốc
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string projectDir = Directory.GetParent(baseDir).Parent.Parent.Parent.FullName;

                // 2. Trỏ đường dẫn ra thư mục Screenshots ở project gốc
                string screenshotFolder = Path.Combine(projectDir, "Screenshots");

                if (!Directory.Exists(screenshotFolder))
                {
                    Directory.CreateDirectory(screenshotFolder);
                }

                // 3. RÚT GỌN TÊN ẢNH
                // Nếu có TC_ID (VD: TC_PROD_03) thì dùng nó. Nếu không có thì lấy đoạn text trước dấu ngoặc đơn của tên hàm.
                string shortName = !string.IsNullOrEmpty(tcId) ? tcId : testName.Split('(')[0];

                // Dọn dẹp ký tự cấm của Windows
                string safeTestName = string.Join("_", shortName.Split(Path.GetInvalidFileNameChars()));

                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileName = $"{safeTestName}_{timestamp}.png";
                string filePath = Path.Combine(screenshotFolder, fileName);

                // 4. Chụp và lưu ảnh
                Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                screenshot.SaveAsFile(filePath);

                return filePath;
            }
            catch (Exception ex)
            {
                TestContext.WriteLine("Lỗi khi chụp ảnh màn hình: " + ex.Message);
                return string.Empty;
            }
        }
    }
}