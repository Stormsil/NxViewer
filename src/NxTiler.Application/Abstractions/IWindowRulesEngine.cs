using NxTiler.Domain.Rules;
using NxTiler.Domain.Tracking;
using NxTiler.Domain.Windowing;

namespace NxTiler.Application.Abstractions;

public interface IWindowRulesEngine
{
    WindowRule? Evaluate(WindowIdentity identity);

    WindowRulesEvaluation EvaluateAll(IEnumerable<WindowIdentity> identities);

    WindowRulesEvaluation EvaluateAll(IEnumerable<TargetWindowInfo> windows);
}

public sealed record WindowRulesEvaluation(
    IReadOnlyList<TargetWindowInfo> Included,
    IReadOnlyList<TargetWindowInfo> Floating,
    IReadOnlyList<TargetWindowInfo> Ignored,
    IReadOnlyDictionary<string, IReadOnlyList<TargetWindowInfo>> Groups);
