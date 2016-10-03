using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using SubtitleDownloader.ViewModel.Dialog;
using SubtitleDownloaderV2.Dialogs;
using SubtitleDownloaderV2.Model;
using SubtitleDownloaderV2.Util;
using SubtitleDownloaderV2.View.Dialog;

namespace SubtitleDownloaderV2.Services
{
    internal class SubsceneService : WebAccessService
    {
        private const bool SUCCESS = true;
        private const bool FAILURE = false;

        private readonly FileEntry selected;

        internal delegate void WriteToProgressWindow(string message, bool success);
        public WriteToProgressWindow WriteProgress;
        
        public SubsceneService(FileEntry selected)
        {
            this.selected = selected;
        }

        public ResultPickerItemViewModel[] FindSearchResults()
        {
            //var matchesWithoutDuplicates = new HashSet<string>();
            var result = new List<ResultPickerItemViewModel>();

            // Look if we got an exact match:
            var exactMatches = Regex.Matches(html, @"<h2 class=""exact"">(.+?)</ul>", RegexOptions.Singleline);
            if (exactMatches.Count == 1)
            {
                var exactMatch = Regex.Matches(exactMatches[0].ToString(), @"/subtitles/(.+?)"">(.+?)</a>");
                if (exactMatch.Count == 1)
                {
                    var matchLink = exactMatch[1].ToString();
                    var matchName = exactMatch[2].ToString();
                    result.Add(new ResultPickerItemViewModel(matchName, matchLink));
                    return result.ToArray();
                }
            }

            // No dice, pick them all:
            var allMatches = Regex.Matches(html, @"/subtitles/(.+?)"">(.+?)</a>");
            for (var i = 0; i < allMatches.Count; i++)
            {
                var match = allMatches[i];
                var matchLink = match.Groups[1].ToString();
                var matchName = match.Groups[2].ToString();
                if (!matchLink.Contains("/subtitles/title") && !matchLink.Contains("release?"))
                {
                    result.Add(new ResultPickerItemViewModel(matchName, matchLink));
                }

            }
            return result.ToArray();
        }

        public string PickCorrectSubtitle()
        {
            var chosenResult = string.Empty;
            var allMatches = Regex.Matches(html, @"<td class=""a1"">(.+?)<td class=""a3"">", RegexOptions.Singleline);

            for (var i = 0; i < allMatches.Count; i++)
            {
                var singleMatch = allMatches[i].ToString();
                if (singleMatch.Contains(Settings.Language) && singleMatch.Contains("positive-icon") &&
                    singleMatch.Contains(selected.release) && singleMatch.Contains(selected.episode))
                {
                    chosenResult = Regex.Match(singleMatch, @"/subtitles/(.+?)"">").Groups[1].ToString();

                    if (!singleMatch.Contains("class=\"a41\""))
                    {
                        // This is not hearing impaired, so we are satisfied.
                        return chosenResult;
                    }
                }
            }
            return chosenResult;
        }

        public string FindDownloadUrl()
        {
            var onlyMatch = Regex.Match(html, @"/subtitle/download(.+?)""");
            return onlyMatch.Groups[1].ToString();
        }

        public void Search()
        {
            WriteProgress($"Looking for {selected.title} in {Settings.Language} ...", SUCCESS);

            RetrieveHtmlAtUrl("http://subscene.com/subtitles/title?q=" + selected.title + "&l=");
            var searchResult = FindSearchResults();
            if (searchResult.Length < 1)
            {
                WriteProgress("FAILURE! Search result gave no hits...", FAILURE);
                return;
            }
            WriteProgress($"Found {searchResult.Length} possible results...", SUCCESS);

            var searchResultPicked = PickCorrectSearchResult(searchResult);
            if (string.IsNullOrEmpty(searchResultPicked))
            {
                WriteProgress("User cancelled the search..", FAILURE);
                return;
            }

            WriteProgress($"Querying for subtitles to {searchResultPicked}...", SUCCESS);
            RetrieveHtmlAtUrl("http://subscene.com/subtitles/" + searchResultPicked);
            selected.url = "http://subscene.com/subtitles/" + searchResultPicked;
            var correctSub = PickCorrectSubtitle();
            if (correctSub.Length < 1)
            {
                WriteProgress("FAILURE! Could not find any subtitles for this release...", FAILURE);
                return;
            }
            WriteProgress($"Found possible match: \"{correctSub}\"...", SUCCESS);

            WriteProgress("Querying download page...", SUCCESS);
            RetrieveHtmlAtUrl("http://subscene.com/subtitles/" + correctSub);
            var downloadLink = FindDownloadUrl();

            var result = InitiateDownload("http://subscene.com/subtitle/download" + downloadLink, selected.GetFullPath()
            );
            if (!result)
            {
                WriteProgress("FAILURE! Error downloading subtitle!", FAILURE);
                return;
            }
            WriteProgress("SUCCESS! Subtitle downloaded!", SUCCESS);

            WriteProgress($"Unpacking rar file at {selected.path}..", SUCCESS);
            UtilityService.UnrarFile(selected.GetFullPath());
        }


        private string PickCorrectSearchResult(ResultPickerItemViewModel[] searchResult)
        {
            if (searchResult.Length == 1)
            {
                return searchResult.First().Address;
            }

            var searchResultPicked = "";

            Application.Current.Dispatcher.Invoke((Action)delegate {
                var dialog = new SearchPickerDialogHandler(searchResult);
                dialog.StartDialog((int result) =>
                {
                    searchResultPicked = result != -1 ? searchResult[result].Address : "";
                });
            });
            
            if (string.IsNullOrEmpty(searchResultPicked)) return string.Empty;

            WriteProgress($"User picked {searchResultPicked}...", SUCCESS);

            selected.url = "http://subscene.com/subtitles/" + searchResultPicked;
            return searchResultPicked;
        }

    }
}
