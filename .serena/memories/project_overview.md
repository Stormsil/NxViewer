# NxTiler Overview

## Purpose
NxTiler is a Windows WPF application (net8.0-windows) that orchestrates window tiling/workspace behavior and provides overlay UI (mask/dim/recording overlays) plus global hotkeys.

## Solution Structure
- `src/NxTiler.App`: WPF UI app using Generic Host + DI, CommunityToolkit.MVVM, WPF-UI, Serilog.
- `src/NxTiler.Application`: application abstractions and DI wiring (`*.Abstractions`).
- `src/NxTiler.Domain`: domain models/enums/settings/windowing types.
- `src/NxTiler.Infrastructure`: Windows/IO implementations (Win32 interop, settings persistence, ffmpeg setup/recording engine, hotkeys, etc.).
- `tests/NxTiler.Tests`: xUnit + Moq tests.

## Recording Pipeline (High Level)
- UI triggers `IRecordingWorkflowService`.
- Workflow resolves capture bounds, shows/controls overlays, resolves ffmpeg via `IFfmpegSetupService`.
- Recording is executed by `IRecordingEngine` (default: `FfmpegRecordingEngine`).
