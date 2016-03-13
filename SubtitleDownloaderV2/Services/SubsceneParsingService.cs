using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SubtitleDownloaderV2.Services;

namespace SubtitleDownloader.Services
{
    class SubsceneParsingService : WebAccessService
    {
        public String[] FindSearchResults()
        {
            HashSet<String> matchesWithoutDuplicates = new HashSet<string>();

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
                    return correct.Substring(0, correct.Length - 2);
                }
            }
            return String.Empty;
        }

        public string FindDownloadUrl()
        {
            Match onlyMatch = Regex.Match(HTML, @"/subtitle/download(.+?)""");
            return onlyMatch.ToString().Substring(0, onlyMatch.ToString().Length - 1);
        }
    }
}
