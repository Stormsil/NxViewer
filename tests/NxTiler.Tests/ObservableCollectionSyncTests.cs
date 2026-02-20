using System.Collections.ObjectModel;
using NxTiler.App.Collections;

namespace NxTiler.Tests;

public sealed class ObservableCollectionSyncTests
{
    [Fact]
    public void Synchronize_ReplacesItemsAndKeepsCollectionInstance()
    {
        var target = new ObservableCollection<int> { 1, 2, 3 };
        var source = new[] { 1, 4, 3 };

        ObservableCollectionSync.Synchronize(target, source);

        Assert.Equal([1, 4, 3], target);
    }

    [Fact]
    public void Synchronize_AddsAndRemovesItems()
    {
        var target = new ObservableCollection<string> { "a", "b", "c" };
        var source = new[] { "x", "y" };

        ObservableCollectionSync.Synchronize(target, source);

        Assert.Equal(["x", "y"], target);
    }
}
