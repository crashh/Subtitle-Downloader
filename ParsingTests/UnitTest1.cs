using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SubtitleDownloader;

namespace ParsingTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void VerifyPath_checkFolderOrFile()
        {
            String pathToC = @"C:\folder\folder.name.with[special]-characters001\";
            String pathToD = @"D:\folder\folder.name.with[special]-characters001\";
            String pathToFile = @"D:\folder\folder.name.with[special]-characters001\file_ending.mkv";

            //IOParsingHelper.getPath(pathToC);
        }
    }
}
