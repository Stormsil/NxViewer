using System.Text.RegularExpressions;

namespace NxTiler.Domain.Tracking;

public sealed record WindowIdentity(
    nint Handle,
    uint ProcessId,
    string ExePath,
    string ExeBaseName,
    string CommandLine,
    string WindowClassName,
    string LastKnownTitle)
{
    public string? NxsFilePath => ExtractNxsPath(CommandLine);

    public string? SessionNameFromNxs =>
        NxsFilePath is not null ? Path.GetFileNameWithoutExtension(NxsFilePath) : null;

    private static string? ExtractNxsPath(string? cmd)
    {
        if (string.IsNullOrEmpty(cmd))
        {
            return null;
        }

        var match = Regex.Match(cmd, @"[""']?([^""'\s]+\.nxs)[""']?", RegexOptions.IgnoreCase);
        var value = match.Groups[1].Value;
        return value.Length > 0 ? value : null;
    }
}
