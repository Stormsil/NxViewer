using Microsoft.Extensions.Hosting;

namespace NxTiler.App;

public partial class App : System.Windows.Application
{
    private static readonly IHost Host = CreateHost();

    public static IServiceProvider Services => Host.Services;
}
