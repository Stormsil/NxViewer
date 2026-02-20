# Cross-Platform: Porting Ant Design to WPF/XAML & Desktop Applications

This document bridges Ant Design's web-oriented design system to WPF desktop applications
and other non-web platforms. The design PRINCIPLES are universal — only the implementation
technology changes.

---

## Philosophy: Design System ≠ Component Library

Ant Design is NOT just "a set of React components." It is a design language — a system of
values, principles, spacing rules, color systems, typography scales, and interaction patterns.
These translate directly to ANY platform:

| Ant Design Concept | Web Implementation | WPF/XAML Implementation |
|-------------------|-------------------|------------------------|
| 8px base grid spacing | CSS margin/padding | XAML Margin/Padding |
| 24-column grid | antd Grid Row/Col | Grid with ColumnDefinitions |
| Brand color #1677FF | CSS variables | SolidColorBrush in ResourceDictionary |
| 14px base font | CSS font-size | FontSize="14" |
| Card component | antd Card | Border + StackPanel with CardStyle |
| Modal dialog | antd Modal | WPF UI Dialog / ContentDialog |
| Notification toast | antd notification | Snackbar / InfoBar (WPF UI) |
| Sidebar navigation | antd Menu (vertical) | NavigationView (WPF UI) |

---

## Color System in XAML

### Resource Dictionary Setup

Define your Ant Design color palette as XAML resources:

```xml
<!-- AntDesignColors.xaml -->
<ResourceDictionary>
  <!-- Brand Colors -->
  <Color x:Key="ColorPrimary">#1677FF</Color>
  <Color x:Key="ColorPrimaryHover">#4096FF</Color>
  <Color x:Key="ColorPrimaryActive">#0958D9</Color>
  <Color x:Key="ColorPrimaryBg">#E6F4FF</Color>
  
  <!-- Functional Colors -->
  <Color x:Key="ColorSuccess">#52C41A</Color>
  <Color x:Key="ColorWarning">#FAAD14</Color>
  <Color x:Key="ColorError">#FF4D4F</Color>
  <Color x:Key="ColorInfo">#1677FF</Color>
  
  <!-- Neutral Colors -->
  <Color x:Key="ColorText">#000000E0</Color>
  <Color x:Key="ColorTextSecondary">#000000A6</Color>
  <Color x:Key="ColorTextTertiary">#00000073</Color>
  <Color x:Key="ColorTextDisabled">#00000040</Color>
  <Color x:Key="ColorBorder">#D9D9D9</Color>
  <Color x:Key="ColorBorderSecondary">#F0F0F0</Color>
  <Color x:Key="ColorBgContainer">#FFFFFF</Color>
  <Color x:Key="ColorBgLayout">#F5F5F5</Color>
  <Color x:Key="ColorBgElevated">#FFFFFF</Color>
  
  <!-- Brushes -->
  <SolidColorBrush x:Key="PrimaryBrush" Color="{StaticResource ColorPrimary}"/>
  <SolidColorBrush x:Key="SuccessBrush" Color="{StaticResource ColorSuccess}"/>
  <SolidColorBrush x:Key="WarningBrush" Color="{StaticResource ColorWarning}"/>
  <SolidColorBrush x:Key="ErrorBrush" Color="{StaticResource ColorError}"/>
  <SolidColorBrush x:Key="TextBrush" Color="{StaticResource ColorText}"/>
  <SolidColorBrush x:Key="TextSecondaryBrush" Color="{StaticResource ColorTextSecondary}"/>
  <SolidColorBrush x:Key="BorderBrush" Color="{StaticResource ColorBorder}"/>
  <SolidColorBrush x:Key="BgLayoutBrush" Color="{StaticResource ColorBgLayout}"/>
</ResourceDictionary>
```

### Dark Mode in XAML

WPF UI (Wpf.Ui) supports theme switching natively:
```csharp
ApplicationThemeManager.Apply(ApplicationTheme.Dark);
```

For your custom Ant Design colors, provide a dark theme dictionary:
```xml
<!-- AntDesignColors.Dark.xaml -->
<ResourceDictionary>
  <Color x:Key="ColorText">#FFFFFFD9</Color>
  <Color x:Key="ColorTextSecondary">#FFFFFFA6</Color>
  <Color x:Key="ColorBgContainer">#141414</Color>
  <Color x:Key="ColorBgLayout">#000000</Color>
  <Color x:Key="ColorBgElevated">#1F1F1F</Color>
  <Color x:Key="ColorBorder">#424242</Color>
  <!-- Primary colors slightly desaturated for dark mode -->
  <Color x:Key="ColorPrimary">#1668DC</Color>
</ResourceDictionary>
```

---

## Typography in XAML

### Font Family

```xml
<Style TargetType="TextBlock">
  <Setter Property="FontFamily" Value="Segoe UI, Microsoft YaHei, sans-serif"/>
  <Setter Property="FontSize" Value="14"/>
  <Setter Property="LineHeight" Value="22"/>
  <Setter Property="Foreground" Value="{StaticResource TextBrush}"/>
</Style>
```

### Typography Scale as Styles

```xml
<!-- Typography Styles following Ant Design scale -->
<Style x:Key="TextCaption" TargetType="TextBlock">
  <Setter Property="FontSize" Value="12"/>
  <Setter Property="LineHeight" Value="20"/>
  <Setter Property="Foreground" Value="{StaticResource TextSecondaryBrush}"/>
</Style>

<Style x:Key="TextBody" TargetType="TextBlock">
  <Setter Property="FontSize" Value="14"/>
  <Setter Property="LineHeight" Value="22"/>
</Style>

<Style x:Key="TextSubtitle" TargetType="TextBlock">
  <Setter Property="FontSize" Value="16"/>
  <Setter Property="LineHeight" Value="24"/>
  <Setter Property="FontWeight" Value="Medium"/>
</Style>

<Style x:Key="TextTitle" TargetType="TextBlock">
  <Setter Property="FontSize" Value="20"/>
  <Setter Property="LineHeight" Value="28"/>
  <Setter Property="FontWeight" Value="Medium"/>
</Style>

<Style x:Key="TextHeading" TargetType="TextBlock">
  <Setter Property="FontSize" Value="24"/>
  <Setter Property="LineHeight" Value="32"/>
  <Setter Property="FontWeight" Value="SemiBold"/>
</Style>

<Style x:Key="TextDisplay" TargetType="TextBlock">
  <Setter Property="FontSize" Value="38"/>
  <Setter Property="LineHeight" Value="46"/>
  <Setter Property="FontWeight" Value="SemiBold"/>
</Style>
```

---

## Spacing System in XAML

### Spacing Constants

Define spacing as `Thickness` resources matching the 8px grid:

```xml
<Thickness x:Key="SpacingXXS">4</Thickness>
<Thickness x:Key="SpacingXS">8</Thickness>
<Thickness x:Key="SpacingSM">12</Thickness>
<Thickness x:Key="SpacingMD">16</Thickness>
<Thickness x:Key="SpacingLG">24</Thickness>
<Thickness x:Key="SpacingXL">32</Thickness>
<Thickness x:Key="SpacingXXL">48</Thickness>

<!-- Directional variants -->
<Thickness x:Key="PaddingCard">24</Thickness>
<Thickness x:Key="PaddingCardBody">24,16</Thickness>
<Thickness x:Key="MarginSection">0,0,0,24</Thickness>
```

### Usage Principle
- Between related elements (label + input): 8px
- Between sibling elements (input + input in form): 16px
- Between sections (card + card): 24px
- Page padding: 24px or 32px

---

## Layout Patterns in XAML

### Application Shell (Sidebar + Content)

```xml
<Grid>
  <Grid.ColumnDefinitions>
    <ColumnDefinition Width="240"/>  <!-- Sidebar, collapsible to 80 -->
    <ColumnDefinition Width="*"/>     <!-- Content area -->
  </Grid.ColumnDefinitions>
  
  <!-- Sidebar Navigation (using WPF UI NavigationView) -->
  <ui:NavigationView Grid.Column="0" PaneDisplayMode="Left">
    <ui:NavigationView.MenuItems>
      <ui:NavigationViewItem Content="Dashboard" Icon="{ui:SymbolIcon Home24}"/>
      <ui:NavigationViewItem Content="Projects" Icon="{ui:SymbolIcon Folder24}"/>
      <ui:NavigationViewItem Content="Settings" Icon="{ui:SymbolIcon Settings24}"/>
    </ui:NavigationView.MenuItems>
  </ui:NavigationView>
  
  <!-- Content Area -->
  <Grid Grid.Column="1" Background="{StaticResource BgLayoutBrush}">
    <Frame x:Name="ContentFrame" Margin="24"/>
  </Grid>
</Grid>
```

### Card Component

```xml
<Style x:Key="AntCard" TargetType="Border">
  <Setter Property="Background" Value="{StaticResource BgContainerBrush}"/>
  <Setter Property="BorderBrush" Value="{StaticResource BorderBrush}"/>
  <Setter Property="BorderThickness" Value="1"/>
  <Setter Property="CornerRadius" Value="8"/>
  <Setter Property="Padding" Value="24"/>
  <Setter Property="Effect">
    <Setter.Value>
      <DropShadowEffect BlurRadius="2" ShadowDepth="1" Opacity="0.03" Color="Black"/>
    </Setter.Value>
  </Setter>
</Style>

<!-- Usage -->
<Border Style="{StaticResource AntCard}">
  <StackPanel>
    <TextBlock Style="{StaticResource TextSubtitle}" Text="Card Title" Margin="0,0,0,16"/>
    <TextBlock Style="{StaticResource TextBody}" Text="Card content goes here."/>
  </StackPanel>
</Border>
```

### Form Layout

```xml
<!-- Single-column form following Ant Design rules -->
<StackPanel MaxWidth="600" Margin="24">
  
  <!-- Form Section Title -->
  <TextBlock Style="{StaticResource TextTitle}" Text="Basic Information" Margin="0,0,0,24"/>
  
  <!-- Form Field Group -->
  <StackPanel Margin="0,0,0,16">  <!-- 16px between field groups -->
    <TextBlock Text="Project Name *" Margin="0,0,0,8"/>  <!-- 8px label-to-input -->
    <TextBox/>
  </StackPanel>
  
  <StackPanel Margin="0,0,0,16">
    <TextBlock Text="Description" Margin="0,0,0,8"/>
    <TextBox AcceptsReturn="True" MinLines="3"/>
    <TextBlock Style="{StaticResource TextCaption}" Text="Brief description of the project." 
               Margin="0,4,0,0"/>
  </StackPanel>
  
  <StackPanel Margin="0,0,0,16">
    <TextBlock Text="Category *" Margin="0,0,0,8"/>
    <ComboBox>
      <ComboBoxItem Content="Development"/>
      <ComboBoxItem Content="Design"/>
      <ComboBoxItem Content="Marketing"/>
    </ComboBox>
  </StackPanel>
  
  <!-- Form Actions -->
  <StackPanel Orientation="Horizontal" HorizontalAlignment="Left" Margin="0,24,0,0">
    <ui:Button Content="Cancel" Margin="0,0,8,0"/>
    <ui:Button Content="Create Project" Appearance="Primary"/>
  </StackPanel>
</StackPanel>
```

### Data Table

In WPF, the DataGrid is the equivalent of Ant Design's Table:

```xml
<DataGrid AutoGenerateColumns="False" 
          CanUserSortColumns="True"
          IsReadOnly="True"
          SelectionMode="Extended"
          GridLinesVisibility="Horizontal"
          BorderThickness="1"
          BorderBrush="{StaticResource BorderBrush}"
          RowHeight="48"
          ColumnHeaderHeight="48">
  <DataGrid.Columns>
    <DataGridCheckBoxColumn Width="48"/>
    <DataGridTextColumn Header="Name" Binding="{Binding Name}" Width="*"/>
    <DataGridTextColumn Header="Status" Binding="{Binding Status}" Width="120"/>
    <DataGridTextColumn Header="Created" Binding="{Binding CreatedAt, StringFormat=yyyy-MM-dd}" Width="120"/>
    <DataGridTemplateColumn Header="Actions" Width="160">
      <DataGridTemplateColumn.CellTemplate>
        <DataTemplate>
          <StackPanel Orientation="Horizontal">
            <ui:HyperlinkButton Content="Edit" Margin="0,0,12,0"/>
            <ui:HyperlinkButton Content="Delete" Foreground="{StaticResource ErrorBrush}"/>
          </StackPanel>
        </DataTemplate>
      </DataGridTemplateColumn.CellTemplate>
    </DataGridTemplateColumn>
  </DataGrid.Columns>
</DataGrid>
```

---

## Component Mapping: Ant Design → WPF

| Ant Design Component | WPF UI Equivalent | Notes |
|---------------------|-------------------|-------|
| Button (Primary) | `<ui:Button Appearance="Primary"/>` | Direct mapping |
| Button (Default) | `<ui:Button/>` | Default appearance |
| Button (Danger) | `<ui:Button Appearance="Danger"/>` | Or custom style |
| Input | `<TextBox/>` | Standard WPF |
| Select | `<ComboBox/>` | Standard WPF |
| Checkbox | `<CheckBox/>` | Standard WPF |
| Radio | `<RadioButton/>` | Standard WPF |
| Switch | `<ToggleSwitch/>` | WPF UI control |
| DatePicker | `<DatePicker/>` | Standard WPF |
| Table | `<DataGrid/>` | With custom styling |
| Card | `<ui:Card/>` or styled `<Border/>` | Custom CardStyle |
| Modal | `<ui:Dialog/>` / ContentDialog | WPF UI dialog |
| Drawer | Custom panel with slide animation | Or third-party Drawer |
| Notification | `<ui:Snackbar/>` / InfoBar | WPF UI controls |
| Message (toast) | `<ui:Snackbar/>` | Auto-dismiss variant |
| Alert | `<ui:InfoBar/>` | Inline alert |
| Tabs | `<TabControl/>` | Standard WPF |
| Steps | Custom ItemsControl | Build with bullet + line template |
| Breadcrumb | Custom ItemsControl with ">" separators | Or TextBlock with Runs |
| Progress | `<ProgressBar/>` | Standard WPF |
| Spin/Loading | `<ui:ProgressRing/>` | WPF UI control |
| Tooltip | `<ToolTip/>` | Standard WPF |
| Popover | `<Popup/>` | With styled content |
| Avatar | Styled `<Ellipse/>` with `<ImageBrush/>` | Custom control |
| Badge | Custom overlay control | Positioned element |
| Tag | Styled `<Border/>` with `<TextBlock/>` | Custom style |
| Timeline | Custom ItemsControl | Dot + line + content template |
| Tree | `<TreeView/>` | Standard WPF |
| Collapse/Accordion | `<Expander/>` | Standard WPF |
| Descriptions | Custom Grid or ItemsControl | Label-value grid layout |
| Statistic | Styled TextBlock pair | Large number + label |
| Result | Custom panel | Icon + title + description + actions |
| Empty State | Custom panel | Illustration + text + CTA |
| Skeleton | Custom rectangles with animation | Pulse animation on gray shapes |
| NavigationView | `<ui:NavigationView/>` | WPF UI — excellent Ant Design sidebar equivalent |

---

## Shadow System in XAML

```xml
<!-- Ant Design shadow equivalents -->
<DropShadowEffect x:Key="ShadowCard" BlurRadius="4" ShadowDepth="2" 
                  Opacity="0.06" Color="Black" Direction="270"/>
                  
<DropShadowEffect x:Key="ShadowDropdown" BlurRadius="16" ShadowDepth="6" 
                  Opacity="0.08" Color="Black" Direction="270"/>
                  
<DropShadowEffect x:Key="ShadowModal" BlurRadius="28" ShadowDepth="9"
                  Opacity="0.12" Color="Black" Direction="270"/>
```

Note: WPF's `DropShadowEffect` is simpler than CSS box-shadow (single shadow only).
For more complex shadows, use layered Borders with different DropShadowEffects,
or use the `BoxShadow` property if available in WPF UI.

---

## Animation/Transition in XAML

### Duration Tokens

```xml
<Duration x:Key="DurationFast">0:0:0.1</Duration>      <!-- 100ms - hover states -->
<Duration x:Key="DurationMedium">0:0:0.2</Duration>     <!-- 200ms - small transitions -->
<Duration x:Key="DurationSlow">0:0:0.3</Duration>       <!-- 300ms - medium transitions -->
```

### Common Transitions

**Fade In**:
```xml
<Storyboard x:Key="FadeIn">
  <DoubleAnimation Storyboard.TargetProperty="Opacity"
                   From="0" To="1" Duration="0:0:0.2">
    <DoubleAnimation.EasingFunction>
      <CubicEase EasingMode="EaseOut"/>
    </DoubleAnimation.EasingFunction>
  </DoubleAnimation>
</Storyboard>
```

**Slide In from Right** (Drawer):
```xml
<Storyboard x:Key="SlideInRight">
  <DoubleAnimation Storyboard.TargetProperty="(UIElement.RenderTransform).(TranslateTransform.X)"
                   From="300" To="0" Duration="0:0:0.25">
    <DoubleAnimation.EasingFunction>
      <CubicEase EasingMode="EaseOut"/>
    </DoubleAnimation.EasingFunction>
  </DoubleAnimation>
</Storyboard>
```

### Easing Functions in XAML

| Ant Design Easing | XAML Easing |
|-------------------|-------------|
| ease-out (entering) | `<CubicEase EasingMode="EaseOut"/>` |
| ease-in (exiting) | `<CubicEase EasingMode="EaseIn"/>` |
| ease-in-out (general) | `<CubicEase EasingMode="EaseInOut"/>` |

---

## General Desktop Adaptation Rules

1. **Click targets**: Desktop has precise mouse, but still respect 32x32px minimums.
   Touch-enabled laptops need 44x44px.

2. **Keyboard navigation**: Every interactive element must be reachable via Tab key.
   Focus indicators must be visible. Shortcuts for power users (Ctrl+S for save, etc.).

3. **Context menus**: Desktop apps can use right-click context menus more aggressively
   than web apps. Use them for secondary actions on right-click.

4. **Window resizing**: Handle window resize gracefully. Use adaptive layouts.
   Minimum window size should be defined (e.g., 800x600).

5. **System tray**: For background processes, use system tray notifications instead of
   in-app notifications.

6. **Native feel**: Desktop users expect native OS feel. Use platform conventions:
   - Windows: Segoe UI font, Windows 11 rounded corners, Fluent Design.
   - Combine Fluent Design (native feel) with Ant Design (design system thinking).

7. **Performance**: Desktop DataGrid can handle 10,000+ rows with virtualization.
   Web tables need pagination. Adjust for platform strengths.

8. **Print support**: Enterprise desktop apps often need print capability.
   Design layouts that work on A4/Letter paper.

---

## Density Mode in XAML

Support regular and compact density modes via separate ResourceDictionary files:

```xml
<!-- DensityRegular.xaml -->
<ResourceDictionary>
  <sys:Double x:Key="ControlHeight">36</sys:Double>
  <sys:Double x:Key="ControlHeightSm">28</sys:Double>
  <sys:Double x:Key="RowHeight">48</sys:Double>
  <sys:Double x:Key="FontSizeBase">14</sys:Double>
  <sys:Double x:Key="LineHeightBase">22</sys:Double>
  <Thickness x:Key="ComponentPadding">8,8,8,8</Thickness>
</ResourceDictionary>

<!-- DensityCompact.xaml -->
<ResourceDictionary>
  <sys:Double x:Key="ControlHeight">28</sys:Double>
  <sys:Double x:Key="ControlHeightSm">24</sys:Double>
  <sys:Double x:Key="RowHeight">36</sys:Double>
  <sys:Double x:Key="FontSizeBase">13</sys:Double>
  <sys:Double x:Key="LineHeightBase">18</sys:Double>
  <Thickness x:Key="ComponentPadding">4,4,4,4</Thickness>
</ResourceDictionary>
```

Switch density at runtime by swapping the merged dictionary. All components referencing
these tokens will automatically adjust.

---

## Token Architecture in XAML

Implement the three-tier token system as nested ResourceDictionaries:

```xml
<!-- 1. GlobalTokens.xaml — raw values -->
<SolidColorBrush x:Key="GlobalColorBlue700" Color="hsl(198,100%,34%)"/>
<SolidColorBrush x:Key="GlobalColorGreen700" Color="#52C41A"/>
<SolidColorBrush x:Key="GlobalColorGray100" Color="#F5F5F5"/>

<!-- 2. SemanticAliases.xaml — functional mapping -->
<StaticResource x:Key="StatusSuccess" ResourceKey="GlobalColorGreen700"/>
<StaticResource x:Key="InteractiveDefault" ResourceKey="GlobalColorBlue700"/>

<!-- 3. ComponentTokens.xaml — component-specific -->
<StaticResource x:Key="ButtonPrimaryBackground" ResourceKey="InteractiveDefault"/>
```

For dark theme switching, provide a DarkAliases.xaml that remaps aliases to
different globals — the same ~50-line approach that makes theme switching trivial.
