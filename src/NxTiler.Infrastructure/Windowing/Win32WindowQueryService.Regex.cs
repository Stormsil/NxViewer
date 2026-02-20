using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace NxTiler.Infrastructure.Windowing;

public sealed partial class Win32WindowQueryService
{
    private Regex? BuildRegex(string? pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
        {
            return null;
        }

        lock (_regexCacheLock)
        {
            if (_invalidRegexCache.Contains(pattern))
            {
                return null;
            }

            if (_regexCache.TryGetValue(pattern, out var cached))
            {
                return cached;
            }

            try
            {
                var compiled = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled, TimeSpan.FromMilliseconds(200));
                _regexCache[pattern] = compiled;
                if (_regexCache.Count > 64)
                {
                    _regexCache.Clear();
                }

                return compiled;
            }
            catch (Exception ex)
            {
                _invalidRegexCache.Add(pattern);
                if (_invalidRegexCache.Count > 64)
                {
                    _invalidRegexCache.Clear();
                }

                logger.LogWarning(ex, "Invalid regex pattern: {Pattern}", pattern);
                return null;
            }
        }
    }
}
