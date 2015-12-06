using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SubtitleDownloader.Interfaces
{
    internal interface IAccessService
    {
        /// <summary>
        /// Access given URL and retrieves the HTML source.
        /// </summary>
        /// <param name="url"> The URL for which to retrieve source from. </param>
        void RetrieveHtmlAtUrl(String url);

        /// <summary>
        /// Initiates download of the given url and moves it to the path given.
        /// </summary>
        /// <param name="url"> Url for the file. </param>
        /// <param name="path"> The path to store the download at. </param>
        /// <returns> Boolean indicating if download was a success. </returns>
        bool InitiateDownload(string url, string path);

        /// <summary>
        /// Finds search results and returns them in an array.
        /// Note: Retrieve correct HTML first.
        /// </summary>
        String[] FindSearchResults();

        /// <summary>
        /// Find the correct subtitle link, ie. english and correct release.
        /// Note: Retrieve correct HTML first.
        /// </summary>
        string PickCorrectSubtitle(string releaseName, string episode);

        /// <summary>
        /// Find the download link on the page and returns it.
        /// Note: Retrieve correct HTML first.
        /// </summary>
        string FindDownloadUrl();
    }
}
