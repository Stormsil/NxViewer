# NxTiler Evergreen Roadmap

Last updated: 2026-02-13  
Owner: NxTiler Core

## Mission

Build a single canonical NxTiler platform on `src/NxTiler.*` with integrated capture, recording, vision, and overlay subsystems.  
Documentation and roadmap are kept evergreen and updated with every material code change.

## Status legend

- `Planned`
- `In Progress`
- `Blocked`
- `Review`
- `Done`

## Wave overview

| Wave | Goal | Window | Status |
|---|---|---|---|
| Wave 0 | Foundation and governance | 1 week | Done |
| Wave 1 | Capture Core | 1-2 weeks | Done |
| Wave 2 | Recording Core | 1-2 weeks | Done |
| Wave 3 | Vision Core | 2+ weeks | Done |
| Wave 4 | Overlay System | 1-2 weeks | Done |
| Wave 5 | Hardening and release baseline | 1 week | Done |
| Wave 6 | Refactor and simplification | 1-2 weeks | Done |

## Wave 0: Foundation

- `NX-001` Freeze legacy root project track (`NxTiler.csproj`) and define canonical `src/*`.
- `NX-002` Add capture/vision libraries into `NxTiler.sln`.
- `NX-003` Unify build and analyzer defaults.
- `NX-004` Create evergreen docs and backlog framework.

## Wave 1: Capture Core

- `NX-010` Introduce `ICaptureService` and `ICaptureWorkflowService`.
- `NX-011` Integrate WGC instant window snapshot (maximize -> stabilize -> capture -> disk + clipboard).
- `NX-012` Shift snapshot with area selection + mask editing.

## Wave 2: Recording Core

- `NX-020` Introduce `IVideoRecordingEngine` for WGC frames -> FFmpeg stdin pipeline.
- `NX-021` Pause/resume with mask editing in pause mode.
- `NX-022` Migrate from `gdigrab` to WGC pipeline with fallback.

Progress notes:
1. `NX-020` completed: `IVideoRecordingEngine` WGC frame pipeline integrated and covered in workflow tests.
2. `NX-021` completed: pause/resume mask edit flow is stable with mask propagation/discard coverage.
3. `NX-022` completed: default recording path switched to WGC with in-run legacy fallback preserved.

## Wave 3: Vision Core

- `NX-030` Introduce `IVisionEngine` and `VisionOrchestrator`.
- `NX-031` Keep temporary template-matching fallback path.
- `NX-032` Add ONNX YOLO engine with feature flags.

Progress notes:
1. `IVisionWorkflowService` + `VisionWorkflowService` added and wired to `WorkspaceOrchestrator` hotkey route (`ToggleVisionMode`).
2. `TemplateVisionEngine` added in infrastructure (`WindowCaptureCL` frame capture + `ImageSearchCL` template matching).
3. Feature-flag aware engine resolution is active (`EnableTemplateMatchingFallback`, `EnableYoloEngine`).
4. `YoloVisionEngine` phase-1 added (model-path validation and controlled fallback path).
5. Vision settings are now configurable from UI (`SettingsPage`): confidence, preferred engine, template directory, YOLO model path, feature flags.
6. `YoloVisionEngine` phase-2 added: ONNX Runtime inference pipeline (preprocess -> model run -> parse -> NMS).
7. `NX-030` and `NX-031` are complete; `NX-032` is completed with optional model-based smoke suite for inference/parsing path validation.

## Wave 4: Overlay System

- `NX-040` Overlay tracking against window movement/resize.
- `NX-041` Visibility policies (`Always`, `OnHover`, `HideOnHover`).
- `NX-042` Overlay content scaling with target window size.

Progress notes:
1. Added `IOverlayTrackingService` contract and `OverlayTrackingService` implementation (`NX-040` done).
2. Overlay window now tracks focused/active target window geometry from `WorkspaceSnapshot`.
3. Visibility policies `Always`/`OnHover`/`HideOnHover` are applied in tracking runtime (`NX-041` done).
4. Overlay runtime now applies proportional content scaling from tracked dimensions.
5. Anchor-aware overlay placement (`TopLeft..BottomRight`) and monitor-bound clamping are active (`NX-042` done).

## Wave 5: Hardening

- `NX-050` Performance and regression pass.
- `NX-051` Cleanup deprecated paths and legacy leftovers.
- `NX-052` Final release docs and stable baseline.

Progress notes:
1. `NX-050` completed with reproducible perf regression tooling (`scripts/perf/perf-smoke.ps1`, `scripts/perf/perf-regression-matrix.ps1`).
2. CI hardening gate added in `.github/workflows/ci.yml` (build/test/perf + perf artifacts upload).
3. `NX-051` completed with canonical-track governance checks and deprecated-path cleanup.
4. `NX-052` completed with reproducible release baseline docs + verification script.

## Wave 6: Refactor and simplification

- `NX-060` Shared FFmpeg process runner for recording engines.
- `NX-061` Recording workflow state machine extraction.
- `NX-062` Dashboard orchestration split.
- `NX-063` Settings normalization decomposition.
- `NX-064` YOLO engine internal decomposition.
- `NX-065` Integrated library structural cleanup.
- `NX-066` ImageSearchCL API facade decomposition.
- `NX-067` Recording engines structural decomposition.
- `NX-068` Recording workflow service decomposition.
- `NX-069` Overlay window code-behind decomposition.
- `NX-070` Snapshot selection window decomposition.
- `NX-071` OverlayWindowViewModel decomposition.
- `NX-072` Recording view-model decomposition.
- `NX-073` OverlayTrackingService decomposition.
- `NX-074` WgcCaptureService decomposition.
- `NX-075` Win32Native decomposition.
- `NX-076` VisionWorkflowService decomposition.
- `NX-077` WorkspaceOrchestrator command/message decomposition.
- `NX-078` JsonSettingsService decomposition.
- `NX-079` FfmpegRecordingEngine finalize-flow decomposition.
- `NX-080` SettingsViewModel decomposition.
- `NX-081` WgcVideoRecordingEngine root-shell decomposition.
- `NX-082` WorkspaceOrchestrator lifecycle-core decomposition.
- `NX-083` LegacyAppSettings decomposition.
- `NX-084` FfmpegRecordingEngine root-shell decomposition.
- `NX-085` LogsViewModel decomposition.
- `NX-086` App bootstrap decomposition.
- `NX-087` RecordingWorkflowService command-slice decomposition.
- `NX-088` JsonSettingsService normalization micro-split.
- `NX-089` ArrangementService decomposition.
- `NX-090` Dashboard command-slice micro-split.
- `NX-091` Overlay tracking code-behind micro-split.
- `NX-092` HotkeysViewModel decomposition.
- `NX-093` RecordingOverlayService decomposition.
- `NX-094` WindowEventMonitorService decomposition.
- `NX-095` MaskOverlayViewModel decomposition.
- `NX-096` FfmpegSetupService decomposition.
- `NX-097` YoloVisionEngine micro-decomposition.
- `NX-098` CaptureWorkflowService decomposition.
- `NX-099` NomachineSessionService decomposition.
- `NX-100` SnapshotSelectionWindow input-flow micro-split.
- `NX-101` GlobalHotkeyService micro-split.
- `NX-102` MaskOverlayWindow code-behind decomposition.
- `NX-103` YoloOutputParser decomposition.
- `NX-104` RecordingWorkflowService engine-routing micro-split.
- `NX-105` Win32WindowQueryService decomposition.
- `NX-106` HotkeyBox control decomposition.
- `NX-107` Ffmpeg finalize-masking flow decomposition.
- `NX-108` WorkspaceOrchestrator arrangement-flow micro-split.
- `NX-109` RecordingBarViewModel state-surface micro-split.
- `NX-110` App host-registration micro-split.
- `NX-111` OverlayWindowViewModel command-surface micro-split.
- `NX-112` VisionWorkflowService scan-flow micro-split.
- `NX-113` WgcVideoRecordingEngine start-flow micro-split.
- `NX-114` RecordingViewModel command-surface micro-split.
- `NX-115` YoloOutputParser parse-flow micro-split.
- `NX-116` WorkspaceOrchestrator lifecycle micro-split.
- `NX-117` WorkspaceOrchestrator hotkey-routing micro-split.
- `NX-118` OverlayWindow lifecycle micro-split.
- `NX-119` LegacyAppSettings property-group micro-split.
- `NX-120` TemplateVisionEngine detection-flow micro-split.
- `NX-121` RecordingWorkflow engine-start routing micro-split.
- `NX-122` RecordingBar workflow-state micro-split.
- `NX-123` SnapshotSelection pointer-flow micro-split.
- `NX-124` YoloOutputParser candidate-flow micro-split.
- `NX-125` Dashboard snapshot-projection micro-split.
- `NX-126` OverlayTrackingService lifecycle micro-split.
- `NX-127` FfmpegRecordingEngine start-flow micro-split.
- `NX-128` MainWindow lifecycle micro-split.
- `NX-129` Win32WindowControlService command-surface micro-split.
- `NX-130` WgcVideoRecordingEngine lifecycle-surface micro-split.
- `NX-131` AppSettingsSnapshot defaults-factory micro-split.
- `NX-132` OverlayWindow tracking-lifecycle micro-split.
- `NX-133` WgcCaptureService bitmap-flow micro-split.
- `NX-134` FfmpegSetupService download-flow micro-split.
- `NX-135` RecordingWorkflowService engine-start helper micro-split.
- `NX-136` SnapshotSelectionWindow selection-flow micro-split.
- `NX-137` AppSettingsSnapshot factory-section micro-split.
- `NX-138` DashboardViewModel lifecycle-surface micro-split.

Planning notes:
1. Hotspot baseline captured in `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`.
2. `NX-060` completed: shared FFmpeg process support extracted and integrated.
3. `NX-063` completed: settings normalization decomposed into section helpers.
4. `NX-061` completed: recording transition logic extracted into explicit state machine.
5. `NX-064` completed: YOLO internals split into session provider, preprocessor, parser, and post-processing components.
6. `NX-065` started with refreshed integrated-library hotspot audit and decomposition slices.
7. `NX-065` progress: `WindowCaptureCL` exception catalog split into focused files with unchanged public types.
8. `NX-065` progress: `WindowManagerCL` `WindowControl` command methods extracted into dedicated partial file.
9. `NX-065` progress: `WindowControl` query/hierarchy/overrides split into dedicated partial files.
10. `NX-065` progress: `ImageSearchCL` `TrackingSession` processing pipeline extracted into dedicated partial file.
11. `NX-065` progress: `ImageSearchCL` `DebugOverlay` interop declarations extracted into dedicated partial file.
12. `NX-065` progress: `ImageSearchCL` `TrackingSession` lifecycle/public API methods extracted into dedicated partial file.
13. `NX-065` progress: `ImageSearchCL` `DebugOverlay` runtime/orchestration and overlay rendering moved into dedicated partial files.
14. `NX-065` progress: `WindowCaptureCL` `CaptureSession` split into core shell + WGC/frame/lifecycle partial files.
15. `NX-065` progress: `ImageSearchCL` `TrackingSession` state transition and event-marshalling moved into dedicated partial file.
16. `NX-065` progress: `WindowCaptureCL` `CaptureFacade` split into dedicated partial files by capture mode/file/settings/utility concerns.
17. `NX-065` progress: `WindowManagerCL` exception catalog split into dedicated files.
18. `NX-062` completed: `DashboardViewModel` split into lifecycle/commands/snapshot partial units.
19. `NX-062` progress: dashboard command orchestration delegated to workspace/recording command services with unit coverage.
20. `NX-062` progress: dashboard busy/error execution policy extracted to `IDashboardCommandExecutionService` with focused unit tests.
21. `NX-066` completed: split oversized `ImageSearchCL/API` facade files (`Search`, `ImageSearchConfiguration`, `Images`) into focused units with API parity tests.
22. `NX-065` progress: `ImageSearchCL` `TemplateMatchingEngine` split into core/single-match/multi-match-NMS partial files.
23. `NX-065` progress: `WindowCaptureCL` `FrameProcessor` split into core/pool/conversion/async partial files.
24. `NX-065` progress: `WindowCaptureCL` `DpiHelper` split into core/awareness/query/native partial files.
25. `NX-065` progress: `ImageSearchCL` `FindResult` and `IObjectSearch` contracts split into focused partial files.
26. `NX-065` progress: `ImageSearchCL` `TrackingConfiguration` split into core/validation/withers partial files.
27. `NX-065` progress: `ImageSearchCL` `FrameQueue` split into core/operations/lifecycle partial files.
28. `NX-065` progress: `WindowCaptureCL` `DirectXDeviceManager` split into core/initialization/resources/lifecycle partial files.
29. `NX-065` progress: `WindowManagerCL` `Window` facade split into core/find/handle partial files.
30. `NX-065` progress: `ImageSearchCL` `ReferenceImage` split into core/factory/lifecycle partial files.
31. `NX-065` progress: `ImageSearchCL` `ImageSearch` one-shot facade split into core/find/find-all partial files.
32. `NX-065` progress: `WindowManagerCL` `WindowFinder` split into core/find/filter partial files.
33. `NX-065` progress: `ImageSearchCL` `DebugOverlay.OverlayWindow` split into core/rendering/native partial files.
34. `NX-065` progress: `WindowManagerCL` `WinApi` split into shell/functions/structures/constants partial files.
35. `NX-065` progress: `WindowCaptureCL` `WindowEnumerator` split into shell/interop with `WindowInfo` moved to dedicated file.
36. `NX-065` progress: `WindowCaptureCL` `ObjectPool`/`KeyedObjectPool` split into dedicated files.
37. `NX-065` completed with green build/tests/governance after integrated-library structural cleanup.
38. Next refactor track queued (`NX-067..NX-070`) based on refreshed canonical hotspot inventory (`Recording engines`, `RecordingWorkflowService`, `OverlayWindow`, `SnapshotSelectionWindow`).
39. `NX-067` started: `FfmpegRecordingEngine` and `WgcVideoRecordingEngine` converted to partial orchestration shells with argument/frame-pump/masking/lifecycle concerns extracted to focused files.
40. `NX-068` started: `RecordingWorkflowService` converted to partial shell with command, overlay callback, state/message, and serialization concerns extracted to focused files.
41. `NX-067` completed: shared `FfmpegArgumentBuilder` added and covered with focused parity tests (`FfmpegArgumentBuilderTests`), validation green (`103/103` tests).
42. `NX-069` started: `OverlayWindow` split into partial files for interaction, positioning persistence, messaging handlers, and tracking/layout behavior.
43. `NX-069` completed with green validation (`build`, `103/103 tests`, `governance`) after `OverlayWindow` decomposition.
44. `NX-070` completed with green validation (`build`, `103/103 tests`, `governance`) after `SnapshotSelectionWindow` decomposition.
45. Next view-model refactor track queued (`NX-071`, `NX-072`) based on refreshed canonical hotspot inventory.
46. `NX-068` completed: `RecordingWorkflowService` now delegates engine-routing concerns to extracted helper methods (`RecordingWorkflowService.EngineRouting.cs`), validation green (`103/103` tests).
47. `NX-071` completed with green validation (`build`, `103/103 tests`, `governance`) after `OverlayWindowViewModel` decomposition.
48. `NX-072` completed with green validation (`build`, `103/103 tests`, `governance`) after `RecordingViewModel` and `RecordingBarViewModel` decomposition.
49. Next infrastructure/service refactor track queued (`NX-073`, `NX-074`) based on refreshed canonical hotspot inventory.
50. `NX-073` completed with green validation (`build`, `103/103 tests`, `governance`) after `OverlayTrackingService` decomposition.
51. `NX-074` completed with green validation (`build`, `103/103 tests`, `governance`) after `WgcCaptureService` decomposition.
52. `NX-075` completed with green validation (`build`, `103/103 tests`, `governance`) after `Win32Native` decomposition.
53. Hotspot inventory refreshed; next candidates for `R14`: `JsonSettingsService`, `VisionWorkflowService`, and `WorkspaceOrchestrator` command/message slices.
54. `NX-076` completed with green validation (`build`, `103/103 tests`, `governance`) after `VisionWorkflowService` decomposition.
55. `NX-077` completed with green validation (`build`, `103/103 tests`, `governance`) after `WorkspaceOrchestrator` command/message decomposition.
56. Hotspot inventory refreshed; next primary canonical candidate is `JsonSettingsService`.
57. `NX-078` completed with green validation (`build`, `103/103 tests`, `governance`) after `JsonSettingsService` decomposition.
58. Hotspot inventory refreshed; next canonical candidates are `FfmpegRecordingEngine.Finalize`, `SettingsViewModel`, and `WgcVideoRecordingEngine`.
59. `R16` queued with planned tasks `NX-079`, `NX-080`, and `NX-081` from refreshed hotspot inventory.
60. `NX-079` completed with green validation (`build`, `103/103 tests`, `governance`) after `FfmpegRecordingEngine` finalize-flow decomposition.
61. Hotspot inventory refreshed; next primary candidates are `SettingsViewModel` and `WgcVideoRecordingEngine`.
62. `NX-080` completed with green validation (`build`, `103/103 tests`, `governance`) after `SettingsViewModel` decomposition.
63. Hotspot inventory refreshed; next primary canonical candidate is `WgcVideoRecordingEngine`.
64. `NX-081` completed with green validation (`build`, `103/103 tests`, `governance`) after `WgcVideoRecordingEngine` root-shell decomposition.
65. Hotspot inventory refreshed; next primary canonical candidates are `WorkspaceOrchestrator.cs` and `LegacyAppSettings`.
66. `NX-082` completed with green validation (`build`, `103/103 tests`, `governance`) after `WorkspaceOrchestrator` lifecycle-core decomposition.
67. Hotspot inventory refreshed; next primary canonical candidates are `LegacyAppSettings` and `FfmpegRecordingEngine.cs`.
68. `NX-083` completed with green validation (`build`, `103/103 tests`, `governance`) after `LegacyAppSettings` decomposition.
69. Hotspot inventory refreshed; next primary canonical candidates are `FfmpegRecordingEngine.cs` and `LogsViewModel`.
70. `NX-084` completed with green validation (`build`, `103/103 tests`, `governance`) after `FfmpegRecordingEngine` root-shell decomposition.
71. Hotspot inventory refreshed; next primary canonical candidates are `LogsViewModel` and `App.xaml.cs`.
72. `NX-085` completed with green validation (`build`, `103/103 tests`, `governance`) after `LogsViewModel` decomposition.
73. `NX-086` completed with green validation (`build`, `103/103 tests`, `governance`) after app bootstrap decomposition (`App.Hosting/Lifecycle/Errors`).
74. Hotspot inventory refreshed; next primary canonical candidates are `RecordingWorkflowService.Commands` and `JsonSettingsService.Normalization` (queued as `NX-087`, `NX-088`).
75. `NX-087` completed with green validation (`build`, `103/103 tests`, `governance`) after `RecordingWorkflowService` command-slice decomposition.
76. `NX-088` completed with green validation (`build`, `103/103 tests`, `governance`) after `JsonSettingsService` normalization micro-split decomposition.
77. Hotspot inventory refreshed; next primary canonical candidate is `ArrangementService` (queued as `NX-089`).
78. `NX-089` completed with green validation (`build`, `103/103 tests`, `governance`) after `ArrangementService` decomposition.
79. Hotspot inventory refreshed; next primary canonical candidate is `DashboardViewModel.Commands` (queued as `NX-090`).
80. `NX-090` completed with green validation (`build`, `103/103 tests`, `governance`) after `DashboardViewModel` command-slice micro-split.
81. Hotspot inventory refreshed; next primary canonical candidates are `OverlayWindow.Tracking` and `HotkeysViewModel` (queued as `NX-091` next).
82. `NX-091` completed with green validation (`build`, `103/103 tests`, `governance`) after `OverlayWindow.Tracking` micro-split.
83. Hotspot inventory refreshed; next primary canonical candidate is `HotkeysViewModel` (queued as `NX-092`).
84. `NX-092` completed with green validation (`build`, `103/103 tests`, `governance`) after `HotkeysViewModel` decomposition.
85. Hotspot inventory refreshed; next primary canonical candidate is `RecordingOverlayService` (queued as `NX-093`).
86. `NX-093` completed with green validation (`build`, `103/103 tests`, `governance`) after `RecordingOverlayService` decomposition.
87. Hotspot inventory refreshed; next primary canonical candidate is `WindowEventMonitorService` (queued as `NX-094`).
88. `NX-094` completed with green validation (`build`, `103/103 tests`, `governance`) after `WindowEventMonitorService` decomposition.
89. Hotspot inventory refreshed; next primary canonical candidates are `MaskOverlayViewModel` and `FfmpegSetupService` (queued as `NX-095` next).
90. `NX-095` completed with green validation (`build`, `103/103 tests`, `governance`) after `MaskOverlayViewModel` decomposition.
91. Hotspot inventory refreshed; next primary canonical candidates are `FfmpegSetupService` and `YoloVisionEngine` (queued as `NX-096` next).
92. `NX-096` completed with green validation (`build`, `103/103 tests`, `governance`) after `FfmpegSetupService` decomposition.
93. Hotspot inventory refreshed; next primary canonical candidates are `YoloVisionEngine` and `CaptureWorkflowService` (queued as `NX-097` next).
94. `NX-097` completed with green validation (`build`, `103/103 tests`, `governance`) after `YoloVisionEngine` micro-decomposition.
95. Hotspot inventory refreshed; next primary canonical candidates are `CaptureWorkflowService` and `NomachineSessionService` (queued as `NX-098` next).
96. `NX-098` completed with green validation (`build`, `103/103 tests`, `governance`) after `CaptureWorkflowService` decomposition.
97. Hotspot inventory refreshed; next primary canonical candidate is `NomachineSessionService` (queued as `NX-099`).
98. `NX-099` completed with green validation (`build`, `103/103 tests`, `governance`) after `NomachineSessionService` decomposition.
99. Hotspot inventory refreshed; next primary canonical candidates are `SnapshotSelectionWindow.Input` and `GlobalHotkeyService` (queued as `NX-100` next).
100. `NX-100` completed with green validation (`build`, `103/103 tests`, `governance`) after `SnapshotSelectionWindow` input-flow micro-split.
101. `NX-101` completed with green validation (`build`, `103/103 tests`, `governance`) after `GlobalHotkeyService` micro-split.
102. Hotspot inventory refreshed; next primary canonical candidates are `MaskOverlayWindow.xaml.cs` and `YoloOutputParser` (queued as `NX-102` next).
103. `NX-102` completed with green validation (`build`, `103/103 tests`, `governance`) after `MaskOverlayWindow` code-behind decomposition.
104. Hotspot inventory refreshed; next primary canonical candidates are `YoloOutputParser` and `RecordingWorkflowService.EngineRouting` (queued as `NX-103` next).
105. `NX-103` completed with green validation (`build`, `103/103 tests`, `governance`) after `YoloOutputParser` decomposition.
106. Hotspot inventory refreshed; next primary canonical candidates are `RecordingWorkflowService.EngineRouting` and `Win32WindowQueryService` (queued as `NX-104` next).
107. `NX-104` completed with green validation (`build`, `103/103 tests`, `governance`) after `RecordingWorkflowService` engine-routing micro-split.
108. Hotspot inventory refreshed; next primary canonical candidates are `Win32WindowQueryService` and `HotkeyBox.xaml.cs` (queued as `NX-105` next).
109. `NX-105` completed with green validation (`build`, `103/103 tests`, `governance`) after `Win32WindowQueryService` decomposition.
110. Hotspot inventory refreshed; next primary canonical candidates are `HotkeyBox.xaml.cs` and `FfmpegRecordingEngine.Finalize.Masking` (queued as `NX-106` next).
111. `NX-106` completed with green validation (`build`, `103/103 tests`, `governance`) after `HotkeyBox` control decomposition.
112. Hotspot inventory refreshed; next primary canonical candidate is `FfmpegRecordingEngine.Finalize.Masking` (queued as `NX-107` next).
113. `NX-107` completed with green validation (`build`, `103/103 tests`, `governance`) after `FfmpegRecordingEngine` finalize-masking micro-split.
114. Hotspot inventory refreshed; next primary canonical candidate is `WorkspaceOrchestrator.Arrangement.cs` (queued as `NX-108` next).
115. `NX-108` completed with green validation (`build`, `103/103 tests`, `governance`) after `WorkspaceOrchestrator` arrangement-flow micro-split.
116. Hotspot inventory refreshed; next primary canonical candidate is `RecordingBarViewModel.State.cs` (queued as `NX-109` next).
117. `NX-109` completed with green validation (`build`, `103/103 tests`, `governance`) after `RecordingBarViewModel` state-surface micro-split.
118. Hotspot inventory refreshed; next primary canonical candidates are `App.Hosting.cs` and `OverlayWindowViewModel.Commands.cs` (queued as `NX-110` next).
119. `NX-110` completed with green validation (`build`, `103/103 tests`, `governance`) after `App.Hosting` host-registration micro-split.
120. Hotspot inventory refreshed; next primary canonical candidates are `OverlayWindowViewModel.Commands.cs` and `VisionWorkflowService.Scan.cs` (queued as `NX-111` next).
121. `NX-111` completed with green validation (`build`, `103/103 tests`, `governance`) after `OverlayWindowViewModel` command-surface micro-split.
122. Hotspot inventory refreshed; next primary canonical candidates are `VisionWorkflowService.Scan.cs` and `WgcVideoRecordingEngine.Start.cs` (queued as `NX-112` next).
123. `NX-112` completed with green validation (`build`, `103/103 tests`, `governance`) after `VisionWorkflowService` scan-flow micro-split.
124. Hotspot inventory refreshed; next primary canonical candidates are `WgcVideoRecordingEngine.Start.cs` and `RecordingViewModel.Commands.cs` (queued as `NX-113` next).
125. `NX-113` completed with green validation (`build`, `103/103 tests`, `governance`) after `WgcVideoRecordingEngine` start-flow micro-split.
126. Hotspot inventory refreshed; next primary canonical candidates are `RecordingViewModel.Commands.cs` and `YoloOutputParser.Parse.cs` (queued as `NX-114` next).
127. `NX-114` completed with green validation (`build`, `103/103 tests`, `governance`) after `RecordingViewModel` command-surface micro-split.
128. Hotspot inventory refreshed; next primary canonical candidates are `YoloOutputParser.Parse.cs` and `WorkspaceOrchestrator.Lifecycle.cs` (queued as `NX-115` next).
129. `NX-115` completed with green validation (`build`, `103/103 tests`, `governance`) after `YoloOutputParser` parse-flow micro-split.
130. Hotspot inventory refreshed; next primary canonical candidates are `WorkspaceOrchestrator.Lifecycle.cs` and `OverlayWindow.xaml.cs` (queued as `NX-116` next).
131. `NX-116` completed with green validation (`build`, `103/103 tests`, `governance`) after `WorkspaceOrchestrator` lifecycle micro-split.
132. Hotspot inventory refreshed; next primary canonical candidates are `WorkspaceOrchestrator.MessageHandling.Hotkeys.cs` and `OverlayWindow.xaml.cs` (queued as `NX-117` next).
133. `NX-117` completed with green validation (`build`, `103/103 tests`, `governance`) after `WorkspaceOrchestrator` hotkey-routing micro-split.
134. `NX-118` completed with green validation (`build`, `103/103 tests`, `governance`) after `OverlayWindow` lifecycle micro-split.
135. Hotspot inventory refreshed; next primary canonical candidates are `LegacyAppSettings.cs` and `TemplateVisionEngine.cs` (queued as `NX-119` next).
136. `NX-119` completed with green validation (`build`, `103/103 tests`, `governance`) after `LegacyAppSettings` property-group micro-split.
137. Hotspot inventory refreshed; next primary canonical candidates are `TemplateVisionEngine.cs` and `YoloOutputParser.Parse.Candidates.cs` (queued as `NX-120` next).
138. `NX-120` completed with green validation (`build`, `103/103 tests`, `governance`) after `TemplateVisionEngine` detection-flow micro-split.
139. Hotspot inventory refreshed; next primary canonical candidates are `RecordingWorkflowService.EngineRouting.Start.cs` and `YoloOutputParser.Parse.Candidates.cs` (queued as `NX-121` next).
140. `NX-121` completed with green validation (`build`, `103/103 tests`, `governance`) after `RecordingWorkflowService` engine-start routing micro-split.
141. Hotspot inventory refreshed; next primary canonical candidates are `RecordingBarViewModel.State.Workflow.cs` and `YoloOutputParser.Parse.Candidates.cs` (queued as `NX-122` next).
142. `NX-122` completed with green validation (`build`, `103/103 tests`, `governance`) after `RecordingBarViewModel` workflow-state micro-split.
143. Hotspot inventory refreshed; next primary canonical candidates are `SnapshotSelectionWindow.Input.Pointer.cs` and `YoloOutputParser.Parse.Candidates.cs` (queued as `NX-123` next).
144. `NX-123` completed with green validation (`build`, `103/103 tests`, `governance`) after `SnapshotSelectionWindow` pointer-flow micro-split.
145. Hotspot inventory refreshed; next primary canonical candidates are `YoloOutputParser.Parse.Candidates.cs` and `FfmpegRecordingEngine.Start.cs` (queued as `NX-124` next).
146. `NX-124` completed with green validation (`build`, `103/103 tests`, `governance`) after `YoloOutputParser` candidate-flow micro-split.
147. Hotspot inventory refreshed; next primary canonical candidates are `DashboardViewModel.Snapshot.cs` and `FfmpegRecordingEngine.Start.cs` (queued as `NX-125` next).
148. `NX-125` completed with green validation (`build`, `103/103 tests`, `governance`) after `DashboardViewModel` snapshot-projection micro-split.
149. Hotspot inventory refreshed; next primary canonical candidates are `OverlayTrackingService.cs` and `FfmpegRecordingEngine.Start.cs` (queued as `NX-126` next).
150. `NX-126` completed with green validation (`build`, `103/103 tests`, `governance`) after `OverlayTrackingService` lifecycle micro-split.
151. Hotspot inventory refreshed; next primary canonical candidates are `FfmpegRecordingEngine.Start.cs` and `Win32WindowControlService.cs` (queued as `NX-127` next).
152. `NX-127` completed with green validation (`build`, `103/103 tests`, `governance`) after `FfmpegRecordingEngine` start-flow micro-split.
153. Hotspot inventory refreshed; next primary canonical candidates are `MainWindow.xaml.cs` and `Win32WindowControlService.cs` (queued as `NX-128` next).
154. `NX-128` completed with green validation (`build`, `103/103 tests`, `governance`) after `MainWindow` lifecycle micro-split.
155. Hotspot inventory refreshed; next primary canonical candidates are `Win32WindowControlService.cs` and `WgcVideoRecordingEngine.Lifecycle.cs` (queued as `NX-129` next).
156. `NX-129` completed with green validation (`build`, `103/103 tests`, `governance`) after `Win32WindowControlService` command-surface micro-split.
157. Hotspot inventory refreshed; next primary canonical candidates are `WgcVideoRecordingEngine.Lifecycle.cs` and `AppSettingsSnapshot.cs` (queued as `NX-130` next).
158. `NX-130` completed with green validation (`build`, `103/103 tests`, `governance`) after `WgcVideoRecordingEngine` lifecycle-surface micro-split.
159. Hotspot inventory refreshed; next primary canonical candidates are `AppSettingsSnapshot.cs` and `OverlayWindow.Tracking.Lifecycle.cs` (queued as `NX-131` next).
160. `NX-131` completed with green validation (`build`, `103/103 tests`, `governance`) after `AppSettingsSnapshot` defaults-factory micro-split.
161. Hotspot inventory refreshed; next primary canonical candidates are `OverlayWindow.Tracking.Lifecycle.cs` and `WgcCaptureService.Bitmap.cs` (queued as `NX-132` next).
162. `NX-132` completed with green validation (`build`, `103/103 tests`, `governance`) after `OverlayWindow` tracking-lifecycle micro-split.
163. Hotspot inventory refreshed; next primary canonical candidates are `WgcCaptureService.Bitmap.cs` and `FfmpegSetupService.Download.cs` (queued as `NX-133` next).
164. `NX-133` completed with green validation (`build`, `103/103 tests`, `governance`) after `WgcCaptureService` bitmap-flow micro-split.
165. Hotspot inventory refreshed; next primary canonical candidates are `FfmpegSetupService.Download.cs` and `RecordingWorkflowService.EngineRouting.Start.Engine.cs` (queued as `NX-134` next).
166. `NX-134` completed with green validation (`build`, `103/103 tests`, `governance`) after `FfmpegSetupService` download-flow micro-split.
167. Hotspot inventory refreshed; next primary canonical candidates are `RecordingWorkflowService.EngineRouting.Start.Engine.cs` and `SnapshotSelectionWindow.Selection.cs` (queued as `NX-135` next).
168. `NX-135` completed with green validation (`build`, `103/103 tests`, `governance`) after `RecordingWorkflowService` engine-start helper micro-split.
169. Hotspot inventory refreshed; next primary canonical candidates are `SnapshotSelectionWindow.Selection.cs` and `AppSettingsSnapshot.Factory.cs` (queued as `NX-136` next).
170. `NX-136` completed with green validation (`build`, `103/103 tests`, `governance`) after `SnapshotSelectionWindow` selection-flow micro-split.
171. Hotspot inventory refreshed; next primary canonical candidates are `AppSettingsSnapshot.Factory.cs` and `DashboardViewModel.Lifecycle.cs` (queued as `NX-137` next).
172. `NX-137` completed with green validation (`build`, `103/103 tests`, `governance`) after `AppSettingsSnapshot` factory-section micro-split.
173. Hotspot inventory refreshed; next primary canonical candidates are `DashboardViewModel.Lifecycle.cs` and `WgcVideoRecordingEngine.Masking.cs` (queued as `NX-138` next).
174. `NX-138` completed with green validation (`build`, `103/103 tests`, `governance`) as final Wave 6 micro-split (`DashboardViewModel` lifecycle-surface).
175. Refactor Freeze Gate activated; roadmap execution switched to feature delivery sequence (`NX-020 -> NX-021 -> NX-022 -> NX-030 -> NX-031 -> NX-032`; capture wave `NX-011/012` completed).
176. `NX-011` marked complete after adding explicit instant-snapshot manual validation matrix and runbook checklist.
177. `NX-012` marked complete after shift-snapshot validation matrix, runbook checklist, and cancel-path unit coverage (`104/104` tests).
178. `NX-020`/`NX-021`/`NX-022` marked complete with WGC recording path active by default and legacy fallback retained.
179. `NX-030`/`NX-031` marked complete; roadmap focus narrowed to `NX-032` YOLO model-based smoke/hardening.
180. `NX-032` marked complete with optional model-based YOLO smoke suite and updated runbook/test-plan instructions (`105/105` tests).
181. Current roadmap baseline has no open `NX-###` items (`98/98 Done`).

## Refactor Freeze Gate (Approved 2026-02-13)

1. `NX-138` is the final allowed Wave 6 micro-split task.
2. After `NX-138` is completed, no new `NX-1xx` refactor micro-split tasks are created.
3. Execution order switches to feature delivery:
feature-delivery track (`NX-011..NX-032`) completed in current roadmap baseline.
4. Weekly roadmap review must track feature progress first; refactor work is allowed only for blocker removal.

## Key risks

1. WGC compatibility and border behavior variation across Windows builds.
2. Transition complexity from `gdigrab` to WGC frame pipeline.
3. Large refactor surface touching settings, workflow orchestration, and UI behavior.
4. Vision subsystem complexity (fallback + YOLO runtime).

## Cadence

1. Weekly roadmap review (status/risk/blockers).
2. Every merged PR updates docs and backlog task state.
3. Architectural decisions are captured in ADR files on the day of decision.
