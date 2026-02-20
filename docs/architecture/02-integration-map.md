# Integration Map (NxTiler + CL Libraries)

Last updated: 2026-02-13

## Principles

1. Libraries remain class libraries inside monorepo.
2. NxTiler consumes them via `ProjectReference`.
3. NxTiler runtime depends on application abstractions, not direct UI-to-library calls.

## Mapping table

| Library | Primary role | NxTiler integration surface |
|---|---|---|
| `WindowManagerCL` | Window discovery/control | `IWindowQueryService`, `IWindowControlService`, future overlay tracking adapters |
| `WindowCaptureCL` | WGC capture sessions | `ICaptureService`, `IVideoRecordingEngine` implementations |
| `ImageSearchCL` | Template matching fallback | `IVisionEngine` fallback implementation (`TemplateVisionEngine`) and YOLO failure fallback target |
| `ImageSearchCL.WindowCapture` | Bridge adapter | Transitional fallback path for capture-to-search |

## Planned adapter ownership

1. `NxTiler.Infrastructure.Windowing`
Adapters around `WindowManagerCL`.
2. `NxTiler.Infrastructure.Capture`
`WgcCaptureService` adapter around `WindowCaptureCL` for snapshot workflows.
2.1 `NxTiler.App.Services`
`SnapshotSelectionService` orchestrates interactive region/mask UX before invoking capture.
2.2 `NxTiler.Infrastructure.Recording`
`WgcVideoRecordingEngine` uses `WindowCaptureCL` frames and pipes them into ffmpeg.
3. `NxTiler.Infrastructure.Vision`
`TemplateVisionEngine` (implemented) + `YoloVisionEngine` (ONNX inference + NMS implemented, internal components split into session/preprocess/parser/postprocess).
4. `NxTiler.App.Services`
`OverlayTrackingService` composes `IWindowControlService` geometry with overlay policies to keep UI overlay aligned to tracked windows.
4.1 `NxTiler.App.Services`
`Win32CursorPositionProvider` feeds cursor position into overlay visibility policy evaluation (`Always` / `OnHover` / `HideOnHover`).
4.2 `NxTiler.App.Services`
`OverlayTrackingService` applies `OverlayAnchor` placement and monitor-bound clamping before emitting tracked state.
5. `NxTiler.Infrastructure.Recording`
WGC-to-FFmpeg pipeline plus fallback compatibility path.

## Dependency direction

1. App -> Application Abstractions.
2. Infrastructure -> CL libraries.
3. Domain has no dependency on Infrastructure or external CL libraries.

## Migration notes

1. Keep current `FfmpegRecordingEngine` path during transition.
2. Introduce WGC engine behind `FeatureFlags.UseWgcRecordingEngine`.
3. Keep template matching fallback behind `FeatureFlags.EnableTemplateMatchingFallback`.

## NX-065 decomposition tracks

1. `WindowCaptureCL`
- done: `API/Exceptions.cs` split into domain-focused exception files without namespace/API changes.
- in progress: `Core/CaptureSession.cs` converted to partial; WGC init/events moved to `CaptureSession.Wgc.cs`, frame capture/conversion moved to `CaptureSession.Frame.cs`, lifecycle/config/disposal moved to `CaptureSession.Lifecycle.cs`.
- in progress: `API/CaptureFacade` converted to partial and split by responsibilities (`Window`, `Screen`, `File`, `Settings`, `Utility`), with `WindowInfo` moved to dedicated file.
- in progress: `Infrastructure/WGC/WindowEnumerator.cs` converted to partial; Win32 interop moved to `WindowEnumerator.Interop.cs`, `WindowInfo` moved to dedicated `WindowInfo.cs`.
- in progress: `Infrastructure/ObjectPool.cs` reduced to generic pool core; keyed variant extracted to `Infrastructure/KeyedObjectPool.cs`.
2. `ImageSearchCL`
- in progress: `Core/TrackingSession.cs` converted to partial; frame-processing internals moved to `TrackingSession.Processing.cs`, lifecycle/public session methods moved to `TrackingSession.Lifecycle.cs`, state/event internals moved to `TrackingSession.State.cs`.
- in progress: `Infrastructure/DebugOverlay.cs` converted to partial; interop declarations moved to `DebugOverlay.Interop.cs`, runtime/orchestration moved to `DebugOverlay.Runtime.cs`, overlay window rendering moved to `DebugOverlay.OverlayWindow.cs`.
3. `WindowManagerCL`
- in progress: `API/WindowControl.cs` converted to partial and split into:
  - `WindowControl.Commands.cs`
  - `WindowControl.Properties.cs`
  - `WindowControl.Hierarchy.cs`
  - `WindowControl.Overrides.cs`
- done: `API/Exceptions.cs` split into dedicated exception files (`WindowManagerException`, `WindowNotFoundException`, `WindowOperationException`, `InvalidWindowHandleException`).
- in progress: `Infrastructure/WinApi.cs` converted to partial and split into:
  - `WinApi.Functions.cs`
  - `WinApi.Structures.cs`
  - `WinApi.Constants.cs`
