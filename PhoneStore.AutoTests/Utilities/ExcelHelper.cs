using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace PhoneStore.AutoTests.Utilities
{
    public static class ExcelHelper
    {
        private static ExcelPackage _package;
        private static string _filePath;

        // Các sheet cần tìm kiếm TC_ID để ghi kết quả
        private static readonly string[] TargetSheets = { "Test Cases (Trang)", "Test Cases (vy)", "Test Cases ( Sang )" };

        public static void InitExistingExcel(string fileName)
        {
            // Khai báo License cho thư viện EPPlus
            ExcelPackage.License.SetNonCommercialPersonal("QA_Team");

            // Tìm đường dẫn file Excel gốc chung cho cả nhóm
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string projectPath = Directory.GetParent(baseDir).Parent.Parent.Parent.FullName;
            _filePath = Path.Combine(projectPath, fileName);

            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException($"KHÔNG TÌM THẤY FILE EXCEL: {_filePath}");
            }

            // Mở file và giữ trên RAM
            _package = new ExcelPackage(new FileInfo(_filePath));
        }

        public static void UpdateTestResult(string tcId, string status, string errorMessage)
        {
            if (_package == null) return;

            foreach (var sheetName in TargetSheets)
            {
                var ws = _package.Workbook.Worksheets[sheetName];
                if (ws == null) continue;

                var tcCell = ws.Cells["A1:D200"].FirstOrDefault(c => c.Value?.ToString().Trim() == tcId);

                if (tcCell != null)
                {
                    int row = tcCell.Start.Row;

                    int actualResultCol = FindColumnIndex(ws, "Actual Result");

                    // Xử lý không dùng ?? cho kiểu int
                    int statusCol = FindColumnIndex(ws, "Status");
                    if (statusCol == -1) statusCol = FindColumnIndex(ws, "Pass/Failed");

                    // 1. Ghi Actual Result
                    if (actualResultCol > 0)
                    {
                        string resultText = status == "Passed"
                            ? "Automation Test chạy thành công."
                            : $"Automation Test phát hiện lỗi: {errorMessage}";
                        ws.Cells[row, actualResultCol].Value = resultText;
                        ws.Cells[row, actualResultCol].Style.WrapText = true;
                    }

                    // 2. Ghi Status và Tô màu
                    if (statusCol > 0)
                    {
                        var cellStatus = ws.Cells[row, statusCol];
                        cellStatus.Value = status.ToUpper();
                        cellStatus.Style.Font.Bold = true;
                        cellStatus.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        if (status == "Passed")
                            cellStatus.Style.Font.Color.SetColor(Color.DarkGreen);
                        else
                            cellStatus.Style.Font.Color.SetColor(Color.Red);
                    }
                    break;
                }
            }
        }

        public static void SaveAndClose()
        {
            if (_package != null)
            {
                _package.Save();
                _package.Dispose();
            }
        }

        private static int FindColumnIndex(ExcelWorksheet ws, string columnName)
        {
            for (int r = 1; r <= 10; r++)
            {
                for (int c = 1; c <= 20; c++)
                {
                    if (ws.Cells[r, c].Value?.ToString().Trim().Equals(columnName, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        return c;
                    }
                }
            }
            return -1;
        }
    }
}