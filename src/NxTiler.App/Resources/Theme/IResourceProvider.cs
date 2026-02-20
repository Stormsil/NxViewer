namespace NxTiler.App.Resources.Theme;

public interface IResourceProvider
{
    bool TryGetResource(string key, out object? value);
}
