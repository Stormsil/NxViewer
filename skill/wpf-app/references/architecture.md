# Architecture Reference

## Table of Contents
1. [Project Structure](#project-structure)
2. [Application Host & DI](#application-host--di)
3. [MVVM with CommunityToolkit](#mvvm-with-communitytoolkit)
4. [Navigation](#navigation)
5. [Services Layer](#services-layer)
6. [Configuration](#configuration)

---

## Project Structure

```
AppName/
│
├── AppName.csproj                  — Project file with package refs
├── App.xaml                        — Theme resources, merged dictionaries
├── App.xaml.cs                     — Host builder, DI registration, startup
├── MainWindow.xaml                 — App shell: FluentWindow + NavigationView
├── MainWindow.xaml.cs              — Minimal code-behind, DI wired
├── appsettings.json                — App configuration (connection strings, etc.)
│
├── Models/                         — Plain data classes
│   ├── AppConfig.cs                — Typed configuration model
│   └── [DomainModel].cs            — Business entities
│
├── ViewModels/                     — One per page/dialog
│   ├── MainWindowViewModel.cs      — Shell ViewModel (navigation state)
│   ├── DashboardViewModel.cs       — Example page VM
│   └── SettingsViewModel.cs        — Settings page VM
│
├── Views/                          — XAML pages
│   └── Pages/
│       ├── DashboardPage.xaml      — Bound to DashboardViewModel
│       └── SettingsPage.xaml       — Bound to SettingsViewModel
│
├── Services/                       — Business logic & data access
│   ├── Interfaces/
│   │   └── I[ServiceName].cs       — Service contracts
│   └── [ServiceName].cs            — Implementations
│
├── Data/                           — Database layer (if needed)
│   ├── AppDbContext.cs             — EF Core DbContext
│   ├── Migrations/                 — EF migrations
│   └── Repositories/              — Repository implementations
│
├── Helpers/
│   ├── Converters/                 — IValueConverter implementations
│   │   └── BoolToVisibilityConverter.cs
│   └── Extensions/                 — Extension methods
│
└── Assets/
    ├── app-icon.ico               — App icon
    └── Images/                    — Static images
```

### .csproj Template

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <UseWPF>true</UseWPF>
    <ApplicationIcon>Assets\app-icon.ico</ApplicationIcon>
    <AssemblyTitle>AppName</AssemblyTitle>
    <Description>Description of your app</Description>
    <Version>1.0.0</Version>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Wpf.Ui" Version="4.*" />
    <PackageReference Include="CommunityToolkit.Mvvm" Version="8.*" />
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="8.*" />
    <PackageReference Include="Serilog.Extensions.Hosting" Version="8.*" />
    <PackageReference Include="Serilog.Sinks.File" Version="6.*" />
    <PackageReference Include="Serilog.Sinks.Debug" Version="3.*" />
  </ItemGroup>

  <ItemGroup>
    <None Update="appsettings.json">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
  </ItemGroup>

</Project>
```

---

## Application Host & DI

Use `Microsoft.Extensions.Hosting` to manage the application lifecycle and DI.
This is the backbone of the app — everything is wired here.

### App.xaml

```xml
<Application
    x:Class="AppName.App"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:ui="http://schemas.lepo.co/wpfui/2022/xaml"
    DispatcherUnhandledException="OnDispatcherUnhandledException">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ui:ThemesDictionary Theme="Dark" />
                <ui:ControlsDictionary />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

### App.xaml.cs — Full Pattern

```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Wpf.Ui;

namespace AppName;

public partial class App : Application
{
    private static readonly IHost _host = Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration((context, config) =>
        {
            config.SetBasePath(AppContext.BaseDirectory);
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        })
        .UseSerilog((context, services, loggerConfig) =>
        {
            var logPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "AppName", "Logs", "log-.txt");

            loggerConfig
                .ReadFrom.Configuration(context.Configuration)
                .WriteTo.Debug()
                .WriteTo.File(logPath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
        })
        .ConfigureServices((context, services) =>
        {
            // WPF UI services
            services.AddSingleton<IThemeService, ThemeService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ISnackbarService, SnackbarService>();
            services.AddSingleton<IContentDialogService, ContentDialogService>();

            // Main window
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();

            // Pages — register as transient so each navigation creates a fresh instance
            services.AddTransient<Views.Pages.DashboardPage>();
            services.AddTransient<ViewModels.DashboardViewModel>();
            services.AddTransient<Views.Pages.SettingsPage>();
            services.AddTransient<ViewModels.SettingsViewModel>();

            // Application services
            // services.AddSingleton<IYourService, YourService>();

            // Configuration
            // services.Configure<AppConfig>(context.Configuration.GetSection("AppConfig"));
        })
        .Build();

    public static IServiceProvider Services => _host.Services;

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();

        var mainWindow = Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();
        Log.CloseAndFlush();

        base.OnExit(e);
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        Log.Fatal(e.Exception, "Unhandled exception");
        e.Handled = true;
    }
}
```

The pattern above does several things right:
- Serilog writes to `%LocalAppData%/AppName/Logs/` — a standard location for Windows apps
- Unhandled exceptions are caught and logged instead of crashing silently
- The host manages startup/shutdown lifecycle cleanly
- All dependencies are registered in one place

---

## MVVM with CommunityToolkit

CommunityToolkit.MVVM uses Roslyn source generators to eliminate boilerplate.
This means you write partial classes with attributes, and the compiler generates the rest.

### ViewModel Base Pattern

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppName.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly ILogger<DashboardViewModel> _logger;
    private readonly IYourService _service;

    // Source generator creates a public property "Title" with change notification
    [ObservableProperty]
    private string _title = "Dashboard";

    // Source generator creates "IsLoading" property
    [ObservableProperty]
    private bool _isLoading;

    // Source generator creates "Items" property
    // Also notifies "HasItems" when Items changes
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasItems))]
    private ObservableCollection<ItemModel> _items = [];

    public bool HasItems => Items.Count > 0;

    public DashboardViewModel(ILogger<DashboardViewModel> logger, IYourService service)
    {
        _logger = logger;
        _service = service;
    }

    // Source generator creates an IAsyncRelayCommand "LoadDataCommand"
    // The command is auto-disabled while executing (prevents double-clicks)
    [RelayCommand]
    private async Task LoadDataAsync(CancellationToken ct)
    {
        try
        {
            IsLoading = true;
            var data = await _service.GetItemsAsync(ct);
            Items = new ObservableCollection<ItemModel>(data);
        }
        catch (OperationCanceledException)
        {
            // Navigation away cancelled the operation — this is fine
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load dashboard data");
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Source generator creates "DeleteItemCommand"
    // CanExecute prevents calling when no item is selected
    [RelayCommand(CanExecute = nameof(CanDeleteItem))]
    private async Task DeleteItemAsync(ItemModel item)
    {
        await _service.DeleteAsync(item.Id);
        Items.Remove(item);
    }

    private bool CanDeleteItem(ItemModel? item) => item is not null;
}
```

### Key Rules for ViewModels

1. **Inherit from `ObservableObject`** (or `ObservableRecipient` if you need messaging).
2. **Class must be `partial`** — source generators add code to the other partial.
3. **Private fields with `[ObservableProperty]`** — the generator creates public properties. Field `_myValue` becomes property `MyValue`.
4. **Methods with `[RelayCommand]`** — the generator creates `IRelayCommand MyMethodCommand`. Async methods get `IAsyncRelayCommand` with built-in cancellation support.
5. **Use `[NotifyPropertyChangedFor]`** to trigger dependent property updates.
6. **Use `[NotifyCanExecuteChangedFor]`** to re-evaluate command CanExecute when a property changes.

### ViewModel for Forms with Validation

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AppName.ViewModels;

// ObservableValidator adds INotifyDataErrorInfo support
public partial class EditItemViewModel : ObservableValidator
{
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Name is required")]
    [MinLength(3, ErrorMessage = "Name must be at least 3 characters")]
    private string _name = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Range(0, 10000, ErrorMessage = "Value must be between 0 and 10000")]
    private decimal _price;

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        ValidateAllProperties();
        if (HasErrors) return;

        // ... save logic
    }

    private bool CanSave() => !HasErrors;
}
```

---

## Navigation

WPF UI provides a built-in navigation system with `NavigationView` and `INavigationService`.

### MainWindow.xaml — Navigation Shell

```xml
<ui:FluentWindow
    x:Class="AppName.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:ui="http://schemas.lepo.co/wpfui/2022/xaml"
    xmlns:pages="clr-namespace:AppName.Views.Pages"
    Title="AppName"
    Width="1200" Height="800"
    MinWidth="800" MinHeight="600"
    WindowStartupLocation="CenterScreen"
    ExtendsContentIntoTitleBar="True">

    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="*" />
        </Grid.RowDefinitions>

        <ui:TitleBar Grid.Row="0" Title="AppName" ShowMaximize="True" ShowMinimize="True" />

        <ui:NavigationView
            x:Name="NavigationView"
            Grid.Row="1"
            IsBackButtonVisible="Visible"
            IsPaneToggleVisible="True"
            PaneDisplayMode="Left"
            OpenPaneLength="250">

            <ui:NavigationView.MenuItems>
                <ui:NavigationViewItem
                    Content="Dashboard"
                    Icon="{ui:SymbolIcon Home24}"
                    TargetPageType="{x:Type pages:DashboardPage}" />
                <ui:NavigationViewItem
                    Content="Items"
                    Icon="{ui:SymbolIcon List24}"
                    TargetPageType="{x:Type pages:ItemsPage}" />
            </ui:NavigationView.MenuItems>

            <ui:NavigationView.FooterMenuItems>
                <ui:NavigationViewItem
                    Content="Settings"
                    Icon="{ui:SymbolIcon Settings24}"
                    TargetPageType="{x:Type pages:SettingsPage}" />
            </ui:NavigationView.FooterMenuItems>

        </ui:NavigationView>
    </Grid>
</ui:FluentWindow>
```

### MainWindow.xaml.cs

```csharp
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace AppName;

public partial class MainWindow : FluentWindow
{
    public MainWindow(
        MainWindowViewModel viewModel,
        INavigationService navigationService,
        ISnackbarService snackbarService,
        IContentDialogService contentDialogService,
        IServiceProvider serviceProvider)
    {
        DataContext = viewModel;
        InitializeComponent();

        // Wire up WPF UI services to the NavigationView
        navigationService.SetNavigationControl(NavigationView);
        snackbarService.SetSnackbarPresenter(/* add SnackbarPresenter to XAML */);
        contentDialogService.SetDialogHost(/* add ContentPresenter to XAML */);

        // Set the page service for navigation resolution via DI
        NavigationView.SetServiceProvider(serviceProvider);
    }
}
```

### Page Code-Behind Pattern

Every page follows this exact pattern — nothing more:

```csharp
using Wpf.Ui.Controls;

namespace AppName.Views.Pages;

public partial class DashboardPage : Page
{
    public DashboardPage(DashboardViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
```

The ViewModel is injected via DI. The page sets it as DataContext. That's it.
All logic lives in the ViewModel.

---

## Services Layer

Services encapsulate business logic and data access. They are always behind interfaces.

### Pattern

```csharp
// Services/Interfaces/IItemService.cs
namespace AppName.Services.Interfaces;

public interface IItemService
{
    Task<IReadOnlyList<Item>> GetAllAsync(CancellationToken ct = default);
    Task<Item?> GetByIdAsync(int id, CancellationToken ct = default);
    Task CreateAsync(Item item, CancellationToken ct = default);
    Task UpdateAsync(Item item, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

// Services/ItemService.cs
namespace AppName.Services;

public class ItemService(AppDbContext db, ILogger<ItemService> logger) : IItemService
{
    public async Task<IReadOnlyList<Item>> GetAllAsync(CancellationToken ct = default)
    {
        logger.LogDebug("Fetching all items");
        return await db.Items.AsNoTracking().ToListAsync(ct);
    }

    // ... other methods follow the same pattern
}
```

Register in DI:
```csharp
services.AddScoped<IItemService, ItemService>();
```

---

## Configuration

### appsettings.json

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    }
  },
  "AppConfig": {
    "DatabasePath": "",
    "Theme": "Dark",
    "Language": "en"
  }
}
```

### Typed Configuration

```csharp
namespace AppName.Models;

public sealed class AppConfig
{
    public string DatabasePath { get; set; } = string.Empty;
    public string Theme { get; set; } = "Dark";
    public string Language { get; set; } = "en";
}
```

Register:
```csharp
services.Configure<AppConfig>(context.Configuration.GetSection("AppConfig"));
```

Inject:
```csharp
public class SettingsViewModel(IOptions<AppConfig> config) : ObservableObject
{
    private readonly AppConfig _config = config.Value;
}
```
