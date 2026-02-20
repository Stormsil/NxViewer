using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using NxTiler.App.Messages;
using NxTiler.Domain.Enums;

namespace NxTiler.App.Services;

public sealed partial class WorkspaceOrchestrator
{
    private void HotkeyServiceOnHotkeyPressed(HotkeyAction action)
    {
        _ = HandleHotkeyPressedAsync(action);
    }

    private async Task HandleHotkeyPressedAsync(HotkeyAction action)
    {
        switch (action)
        {
            case HotkeyAction.ToggleOverlay:
                _messenger.Send(new OverlayToggleRequestedMessage());
                break;
            case HotkeyAction.ToggleMainWindow:
                RequestMainWindowToggle();
                break;
            case HotkeyAction.CycleMode:
                await CycleModeAsync();
                break;
            case HotkeyAction.ToggleMinimizeAll:
                await ToggleMinimizeAllAsync();
                break;
            case HotkeyAction.FocusPreviousWindow:
                await NavigateAsync(-1);
                break;
            case HotkeyAction.FocusNextWindow:
                await NavigateAsync(1);
                break;
            case HotkeyAction.InstantSnapshot:
                await ExecuteSnapshotAsync(region: false);
                break;
            case HotkeyAction.RegionSnapshotWithMask:
                await ExecuteSnapshotAsync(region: true);
                break;
            case HotkeyAction.ToggleVisionMode:
                await ExecuteVisionToggleAsync();
                break;
            case HotkeyAction.StartOrConfirmRecording:
                await HandleStartOrConfirmRecordingHotkeyAsync();
                break;
            case HotkeyAction.PauseOrResumeRecording:
                await HandlePauseOrResumeRecordingHotkeyAsync();
                break;
            case HotkeyAction.StopOrCancelRecording:
                await HandleStopOrCancelRecordingHotkeyAsync();
                break;
            case HotkeyAction.OpenGridEditor:
                await HandleOpenGridEditorAsync();
                break;
            case HotkeyAction.ApplyGridPreset1:
            case HotkeyAction.ApplyGridPreset2:
            case HotkeyAction.ApplyGridPreset3:
            case HotkeyAction.ApplyGridPreset4:
            case HotkeyAction.ApplyGridPreset5:
            case HotkeyAction.ApplyGridPreset6:
            case HotkeyAction.ApplyGridPreset7:
            case HotkeyAction.ApplyGridPreset8:
            case HotkeyAction.ApplyGridPreset9:
            case HotkeyAction.ApplyGridPreset10:
                var presetIndex = (int)action - (int)HotkeyAction.ApplyGridPreset1;
                await HandleApplyGridPresetAsync(presetIndex);
                break;
        }
    }

    private Task HandleOpenGridEditorAsync()
    {
        _logger.LogDebug("OpenGridEditor hotkey pressed (ImGui grid editor not yet active).");
        return Task.CompletedTask;
    }

    private async Task HandleApplyGridPresetAsync(int presetIndex)
    {
        if (_gridPresetService is null)
        {
            return;
        }

        var presets = _gridPresetService.Presets;
        if (presetIndex < 0 || presetIndex >= presets.Count)
        {
            _logger.LogDebug("Grid preset index {Index} out of range (count={Count}).", presetIndex, presets.Count);
            return;
        }

        var targetHandle = _focusedWindow != nint.Zero ? _focusedWindow
            : _targets.Count > 0 ? _targets[0].Handle
            : nint.Zero;

        if (targetHandle == nint.Zero)
        {
            return;
        }

        try
        {
            await _gridPresetService.ApplyPresetAsync(presets[presetIndex].Id, targetHandle);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply grid preset {Name}.", presets[presetIndex].Name);
        }
    }
}
