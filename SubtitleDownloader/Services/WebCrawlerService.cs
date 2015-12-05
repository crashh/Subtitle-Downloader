using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using SubtitleDownloader.Services;

namespace SubtitleDownloader
{
    class WebCrawlerService
    {
        private String HTML { get; set; }

        /// <summary>
        /// Access given URL and retrieves the HTML source.
        /// </summary>
        public void RetrieveHtmlAtUrl(String url)
        {
            try
            {
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "GET";
                WebResponse myResponse = httpRequest.GetResponse();
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
            MatchCollection allMatches = 
                Regex.Matches(HTML, @"<td class=""a1"">(.+?)<td class=""a3"">", RegexOptions.Singleline);
            for (int i = 0; i < allMatches.Count; i++)
            {
                String singleMatch = allMatches[i].ToString();
                if (singleMatch.Contains("English") && singleMatch.Contains("positive-icon") &&
                    singleMatch.Contains(releaseName) && singleMatch.Contains(episode))
                {
                    String correct = Regex.Match(singleMatch, @"/subtitles/(.+?)"">").ToString(); 
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
            using (WebClient client = new WebClient())
            {
                try
                {
                    UtilityService utilSerivce = new UtilityService();
                    String pathToFolder = utilSerivce.GetPath(path);
                    if (!Directory.Exists(pathToFolder)) { return false; }
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
    }
}
