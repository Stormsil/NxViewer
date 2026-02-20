using System.Windows.Interop;
using Microsoft.Extensions.Logging;
using NxTiler.Domain.Enums;
using NxTiler.Domain.Settings;
using NxTiler.Infrastructure.Native;

namespace NxTiler.Infrastructure.Hotkeys;

public sealed partial class GlobalHotkeyService
{
    public async Task RegisterAllAsync(HotkeysSettings settings, CancellationToken ct = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ct.ThrowIfCancellationRequested();

        EnsureSource();
        await UnregisterAllAsync(ct);
        _idToAction.Clear();

        Register(100, settings.ToggleOverlay, HotkeyAction.ToggleOverlay);
        Register(101, settings.ToggleMainWindow, HotkeyAction.ToggleMainWindow);
        Register(102, settings.CycleMode, HotkeyAction.CycleMode);
        Register(103, settings.ToggleMinimize, HotkeyAction.ToggleMinimizeAll);
        Register(104, settings.NavigatePrevious, HotkeyAction.FocusPreviousWindow);
        Register(105, settings.NavigateNext, HotkeyAction.FocusNextWindow);
        Register(106, settings.InstantSnapshot, HotkeyAction.InstantSnapshot);
        Register(107, settings.RegionSnapshot, HotkeyAction.RegionSnapshotWithMask);
        Register(108, settings.Record, HotkeyAction.StartOrConfirmRecording);
        Register(109, settings.Pause, HotkeyAction.PauseOrResumeRecording);
        Register(110, settings.Stop, HotkeyAction.StopOrCancelRecording);
        Register(111, settings.ToggleVision, HotkeyAction.ToggleVisionMode);
    }

    private void Register(int id, HotkeyBinding binding, HotkeyAction action)
    {
        if (_source is null || binding.IsEmpty)
        {
            return;
        }

        if (Win32Native.RegisterHotKey(_source.Handle, id, binding.Modifiers, (uint)binding.VirtualKey))
        {
            _registeredIds.Add(id);
            _idToAction[id] = action;
        }
        else
        {
            logger.LogWarning("Failed to register hotkey {Action} [mod={Modifiers}, key={Key}]", action, binding.Modifiers, binding.VirtualKey);
        }
    }

    private void EnsureSource()
    {
        if (_source is not null)
        {
            return;
        }

        var parameters = new HwndSourceParameters("NxTilerHotkeys")
        {
            Width = 0,
            Height = 0,
            WindowStyle = 0x800000,
        };

        _source = new HwndSource(parameters);
        _source.AddHook(WndProc);
    }
}
