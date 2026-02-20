namespace NxTiler.Domain.Rules;

public sealed record WindowRule(
    string Id,
    string Name,
    int Priority,
    bool IsEnabled,
    IReadOnlyList<RuleCondition> Conditions,
    RuleAction Action,
    string? GroupId = null);
