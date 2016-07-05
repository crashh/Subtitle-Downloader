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
        public static List<string> ReleaseNames = new List<string> {
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


        public static List<string> ReleaseNamesSecondary = new List<string>
        {
            "TLA",
            "FoV",
            "720p",
            "1080p",
            "x264"
        };

        public static List<string> FileTypeNames = new List<string>
        {
            ".mkv",
            ".mp4",
            ".avi"
        };
    }
}