using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SubtitleDownloader
{
    class HelperIO
    {
        /// <summary>
        /// Retrieves all files/directories at current set directory, excluded those which already
        /// contains subtitles or are ignored.
        /// </summary>
        /// <param name="settingsForm">Current active settings object</param>
        /// <param name="ignoredFiles">Files to ignore as a List of Strings</param>
        /// <returns></returns>
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

        private static String[] removeFoldersWithSubtitles(String[] directoryContents, List<String> ignoredFiles)
        {
            List<String> cleanedArray = new List<String>();
            foreach (String elem in directoryContents)
            {
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
                if (!subtitleExist && !ignoredFiles.Contains(Path.GetFileName(elem)))
                {
                    cleanedArray.Add(elem);
                }
            }
            return cleanedArray.ToArray();
        }

        public static String[] isolateReleaseName(String[] dirContents)
        {
            List<String> expectedNames = new List<String> {
                "KILLERS", "DIMENSION", "SPARKS", "MAJESTiC", "ROVERS", "LOL", "GHOULS", "EVO", "ETRG"
            };
            List<String> releaseNames = new List<String>(); ;
            foreach (String elem in dirContents)
            {
                bool nameFound = false;
                String[] split = Path.GetFileName(elem).Split(new char[] {'.', '-', '_', '[', ']', ' '});
                foreach (String del in split)
                {
                    if (expectedNames.Contains(del))
                    {
                        releaseNames.Add(del);
                        nameFound = true;
                        break;
                    }
                }
                if (!nameFound && (split[split.Length - 1] == "mp4" || split[split.Length - 1] == "mkv"))
                {
                    releaseNames.Add(split[split.Length - 2]);
                }
                else if (!nameFound)
                {
                    releaseNames.Add(split[split.Length - 1]);
                }
            }

            return releaseNames.ToArray();
        }

        public static String[] isolateTitleName(String[] dirContents)
        {
            List<String> haltOnThis = new List<String> {
                "S0", "DIMENSION", "SPARKS", "MAJESTiC", "ROVERS", "LOL", "GHOULS", "EVO", "ETRG"
            };
            List<String> titleNames = new List<String>(); ;
            foreach (String elem in dirContents)
            {
                String[] split = Path.GetFileName(elem).Split(new char[] { '.', '-', '_', '[', ']', ' ' });
                String concatName = "";
                foreach (String del in split)
                {
                    if (del.Contains("S0") || del.Contains("S1") || del.Contains("201") || del.Contains("200") || del.Contains("199"))
                        break;                    
                    concatName += del + " ";
                }
                titleNames.Add(concatName);
            }

            return titleNames.ToArray();
        }
    }
}
