using System;
using System.Collections.Generic;

namespace SubtitleDownloaderV2.Util
{
    /// <summary>
    /// Holds static content.
    /// Contents here are default values, they may be overriden by the settings model.
    /// </summary>
    public static class ExpectedNames
    {
        public static List<String> ReleaseNames = new List<String> {
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


        public static List<String> ReleaseNamesSecondary = new List<String>
        {
            "TLA",
            "FoV",
            "720p",
            "1080p",
            "x264"
        };

        public static List<String> FileTypeNames = new List<String>
        {
            ".mkv",
            ".mp4",
            ".avi"
        };
    }
}