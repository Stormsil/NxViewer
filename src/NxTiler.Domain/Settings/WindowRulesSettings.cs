using NxTiler.Domain.Rules;

namespace NxTiler.Domain.Settings;

public sealed record WindowRulesSettings(
    IReadOnlyList<WindowRule> Rules,
    bool EnableRulesEngine);
