using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace NxTiler.Infrastructure.Nomachine;

public sealed partial class NomachineSessionService
{
    private IEnumerable<string> FilterSessionNames(IEnumerable<string> sessionNames, string regex)
    {
        if (string.IsNullOrWhiteSpace(regex))
        {
            return sessionNames;
        }

        Regex? compiled;
        lock (_regexCacheLock)
        {
            if (_invalidRegexCache.Contains(regex))
            {
                return sessionNames;
            }

            if (!_regexCache.TryGetValue(regex, out compiled))
            {
                try
                {
                    compiled = new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled, TimeSpan.FromMilliseconds(200));
                    _regexCache[regex] = compiled;
                    if (_regexCache.Count > 64)
                    {
                        _regexCache.Clear();
                    }
                }
                catch (Exception ex)
                {
                    _invalidRegexCache.Add(regex);
                    if (_invalidRegexCache.Count > 64)
                    {
                        _invalidRegexCache.Clear();
                    }

                    logger.LogWarning(ex, "Invalid session filter regex: {Regex}", regex);
                    return sessionNames;
                }
            }
        }

        return sessionNames.Where(x => compiled.IsMatch(x));
    }
}
