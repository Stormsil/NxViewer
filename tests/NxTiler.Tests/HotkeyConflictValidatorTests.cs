using NxTiler.Application.Validation;
using NxTiler.Domain.Settings;

namespace NxTiler.Tests;

public sealed class HotkeyConflictValidatorTests
{
    [Fact]
    public void HasConflicts_ReturnsTrue_WhenDuplicateBindingsExist()
    {
        var settings = CreateSettings(
            toggleOverlay: new HotkeyBinding(2, 112),
            toggleMainWindow: new HotkeyBinding(2, 112));

        var hasConflicts = HotkeyConflictValidator.HasConflicts(settings);

        Assert.True(hasConflicts);
    }

    [Fact]
    public void HasConflicts_ReturnsFalse_WhenOnlyEmptyBindingsOverlap()
    {
        var settings = CreateSettings(
            pause: HotkeyBinding.Empty,
            stop: HotkeyBinding.Empty);

        var hasConflicts = HotkeyConflictValidator.HasConflicts(settings);

        Assert.False(hasConflicts);
    }

    [Fact]
    public void HasConflicts_ReturnsFalse_WhenAllBindingsAreUnique()
    {
        var settings = CreateSettings();

        var hasConflicts = HotkeyConflictValidator.HasConflicts(settings);

        Assert.False(hasConflicts);
    }

    [Fact]
    public void HasConflicts_ReturnsTrue_WhenSnapshotAndRecordingBindingsOverlap()
    {
        var settings = CreateSettings(
            instantSnapshot: new HotkeyBinding(0, 113),
            record: new HotkeyBinding(0, 113));

        var hasConflicts = HotkeyConflictValidator.HasConflicts(settings);

        Assert.True(hasConflicts);
    }

    [Fact]
    public void HasConflicts_ReturnsTrue_WhenVisionAndSnapshotBindingsOverlap()
    {
        var settings = CreateSettings(
            instantSnapshot: new HotkeyBinding(6, 113),
            toggleVision: new HotkeyBinding(6, 113));

        var hasConflicts = HotkeyConflictValidator.HasConflicts(settings);

        Assert.True(hasConflicts);
    }

    private static HotkeysSettings CreateSettings(
        HotkeyBinding? toggleOverlay = null,
        HotkeyBinding? toggleMainWindow = null,
        HotkeyBinding? cycleMode = null,
        HotkeyBinding? toggleMinimize = null,
        HotkeyBinding? navigatePrevious = null,
        HotkeyBinding? navigateNext = null,
        HotkeyBinding? instantSnapshot = null,
        HotkeyBinding? regionSnapshot = null,
        HotkeyBinding? record = null,
        HotkeyBinding? pause = null,
        HotkeyBinding? stop = null,
        HotkeyBinding? toggleVision = null)
    {
        return new HotkeysSettings(
            ToggleOverlay: toggleOverlay ?? new HotkeyBinding(0, 112),
            ToggleMainWindow: toggleMainWindow ?? new HotkeyBinding(2, 112),
            CycleMode: cycleMode ?? new HotkeyBinding(4, 112),
            ToggleMinimize: toggleMinimize ?? new HotkeyBinding(0, 192),
            NavigatePrevious: navigatePrevious ?? new HotkeyBinding(0, 37),
            NavigateNext: navigateNext ?? new HotkeyBinding(0, 39),
            InstantSnapshot: instantSnapshot ?? new HotkeyBinding(6, 113),
            RegionSnapshot: regionSnapshot ?? new HotkeyBinding(6, 114),
            Record: record ?? new HotkeyBinding(0, 113),
            Pause: pause ?? new HotkeyBinding(0, 114),
            Stop: stop ?? new HotkeyBinding(0, 115))
        {
            ToggleVision = toggleVision ?? new HotkeyBinding(6, 115),
        };
    }
}
