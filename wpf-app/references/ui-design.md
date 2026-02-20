# UI Design Reference

## Table of Contents
1. [Design Philosophy](#design-philosophy)
2. [WPF UI Controls](#wpf-ui-controls)
3. [Theming](#theming)
4. [Layout Patterns](#layout-patterns)
5. [Responsive Design](#responsive-design)
6. [Common UI Patterns](#common-ui-patterns)
7. [Icons](#icons)
8. [Typography & Spacing](#typography--spacing)

---

## Design Philosophy

The goal is a **strict, professional, modern** Windows desktop application that looks like it belongs
in the Windows 11 ecosystem. Think: Windows Settings, Microsoft Store, DevHome — clean, spacious,
functional, no decoration for decoration's sake.

Principles:
- **Content over chrome** — minimize visual noise, let data breathe
- **Consistency** — same spacing, same control usage patterns throughout
- **Depth through elevation** — use Cards and subtle shadows instead of borders and lines
- **Motion with purpose** — transitions guide attention, they don't entertain
- **Dark-first** — design in dark mode, verify in light mode. Most professional users prefer dark.

What makes an app look like AI slop:
- Random gradient backgrounds
- Excessive rounded corners with thick colored borders
- Inconsistent spacing (8px here, 13px there, 20px somewhere else)
- Every section has a different visual treatment
- Animations on everything
- Using stock/emoji icons mixed with system icons

---

## WPF UI Controls

WPF UI provides Fluent Design replacements for standard WPF controls.
**Always** use the `ui:` namespace variants. They inherit Fluent styling automatically.

XAML namespace declaration:
```xml
xmlns:ui="http://schemas.lepo.co/wpfui/2022/xaml"
```

### Essential Controls

#### Windows & Navigation

| Control | Usage |
|---------|-------|
| `ui:FluentWindow` | **Always** use instead of `Window`. Provides Mica/Acrylic backdrop, proper title bar |
| `ui:TitleBar` | Custom title bar with min/max/close. Set `ExtendsContentIntoTitleBar="True"` on window |
| `ui:NavigationView` | Sidebar navigation. Primary navigation for multi-page apps |
| `ui:BreadcrumbBar` | Hierarchical navigation for deep content structures |

#### Input Controls

| Control | Usage | Notes |
|---------|-------|-------|
| `ui:TextBox` | Text input | Supports `PlaceholderText`, `Icon` |
| `ui:PasswordBox` | Password input | Fluent styled, has reveal button |
| `ui:NumberBox` | Numeric input | Built-in increment/decrement, validation |
| `ui:AutoSuggestBox` | Search with suggestions | Bind `ItemsSource` for suggestions |
| `ui:ToggleSwitch` | Boolean setting | Prefer over CheckBox for settings |
| `ui:RatingControl` | Star rating | For user feedback scenarios |
| `ui:CalendarDatePicker`| Date selection | |

#### Layout & Containers

| Control | Usage |
|---------|-------|
| `ui:Card` | Group related content. The primary container element. |
| `ui:CardExpander` | Collapsible card for secondary content |
| `ui:InfoBar` | Status messages (info, success, warning, error) |
| `ui:Flyout` | Lightweight popup attached to a control |

#### Actions

| Control | Usage | Notes |
|---------|-------|-------|
| `ui:Button` | Primary actions | Set `Appearance="Primary"` for primary actions |
| `ui:HyperlinkButton` | Inline links | |
| `ui:DropDownButton` | Button with menu | |
| `ui:SplitButton` | Default action + menu | |

#### Feedback

| Control | Usage |
|---------|-------|
| `ui:ProgressRing` | Indeterminate loading |
| `ui:ProgressBar` | Determinate progress |
| `ui:SnackbarPresenter` | Toast notifications at bottom |
| `ui:ContentDialog` | Modal dialogs (confirm, input) |

### Control Usage Examples

#### Card Layout with Actions

```xml
<ui:Card Margin="0,0,0,8">
    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="*" />
            <ColumnDefinition Width="Auto" />
        </Grid.ColumnDefinitions>

        <StackPanel Grid.Column="0" VerticalAlignment="Center">
            <TextBlock
                Text="{Binding Name}"
                FontWeight="SemiBold"
                Style="{StaticResource BodyStrongTextBlockStyle}" />
            <TextBlock
                Text="{Binding Description}"
                Foreground="{DynamicResource TextFillColorSecondaryBrush}"
                Style="{StaticResource CaptionTextBlockStyle}" />
        </StackPanel>

        <StackPanel Grid.Column="1" Orientation="Horizontal" Spacing="8">
            <ui:Button
                Content="Edit"
                Icon="{ui:SymbolIcon Edit24}"
                Command="{Binding DataContext.EditCommand, RelativeSource={RelativeSource AncestorType=Page}}"
                CommandParameter="{Binding}" />
            <ui:Button
                Content="Delete"
                Appearance="Danger"
                Icon="{ui:SymbolIcon Delete24}"
                Command="{Binding DataContext.DeleteCommand, RelativeSource={RelativeSource AncestorType=Page}}"
                CommandParameter="{Binding}" />
        </StackPanel>
    </Grid>
</ui:Card>
```

#### Form Layout

```xml
<StackPanel Spacing="16" MaxWidth="600">
    <ui:TextBox
        PlaceholderText="Enter item name"
        Text="{Binding Name, UpdateSourceTrigger=PropertyChanged, ValidatesOnDataErrors=True}"
        Icon="{ui:SymbolIcon TextDescription24}" />

    <ui:NumberBox
        PlaceholderText="Price"
        Value="{Binding Price, UpdateSourceTrigger=PropertyChanged}"
        Minimum="0"
        Maximum="100000"
        SmallChange="1"
        LargeChange="100" />

    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="*" />
            <ColumnDefinition Width="*" />
        </Grid.ColumnDefinitions>

        <ui:Button
            Grid.Column="0"
            Content="Cancel"
            HorizontalAlignment="Stretch"
            Margin="0,0,4,0"
            Command="{Binding CancelCommand}" />
        <ui:Button
            Grid.Column="1"
            Content="Save"
            Appearance="Primary"
            HorizontalAlignment="Stretch"
            Margin="4,0,0,0"
            Command="{Binding SaveCommand}" />
    </Grid>
</StackPanel>
```

#### Loading State Pattern

```xml
<Grid>
    <!-- Content -->
    <ScrollViewer Visibility="{Binding IsLoading, Converter={StaticResource InverseBoolToVisibility}}">
        <ItemsControl ItemsSource="{Binding Items}">
            <!-- ... item template ... -->
        </ItemsControl>
    </ScrollViewer>

    <!-- Loading overlay -->
    <Grid Visibility="{Binding IsLoading, Converter={StaticResource BoolToVisibility}}">
        <StackPanel HorizontalAlignment="Center" VerticalAlignment="Center">
            <ui:ProgressRing IsIndeterminate="True" Width="48" Height="48" />
            <TextBlock
                Text="Loading..."
                Margin="0,12,0,0"
                HorizontalAlignment="Center"
                Foreground="{DynamicResource TextFillColorSecondaryBrush}" />
        </StackPanel>
    </Grid>

    <!-- Empty state -->
    <StackPanel
        HorizontalAlignment="Center"
        VerticalAlignment="Center"
        Visibility="{Binding HasNoItems, Converter={StaticResource BoolToVisibility}}">
        <ui:SymbolIcon Symbol="Inbox24" FontSize="48"
            Foreground="{DynamicResource TextFillColorTertiaryBrush}" />
        <TextBlock
            Text="No items yet"
            Margin="0,12,0,4"
            HorizontalAlignment="Center"
            Style="{StaticResource SubtitleTextBlockStyle}" />
        <TextBlock
            Text="Add your first item to get started"
            HorizontalAlignment="Center"
            Foreground="{DynamicResource TextFillColorSecondaryBrush}" />
        <ui:Button
            Content="Add Item"
            Appearance="Primary"
            Margin="0,16,0,0"
            HorizontalAlignment="Center"
            Command="{Binding AddItemCommand}" />
    </StackPanel>
</Grid>
```

---

## Theming

### Theme Resources

WPF UI provides dynamic resources that automatically switch between light and dark themes.

#### Text Colors
```xml
<!-- Primary text (headings, important content) -->
Foreground="{DynamicResource TextFillColorPrimaryBrush}"

<!-- Secondary text (descriptions, labels) -->
Foreground="{DynamicResource TextFillColorSecondaryBrush}"

<!-- Tertiary text (disabled, placeholder) -->
Foreground="{DynamicResource TextFillColorTertiaryBrush}"
```

#### Backgrounds & Surfaces
```xml
<!-- App background (use on the main window content area) -->
Background="{DynamicResource ApplicationBackgroundBrush}"

<!-- Card/elevated surfaces -->
Background="{DynamicResource CardBackgroundFillColorDefaultBrush}"

<!-- Subtle backgrounds (hover states, alternating rows) -->
Background="{DynamicResource SubtleFillColorSecondaryBrush}"
```

#### Accent Colors
```xml
<!-- Primary accent (buttons, active elements) -->
Background="{DynamicResource SystemAccentColorPrimaryBrush}"
Foreground="{DynamicResource AccentTextFillColorPrimaryBrush}"
```

#### Borders & Dividers
```xml
<!-- Standard border -->
BorderBrush="{DynamicResource CardStrokeColorDefaultBrush}"

<!-- Divider line -->
BorderBrush="{DynamicResource DividerStrokeColorDefaultBrush}"
```

### Theme Switching in Code

```csharp
using Wpf.Ui;
using Wpf.Ui.Appearance;

// In SettingsViewModel or wherever theme toggle lives:
[RelayCommand]
private void ToggleTheme()
{
    var currentTheme = ApplicationThemeManager.GetAppTheme();
    var newTheme = currentTheme == ApplicationTheme.Dark
        ? ApplicationTheme.Light
        : ApplicationTheme.Dark;
    ApplicationThemeManager.Apply(newTheme);
}
```

### Mica / Acrylic Backdrop

```xml
<ui:FluentWindow
    WindowBackdropType="Mica">
    <!-- Mica backdrop requires Windows 11. Falls back gracefully on Win10. -->
    <!-- Options: None, Mica, Acrylic, Tabbed -->
</ui:FluentWindow>
```

---

## Layout Patterns

### Page Layout Template

Every page follows this structure:

```xml
<Page
    x:Class="AppName.Views.Pages.DashboardPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:ui="http://schemas.lepo.co/wpfui/2022/xaml"
    Margin="0">

    <Grid Margin="24,16,24,24">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />   <!-- Header -->
            <RowDefinition Height="*" />       <!-- Content -->
        </Grid.RowDefinitions>

        <!-- Page header -->
        <StackPanel Grid.Row="0" Margin="0,0,0,24">
            <TextBlock
                Text="Dashboard"
                Style="{StaticResource TitleLargeTextBlockStyle}" />
            <TextBlock
                Text="Overview of your workspace"
                Foreground="{DynamicResource TextFillColorSecondaryBrush}"
                Style="{StaticResource BodyTextBlockStyle}"
                Margin="0,4,0,0" />
        </StackPanel>

        <!-- Page content -->
        <ScrollViewer Grid.Row="1" VerticalScrollBarVisibility="Auto">
            <StackPanel Spacing="8">
                <!-- Content goes here -->
            </StackPanel>
        </ScrollViewer>
    </Grid>
</Page>
```

### Spacing System

Use consistent spacing based on a 4px grid:

| Token | Value | Usage |
|-------|-------|-------|
| `xs` | `4` | Between inline elements, icon-to-text |
| `sm` | `8` | Between cards in a list, between form fields |
| `md` | `16` | Between sections within a page |
| `lg` | `24` | Page padding, between major sections |
| `xl` | `32` | Between page header and content |

Apply through `Margin` and `Spacing`:
```xml
<StackPanel Spacing="8" Margin="24,16,24,24">
```

---

## Responsive Design

WPF doesn't have CSS breakpoints, but you can achieve responsiveness with:

### Adaptive Columns with UniformGrid

```xml
<UniformGrid Columns="3" Margin="0">
    <ui:Card Margin="4"><TextBlock Text="Card 1" /></ui:Card>
    <ui:Card Margin="4"><TextBlock Text="Card 2" /></ui:Card>
    <ui:Card Margin="4"><TextBlock Text="Card 3" /></ui:Card>
</UniformGrid>
```

### WrapPanel for Flow Layout

```xml
<WrapPanel>
    <ui:Card Width="300" Margin="4"><!-- content --></ui:Card>
    <ui:Card Width="300" Margin="4"><!-- content --></ui:Card>
    <ui:Card Width="300" Margin="4"><!-- content --></ui:Card>
</WrapPanel>
```

### DataTrigger for Adaptive Layout

```xml
<Grid>
    <Grid.Style>
        <Style TargetType="Grid">
            <Setter Property="Visibility" Value="Visible" />
            <Style.Triggers>
                <DataTrigger Binding="{Binding ActualWidth, RelativeSource={RelativeSource AncestorType=Window}, 
                    Converter={StaticResource LessThanConverter}, ConverterParameter=800}" Value="True">
                    <!-- Switch to single-column layout when window is narrow -->
                </DataTrigger>
            </Style.Triggers>
        </Style>
    </Grid.Style>
</Grid>
```

### MinWidth / MaxWidth for Content

```xml
<!-- Keep forms readable at any window size -->
<StackPanel MaxWidth="600" HorizontalAlignment="Left">
    <!-- Form controls -->
</StackPanel>

<!-- Set minimum window size in code -->
<!-- MinWidth="800" MinHeight="600" on FluentWindow -->
```

---

## Common UI Patterns

### Settings Page

```xml
<ScrollViewer VerticalScrollBarVisibility="Auto">
    <StackPanel MaxWidth="700" Spacing="4">

        <!-- Section: Appearance -->
        <TextBlock
            Text="Appearance"
            Style="{StaticResource BodyStrongTextBlockStyle}"
            Margin="0,0,0,8" />

        <ui:Card>
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="*" />
                    <ColumnDefinition Width="Auto" />
                </Grid.ColumnDefinitions>
                <StackPanel>
                    <TextBlock Text="App Theme" FontWeight="SemiBold" />
                    <TextBlock
                        Text="Select which theme to display"
                        Foreground="{DynamicResource TextFillColorSecondaryBrush}"
                        Style="{StaticResource CaptionTextBlockStyle}" />
                </StackPanel>
                <ComboBox Grid.Column="1" Width="150"
                    SelectedItem="{Binding SelectedTheme}"
                    ItemsSource="{Binding AvailableThemes}" />
            </Grid>
        </ui:Card>

        <ui:Card>
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="*" />
                    <ColumnDefinition Width="Auto" />
                </Grid.ColumnDefinitions>
                <StackPanel>
                    <TextBlock Text="Compact mode" FontWeight="SemiBold" />
                    <TextBlock
                        Text="Reduce spacing between elements"
                        Foreground="{DynamicResource TextFillColorSecondaryBrush}"
                        Style="{StaticResource CaptionTextBlockStyle}" />
                </StackPanel>
                <ui:ToggleSwitch Grid.Column="1"
                    IsChecked="{Binding IsCompactMode}" />
            </Grid>
        </ui:Card>

        <!-- Section: About -->
        <TextBlock
            Text="About"
            Style="{StaticResource BodyStrongTextBlockStyle}"
            Margin="0,24,0,8" />

        <ui:Card>
            <StackPanel Spacing="4">
                <TextBlock Text="AppName" FontWeight="SemiBold" />
                <TextBlock
                    Text="{Binding AppVersion, StringFormat='Version {0}'}"
                    Foreground="{DynamicResource TextFillColorSecondaryBrush}" />
            </StackPanel>
        </ui:Card>
    </StackPanel>
</ScrollViewer>
```

### Confirmation Dialog

```csharp
// In ViewModel — use IContentDialogService
var result = await _contentDialogService.ShowSimpleDialogAsync(
    new SimpleContentDialogCreateOptions
    {
        Title = "Delete item?",
        Content = "This action cannot be undone.",
        PrimaryButtonText = "Delete",
        CloseButtonText = "Cancel"
    });

if (result == ContentDialogResult.Primary)
{
    await DeleteItemAsync(item);
}
```

### Snackbar Notification

```csharp
// In ViewModel — use ISnackbarService
_snackbarService.Show(
    "Item saved",
    "Your changes have been saved successfully.",
    ControlAppearance.Success,
    new SymbolIcon(SymbolRegular.Checkmark24),
    TimeSpan.FromSeconds(3));
```

---

## Icons

WPF UI includes Fluent System Icons. Use `SymbolRegular` enum for icon references.

```xml
<!-- In XAML -->
<ui:SymbolIcon Symbol="Home24" />
<ui:Button Icon="{ui:SymbolIcon Save24}" Content="Save" />

<!-- Common icons -->
<!-- Home24, Settings24, Add24, Delete24, Edit24, Save24, Search24,
     ArrowLeft24, ArrowRight24, Checkmark24, Dismiss24, Warning24,
     Info24, Person24, Folder24, Document24, Database24, Filter24,
     ArrowSync24, Eye24, EyeOff24, Copy24, Share24 -->
```

Browse all available icons: search "Fluent System Icons" for the full catalog.

---

## Typography & Spacing

### Text Styles

WPF UI provides predefined text styles that match Fluent Design:

```xml
<!-- Large page title -->
<TextBlock Style="{StaticResource TitleLargeTextBlockStyle}" />

<!-- Section title -->
<TextBlock Style="{StaticResource TitleTextBlockStyle}" />

<!-- Subsection title -->
<TextBlock Style="{StaticResource SubtitleTextBlockStyle}" />

<!-- Strong body text (labels, card titles) -->
<TextBlock Style="{StaticResource BodyStrongTextBlockStyle}" />

<!-- Normal body text -->
<TextBlock Style="{StaticResource BodyTextBlockStyle}" />

<!-- Small text (descriptions, timestamps) -->
<TextBlock Style="{StaticResource CaptionTextBlockStyle}" />
```

Use these consistently. Don't set `FontSize` manually unless you have a very specific reason.
The styles handle size, weight, and line height to match the Fluent Design type ramp.
