using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SubtitleDownloader.Services;

namespace ParsingTests
{
    [TestClass]
    public class ParseDirectoryContentsTest
    {
        [TestMethod]
        public void VerifyClean_CleanDirectoryListing()
        {
            String[] dirContentMock = mockDirContents();
            String[] expectedResult =
            {
                @"D:\folder\some_file.with-weird name-2015[cctv]",
                @"D:\folder\file.mkv",
                @"D:\folder\folder2\folder3\folder4\test.avi"
            };

            ParseDirectoryContents pdct = new ParseDirectoryContents(dirContentMock);
            pdct.CleanDirectoryListing(false);

            CollectionAssert.AreEqual(expectedResult, pdct.DirContents);

        }

        [TestMethod]
        public void VerifyIsolate_IsolateTitleName()
        {
            String[] dirContentMock = mockDirContentsClean();
            String[] expectedResult =
            {
                @"some file with weird name",
                @"file",
                @"test avi"
            };

            ParseDirectoryContents pdct = new ParseDirectoryContents(dirContentMock);
            String[] result = pdct.IsolateTitleName();

            CollectionAssert.AreEqual(expectedResult, result);
        }

        private String[] mockDirContents()
        {
            String[] dirContentMock =
                {
                @"D:\folder\some_file.with-weird name-2015[cctv]",
                @"D:\folder\file.mkv",
                @"D:\folder\folder2\folder3\folder4\test.avi",
                @"D:\folder\desktop.ini",
                @"D:\folder\Movies",
                @"D:\folder\Thumbs.db",
            };

            return dirContentMock;
        }

        private String[] mockDirContentsClean()
        {
            String[] dirContentMock =
                {
                @"D:\folder\some_file.with-weird name-2015[cctv]",
                @"D:\folder\file S02E01.mkv",
                @"D:\folder\folder2\folder3\folder4\test.avi",
            };

            return dirContentMock;
        }
    }
}
