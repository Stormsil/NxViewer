# NxTiler Changelog

All notable architecture and process changes are documented here.

## [Unreleased]

### Added

1. Evergreen roadmap and backlog framework:
- `docs/roadmap/ROADMAP.md`
- `docs/roadmap/BACKLOG.md`
2. Architecture baseline and target docs:
- `docs/architecture/00-current-state.md`
- `docs/architecture/01-target-architecture.md`
- `docs/architecture/02-integration-map.md`
3. Initial ADR set:
- `docs/architecture/adr/ADR-001-canonical-codebase-and-runtime.md`
- `docs/architecture/adr/ADR-002-monorepo-library-integration.md`
4. Testing and operations docs:
- `docs/testing/TEST-PLAN.md`
- `docs/ops/RUNBOOK.md`
5. PR template with mandatory docs checklist:
- `.github/pull_request_template.md`
6. Build governance files:
- `Directory.Build.props`
- `Directory.Build.targets`
7. Legacy freeze policy:
- `LEGACY_ROOT_FREEZE.md`
8. Capture workflow implementation (first pass):
- `src/NxTiler.Infrastructure/Capture/WgcCaptureService.cs`
- `src/NxTiler.App/Services/CaptureWorkflowService.cs`
- `src/NxTiler.App/Services/WorkspaceOrchestrator.MessageHandling.cs` (`HotkeyAction.InstantSnapshot` route)
9. Region snapshot interactive overlay (first pass):
- `src/NxTiler.App/Services/ISnapshotSelectionService.cs`
- `src/NxTiler.App/Services/SnapshotSelectionService.cs`
- `src/NxTiler.App/Services/SnapshotSelectionResult.cs`
- `src/NxTiler.App/Views/SnapshotSelectionWindow.xaml`
- `src/NxTiler.App/Views/SnapshotSelectionWindow.xaml.cs`
- `tests/NxTiler.Tests/CaptureWorkflowServiceTests.cs`
10. Configurable capture hotkeys:
- `HotkeysSettings` expanded with `InstantSnapshot` and `RegionSnapshot`
- Hotkey registration wiring in `GlobalHotkeyService`
- Hotkeys UI updates in `HotkeysPage` / `HotkeysViewModel`
- normalization fallback for missing hotkey fields in `JsonSettingsService`
11. Recording engine dual-path (first pass):
- `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.cs`
- `src/NxTiler.App/Services/RecordingWorkflowService.cs` routing by `FeatureFlags.UseWgcRecordingEngine`
- `src/NxTiler.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs` registration
- `tests/NxTiler.Tests/RecordingWorkflowServiceTests.cs` feature-flag routing test
12. WGC recording pause/resume hardening (workflow level):
- explicit test coverage for mask propagation to `IVideoRecordingEngine.StopAsync`
- explicit test coverage for discard path via `IVideoRecordingEngine.AbortAsync`
13. Recording fallback hardening:
- auto-fallback to legacy `FfmpegRecordingEngine` when WGC engine start fails
- workflow diagnostics for fallback transitions
- test coverage for `WGC start fail -> legacy start` path
14. Vision workflow baseline:
- `src/NxTiler.Application/Abstractions/IVisionWorkflowService.cs`
- `src/NxTiler.App/Services/VisionWorkflowService.cs`
- hotkey route in `src/NxTiler.App/Services/WorkspaceOrchestrator.MessageHandling.cs` (`ToggleVisionMode`)
- unit tests in `tests/NxTiler.Tests/VisionWorkflowServiceTests.cs`
15. Template fallback vision engine:
- `src/NxTiler.Infrastructure/Vision/TemplateVisionEngine.cs`
- DI registration of `IVisionEngine` in infrastructure
- template root support via `%LocalAppData%\NxTiler\VisionTemplates` and env var `NXTILER_VISION_TEMPLATES`
16. Vision hotkey settings integration:
- `HotkeysSettings.ToggleVision`
- runtime registration in `GlobalHotkeyService`
- Hotkeys UI and conflict validation updates
- settings normalization/migration defaults for missing `ToggleVision`
17. New architecture decision record:
- `docs/architecture/adr/ADR-003-vision-workflow-and-template-fallback.md`
18. YOLO engine staged baseline:
- `src/NxTiler.Infrastructure/Vision/YoloVisionEngine.cs` (phase-1)
- DI registration in `src/NxTiler.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs`
- workflow fallback `yolo -> template` in `src/NxTiler.App/Services/VisionWorkflowService.cs`
19. Vision settings expansion and UI wiring:
- `VisionSettings.TemplateDirectory`
- `VisionSettings.YoloModelPath`
- settings page/viewmodel support in `src/NxTiler.App/Views/Pages/SettingsPage.xaml` and `src/NxTiler.App/ViewModels/SettingsViewModel.cs`
20. Additional vision workflow tests:
- yolo failure with template fallback
- yolo failure without template fallback
- settings normalization for missing vision path fields
21. New architecture decision record:
- `docs/architecture/adr/ADR-004-yolo-staged-rollout-with-template-fallback.md`
22. YOLO ONNX phase-2 implementation:
- `src/NxTiler.Infrastructure/Vision/YoloVisionEngine.cs` now performs ONNX inference and postprocessing
- added ONNX runtime dependency in `src/NxTiler.Infrastructure/NxTiler.Infrastructure.csproj`
- added unit tests in `tests/NxTiler.Tests/YoloVisionEngineTests.cs`
23. New architecture decision record:
- `docs/architecture/adr/ADR-005-yolo-onnx-runtime-cpu-baseline.md`
24. Overlay tracking baseline:
- `src/NxTiler.Application/Abstractions/IOverlayTrackingService.cs`
- `src/NxTiler.App/Services/OverlayTrackingService.cs`
- `src/NxTiler.Domain/Overlay/OverlayTrackingRequest.cs`
- `src/NxTiler.Domain/Overlay/OverlayTrackingState.cs`
- integration in `src/NxTiler.App/Views/OverlayWindow.xaml.cs`
- tests in `tests/NxTiler.Tests/OverlayTrackingServiceTests.cs`
25. New architecture decision record:
- `docs/architecture/adr/ADR-006-overlay-tracking-service-baseline.md`
26. Overlay visibility policy hardening:
- `src/NxTiler.App/Services/ICursorPositionProvider.cs`
- `src/NxTiler.App/Services/Win32CursorPositionProvider.cs`
- updated `src/NxTiler.App/Services/OverlayTrackingService.cs`
- expanded tests in `tests/NxTiler.Tests/OverlayTrackingServiceTests.cs`
- settings compatibility test in `tests/NxTiler.Tests/SettingsServiceTests.cs`
27. New architecture decision record:
- `docs/architecture/adr/ADR-007-overlay-visibility-and-scaling-baseline.md`
28. Overlay anchor model baseline:
- `OverlayPoliciesSettings.Anchor` and `OverlayTrackingRequest.Anchor`
- anchor-aware placement and monitor clamping in `OverlayTrackingService`
- expanded overlay scaling/anchor tests in `tests/NxTiler.Tests/OverlayTrackingServiceTests.cs`
- settings normalization test for invalid anchor in `tests/NxTiler.Tests/SettingsServiceTests.cs`
29. Perf regression baseline tooling:
- `scripts/perf/perf-smoke.ps1`
- `scripts/perf/perf-regression-matrix.ps1`
- `docs/testing/PERF-BASELINE.md`
- output artifacts:
  - `artifacts/perf/perf-smoke-debug-latest.json`
  - `artifacts/perf/perf-smoke-release-latest.json`
30. CI hardening gate:
- `.github/workflows/ci.yml` (`build + tests + perf matrix + artifacts upload`)
31. Canonical track governance hardening:
- `scripts/governance/validate-canonical-track.ps1`
- CI check step before build in `.github/workflows/ci.yml`
- PR checklist line in `.github/pull_request_template.md`
32. Deprecated path cleanup:
- removed unused `src/NxTiler.Domain/Overlay/OverlayVisibilityPolicy.cs`
33. New architecture decision record:
- `docs/architecture/adr/ADR-008-canonical-track-governance.md`
34. Release baseline tooling:
- `scripts/release/verify-release-baseline.ps1`
- `docs/release/RELEASE-BASELINE.md`
35. Refactor planning artifacts:
- `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
- new backlog tasks `NX-060..NX-065` for decomposition wave
36. Shared FFmpeg process support refactor:
- `src/NxTiler.Infrastructure/Recording/FfmpegProcessSupport.cs`
- `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.cs`
- `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.cs`
- `tests/NxTiler.Tests/FfmpegProcessSupportTests.cs`
37. Settings normalization decomposition:
- `src/NxTiler.Infrastructure/Settings/JsonSettingsService.cs` refactored into section normalizers with unchanged behavior.
38. Recording workflow state machine extraction:
- `src/NxTiler.App/Services/RecordingWorkflowStateMachine.cs`
- `src/NxTiler.App/Services/RecordingWorkflowService.cs`
- `tests/NxTiler.Tests/RecordingWorkflowStateMachineTests.cs`
39. YOLO internal decomposition refactor:
- `src/NxTiler.Infrastructure/Vision/IYoloSessionProvider.cs`
- `src/NxTiler.Infrastructure/Vision/IYoloPreprocessor.cs`
- `src/NxTiler.Infrastructure/Vision/IYoloOutputParser.cs`
- `src/NxTiler.Infrastructure/Vision/YoloSessionProvider.cs`
- `src/NxTiler.Infrastructure/Vision/YoloPreprocessor.cs`
- `src/NxTiler.Infrastructure/Vision/YoloOutputParser.cs`
- `src/NxTiler.Infrastructure/Vision/YoloDetectionPostProcessor.cs`
- `tests/NxTiler.Tests/YoloPreprocessorTests.cs`
- `tests/NxTiler.Tests/YoloOutputParserTests.cs`
- `tests/NxTiler.Tests/YoloDetectionPostProcessorTests.cs`
40. `WindowCaptureCL` exception-catalog split (first `NX-065` slice):
- removed `src/WindowCaptureCL/API/Exceptions.cs`
- added:
  - `src/WindowCaptureCL/API/CaptureExceptionBase.cs`
  - `src/WindowCaptureCL/API/CapturePlatformExceptions.cs`
  - `src/WindowCaptureCL/API/CaptureResourceExceptions.cs`
  - `src/WindowCaptureCL/API/CaptureValidationExceptions.cs`
  - `src/WindowCaptureCL/API/CaptureSessionExceptions.cs`
41. `WindowManagerCL` `WindowControl` structural split (second `NX-065` slice):
- `src/WindowManagerCL/WindowManagerCL/API/WindowControl.cs` converted to partial type.
- command/mutation methods moved to `src/WindowManagerCL/WindowManagerCL/API/WindowControl.Commands.cs`.
42. `WindowManagerCL` `WindowControl` structural split extended:
- query properties moved to `src/WindowManagerCL/WindowManagerCL/API/WindowControl.Properties.cs`
- hierarchy methods moved to `src/WindowManagerCL/WindowManagerCL/API/WindowControl.Hierarchy.cs`
- object overrides moved to `src/WindowManagerCL/WindowManagerCL/API/WindowControl.Overrides.cs`
43. `ImageSearchCL` tracking-session structural split (third `NX-065` slice):
- `src/ImageSearchCL/Core/TrackingSession.cs` converted to partial type
- processing/event internals moved to `src/ImageSearchCL/Core/TrackingSession.Processing.cs`
44. `ImageSearchCL` debug-overlay structural split (fourth `NX-065` slice):
- `src/ImageSearchCL/Infrastructure/DebugOverlay.cs` converted to partial type
- interop declarations moved to `src/ImageSearchCL/Infrastructure/DebugOverlay.Interop.cs`
45. Tracking-session lifecycle extraction:
- lifecycle/public-session methods moved to `src/ImageSearchCL/Core/TrackingSession.Lifecycle.cs`
46. Debug-overlay runtime/rendering extraction:
- runtime/orchestration methods moved to `src/ImageSearchCL/Infrastructure/DebugOverlay.Runtime.cs`
- overlay window rendering type moved to `src/ImageSearchCL/Infrastructure/DebugOverlay.OverlayWindow.cs`
47. Capture-session structural extraction:
- `src/WindowCaptureCL/Core/CaptureSession.cs` converted to partial core shell
- WGC init/events moved to `src/WindowCaptureCL/Core/CaptureSession.Wgc.cs`
- frame capture/conversion moved to `src/WindowCaptureCL/Core/CaptureSession.Frame.cs`
- lifecycle/config/disposal moved to `src/WindowCaptureCL/Core/CaptureSession.Lifecycle.cs`
48. Tracking-session state/event extraction:
- state transition and event-marshalling internals moved to `src/ImageSearchCL/Core/TrackingSession.State.cs`
49. Capture-facade structural extraction:
- `src/WindowCaptureCL/API/CaptureFacade.cs` converted to partial root
- window capture methods moved to `src/WindowCaptureCL/API/CaptureFacade.Window.cs`
- monitor/region capture methods moved to `src/WindowCaptureCL/API/CaptureFacade.Screen.cs`
- file-save methods moved to `src/WindowCaptureCL/API/CaptureFacade.File.cs`
- defaults/settings methods moved to `src/WindowCaptureCL/API/CaptureFacade.Settings.cs`
- utility/introspection methods moved to `src/WindowCaptureCL/API/CaptureFacade.Utility.cs`
- `WindowInfo` moved to `src/WindowCaptureCL/API/WindowInfo.cs`
50. WindowManager exception-catalog split:
- removed `src/WindowManagerCL/WindowManagerCL/API/Exceptions.cs`
- added:
  - `src/WindowManagerCL/WindowManagerCL/API/WindowManagerException.cs`
  - `src/WindowManagerCL/WindowManagerCL/API/WindowNotFoundException.cs`
  - `src/WindowManagerCL/WindowManagerCL/API/WindowOperationException.cs`
  - `src/WindowManagerCL/WindowManagerCL/API/InvalidWindowHandleException.cs`
51. DashboardViewModel structural split (`NX-062` start):
- `src/NxTiler.App/ViewModels/DashboardViewModel.cs` now holds state/properties/ctor
- lifecycle moved to `src/NxTiler.App/ViewModels/DashboardViewModel.Lifecycle.cs`
- command handlers moved to `src/NxTiler.App/ViewModels/DashboardViewModel.Commands.cs`
- snapshot/message projection moved to `src/NxTiler.App/ViewModels/DashboardViewModel.Snapshot.cs`
52. Dashboard command-service extraction:
- `src/NxTiler.App/Services/IDashboardWorkspaceCommandService.cs`
- `src/NxTiler.App/Services/DashboardWorkspaceCommandService.cs`
- `src/NxTiler.App/Services/IDashboardRecordingCommandService.cs`
- `src/NxTiler.App/Services/DashboardRecordingCommandService.cs`
- app DI registration in `src/NxTiler.App/App.xaml.cs`
- command-service unit tests:
  - `tests/NxTiler.Tests/DashboardWorkspaceCommandServiceTests.cs`
  - `tests/NxTiler.Tests/DashboardRecordingCommandServiceTests.cs`
53. Dashboard command execution policy extraction:
- `src/NxTiler.App/Services/IDashboardCommandExecutionService.cs`
- `src/NxTiler.App/Services/DashboardCommandExecutionService.cs`
- app DI registration in `src/NxTiler.App/App.xaml.cs`
- unit tests:
  - `tests/NxTiler.Tests/DashboardCommandExecutionServiceTests.cs`
54. Roadmap/backlog refresh after refactor pass:
- `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md` refreshed with post-`NX-062` hotspot inventory.
- `docs/roadmap/BACKLOG.md` updated (`NX-062` done, `NX-066` done).
55. ImageSearch API facade decomposition (`NX-066`):
- `src/ImageSearchCL/API/Search.cs`
- `src/ImageSearchCL/API/Search.For.cs`
- `src/ImageSearchCL/API/Search.Find.cs`
- `src/ImageSearchCL/API/Search.SearchBuilder.cs`
- `src/ImageSearchCL/API/ImageSearchConfiguration.cs`
- `src/ImageSearchCL/API/ImageSearchConfiguration.Properties.cs`
- `src/ImageSearchCL/API/TemplateMatchMode.cs`
- `src/ImageSearchCL/API/Images.cs`
- `src/ImageSearchCL/API/Images.Collection.cs`
- `src/ImageSearchCL/API/Images.Factory.cs`
- `src/ImageSearchCL/API/Images.Lifecycle.cs`
- parity tests:
  - `tests/NxTiler.Tests/ImageSearchApiFacadeTests.cs`
56. ImageSearch infrastructure decomposition (`NX-065` slice):
- `src/ImageSearchCL/Infrastructure/TemplateMatchingEngine.cs` (partial shell)
- `src/ImageSearchCL/Infrastructure/TemplateMatchingEngine.Single.cs`
- `src/ImageSearchCL/Infrastructure/TemplateMatchingEngine.Multi.cs`
57. WindowCapture DirectX decomposition (`NX-065` slice):
- `src/WindowCaptureCL/Infrastructure/DirectX/FrameProcessor.cs` (partial shell)
- `src/WindowCaptureCL/Infrastructure/DirectX/FrameProcessor.Pool.cs`
- `src/WindowCaptureCL/Infrastructure/DirectX/FrameProcessor.Conversion.cs`
- `src/WindowCaptureCL/Infrastructure/DirectX/FrameProcessor.Async.cs`
58. WindowCapture DPI-helper decomposition (`NX-065` slice):
- `src/WindowCaptureCL/Infrastructure/DpiHelper.cs` (partial shell)
- `src/WindowCaptureCL/Infrastructure/DpiHelper.Awareness.cs`
- `src/WindowCaptureCL/Infrastructure/DpiHelper.Query.cs`
- `src/WindowCaptureCL/Infrastructure/DpiHelper.Native.cs`
59. ImageSearch API contract decomposition (`NX-065` slice):
- `src/ImageSearchCL/API/FindResult.cs` (partial shell)
- `src/ImageSearchCL/API/FindResult.Geometry.cs`
- `src/ImageSearchCL/API/FindResult.Equality.cs`
- `src/ImageSearchCL/API/IObjectSearch.cs` (partial shell)
- `src/ImageSearchCL/API/IObjectSearch.Events.cs`
- `src/ImageSearchCL/API/IObjectSearch.Contract.cs`
60. ImageSearch tracking-config decomposition (`NX-065` slice):
- `src/ImageSearchCL/API/TrackingConfiguration.cs` (partial shell)
- `src/ImageSearchCL/API/TrackingConfiguration.Validation.cs`
- `src/ImageSearchCL/API/TrackingConfiguration.Withers.cs`
61. ImageSearch frame-queue decomposition (`NX-065` slice):
- `src/ImageSearchCL/Infrastructure/FrameQueue.cs` (partial shell)
- `src/ImageSearchCL/Infrastructure/FrameQueue.Operations.cs`
- `src/ImageSearchCL/Infrastructure/FrameQueue.Lifecycle.cs`
62. WindowCapture DirectX device-manager decomposition (`NX-065` slice):
- `src/WindowCaptureCL/Infrastructure/DirectX/DirectXDeviceManager.cs` (partial shell)
- `src/WindowCaptureCL/Infrastructure/DirectX/DirectXDeviceManager.Initialization.cs`
- `src/WindowCaptureCL/Infrastructure/DirectX/DirectXDeviceManager.Resources.cs`
- `src/WindowCaptureCL/Infrastructure/DirectX/DirectXDeviceManager.Lifecycle.cs`
63. WindowManager window-facade decomposition (`NX-065` slice):
- `src/WindowManagerCL/WindowManagerCL/API/Window.cs` (partial shell)
- `src/WindowManagerCL/WindowManagerCL/API/Window.Find.cs`
- `src/WindowManagerCL/WindowManagerCL/API/Window.Handle.cs`
64. ImageSearch reference-image decomposition (`NX-065` slice):
- `src/ImageSearchCL/API/ReferenceImage.cs` (partial shell)
- `src/ImageSearchCL/API/ReferenceImage.Factory.cs`
- `src/ImageSearchCL/API/ReferenceImage.Lifecycle.cs`
65. ImageSearch one-shot facade decomposition (`NX-065` slice):
- `src/ImageSearchCL/API/ImageSearch.cs` (partial shell)
- `src/ImageSearchCL/API/ImageSearch.Find.cs`
- `src/ImageSearchCL/API/ImageSearch.FindAll.cs`
66. WindowManager core-finder decomposition (`NX-065` slice):
- `src/WindowManagerCL/WindowManagerCL/Core/WindowFinder.cs` (partial shell)
- `src/WindowManagerCL/WindowManagerCL/Core/WindowFinder.Find.cs`
- `src/WindowManagerCL/WindowManagerCL/Core/WindowFinder.Filter.cs`
67. ImageSearch overlay-window decomposition (`NX-065` slice):
- `src/ImageSearchCL/Infrastructure/DebugOverlay.OverlayWindow.cs` (partial shell)
- `src/ImageSearchCL/Infrastructure/DebugOverlay.OverlayWindow.Rendering.cs`
- `src/ImageSearchCL/Infrastructure/DebugOverlay.OverlayWindow.Native.cs`
68. Windowing interop decomposition (`NX-065` slice):
- `src/WindowManagerCL/WindowManagerCL/Infrastructure/WinApi.cs` (partial shell)
- `src/WindowManagerCL/WindowManagerCL/Infrastructure/WinApi.Functions.cs`
- `src/WindowManagerCL/WindowManagerCL/Infrastructure/WinApi.Structures.cs`
- `src/WindowManagerCL/WindowManagerCL/Infrastructure/WinApi.Constants.cs`
- `src/WindowCaptureCL/Infrastructure/WGC/WindowEnumerator.cs` (partial shell)
- `src/WindowCaptureCL/Infrastructure/WGC/WindowEnumerator.Interop.cs`
- `src/WindowCaptureCL/Infrastructure/WGC/WindowInfo.cs`
69. WindowCapture pool split (`NX-065` slice):
- `src/WindowCaptureCL/Infrastructure/ObjectPool.cs` (single responsibility `ObjectPool<T>`)
- `src/WindowCaptureCL/Infrastructure/KeyedObjectPool.cs`
70. Recording engine decomposition (`NX-067` slice 1):
- `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.cs` (partial orchestration shell)
- `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.Arguments.cs`
- `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.Finalize.cs`
- `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.Lifecycle.cs`
- `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.cs` (partial orchestration shell)
- `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.Arguments.cs`
- `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.FramePump.cs`
- `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.Lifecycle.cs`
- `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.Masking.cs`
- `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.Geometry.cs`
71. Recording workflow service decomposition (`NX-068` slice 1):
- `src/NxTiler.App/Services/RecordingWorkflowService.cs` (partial root shell)
- `src/NxTiler.App/Services/RecordingWorkflowService.Commands.cs`
- `src/NxTiler.App/Services/RecordingWorkflowService.Overlay.cs`
- `src/NxTiler.App/Services/RecordingWorkflowService.State.cs`
- `src/NxTiler.App/Services/RecordingWorkflowService.Execution.cs`
72. Shared recording argument builder (`NX-067` slice 2):
- `src/NxTiler.Infrastructure/Recording/FfmpegArgumentBuilder.cs`
- updated consumers:
  - `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.Arguments.cs`
  - `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.Arguments.cs`
- focused parity tests:
  - `tests/NxTiler.Tests/FfmpegArgumentBuilderTests.cs`
73. Overlay window decomposition (`NX-069` slice 1):
- `src/NxTiler.App/Views/OverlayWindow.xaml.cs` (partial root shell)
- `src/NxTiler.App/Views/OverlayWindow.Interaction.cs`
- `src/NxTiler.App/Views/OverlayWindow.Positioning.cs`
- `src/NxTiler.App/Views/OverlayWindow.Messaging.cs`
- `src/NxTiler.App/Views/OverlayWindow.Tracking.cs`
74. Snapshot selection window decomposition (`NX-070` slice 1):
- `src/NxTiler.App/Views/SnapshotSelectionWindow.xaml.cs` (partial root shell)
- `src/NxTiler.App/Views/SnapshotSelectionWindow.Input.cs`
- `src/NxTiler.App/Views/SnapshotSelectionWindow.Selection.cs`
- `src/NxTiler.App/Views/SnapshotSelectionWindow.Layout.cs`
- `src/NxTiler.App/Views/SnapshotSelectionWindow.Conversion.cs`
75. Recording workflow engine-routing helper (`NX-068` slice 2):
- `src/NxTiler.App/Services/RecordingWorkflowService.EngineRouting.cs`
- `src/NxTiler.App/Services/RecordingWorkflowService.Commands.cs` delegates ffmpeg resolve/start/finalize/abort routing.
76. OverlayWindowViewModel decomposition (`NX-071` slice 1):
- `src/NxTiler.App/ViewModels/OverlayWindowViewModel.cs` (partial root shell)
- `src/NxTiler.App/ViewModels/OverlayWindowViewModel.Commands.cs`
- `src/NxTiler.App/ViewModels/OverlayWindowViewModel.Messaging.cs`
- `src/NxTiler.App/ViewModels/OverlayWindowViewModel.Snapshot.cs`
- `src/NxTiler.App/ViewModels/OverlayWindowViewModel.Lifecycle.cs`
77. Recording view-model decomposition (`NX-072` slice 1):
- `src/NxTiler.App/ViewModels/RecordingViewModel.cs` (partial root shell)
- `src/NxTiler.App/ViewModels/RecordingViewModel.Commands.cs`
- `src/NxTiler.App/ViewModels/RecordingViewModel.State.cs`
- `src/NxTiler.App/ViewModels/RecordingViewModel.Lifecycle.cs`
- `src/NxTiler.App/ViewModels/RecordingBarViewModel.cs` (partial root shell)
- `src/NxTiler.App/ViewModels/RecordingBarViewModel.Commands.cs`
- `src/NxTiler.App/ViewModels/RecordingBarViewModel.State.cs`
- `src/NxTiler.App/ViewModels/RecordingBarViewModel.Lifecycle.cs`
78. Overlay tracking service decomposition (`NX-073` slice 1):
- `src/NxTiler.App/Services/OverlayTrackingService.cs` (partial root shell)
- `src/NxTiler.App/Services/OverlayTrackingService.Lifecycle.cs`
- `src/NxTiler.App/Services/OverlayTrackingService.Loop.cs`
- `src/NxTiler.App/Services/OverlayTrackingService.State.cs`
- `src/NxTiler.App/Services/OverlayTrackingService.Visibility.cs`
- `src/NxTiler.App/Services/OverlayTrackingService.Geometry.cs`
79. Capture service decomposition (`NX-074` slice 1):
- `src/NxTiler.Infrastructure/Capture/WgcCaptureService.cs` (partial root shell)
- `src/NxTiler.Infrastructure/Capture/WgcCaptureService.WindowPreparation.cs`
- `src/NxTiler.Infrastructure/Capture/WgcCaptureService.Geometry.cs`
- `src/NxTiler.Infrastructure/Capture/WgcCaptureService.Bitmap.cs`
- `src/NxTiler.Infrastructure/Capture/WgcCaptureService.Output.cs`
80. Win32 native interop decomposition (`NX-075` slice 1):
- `src/NxTiler.Infrastructure/Native/Win32Native.cs` (partial root shell)
- `src/NxTiler.Infrastructure/Native/Win32Native.Interop.cs`
- `src/NxTiler.Infrastructure/Native/Win32Native.Constants.cs`
- `src/NxTiler.Infrastructure/Native/Win32Native.Structures.cs`
- `src/NxTiler.Infrastructure/Native/Win32Native.Helpers.cs`
81. Vision workflow service decomposition (`NX-076` slice 1):
- `src/NxTiler.App/Services/VisionWorkflowService.cs` (partial root shell)
- `src/NxTiler.App/Services/VisionWorkflowService.Commands.cs`
- `src/NxTiler.App/Services/VisionWorkflowService.Scan.cs`
- `src/NxTiler.App/Services/VisionWorkflowService.EngineResolution.cs`
- `src/NxTiler.App/Services/VisionWorkflowService.TargetResolution.cs`
- `src/NxTiler.App/Services/VisionWorkflowService.Execution.cs`
82. Workspace orchestrator command/message decomposition (`NX-077` slice 1):
- `src/NxTiler.App/Services/WorkspaceOrchestrator.Commands.cs` (partial shell)
- `src/NxTiler.App/Services/WorkspaceOrchestrator.Commands.Arrangement.cs`
- `src/NxTiler.App/Services/WorkspaceOrchestrator.Commands.Navigation.cs`
- `src/NxTiler.App/Services/WorkspaceOrchestrator.Commands.Sessions.cs`
- `src/NxTiler.App/Services/WorkspaceOrchestrator.MessageHandling.cs` (partial shell)
- `src/NxTiler.App/Services/WorkspaceOrchestrator.MessageHandling.Events.cs`
- `src/NxTiler.App/Services/WorkspaceOrchestrator.MessageHandling.Hotkeys.cs`
- `src/NxTiler.App/Services/WorkspaceOrchestrator.MessageHandling.Execution.cs`
- `src/NxTiler.App/Services/WorkspaceOrchestrator.MessageHandling.Registration.cs`
83. Settings service decomposition (`NX-078` slice 1):
- `src/NxTiler.Infrastructure/Settings/JsonSettingsService.cs` (partial root shell)
- `src/NxTiler.Infrastructure/Settings/JsonSettingsService.Loading.cs`
- `src/NxTiler.Infrastructure/Settings/JsonSettingsService.Persistence.cs`
- `src/NxTiler.Infrastructure/Settings/JsonSettingsService.Normalization.cs`
84. FFmpeg finalize-flow decomposition (`NX-079` slice 1):
- `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.Finalize.cs` (orchestration shell)
- `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.Finalize.Single.cs`
- `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.Finalize.Concat.cs`
- `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.Finalize.Masking.cs`
85. Settings view-model decomposition (`NX-080` slice 1):
- `src/NxTiler.App/ViewModels/SettingsViewModel.cs` (partial root shell)
- `src/NxTiler.App/ViewModels/SettingsViewModel.Commands.cs`
- `src/NxTiler.App/ViewModels/SettingsViewModel.Snapshot.cs`
86. WGC recording root-shell decomposition (`NX-081` slice 1):
- `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.cs` (partial root shell)
- `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.Start.cs`
- `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.Control.cs`
- `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.Stop.cs`
87. Workspace orchestrator lifecycle-core decomposition (`NX-082` slice 1):
- `src/NxTiler.App/Services/WorkspaceOrchestrator.cs` (state/constructor shell)
- `src/NxTiler.App/Services/WorkspaceOrchestrator.Lifecycle.cs`
- `src/NxTiler.App/Services/WorkspaceOrchestrator.Monitoring.cs`
88. Legacy settings decomposition (`NX-083` slice 1):
- `src/NxTiler.Infrastructure/Legacy/LegacyAppSettings.cs` (partial root shell)
- `src/NxTiler.Infrastructure/Legacy/LegacyAppSettings.Hotkeys.cs`
- `src/NxTiler.Infrastructure/Legacy/LegacyAppSettings.Recording.cs`
89. FFmpeg recording root-shell decomposition (`NX-084` slice 1):
- `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.cs` (root shell)
- `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.Start.cs`
- `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.Segments.cs`
90. Logs view-model decomposition (`NX-085` slice 1):
- `src/NxTiler.App/ViewModels/LogsViewModel.cs` (partial root shell)
- `src/NxTiler.App/ViewModels/LogsViewModel.Filtering.cs`
- `src/NxTiler.App/ViewModels/LogsViewModel.Lifecycle.cs`
- `src/NxTiler.App/ViewModels/LogsViewModel.Commands.cs`
91. App bootstrap decomposition (`NX-086` slice 1):
- `src/NxTiler.App/App.xaml.cs` (root shell)
- `src/NxTiler.App/App.Hosting.cs`
- `src/NxTiler.App/App.Lifecycle.cs`
- `src/NxTiler.App/App.Errors.cs`
92. Recording workflow command-slice decomposition (`NX-087` slice 1):
- `src/NxTiler.App/Services/RecordingWorkflowService.Commands.cs` (root shell)
- `src/NxTiler.App/Services/RecordingWorkflowService.Commands.MaskEditing.cs`
- `src/NxTiler.App/Services/RecordingWorkflowService.Commands.Control.cs`
- `src/NxTiler.App/Services/RecordingWorkflowService.Commands.Completion.cs`
93. Settings normalization micro-split (`NX-088` slice 1):
- `src/NxTiler.Infrastructure/Settings/JsonSettingsService.Normalization.cs` (root shell)
- `src/NxTiler.Infrastructure/Settings/JsonSettingsService.Normalization.Core.cs`
- `src/NxTiler.Infrastructure/Settings/JsonSettingsService.Normalization.General.cs`
- `src/NxTiler.Infrastructure/Settings/JsonSettingsService.Normalization.Features.cs`
- `src/NxTiler.Infrastructure/Settings/JsonSettingsService.Normalization.Overlay.cs`
94. Arrangement service decomposition (`NX-089` slice 1):
- `src/NxTiler.Application/Services/ArrangementService.cs` (orchestration shell)
- `src/NxTiler.Application/Services/ArrangementService.Grid.cs`
- `src/NxTiler.Application/Services/ArrangementService.Focus.cs`
- `src/NxTiler.Application/Services/ArrangementService.Layout.cs`
95. Dashboard command-slice micro-split (`NX-090` slice 1):
- `src/NxTiler.App/ViewModels/DashboardViewModel.Commands.cs` (root shell)
- `src/NxTiler.App/ViewModels/DashboardViewModel.Commands.Workspace.cs`
- `src/NxTiler.App/ViewModels/DashboardViewModel.Commands.Recording.cs`
- `src/NxTiler.App/ViewModels/DashboardViewModel.Commands.State.cs`
- `src/NxTiler.App/ViewModels/DashboardViewModel.Commands.Execution.cs`
96. Overlay tracking code-behind micro-split (`NX-091` slice 1):
- `src/NxTiler.App/Views/OverlayWindow.Tracking.cs` (root shell)
- `src/NxTiler.App/Views/OverlayWindow.Tracking.Lifecycle.cs`
- `src/NxTiler.App/Views/OverlayWindow.Tracking.State.cs`
- `src/NxTiler.App/Views/OverlayWindow.Tracking.Scale.cs`
97. Hotkeys view-model decomposition (`NX-092` slice 1):
- `src/NxTiler.App/ViewModels/HotkeysViewModel.cs` (root shell)
- `src/NxTiler.App/ViewModels/HotkeysViewModel.Commands.cs`
- `src/NxTiler.App/ViewModels/HotkeysViewModel.Snapshot.cs`
98. Recording overlay service decomposition (`NX-093` slice 1):
- `src/NxTiler.App/Services/RecordingOverlayService.cs` (root shell)
- `src/NxTiler.App/Services/RecordingOverlayService.Lifecycle.cs`
- `src/NxTiler.App/Services/RecordingOverlayService.Mode.cs`
- `src/NxTiler.App/Services/RecordingOverlayService.Masks.cs`
- `src/NxTiler.App/Services/RecordingOverlayService.Ui.cs`
99. Window event monitor service decomposition (`NX-094` slice 1):
- `src/NxTiler.Infrastructure/Windowing/WindowEventMonitorService.cs` (root shell)
- `src/NxTiler.Infrastructure/Windowing/WindowEventMonitorService.Lifecycle.cs`
- `src/NxTiler.Infrastructure/Windowing/WindowEventMonitorService.Events.cs`
100. Mask overlay view-model decomposition (`NX-095` slice 1):
- `src/NxTiler.App/ViewModels/MaskOverlayViewModel.cs` (root shell)
- `src/NxTiler.App/ViewModels/MaskOverlayViewModel.Masks.cs`
- `src/NxTiler.App/ViewModels/MaskOverlayViewModel.State.cs`
- `src/NxTiler.App/ViewModels/MaskOverlayViewModel.Commands.cs`
101. FFmpeg setup service decomposition (`NX-096` slice 1):
- `src/NxTiler.Infrastructure/Recording/FfmpegSetupService.cs` (root shell)
- `src/NxTiler.Infrastructure/Recording/FfmpegSetupService.Resolve.cs`
- `src/NxTiler.Infrastructure/Recording/FfmpegSetupService.Download.cs`
- `src/NxTiler.Infrastructure/Recording/FfmpegSetupService.Probe.cs`
102. YOLO vision-engine micro-decomposition (`NX-097` slice 1):
- `src/NxTiler.Infrastructure/Vision/YoloVisionEngine.cs` (root shell)
- `src/NxTiler.Infrastructure/Vision/YoloVisionEngine.Detection.cs`
- `src/NxTiler.Infrastructure/Vision/YoloVisionEngine.Configuration.cs`
- `src/NxTiler.Infrastructure/Vision/YoloVisionEngine.Lifecycle.cs`
103. Capture workflow service decomposition (`NX-098` slice 1):
- `src/NxTiler.App/Services/CaptureWorkflowService.cs` (root shell)
- `src/NxTiler.App/Services/CaptureWorkflowService.Commands.cs`
- `src/NxTiler.App/Services/CaptureWorkflowService.TargetResolution.cs`
- `src/NxTiler.App/Services/CaptureWorkflowService.Execution.cs`
104. NoMachine session service decomposition (`NX-099` slice 1):
- `src/NxTiler.Infrastructure/Nomachine/NomachineSessionService.cs` (root shell)
- `src/NxTiler.Infrastructure/Nomachine/NomachineSessionService.Discovery.cs`
- `src/NxTiler.Infrastructure/Nomachine/NomachineSessionService.Launch.cs`
- `src/NxTiler.Infrastructure/Nomachine/NomachineSessionService.Filtering.cs`
105. Snapshot selection input-flow micro-split (`NX-100` slice 1):
- `src/NxTiler.App/Views/SnapshotSelectionWindow.Input.cs` (root shell)
- `src/NxTiler.App/Views/SnapshotSelectionWindow.Input.Keyboard.cs`
- `src/NxTiler.App/Views/SnapshotSelectionWindow.Input.Pointer.cs`
106. Global hotkey service micro-split (`NX-101` slice 1):
- `src/NxTiler.Infrastructure/Hotkeys/GlobalHotkeyService.cs` (root shell)
- `src/NxTiler.Infrastructure/Hotkeys/GlobalHotkeyService.Registration.cs`
- `src/NxTiler.Infrastructure/Hotkeys/GlobalHotkeyService.Lifecycle.cs`
- `src/NxTiler.Infrastructure/Hotkeys/GlobalHotkeyService.WndProc.cs`
107. Mask overlay window code-behind decomposition (`NX-102` slice 1):
- `src/NxTiler.App/Views/MaskOverlayWindow.xaml.cs` (root shell)
- `src/NxTiler.App/Views/MaskOverlayWindow.Lifecycle.cs`
- `src/NxTiler.App/Views/MaskOverlayWindow.Mode.cs`
- `src/NxTiler.App/Views/MaskOverlayWindow.Input.cs`
108. YOLO output parser decomposition (`NX-103` slice 1):
- `src/NxTiler.Infrastructure/Vision/YoloOutputParser.cs` (root shell)
- `src/NxTiler.Infrastructure/Vision/YoloOutputParser.Parse.cs`
- `src/NxTiler.Infrastructure/Vision/YoloOutputParser.Math.cs`
109. Recording engine-routing micro-split (`NX-104` slice 1):
- `src/NxTiler.App/Services/RecordingWorkflowService.EngineRouting.cs` (root shell)
- `src/NxTiler.App/Services/RecordingWorkflowService.EngineRouting.Start.cs`
- `src/NxTiler.App/Services/RecordingWorkflowService.EngineRouting.Completion.cs`
110. Win32 window-query service decomposition (`NX-105` slice 1):
- `src/NxTiler.Infrastructure/Windowing/Win32WindowQueryService.cs` (root shell)
- `src/NxTiler.Infrastructure/Windowing/Win32WindowQueryService.Query.cs`
- `src/NxTiler.Infrastructure/Windowing/Win32WindowQueryService.Regex.cs`
111. HotkeyBox control decomposition (`NX-106` slice 1):
- `src/NxTiler.App/Controls/HotkeyBox.xaml.cs` (root shell)
- `src/NxTiler.App/Controls/HotkeyBox.Input.cs`
- `src/NxTiler.App/Controls/HotkeyBox.Display.cs`
112. FFmpeg finalize-masking flow decomposition (`NX-107` slice 1):
- `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.Finalize.Masking.cs` (orchestration shell)
- `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.Finalize.Masking.Filters.cs`
- `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.Finalize.Masking.Process.cs`
- `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.Finalize.Masking.Output.cs`
113. Workspace-orchestrator arrangement micro-split (`NX-108` slice 1):
- `src/NxTiler.App/Services/WorkspaceOrchestrator.Arrangement.cs` (root shell)
- `src/NxTiler.App/Services/WorkspaceOrchestrator.Arrangement.Targets.cs`
- `src/NxTiler.App/Services/WorkspaceOrchestrator.Arrangement.Flow.cs`
- `src/NxTiler.App/Services/WorkspaceOrchestrator.Arrangement.Messaging.cs`
114. Recording-bar state-surface micro-split (`NX-109` slice 1):
- `src/NxTiler.App/ViewModels/RecordingBarViewModel.State.cs` (root shell)
- `src/NxTiler.App/ViewModels/RecordingBarViewModel.State.Workflow.cs`
- `src/NxTiler.App/ViewModels/RecordingBarViewModel.State.Timer.cs`
115. App host-registration micro-split (`NX-110` slice 1):
- `src/NxTiler.App/App.Hosting.cs` (host bootstrap + orchestration shell)
- `src/NxTiler.App/App.Hosting.Services.Core.cs`
- `src/NxTiler.App/App.Hosting.Services.Ui.cs`
- `src/NxTiler.App/App.Hosting.Services.Workflow.cs`
- `src/NxTiler.App/App.Hosting.Services.ViewModels.cs`
- `src/NxTiler.App/App.Hosting.Services.Views.cs`
116. Overlay-window-viewmodel command-surface micro-split (`NX-111` slice 1):
- `src/NxTiler.App/ViewModels/OverlayWindowViewModel.Commands.cs` (root shell)
- `src/NxTiler.App/ViewModels/OverlayWindowViewModel.Commands.Actions.cs`
- `src/NxTiler.App/ViewModels/OverlayWindowViewModel.Commands.Toggles.cs`
- `src/NxTiler.App/ViewModels/OverlayWindowViewModel.Commands.Execution.cs`
117. Vision workflow scan-flow micro-split (`NX-112` slice 1):
- `src/NxTiler.App/Services/VisionWorkflowService.Scan.cs` (root shell)
- `src/NxTiler.App/Services/VisionWorkflowService.Scan.Run.cs`
- `src/NxTiler.App/Services/VisionWorkflowService.Scan.Fallback.cs`
- `src/NxTiler.App/Services/VisionWorkflowService.Scan.Request.cs`
118. WGC recording start-flow micro-split (`NX-113` slice 1):
- `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.Start.cs` (orchestration shell)
- `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.Start.Validation.cs`
- `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.Start.Session.cs`
- `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.Start.Bootstrap.cs`
119. Recording-viewmodel command-surface micro-split (`NX-114` slice 1):
- `src/NxTiler.App/ViewModels/RecordingViewModel.Commands.cs` (root shell)
- `src/NxTiler.App/ViewModels/RecordingViewModel.Commands.Settings.cs`
- `src/NxTiler.App/ViewModels/RecordingViewModel.Commands.Workflow.cs`
- `src/NxTiler.App/ViewModels/RecordingViewModel.Commands.Execution.cs`
120. YOLO parser parse-flow micro-split (`NX-115` slice 1):
- `src/NxTiler.Infrastructure/Vision/YoloOutputParser.Parse.cs` (root shell)
- `src/NxTiler.Infrastructure/Vision/YoloOutputParser.Parse.Shape.cs`
- `src/NxTiler.Infrastructure/Vision/YoloOutputParser.Parse.Candidates.cs`
121. Workspace-orchestrator lifecycle micro-split (`NX-116` slice 1):
- `src/NxTiler.App/Services/WorkspaceOrchestrator.Lifecycle.cs` (root shell)
- `src/NxTiler.App/Services/WorkspaceOrchestrator.Lifecycle.Start.cs`
- `src/NxTiler.App/Services/WorkspaceOrchestrator.Lifecycle.Dispose.cs`
- `src/NxTiler.App/Services/WorkspaceOrchestrator.Lifecycle.Rollback.cs`
122. Workspace-orchestrator hotkey-routing micro-split (`NX-117` slice 1):
- `src/NxTiler.App/Services/WorkspaceOrchestrator.MessageHandling.Hotkeys.cs` (root shell)
- `src/NxTiler.App/Services/WorkspaceOrchestrator.MessageHandling.Hotkeys.Routing.cs`
- `src/NxTiler.App/Services/WorkspaceOrchestrator.MessageHandling.Hotkeys.Recording.cs`
- `src/NxTiler.App/Services/WorkspaceOrchestrator.MessageHandling.Hotkeys.CaptureVision.cs`
123. Overlay-window lifecycle micro-split (`NX-118` slice 1):
- `src/NxTiler.App/Views/OverlayWindow.xaml.cs` (lifecycle extracted)
- `src/NxTiler.App/Views/OverlayWindow.Lifecycle.cs`
124. Legacy app-settings property-group micro-split (`NX-119` slice 1):
- `src/NxTiler.Infrastructure/Legacy/LegacyAppSettings.cs` (singleton/type shell)
- `src/NxTiler.Infrastructure/Legacy/LegacyAppSettings.Filters.cs`
- `src/NxTiler.Infrastructure/Legacy/LegacyAppSettings.Layout.cs`
- `src/NxTiler.Infrastructure/Legacy/LegacyAppSettings.OverlayUi.cs`
- `src/NxTiler.Infrastructure/Legacy/LegacyAppSettings.Collections.cs`
125. Template-vision-engine detection-flow micro-split (`NX-120` slice 1):
- `src/NxTiler.Infrastructure/Vision/TemplateVisionEngine.cs` (partial root shell)
- `src/NxTiler.Infrastructure/Vision/TemplateVisionEngine.Detection.cs`
- `src/NxTiler.Infrastructure/Vision/TemplateVisionEngine.Configuration.cs`
126. Recording-workflow engine-start routing micro-split (`NX-121` slice 1):
- `src/NxTiler.App/Services/RecordingWorkflowService.EngineRouting.Start.cs` (root shell)
- `src/NxTiler.App/Services/RecordingWorkflowService.EngineRouting.Start.Ffmpeg.cs`
- `src/NxTiler.App/Services/RecordingWorkflowService.EngineRouting.Start.Engine.cs`
127. Recording-bar workflow-state micro-split (`NX-122` slice 1):
- `src/NxTiler.App/ViewModels/RecordingBarViewModel.State.Workflow.cs` (state-dispatch shell)
- `src/NxTiler.App/ViewModels/RecordingBarViewModel.State.Workflow.Projection.cs`
128. Snapshot-selection pointer-flow micro-split (`NX-123` slice 1):
- `src/NxTiler.App/Views/SnapshotSelectionWindow.Input.Pointer.cs` (root shell)
- `src/NxTiler.App/Views/SnapshotSelectionWindow.Input.Pointer.Down.cs`
- `src/NxTiler.App/Views/SnapshotSelectionWindow.Input.Pointer.Drag.cs`
- `src/NxTiler.App/Views/SnapshotSelectionWindow.Input.Pointer.Context.cs`
129. YOLO parser candidate-flow micro-split (`NX-124` slice 1):
- `src/NxTiler.Infrastructure/Vision/YoloOutputParser.Parse.Candidates.cs`
- `src/NxTiler.Infrastructure/Vision/YoloOutputParser.Parse.Candidates.Scoring.cs`
- `src/NxTiler.Infrastructure/Vision/YoloOutputParser.Parse.Candidates.Bounds.cs`
130. Dashboard snapshot-projection micro-split (`NX-125` slice 1):
- `src/NxTiler.App/ViewModels/DashboardViewModel.Snapshot.cs` (root shell)
- `src/NxTiler.App/ViewModels/DashboardViewModel.Snapshot.Messages.cs`
- `src/NxTiler.App/ViewModels/DashboardViewModel.Snapshot.Projection.cs`
131. Overlay-tracking-service lifecycle micro-split (`NX-126` slice 1):
- `src/NxTiler.App/Services/OverlayTrackingService.cs` (core state/event shell)
- `src/NxTiler.App/Services/OverlayTrackingService.Start.cs`
- `src/NxTiler.App/Services/OverlayTrackingService.Stop.cs`
132. FFmpeg-recording start-flow micro-split (`NX-127` slice 1):
- `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.Start.cs` (orchestration shell)
- `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.Start.Validation.cs`
- `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.Start.Geometry.cs`
- `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.Start.Session.cs`
133. Main-window lifecycle micro-split (`NX-128` slice 1):
- `src/NxTiler.App/MainWindow.xaml.cs` (lifecycle extracted)
- `src/NxTiler.App/MainWindow.Lifecycle.cs`

### Changed

1. `NxTiler.sln` now includes:
- `WindowCaptureCL`
- `WindowManagerCL`
- `ImageSearchCL`
- `ImageSearchCL.WindowCapture`
2. `NxTiler.Infrastructure` now references `WindowCaptureCL` and `ImageSearchCL` via `ProjectReference`.
3. `NxTiler.App` and `NxTiler.Tests` now target `x64` to align with `WindowCaptureCL` runtime architecture.
4. `NxTiler.Infrastructure` and `ImageSearchCL.WindowCapture` now target `x64` to align with `WindowCaptureCL`.
5. `AppSettingsSnapshot` schema default moved to `4`.
6. `TemplateVisionEngine` now resolves template directory by settings first, then env var fallback.
7. `YoloVisionEngine` upgraded from validation stub to executable detection pipeline (preprocess + decode + NMS).
8. Overlay window behavior changed from static/manual-only placement to tracked placement when target window is available.
9. `OverlayWindow` now applies proportional scale transform from tracked dimensions when `OverlayPolicies.ScaleWithWindow` is enabled.
10. `JsonSettingsService` now normalizes legacy `OverlayPolicies.HideOnHover` into canonical `OverlayVisibilityMode`.
11. `JsonSettingsService` now normalizes invalid overlay anchor values to `OverlayAnchor.TopLeft`.
12. PR template checklist now includes perf smoke validation line.
13. PR template checklist now includes canonical governance validation line.
14. `YoloVisionEngine` now acts as orchestration shell over extracted internal components while keeping `IVisionEngine` unchanged.
15. `WindowCaptureCL` public exception API is now physically split across multiple files with unchanged type names and constructors.
16. `WindowManagerCL` public `WindowControl` API is unchanged, with command methods extracted to partial for cleaner query/command separation.
17. `WindowManagerCL` `WindowControl` is now fully organized across dedicated partial files (`Commands/Properties/Hierarchy/Overrides`) with unchanged public behavior.
18. `ImageSearchCL` `TrackingSession` public behavior is unchanged while internal processing pipeline is isolated in a dedicated partial file.
19. `ImageSearchCL` `DebugOverlay` behavior is unchanged while interop declarations are isolated in a dedicated partial file.
20. `ImageSearchCL` `TrackingSession` behavior is unchanged while lifecycle/public-session methods are isolated in a dedicated partial file.
21. `ImageSearchCL` `DebugOverlay` behavior is unchanged while runtime/orchestration and overlay rendering are isolated in dedicated partial files.
22. `WindowCaptureCL` `CaptureSession` behavior is unchanged while internal WGC, frame, and lifecycle concerns are isolated in dedicated partial files.
23. `ImageSearchCL` `TrackingSession` behavior is unchanged while state transition/event-marshalling concerns are isolated in a dedicated partial file.
24. `WindowCaptureCL` `CaptureFacade` behavior is unchanged while capture/file/settings/utility concerns are isolated in dedicated partial files.
25. `WindowManagerCL` exception behavior is unchanged while exception definitions are isolated in dedicated files.
26. `DashboardViewModel` behavior is unchanged while lifecycle/command/snapshot concerns are isolated in dedicated partial files.
27. Dashboard command behavior is unchanged while orchestration delegation moved from `DashboardViewModel` to dedicated command services.
28. Dashboard busy/error handling behavior is unchanged while execution policy moved from `DashboardViewModel` to dedicated command execution service.
29. `NX-062` is now tracked as complete in roadmap/backlog with build/test/governance verification.
30. `ImageSearchCL` API behavior is unchanged while facade/config/collection concerns are isolated into dedicated files (`NX-066`).
31. `ImageSearchCL` template-matching behavior is unchanged while single-match and multi-match/NMS concerns are isolated into dedicated partial files.
32. `WindowCaptureCL` DirectX frame-processing behavior is unchanged while pooling, conversion, and async concerns are isolated into dedicated partial files.
33. `WindowCaptureCL` DPI-helper behavior is unchanged while awareness, query/conversion, and native interop concerns are isolated into dedicated partial files.
34. `ImageSearchCL` API result and session contracts are behaviorally unchanged while geometry/equality and events/state-method declarations are isolated into dedicated partial files.
35. `ImageSearchCL` tracking-configuration behavior is unchanged while constructor, validation, and immutable-withers concerns are isolated into dedicated partial files.
36. `ImageSearchCL` frame-queue behavior is unchanged while buffering operations and lifecycle concerns are isolated into dedicated partial files.
37. `WindowCaptureCL` DirectX device-manager behavior is unchanged while initialization, staging-resource allocation, and lifecycle concerns are isolated into dedicated partial files.
38. `WindowManagerCL` window-facade behavior is unchanged while search/discovery and handle/foreground concerns are isolated into dedicated partial files.
39. `ImageSearchCL` reference-image behavior is unchanged while constructor core, factory helpers, and lifecycle/validation concerns are isolated into dedicated partial files.
40. `ImageSearchCL` one-shot search behavior is unchanged while single-match and multi-match entry points are isolated into dedicated partial files.
41. `WindowManagerCL` finder behavior is unchanged while window enumeration and filtering/regex concerns are isolated into dedicated partial files.
42. `ImageSearchCL` overlay-window behavior is unchanged while core form, layered rendering flow, and native interop concerns are isolated into dedicated partial files.
43. Win32 interop behavior is unchanged while `WindowManagerCL` `WinApi` and `WindowCaptureCL` `WindowEnumerator` concerns are isolated into dedicated partial files.
44. Pooling behavior is unchanged while `WindowCaptureCL` generic and keyed pool types are isolated into dedicated files.
45. Recording behavior is unchanged while `FfmpegRecordingEngine` and `WgcVideoRecordingEngine` are reorganized into partial orchestration shells with extracted argument/lifecycle/masking/frame-pump helpers.
46. Recording workflow behavior is unchanged while `RecordingWorkflowService` is reorganized into partial files for commands, overlay handlers, state/message transitions, and serialized-execution helpers.
47. ffmpeg command-string behavior is unchanged while both recording engines now share `FfmpegArgumentBuilder` with focused unit-test coverage.
48. Overlay window behavior is unchanged while code-behind responsibilities are split into focused partial files for interaction, persistence, messaging, and tracking.
49. Snapshot selection behavior is unchanged while code-behind responsibilities are split into focused partial files for input flow, selection/mask logic, layout, and coordinate conversion.
50. Recording workflow behavior is unchanged while engine-routing concerns are isolated into dedicated helper methods.
51. Overlay view-model behavior is unchanged while commands, lifecycle, messaging, and snapshot projection concerns are isolated into focused partial files.
52. Recording view-model behavior is unchanged while command, lifecycle, and workflow-message concerns are isolated into focused partial files.
53. Overlay tracking behavior is unchanged while lifecycle, loop, state computation, visibility policy, and geometry placement concerns are isolated into focused partial files.
54. Snapshot capture behavior is unchanged while `WgcCaptureService` concerns are isolated into focused partial files for window preparation, geometry mapping, bitmap/mask rendering, and output persistence.
55. Win32 interop behavior is unchanged while `Win32Native` concerns are isolated into focused partial files for declarations, constants, structures, and helper operations.
56. Vision workflow behavior is unchanged while `VisionWorkflowService` concerns are isolated into focused partial files for commands, scan/fallback flow, engine resolution, target lookup, and serialized execution.
57. Workspace orchestrator behavior is unchanged while command and message-handling concerns are isolated into focused partial files.
58. Settings service behavior is unchanged while `JsonSettingsService` concerns are isolated into focused partial files for loading, persistence, and normalization.
59. FFmpeg finalize behavior is unchanged while single-segment, concat, and masking concerns are isolated into focused partial files.
60. Settings UI behavior is unchanged while `SettingsViewModel` concerns are isolated into focused partial files for command actions and snapshot projection/load.
61. WGC recording behavior is unchanged while `WgcVideoRecordingEngine` root-shell concerns are isolated into focused partial files for start/control/stop orchestration.
62. Workspace orchestration behavior is unchanged while lifecycle and monitor-management concerns are isolated into focused partial files.
63. Legacy settings behavior is unchanged while `LegacyAppSettings` concerns are isolated into focused partial files for base shell, hotkeys, and recording defaults.
64. FFmpeg recording startup/segment behavior is unchanged while `FfmpegRecordingEngine` root-shell concerns are isolated into focused partial files.
65. Logs panel behavior is unchanged while `LogsViewModel` concerns are isolated into focused partial files for filtering, lifecycle subscriptions, and clipboard/clear commands.
66. App startup/shutdown behavior is unchanged while `App` bootstrap concerns are isolated into focused partial files for host wiring, lifecycle, and exception handling.
67. Recording workflow behavior is unchanged while `RecordingWorkflowService` command concerns are isolated into focused partial files for mask-edit, control, and completion/cancel paths.
68. Settings normalization behavior is unchanged while `JsonSettingsService` policies are isolated into focused partial files for core/general/feature/overlay normalization.
69. Arrangement behavior is unchanged while `ArrangementService` concerns are isolated into focused partial files for grid/focus/max-size strategy and shared layout helpers.
70. Dashboard command behavior is unchanged while `DashboardViewModel` command concerns are isolated into focused partial files for workspace/recording actions, state hooks, and busy-execution helper.
71. Overlay tracking behavior is unchanged while `OverlayWindow` tracking concerns are isolated into focused partial files for lifecycle, tracking-state projection, and scaling helpers.
72. Hotkeys UI behavior is unchanged while `HotkeysViewModel` concerns are isolated into focused partial files for save/reset commands and settings projection mapping.
73. Recording overlay behavior is unchanged while `RecordingOverlayService` concerns are isolated into focused partial files for lifecycle, mode transitions, mask extraction, and UI-invoke helper.
74. Window monitor behavior is unchanged while `WindowEventMonitorService` concerns are isolated into focused partial files for hook lifecycle and WinEvent callback/debounce dispatch.
75. Mask overlay behavior is unchanged while `MaskOverlayViewModel` concerns are isolated into focused partial files for mask operations, mode-state transitions, and messenger command actions.
76. FFmpeg setup behavior is unchanged while `FfmpegSetupService` concerns are isolated into focused partial files for resolve/save, download/extract, and PATH probe logic.
77. YOLO engine behavior is unchanged while `YoloVisionEngine` orchestration concerns are isolated into focused partial files for detection pipeline, model/label resolution, and lifecycle disposal.
78. Capture workflow behavior is unchanged while `CaptureWorkflowService` concerns are isolated into focused partial files for command flow, target resolution, and serialized execution/reporting helpers.
79. NoMachine session behavior is unchanged while `NomachineSessionService` concerns are isolated into focused partial files for discovery, launch/open, and regex filtering logic.
80. Snapshot selection behavior is unchanged while `SnapshotSelectionWindow` input concerns are isolated into focused partial files for keyboard actions and pointer interaction flow.
81. Global hotkey behavior is unchanged while `GlobalHotkeyService` concerns are isolated into focused partial files for registration, lifecycle unregistration, and Win32 message dispatch.
82. Mask overlay window behavior is unchanged while `MaskOverlayWindow` code-behind concerns are isolated into focused partial files for lifecycle, mode transitions, and pointer input.
83. YOLO output parser behavior is unchanged while `YoloOutputParser` concerns are isolated into focused partial files for shape routing/candidate extraction and numeric helper utilities.
84. Recording workflow engine-routing behavior is unchanged while `RecordingWorkflowService` routing concerns are isolated into focused partial files for resolve/start and abort/finalize paths.
85. Window query behavior is unchanged while `Win32WindowQueryService` concerns are isolated into focused partial files for query enumeration/projection and regex-cache helpers.
86. Hotkey control behavior is unchanged while `HotkeyBox` concerns are isolated into focused partial files for keyboard capture flow and display-text update projection.
87. FFmpeg masking-finalize behavior is unchanged while `FfmpegRecordingEngine` masking concerns are isolated into focused partial files for filter building, process execution, and final output promotion.
88. Workspace arrangement behavior is unchanged while `WorkspaceOrchestrator` arrangement concerns are isolated into focused partial files for target refresh/query options, arrange execution flow, and snapshot/status publishing.
89. Recording-bar behavior is unchanged while `RecordingBarViewModel` state concerns are isolated into focused partial files for workflow-state/message handling and timer projection.
90. App startup/DI behavior is unchanged while `App.Hosting` registration concerns are isolated into focused partial files for core/UI/workflow/view-model/view registrations.
91. Overlay view-model command behavior is unchanged while `OverlayWindowViewModel` command concerns are isolated into focused partial files for action handlers, toggle callbacks, and execution/error-policy flow.
92. Vision workflow scan behavior is unchanged while `VisionWorkflowService` scan concerns are isolated into focused partial files for core execution, YOLO fallback path, and request construction.
93. WGC recording start behavior is unchanged while `WgcVideoRecordingEngine` start concerns are isolated into focused partial files for validation, session initialization, and bootstrap.
94. Recording view-model command behavior is unchanged while `RecordingViewModel` command concerns are isolated into focused partial files for settings/ffmpeg actions, workflow actions, and shared busy/error execution handling.
95. YOLO parser behavior is unchanged while `YoloOutputParser` parse concerns are isolated into focused partial files for shape resolution and candidate extraction flow.
96. Workspace orchestrator lifecycle behavior is unchanged while lifecycle concerns are isolated into focused partial files for start, dispose, and rollback flows.
97. Workspace orchestrator hotkey-routing behavior is unchanged while hotkey concerns are isolated into focused partial files for routing, recording handlers, and capture/vision handlers.
98. Overlay-window behavior is unchanged while lifecycle handlers are isolated into `OverlayWindow.Lifecycle.cs`.
99. Legacy settings behavior is unchanged while `LegacyAppSettings` concerns are isolated into focused partial files for filters/layout/overlay-ui/collections.
100. Template-vision behavior is unchanged while `TemplateVisionEngine` concerns are isolated into focused partial files for detection-flow and template-resolution helpers.
101. Recording engine-start routing behavior is unchanged while `RecordingWorkflowService` start-routing concerns are isolated into focused partial files for ffmpeg resolve/download and engine start/fallback helpers.
102. Recording-bar workflow-state behavior is unchanged while per-state UI projection concerns are isolated into dedicated helper partial file.
103. Snapshot pointer-input behavior is unchanged while pointer-down/drag/context-remove concerns are isolated into focused partial files.
104. YOLO candidate parsing behavior is unchanged while scoring and bounds-projection concerns are isolated into dedicated helper partial files.
105. Dashboard snapshot/recording projection behavior is unchanged while message/debounce handlers and projection mapping concerns are isolated into focused partial files.
106. Overlay tracking lifecycle behavior is unchanged while start/update-target and stop/dispose concerns are isolated into dedicated helper partial files.
107. FFmpeg recording start behavior is unchanged while start validation, geometry alignment, and session/bootstrap concerns are isolated into focused partial files.
108. Main-window shell behavior is unchanged while lifecycle handlers are isolated into `MainWindow.Lifecycle.cs`.
109. Window-control behavior is unchanged while `Win32WindowControlService` concerns are isolated into focused partial files for command operations, placement flow, and bounds queries.
110. WGC recording lifecycle behavior is unchanged while `WgcVideoRecordingEngine` lifecycle concerns are isolated into focused partial files for stop/finalize orchestration and cleanup/reset helpers.
111. Settings-defaults behavior is unchanged while `AppSettingsSnapshot` concerns are isolated into focused partial files for root contract, default-section properties, and default-factory helpers.
112. Overlay tracking-lifecycle behavior is unchanged while `OverlayWindow` lifecycle concerns are isolated into focused partial files for visibility callback handling, tracking start/update, and stop/reset helpers.
113. Snapshot bitmap behavior is unchanged while `WgcCaptureService` bitmap concerns are isolated into focused partial files for output projection, mask rendering, and PNG encoding helpers.
114. FFmpeg setup-download behavior is unchanged while `FfmpegSetupService` download concerns are isolated into focused partial files for orchestration, transfer progress, extraction/discovery, and cleanup helpers.
115. Recording engine-start routing behavior is unchanged while `RecordingWorkflowService` start-engine concerns are isolated into focused partial files for preferred WGC startup and legacy/failure fallback helpers.
116. Snapshot selection behavior is unchanged while `SnapshotSelectionWindow` selection concerns are isolated into focused partial files for region selection, mask create/finalize, and mask move/hit-test helpers.
117. Settings default-factory behavior is unchanged while `AppSettingsSnapshot` factory concerns are isolated into focused partial files for base composition, hotkey defaults, and feature-section defaults.
118. Roadmap governance updated with `Refactor Freeze Gate`: `NX-138` marked as final Wave 6 micro-split before switching execution focus to feature-delivery waves.
119. Dashboard lifecycle behavior is unchanged while `DashboardViewModel` lifecycle concerns are isolated into focused partial files for activate/deactivate/dispose and message-registration/startup-sync helpers.
120. Capture docs were hardened for `NX-011`: added explicit instant-snapshot manual validation matrix in `docs/testing/TEST-PLAN.md` and matching runbook checklist in `docs/ops/RUNBOOK.md`.
121. Shift snapshot track (`NX-012`) gained additional cancel-path workflow coverage in `tests/NxTiler.Tests/CaptureWorkflowServiceTests.cs`.
122. Wave 1 capture docs were expanded with explicit shift-snapshot mask interaction validation matrix and runbook checklist.
123. Recording defaults updated for `NX-022`: `FeatureFlags.UseWgcRecordingEngine` now defaults to `true` in `AppSettingsSnapshot` while legacy fallback remains active.
124. Wave 2 (`NX-020/021/022`) marked complete with validated WGC start/pause/resume/stop path and fallback workflow coverage.
125. Added optional model-based YOLO smoke coverage in `tests/NxTiler.Tests/YoloModelSmokeTests.cs` (env-driven model/images).
126. Wave 3 (`NX-030/031/032`) marked complete with ONNX pipeline, fallback routing, and smoke-suite execution guidance in docs.
