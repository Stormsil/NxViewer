using Wpf.Ui.Abstractions;

namespace NxTiler.App.Services;

public sealed class ServiceProviderNavigationViewPageProvider(IServiceProvider serviceProvider)
    : INavigationViewPageProvider
{
    public object? GetPage(Type pageType)
    {
        ArgumentNullException.ThrowIfNull(pageType);
        return serviceProvider.GetService(pageType);
    }
}
