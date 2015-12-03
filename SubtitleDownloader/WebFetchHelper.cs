using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace SubtitleDownloader
{
    class WebFetchHelper
    {
        /// <summary>
        /// Access a webpage and returns all contents as a String.
        /// </summary>
        public static String accessWeb(String URL)
        {
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(URL);
            myRequest.Method = "GET";
            WebResponse myResponse = myRequest.GetResponse();
            StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            string result = sr.ReadToEnd();
            sr.Close();
            myResponse.Close();
            return result;
        }

        /// <summary>
        /// Given an entire webpages HTML source as a string.
        /// Finds search results and returns them in an array.
        /// </summary>
        public static String[] findCorrectEntry(String webContents)
        {
            MatchCollection allMatches = Regex.Matches(webContents, @"/subtitles/(.+?)"">");
            HashSet<String> matchesWithoutDuplicates = new HashSet<string>();
            for (int i = 0; i < allMatches.Count; i++)
            {
                String match = allMatches[i].ToString();
                if (!match.Contains("/subtitles/title") && !match.Contains("release?"))
                {
                    matchesWithoutDuplicates.Add(match.Substring(11, match.LastIndexOf('"') - 11));
                }                    
            }
            return matchesWithoutDuplicates.ToArray();
        }

        /// <summary>
        /// Given an entire webpages HTML source as a string.
        /// Find the correct subtitle link, ie. english and correct release.
        /// TODO: Look for non-hearing impaired first.
        /// </summary>
        internal static string findCorrectSub(String findCorrectSub, string releaseName, string episode)
        {
            MatchCollection allMatches = Regex.Matches(findCorrectSub, @"<td class=""a1"">(.+?)<td class=""a3"">", RegexOptions.Singleline);
            for (int i = 0; i < allMatches.Count; i++)
            {
                String singleMatch = allMatches[i].ToString();
                if (singleMatch.Contains("English") && singleMatch.Contains(releaseName) && singleMatch.Contains(episode))
                {
                    String correct = Regex.Match(singleMatch, @"/subtitles/(.+?)"">").ToString(); // TODO: Could be improved in runtime.
                    return correct.Substring(0, correct.Length-2);
                }
            }
            return null;
        }

        /// <summary>
        /// Given an entire webpages HTML source as a string.
        /// Find the download link on the page and returns it.
        /// </summary>
        internal static string findDownloadLink(string downloadPage)
        {
            Match onlyMatch = Regex.Match(downloadPage, @"/subtitle/download(.+?)""", RegexOptions.Singleline);
            return onlyMatch.ToString().Substring(0,onlyMatch.ToString().Length-1);
        }

        /// <summary>
        /// Initiates download of the given URL and moves it to correct directory when done.
        /// </summary>
        public static bool downloadFile(string url, string path, string name)
        {
            // Create an instance of WebClient
            using (var client = new WebClient())
            {
                try
                {
                    IOParsing ioParser = new IOParsing();
                    String pathToFolder = ioParser.getPath(path);
                    client.DownloadFile(url, pathToFolder + "/autosub-pull.rar");
                } catch (System.Exception e)
                {
                    //Write exception dump.
                    TextWriter settingsFile = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "SubDownloaderDUMP.txt", true);
                    settingsFile.Write(e);
                    settingsFile.Close();
                    return false;
                }
                
                return true;                
            }
        }

        /// <summary>
        /// Unrars the file at specified location.
        /// Note: this does not wait for download to complete, so might not always work.
        /// </summary>
        public static void unrarFile(String location)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            IOParsing ioParser = new IOParsing();
            String pathToFolder = ioParser.getPath(location);

            if (pathToFolder.Substring(0,1) != "C")
            {
                cmd.StandardInput.WriteLine(pathToFolder.Substring(0, 1) + ":");
            }

            cmd.StandardInput.WriteLine("cd \"" + pathToFolder + "\"");
            cmd.StandardInput.WriteLine("unzip \"autosub-pull.rar\"");
            cmd.StandardInput.WriteLine("rm \"autosub-pull.rar\"");
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            Console.WriteLine(cmd.StandardOutput.ReadToEnd());
        }
    }
}
