using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SubtitleDownloader
{
    class IOParsingHelper
    {
        /// <summary>
        /// Retrieves all files/directories at current set directory, excluded those which already
        /// contains subtitles or are ignored.
        /// </summary>
        public static String[] getDirectoryListing(SettingsForm settingsForm)
        {
            String[] directoryContents = null;
            if (System.IO.Directory.Exists(settingsForm.torrentDownloadPath))
            {
                directoryContents = removeFoldersWithSubtitles(
                                                Directory.GetFileSystemEntries(settingsForm.torrentDownloadPath),
                                                settingsForm.ignoreAlreadySubbedFolders);
            }
            return directoryContents;
        }

        /// <summary>
        /// Helper method:
        /// Remove entires that already contains subtitles, or are set to be ignored.
        /// </summary>
        private static String[] removeFoldersWithSubtitles(String[] directoryContents, bool ignoreFlag)
        {
            List<String> cleanedArray = new List<String>();
            List<String> ignoredFiles = new List<String> { "desktop.ini", "Thumbs.db", "Movies", "Series" };
            foreach (String elem in directoryContents)
            {
                // Attempt to get folder contents, ignore if not a folder:
                bool subtitleExist = false;
                try
                {
                    foreach (string f in Directory.GetFiles(elem))
                    {
                        if (f.EndsWith(".srt") || f.EndsWith(".sub") || f.EndsWith(".src"))
                        {
                            subtitleExist = true && ignoreFlag;
                        }
                    }
                } catch (System.Exception) { /*ignore this entry*/ }

                // Add to array if ignored or had subtitle:
                if (!subtitleExist && !ignoredFiles.Contains(Path.GetFileName(elem)) && !Path.GetFileName(elem).Contains(".srt"))
                    cleanedArray.Add(elem);
            }
            return cleanedArray.ToArray();
        }

        /// <summary>
        /// Attempts to isolate the gorup who released the file, to know which versio best fits this file.
        /// </summary>
        public static String[] isolateReleaseName(String[] dirContents, SettingsForm settings)
        {
            // Set a series of expected names that might pop up, prioritised picks:
            List<String> expectedNames = settings.expectedNames;
            List<String> expectedNamesSecondary = new List<String> { "TLA", "FoV", "720p", "1080p", "x264" };
            // List to story matches in, note that index maps directly to dir contents:
            List<String> releaseNames = new List<String>(); ;

            foreach (String elem in dirContents)
            {
                bool nameFound = false;
                String[] split = Path.GetFileName(elem).Split(new char[] {'.', '-', '_', '[', ']', ' '});

                // Look at each split element and check if match pre-set names:
                foreach (String del in split)
                {
                    if (expectedNames.Contains(del))
                    {
                        releaseNames.Add(del);
                        nameFound = true;
                        break;
                    }
                }
                if (!nameFound)
                {
                    // Look for secondary hits:
                    foreach (String del in split)
                    {
                        if (expectedNamesSecondary.Contains(del))
                        {
                            releaseNames.Add(del);
                            nameFound = true;
                            break;
                        }
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
        public static String[] isolateEpisodeNumber(String[] dirContents)
        {
            List<String> episodeNumber = new List<String>(); ;

            foreach (String elem in dirContents)
            {
                Match singleMatch = Regex.Match(elem, @"S\d{2}E\d{2}");
                episodeNumber.Add(singleMatch.ToString());
            }

            return episodeNumber.ToArray();
        }

        /// <summary>
        /// Attempts to isolate the actual name of the file entry.
        /// </summary>
        public static String[] isolateTitleName(String[] dirContents)
        {
            List<String> titleNames = new List<String>(); ;

            foreach (String elem in dirContents)
            {
                String[] split = Path.GetFileName(elem).Split(new char[] { '.', '-', '_', '[', ']', ' ' });
                String concatName = "";

                // Assume name starts at beginning and ends when specific keywords show instead:
                foreach (String del in split)
                {
                    // TODO: This is very fragile, find another way.
                    if (del.Contains("S0") || del.Contains("S1") || del.Contains("201") || del.Contains("200") || del.Contains("199")
                        || del.Contains("720") || del.Contains("1080") || del.Contains("HDTV")) break;
                    concatName += del + " ";
                }

                titleNames.Add(concatName);
            }

            return titleNames.ToArray();
        }

        /// <summary>
        /// Checks wether a given path points to a file or a folder. Always returns path to the folder.
        /// </summary>
        public static String checkFolderOrFile(String path)
        {
            if (path.Contains(".mkv") || path.Contains(".mp4") || path.Contains(".avi"))
            {
                // Path points directly to a file:
                return path.Substring(0, (path.Length - Path.GetFileName(path).Length) - 1);
            }           
            else
            {
                // Path points to a folder:
                return path;
            }                         
        }
    }
}
