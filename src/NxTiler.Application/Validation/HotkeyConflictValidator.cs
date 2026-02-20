using NxTiler.Domain.Settings;

namespace NxTiler.Application.Validation;

public static class HotkeyConflictValidator
{
    public static bool HasConflicts(HotkeysSettings settings)
    {
        var allBindings = new[]
        {
            settings.ToggleOverlay,
            settings.ToggleMainWindow,
            settings.CycleMode,
            settings.ToggleMinimize,
            settings.NavigatePrevious,
            settings.NavigateNext,
            settings.InstantSnapshot,
            settings.RegionSnapshot,
            settings.Record,
            settings.Pause,
            settings.Stop,
            settings.ToggleVision,
        };

        var seen = new HashSet<(uint Mod, int Key)>();
        foreach (var binding in allBindings)
        {
            if (binding.IsEmpty)
            {
                continue;
            }

            var identity = (binding.Modifiers, binding.VirtualKey);
            if (!seen.Add(identity))
            {
                return true;
            }
        }

        return false;
    }
}
