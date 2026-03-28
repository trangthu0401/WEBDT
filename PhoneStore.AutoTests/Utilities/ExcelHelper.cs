using OfficeOpenXml;
using System;
using System.IO;

namespace PhoneStore.AutoTests.Utilities
{
    public static class ExcelHelper
    {
        public static void LogTestResult(string testName, string status, string message, string screenshotPath)
        {
            // 1. Tự động tạo folder Reports nếu chưa có
            string projectDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
            string reportFolder = Path.Combine(projectDir, "Reports");
            if (!Directory.Exists(reportFolder)) Directory.CreateDirectory(reportFolder);

            string filePath = Path.Combine(reportFolder, "TestReport.xlsx");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var sheet = package.Workbook.Worksheets.Count > 0
                            ? package.Workbook.Worksheets[0]
                            : package.Workbook.Worksheets.Add("KetQuaTest");

                if (sheet.Dimension == null)
                {
                    sheet.Cells[1, 1].Value = "Test Case";
                    sheet.Cells[1, 2].Value = "Status";
                    sheet.Cells[1, 3].Value = "Time";
                    sheet.Cells[1, 4].Value = "Log Message";
                    sheet.Cells[1, 5].Value = "Screenshot (Click to open)";
                    sheet.Row(1).Style.Font.Bold = true;
                }

                int row = sheet.Dimension.End.Row + 1;
                sheet.Cells[row, 1].Value = testName;
                sheet.Cells[row, 2].Value = status;
                sheet.Cells[row, 3].Value = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
                sheet.Cells[row, 4].Value = message;

                // 2. Chèn Link ảnh lỗi (Nếu có)
                if (!string.IsNullOrEmpty(screenshotPath))
                {
                    var fileUri = new Uri(screenshotPath);
                    sheet.Cells[row, 5].Hyperlink = fileUri;
                    sheet.Cells[row, 5].Value = "Mở ảnh lỗi";
                    sheet.Cells[row, 5].Style.Font.UnderLine = true;
                    sheet.Cells[row, 5].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                }

                sheet.Cells.AutoFitColumns();
                package.Save();
            }
        }
    }
}