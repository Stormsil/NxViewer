using NxTiler.Application.Services;
using NxTiler.Domain.Enums;
using NxTiler.Domain.Windowing;

namespace NxTiler.Tests;

public sealed class ArrangementServiceTests
{
    [Fact]
    public void BuildPlacements_GridMode_ReturnsPlacementForEachWindow()
    {
        var service = new ArrangementService();
        var windows = CreateWindows(4);
        var context = new ArrangementContext(TileMode.Grid, new WindowBounds(0, 0, 1920, 1080), Gap: 8, TopPad: 20, FocusedWindow: null);

        var placements = service.BuildPlacements(windows, TileMode.Grid, context);

        Assert.Equal(4, placements.Count);
        Assert.Equal(4, placements.Select(x => x.Handle).Distinct().Count());
        Assert.All(placements, placement =>
        {
            Assert.True(placement.Width > 0);
            Assert.True(placement.Height > 0);
            Assert.True(placement.Y >= 20);
        });
    }

    [Fact]
    public void BuildPlacements_FocusMode_FocusedWindowGetsLargestArea()
    {
        var service = new ArrangementService();
        var windows = CreateWindows(5);
        var focused = windows[2].Handle;
        var context = new ArrangementContext(TileMode.Focus, new WindowBounds(0, 0, 1600, 900), Gap: 8, TopPad: 16, FocusedWindow: focused);

        var placements = service.BuildPlacements(windows, TileMode.Focus, context);
        var focusedPlacement = placements.Single(x => x.Handle == focused);
        var largestArea = placements.Max(x => x.Width * x.Height);

        Assert.Equal(windows.Count, placements.Count);
        Assert.Equal(largestArea, focusedPlacement.Width * focusedPlacement.Height);
    }

    [Fact]
    public void BuildPlacements_MaxSizeMode_ReturnsSingleFocusedPlacement()
    {
        var service = new ArrangementService();
        var windows = CreateWindows(3);
        var focused = windows[1].Handle;
        var context = new ArrangementContext(TileMode.MaxSize, new WindowBounds(100, 50, 1200, 700), Gap: 0, TopPad: 0, FocusedWindow: focused);

        var placements = service.BuildPlacements(windows, TileMode.MaxSize, context);

        var placement = Assert.Single(placements);
        Assert.Equal(focused, placement.Handle);
        Assert.Equal(100, placement.X);
        Assert.Equal(50, placement.Y);
        Assert.Equal(1200, placement.Width);
        Assert.Equal(700, placement.Height);
    }

    private static IReadOnlyList<TargetWindowInfo> CreateWindows(int count)
    {
        var windows = new List<TargetWindowInfo>(count);
        for (var i = 0; i < count; i++)
        {
            windows.Add(new TargetWindowInfo(
                Handle: (nint)(i + 1),
                Title: $"NoMachine - Window {i + 1}",
                SourceName: $"WoW{i + 1}",
                IsMaximized: false,
                ProcessId: 1000u + (uint)i));
        }

        return windows;
    }
}
