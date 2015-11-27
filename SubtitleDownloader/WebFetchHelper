using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SubtitleDownloader
{
    class FetchSubtitle
    {

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
        /// Helper method:
        /// Initiates download of the given URL and moves it to correct directory when done.
        /// </summary>
        public static void downloadFile(string url, string path)
        {
            // Create an instance of WebClient
            WebClient client = new WebClient();
            // Hookup DownloadFileCompleted Event
            //client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);

            // Start the download and copy the file to given path
            client.DownloadFileAsync(new Uri(url), path);
        }

    }
}
