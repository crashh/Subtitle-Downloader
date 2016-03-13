using System;
using System.Collections.Generic;

namespace SubtitleDownloaderV2.Util
{
    /// <summary>
    /// Holds static content.
    /// </summary>
    public static class ExpectedNames
    {
        public static readonly List<String> ReleaseNames = new List<String> {
            "KILLERS",
            "DIMENSION",
            "SPARKS",
            "MAJESTiC",
            "YIFY",
            "JYK",
            "Hive",
            "ROVERS",
            "LOL",
            "GHOULS",
            "EVO",
            "ETRG",
            "FUM",
            "BATV",
            "W4F"
        };


        public static readonly List<String> ReleaseNamesSecondary = new List<String>
        {
            "TLA",
            "FoV",
            "720p",
            "1080p",
            "x264"
        };

        public static readonly List<String> FileTypeNames = new List<String>
        {
            ".mkv",
            ".mp4",
            ".avi"
        };
    }
}