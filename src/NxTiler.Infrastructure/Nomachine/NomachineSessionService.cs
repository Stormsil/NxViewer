using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using NxTiler.Application.Abstractions;

namespace NxTiler.Infrastructure.Nomachine;

public sealed partial class NomachineSessionService(ILogger<NomachineSessionService> logger) : INomachineSessionService
{
    private readonly object _regexCacheLock = new();
    private readonly Dictionary<string, Regex> _regexCache = new(StringComparer.Ordinal);
    private readonly HashSet<string> _invalidRegexCache = new(StringComparer.Ordinal);
}
