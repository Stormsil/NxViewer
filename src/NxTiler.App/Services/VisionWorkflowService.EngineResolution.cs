using NxTiler.Application.Abstractions;
using NxTiler.Domain.Settings;

namespace NxTiler.App.Services;

public sealed partial class VisionWorkflowService
{
    private IVisionEngine? ResolveEngine()
    {
        var settings = settingsService.Current;
        var featureFlags = settings.FeatureFlags;

        if (!string.IsNullOrWhiteSpace(settings.Vision.PreferredEngine)
            && _engines.TryGetValue(settings.Vision.PreferredEngine, out var preferred)
            && IsEngineEnabled(preferred.Name, featureFlags))
        {
            return preferred;
        }

        if (featureFlags.EnableYoloEngine && _engines.TryGetValue("yolo", out var yolo))
        {
            return yolo;
        }

        if (featureFlags.EnableTemplateMatchingFallback && _engines.TryGetValue("template", out var template))
        {
            return template;
        }

        return _engines.Values.FirstOrDefault(x => IsEngineEnabled(x.Name, featureFlags));
    }

    private static bool IsEngineEnabled(string engineName, FeatureFlagsSettings featureFlags)
    {
        if (engineName.Equals("yolo", StringComparison.OrdinalIgnoreCase))
        {
            return featureFlags.EnableYoloEngine;
        }

        if (engineName.Equals("template", StringComparison.OrdinalIgnoreCase))
        {
            return featureFlags.EnableTemplateMatchingFallback;
        }

        return true;
    }

    private IVisionEngine? ResolveTemplateFallback()
    {
        var featureFlags = settingsService.Current.FeatureFlags;
        if (!featureFlags.EnableTemplateMatchingFallback)
        {
            return null;
        }

        return _engines.TryGetValue("template", out var template) ? template : null;
    }
}
