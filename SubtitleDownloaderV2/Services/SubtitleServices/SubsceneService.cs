using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using SubtitleDownloader.Properties;
using SubtitleDownloader.ViewModel.SubtitleSearch.Dialog;
using SubtitleDownloaderV2.Dialogs;
using SubtitleDownloaderV2.Model;
using SubtitleDownloaderV2.Util;

namespace SubtitleDownloaderV2.Services
{
    internal class SubsceneService : WebAccessService
    {
        private const bool SUCCESS = true;
        private const bool FAILURE = false;

        private readonly FileEntry _selected;

        internal delegate void WriteToProgressWindow(string message, bool success);
        private readonly WriteToProgressWindow _writeProgress;
        
        public SubsceneService(FileEntry selected, WriteToProgressWindow writeToProgressWindow)
        {
            _selected = selected;
            _writeProgress = writeToProgressWindow;
        }

        public ResultPickerItemViewModel[] FindSearchResults()
        {
            var result = new List<ResultPickerItemViewModel>();

            // Look if we got an exact match:
            var exactMatches = Regex.Matches(html, @"<h2 class=""exact"">(.+?)</ul>", RegexOptions.Singleline);
            if (exactMatches.Count == 1)
            {
                var exactMatch = Regex.Matches(exactMatches[0].ToString(), @"/subtitles/(.+?)"">(.+?)</a>");
                if (exactMatch.Count > 2)
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
                if (singleMatch.Contains(SubSearchSettings.Default.SelectedLanguage) && singleMatch.Contains("positive-icon") &&
                    singleMatch.Contains(_selected.Release) && singleMatch.Contains(_selected.Episode))
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
            if (_selected == null)
            {
                return;
            }

            _writeProgress($"Looking for {_selected.Title} in {SubSearchSettings.Default.SelectedLanguage} ...", SUCCESS);

            RetrieveHtmlAtUrl("http://subscene.com/subtitles/title?q=" + _selected.Title + "&l=");
            var searchResult = FindSearchResults();
            if (searchResult.Length < 1)
            {
                _writeProgress("FAILURE! Search result gave no hits...", FAILURE);
                return;
            }
            _writeProgress($"Found {searchResult.Length} possible results...", SUCCESS);

            var searchResultPicked = PickCorrectSearchResult(searchResult);
            if (string.IsNullOrEmpty(searchResultPicked))
            {
                _writeProgress("User cancelled the search..", FAILURE);
                return;
            }

            _writeProgress($"Querying for subtitles to {searchResultPicked}...", SUCCESS);
            RetrieveHtmlAtUrl("http://subscene.com/subtitles/" + searchResultPicked);
            _selected.Url = "http://subscene.com/subtitles/" + searchResultPicked;
            var correctSub = PickCorrectSubtitle();
            if (correctSub.Length < 1)
            {
                _writeProgress("FAILURE! Could not find any subtitles for this release...", FAILURE);
                return;
            }
            _writeProgress($"Found possible match: \"{correctSub}\"...", SUCCESS);

            _writeProgress("Querying download page...", SUCCESS);
            RetrieveHtmlAtUrl("http://subscene.com/subtitles/" + correctSub);
            var downloadLink = FindDownloadUrl();

            var result = InitiateDownload("http://subscene.com/subtitle/download" + downloadLink, _selected.GetFullPath());
            if (!result)
            {
                _writeProgress("FAILURE! Error downloading subtitle!", FAILURE);
                return;
            }
            _writeProgress("SUCCESS! Subtitle downloaded!", SUCCESS);

            _writeProgress($"Unpacking rar file at {_selected.Path}..", SUCCESS);
            UtilityService.UnrarFile(_selected.GetFullPath());
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

            if (string.IsNullOrEmpty(searchResultPicked))
            {
                return string.Empty;
            }

            _writeProgress($"User picked {searchResultPicked}...", SUCCESS);

            _selected.Url = "http://subscene.com/subtitles/" + searchResultPicked;
            return searchResultPicked;
        }

    }
}
