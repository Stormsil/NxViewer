namespace NxTiler.App.Resources.Theme;

public sealed class WpfResourceProvider : IResourceProvider
{
    public bool TryGetResource(string key, out object? value)
    {
        value = System.Windows.Application.Current?.TryFindResource(key);
        return value is not null;
    }
}
