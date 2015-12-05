using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SubtitleDownloader
{
    public class IOParsingService
    {
        public String[] DirContents { get; set; }

        public IOParsingService(String[] directoryEntries)
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
                    && !fileName.Contains(".rar"))) { cleanedArray.Add(elem); }
            }
            DirContents = cleanedArray.ToArray();
        }

        /// <summary>
        /// Attempts to isolate the group who released the file, to know which versio best fits this file.
        /// </summary>
        public String[] IsolateReleaseName(List<String> expectedNames)
        {
            List<String> expectedNamesSecondary = new List<String> { "TLA", "FoV", "720p", "1080p", "x264" };
            // List to story matches in, note that index maps directly to dir contents:
            List<String> releaseNames = new List<String>();

            foreach (String elem in DirContents)
            {
                bool nameFound = false;
                string fileName = Path.GetFileName(elem);
                if (fileName == null) continue;
                String[] split = fileName.Split('.', '-', '_', '[', ']', ' ');

                // Look at each split element and check if match pre-set names:
                foreach (String del in split.Where(del => expectedNames.Contains(del)))
                {
                    releaseNames.Add(del);
                    nameFound = true;
                    break;
                }
                if (!nameFound)
                {
                    // Look for secondary hits:
                    foreach (String del in split.Where(del => expectedNamesSecondary.Contains(del)))
                    {
                        releaseNames.Add(del);
                        nameFound = true;
                        break;
                    }
                }
                if (!nameFound)
                {
                    if (split[split.Length - 1] == "mp4" || split[split.Length - 1] == "mkv" || split[split.Length - 1] == "avi")
                        releaseNames.Add(split[split.Length - 2]); // Assume it's last entry (but not the file type).
                    else
                        releaseNames.Add(split[split.Length - 1]); // Assume it's last entry.
                }
            }

            return releaseNames.ToArray();
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
                episodeNumber.Add(singleMatch.ToString());
            }

            return episodeNumber.ToArray();
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
                String concatName = "";

                // Assume name starts at beginning and ends when specific keywords show instead:
                foreach (String del in split)
                {
                    // TODO: This is very fragile, find another way.
                    if (del.Contains("S0") || del.Contains("S1") || del.Contains("201") || del.Contains("200") || del.Contains("199")
                        || del.Contains("720") || del.Contains("1080") || del.Contains("HDTV"))
                        break;
                    concatName += del + " ";
                }

                titleNames.Add(concatName);
            }

            return titleNames.ToArray();
        }
    }
}
