using System.Text.RegularExpressions;

namespace NxTiler.Application.Parsing;

public static class SessionNameParser
{
    private static readonly Regex FallbackRegex = new(@"\b(?:WoW|Poe)\d+\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled, TimeSpan.FromMilliseconds(200));

    public static string ExtractSessionNameFromTitle(string title)
    {
        var idx1 = title.IndexOf(" - NoMachine", StringComparison.OrdinalIgnoreCase);
        if (idx1 > 0)
        {
            return title[..idx1].Trim();
        }

        const string token = "NoMachine - ";
        var idx2 = title.IndexOf(token, StringComparison.OrdinalIgnoreCase);
        if (idx2 >= 0 && idx2 + token.Length < title.Length)
        {
            return title[(idx2 + token.Length)..].Trim();
        }

        var match = FallbackRegex.Match(title);
        return match.Success ? match.Value : string.Empty;
    }

    public static int ParseNumericSuffix(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return int.MaxValue;
        }

        var match = Regex.Match(value, @"(\d+)$", RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(200));
        if (!match.Success)
        {
            return int.MaxValue;
        }

        return int.TryParse(match.Groups[1].Value, out var numericSuffix) ? numericSuffix : int.MaxValue;
    }
}
