using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WebApi.Api.Tests.Helpers
{
    public class TestDataHelper
    {
        public static string LoadTestData(string fileName)
        {
            return File.ReadAllText($"./Files/{fileName}");

        }
    }
}
