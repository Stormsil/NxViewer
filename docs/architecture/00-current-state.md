# Current State (Canonical Baseline)

Last updated: 2026-02-13

## Canonical projects

1. `src/NxTiler.Domain`
2. `src/NxTiler.Application`
3. `src/NxTiler.Infrastructure`
4. `src/NxTiler.App`
5. `tests/NxTiler.Tests`

## Integrated library projects

1. `src/WindowCaptureCL/WindowCaptureCL.csproj`
2. `src/WindowManagerCL/WindowManagerCL/WindowManagerCL.csproj`
3. `src/ImageSearchCL/ImageSearchCL.csproj`
4. `src/ImageSearchCL/ImageSearchCL.WindowCapture/ImageSearchCL.WindowCapture.csproj`

## Legacy track

The root legacy project (`NxTiler.csproj` and root WPF files) is frozen and not used for new implementation work.

## Existing orchestration shape

1. `WorkspaceOrchestrator` coordinates window discovery, arrangement, tray, and hotkeys.
2. `RecordingWorkflowService` manages mask editing and recording lifecycle.
3. `FfmpegRecordingEngine` is current default recording backend (`gdigrab` based).
4. `CaptureWorkflowService` + `WgcCaptureService` provide initial instant/region snapshot workflow using `WindowCaptureCL`.
5. Region snapshot now has an interactive two-step overlay flow (region selection + mask editing) via `SnapshotSelectionService`.
6. Settings are persisted by `JsonSettingsService` with legacy migration support.
7. Hotkey model includes dedicated capture actions (instant + region snapshot) and runtime registration.
8. Recording workflow supports dual engine routing:
`FfmpegRecordingEngine` (`gdigrab`) fallback and `WgcVideoRecordingEngine` (`WindowCaptureCL` frame pump) via feature flag.
9. Pause/resume in WGC route keeps editable mask workflow and applies masks on finalize.
10. On WGC startup failure, workflow auto-falls back to legacy engine with status diagnostics.
11. Vision workflow baseline exists:
`VisionWorkflowService` resolves target window, routes by feature flags to `IVisionEngine`, and is wired to `ToggleVisionMode` hotkey.
12. `TemplateVisionEngine` is integrated in infrastructure:
`WindowCaptureCL` captures obscured windows, `ImageSearchCL` performs template matching from `VisionTemplates` directory.
13. Hotkey model includes `ToggleVision` binding with runtime registration and settings normalization.
14. `YoloVisionEngine` phase-1 is integrated as an engine slot:
model-path validation + controlled failure path + workflow fallback to template engine (when enabled).
15. Vision settings are editable in app settings UI:
confidence threshold, preferred engine, template directory, YOLO model path, and vision feature flags.
16. `YoloVisionEngine` now contains phase-2 ONNX inference with decomposed internals:
`YoloSessionProvider`, `YoloPreprocessor`, `YoloOutputParser`, and `YoloDetectionPostProcessor` keep the engine as orchestration shell.
17. Overlay tracking baseline exists:
`IOverlayTrackingService` + `OverlayTrackingService` + `OverlayWindow` integration for focused window geometry syncing.
18. Overlay visibility policies are now enforced in runtime tracking (`Always` / `OnHover` / `HideOnHover`).
19. Overlay visibility policy evaluation is testable via injected cursor provider abstraction (`ICursorPositionProvider`).
20. Overlay window now applies proportional scaling via tracked width/height (`ScaleTransform`).
21. Overlay tracking supports anchor-aware placement (`TopLeft..BottomRight`) with monitor-bound clamping.
22. Canonical track governance is automated:
`scripts/governance/validate-canonical-track.ps1` ensures solution/project wiring stays on `src/*` and keeps legacy root frozen.
23. Refactor hotspots and next decomposition tracks are documented in:
`docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`.
24. Recording infrastructure now uses shared FFmpeg process support helpers to unify stderr-tail capture and exit-timeout handling across engines.
25. Settings normalization in `JsonSettingsService` is now decomposed into section-specific helpers for maintainability.
26. Recording workflow transitions are managed by explicit transition table (`RecordingWorkflowStateMachine`) integrated into `RecordingWorkflowService`.
27. `DashboardViewModel` has been structurally split into partial units (`Lifecycle`, `Commands`, `Snapshot`) while preserving runtime behavior.
28. `DashboardViewModel` command execution is now delegated to `DashboardWorkspaceCommandService` and `DashboardRecordingCommandService`.
29. Dashboard busy/error command execution policy is centralized in `DashboardCommandExecutionService` and consumed by `DashboardViewModel`.
30. `ImageSearchCL` API facades (`Search`, `ImageSearchConfiguration`, `Images`) are now decomposed into focused partial files with unchanged public behavior.
31. `ImageSearchCL` template matching infrastructure is split into dedicated single-match and multi-match/NMS partial files under `TemplateMatchingEngine`.
32. `WindowCaptureCL` DirectX `FrameProcessor` is split into dedicated partial files for pool/lifecycle, conversion overloads, and async wrapper.
33. `WindowCaptureCL` `DpiHelper` is split into dedicated partial files for awareness setup, DPI query/conversion, and native interop.
34. `ImageSearchCL` `FindResult` and `IObjectSearch` API contracts are split into focused partial files with unchanged behavior/signatures.
35. `ImageSearchCL` `TrackingConfiguration` is split into dedicated partial files for constructor core, validation, and immutable withers.
36. `ImageSearchCL` `FrameQueue` is split into dedicated partial files for queue operations and lifecycle.
37. `WindowCaptureCL` `DirectXDeviceManager` is split into dedicated partial files for initialization, resource allocation, and lifecycle.
38. `WindowManagerCL` `Window` facade is split into dedicated partial files for search/discovery and handle/foreground operations.
39. `ImageSearchCL` `ReferenceImage` is split into dedicated partial files for constructor core, static factory helpers, and lifecycle/format validation.
40. `ImageSearchCL` `ImageSearch` one-shot facade is split into dedicated partial files for single-match and multi-match entry points.
41. `WindowManagerCL` `WindowFinder` is split into dedicated partial files for enumeration and filter/regex operations.
42. `ImageSearchCL` nested `DebugOverlay.OverlayWindow` is split into dedicated partial files for core form, rendering flow, and native interop.
43. `WindowManagerCL` `Infrastructure/WinApi` is split into dedicated partial files for declarations, structs, and constants.
44. `WindowCaptureCL` `Infrastructure/WGC/WindowEnumerator` is split into dedicated partial files for core API and Win32 interop; `WindowInfo` moved to dedicated file.
45. `WindowCaptureCL` pool types are split into dedicated files: `ObjectPool<T>` and `KeyedObjectPool<TKey, TValue>`.
46. Recording engines are now structurally decomposed into partial orchestration shells:
`FfmpegRecordingEngine` and `WgcVideoRecordingEngine` have dedicated files for arguments, lifecycle, masking, frame pump, and geometry concerns.
47. `RecordingWorkflowService` is now structurally decomposed into partial files:
commands, overlay callbacks, state/message transitions, and serialized execution helpers are isolated while keeping `IRecordingWorkflowService` behavior unchanged.
48. Recording engines now share internal `FfmpegArgumentBuilder` for command-string generation with dedicated parity tests.
49. `OverlayWindow` code-behind is now structurally decomposed into partial files for interaction, position persistence, messenger wiring, and tracking/layout behavior.
50. `SnapshotSelectionWindow` code-behind is now structurally decomposed into partial files for input handling, region/mask operations, layout updates, and DIP-to-pixel conversion helpers.
51. `RecordingWorkflowService` additionally isolates engine-routing/ffmpeg-resolve/finalization helpers in `RecordingWorkflowService.EngineRouting.cs`.
52. `OverlayWindowViewModel` is now structurally decomposed into partial files for commands, lifecycle, messenger handlers, and snapshot projection/debounce logic.
53. `RecordingViewModel` and `RecordingBarViewModel` are now structurally decomposed into partial files for commands, state/messaging handlers, and lifecycle concerns.
54. `OverlayTrackingService` is now structurally decomposed into partial files for lifecycle control, polling loop, state computation, visibility policy, and geometry placement.
55. `WgcCaptureService` is now structurally decomposed into partial files for window preparation/stabilization, bounds geometry mapping, bitmap/mask rendering, and output persistence (disk + clipboard).
56. `Win32Native` is now structurally decomposed into partial files for interop declarations, constants, native data structures, and window/screen helper operations.
57. `VisionWorkflowService` is now structurally decomposed into partial files for toggle/scan orchestration, engine resolution, YOLO fallback handling, target-window resolution, and serialized execution.
58. `WorkspaceOrchestrator` command and message handling are now structurally decomposed into focused partial files for arrangement/navigation/session commands and hotkey/event/registration execution paths.
59. `JsonSettingsService` is now structurally decomposed into partial files for initial/backup loading, atomic persistence, and settings normalization policies.
60. `FfmpegRecordingEngine` finalize flow is now structurally decomposed into focused partial files for single-segment finalization, concat finalization, and optional masking pass.
61. `SettingsViewModel` is now structurally decomposed into focused partial files for command handling and snapshot load/build projection.
62. `WgcVideoRecordingEngine` is now structurally decomposed into focused partial files for startup/probe+process boot, pause/resume controls, and stop/abort orchestration.
63. `WorkspaceOrchestrator` lifecycle-core concerns are now structurally decomposed into focused partial files for startup/disposal rollback and monitor start/stop management.
64. `LegacyAppSettings` is now structurally decomposed into focused partial files for base settings shell, hotkey settings, and recording settings defaults.
65. `FfmpegRecordingEngine` root-shell concerns are now structurally decomposed into focused partial files for capture-area startup validation/configuration and segment process startup.
66. `LogsViewModel` is now structurally decomposed into focused partial files for lifecycle subscription/disposal, view filtering rules, and log commands.
67. App bootstrap is now structurally decomposed into focused partial files for host wiring, startup/shutdown lifecycle, and global exception handling.
68. `RecordingWorkflowService` command surface is now structurally decomposed into focused partial files for mask-editing start, recording control (start/pause/resume), and completion/cancel flows.
69. `JsonSettingsService` normalization surface is now structurally decomposed into focused partial files for core orchestration, general sections, feature-related sections, and overlay policy normalization.
70. `ArrangementService` is now structurally decomposed into focused partial files for grid strategy, focus/max-size strategy, and shared layout construction helpers.
71. `DashboardViewModel` command surface is now structurally decomposed into focused partial files for workspace commands, recording commands, state-change hooks, and busy-execution policy helper.
72. `OverlayWindow` tracking code-behind surface is now structurally decomposed into focused partial files for tracking lifecycle, tracking-state projection, and scale transform helpers.
73. `HotkeysViewModel` is now structurally decomposed into focused partial files for command handling and hotkey snapshot projection/load mapping.
74. `RecordingOverlayService` is now structurally decomposed into focused partial files for overlay lifecycle/show-close, recording-mode interactions, mask extraction, and UI-dispatch invocation helper.
75. `WindowEventMonitorService` is now structurally decomposed into focused partial files for hook lifecycle/disposal and WinEvent callback/debounce event projection.
76. `MaskOverlayViewModel` is now structurally decomposed into focused partial files for mask geometry operations, mode-state transitions, and confirmation/cancel commands.
77. `FfmpegSetupService` is now structurally decomposed into focused partial files for FFmpeg path resolve/save flow, download/extract flow, and PATH probe helper.
78. `YoloVisionEngine` orchestration is now structurally decomposed into focused partial files for detect-execution flow, model/label resolution helpers, and lifecycle disposal.
79. `CaptureWorkflowService` is now structurally decomposed into focused partial files for workflow command flow, target window resolution, and serialized execution + logging helpers.
80. `NomachineSessionService` is now structurally decomposed into focused partial files for session discovery ordering, launch/open flows, and regex-based session filtering.
81. `SnapshotSelectionWindow` input flow is now structurally decomposed into focused partial files for keyboard actions and pointer interaction flow.
82. `GlobalHotkeyService` is now structurally decomposed into focused partial files for registration map setup, lifecycle/unregister handling, and Win32 hotkey message dispatch.
83. `MaskOverlayWindow` code-behind is now structurally decomposed into focused partial files for window lifecycle, recording/edit mode transitions, and pointer-input interactions.
84. `YoloOutputParser` is now structurally decomposed into focused partial files for rank/shape routing + candidate extraction loop and numeric helper utilities.
85. `RecordingWorkflowService` engine-routing helpers are now structurally decomposed into focused partial files for ffmpeg resolve/start path and abort/finalize completion path.
86. `Win32WindowQueryService` is now structurally decomposed into focused partial files for window query enumeration/projection flow and regex-cache compilation helpers.
87. `HotkeyBox` control is now structurally decomposed into focused partial files for keyboard capture flow and display-text update projection.
88. `FfmpegRecordingEngine` masking-finalize flow is now structurally decomposed into focused partial files for mask-filter construction, ffmpeg process execution, and masked-output promotion.
89. `WorkspaceOrchestrator` arrangement surface is now structurally decomposed into focused partial files for target refresh/query options, arrange execution flow, and snapshot/status message publishing.
90. `RecordingBarViewModel` state surface is now structurally decomposed into focused partial files for workflow-state/message handling and timer-based elapsed-time projection.
91. App host-registration surface is now structurally decomposed into focused partial files for core DI wiring, UI infrastructure registrations, workflow service registrations, and presentation registrations.
92. `OverlayWindowViewModel` command surface is now structurally decomposed into focused partial files for overlay command actions, toggle callbacks, and command execution/error-policy flow.
93. `VisionWorkflowService` scan surface is now structurally decomposed into focused partial files for core scan execution, YOLO fallback path, and vision-request construction.
94. `WgcVideoRecordingEngine` start surface is now structurally decomposed into focused partial files for startup input validation, session/path initialization, and ffmpeg/frame-pump bootstrap.
95. `RecordingViewModel` command surface is now structurally decomposed into focused partial files for settings/ffmpeg commands, recording workflow commands, and shared busy/error execution wrapper.
96. `YoloOutputParser` parse surface is now structurally decomposed into focused partial files for output-shape resolution and per-box candidate extraction flow.
97. `WorkspaceOrchestrator` lifecycle surface is now structurally decomposed into focused partial files for start initialization flow, async dispose/shutdown flow, and failed-start rollback logic.
98. `WorkspaceOrchestrator` hotkey message-handling surface is now structurally decomposed into focused partial files for action routing, recording action handlers, and capture/vision action handlers.
99. `OverlayWindow` lifecycle handlers are now isolated in dedicated partial file (`OverlayWindow.Lifecycle.cs`) with unchanged window behavior.
100. `LegacyAppSettings` legacy-schema surface is now structurally decomposed into focused partial files for filters, layout, overlay/UI, and collection defaults.
101. `TemplateVisionEngine` is now structurally decomposed into focused partial files for detection pipeline flow and template-directory/template-file resolution helpers.
102. `RecordingWorkflowService` engine-start routing surface is now structurally decomposed into focused partial files for ffmpeg resolve/download flow and engine start/fallback helpers.
103. `RecordingBarViewModel` workflow-state surface is now structurally decomposed into focused partial files for state dispatch and per-state UI projection helpers.
104. `SnapshotSelectionWindow` pointer-input surface is now structurally decomposed into focused partial files for pointer-down routing, drag/update flow, and right-click context remove handling.
105. `YoloOutputParser` candidate flow is now structurally decomposed into focused partial files for candidate orchestration, score/class resolution, and bounds projection helpers.
106. `DashboardViewModel` snapshot surface is now structurally decomposed into focused partial files for messenger/debounce handlers and snapshot/recording projection mapping helpers.
107. `OverlayTrackingService` lifecycle surface is now structurally decomposed into focused partial files for start/update-target and stop/dispose helper flows.
108. `FfmpegRecordingEngine` start surface is now structurally decomposed into focused partial files for start-parameter validation, virtual-screen geometry alignment, and start-session/bootstrap folder helpers.
109. `MainWindow` lifecycle handlers are now isolated in `MainWindow.Lifecycle.cs` with unchanged shell startup/shutdown behavior.
110. `Win32WindowControlService` is now structurally decomposed into focused partial files for window command operations, placement application helpers, and window/monitor bounds queries.
111. `WgcVideoRecordingEngine` lifecycle surface is now structurally decomposed into focused partial files for stop/finalize orchestration and cleanup/reset helper flow.
112. `AppSettingsSnapshot` defaults surface is now structurally decomposed into focused partial files for root contract, optional-section defaults, and default-factory helper composition.
113. `OverlayWindow` tracking lifecycle surface is now structurally decomposed into focused partial files for visibility callback handling, tracking start/update flow, and tracking stop/reset flow.
114. `WgcCaptureService` bitmap surface is now structurally decomposed into focused partial files for output projection, mask rendering, and PNG encoding helpers.
115. `FfmpegSetupService` download surface is now structurally decomposed into focused partial files for download flow orchestration, transfer progress handling, archive extraction/discovery, and zip cleanup.
116. `RecordingWorkflowService` engine-start routing helper surface is now structurally decomposed into focused partial files for preferred WGC start path and legacy/failure fallback helper flow.
117. `SnapshotSelectionWindow` selection surface is now structurally decomposed into focused partial files for region selection, mask creation/finalization, and mask move/hit-test helpers.
118. `AppSettingsSnapshot` default-factory surface is now structurally decomposed into focused partial files for base snapshot composition, hotkey defaults, and feature-section defaults.
119. Refactor Freeze Gate is approved: `NX-138` is the final Wave 6 micro-split task; after that, execution focus switches to functional waves (`NX-011/012`, `NX-020/021/022`, `NX-030/031/032`).
120. `DashboardViewModel` lifecycle surface is now structurally decomposed into focused partial files for activation, deactivation, disposal, and messenger/startup sync helpers.
121. `NX-011` instant snapshot track has explicit manual validation matrix and runbook checklist; remaining primary work is functional continuation from `NX-012` onward.
122. `NX-012` shift snapshot track includes selection/mask interaction checklist and workflow cancel-path unit coverage in `CaptureWorkflowServiceTests`.
123. WGC recording path is now default in `AppSettingsSnapshot` (`FeatureFlags.UseWgcRecordingEngine=true`) with runtime fallback to legacy engine preserved for compatibility.
124. Vision baseline now includes optional model-based smoke coverage (`YoloModelSmokeTests`) for ONNX inference/parsing path validation when smoke env vars are configured.

## Gaps before target architecture

1. Snapshot workflow exists but still needs end-to-end manual hardening (multi-monitor, DPI, and overlap scenarios).
2. YOLO runtime hardening remains pending: model compatibility matrix, perf tuning, and validation with production models.
3. Overlay scaling and anchor placement baseline are implemented; remaining work is advanced DPI/UX tuning beyond current roadmap baseline.
4. Legacy compatibility paths still exist by design where migration safety is required (recording fallback and settings migration).
5. Library projects are in solution; runtime wiring is still partial across subsystems.
