using NxTiler.Domain.Settings;

namespace NxTiler.Infrastructure.Settings;

public sealed partial class JsonSettingsService
{
    private static WindowRulesSettings NormalizeRules(WindowRulesSettings rules, WindowRulesSettings defaults)
    {
        var normalized = rules with
        {
            Rules = rules.Rules ?? defaults.Rules,
        };

        // Ensure the built-in NoMachine rule is present when rules engine is enabled
        if (normalized.EnableRulesEngine && !normalized.Rules.Any(r => r.Id == "nomachine-default"))
        {
            var defaultRule = defaults.Rules.FirstOrDefault(r => r.Id == "nomachine-default");
            if (defaultRule is not null)
            {
                normalized = normalized with
                {
                    Rules = new[] { defaultRule }.Concat(normalized.Rules).ToArray(),
                };
            }
        }

        return normalized;
    }

    private static WindowGroupsSettings NormalizeGroups(WindowGroupsSettings groups)
    {
        return groups with
        {
            Groups = groups.Groups ?? Array.Empty<NxTiler.Domain.Rules.WindowGroup>(),
        };
    }

    private static GridPresetsSettings NormalizeGridPresets(GridPresetsSettings presets, GridPresetsSettings defaults)
    {
        return presets with
        {
            DefaultGrid = presets.DefaultGrid ?? defaults.DefaultGrid,
            Presets = presets.Presets ?? Array.Empty<NxTiler.Domain.Grid.GridPreset>(),
        };
    }
}
