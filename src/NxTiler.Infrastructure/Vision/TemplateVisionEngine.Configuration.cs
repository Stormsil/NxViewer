namespace NxTiler.Infrastructure.Vision;

public sealed partial class TemplateVisionEngine
{
    private string ResolveTemplateDirectory()
    {
        var fromSettings = settingsService.Current.Vision.TemplateDirectory;
        if (!string.IsNullOrWhiteSpace(fromSettings))
        {
            return fromSettings;
        }

        var fromEnvironment = Environment.GetEnvironmentVariable(TemplateDirectoryVariable);
        if (!string.IsNullOrWhiteSpace(fromEnvironment))
        {
            return fromEnvironment;
        }

        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "NxTiler",
            "VisionTemplates");
    }

    private static IEnumerable<string> EnumerateTemplateFiles(string directory)
    {
        if (!Directory.Exists(directory))
        {
            return Array.Empty<string>();
        }

        var patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp" };
        return patterns
            .SelectMany(pattern => Directory.EnumerateFiles(directory, pattern, SearchOption.TopDirectoryOnly))
            .Distinct(StringComparer.OrdinalIgnoreCase);
    }
}
