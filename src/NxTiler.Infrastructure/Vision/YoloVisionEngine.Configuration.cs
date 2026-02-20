namespace NxTiler.Infrastructure.Vision;

public sealed partial class YoloVisionEngine
{
    private string ResolveModelPath()
    {
        var fromSettings = _settingsService.Current.Vision.YoloModelPath;
        if (!string.IsNullOrWhiteSpace(fromSettings))
        {
            return fromSettings;
        }

        return Environment.GetEnvironmentVariable(YoloModelPathVariable) ?? string.Empty;
    }

    private static string ResolveLabel(int classId, IReadOnlyList<string> labels)
    {
        if (classId >= 0 && classId < labels.Count)
        {
            return labels[classId];
        }

        return $"class_{classId}";
    }
}
