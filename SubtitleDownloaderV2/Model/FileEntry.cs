using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SubtitleDownloaderV2.Util;

namespace SubtitleDownloaderV2.Model
{
    public class FileEntry
    {
        public string path { get; set; } 

        public string filename { get; set; }

        public string title { get; set; }

        public string release { get; set; }

        public string episode { get; set; }

        public bool subtitleExists { get; set; }

        public string url { get; set; }
        
        private bool isDirectory;
        public bool IsDirectory {
            get { return (AllEntries?.Count ?? 0) > 1; }
            private set { this.isDirectory = value; }
        }

        public ObservableCollection<FileEntry> AllEntries { get; set; }

        #region Initialize
        public FileEntry(string path)
        {
            this.path = path;
            this.subtitleExists = false;
            this.AllEntries = new ObservableCollection<FileEntry>();
            this.IsDirectory = isDirectory;

            if (string.IsNullOrEmpty(path)) throw new NullReferenceException("domain");

            this.filename = Path.GetFileName(path);
        }

        public FileEntry(string path, string title, string release, string episode)
        {
            this.path = path;
            this.title = title;
            this.release = release;
            this.episode = episode;
            this.subtitleExists = false;
            this.IsDirectory = isDirectory;

            this.AllEntries = new ObservableCollection<FileEntry>();

            this.filename = Path.GetFileName(path);
        }

        public void DefineEntriesFromPath()
        {
            if (string.IsNullOrEmpty(filename))
            {
                return;
            }

            var split = filename.Split('.', '-', '_', '[', ']', ' ', '(', ')');
            ExtractName(split);
            ExtractRelease(split);
            ExtractEpisode();
        }

        public void DefineEntriesWithDefault(string name, string release, string episode)
        {
            if (string.IsNullOrEmpty(name) == false)
            {
                this.title = name;
            }
            if (string.IsNullOrEmpty(release) == false)
            {
                var primaryFound = ExpectedNames.ReleaseNames.Contains(this.release);
                var secondaryFound = ExpectedNames.ReleaseNamesSecondary.Contains(this.release);

                if (primaryFound == false && secondaryFound == false)
                {
                    this.release = release;
                }
            }
            if (string.IsNullOrEmpty(episode) == false)
            {
                this.title = episode;
            }
        }
        #endregion Initialize

        #region ExtractMethods
        private void ExtractName(string[] split)
        {
            string concatName = split[0];
            for (int index = 1; index < split.Length; index++)
            {
                string del = split[index];
                if ((del.Any(c => char.IsDigit(c)) && del.Length > 2) || del.Contains("HDTV") || del.Contains("  "))
                    break;
                concatName += " " + del;
            }
            title = concatName.TrimStart();
        }

        private void ExtractRelease(string[] split)
        {
            release = "";
            release = split.FirstOrDefault(x => ExpectedNames.ReleaseNames.Contains(x));

            if (string.IsNullOrEmpty(release))
            {
                release = split.FirstOrDefault(x => ExpectedNames.ReleaseNamesSecondary.Contains(x));
            }

            if (string.IsNullOrEmpty(release))
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
        #endregion ExtractMethods

        public string GetFullPath()
        {
            return path;
        }

    }
}