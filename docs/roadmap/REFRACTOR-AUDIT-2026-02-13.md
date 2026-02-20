# NxTiler Refactor Audit (2026-02-13)

Last updated: 2026-02-13

## Scope

Static audit of canonical codebase and integrated libraries:

1. `src/NxTiler.*`
2. `src/WindowCaptureCL`
3. `src/WindowManagerCL`
4. `src/ImageSearchCL`

## Hotspot inventory (by size/complexity)

Top canonical hotspots (LOC):

1. `src/NxTiler.App/ViewModels/LogsViewModel.cs` (~183)
2. `src/NxTiler.App/App.xaml.cs` (~175)
3. `src/NxTiler.Infrastructure/Settings/JsonSettingsService.Normalization.cs` (~164)
4. `src/NxTiler.App/Services/RecordingWorkflowService.Commands.cs` (~159)
5. `src/NxTiler.App/Views/OverlayWindow.Tracking.cs` (~156)
6. `src/NxTiler.App/ViewModels/DashboardViewModel.Commands.cs` (~154)
7. `src/NxTiler.App/Services/RecordingOverlayService.cs` (~149)

Top integrated-library hotspots (LOC):

1. `src/WindowCaptureCL/Core/CaptureSession.Wgc.cs` (~197)
2. `src/ImageSearchCL/Core/TrackingSession.Lifecycle.cs` (~183)
3. `src/ImageSearchCL/Infrastructure/DebugOverlay.Runtime.cs` (~165)
4. `src/ImageSearchCL/Core/TrackingSession.Processing.cs` (~157)
5. `src/WindowCaptureCL/Infrastructure/DirectX/FrameProcessor.Conversion.cs` (~150)
6. `src/WindowCaptureCL/Infrastructure/WGC/MonitorEnumerator.cs` (~149)
7. `src/WindowCaptureCL/Infrastructure/WGC/WgcInterop.cs` (~146)

## Priority findings

## R1: Recording engines duplicate FFmpeg process orchestration (Done)

Files:

1. `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.cs:136`
2. `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.cs:197`
3. `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.cs:543`
4. `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.cs:496`

Risk:

1. Divergent behavior in timeout, stderr handling, and masking fallback.
2. Hard to change FFmpeg policies safely in one place.

Refactor direction:

1. Introduce shared FFmpeg process runner + stderr tail utility.
2. Move masking command construction to shared component.

## R2: RecordingWorkflowService combines state machine + engine routing + UI messaging (Partially done)

Files:

1. `src/NxTiler.App/Services/RecordingWorkflowService.cs:108`
2. `src/NxTiler.App/Services/RecordingWorkflowService.cs:258`
3. `src/NxTiler.App/Services/RecordingWorkflowService.cs:388`

Risk:

1. Transition bugs are hard to isolate.
2. New recording behavior increases branching cost.

Refactor direction:

1. Extract explicit recording state machine/transition map.
2. Move engine selection/fallback policy to dedicated strategy service.

## R3: DashboardViewModel has high orchestration density (Done)

Files:

1. `src/NxTiler.App/ViewModels/DashboardViewModel.cs:172`
2. `src/NxTiler.App/ViewModels/DashboardViewModel.cs:332`
3. `src/NxTiler.App/ViewModels/DashboardViewModel.cs:386`

Risk:

1. UI command changes impact unrelated snapshot and recording state logic.
2. Hard to unit-test command groups independently.

Refactor direction:

1. Split command handling into feature-focused command services.
2. Keep ViewModel as state projection + binding surface.

Current status:

1. `DashboardViewModel` converted to partial and split into focused units:
- `DashboardViewModel.cs` (state/properties)
- `DashboardViewModel.Lifecycle.cs`
- `DashboardViewModel.Commands.cs`
- `DashboardViewModel.Snapshot.cs`
2. Command groups extracted into dedicated injectable services:
- `IDashboardWorkspaceCommandService` / `DashboardWorkspaceCommandService`
- `IDashboardRecordingCommandService` / `DashboardRecordingCommandService`
3. Busy/error execution policy extracted into dedicated service:
- `IDashboardCommandExecutionService` / `DashboardCommandExecutionService`
4. Remaining optional follow-up: evaluate snapshot-projection extraction only if `DashboardViewModel.Snapshot.cs` complexity increases.

## R4: JsonSettingsService normalization is monolithic (Done)

Files:

1. `src/NxTiler.Infrastructure/Settings/JsonSettingsService.cs:163`
2. `src/NxTiler.Infrastructure/Settings/JsonSettingsService.cs:252`

Risk:

1. Schema evolution increases merge conflicts and regression risk.
2. Single method grows with each feature flag/settings section.

Refactor direction:

1. Extract per-section normalizers (`Hotkeys`, `Vision`, `OverlayPolicies`, etc.).
2. Keep coordinator in `JsonSettingsService` thin.

## R5: YoloVisionEngine mixes session lifecycle, preprocess, parse, and NMS in one type (Done)

Files:

1. `src/NxTiler.Infrastructure/Vision/YoloVisionEngine.cs:30`
2. `src/NxTiler.Infrastructure/Vision/YoloVisionEngine.cs:222`
3. `src/NxTiler.Infrastructure/Vision/YoloVisionEngine.cs:257`

Risk:

1. Hard to benchmark/replace preprocess or parser independently.
2. Model-compatibility changes affect one large class.

Implemented:

1. Extracted contracts:
- `IYoloSessionProvider`
- `IYoloPreprocessor`
- `IYoloOutputParser`
2. Added implementations:
- `YoloSessionProvider`
- `YoloPreprocessor`
- `YoloOutputParser`
- `YoloDetectionPostProcessor`
3. `YoloVisionEngine` is now orchestration shell while preserving `IVisionEngine`.
4. Added focused tests for preprocessor/parser/NMS components.

## R6: Integrated CL libraries have oversized API surface types (Done)

Files:

1. `src/WindowCaptureCL/API/Capture*Exceptions*.cs`
2. `src/ImageSearchCL/Core/TrackingSession.cs`
3. `src/WindowManagerCL/WindowManagerCL/API/WindowControl.cs`

Risk:

1. Library internals are harder to evolve and test in isolation.
2. Public API changes may require large-file edits with high conflict risk.

Refactor direction:

1. Split exception catalog into partial files by domain.
2. Extract `TrackingSession` state handling from frame-processing pipeline.
3. Separate `WindowControl` query methods from mutating commands.

Current decomposition slices:

1. `WindowCaptureCL`
- completed: `Exceptions.cs` split into grouped files (`CaptureExceptionBase`, `CapturePlatformExceptions`, `CaptureResourceExceptions`, `CaptureValidationExceptions`, `CaptureSessionExceptions`).
- in progress: `CaptureSession.cs` converted to partial; session init/events split to `CaptureSession.Wgc.cs`, frame capture/conversion split to `CaptureSession.Frame.cs`, lifecycle/config/disposal split to `CaptureSession.Lifecycle.cs`.
- in progress: `CaptureFacade.cs` converted to partial and split to mode-focused files (`Window`, `Screen`, `File`, `Settings`, `Utility`) with `WindowInfo` moved to dedicated file.
- in progress: `FrameProcessor.cs` converted to partial shell with staging pool/lifecycle (`FrameProcessor.Pool.cs`), conversion overloads (`FrameProcessor.Conversion.cs`), and async wrapper (`FrameProcessor.Async.cs`).
- in progress: `DpiHelper.cs` converted to partial shell with awareness (`DpiHelper.Awareness.cs`), query/conversion (`DpiHelper.Query.cs`), and native interop (`DpiHelper.Native.cs`) units.
- in progress: `DirectXDeviceManager.cs` converted to partial shell with initialization (`DirectXDeviceManager.Initialization.cs`), resource allocation (`DirectXDeviceManager.Resources.cs`), and lifecycle (`DirectXDeviceManager.Lifecycle.cs`) units.
- in progress: `WindowEnumerator.cs` converted to partial shell with interop declarations moved to `WindowEnumerator.Interop.cs` and `WindowInfo` moved to dedicated file `WindowInfo.cs`.
- in progress: `ObjectPool.cs` simplified to single-type responsibility with `KeyedObjectPool<TKey, TValue>` extracted to dedicated `KeyedObjectPool.cs`.
- next: evaluate additional split of frame-throttle/telemetry concerns if class keeps growing.
2. `ImageSearchCL`
- in progress: `TrackingSession.cs` converted to partial; processing internals extracted to `TrackingSession.Processing.cs`; lifecycle/public session methods extracted to `TrackingSession.Lifecycle.cs`; state/event internals extracted to `TrackingSession.State.cs`.
- next: evaluate optional micro-splits only if behavior changes require them.
- in progress: `DebugOverlay.cs` converted to partial and Win32 interop declarations extracted to `DebugOverlay.Interop.cs`.
- in progress: runtime/orchestration extracted to `DebugOverlay.Runtime.cs` and overlay rendering extracted to `DebugOverlay.OverlayWindow.cs`.
- in progress: nested `OverlayWindow` split into partial shell (`DebugOverlay.OverlayWindow.cs`), rendering flow (`DebugOverlay.OverlayWindow.Rendering.cs`), and native interop (`DebugOverlay.OverlayWindow.Native.cs`).
- in progress: `TemplateMatchingEngine.cs` converted to partial shell with single-match flow (`TemplateMatchingEngine.Single.cs`) and multi-match/NMS flow (`TemplateMatchingEngine.Multi.cs`) extracted.
- in progress: `FrameQueue.cs` converted to partial shell with operations (`FrameQueue.Operations.cs`) and lifecycle (`FrameQueue.Lifecycle.cs`) extracted.
3. `WindowManagerCL`
- completed: `WindowControl` split into partial files (`Commands`, `Properties`, `Hierarchy`, `Overrides`) with unchanged public API.
- completed: exception catalog split from monolithic file into dedicated exception files.
- in progress: `Window.cs` facade converted to partial shell with search/discovery (`Window.Find.cs`) and handle/foreground methods (`Window.Handle.cs`) extracted.
- in progress: `WindowFinder.cs` converted to partial shell with enumeration (`WindowFinder.Find.cs`) and filter/regex helpers (`WindowFinder.Filter.cs`) extracted.
- in progress: `Infrastructure/WinApi.cs` converted to partial shell with declarations/constants/structures extracted to `WinApi.Functions.cs`, `WinApi.Constants.cs`, and `WinApi.Structures.cs`.
- result: integrated-library hotspot envelope reduced; largest remaining files are now below ~200 LOC and separated by concern.

## R7: ImageSearchCL API facade has high concentration in `Search`/config types (Done)

Files:

1. `src/ImageSearchCL/API/Search.cs`
2. `src/ImageSearchCL/API/ImageSearchConfiguration.cs`
3. `src/ImageSearchCL/API/Images.cs`

Risk:

1. Public API evolution requires high-churn edits in large files.
2. Builder + static facade + input validation are coupled, making incremental changes harder.

Refactor direction:

1. Split `Search` into partial units (`For/ForAny`, `Find/FindAll`, nested builder) without changing public signatures.
2. Split configuration surface (`ImageSearchConfiguration`, `Images`) into focused partial/domain files while preserving API.
3. Add focused API parity tests for fluent/builder entry points before deeper internal decomposition.

Implemented:

1. `Search` facade split into dedicated partial files (`Search.For`, `Search.Find`, `SearchBuilder`).
2. `ImageSearchConfiguration` split into core fields/properties and enum moved to dedicated file.
3. `Images` API split into collection/factory/lifecycle partial files.
4. Added `ImageSearchApiFacadeTests` for fluent API argument and behavior parity.
5. `FindResult` API model split into core/geometry/equality partial files.
6. `IObjectSearch` API contract split into shell/events/state-method contract partial files.
7. `TrackingConfiguration` API model split into core/validation/withers partial files.
8. `ReferenceImage` API model split into core/factory/lifecycle partial files.
9. `ImageSearch` API facade split into core/find/find-all partial files.

## R8: Recording engines still concentrate command/pipeline internals (Done)

Files:

1. `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.cs`
2. `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.cs`

Risk:

1. New recording options/mask paths still require edits in very large files.
2. Engine-specific regressions are harder to isolate quickly.

Refactor direction:

1. Extract FFmpeg argument/profile builder and frame-pipe helpers behind internal collaborators.
2. Keep engines as orchestration shells with unchanged contracts.

Current status:

1. `FfmpegRecordingEngine` converted to partial orchestration shell with extracted files:
- `FfmpegRecordingEngine.Arguments.cs`
- `FfmpegRecordingEngine.Finalize.cs`
- `FfmpegRecordingEngine.Lifecycle.cs`
2. `WgcVideoRecordingEngine` converted to partial orchestration shell with extracted files:
- `WgcVideoRecordingEngine.Arguments.cs`
- `WgcVideoRecordingEngine.FramePump.cs`
- `WgcVideoRecordingEngine.Lifecycle.cs`
- `WgcVideoRecordingEngine.Masking.cs`
- `WgcVideoRecordingEngine.Geometry.cs`
3. Validation after slice is green (`build`, `tests`, `governance`).
4. Added shared `FfmpegArgumentBuilder` consumed by both engines for segment/concat/masking/image-pipe args.
5. Added focused argument parity tests in `tests/NxTiler.Tests/FfmpegArgumentBuilderTests.cs`.

## R9: RecordingWorkflowService still combines routing + messaging + finalization (Done)

Files:

1. `src/NxTiler.App/Services/RecordingWorkflowService.cs`

Risk:

1. Workflow regressions are harder to diagnose because control flow spans many concerns.
2. Additional recording modes will keep increasing branching complexity.

Refactor direction:

1. Extract engine-routing policy and status-message projection into dedicated collaborators.
2. Extract finalize/abort output handling into focused helper service.

Current status:

1. `RecordingWorkflowService` converted to partial root shell.
2. Extracted files:
- `RecordingWorkflowService.Commands.cs`
- `RecordingWorkflowService.Overlay.cs`
- `RecordingWorkflowService.State.cs`
- `RecordingWorkflowService.Execution.cs`
3. Added engine-routing helper:
- `RecordingWorkflowService.EngineRouting.cs`
4. Validation after slices is green (`build`, `tests`, `governance`).

## R10: Overlay/snapshot code-behind concentration in app layer (Done)

Files:

1. `src/NxTiler.App/Views/OverlayWindow.xaml.cs`
2. `src/NxTiler.App/Views/SnapshotSelectionWindow.xaml.cs`

Risk:

1. UI behavior updates require editing large code-behind files with mixed responsibilities.
2. Harder to add focused tests for interaction logic.

Refactor direction:

1. Split view code-behind by lifecycle, placement/rendering, and interaction behavior.
2. Extract mask/selection interaction logic to testable collaborators where possible.

Current status:

1. `OverlayWindow.xaml.cs` converted to partial root shell.
2. Extracted files:
- `OverlayWindow.Interaction.cs`
- `OverlayWindow.Positioning.cs`
- `OverlayWindow.Messaging.cs`
- `OverlayWindow.Tracking.cs`
3. Validation after slice is green (`build`, `tests`, `governance`).
4. `SnapshotSelectionWindow.xaml.cs` converted to partial root shell with extracted files:
- `SnapshotSelectionWindow.Input.cs`
- `SnapshotSelectionWindow.Selection.cs`
- `SnapshotSelectionWindow.Layout.cs`
- `SnapshotSelectionWindow.Conversion.cs`
5. Validation after second slice is green (`build`, `tests`, `governance`).

## R11: App ViewModel orchestration density (Done)

Files:

1. `src/NxTiler.App/ViewModels/OverlayWindowViewModel.cs`
2. `src/NxTiler.App/ViewModels/RecordingViewModel.cs`
3. `src/NxTiler.App/ViewModels/RecordingBarViewModel.cs`

Risk:

1. UI-state and command orchestration changes stay concentrated in large view-model files.
2. Feature additions increase coupling between projection and interaction logic.

Refactor direction:

1. Split view-models into partials by concerns (state projection, commands, lifecycle/messages).
2. Extract reusable command orchestration to dedicated services where behavior crosses view-model boundaries.

Current status:

1. `OverlayWindowViewModel` decomposition completed:
- `OverlayWindowViewModel.cs` converted to partial shell.
- extracted `Commands`, `Messaging`, `Snapshot`, and `Lifecycle` partial files.
2. `RecordingViewModel` decomposition completed:
- `RecordingViewModel.cs` converted to partial shell.
- extracted `Commands`, `State`, and `Lifecycle` partial files.
3. `RecordingBarViewModel` decomposition completed:
- `RecordingBarViewModel.cs` converted to partial shell.
- extracted `Commands`, `State`, and `Lifecycle` partial files.
4. Validation after slices is green (`build`, `tests`, `governance`).

## R12: Canonical infrastructure/service hotspots (Done)

Files:

1. `src/NxTiler.Infrastructure/Settings/JsonSettingsService.cs`
2. `src/NxTiler.Infrastructure/Capture/WgcCaptureService.cs`

Risk:

1. New feature work still converges on a few dense service classes.
2. Regression surface is larger when modifying settings/tracking/capture behaviors.

Refactor direction:

1. Split service internals into focused policy/helpers while preserving contracts.
2. Add focused tests around extracted helpers before larger behavior changes.

Current status:

1. `OverlayTrackingService` decomposition completed:
- `OverlayTrackingService.cs` converted to partial root shell.
- extracted `Lifecycle`, `Loop`, `State`, `Visibility`, and `Geometry` partial files.
2. `WgcCaptureService` decomposition completed:
- `WgcCaptureService.cs` converted to partial root shell.
- extracted `WindowPreparation`, `Geometry`, `Bitmap`, and `Output` partial files.
3. Validation after slices is green (`build`, `tests`, `governance`).

## R13: Native interop concentration in infrastructure (Done)

Files:

1. `src/NxTiler.Infrastructure/Native/Win32Native.cs`

Risk:

1. Interop declarations, structures, constants, and helper logic concentrated in one file increase maintenance risk.
2. Cross-cutting edits for capture/overlay/hotkeys touch the same file and increase merge friction.

Refactor direction:

1. Split `Win32Native` into partial files by concern (interop/constants/structures/helpers).
2. Preserve existing method names/signatures/constants to avoid behavior changes.

Current status:

1. `Win32Native` decomposition completed:
- `Win32Native.cs` converted to partial root shell.
- extracted `Win32Native.Interop.cs`, `Win32Native.Constants.cs`, `Win32Native.Structures.cs`, and `Win32Native.Helpers.cs`.
2. Validation after slice is green (`build`, `tests`, `governance`).

## R14: Canonical app-service orchestration density (Done)

Files:

1. `src/NxTiler.App/Services/VisionWorkflowService.cs`
2. `src/NxTiler.App/Services/WorkspaceOrchestrator.Commands.cs`
3. `src/NxTiler.App/Services/WorkspaceOrchestrator.MessageHandling.cs`

Risk:

1. New behavior around vision/hotkey workflows still concentrates in a few app-service files.
2. Routing and fallback policy changes can create broad regression surface.

Refactor direction:

1. Split `VisionWorkflowService` into scan execution, engine selection, and fallback policy units.
2. Extract `WorkspaceOrchestrator` command/message routing helpers into focused collaborators.

Current status:

1. `VisionWorkflowService` decomposition completed:
- `VisionWorkflowService.cs` converted to partial root shell.
- extracted `Commands`, `Scan`, `EngineResolution`, `TargetResolution`, and `Execution` partial files.
2. `WorkspaceOrchestrator` decomposition completed for command/message concentration:
- `WorkspaceOrchestrator.Commands.cs` converted to partial shell with extracted `Commands.Arrangement`, `Commands.Navigation`, and `Commands.Sessions`.
- `WorkspaceOrchestrator.MessageHandling.cs` converted to partial shell with extracted `Events`, `Hotkeys`, `Execution`, and `Registration` partial files.
3. Validation after slices is green (`build`, `tests`, `governance`).

## R15: Settings service structural concentration (Done)

Files:

1. `src/NxTiler.Infrastructure/Settings/JsonSettingsService.cs`

Risk:

1. Load/reload, persistence, and normalization logic in one file increases review and regression surface.
2. Settings evolution can cause high-churn edits in a single service file.

Refactor direction:

1. Split settings service by concern into loading, persistence, and normalization units.
2. Preserve `ISettingsService` behavior and migration/normalization semantics.

Current status:

1. `JsonSettingsService` decomposition completed:
- `JsonSettingsService.cs` converted to partial root shell.
- extracted `JsonSettingsService.Loading.cs`, `JsonSettingsService.Persistence.cs`, and `JsonSettingsService.Normalization.cs`.
2. Validation after slice is green (`build`, `tests`, `governance`).

## R16: Remaining canonical concentration hotspots (Done)

Files:

1. `src/NxTiler.App/ViewModels/SettingsViewModel.cs`
2. `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.cs`
3. `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.Finalize.cs`

Risk:

1. Finalization and settings UI changes still converge on large files.
2. Recording and settings changes may produce broad diffs with higher merge overhead.

Refactor direction:

1. Extract ffmpeg finalize subflows into focused helpers while preserving recording outputs.
2. Split `SettingsViewModel` into tab/section-specific command and state projection units.
3. Keep behavior parity validated by existing test suite and governance checks.

Current status:

1. `FfmpegRecordingEngine` finalize-flow decomposition completed:
- `FfmpegRecordingEngine.Finalize.cs` reduced to orchestration shell.
- extracted `FfmpegRecordingEngine.Finalize.Single.cs`, `FfmpegRecordingEngine.Finalize.Concat.cs`, and `FfmpegRecordingEngine.Finalize.Masking.cs`.
2. `SettingsViewModel` decomposition completed:
- `SettingsViewModel.cs` converted to partial root shell.
- extracted `SettingsViewModel.Commands.cs` and `SettingsViewModel.Snapshot.cs`.
3. `WgcVideoRecordingEngine` root-shell decomposition completed:
- `WgcVideoRecordingEngine.cs` converted to partial root shell.
- extracted `WgcVideoRecordingEngine.Start.cs`, `WgcVideoRecordingEngine.Control.cs`, and `WgcVideoRecordingEngine.Stop.cs`.
4. Validation after slices is green (`build`, `tests`, `governance`).

## R17: Next canonical concentration hotspots (Done)

Files:

1. `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.cs`
2. `src/NxTiler.Infrastructure/Legacy/LegacyAppSettings.cs`
3. `src/NxTiler.App/Services/WorkspaceOrchestrator.cs`

Risk:

1. Lifecycle/startup orchestration remains concentrated in a single orchestrator root file.
2. Legacy settings parsing and recording engine root concerns still create larger change surfaces.

Refactor direction:

1. Split orchestrator lifecycle shell (`Start/Dispose/monitor setup`) into focused units.
2. Split legacy settings structures/parsing helpers and recording-engine root shell concerns while preserving behavior.

Current status:

1. `WorkspaceOrchestrator` lifecycle-core decomposition completed:
- `WorkspaceOrchestrator.cs` reduced to state/constructor shell.
- extracted `WorkspaceOrchestrator.Lifecycle.cs` and `WorkspaceOrchestrator.Monitoring.cs`.
2. `LegacyAppSettings` decomposition completed:
- `LegacyAppSettings.cs` converted to partial root shell.
- extracted `LegacyAppSettings.Hotkeys.cs` and `LegacyAppSettings.Recording.cs`.
3. `FfmpegRecordingEngine` root-shell decomposition completed:
- `FfmpegRecordingEngine.cs` reduced to root shell.
- extracted `FfmpegRecordingEngine.Start.cs` and `FfmpegRecordingEngine.Segments.cs`.
4. Validation after slices is green (`build`, `tests`, `governance`).

## R18: Next canonical UI hotspots (Done)

Files:

1. `src/NxTiler.App/ViewModels/LogsViewModel.cs`
2. `src/NxTiler.App/App.xaml.cs`

Risk:

1. UI lifecycle and logging projection were concentrated in a few files.
2. Startup edits in `App.xaml.cs` created broad merge surfaces.

Refactor direction:

1. Split `LogsViewModel` by lifecycle/state/commands.
2. Split app bootstrap/startup wiring in `App.xaml.cs`.

Current status:

1. `LogsViewModel` decomposition completed:
- `LogsViewModel.cs` reduced to root shell.
- extracted `LogsViewModel.Filtering.cs`, `LogsViewModel.Lifecycle.cs`, and `LogsViewModel.Commands.cs`.
2. App bootstrap decomposition completed:
- `App.xaml.cs` reduced to root shell.
- extracted `App.Hosting.cs`, `App.Lifecycle.cs`, and `App.Errors.cs`.
3. Validation after slices is green (`build`, `tests`, `governance`).

## R19: Next canonical service hotspots (Done)

Files:

1. `src/NxTiler.App/Services/RecordingWorkflowService.Commands.cs`
2. `src/NxTiler.Infrastructure/Settings/JsonSettingsService.Normalization.cs`
3. `src/NxTiler.Application/Services/ArrangementService.cs`

Risk:

1. Recording command flow and settings normalization remain concentrated hotspots.
2. Core arrangement policy changes can still produce large diffs.

Refactor direction:

1. Split recording commands into focused command-group slices while preserving workflow semantics.
2. Micro-split normalization policies in `JsonSettingsService` by section responsibility.
3. Consider arrangement-service structural split only after command/normalization hotspots are reduced.

Current status:

1. `RecordingWorkflowService` command-slice decomposition completed:
- `RecordingWorkflowService.Commands.cs` reduced to root shell.
- extracted `RecordingWorkflowService.Commands.MaskEditing.cs`, `RecordingWorkflowService.Commands.Control.cs`, and `RecordingWorkflowService.Commands.Completion.cs`.
2. `JsonSettingsService` normalization micro-split completed:
- `JsonSettingsService.Normalization.cs` reduced to root shell.
- extracted `JsonSettingsService.Normalization.Core.cs`, `JsonSettingsService.Normalization.General.cs`, `JsonSettingsService.Normalization.Features.cs`, and `JsonSettingsService.Normalization.Overlay.cs`.
3. Validation after slices is green (`build`, `tests`, `governance`).
4. `ArrangementService` decomposition completed:
- `ArrangementService.cs` reduced to orchestration shell.
- extracted `ArrangementService.Grid.cs`, `ArrangementService.Focus.cs`, and `ArrangementService.Layout.cs`.
5. Validation remains green after final `R19` slice (`build`, `tests`, `governance`).

## R20: Next canonical UI/infrastructure hotspots (Done)

Files:

1. `src/NxTiler.App/ViewModels/DashboardViewModel.Commands.cs`
2. `src/NxTiler.App/Views/OverlayWindow.Tracking.cs`
3. `src/NxTiler.App/ViewModels/HotkeysViewModel.cs`

Risk:

1. Large UI command/interaction files still create broad merge surfaces.
2. Overlay and hotkey behavior adjustments can increase regression risk if refactored without granular slices.

Refactor direction:

1. Micro-split dashboard commands by command category.
2. Split overlay tracking code-behind by polling/visibility/transform responsibilities.
3. Split hotkeys view-model by load/save/projection concerns.

Current status:

1. `DashboardViewModel` command-slice micro-split completed:
- `DashboardViewModel.Commands.cs` reduced to root shell.
- extracted `DashboardViewModel.Commands.Workspace.cs`, `DashboardViewModel.Commands.Recording.cs`, `DashboardViewModel.Commands.State.cs`, and `DashboardViewModel.Commands.Execution.cs`.
2. `OverlayWindow.Tracking` micro-split completed:
- `OverlayWindow.Tracking.cs` reduced to root shell.
- extracted `OverlayWindow.Tracking.Lifecycle.cs`, `OverlayWindow.Tracking.State.cs`, and `OverlayWindow.Tracking.Scale.cs`.
3. `HotkeysViewModel` decomposition completed:
- `HotkeysViewModel.cs` reduced to root shell.
- extracted `HotkeysViewModel.Commands.cs` and `HotkeysViewModel.Snapshot.cs`.
4. Validation after slices is green (`build`, `tests`, `governance`).

## R21: Next canonical app/infrastructure hotspots (Done)

Files:

1. `src/NxTiler.App/Services/RecordingOverlayService.cs`
2. `src/NxTiler.Infrastructure/Windowing/WindowEventMonitorService.cs`
3. `src/NxTiler.App/ViewModels/MaskOverlayViewModel.cs`

Risk:

1. Overlay and window-monitor control paths are still concentrated in large files.
2. UI overlay refactors can introduce behavior regressions if not split incrementally.

Refactor direction:

1. Split `RecordingOverlayService` by lifecycle/window-host/mask-bridging concerns.
2. Split window monitor service by subscription lifecycle and event projection concerns.
3. Split `MaskOverlayViewModel` by state/projection and command handling.

Current status:

1. `RecordingOverlayService` decomposition completed:
- `RecordingOverlayService.cs` reduced to root shell.
- extracted `RecordingOverlayService.Lifecycle.cs`, `RecordingOverlayService.Mode.cs`, `RecordingOverlayService.Masks.cs`, and `RecordingOverlayService.Ui.cs`.
2. `WindowEventMonitorService` decomposition completed:
- `WindowEventMonitorService.cs` reduced to root shell.
- extracted `WindowEventMonitorService.Lifecycle.cs` and `WindowEventMonitorService.Events.cs`.
3. `MaskOverlayViewModel` decomposition completed:
- `MaskOverlayViewModel.cs` reduced to root shell.
- extracted `MaskOverlayViewModel.Masks.cs`, `MaskOverlayViewModel.State.cs`, and `MaskOverlayViewModel.Commands.cs`.
4. Validation after slices is green (`build`, `tests`, `governance`).

## R22: Next canonical infrastructure hotspots (Done)

Files:

1. `src/NxTiler.Infrastructure/Recording/FfmpegSetupService.cs`
2. `src/NxTiler.Infrastructure/Vision/YoloVisionEngine.cs`
3. `src/NxTiler.App/Services/CaptureWorkflowService.cs`

Risk:

1. Setup and vision orchestration paths are still concentrated in large files.
2. Capture-flow and setup refactors may impact runtime workflows if changed non-incrementally.

Refactor direction:

1. Split FFmpeg setup service by detection/probe and install-download helpers.
2. Split `YoloVisionEngine` orchestration path by execution and settings-resolution helpers.
3. Split capture workflow command routing from output/reporting helpers.

Current status:

1. `FfmpegSetupService` decomposition completed:
- `FfmpegSetupService.cs` reduced to root shell.
- extracted `FfmpegSetupService.Resolve.cs`, `FfmpegSetupService.Download.cs`, and `FfmpegSetupService.Probe.cs`.
2. `YoloVisionEngine` micro-decomposition completed:
- `YoloVisionEngine.cs` reduced to root shell.
- extracted `YoloVisionEngine.Detection.cs`, `YoloVisionEngine.Configuration.cs`, and `YoloVisionEngine.Lifecycle.cs`.
3. `CaptureWorkflowService` decomposition completed:
- `CaptureWorkflowService.cs` reduced to root shell.
- extracted `CaptureWorkflowService.Commands.cs`, `CaptureWorkflowService.TargetResolution.cs`, and `CaptureWorkflowService.Execution.cs`.
4. Validation after slices is green (`build`, `tests`, `governance`).

## R23: Next canonical workflow/infrastructure hotspots (Done)

Files:

1. `src/NxTiler.Infrastructure/Nomachine/NomachineSessionService.cs`
2. `src/NxTiler.App/Views/SnapshotSelectionWindow.Input.cs`
3. `src/NxTiler.Infrastructure/Hotkeys/GlobalHotkeyService.cs`

Risk:

1. Session discovery and hotkey registration logic remain concentrated in larger files.
2. Snapshot input-flow refactoring can affect interactive UX if split too aggressively.

Refactor direction:

1. Split NoMachine session discovery service by scan/parse/filter helpers.
2. Split snapshot input flow by pointer events and keyboard actions.
3. Split global hotkey service by registration lifecycle and conflict/validation helpers.

Current status:

1. `NomachineSessionService` decomposition completed:
- `NomachineSessionService.cs` reduced to root shell.
- extracted `NomachineSessionService.Discovery.cs`, `NomachineSessionService.Launch.cs`, and `NomachineSessionService.Filtering.cs`.
2. `SnapshotSelectionWindow.Input` micro-split completed:
- `SnapshotSelectionWindow.Input.cs` reduced to root shell.
- extracted `SnapshotSelectionWindow.Input.Keyboard.cs` and `SnapshotSelectionWindow.Input.Pointer.cs`.
3. `GlobalHotkeyService` micro-split completed:
- `GlobalHotkeyService.cs` reduced to root shell.
- extracted `GlobalHotkeyService.Registration.cs`, `GlobalHotkeyService.Lifecycle.cs`, and `GlobalHotkeyService.WndProc.cs`.
4. Validation after slices is green (`build`, `tests`, `governance`).

## R24: Next canonical UI/infrastructure hotspots (Done)

Files:

1. `src/NxTiler.App/Views/MaskOverlayWindow.xaml.cs`
2. `src/NxTiler.Infrastructure/Vision/YoloOutputParser.cs`
3. `src/NxTiler.App/Services/RecordingWorkflowService.EngineRouting.cs`

Risk:

1. Overlay interaction code-behind and output parsing logic remain concentrated.
2. Refactors around recording routing may touch critical stop/finalize behavior paths.

Refactor direction:

1. Split mask-overlay input and visual-update code-behind responsibilities.
2. Split YOLO output parser by tensor-shape handling and box extraction helpers.
3. Split recording engine-routing helpers by start/finalize/abort branches.

Current status:

1. `MaskOverlayWindow` code-behind decomposition completed:
- `MaskOverlayWindow.xaml.cs` reduced to root shell.
- extracted `MaskOverlayWindow.Lifecycle.cs`, `MaskOverlayWindow.Mode.cs`, and `MaskOverlayWindow.Input.cs`.
2. `YoloOutputParser` decomposition completed:
- `YoloOutputParser.cs` reduced to root shell.
- extracted `YoloOutputParser.Parse.cs` and `YoloOutputParser.Math.cs`.
3. `RecordingWorkflowService.EngineRouting` decomposition completed:
- `RecordingWorkflowService.EngineRouting.cs` reduced to root shell.
- extracted `RecordingWorkflowService.EngineRouting.Start.cs` and `RecordingWorkflowService.EngineRouting.Completion.cs`.
4. Validation after slices is green (`build`, `tests`, `governance`).

## R25: Next canonical infrastructure/UI hotspots (Done)

Files:

1. `src/NxTiler.Infrastructure/Windowing/Win32WindowQueryService.cs`
2. `src/NxTiler.App/Controls/HotkeyBox.xaml.cs`
3. `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.Finalize.Masking.cs`

Risk:

1. Window query and masking/finalize helpers remain concentrated and touch critical paths.
2. UI control-level refactors can impact user interaction behavior if not split incrementally.

Refactor direction:

1. Split Win32 window query into projection/filter/query helpers.
2. Split HotkeyBox control behavior by key-capture and text-render/update helpers.
3. Split finalize-masking helper by pipeline stage (input prep/process/final output).

Current status:

1. `Win32WindowQueryService` decomposition completed:
- `Win32WindowQueryService.cs` reduced to root shell.
- extracted `Win32WindowQueryService.Query.cs` and `Win32WindowQueryService.Regex.cs`.
2. `HotkeyBox` decomposition completed:
- `HotkeyBox.xaml.cs` reduced to root shell.
- extracted `HotkeyBox.Input.cs` and `HotkeyBox.Display.cs`.
3. Validation after slice is green (`build`, `tests`, `governance`).
4. `FfmpegRecordingEngine.Finalize.Masking` decomposition completed:
- `FfmpegRecordingEngine.Finalize.Masking.cs` reduced to orchestration shell.
- extracted `FfmpegRecordingEngine.Finalize.Masking.Filters.cs`, `FfmpegRecordingEngine.Finalize.Masking.Process.cs`, and `FfmpegRecordingEngine.Finalize.Masking.Output.cs`.
5. Validation after slices is green (`build`, `tests`, `governance`).

## R26: Next canonical app-layer hotspots (In Progress)

Files:

1. `src/NxTiler.App/Services/WorkspaceOrchestrator.Arrangement.cs`
2. `src/NxTiler.App/ViewModels/RecordingBarViewModel.State.cs`
3. `src/NxTiler.App/App.Hosting.cs`

Risk:

1. Arrangement and view-model state handlers remain concentrated and can increase merge friction.
2. App host-registration file still mixes service wiring concerns across multiple subsystems.

Refactor direction:

1. Split `WorkspaceOrchestrator.Arrangement` by arrange/apply/fit helpers.
2. Split `RecordingBarViewModel.State` by workflow-message and projection helpers.
3. Split `App.Hosting` by service-registration groups.

Current status:

1. `WorkspaceOrchestrator.Arrangement` micro-split completed:
- `WorkspaceOrchestrator.Arrangement.cs` reduced to root shell.
- extracted `WorkspaceOrchestrator.Arrangement.Targets.cs`, `WorkspaceOrchestrator.Arrangement.Flow.cs`, and `WorkspaceOrchestrator.Arrangement.Messaging.cs`.
2. Validation after slice is green (`build`, `tests`, `governance`).
3. `RecordingBarViewModel.State` micro-split completed:
- `RecordingBarViewModel.State.cs` reduced to root shell.
- extracted `RecordingBarViewModel.State.Workflow.cs` and `RecordingBarViewModel.State.Timer.cs`.
4. Validation after slice is green (`build`, `tests`, `governance`).
5. `App.Hosting` micro-split completed:
- `App.Hosting.cs` reduced to host bootstrap/orchestration shell.
- extracted `App.Hosting.Services.Core.cs`, `App.Hosting.Services.Ui.cs`, `App.Hosting.Services.Workflow.cs`, `App.Hosting.Services.ViewModels.cs`, and `App.Hosting.Services.Views.cs`.
6. Validation after slice is green (`build`, `tests`, `governance`).
7. `OverlayWindowViewModel.Commands` micro-split completed:
- `OverlayWindowViewModel.Commands.cs` reduced to root shell.
- extracted `OverlayWindowViewModel.Commands.Actions.cs`, `OverlayWindowViewModel.Commands.Toggles.cs`, and `OverlayWindowViewModel.Commands.Execution.cs`.
8. Validation after slice is green (`build`, `tests`, `governance`).
9. `VisionWorkflowService.Scan` micro-split completed:
- `VisionWorkflowService.Scan.cs` reduced to root shell.
- extracted `VisionWorkflowService.Scan.Run.cs`, `VisionWorkflowService.Scan.Fallback.cs`, and `VisionWorkflowService.Scan.Request.cs`.
10. Validation after slice is green (`build`, `tests`, `governance`).
11. Refreshed hotspot order:
- `WgcVideoRecordingEngine.Start.cs`
- `RecordingViewModel.Commands.cs`
- `LegacyAppSettings.cs`
12. Next queued task: `NX-113` (`WgcVideoRecordingEngine.Start` micro-split).
13. `WgcVideoRecordingEngine.Start` micro-split completed:
- `WgcVideoRecordingEngine.Start.cs` reduced to orchestration shell.
- extracted `WgcVideoRecordingEngine.Start.Validation.cs`, `WgcVideoRecordingEngine.Start.Session.cs`, and `WgcVideoRecordingEngine.Start.Bootstrap.cs`.
14. Validation after slice is green (`build`, `tests`, `governance`).
15. Refreshed hotspot order:
- `RecordingViewModel.Commands.cs`
- `YoloOutputParser.Parse.cs`
- `LegacyAppSettings.cs`
16. Next queued task: `NX-114` (`RecordingViewModel.Commands` micro-split).
17. `RecordingViewModel.Commands` micro-split completed:
- `RecordingViewModel.Commands.cs` reduced to root shell.
- extracted `RecordingViewModel.Commands.Settings.cs`, `RecordingViewModel.Commands.Workflow.cs`, and `RecordingViewModel.Commands.Execution.cs`.
18. Validation after slice is green (`build`, `tests`, `governance`).
19. Refreshed hotspot order:
- `YoloOutputParser.Parse.cs`
- `WorkspaceOrchestrator.Lifecycle.cs`
- `LegacyAppSettings.cs`
20. Next queued task: `NX-115` (`YoloOutputParser.Parse` micro-split).
21. `YoloOutputParser.Parse` micro-split completed:
- `YoloOutputParser.Parse.cs` reduced to root shell.
- extracted `YoloOutputParser.Parse.Shape.cs` and `YoloOutputParser.Parse.Candidates.cs`.
22. Validation after slice is green (`build`, `tests`, `governance`).
23. Refreshed hotspot order:
- `WorkspaceOrchestrator.Lifecycle.cs`
- `OverlayWindow.xaml.cs`
- `LegacyAppSettings.cs`
24. Next queued task: `NX-116` (`WorkspaceOrchestrator.Lifecycle` micro-split).
25. `WorkspaceOrchestrator.Lifecycle` micro-split completed:
- `WorkspaceOrchestrator.Lifecycle.cs` reduced to root shell.
- extracted `WorkspaceOrchestrator.Lifecycle.Start.cs`, `WorkspaceOrchestrator.Lifecycle.Dispose.cs`, and `WorkspaceOrchestrator.Lifecycle.Rollback.cs`.
26. Validation after slice is green (`build`, `tests`, `governance`).
27. Refreshed hotspot order:
- `WorkspaceOrchestrator.MessageHandling.Hotkeys.cs`
- `OverlayWindow.xaml.cs`
- `LegacyAppSettings.cs`
28. Next queued task: `NX-117` (`WorkspaceOrchestrator.MessageHandling.Hotkeys` micro-split).
29. `WorkspaceOrchestrator.MessageHandling.Hotkeys` micro-split completed:
- `WorkspaceOrchestrator.MessageHandling.Hotkeys.cs` reduced to root shell.
- extracted `WorkspaceOrchestrator.MessageHandling.Hotkeys.Routing.cs`, `WorkspaceOrchestrator.MessageHandling.Hotkeys.Recording.cs`, and `WorkspaceOrchestrator.MessageHandling.Hotkeys.CaptureVision.cs`.
30. Validation after slice is green (`build`, `tests`, `governance`).
31. `OverlayWindow` lifecycle micro-split completed:
- lifecycle handlers moved from `OverlayWindow.xaml.cs` to `OverlayWindow.Lifecycle.cs`.
32. Validation after slice is green (`build`, `tests`, `governance`).
33. Refreshed hotspot order:
- `LegacyAppSettings.cs`
- `TemplateVisionEngine.cs`
- `YoloOutputParser.Parse.Candidates.cs`
34. Next queued task: `NX-119` (`LegacyAppSettings` property-group micro-split).
35. `LegacyAppSettings` property-group micro-split completed:
- `LegacyAppSettings.cs` reduced to singleton/type shell.
- extracted `LegacyAppSettings.Filters.cs`, `LegacyAppSettings.Layout.cs`, `LegacyAppSettings.OverlayUi.cs`, and `LegacyAppSettings.Collections.cs`.
36. Validation after slice is green (`build`, `tests`, `governance`).
37. Refreshed hotspot order:
- `TemplateVisionEngine.cs`
- `YoloOutputParser.Parse.Candidates.cs`
- `RecordingBarViewModel.State.Workflow.cs`
38. Next queued task: `NX-120` (`TemplateVisionEngine` micro-split).
39. `TemplateVisionEngine` micro-split completed:
- `TemplateVisionEngine.cs` converted to partial root shell.
- extracted `TemplateVisionEngine.Detection.cs` and `TemplateVisionEngine.Configuration.cs`.
40. Validation after slice is green (`build`, `tests`, `governance`).
41. Refreshed hotspot order:
- `RecordingWorkflowService.EngineRouting.Start.cs`
- `YoloOutputParser.Parse.Candidates.cs`
- `RecordingBarViewModel.State.Workflow.cs`
42. Next queued task: `NX-121` (`RecordingWorkflowService.EngineRouting.Start` micro-split).
43. `RecordingWorkflowService.EngineRouting.Start` micro-split completed:
- `RecordingWorkflowService.EngineRouting.Start.cs` reduced to root shell.
- extracted `RecordingWorkflowService.EngineRouting.Start.Ffmpeg.cs` and `RecordingWorkflowService.EngineRouting.Start.Engine.cs`.
44. Validation after slice is green (`build`, `tests`, `governance`).
45. Refreshed hotspot order:
- `RecordingBarViewModel.State.Workflow.cs`
- `YoloOutputParser.Parse.Candidates.cs`
- `FfmpegRecordingEngine.Start.cs`
46. Next queued task: `NX-122` (`RecordingBarViewModel.State.Workflow` micro-split).
47. `RecordingBarViewModel.State.Workflow` micro-split completed:
- `RecordingBarViewModel.State.Workflow.cs` reduced to state-dispatch shell.
- extracted `RecordingBarViewModel.State.Workflow.Projection.cs`.
48. Validation after slice is green (`build`, `tests`, `governance`).
49. Refreshed hotspot order:
- `SnapshotSelectionWindow.Input.Pointer.cs`
- `YoloOutputParser.Parse.Candidates.cs`
- `FfmpegRecordingEngine.Start.cs`
50. Next queued task: `NX-123` (`SnapshotSelectionWindow.Input.Pointer` micro-split).
51. `SnapshotSelectionWindow.Input.Pointer` micro-split completed:
- `SnapshotSelectionWindow.Input.Pointer.cs` reduced to root shell.
- extracted `SnapshotSelectionWindow.Input.Pointer.Down.cs`, `SnapshotSelectionWindow.Input.Pointer.Drag.cs`, and `SnapshotSelectionWindow.Input.Pointer.Context.cs`.
52. Validation after slice is green (`build`, `tests`, `governance`).
53. Refreshed hotspot order:
- `YoloOutputParser.Parse.Candidates.cs`
- `FfmpegRecordingEngine.Start.cs`
- `OverlayTrackingService.cs`
54. Next queued task: `NX-124` (`YoloOutputParser.Parse.Candidates` micro-split).
55. `YoloOutputParser.Parse.Candidates` micro-split completed:
- candidate orchestration kept in `YoloOutputParser.Parse.Candidates.cs`.
- extracted `YoloOutputParser.Parse.Candidates.Scoring.cs` and `YoloOutputParser.Parse.Candidates.Bounds.cs`.
56. Validation after slice is green (`build`, `tests`, `governance`).
57. Refreshed hotspot order:
- `DashboardViewModel.Snapshot.cs`
- `FfmpegRecordingEngine.Start.cs`
- `OverlayTrackingService.cs`
58. Next queued task: `NX-125` (`DashboardViewModel.Snapshot` micro-split).
59. `DashboardViewModel.Snapshot` micro-split completed:
- `DashboardViewModel.Snapshot.cs` reduced to root shell.
- extracted `DashboardViewModel.Snapshot.Messages.cs` and `DashboardViewModel.Snapshot.Projection.cs`.
60. Validation after slice is green (`build`, `tests`, `governance`).
61. Refreshed hotspot order:
- `OverlayTrackingService.cs`
- `FfmpegRecordingEngine.Start.cs`
- `WgcVideoRecordingEngine.Lifecycle.cs`
62. Next queued task: `NX-126` (`OverlayTrackingService` lifecycle micro-split).
63. `OverlayTrackingService` lifecycle micro-split completed:
- `OverlayTrackingService.cs` reduced to core state/event shell.
- extracted `OverlayTrackingService.Start.cs` and `OverlayTrackingService.Stop.cs`.
64. Validation after slice is green (`build`, `tests`, `governance`).
65. Refreshed hotspot order:
- `FfmpegRecordingEngine.Start.cs`
- `Win32WindowControlService.cs`
- `WgcVideoRecordingEngine.Lifecycle.cs`
66. Next queued task: `NX-127` (`FfmpegRecordingEngine.Start` micro-split).
67. `FfmpegRecordingEngine.Start` micro-split completed:
- `FfmpegRecordingEngine.Start.cs` reduced to orchestration shell.
- extracted `FfmpegRecordingEngine.Start.Validation.cs`, `FfmpegRecordingEngine.Start.Geometry.cs`, and `FfmpegRecordingEngine.Start.Session.cs`.
68. Validation after slice is green (`build`, `tests`, `governance`).
69. Refreshed hotspot order:
- `MainWindow.xaml.cs`
- `Win32WindowControlService.cs`
- `WgcVideoRecordingEngine.Lifecycle.cs`
70. Next queued task: `NX-128` (`MainWindow.xaml.cs` lifecycle micro-split).
71. `MainWindow` lifecycle micro-split completed:
- lifecycle handlers moved from `MainWindow.xaml.cs` to `MainWindow.Lifecycle.cs`.
72. Validation after slice is green (`build`, `tests`, `governance`).
73. Refreshed hotspot order:
- `Win32WindowControlService.cs`
- `WgcVideoRecordingEngine.Lifecycle.cs`
- `WgcCaptureService.Bitmap.cs`
74. Next queued task: `NX-129` (`Win32WindowControlService` command-surface micro-split).
75. `Win32WindowControlService` command-surface micro-split completed:
- `Win32WindowControlService.cs` reduced to partial root shell.
- extracted `Win32WindowControlService.Commands.cs`, `Win32WindowControlService.Placement.cs`, and `Win32WindowControlService.Bounds.cs`.
76. Validation after slice is green (`build`, `tests`, `governance`).
77. Refreshed hotspot order:
- `WgcVideoRecordingEngine.Lifecycle.cs`
- `AppSettingsSnapshot.cs`
- `OverlayWindow.Tracking.Lifecycle.cs`
78. Next queued task: `NX-130` (`WgcVideoRecordingEngine.Lifecycle` micro-split).
79. `WgcVideoRecordingEngine.Lifecycle` micro-split completed:
- `WgcVideoRecordingEngine.Lifecycle.cs` reduced to partial root shell.
- extracted `WgcVideoRecordingEngine.Lifecycle.StopInternal.cs` and `WgcVideoRecordingEngine.Lifecycle.Cleanup.cs`.
80. Validation after slice is green (`build`, `tests`, `governance`).
81. Refreshed hotspot order:
- `AppSettingsSnapshot.cs`
- `OverlayWindow.Tracking.Lifecycle.cs`
- `WgcCaptureService.Bitmap.cs`
82. Next queued task: `NX-131` (`AppSettingsSnapshot` defaults-factory micro-split).
83. `AppSettingsSnapshot` defaults-factory micro-split completed:
- `AppSettingsSnapshot.cs` reduced to partial root shell.
- extracted `AppSettingsSnapshot.DefaultSections.cs` and `AppSettingsSnapshot.Factory.cs`.
84. Validation after slice is green (`build`, `tests`, `governance`).
85. Refreshed hotspot order:
- `OverlayWindow.Tracking.Lifecycle.cs`
- `WgcCaptureService.Bitmap.cs`
- `FfmpegSetupService.Download.cs`
86. Next queued task: `NX-132` (`OverlayWindow.Tracking.Lifecycle` micro-split).
87. `OverlayWindow.Tracking.Lifecycle` micro-split completed:
- `OverlayWindow.Tracking.Lifecycle.cs` reduced to partial root shell.
- extracted `OverlayWindow.Tracking.Lifecycle.Visibility.cs`, `OverlayWindow.Tracking.Lifecycle.Start.cs`, and `OverlayWindow.Tracking.Lifecycle.Stop.cs`.
88. Validation after slice is green (`build`, `tests`, `governance`).
89. Refreshed hotspot order:
- `WgcCaptureService.Bitmap.cs`
- `FfmpegSetupService.Download.cs`
- `RecordingWorkflowService.EngineRouting.Start.Engine.cs`
90. Next queued task: `NX-133` (`WgcCaptureService.Bitmap` micro-split).
91. `WgcCaptureService.Bitmap` micro-split completed:
- `WgcCaptureService.Bitmap.cs` reduced to partial root shell.
- extracted `WgcCaptureService.Bitmap.Output.cs`, `WgcCaptureService.Bitmap.Masks.cs`, and `WgcCaptureService.Bitmap.Encoding.cs`.
92. Validation after slice is green (`build`, `tests`, `governance`).
93. Refreshed hotspot order:
- `FfmpegSetupService.Download.cs`
- `RecordingWorkflowService.EngineRouting.Start.Engine.cs`
- `AppSettingsSnapshot.Factory.cs`
94. Next queued task: `NX-134` (`FfmpegSetupService.Download` micro-split).
95. `FfmpegSetupService.Download` micro-split completed:
- `FfmpegSetupService.Download.cs` reduced to partial root shell.
- extracted `FfmpegSetupService.Download.Flow.cs`, `FfmpegSetupService.Download.Transfer.cs`, `FfmpegSetupService.Download.Extract.cs`, and `FfmpegSetupService.Download.Cleanup.cs`.
96. Validation after slice is green (`build`, `tests`, `governance`).
97. Refreshed hotspot order:
- `RecordingWorkflowService.EngineRouting.Start.Engine.cs`
- `SnapshotSelectionWindow.Selection.cs`
- `AppSettingsSnapshot.Factory.cs`
98. Next queued task: `NX-135` (`RecordingWorkflowService.EngineRouting.Start.Engine` micro-split).
99. `RecordingWorkflowService.EngineRouting.Start.Engine` micro-split completed:
- `RecordingWorkflowService.EngineRouting.Start.Engine.cs` reduced to orchestration shell.
- extracted `RecordingWorkflowService.EngineRouting.Start.Engine.Preferred.cs` and `RecordingWorkflowService.EngineRouting.Start.Engine.Legacy.cs`.
100. Validation after slice is green (`build`, `tests`, `governance`).
101. Refreshed hotspot order:
- `SnapshotSelectionWindow.Selection.cs`
- `AppSettingsSnapshot.Factory.cs`
- `DashboardViewModel.Lifecycle.cs`
102. Next queued task: `NX-136` (`SnapshotSelectionWindow.Selection` micro-split).
103. `SnapshotSelectionWindow.Selection` micro-split completed:
- `SnapshotSelectionWindow.Selection.cs` reduced to partial root shell.
- extracted `SnapshotSelectionWindow.Selection.Region.cs`, `SnapshotSelectionWindow.Selection.MaskCreate.cs`, and `SnapshotSelectionWindow.Selection.MaskMove.cs`.
104. Validation after slice is green (`build`, `tests`, `governance`).
105. Refreshed hotspot order:
- `AppSettingsSnapshot.Factory.cs`
- `DashboardViewModel.Lifecycle.cs`
- `SnapshotSelectionWindow.xaml.cs`
106. Next queued task: `NX-137` (`AppSettingsSnapshot.Factory` micro-split).
107. `AppSettingsSnapshot.Factory` micro-split completed:
- `AppSettingsSnapshot.Factory.cs` reduced to base snapshot-construction flow.
- extracted `AppSettingsSnapshot.Factory.Hotkeys.cs` and `AppSettingsSnapshot.Factory.Features.cs`.
108. Validation after slice is green (`build`, `tests`, `governance`).
109. Refreshed hotspot order:
- `DashboardViewModel.Lifecycle.cs`
- `WgcVideoRecordingEngine.Masking.cs`
- `SnapshotSelectionWindow.xaml.cs`
110. Next queued task: `NX-138` (`DashboardViewModel.Lifecycle` micro-split).
111. Refactor Freeze Gate approved:
- `NX-138` is the final allowed Wave 6 micro-split task.
- after `NX-138` completion, no new `NX-1xx` micro-split tasks are queued.
- execution focus switches to functional waves (`NX-011/012`, `NX-020/021/022`, `NX-030/031/032`).
112. `DashboardViewModel.Lifecycle` micro-split completed:
- `DashboardViewModel.Lifecycle.cs` reduced to partial root shell.
- extracted `DashboardViewModel.Lifecycle.Activate.cs`, `DashboardViewModel.Lifecycle.Deactivate.cs`, `DashboardViewModel.Lifecycle.Dispose.cs`, and `DashboardViewModel.Lifecycle.Messaging.cs`.
113. Validation after slice is green (`build`, `tests`, `governance`).
114. Wave 6 refactor track is now closed by gate; feature-delivery baseline (`NX-011..NX-032`) is complete in current roadmap.

## Recommended execution order

1. Start next roadmap cycle from post-`NX-032` hardening/product goals (no open `NX-0xx/1xx/2xx/3xx` items in current baseline).

## Guardrails

1. No external behavior changes during refactor waves.
2. Every refactor PR must keep `docs/` in sync and preserve green gates:
- build
- tests
- governance check
- perf matrix
3. After `NX-138`, Wave 6 refactor work is allowed only to unblock functional delivery.
