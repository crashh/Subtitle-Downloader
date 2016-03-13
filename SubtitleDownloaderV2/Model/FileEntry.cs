using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SubtitleDownloaderV2.Util;

namespace SubtitleDownloaderV2.Model
{
    public class FileEntry
    {
        private readonly string path;

        public string filename { get; set; }

        public string title { get; set; }

        public string release { get; set; }

        public string episode { get; set; }

        public bool subtitleExists;

        public string url;

        public FileEntry(string path)
        {
            this.path = path;
            this.subtitleExists = false;

            if (String.IsNullOrEmpty(path)) throw new NullReferenceException("domain");

            this.filename = Path.GetFileName(path);
        }

        public FileEntry(string path, string title, string release)
        {
            this.path = path;
            this.title = title;
            this.release = release;
            this.subtitleExists = false;

            this.filename = Path.GetFileName(path);
        }

        public void DefineEntriesFromPath()
        {
            if (String.IsNullOrEmpty(filename)) return;

            String[] split = filename.Split('.', '-', '_', '[', ']', ' ');
            ExtractName(split);
            ExtractRelease(split);
            ExtractEpisode();
        }

        private void ExtractName(string[] split)
        {
            String concatName = split[0];
            for (int index = 1; index < split.Length; index++)
            {
                string del = split[index];
                if ((del.Any(c => char.IsDigit(c)) && del.Length > 2) || del.Contains("HDTV") || del.Contains("  "))
                    break;
                concatName += " " + del;
            }
            title = concatName;
        }

        private void ExtractRelease(string[] split)
        {
            release = "";
            release = split.FirstOrDefault(x => ExpectedNames.ReleaseNames.Contains(x));

            if (String.IsNullOrEmpty(release))
            {
                release = split.FirstOrDefault(x => ExpectedNames.ReleaseNamesSecondary.Contains(x));
            }

            if (String.IsNullOrEmpty(release))
            {
                release = ExpectedNames.FileTypeNames.Contains(split[split.Length - 1]) 
                    ? split[split.Length - 2] 
                    : split[split.Length - 1];
            }
        }

        private void ExtractEpisode()
        {
            Match singleMatch = Regex.Match(filename, @"S\d{2}E\d{2}");
            if (singleMatch.Success)
            {
                episode = singleMatch.ToString();
            }
            else
            {
                singleMatch = Regex.Match(filename, @"\d{1}x\d{2}");
                episode = singleMatch.ToString();
            }
        }

        public string GetFullPath()
        {
            return path;
        }

    }
}