using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace PhoneStore.AutoTests.Utilities
{
    public static class JsonReader
    {
        public static IEnumerable<TestCaseData> GetTestData(string jsonFileName)
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory ?? string.Empty;
            var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            var testDir = (TestContext.CurrentContext?.TestDirectory) ?? baseDir;

            // Candidate folders to search (include both TestData and DataTests)
            var candidates = new[]
            {
                Path.Combine(baseDir, "TestData", jsonFileName),
                Path.Combine(baseDir, "DataTests", jsonFileName),
                Path.Combine(testDir, "TestData", jsonFileName),
                Path.Combine(testDir, "DataTests", jsonFileName),
                Path.Combine(assemblyDir, "TestData", jsonFileName),
                Path.Combine(assemblyDir, "DataTests", jsonFileName)
            }.Distinct().ToList();

            // Also attempt to walk up from baseDir safely (avoid NullReference)
            try
            {
                var dir = new DirectoryInfo(baseDir);
                for (int i = 0; i < 4 && dir?.Parent != null; i++)
                {
                    dir = dir.Parent;
                    if (dir == null) break;
                    candidates.Add(Path.Combine(dir.FullName, "TestData", jsonFileName));
                    candidates.Add(Path.Combine(dir.FullName, "DataTests", jsonFileName));
                }
            }
            catch
            {
                // ignore any unexpected path traversal errors; candidates already populated
            }

            string filePath = candidates.FirstOrDefault(File.Exists);

            if (filePath == null)
            {
                throw new FileNotFoundException(
                    "Không tìm thấy file dữ liệu. Đã thử các đường dẫn:\n" + string.Join("\n", candidates));
            }

            string jsonContent = File.ReadAllText(filePath);
            JArray jsonArray = JArray.Parse(jsonContent);

            foreach (JToken token in jsonArray)
            {
                JObject item = (JObject)token;
                string testCaseID = item["TestCaseID"]?.ToString() ?? "Unknown_TestCase";

                yield return new TestCaseData(item).SetName(testCaseID);
            }
        }
    }
}