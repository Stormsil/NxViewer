namespace NxTiler.Domain.Rules;

public enum RuleConditionKind
{
    ProcessName = 0,
    ExePathRegex = 1,
    CommandLineRegex = 2,
    WindowClassName = 3,
    TitleRegex = 4,
}
