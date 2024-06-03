using System.Text.RegularExpressions;

namespace JobCrawler.Domain.Helpers;

public static class TimeExtractor
{
    public static int? GetMinutes(string timeString)
    {
        // Regular expression to match the pattern "xx minutes ago" or "xx minute ago"
        var match = Regex.Match(timeString, @"(\d+)\s+minute(s?)\s+ago", RegexOptions.IgnoreCase);

        if (match.Success && int.TryParse(match.Groups[1].Value, out var minutes))
        {
            return minutes;
        }

        return null;
    }
    
    public static int? GetSeconds(string timeString)
    {
        // Regular expression to match the pattern "xx seconds ago" or "xx second ago"
        var match = Regex.Match(timeString, @"(\d+)\s+minute(s?)\s+ago", RegexOptions.IgnoreCase);

        if (match.Success && int.TryParse(match.Groups[1].Value, out var minutes))
        {
            return minutes * 60;
        }

        return null;
    }
}