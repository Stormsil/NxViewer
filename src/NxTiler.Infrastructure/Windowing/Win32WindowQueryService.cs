using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using NxTiler.Application.Abstractions;

namespace NxTiler.Infrastructure.Windowing;

public sealed partial class Win32WindowQueryService(
    ILogger<Win32WindowQueryService> logger,
    IWindowTracker? windowTracker = null) : IWindowQueryService
{
    private readonly object _regexCacheLock = new();
    private readonly Dictionary<string, Regex> _regexCache = new(StringComparer.Ordinal);
    private readonly HashSet<string> _invalidRegexCache = new(StringComparer.Ordinal);
}
