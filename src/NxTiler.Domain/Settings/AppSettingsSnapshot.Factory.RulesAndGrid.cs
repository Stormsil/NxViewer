using NxTiler.Domain.Enums;
using NxTiler.Domain.Grid;
using NxTiler.Domain.Rules;

namespace NxTiler.Domain.Settings;

public sealed partial record AppSettingsSnapshot
{
    private static WindowRulesSettings CreateDefaultWindowRulesSettings()
    {
        var defaultRule = new WindowRule(
            Id: "nomachine-default",
            Name: "NoMachine (default)",
            Priority: 100,
            IsEnabled: true,
            Conditions: new[]
            {
                new RuleCondition(RuleConditionKind.ProcessName, @"^nxplayer(\.exe)?$"),
            },
            Action: RuleAction.Include);

        return new WindowRulesSettings(
            Rules: new[] { defaultRule },
            EnableRulesEngine: true);
    }

    private static WindowGroupsSettings CreateDefaultWindowGroupsSettings()
    {
        return new WindowGroupsSettings(Groups: Array.Empty<WindowGroup>());
    }

    private static GridPresetsSettings CreateDefaultGridPresetsSettings()
    {
        return new GridPresetsSettings(
            DefaultGrid: new GridDimensions(Cols: 6, Rows: 4),
            Presets: Array.Empty<GridPreset>());
    }
}
