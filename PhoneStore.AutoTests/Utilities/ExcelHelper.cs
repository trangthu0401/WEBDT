using OfficeOpenXml; // Nhớ cài thư viện EPPlus trong NuGet nhé
using System;
using System.IO;

namespace PhoneStore.AutoTests.Utilities
{
    public static class ExcelHelper
    {
        private static readonly object _lock = new object();

        public static void LogTestResult(string testName, string status, string message, string screenshotPath)
        {
            lock (_lock)
            {
            // 1. Cấu hình bản quyền EPPlus (Bắt buộc để không báo lỗi)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // 2. Lấy đường dẫn gốc của Project (Thoát khỏi cái lồng bin/Debug/net8.0)
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            // Lùi 3 cấp để về đúng thư mục PhoneStore.AutoTests (Sử dụng Path.GetFullPath để tương thích chuẩn với dấu \\ ở cuối)
            string projectDir = Path.GetFullPath(Path.Combine(baseDir, @"..\..\..\"));

            // 3. Kiểm tra và tạo thư mục Reports ngay ngoài Project
            string reportFolder = Path.Combine(projectDir, "Reports");
            if (!Directory.Exists(reportFolder))
            {
                Directory.CreateDirectory(reportFolder);
            }

            // 4. Đường dẫn file Excel
            string filePath = Path.Combine(reportFolder, "TestReport.xlsx");

            // 5. Ghi dữ liệu vào file
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                // Lấy sheet đầu tiên, nếu chưa có thì tạo mới
                var sheet = package.Workbook.Worksheets.Count > 0
                            ? package.Workbook.Worksheets[0]
                            : package.Workbook.Worksheets.Add("Kết quả Test");

                // Nếu là file mới toanh, tạo dòng Tiêu đề (Header)
                if (sheet.Dimension == null)
                {
                    sheet.Cells[1, 1].Value = "Tên Test Case";
                    sheet.Cells[1, 2].Value = "Trạng Thái";
                    sheet.Cells[1, 3].Value = "Thời Gian";
                    sheet.Cells[1, 4].Value = "Lỗi / Ghi Chú";
                    sheet.Cells[1, 5].Value = "Link Ảnh Lỗi";

                    // Tô màu xám và in đậm cho Header cho chuyên nghiệp
                    using (var range = sheet.Cells[1, 1, 1, 5])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }
                }

                int row = sheet.Dimension?.End.Row + 1 ?? 2;
                sheet.Cells[row, 1].Value = testName;
                sheet.Cells[row, 2].Value = status;
                sheet.Cells[row, 3].Value = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
                sheet.Cells[row, 4].Value = message;

                // 6. Chèn Link Screenshot nếu test có lỗi (Click vào là mở ảnh ngay)
                if (!string.IsNullOrEmpty(screenshotPath) && File.Exists(screenshotPath))
                {
                    var fileUri = new Uri(screenshotPath);
                    sheet.Cells[row, 5].Hyperlink = fileUri;
                    sheet.Cells[row, 5].Value = "Xem ảnh lỗi";
                    sheet.Cells[row, 5].Style.Font.UnderLine = true;
                    sheet.Cells[row, 5].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                }

                sheet.Cells.AutoFitColumns(); // Tự động dãn cột cho đẹp
                try
                {
                    package.Save();
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Lỗi: Không thể lưu file Excel do file đang mở ({ex.Message}). Tạo file dự phòng...");
                    string fallbackPath = Path.Combine(reportFolder, $"TestReport_Fallback_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
                    package.SaveAs(new FileInfo(fallbackPath));
                }
                }
            }
        }
    }
}