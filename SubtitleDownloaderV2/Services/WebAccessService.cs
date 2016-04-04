using System;
using System.IO;
using System.Net;
using System.Text;

namespace SubtitleDownloaderV2.Services
{
    internal class WebAccessService
    {
        internal String HTML;

        /// <summary>
        /// Fetches HTML source code at given url
        /// </summary>
        /// <param name="url">
        /// The target URL
        /// </param>
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
                HTML = null;
            }
        }

        /// <summary>
        /// Starts a download of the given file at target url.
        /// </summary>
        /// <param name="url">
        /// Full url to target file.
        /// </param>
        /// <param name="path">
        /// Full path to directory to store file at.
        /// </param>
        /// <returns></returns>
        public bool InitiateDownload(string url, string path)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    String pathToFolder = UtilityService.GetPath(path);

                    if (!Directory.Exists(pathToFolder)) { return false; }

                    client.DownloadFile(url, pathToFolder + "/autosub-pull.rar");
                }
                catch (Exception e)
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
