using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using NxTiler.Application.Abstractions;
using NxTiler.Domain.Rules;
using NxTiler.Domain.Tracking;
using NxTiler.Domain.Windowing;

namespace NxTiler.Infrastructure.Windowing;

public sealed class WindowRulesEngine(ISettingsService settingsService) : IWindowRulesEngine
{
    private static readonly RegexOptions DefaultOptions =
        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant;

    private readonly ConcurrentDictionary<string, Regex> _regexCache = new(StringComparer.Ordinal);

    public WindowRule? Evaluate(WindowIdentity identity)
    {
        var settings = settingsService.Current.Rules;
        if (!settings.EnableRulesEngine)
        {
            return null;
        }

        var rules = settings.Rules
            .Where(static r => r.IsEnabled)
            .OrderBy(static r => r.Priority);

        foreach (var rule in rules)
        {
            if (MatchesAll(rule.Conditions, identity))
            {
                return rule;
            }
        }

        return null;
    }

    public WindowRulesEvaluation EvaluateAll(IEnumerable<WindowIdentity> identities)
    {
        var windows = identities.Select(id => new TargetWindowInfo(
            Handle: id.Handle,
            Title: id.LastKnownTitle,
            SourceName: id.SessionNameFromNxs ?? id.LastKnownTitle,
            IsMaximized: false,
            ProcessId: id.ProcessId)
        {
            Identity = id,
        }).ToList();

        return EvaluateAll(windows);
    }

    public WindowRulesEvaluation EvaluateAll(IEnumerable<TargetWindowInfo> windows)
    {
        var included = new List<TargetWindowInfo>();
        var floating = new List<TargetWindowInfo>();
        var ignored = new List<TargetWindowInfo>();
        var groups = new Dictionary<string, List<TargetWindowInfo>>(StringComparer.Ordinal);

        var settings = settingsService.Current.Rules;

        foreach (var window in windows)
        {
            if (!settings.EnableRulesEngine)
            {
                included.Add(window);
                continue;
            }

            var identity = window.Identity;
            if (identity is null)
            {
                included.Add(window);
                continue;
            }

            var rule = Evaluate(identity);
            if (rule is null)
            {
                included.Add(window);
                continue;
            }

            switch (rule.Action)
            {
                case RuleAction.Include:
                    included.Add(window);
                    break;

                case RuleAction.Float:
                    floating.Add(window);
                    break;

                case RuleAction.Ignore:
                    ignored.Add(window);
                    break;

                case RuleAction.AssignGroup when rule.GroupId is not null:
                    if (!groups.TryGetValue(rule.GroupId, out var groupList))
                    {
                        groupList = new List<TargetWindowInfo>();
                        groups[rule.GroupId] = groupList;
                    }

                    groupList.Add(window);
                    break;

                default:
                    included.Add(window);
                    break;
            }
        }

        return new WindowRulesEvaluation(
            Included: included,
            Floating: floating,
            Ignored: ignored,
            Groups: groups.ToDictionary(
                static kvp => kvp.Key,
                static kvp => (IReadOnlyList<TargetWindowInfo>)kvp.Value,
                StringComparer.Ordinal));
    }

    private bool MatchesAll(IReadOnlyList<RuleCondition> conditions, WindowIdentity identity)
    {
        foreach (var condition in conditions)
        {
            var matches = MatchesCondition(condition, identity);
            if (condition.IsNegated ? matches : !matches)
            {
                return false;
            }
        }

        return true;
    }

    private bool MatchesCondition(RuleCondition condition, WindowIdentity identity)
    {
        return condition.Kind switch
        {
            RuleConditionKind.ProcessName => GetOrCompileRegex(condition.Pattern).IsMatch(identity.ExeBaseName),
            RuleConditionKind.ExePathRegex => GetOrCompileRegex(condition.Pattern).IsMatch(identity.ExePath),
            RuleConditionKind.CommandLineRegex => GetOrCompileRegex(condition.Pattern).IsMatch(identity.CommandLine),
            RuleConditionKind.WindowClassName => GetOrCompileRegex(condition.Pattern).IsMatch(identity.WindowClassName),
            RuleConditionKind.TitleRegex => GetOrCompileRegex(condition.Pattern).IsMatch(identity.LastKnownTitle),
            _ => false,
        };
    }

    private Regex GetOrCompileRegex(string pattern)
    {
        return _regexCache.GetOrAdd(pattern, static p => new Regex(p, DefaultOptions));
    }
}
