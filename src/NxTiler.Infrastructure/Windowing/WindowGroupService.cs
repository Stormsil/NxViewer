using System.Collections.Concurrent;
using NxTiler.Application.Abstractions;
using NxTiler.Domain.Rules;
using NxTiler.Domain.Windowing;

namespace NxTiler.Infrastructure.Windowing;

public sealed class WindowGroupService(ISettingsService settingsService) : IWindowGroupService
{
    private readonly ConcurrentDictionary<nint, string> _windowToGroup = new();

    public event EventHandler? GroupsChanged;

    public IReadOnlyDictionary<string, WindowGroupState> Groups
    {
        get
        {
            var configGroups = settingsService.Current.Groups.Groups;
            var result = new Dictionary<string, WindowGroupState>(StringComparer.Ordinal);

            foreach (var group in configGroups)
            {
                var windows = _windowToGroup
                    .Where(kvp => kvp.Value == group.Id)
                    .Select(kvp => new TargetWindowInfo(kvp.Key, string.Empty, string.Empty, false, 0))
                    .ToList();

                result[group.Id] = new WindowGroupState(
                    Config: group,
                    Windows: windows,
                    FocusedWindow: null);
            }

            return result;
        }
    }

    public void AssignWindow(nint handle, string groupId)
    {
        _windowToGroup[handle] = groupId;
        GroupsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RemoveWindow(nint handle)
    {
        _windowToGroup.TryRemove(handle, out _);
        GroupsChanged?.Invoke(this, EventArgs.Empty);
    }
}
