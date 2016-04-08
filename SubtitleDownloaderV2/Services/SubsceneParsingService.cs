using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SubtitleDownloaderV2.Properties;
using SubtitleDownloaderV2.Services;
using Settings = SubtitleDownloaderV2.Util.Settings;

namespace SubtitleDownloader.Services
{
    class SubsceneParsingService : WebAccessService
    {
        public string[] FindSearchResults()
        {
            HashSet<string> matchesWithoutDuplicates = new HashSet<string>();

            // Look if we got exact match:
            MatchCollection exactMatches = Regex.Matches(HTML, @"<h2 class=""exact"">(.+?)</ul>", RegexOptions.Singleline);
            if (exactMatches.Count == 1)
            {
                exactMatches = Regex.Matches(exactMatches[0].ToString() , @"/subtitles/(.+?)"">");
                if (exactMatches.Count == 1)
                {
                    string match = exactMatches[0].ToString();
                    matchesWithoutDuplicates.Add(match.Substring(11, match.LastIndexOf('"') - 11));
                    return matchesWithoutDuplicates.ToArray();
                }
            }

            // No dice, pick them all:
            MatchCollection allMatches = Regex.Matches(HTML, @"/subtitles/(.+?)"">");
            for (var i = 0; i < allMatches.Count; i++)
            {
                string match = allMatches[i].ToString();
                if (!match.Contains("/subtitles/title") && !match.Contains("release?"))
                {
                    matchesWithoutDuplicates.Add(match.Substring(11, match.LastIndexOf('"') - 11));
                }
            }
            return matchesWithoutDuplicates.ToArray();
        }

        public string PickCorrectSubtitle(string releaseName, string episode)
        {
            MatchCollection allMatches =
                Regex.Matches(HTML, @"<td class=""a1"">(.+?)<td class=""a3"">", RegexOptions.Singleline);
            for (int i = 0; i < allMatches.Count; i++)
            {
                string singleMatch = allMatches[i].ToString();
                if (singleMatch.Contains(Settings.Language) && singleMatch.Contains("positive-icon") &&
                    singleMatch.Contains(releaseName) && singleMatch.Contains(episode))
                {
                    string correct = Regex.Match(singleMatch, @"/subtitles/(.+?)"">").ToString();
                    return correct.Substring(0, correct.Length - 2);
                }
            }
            return string.Empty;
        }

        public string FindDownloadUrl()
        {
            Match onlyMatch = Regex.Match(HTML, @"/subtitle/download(.+?)""");
            return onlyMatch.ToString().Substring(0, onlyMatch.ToString().Length - 1);
        }
    }
}
