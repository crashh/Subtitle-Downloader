using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using SubtitleDownloaderV2.Util;

namespace SubtitleDownloaderV2.Model
{
    public class FileEntry
    {
        public string Path { get; set; } 

        public string Filename { get; set; }

        public string Title { get; set; }

        public string Release { get; set; }

        public string Episode { get; set; }

        public bool SubtitleExists { get; set; }

        public string Url { get; set; }
        
        private bool isDirectory;
        public bool IsDirectory {
            get { return (AllEntries?.Count ?? 0) > 1; }
            private set { this.isDirectory = value; }
        }

        public ICommand OpenFolderCmd { get; }

        public ObservableCollection<FileEntry> AllEntries { get; set; }

        #region Initialize
        public FileEntry(string path)
        {
            this.Path = path;
            this.SubtitleExists = false;
            this.AllEntries = new ObservableCollection<FileEntry>();
            this.IsDirectory = isDirectory;

            if (string.IsNullOrEmpty(path)) throw new NullReferenceException("domain");

            this.Filename = System.IO.Path.GetFileName(path);

            this.OpenFolderCmd = new RelayCommand(this.OpenFolder);
        }

        public FileEntry(string path, string title, string release, string episode) : this(path)
        {
            this.Title = title;
            this.Release = release;
            this.Episode = episode;
        }

        public void DefineEntriesFromPath()
        {
            if (string.IsNullOrEmpty(Filename))
            {
                return;
            }

            var split = Filename.Split('.', '-', '_', '[', ']', ' ', '(', ')');
            ExtractName(split);
            ExtractRelease(split);
            ExtractEpisode();
        }

        public void DefineEntriesWithDefault(string name, string release, string episode)
        {
            if (string.IsNullOrEmpty(name) == false)
            {
                this.Title = name;
            }
            if (string.IsNullOrEmpty(release) == false)
            {
                var primaryFound = ExpectedNames.ReleaseNames.Contains(this.Release);
                var secondaryFound = ExpectedNames.ReleaseNamesSecondary.Contains(this.Release);

                if (primaryFound == false && secondaryFound == false)
                {
                    this.Release = release;
                }
            }
            if (string.IsNullOrEmpty(episode) == false)
            {
                this.Title = episode;
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
            Title = concatName.TrimStart();
        }

        private void ExtractRelease(string[] split)
        {
            Release = "";
            Release = split.FirstOrDefault(x => ExpectedNames.ReleaseNames.Contains(x));

            if (string.IsNullOrEmpty(Release))
            {
                Release = split.FirstOrDefault(x => ExpectedNames.ReleaseNamesSecondary.Contains(x));
            }

            if (string.IsNullOrEmpty(Release))
            {
                Release = ExpectedNames.FileTypeNames.Contains(split[split.Length - 1]) 
                    ? split[split.Length - 2] 
                    : split[split.Length - 1];
            }
        }

        private void ExtractEpisode()
        {
            Match singleMatch = Regex.Match(Filename, @"S\d{2}E\d{2}");
            if (singleMatch.Success)
            {
                Episode = singleMatch.ToString();
            }
            else
            {
                singleMatch = Regex.Match(Filename, @"\d{1}x\d{2}");
                Episode = singleMatch.ToString();
            }
        }
        #endregion ExtractMethods

        public string GetFullPath()
        {
            return Path;
        }

        private void OpenFolder()
        {
            try
            {
                Process.Start(this.GetFullPath());
            }
            catch (Exception)
            {
                //ignored
            }
        }

    }
}