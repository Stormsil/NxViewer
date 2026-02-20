# Advanced Reference

## Table of Contents
1. [Performance & Responsiveness](#performance--responsiveness)
2. [Logging with Serilog](#logging-with-serilog)
3. [User Settings Persistence](#user-settings-persistence)
4. [Validation](#validation)
5. [Value Converters](#value-converters)
6. [Error Handling Patterns](#error-handling-patterns)
7. [Dialogs & Notifications](#dialogs--notifications)
8. [Data Virtualization](#data-virtualization)
9. [Single Instance App](#single-instance-app)
10. [Keyboard Shortcuts](#keyboard-shortcuts)

---

## Performance & Responsiveness

The #1 complaint about desktop apps is "it freezes." Every I/O operation must run
off the UI thread. WPF's threading model is simple: only the UI thread can touch UI elements.
Everything else runs on background threads via `Task.Run` or `async/await`.

### Golden Rules

1. **Never block the UI thread** — no `Thread.Sleep`, no synchronous file I/O, no `.Result` or `.Wait()` on tasks.
2. **Use `[RelayCommand]` for async commands** — they handle `async Task` methods with proper cancellation.
3. **Update UI from the Dispatcher** — if a background task needs to update a collection, marshal to UI thread.
4. **Virtualize large lists** — use `VirtualizingStackPanel` for lists with >50 items.

### Async Command Pattern

```csharp
[ObservableProperty]
private bool _isLoading;

[RelayCommand]
private async Task LoadDataAsync(CancellationToken ct)
{
    IsLoading = true;
    try
    {
        // Heavy work runs on thread pool, UI thread is free
        var data = await Task.Run(() => _service.ProcessLargeDataset(), ct);

        // Update ObservableCollection on UI thread
        Application.Current.Dispatcher.Invoke(() =>
        {
            Items.Clear();
            foreach (var item in data)
                Items.Add(item);
        });
    }
    catch (OperationCanceledException)
    {
        // User navigated away — this is normal, not an error
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to load data");
        _snackbarService.Show("Error", "Failed to load data. Please try again.",
            ControlAppearance.Danger, null, TimeSpan.FromSeconds(5));
    }
    finally
    {
        IsLoading = false;
    }
}
```

### Debounce Search Input

For search-as-you-type scenarios, debounce to avoid querying on every keystroke:

```csharp
private CancellationTokenSource? _searchCts;

partial void OnSearchTextChanged(string value)
{
    _searchCts?.Cancel();
    _searchCts = new CancellationTokenSource();

    _ = SearchWithDebounceAsync(value, _searchCts.Token);
}

private async Task SearchWithDebounceAsync(string query, CancellationToken ct)
{
    try
    {
        await Task.Delay(300, ct); // 300ms debounce
        if (ct.IsCancellationRequested) return;

        var results = await _service.SearchAsync(query, ct);
        Items = new ObservableCollection<Item>(results);
    }
    catch (OperationCanceledException) { }
}
```

### Virtualization for Large Lists

```xml
<ListView
    ItemsSource="{Binding Items}"
    VirtualizingPanel.IsVirtualizing="True"
    VirtualizingPanel.VirtualizationMode="Recycling"
    VirtualizingPanel.ScrollUnit="Pixel"
    ScrollViewer.IsDeferredScrollingEnabled="True">
    <ListView.ItemsPanel>
        <ItemsPanelTemplate>
            <VirtualizingStackPanel />
        </ItemsPanelTemplate>
    </ListView.ItemsPanel>
</ListView>
```

---

## Logging with Serilog

Serilog is configured in `App.xaml.cs` (see architecture.md). Use structured logging throughout.

### Usage Pattern

```csharp
public class ItemService(AppDbContext db, ILogger<ItemService> logger) : IItemService
{
    public async Task<Item> CreateAsync(Item item, CancellationToken ct)
    {
        logger.LogInformation("Creating item {ItemName}", item.Name);

        try
        {
            db.Items.Add(item);
            await db.SaveChangesAsync(ct);
            logger.LogInformation("Created item {ItemId}: {ItemName}", item.Id, item.Name);
            return item;
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database error creating item {ItemName}", item.Name);
            throw;
        }
    }
}
```

### Log Levels Guide

| Level | When to use | Example |
|-------|------------|---------|
| `Debug` | Diagnostic detail, disabled in prod | "Fetching 47 items from cache" |
| `Information` | Normal operations, key business events | "User created item 'Report Q4'" |
| `Warning` | Unexpected but recoverable situations | "Config file missing, using defaults" |
| `Error` | Failures that need attention | "Database connection failed after 3 retries" |
| `Fatal` | App cannot continue | "Unhandled exception in main thread" |

### Structured Logging — Do's and Don'ts

```csharp
// Good — structured, searchable
logger.LogInformation("Item {ItemId} updated by {Action}", item.Id, "rename");

// Bad — string interpolation loses structure
logger.LogInformation($"Item {item.Id} updated by rename");

// Bad — too verbose, not actionable
logger.LogInformation("Entering method GetAllAsync");
```

---

## User Settings Persistence

For user preferences (theme, language, window position), use a JSON file
in `%LocalAppData%`. This is simpler and more flexible than `Settings.settings`.

### Settings Service

```csharp
namespace AppName.Services;

public interface ISettingsService
{
    UserSettings Current { get; }
    Task SaveAsync();
}

public class UserSettings
{
    public string Theme { get; set; } = "Dark";
    public string Language { get; set; } = "en";
    public double WindowWidth { get; set; } = 1200;
    public double WindowHeight { get; set; } = 800;
    public double WindowLeft { get; set; } = double.NaN;
    public double WindowTop { get; set; } = double.NaN;
    public bool IsMaximized { get; set; }
    public bool IsCompactMode { get; set; }
}

public class SettingsService : ISettingsService
{
    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "AppName", "settings.json");

    public UserSettings Current { get; private set; } = new();

    public SettingsService()
    {
        Load();
    }

    private void Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                Current = JsonSerializer.Deserialize<UserSettings>(json) ?? new();
            }
        }
        catch
        {
            Current = new UserSettings();
        }
    }

    public async Task SaveAsync()
    {
        var directory = Path.GetDirectoryName(SettingsPath)!;
        Directory.CreateDirectory(directory);

        var json = JsonSerializer.Serialize(Current, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        await File.WriteAllTextAsync(SettingsPath, json);
    }
}
```

Register as singleton:
```csharp
services.AddSingleton<ISettingsService, SettingsService>();
```

### Restoring Window Position

```csharp
// In MainWindow.xaml.cs
public MainWindow(ISettingsService settings, ...)
{
    InitializeComponent();

    var s = settings.Current;
    Width = s.WindowWidth;
    Height = s.WindowHeight;
    if (!double.IsNaN(s.WindowLeft)) Left = s.WindowLeft;
    if (!double.IsNaN(s.WindowTop)) Top = s.WindowTop;
    if (s.IsMaximized) WindowState = WindowState.Maximized;

    Closing += async (_, _) =>
    {
        if (WindowState == WindowState.Normal)
        {
            s.WindowWidth = Width;
            s.WindowHeight = Height;
            s.WindowLeft = Left;
            s.WindowTop = Top;
        }
        s.IsMaximized = WindowState == WindowState.Maximized;
        await settings.SaveAsync();
    };
}
```

---

## Validation

Use `ObservableValidator` from CommunityToolkit.MVVM with `DataAnnotations` or `FluentValidation`.

### DataAnnotations (simple cases)

```csharp
public partial class EditViewModel : ObservableValidator
{
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Name is required")]
    [MinLength(2, ErrorMessage = "Name must be at least 2 characters")]
    [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    private string _name = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Range(0.01, 999999.99, ErrorMessage = "Price must be between 0.01 and 999,999.99")]
    private decimal _price;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    private string _email = string.Empty;

    [RelayCommand]
    private async Task SaveAsync()
    {
        ValidateAllProperties();
        if (HasErrors) return;

        // proceed with save
    }
}
```

### XAML Error Display

WPF UI controls automatically show validation errors when bound with `ValidatesOnDataErrors`:

```xml
<ui:TextBox
    PlaceholderText="Item name"
    Text="{Binding Name, UpdateSourceTrigger=PropertyChanged, ValidatesOnDataErrors=True}" />
```

The control renders a red border and tooltip with the error message automatically.

### FluentValidation (complex cases)

For rules that depend on multiple fields or external data:

```csharp
public class ItemValidator : AbstractValidator<EditViewModel>
{
    public ItemValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name is too long");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be positive")
            .LessThan(1_000_000).WithMessage("Price is unrealistic");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date must be after start date");
    }
}
```

---

## Value Converters

Common converters every WPF app needs. Place in `Helpers/Converters/`.

### BoolToVisibilityConverter

```csharp
namespace AppName.Helpers.Converters;

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is true ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value is Visibility.Visible;
}

public class InverseBoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is true ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value is Visibility.Collapsed;
}
```

### Register in App.xaml

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ui:ThemesDictionary Theme="Dark" />
            <ui:ControlsDictionary />
        </ResourceDictionary.MergedDictionaries>

        <converters:BoolToVisibilityConverter x:Key="BoolToVisibility" />
        <converters:InverseBoolToVisibilityConverter x:Key="InverseBoolToVisibility" />
    </ResourceDictionary>
</Application.Resources>
```

Usage:
```xml
<Grid Visibility="{Binding IsLoading, Converter={StaticResource BoolToVisibility}}" />
```

### Other Useful Converters

```csharp
// Null to Visibility
public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        => value is not null ? Visibility.Visible : Visibility.Collapsed;
    // ...
}

// DateTime formatting
public class RelativeDateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not DateTime dt) return string.Empty;
        var diff = DateTime.Now - dt;
        return diff.TotalMinutes < 1 ? "Just now"
             : diff.TotalHours < 1 ? $"{(int)diff.TotalMinutes}m ago"
             : diff.TotalDays < 1 ? $"{(int)diff.TotalHours}h ago"
             : diff.TotalDays < 7 ? $"{(int)diff.TotalDays}d ago"
             : dt.ToString("MMM dd, yyyy");
    }
    // ...
}
```

---

## Error Handling Patterns

### Global Exception Handler

Already configured in App.xaml.cs via `DispatcherUnhandledException`. But also handle
task exceptions that bubble up:

```csharp
// In App.xaml.cs constructor or OnStartup:
TaskScheduler.UnobservedTaskException += (_, e) =>
{
    Log.Error(e.Exception, "Unobserved task exception");
    e.SetObserved();
};

AppDomain.CurrentDomain.UnhandledException += (_, e) =>
{
    if (e.ExceptionObject is Exception ex)
        Log.Fatal(ex, "AppDomain unhandled exception");
};
```

### Service-Level Error Handling

Services should throw meaningful exceptions. ViewModels catch and display them:

```csharp
// Service — throw domain-specific exceptions
public class ItemNotFoundException(int id) : Exception($"Item {id} not found");

// ViewModel — catch and show user-friendly messages
[RelayCommand]
private async Task LoadItemAsync(int id)
{
    try
    {
        CurrentItem = await _service.GetByIdAsync(id);
    }
    catch (ItemNotFoundException)
    {
        _snackbarService.Show("Not found", "The requested item no longer exists.",
            ControlAppearance.Caution, null, TimeSpan.FromSeconds(4));
        _navigationService.GoBack();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error loading item {ItemId}", id);
        _snackbarService.Show("Error", "Something went wrong. Please try again.",
            ControlAppearance.Danger, null, TimeSpan.FromSeconds(5));
    }
}
```

---

## Dialogs & Notifications

### ContentDialog for User Decisions

```csharp
// Inject IContentDialogService into ViewModel
private readonly IContentDialogService _dialogService;

[RelayCommand]
private async Task DeleteAsync(Item item)
{
    var result = await _dialogService.ShowSimpleDialogAsync(
        new SimpleContentDialogCreateOptions
        {
            Title = "Delete item",
            Content = $"Are you sure you want to delete \"{item.Name}\"? This cannot be undone.",
            PrimaryButtonText = "Delete",
            CloseButtonText = "Cancel"
        });

    if (result != ContentDialogResult.Primary) return;

    await _service.DeleteAsync(item.Id);
    Items.Remove(item);
    _snackbarService.Show("Deleted", $"\"{item.Name}\" has been removed.",
        ControlAppearance.Info, null, TimeSpan.FromSeconds(3));
}
```

### Snackbar Patterns

```csharp
// Success
_snackbarService.Show("Saved", "Changes saved successfully.",
    ControlAppearance.Success, new SymbolIcon(SymbolRegular.Checkmark24), TimeSpan.FromSeconds(3));

// Warning
_snackbarService.Show("Warning", "Some fields were skipped.",
    ControlAppearance.Caution, new SymbolIcon(SymbolRegular.Warning24), TimeSpan.FromSeconds(4));

// Error
_snackbarService.Show("Error", "Operation failed. Check logs for details.",
    ControlAppearance.Danger, new SymbolIcon(SymbolRegular.DismissCircle24), TimeSpan.FromSeconds(5));
```

---

## Data Virtualization

For very large datasets (10,000+ items), implement incremental loading:

```csharp
// Simplified pagination ViewModel
[ObservableProperty]
private ObservableCollection<Item> _items = [];

private int _currentPage;
private const int PageSize = 50;
private bool _hasMore = true;

[RelayCommand]
private async Task LoadMoreAsync()
{
    if (!_hasMore) return;

    var page = await _service.GetPageAsync(_currentPage, PageSize);
    foreach (var item in page)
        Items.Add(item);

    _hasMore = page.Count == PageSize;
    _currentPage++;
}
```

---

## Single Instance App

Prevent multiple instances of the app from running simultaneously:

```csharp
// In App.xaml.cs
private static readonly Mutex _mutex = new(true, "AppName-SingleInstance-{GUID}");

protected override void OnStartup(StartupEventArgs e)
{
    if (!_mutex.WaitOne(TimeSpan.Zero, true))
    {
        // Another instance is running — activate it and exit
        MessageBox.Show("Application is already running.", "AppName",
            MessageBoxButton.OK, MessageBoxImage.Information);
        Shutdown();
        return;
    }

    // Normal startup continues...
}
```

---

## Keyboard Shortcuts

### InputBindings in XAML

```xml
<Window.InputBindings>
    <KeyBinding Key="N" Modifiers="Ctrl" Command="{Binding NewItemCommand}" />
    <KeyBinding Key="S" Modifiers="Ctrl" Command="{Binding SaveCommand}" />
    <KeyBinding Key="F" Modifiers="Ctrl" Command="{Binding FocusSearchCommand}" />
    <KeyBinding Key="Delete" Command="{Binding DeleteSelectedCommand}" />
    <KeyBinding Key="F5" Command="{Binding RefreshCommand}" />
</Window.InputBindings>
```

### Focus Search Box

```csharp
// Use a Messenger to communicate from ViewModel to View
// when ViewModel doesn't know about specific controls

// ViewModel:
[RelayCommand]
private void FocusSearch()
{
    WeakReferenceMessenger.Default.Send(new FocusSearchMessage());
}

// Message:
public sealed class FocusSearchMessage;

// View code-behind:
public DashboardPage(DashboardViewModel vm)
{
    DataContext = vm;
    InitializeComponent();

    WeakReferenceMessenger.Default.Register<FocusSearchMessage>(this, (_, _) =>
    {
        SearchBox.Focus();
    });
}
```
