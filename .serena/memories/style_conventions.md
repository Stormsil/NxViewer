# Style And Conventions

## C# / .NET
- Target framework: .NET 8, WPF (`net8.0-windows10.0.19041.0` for app/tests/infrastructure).
- `Nullable` enabled, `ImplicitUsings` enabled.
- Prefer file-scoped namespaces in `src/*`.

## Architecture
- Abstractions live in `src/NxTiler.Application/Abstractions`.
- Implementations live in `src/NxTiler.Infrastructure/*` and are wired via `src/NxTiler.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs`.
- UI + workflows live in `src/NxTiler.App` and use DI + `IMessenger` (CommunityToolkit).

## Logging
- `Microsoft.Extensions.Logging` APIs in code; `Serilog` configured in `src/NxTiler.App/App.xaml.cs`.
