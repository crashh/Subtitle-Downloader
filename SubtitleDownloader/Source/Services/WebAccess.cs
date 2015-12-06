using System;
using System.IO;
using System.Net;
using System.Text;
using SubtitleDownloader.Interfaces;

namespace SubtitleDownloader.Services
{
    internal class WebAccess
    {
        // ReSharper disable once InconsistentNaming
        internal String HTML;
        
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

        public bool InitiateDownload(string url, string path)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    UtilityService utilSerivce = new UtilityService();
                    String pathToFolder = utilSerivce.GetPath(path);

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
