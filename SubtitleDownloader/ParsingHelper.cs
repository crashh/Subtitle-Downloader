using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SubtitleDownloader
{
    class ParsingHelper
    {
        /// <summary>
        /// Retrieves all files/directories at current set directory, excluded those which already
        /// contains subtitles or are ignored.
        /// </summary>
        public static String[] getDirectoryListing(Settings settingsForm, List<String> ignoredFiles)
        {
            String[] directoryContents;
            if (System.IO.Directory.Exists(settingsForm.torrentDownloadPath))
            {
                directoryContents = removeFoldersWithSubtitles(
                                                Directory.GetFileSystemEntries(settingsForm.torrentDownloadPath),
                                                ignoredFiles);
                return directoryContents;
            }
            return new String[] { "Directory not found" };
        }

        /// <summary>
        /// Helper method:
        /// Remove entires that already contains subtitles, or are set to be ignored.
        /// </summary>
        private static String[] removeFoldersWithSubtitles(String[] directoryContents, List<String> ignoredFiles)
        {
            List<String> cleanedArray = new List<String>();
            foreach (String elem in directoryContents)
            {
                // Attempt to get folder contents, ignore if not a folder:
                bool subtitleExist = false;
                try
                {
                    foreach (string f in Directory.GetFiles(elem))
                    {
                        if (f.EndsWith(".srt") || f.EndsWith(".sub"))
                        {
                            subtitleExist = true;
                        }
                    }
                } catch (System.Exception) { /*ignore this file*/ }

                // Add to array if not ignored or had subtitle:
                if (!subtitleExist && !ignoredFiles.Contains(Path.GetFileName(elem)))
                    cleanedArray.Add(elem);
            }
            return cleanedArray.ToArray();
        }

        /// <summary>
        /// Attempts to isolate the gorup who released the file, to know which versio best fits this file.
        /// </summary>
        public static String[] isolateReleaseName(String[] dirContents)
        {
            // Set a series of expected names that might pop up, prioritised picks:
            List<String> expectedNames = new List<String> {
                "KILLERS", "DIMENSION", "SPARKS", "MAJESTiC", "ROVERS", "LOL", "GHOULS", "EVO", "ETRG" };
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
                    if (split[split.Length - 1] == "mp4" || split[split.Length - 1] == "mkv")
                        releaseNames.Add(split[split.Length - 2]);
                    else
                        releaseNames.Add(split[split.Length - 1]);
                }
            }

            return releaseNames.ToArray();
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
                    if (del.Contains("S0") || del.Contains("S1") || del.Contains("201") || del.Contains("200") || del.Contains("199")
                        || del.Contains("720") || del.Contains("1080") || del.Contains("HDTV")) break;
                    concatName += del + " ";
                }

                titleNames.Add(concatName);
            }

            return titleNames.ToArray();
        }
    }
}
