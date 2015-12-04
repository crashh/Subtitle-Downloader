using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace SubtitleDownloader
{
    class WebCrawler
    {
        private String HTML { get; set; }

        /// <summary>
        /// Access given URL and retrieves the HTML source.
        /// </summary>
        public void RetrieveHtmlAtUrl(String url)
        {
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
                myRequest.Method = "GET";
                WebResponse myResponse = myRequest.GetResponse();
                StreamReader streamReader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                HTML = streamReader.ReadToEnd();
                streamReader.Close();
                myResponse.Close();
            }
            catch (Exception)
            {
                // Let it happen.
            }
        }

        /// <summary>
        /// Finds search results and returns them in an array.
        /// Note: Retrieve correct HTML first.
        /// </summary>
        public String[] PickCorrectSearchEntry()
        {
            MatchCollection allMatches = Regex.Matches(HTML, @"/subtitles/(.+?)"">");
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
        /// Find the correct subtitle link, ie. english and correct release.
        /// Note: Retrieve correct HTML first.
        /// TODO: Look for non-hearing impaired first.
        /// </summary>
        public string PickCorrectSubtitle(string releaseName, string episode)
        {
            MatchCollection allMatches = Regex.Matches(HTML, @"<td class=""a1"">(.+?)<td class=""a3"">", RegexOptions.Singleline);
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
        /// Find the download link on the page and returns it.
        /// Note: Retrieve correct HTML first.
        /// </summary>
        public string FindDownloadUrl()
        {
            Match onlyMatch = Regex.Match(HTML, @"/subtitle/download(.+?)""", RegexOptions.Singleline);
            return onlyMatch.ToString().Substring(0,onlyMatch.ToString().Length-1);
        }

        /// <summary>
        /// Initiates download of the given url and moves it to correct directory when done.
        /// Note: Retrieve correct HTML first.
        /// </summary>
        public bool InitiateDownload(string url, string path, string name)
        {
            // Create an instance of WebClient
            using (WebClient client = new WebClient())
            {
                try
                {
                    IOParsing ioParser = new IOParsing();
                    String pathToFolder = ioParser.GetPath(path);
                    client.DownloadFile(url, pathToFolder + "/autosub-pull.rar");
                } catch (Exception e)
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
        public void UnrarFile(String location)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            IOParsing ioParser = new IOParsing();
            String pathToFolder = ioParser.GetPath(location);

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
