using System.Collections.ObjectModel;

namespace NxTiler.App.Collections;

public static class ObservableCollectionSync
{
    public static void Synchronize<T>(ObservableCollection<T> target, IReadOnlyList<T> source)
    {
        var sharedCount = Math.Min(target.Count, source.Count);
        for (var i = 0; i < sharedCount; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(target[i], source[i]))
            {
                target[i] = source[i];
            }
        }

        for (var i = target.Count - 1; i >= source.Count; i--)
        {
            target.RemoveAt(i);
        }

        for (var i = sharedCount; i < source.Count; i++)
        {
            target.Add(source[i]);
        }
    }
}
