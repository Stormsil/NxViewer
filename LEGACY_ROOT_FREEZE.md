# Legacy Root Freeze

The legacy root WPF project (`NxTiler.csproj` and root `*.xaml/*.cs` files) is frozen.

## Canonical codebase

Use only:

- `src/NxTiler.App`
- `src/NxTiler.Application`
- `src/NxTiler.Domain`
- `src/NxTiler.Infrastructure`
- `src/WindowCaptureCL`
- `src/WindowManagerCL`
- `src/ImageSearchCL`

## Rules

1. No new features are added to root legacy files.
2. Root legacy files are touched only for emergency compatibility fixes.
3. All new architecture, contracts, and behavior must be implemented in `src/*`.
