---
name: wpf-app
description: >
  Build professional, optimized, modern WPF desktop applications using WPF UI (Fluent Design) 
  and CommunityToolkit.MVVM. Use this skill whenever the user asks to create, scaffold, design, 
  or develop a Windows desktop application, WPF app, .NET desktop app, or any C#/XAML desktop project.
  Also trigger when the user mentions: WPF, XAML, desktop app, Windows app, Fluent UI for desktop,
  WPF UI, CommunityToolkit, MVVM pattern for desktop, SQLite local storage, or wants a professional 
  desktop tool/utility. This skill produces production-grade code with clean architecture, not AI slop.
---

# WPF Application Development Skill

Build professional Windows desktop applications with modern Fluent Design, clean MVVM architecture,
and production-grade code quality. Every app produced by this skill should feel like it was crafted
by an experienced .NET developer who cares about UX, performance, and maintainability.

## Before You Start

Read the relevant reference files based on what the app needs:

| Need | Reference file |
|------|---------------|
| **Always** | `references/architecture.md` — project structure, DI, MVVM patterns |
| **Always** | `references/ui-design.md` — WPF UI controls, theming, layout |
| **Database needed** | `references/data-layer.md` — SQLite, EF Core, Repository pattern |
| **Complex app** | `references/advanced.md` — performance, logging, settings, validation |

Read `references/architecture.md` and `references/ui-design.md` at minimum before writing any code.

## Core Design Decisions

Every app built with this skill uses:

- **.NET 8+** (LTS, modern C# features)
- **WPF UI** (`Wpf.Ui`) — Fluent Design System controls, theming, navigation
- **CommunityToolkit.MVVM** (`CommunityToolkit.Mvvm`) — source generators for clean ViewModels
- **Microsoft.Extensions.DependencyInjection** — proper IoC
- **Microsoft.Extensions.Hosting** — generic host for lifecycle management

## Workflow

### 1. Understand the App

Before writing code, clarify:
- What problem does the app solve? Who uses it?
- What are the main screens/pages?
- Does it need a local database? What data does it manage?
- Does it need system tray, notifications, file operations?

### 2. Scaffold the Project

Create the solution structure. The project layout is non-negotiable — it keeps things clean
as the app grows. See `references/architecture.md` for the full structure.

```
AppName/
├── App.xaml / App.xaml.cs          — Host setup, DI, theme init
├── MainWindow.xaml / .cs           — Shell with NavigationView
├── Models/                         — Data models (POCOs)
├── ViewModels/                     — One per page, use source generators
├── Views/                          — XAML pages, minimal code-behind
├── Services/                       — Business logic, data access
├── Helpers/                        — Converters, extensions, utilities
├── Assets/                         — Icons, images, fonts
└── appsettings.json                — Configuration
```

### 3. Build the Shell First

The main window is the skeleton of the app. Always use `FluentWindow` with a `NavigationView`.
Get the shell working and navigating between empty pages before building features. This catches
layout issues early and gives a tangible sense of progress.

### 4. Build Features Page by Page

For each feature:
1. Create the Model (if new data is involved)
2. Create the ViewModel (with `[ObservableProperty]` and `[RelayCommand]`)
3. Create the View (XAML page bound to ViewModel)
4. Register in DI container
5. Add navigation entry

### 5. Polish

- Test theme switching (Light/Dark)
- Verify responsive layout at different window sizes
- Check that async operations show loading states
- Ensure all strings could be localized (no magic strings in UI)

## Anti-Patterns to Avoid

These are the hallmarks of AI-generated slop. Never do these:

- **Code-behind logic** — If your `.xaml.cs` has more than DI constructor + `InitializeComponent()`, something is wrong. Business logic belongs in ViewModels and Services.
- **God ViewModels** — One ViewModel per page/dialog. If a ViewModel exceeds ~200 lines, decompose it.
- **Synchronous I/O on UI thread** — Every file, database, or network operation must be `async`. Use `[RelayCommand]` which generates async command wrappers automatically.
- **Hardcoded strings in XAML** — Use resource dictionaries or constants. Colors, font sizes, margins — define once, reuse everywhere.
- **Ignored MVVM** — Never create UI elements in C# code. Never access controls by name from ViewModel. The ViewModel doesn't know the View exists.
- **Empty catch blocks** — Always log exceptions. `catch (Exception ex) { _logger.LogError(ex, "..."); }` at minimum.
- **Nested Grid rows 10 levels deep** — If your layout is that complex, decompose into UserControls.
- **Default WPF chrome** — Always use `FluentWindow` with `TitleBar` control. The default Windows chrome looks dated.

## Code Style

- Use `file-scoped namespaces` (`namespace App.ViewModels;`)
- Use `primary constructors` for DI injection where appropriate
- Use C# 12+ features: collection expressions, pattern matching, raw string literals
- Naming: `_camelCase` for private fields, `PascalCase` for properties/methods
- One class per file. File name matches class name.
- XAML: indent with 4 spaces, one attribute per line for elements with >2 attributes
- Comments: explain WHY, not WHAT. Code should be self-documenting.

## NuGet Packages — Quick Reference

```xml
<!-- Core — always include -->
<PackageReference Include="Wpf.Ui" Version="4.*" />
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.*" />
<PackageReference Include="Microsoft.Extensions.Hosting" Version="8.*" />

<!-- Database — when local storage is needed -->
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.*" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.*" />

<!-- Logging — always include -->
<PackageReference Include="Serilog.Extensions.Hosting" Version="8.*" />
<PackageReference Include="Serilog.Sinks.File" Version="6.*" />
<PackageReference Include="Serilog.Sinks.Debug" Version="3.*" />

<!-- Validation — when forms are involved -->
<PackageReference Include="FluentValidation" Version="11.*" />
```

## Output Checklist

Before delivering the app to the user, verify:

- [ ] App compiles without warnings
- [ ] Dark/Light theme switching works
- [ ] Window is resizable with proper minimum size
- [ ] Navigation between all pages works
- [ ] Async operations don't freeze UI
- [ ] Exception handling is present for all I/O
- [ ] No hardcoded absolute paths
- [ ] Services are registered in DI
- [ ] ViewModels use source generators (`[ObservableProperty]`, `[RelayCommand]`)
- [ ] XAML uses WPF UI controls (`ui:Button`, `ui:TextBox`, `ui:Card`, etc.)
