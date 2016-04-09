using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
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
    
        private string[] allResults;
        private string chosenResult;

        internal delegate void WriteToProgressWindow(string message, bool success);
        public WriteToProgressWindow WriteProgress;
        
        public SubsceneService(FileEntry selected)
        {
            this.selected = selected;
        }

        public string[] FindSearchResults()
        {
            var matchesWithoutDuplicates = new HashSet<string>();

            // Look if we got exact match:
            var exactMatches = Regex.Matches(html, @"<h2 class=""exact"">(.+?)</ul>", RegexOptions.Singleline);
            if (exactMatches.Count == 1)
            {
                exactMatches = Regex.Matches(exactMatches[0].ToString(), @"/subtitles/(.+?)"">");
                if (exactMatches.Count == 1)
                {
                    var match = exactMatches[0].ToString();
                    matchesWithoutDuplicates.Add(match.Substring(11, match.LastIndexOf('"') - 11));
                    allResults = matchesWithoutDuplicates.ToArray();
                    return allResults;
                }
            }

            // No dice, pick them all:
            var allMatches = Regex.Matches(html, @"/subtitles/(.+?)"">");
            for (var i = 0; i < allMatches.Count; i++)
            {
                var match = allMatches[i].ToString();
                if (!match.Contains("/subtitles/title") && !match.Contains("release?"))
                {
                    matchesWithoutDuplicates.Add(match.Substring(11, match.LastIndexOf('"') - 11));
                }
            }
            allResults = matchesWithoutDuplicates.ToArray();
            return allResults;
        }

        public string PickCorrectSubtitle()
        {
            var allMatches =
                Regex.Matches(html, @"<td class=""a1"">(.+?)<td class=""a3"">", RegexOptions.Singleline);
            for (var i = 0; i < allMatches.Count; i++)
            {
                var singleMatch = allMatches[i].ToString();
                if (singleMatch.Contains(Settings.Language) && singleMatch.Contains("positive-icon") &&
                    singleMatch.Contains(selected.release) && singleMatch.Contains(selected.episode))
                {
                    var correct = Regex.Match(singleMatch, @"/subtitles/(.+?)"">").ToString();
                    chosenResult = correct.Substring(0, correct.Length - 2);
                    return chosenResult;
                }
            }
            chosenResult = string.Empty;
            return chosenResult;
        }

        public string FindDownloadUrl()
        {
            var onlyMatch = Regex.Match(html, @"/subtitle/download(.+?)""");
            return onlyMatch.ToString().Substring(0, onlyMatch.ToString().Length - 1);
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
                WriteProgress("User cancelled..", FAILURE);
                return;
            }

            WriteProgress($"Querying for subtitles to {searchResultPicked}...", SUCCESS);
            RetrieveHtmlAtUrl("http://subscene.com/subtitles/" + searchResultPicked);
            var correctSub = PickCorrectSubtitle();
            if (correctSub.Length < 1)
            {
                WriteProgress("FAILURE! Could not find any subtitles for this release...", FAILURE);
                return;
            }
            WriteProgress("Found possible match...", SUCCESS);

            WriteProgress("Querying download page...", SUCCESS);
            RetrieveHtmlAtUrl("http://subscene.com/" + correctSub);
            var downloadLink = FindDownloadUrl();

            var result = InitiateDownload("http://subscene.com" + downloadLink, selected.GetFullPath()
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


        private string PickCorrectSearchResult(string[] searchResult)
        {
            if (searchResult.Length == 1)
            {
                return searchResult.First();
            }

            var searchResultPicked = "";

            Application.Current.Dispatcher.Invoke((Action)delegate {
                var pickEntryForm = new ResultPickerView(searchResult);
                pickEntryForm.ShowDialog();

                if (pickEntryForm.getReturnValue() != -1)
                {
                    searchResultPicked = searchResult[pickEntryForm.getReturnValue()];
                } 

                pickEntryForm.Close();
            });

            if (string.IsNullOrEmpty(searchResultPicked)) return string.Empty;

            WriteProgress($"User picked {searchResultPicked}...", SUCCESS);

            selected.url = "http://subscene.com/subtitles/" + searchResultPicked;
            return searchResultPicked;
        }

    }
}
