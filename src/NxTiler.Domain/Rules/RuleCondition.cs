namespace NxTiler.Domain.Rules;

public sealed record RuleCondition(
    RuleConditionKind Kind,
    string Pattern,
    bool IsNegated = false);
