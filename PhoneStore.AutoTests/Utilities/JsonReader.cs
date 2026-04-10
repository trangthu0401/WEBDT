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
                throw new FileNotFoundException($"Không tìm thấy file {jsonFileName}. Hãy đặt file trong thư mục DataTests và set Copy if newer.");

            return filePath;
        }

        /// <summary>
        /// Lấy dòng dữ liệu theo TestCaseID (hỗ trợ cả mảng và object)
        /// </summary>
        public static dynamic GetTestRow(string jsonFileName, string testCaseID)
        {
            string jsonContent = File.ReadAllText(GetFilePath(jsonFileName));
            jsonContent = jsonContent.Trim();

            if (jsonContent.StartsWith("["))
            {
                // Dạng mảng các object
                var array = JArray.Parse(jsonContent);
                var item = array.FirstOrDefault(x => x["TestCaseID"]?.ToString() == testCaseID);
                if (item == null)
                    throw new Exception($"Không tìm thấy TestCaseID '{testCaseID}' trong mảng JSON!");
                return item.ToObject<dynamic>();
            }
            else
            {
                // Dạng object với key là TestCaseID
                var obj = JObject.Parse(jsonContent);
                var data = obj[testCaseID];
                if (data == null)
                    throw new Exception($"Không tìm thấy key '{testCaseID}' trong object JSON!");
                return data.ToObject<dynamic>();
            }
        }

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