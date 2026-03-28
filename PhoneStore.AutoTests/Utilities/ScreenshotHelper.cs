using OpenQA.Selenium;
using System;
using System.IO;

namespace PhoneStore.AutoTests.Utilities
{
    public class ScreenshotHelper
    {
        public static string TakeScreenshot(IWebDriver driver, string testName)
        {
            // Tạo thư mục Screenshots trong project nếu chưa có
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Screenshots");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            string fileName = $"{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            string filePath = Path.Combine(folderPath, fileName);

            Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            screenshot.SaveAsFile(filePath);

            return filePath; // Trả về đường dẫn để lưu vào Excel
        }
    }
}