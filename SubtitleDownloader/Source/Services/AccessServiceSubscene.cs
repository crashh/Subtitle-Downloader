using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SubtitleDownloader.Services;

namespace SubtitleDownloader
{
    class AccessServiceSubscene : WebAccess, IAccessService
    {
        public String[] FindSearchResults()
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
            return String.Empty;
        }

        public string FindDownloadUrl()
        {
            Match onlyMatch = Regex.Match(HTML, @"/subtitle/download(.+?)""");
            return onlyMatch.ToString().Substring(0,onlyMatch.ToString().Length-1);
        }
    }
}
