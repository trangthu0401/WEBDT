using OpenQA.Selenium;
using System;
using System.IO;

namespace PhoneStore.AutoTests.Utilities
{
    public class ScreenshotHelper
    {
        public static string TakeScreenshot(IWebDriver driver, string testName)
        {
            if (driver == null) return string.Empty;

            try
            {
                // 1. Lùi 3 cấp để về thư mục Project gốc
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string projectDir = Directory.GetParent(baseDir).Parent.Parent.Parent.FullName;

                // 2. Trỏ đường dẫn ra thư mục Screenshots ở project gốc
                string screenshotFolder = Path.Combine(projectDir, "Screenshots");

                // 3. KIỂM TRA FOLDER ẢNH TỒN TẠI CHƯA -> CHƯA THÌ TẠO MỚI
                if (!Directory.Exists(screenshotFolder))
                {
                    Directory.CreateDirectory(screenshotFolder);
                }

                // Chỉnh lại tên file cho hợp lệ (tránh các ký tự đặc biệt)
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string safeTestName = testName.Replace("\"", "").Replace("\\", "").Replace("/", "");
                string fileName = $"{safeTestName}_{timestamp}.png";

                string filePath = Path.Combine(screenshotFolder, fileName);

                // 4. Chụp và lưu ảnh
                Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                screenshot.SaveAsFile(filePath);

                return filePath; // Trả về đường dẫn để ExcelHelper lưu làm link
            }
            catch (Exception ex)
            {
                TestContext.WriteLine("Lỗi khi chụp ảnh màn hình: " + ex.Message);
                return string.Empty;
            }
        }
    }
}