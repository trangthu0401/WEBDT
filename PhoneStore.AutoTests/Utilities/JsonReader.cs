using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace PhoneStore.AutoTests.Utilities
{
    public static class JsonReader
    {
        private static string GetFilePath(string jsonFileName)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string[] paths = {
                Path.Combine(baseDir, "DataTests", jsonFileName),
                Path.Combine(baseDir, "..", "..", "..", "DataTests", jsonFileName),
                Path.Combine(baseDir, "..", "..", "..", "..", "DataTests", jsonFileName)
            };

            string filePath = paths.FirstOrDefault(File.Exists);
            if (filePath == null)
                throw new FileNotFoundException($"Hổng thấy file {jsonFileName}. Vy nhớ chuột phải file JSON chọn Properties -> Copy if newer nhé!");

            return filePath;
        }

        // Hàm dành riêng cho Admin (Lấy 1 ID cụ thể)
        public static dynamic GetTestRow(string jsonFileName, string testCaseID)
        {
            string jsonContent = File.ReadAllText(GetFilePath(jsonFileName));
            JObject jsonObj = JObject.Parse(jsonContent);

            var data = jsonObj[testCaseID];
            if (data == null)
                throw new Exception($"Lỗi: Không tìm thấy nhãn '{testCaseID}' trong file JSON!");

            return data.ToObject<dynamic>();
        }

        // Hàm dành cho các test cũ (Fix lỗi CS0117)
        public static IEnumerable<TestCaseData> GetTestData(string jsonFileName)
        {
            string jsonContent = File.ReadAllText(GetFilePath(jsonFileName));
            if (jsonContent.Trim().StartsWith("["))
            {
                foreach (var item in JArray.Parse(jsonContent))
                    yield return new TestCaseData(item.ToObject<dynamic>());
            }
            else
            {
                foreach (var prop in JObject.Parse(jsonContent).Properties())
                    yield return new TestCaseData(prop.Value.ToObject<dynamic>()).SetName(prop.Name);
            }
        }
    }
}