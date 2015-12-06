using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SubtitleDownloader;
using SubtitleDownloader.Services;

namespace ParsingTests
{
    [TestClass]
    public class UtilityTest
    {
        [TestMethod]
        public void VerifyPath_checkFolderOrFile()
        {
            String[] pathToTest =
            {
                @"C:\folder\folder.name.with[special]-characters001\",
                @"C:\folder\folder.name.with[special]-characters001\file_ending.avi",
                @"D:\folder\folder.name.with[special]-characters001\",
                @"D:\folder\folder.name.with[special]-characters001\file_ending.mkv",
                @"D:\"
            };
            String[] expectedResults =
            {
                @"C:\folder\folder.name.with[special]-characters001\",
                @"C:\folder\folder.name.with[special]-characters001\",
                @"D:\folder\folder.name.with[special]-characters001\",
                @"D:\folder\folder.name.with[special]-characters001\",
                @"D:\"
            };
            UtilityService utilityService = new UtilityService();

            for (int i = 0; i < 3; i++)
            {
                String result = utilityService.GetPath(pathToTest[i]);
                Assert.AreEqual(expectedResults[i], result, true);
            }
        }
    }
}
