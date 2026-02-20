using NxTiler.Domain.Rules;
using NxTiler.Domain.Windowing;

namespace NxTiler.Application.Abstractions;

public interface IWindowGroupService
{
    IReadOnlyDictionary<string, WindowGroupState> Groups { get; }

    void AssignWindow(nint handle, string groupId);

    void RemoveWindow(nint handle);

    event EventHandler GroupsChanged;
}

public sealed record WindowGroupState(
    WindowGroup Config,
    IReadOnlyList<TargetWindowInfo> Windows,
    nint? FocusedWindow);
