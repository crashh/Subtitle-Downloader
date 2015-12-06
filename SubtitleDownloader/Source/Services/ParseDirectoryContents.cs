using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SubtitleDownloader
{
    public class ParseDirectoryContents
    {
        public String[] DirContents { get; set; }

        /// <summary>
        /// Constructor, sets the directory contents such that the methods can retrieve correct information.
        /// </summary>
        /// <param name="directoryEntries"> String array with directory contents. </param>
        public ParseDirectoryContents(String[] directoryEntries)
        {
            DirContents = directoryEntries;
        }

        /// <summary>
        /// Returns the given directory listing, with excluded names and, if set, folders which contain subtitles, 
        /// removed from the list.
        /// </summary>
        public void CleanDirectoryListing(bool ignoreFlag)
        {
            List<String> cleanedArray = new List<String>();
            List<String> ignoredFiles = new List<String> { "desktop.ini", "Thumbs.db", "Movies", "Series" };
            foreach (String elem in DirContents)
            {
                bool subtitleExist = false;
                if (Directory.Exists(elem))
                {
                    foreach (string dirEntry in Directory.GetFiles(elem))
                    {
                        if (dirEntry.EndsWith(".srt") || dirEntry.EndsWith(".sub") || dirEntry.EndsWith(".src"))
                        {
                            subtitleExist = ignoreFlag;
                        }
                    }
                }

                String fileName = Path.GetFileName(elem);
                if (fileName != null && (!subtitleExist && !ignoredFiles.Contains(fileName) && !fileName.Contains(".srt")
                    && !fileName.Contains(".rar")))
                { cleanedArray.Add(elem); }
            }
            DirContents = cleanedArray.ToArray();
        }

        /// <summary>
        /// Attempts to isolate the actual name of the file entry.
        /// </summary>
        public String[] IsolateTitleName()
        {
            List<String> titleNames = new List<String>();

            foreach (String elem in DirContents)
            {
                string fileName = Path.GetFileName(elem);
                if (fileName == null) continue;
                String[] split = fileName.Split('.', '-', '_', '[', ']', ' ');

                // Assume name starts at beginning and ends when specific keywords show:
                String concatName = split[0];
                for (int index = 1; index < split.Length; index++)
                {
                    string del = split[index];
                    if ((del.Any(c => char.IsDigit(c)) && del.Length > 2) || del.Contains("HDTV"))
                        break;
                    concatName += " " + del;
                }

                titleNames.Add(concatName);
            }

            return titleNames.ToArray();
        }

        /// <summary>
        /// Attempts to isolate the group who released the file, to know which versio best fits this file.
        /// </summary>
        public String[] IsolateReleaseName(List<String> expectedNames)
        {
            List<String> expectedNamesSecondary = new List<String> { "TLA", "FoV", "720p", "1080p", "x264" };
            List<String> releaseNames = new List<String>();

            foreach (String elem in DirContents)
            {
                // Error handling, entry might not exist:
                string fileName = Path.GetFileName(elem);
                if (fileName == null) continue;

                String[] split = fileName.Split('.', '-', '_', '[', ']', ' ');

                bool nameFound = LookForExpectedNames(expectedNames, split, releaseNames);
                if (!nameFound)
                {
                    nameFound = LookForExpectedNames(expectedNamesSecondary, split, releaseNames);
                }
                if (!nameFound)
                {
                    AssumeLastEntry(split, releaseNames);
                }
            }

            return releaseNames.ToArray();
        }

        private static bool LookForExpectedNames(List<string> expectedNames, string[] split, List<string> releaseNames)
        {
            foreach (String del in split.Where(del => expectedNames.Contains(del)))
            {
                releaseNames.Add(del);
                return true;
            }
            return false;
        }

        private static void AssumeLastEntry(string[] split, List<string> releaseNames)
        {
            if (split[split.Length - 1] == "mp4" || split[split.Length - 1] == "mkv" || split[split.Length - 1] == "avi")
                releaseNames.Add(split[split.Length - 2]); // Assume it's last entry (but not the file type).
            else
                releaseNames.Add(split[split.Length - 1]); // Assume it's last entry.
        }

        /// <summary>
        /// Attempts to isolate episode number, if one exists.
        /// </summary>
        /// <returns></returns>
        public String[] IsolateEpisodeNumber()
        {
            List<String> episodeNumber = new List<String>();

            foreach (String elem in DirContents)
            {
                Match singleMatch = Regex.Match(elem, @"S\d{2}E\d{2}");
                if (singleMatch.Success)
                {
                    episodeNumber.Add(singleMatch.ToString());
                }
                else
                {
                    singleMatch = Regex.Match(elem, @"\d{1}x\d{2}");
                    episodeNumber.Add(singleMatch.ToString());
                }
            }

            return episodeNumber.ToArray();
        }
    }
}
