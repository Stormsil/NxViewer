# NxTiler Backlog

Last updated: 2026-02-13

## Task template

```md
## NX-###
Status: Planned | In Progress | Blocked | Review | Done
Priority: P0 | P1 | P2
Estimate: S | M | L | XL

Title:
Scope:
In:
Out:
Dependencies:
Risk:
Acceptance:
Test Cases:
Docs to update:
```

## Execution policy (Refactor Freeze Gate, approved 2026-02-13)

1. `NX-138` is the final allowed micro-split task in Wave 6.
2. After `NX-138` reaches `Done`, no new `NX-1xx` refactor micro-split tasks are added.
3. Execution focus switches to feature delivery only:
`NX-032` (`NX-011/012`, `NX-020/021/022`, and `NX-030/031` completed).
4. New backlog entries after this gate must target functional behavior, hardening, or release readiness.

## Tasks

## NX-001
Status: Done  
Priority: P0  
Estimate: S

Title: Freeze legacy root track
Scope: Mark root legacy project/files as deprecated and prevent accidental new feature development there.
In: Add explicit freeze policy and build-time warning for `NxTiler.csproj`.
Out: Deleting legacy files.
Dependencies: None.
Risk: Low.
Acceptance:
1. Freeze policy is documented in repository.
2. Building root legacy project shows freeze warning.
Test Cases:
1. Build `NxTiler.csproj` and verify warning `NXLEGACY001`.
Docs to update:
1. `LEGACY_ROOT_FREEZE.md`
2. `docs/architecture/00-current-state.md`

## NX-002
Status: Done  
Priority: P0  
Estimate: S

Title: Add library projects to canonical solution
Scope: Include window capture, window manager, and image search libraries in `NxTiler.sln`.
In: Add `WindowCaptureCL`, `WindowManagerCL`, `ImageSearchCL`, `ImageSearchCL.WindowCapture`.
Out: Refactoring internal library implementations.
Dependencies: None.
Risk: Low.
Acceptance:
1. Projects are present in `NxTiler.sln`.
Test Cases:
1. `dotnet build NxTiler.sln`.
Docs to update:
1. `docs/architecture/02-integration-map.md`
2. `docs/changelog/CHANGELOG.md`

## NX-003
Status: Done  
Priority: P0  
Estimate: S

Title: Unify build and analyzer defaults
Scope: Add repository-level build conventions.
In: Add `Directory.Build.props` and `Directory.Build.targets`.
Out: Treat warnings as errors globally.
Dependencies: None.
Risk: Low.
Acceptance:
1. Shared props/targets exist and are applied.
Test Cases:
1. `dotnet build NxTiler.sln`.
Docs to update:
1. `docs/ops/RUNBOOK.md`
2. `docs/changelog/CHANGELOG.md`

## NX-004
Status: Done  
Priority: P0  
Estimate: M

Title: Create evergreen docs skeleton
Scope: Add roadmap, backlog, architecture, ADR, testing, ops, and changelog docs.
In: Core docs and PR template with docs checklist.
Out: Full implementation-level design docs for all future tasks.
Dependencies: NX-001.
Risk: Low.
Acceptance:
1. Requested docs tree exists and is populated.
2. PR template includes mandatory docs update checklist.
Test Cases:
1. Manual docs tree review.
Docs to update:
1. All files under `docs/`
2. `.github/pull_request_template.md`

## NX-010
Status: Done  
Priority: P0  
Estimate: M

Title: Introduce capture and workflow abstractions
Scope: Add application-level interfaces and baseline domain contracts for capture.
In: `ICaptureService`, `ICaptureWorkflowService`, capture domain records/enums.
Out: Full WGC implementation.
Dependencies: NX-002.
Risk: Medium.
Acceptance:
1. New abstractions compile and are available for integration.
Test Cases:
1. `dotnet build NxTiler.sln`.
Docs to update:
1. `docs/architecture/01-target-architecture.md`
2. `docs/architecture/02-integration-map.md`

## NX-011
Status: Done  
Priority: P0  
Estimate: L

Title: WGC instant window snapshot pipeline
Scope: End-to-end instant snapshot flow with maximize, stabilization wait, capture, disk save, clipboard copy.
In: Service implementation, orchestration, UI command and hotkey support.
Out: Region selection and recording.
Dependencies: NX-010.
Risk: Medium.
Acceptance:
1. Instant snapshot works for selected NoMachine window.
2. Output is saved to disk and clipboard.
Progress:
1. Implemented first pass: `WgcCaptureService` + `CaptureWorkflowService` + orchestrator hotkey route for `InstantSnapshot`.
2. Added configurable hotkeys for `InstantSnapshot` and `RegionSnapshotWithMask` (settings + registration + UI).
3. `WgcCaptureService` enforces `maximize -> stabilize -> capture` and `disk + clipboard` output path handling.
4. Manual validation matrix documented in `docs/testing/TEST-PLAN.md` and execution checklist added in `docs/ops/RUNBOOK.md`.
Test Cases:
1. Snapshot integration tests.
2. Manual validation with window overlap.
Docs to update:
1. `docs/testing/TEST-PLAN.md`
2. `docs/ops/RUNBOOK.md`

## NX-012
Status: Done  
Priority: P0  
Estimate: L

Title: Shift snapshot with masks
Scope: Area selection and mask editor for still image snapshots.
In: Selection overlay, add/remove mask interactions, baked final output.
Out: Video mask editing.
Dependencies: NX-011.
Risk: Medium.
Acceptance:
1. User can select region and edit masks before final save.
Progress:
1. Added interactive region + mask overlay (`SnapshotSelectionWindow`) with two-phase flow.
2. Mask editor supports add, move (drag), and remove (right-click).
3. `CaptureWorkflowService` now uses interactive selection for region snapshot when selection service is registered.
4. Added cancel-path unit coverage in `tests/NxTiler.Tests/CaptureWorkflowServiceTests.cs` (`RunRegionSnapshotAsync_ReturnsFailure_WhenInteractiveSelectionIsCanceled`).
5. Added explicit manual validation matrix and runbook checklist for shift snapshot and mask interactions.
Test Cases:
1. UI interaction tests for mask operations.
2. Workflow cancel-path unit test (`CaptureWorkflowServiceTests`).
Docs to update:
1. `docs/testing/TEST-PLAN.md`
2. `docs/ops/RUNBOOK.md`

## NX-020
Status: Done  
Priority: P0  
Estimate: XL

Title: WGC frame-to-FFmpeg recording engine
Scope: Introduce `IVideoRecordingEngine` implementation with FFmpeg stdin pipeline.
In: Start/pause/resume/stop for frame-based recording.
Out: Vision overlay integration.
Dependencies: NX-010.
Risk: High.
Acceptance:
1. Recording engine uses WGC frames instead of `gdigrab`.
Progress:
1. Added `WgcVideoRecordingEngine` with frame pump (`WindowCaptureCL` -> ffmpeg `image2pipe`).
2. Added dual-engine routing in `RecordingWorkflowService` via `FeatureFlags.UseWgcRecordingEngine`.
3. Legacy `gdigrab` path remains active as fallback.
4. Workflow coverage exists for WGC engine start/pause/resume/stop path (`tests/NxTiler.Tests/RecordingWorkflowServiceTests.cs`).
Test Cases:
1. Recording engine integration tests.
Docs to update:
1. `docs/architecture/01-target-architecture.md`
2. `docs/testing/TEST-PLAN.md`

## NX-021
Status: Done  
Priority: P0  
Estimate: L

Title: Pause/resume with editable masks
Scope: Allow mask edits during paused recording.
In: Pause editor state and mask continuity.
Out: YOLO-based dynamic masks.
Dependencies: NX-020.
Risk: Medium.
Acceptance:
1. Masks can be edited during pause and applied after resume.
Progress:
1. `RecordingWorkflowService` pause/resume for WGC path now routes to `IVideoRecordingEngine` without switching engine mode.
2. Finalized masks from overlay are mapped and passed to WGC stop path (`StopAsync`) after pause/resume cycles.
3. Added tests for WGC path: mask propagation and discard flow through `AbortAsync`.
Test Cases:
1. Pause/resume workflow tests.
Docs to update:
1. `docs/testing/TEST-PLAN.md`

## NX-022
Status: Done  
Priority: P0  
Estimate: L

Title: Recording migration with fallback
Scope: Move from `gdigrab` path to WGC recording with configurable fallback.
In: Feature flags and compatibility routing.
Out: Remove fallback entirely.
Dependencies: NX-020.
Risk: High.
Acceptance:
1. New path is default.
2. Fallback path remains operational.
Progress:
1. Added runtime auto-fallback: if WGC engine start fails, workflow switches to legacy engine in the same run.
2. Added workflow test coverage for fallback path (`WGC start fail -> legacy start`).
3. Added user-visible diagnostics messages for fallback transitions.
4. Switched default recording engine flag to WGC (`AppSettingsSnapshot.CreateDefault().FeatureFlags.UseWgcRecordingEngine = true`) while preserving legacy fallback.
Test Cases:
1. Both engines pass smoke tests.
Docs to update:
1. `docs/ops/RUNBOOK.md`

## NX-030
Status: Done  
Priority: P0  
Estimate: L

Title: Vision engine abstraction and orchestrator
Scope: Add `IVisionEngine` and orchestrator workflow.
In: Core vision contracts and orchestration lifecycle.
Out: YOLO runtime specifics.
Dependencies: NX-010.
Risk: Medium.
Acceptance:
1. Vision engine interface and orchestrator integrated into app architecture.
Progress:
1. Added `IVisionWorkflowService` and `VisionWorkflowService`.
2. Added hotkey route in `WorkspaceOrchestrator` for `HotkeyAction.ToggleVisionMode`.
3. Added unit coverage in `tests/NxTiler.Tests/VisionWorkflowServiceTests.cs`.
Test Cases:
1. Orchestrator unit tests.
Docs to update:
1. `docs/architecture/01-target-architecture.md`
2. `docs/testing/TEST-PLAN.md`

## NX-031
Status: Done  
Priority: P0  
Estimate: M

Title: Template matching fallback mode
Scope: Keep temporary OpenCV-based fallback while YOLO matures.
In: Feature flags and runtime selection logic.
Out: Permanent support commitment.
Dependencies: NX-030.
Risk: Medium.
Acceptance:
1. Template fallback can be enabled/disabled with feature flags.
Progress:
1. Added `NxTiler.Infrastructure.Vision.TemplateVisionEngine`.
2. Added DI registration of `IVisionEngine` implementation.
3. `VisionWorkflowService` routes engine selection by feature flags and preferred engine.
Test Cases:
1. Engine selection tests.
Docs to update:
1. `docs/architecture/02-integration-map.md`
2. `docs/ops/RUNBOOK.md`

## NX-032
Status: Done  
Priority: P0  
Estimate: XL

Title: ONNX YOLO engine
Scope: Add YOLO detection engine and integrate with vision orchestrator.
In: ONNX runtime integration, model loading, detections.
Out: model training pipeline.
Dependencies: NX-030.
Risk: High.
Acceptance:
1. YOLO detections available through `IVisionEngine`.
Progress:
1. Added `YoloVisionEngine` phase-1 implementation and DI registration.
2. Added workflow fallback behavior: when `yolo` fails, workflow falls back to template engine (if enabled).
3. Expanded `VisionSettings` and Settings UI for model/template paths and engine flags.
4. Added unit coverage for yolo failure fallback routing.
5. Added phase-2 ONNX pipeline in `YoloVisionEngine`:
- model loading via `Microsoft.ML.OnnxRuntime`
- frame preprocessing (letterbox + tensor)
- detection parsing and NMS postprocessing
6. Added optional model-based smoke suite in `tests/NxTiler.Tests/YoloModelSmokeTests.cs` (`NXTILER_YOLO_MODEL_SMOKE`, `NXTILER_YOLO_SMOKE_IMAGES`).
Test Cases:
1. Vision integration tests with model.
Docs to update:
1. `docs/testing/TEST-PLAN.md`
2. `docs/ops/RUNBOOK.md`

## NX-040
Status: Done  
Priority: P1  
Estimate: L

Title: Overlay tracking service
Scope: Overlay follows target window movement and size changes.
In: `IOverlayTrackingService`, event hooks, geometry sync.
Out: advanced overlay authoring.
Dependencies: NX-010.
Risk: Medium.
Acceptance:
1. Overlay remains aligned with target window.
Progress:
1. Added `IOverlayTrackingService` in `NxTiler.Application.Abstractions`.
2. Implemented `NxTiler.App.Services.OverlayTrackingService` (target polling + placement updates + policy visibility).
3. Integrated tracking into `OverlayWindow` lifecycle and workspace snapshot updates.
4. Added unit coverage in `tests/NxTiler.Tests/OverlayTrackingServiceTests.cs`.
Test Cases:
1. Overlay tracking integration tests.
Docs to update:
1. `docs/testing/TEST-PLAN.md`

## NX-041
Status: Done  
Priority: P2  
Estimate: M

Title: Overlay visibility policies
Scope: Implement `Always`, `OnHover`, `HideOnHover`.
In: Policy model and runtime behavior.
Out: role-based visibility.
Dependencies: NX-040.
Risk: Medium.
Acceptance:
1. All three visibility modes work as configured.
Progress:
1. Added cursor position provider abstraction (`ICursorPositionProvider`) and Win32 implementation for deterministic policy evaluation.
2. Extended `OverlayTrackingService` to evaluate visibility modes through injected cursor provider.
3. Added runtime compatibility mapping from legacy `HideOnHover` flag to canonical `OverlayVisibilityMode`.
4. Added unit coverage for `OnHover` and `HideOnHover` transitions in `tests/NxTiler.Tests/OverlayTrackingServiceTests.cs`.
5. Added settings normalization test for legacy `HideOnHover` migration in `tests/NxTiler.Tests/SettingsServiceTests.cs`.
Test Cases:
1. UI behavior tests.
2. `OverlayTrackingServiceTests` for `Always`, `OnHover`, and `HideOnHover`.
3. `SettingsServiceTests` for overlay visibility normalization.
Docs to update:
1. `docs/ops/RUNBOOK.md`
2. `docs/testing/TEST-PLAN.md`

## NX-042
Status: Done  
Priority: P2  
Estimate: M

Title: Overlay content scaling
Scope: Scale overlay content with target window resize.
In: Anchor and scale policy model support.
Out: custom transform editor.
Dependencies: NX-040.
Risk: Medium.
Acceptance:
1. Overlay scales proportionally with target.
Progress:
1. `OverlayWindow` now applies tracked scale factors through `ScaleTransform` from `OverlayTrackingState` dimensions.
2. Tracking lifecycle resets scaling on hide/stop to avoid stale transforms.
3. Added anchor-aware placement model (`OverlayAnchor`) in `OverlayPoliciesSettings` and `OverlayTrackingRequest`.
4. `OverlayTrackingService` now resolves anchor placement (`TopLeft..BottomRight`) and clamps coordinates to monitor bounds.
5. Added scaling/anchor test coverage in `tests/NxTiler.Tests/OverlayTrackingServiceTests.cs`:
- non-uniform scaling
- min-size clamping
- top-right anchor placement
- bottom-right anchor monitor clamping
6. Added settings normalization coverage for invalid anchor fallback in `tests/NxTiler.Tests/SettingsServiceTests.cs`.
Test Cases:
1. Resize tests across multiple dimensions.
2. Anchor placement and monitor clamp tests.
Docs to update:
1. `docs/testing/TEST-PLAN.md`
2. `docs/architecture/01-target-architecture.md`

## NX-050
Status: Done  
Priority: P0  
Estimate: L

Title: Performance and regression hardening
Scope: Final perf pass and regression verification.
In: Benchmarks/smoke runs and stabilization fixes.
Out: new feature work.
Dependencies: NX-011, NX-020, NX-030, NX-040.
Risk: Medium.
Acceptance:
1. Baseline perf targets defined and validated.
2. Existing regression suite remains green.
Progress:
1. Added reproducible perf smoke harness: `scripts/perf/perf-smoke.ps1` (build + tests + threshold checks + JSON artifact).
2. Added baseline document: `docs/testing/PERF-BASELINE.md`.
3. Added matrix runner `scripts/perf/perf-regression-matrix.ps1` (`Debug` + `Release`).
4. Added CI gate in `.github/workflows/ci.yml` (build + tests + perf matrix + artifacts upload).
5. Validated current matrix run:
- Debug: build `2.22s`, tests `2.55s`, total `4.77s`
- Release: build `1.98s`, tests `2.17s`, total `4.15s`
6. Regression suite remains green (`75/75`).
Test Cases:
1. Full solution build + test matrix.
2. Perf smoke script run with thresholds.
Docs to update:
1. `docs/testing/TEST-PLAN.md`
2. `docs/changelog/CHANGELOG.md`
3. `docs/testing/PERF-BASELINE.md`

## NX-051
Status: Done  
Priority: P1  
Estimate: M

Title: Deprecated path cleanup
Scope: Remove/retire deprecated legacy wiring and stale code paths.
In: Cleanup and migration completion.
Out: unrelated refactors.
Dependencies: NX-050.
Risk: Medium.
Acceptance:
1. Deprecated paths removed or explicitly archived.
Progress:
1. Removed stale unused domain type `src/NxTiler.Domain/Overlay/OverlayVisibilityPolicy.cs`.
2. Added canonical governance validator `scripts/governance/validate-canonical-track.ps1`.
3. Added CI enforcement step in `.github/workflows/ci.yml` (canonical track check before build).
4. Extended PR checklist with canonical governance check line.
5. Validated governance check locally (pass).
Test Cases:
1. Build/test and startup smoke checks.
2. `powershell -ExecutionPolicy Bypass -File .\scripts\governance\validate-canonical-track.ps1`.
Docs to update:
1. `docs/architecture/00-current-state.md`
2. `docs/changelog/CHANGELOG.md`

## NX-052
Status: Done  
Priority: P0  
Estimate: M

Title: Release baseline documentation
Scope: Finalize release docs and stable baseline declaration.
In: Ops/testing/changelog release readiness updates.
Out: New functionality.
Dependencies: NX-050, NX-051.
Risk: Low.
Acceptance:
1. Release baseline is documented and reproducible.
Progress:
1. Added release baseline document `docs/release/RELEASE-BASELINE.md`.
2. Added one-command release verification script `scripts/release/verify-release-baseline.ps1`.
3. Release verification script validates governance + build + tests + perf matrix.
4. Verified locally with successful end-to-end run.
Test Cases:
1. Clean clone build and smoke validation.
2. `powershell -ExecutionPolicy Bypass -File .\scripts\release\verify-release-baseline.ps1`
Docs to update:
1. `docs/ops/RUNBOOK.md`
2. `docs/changelog/CHANGELOG.md`
3. `docs/release/RELEASE-BASELINE.md`

## NX-060
Status: Done  
Priority: P1  
Estimate: L

Title: Shared FFmpeg process runner
Scope: Extract common FFmpeg process lifecycle logic used by both recording engines.
In: stderr tail capture, wait/timeout, graceful stop/kill fallback, process start validation.
Out: replacing recording engine business behavior.
Dependencies: NX-020, NX-022.
Risk: Medium.
Acceptance:
1. `FfmpegRecordingEngine` and `WgcVideoRecordingEngine` use shared FFmpeg utility layer.
2. No behavior regression in start/stop/finalize/abort flows.
Progress:
1. Added shared process support layer:
- `src/NxTiler.Infrastructure/Recording/FfmpegProcessSupport.cs`
- `ProcessStderrTail`
- `ProcessExitAwaiter`
2. Refactored both recording engines to consume shared support:
- `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.cs`
- `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.cs`
3. Added unit tests:
- `tests/NxTiler.Tests/FfmpegProcessSupportTests.cs`
4. Verified no regression in solution build and test suite.
Test Cases:
1. Existing recording workflow tests stay green.
2. New unit tests for shared FFmpeg runner timeouts/error tail handling.
Docs to update:
1. `docs/architecture/01-target-architecture.md`
2. `docs/changelog/CHANGELOG.md`

## NX-061
Status: Done  
Priority: P1  
Estimate: L

Title: Recording workflow state machine extraction
Scope: Move recording state transitions into dedicated, testable state machine component.
In: explicit transition map, invalid transition handling, engine policy handoff.
Out: redesign of overlay UX.
Dependencies: NX-020, NX-021, NX-022.
Risk: Medium.
Acceptance:
1. `RecordingWorkflowService` delegates transitions to state machine.
2. State transition coverage includes idle/mask/edit/record/pause/saving/cancel paths.
Progress:
1. Added explicit transition table component:
- `src/NxTiler.App/Services/RecordingWorkflowStateMachine.cs`
2. Integrated state machine into:
- `src/NxTiler.App/Services/RecordingWorkflowService.cs`
3. Added transition-table unit coverage:
- `tests/NxTiler.Tests/RecordingWorkflowStateMachineTests.cs`
4. Existing workflow tests remain green after integration.
Test Cases:
1. Recording workflow tests expanded for transition table behavior.
Docs to update:
1. `docs/architecture/00-current-state.md`
2. `docs/testing/TEST-PLAN.md`

## NX-062
Status: Done  
Priority: P2  
Estimate: M

Title: Dashboard orchestration split
Scope: Reduce DashboardViewModel orchestration density by extracting command groups/services.
In: split recording commands, workspace commands, and snapshot projection helpers.
Out: visual redesign of dashboard pages.
Dependencies: NX-061.
Risk: Medium.
Acceptance:
1. `DashboardViewModel` shrinks to state projection + binding coordination.
2. Command behavior remains unchanged.
Progress:
1. `DashboardViewModel` converted to partial and split into focused units:
- `src/NxTiler.App/ViewModels/DashboardViewModel.cs` (state/properties/ctor)
- `src/NxTiler.App/ViewModels/DashboardViewModel.Lifecycle.cs` (activate/deactivate/dispose)
- `src/NxTiler.App/ViewModels/DashboardViewModel.Commands.cs` (workspace/recording commands)
- `src/NxTiler.App/ViewModels/DashboardViewModel.Snapshot.cs` (snapshot/message projection)
2. Added command-service extraction for orchestration delegation:
- `src/NxTiler.App/Services/IDashboardWorkspaceCommandService.cs`
- `src/NxTiler.App/Services/DashboardWorkspaceCommandService.cs`
- `src/NxTiler.App/Services/IDashboardRecordingCommandService.cs`
- `src/NxTiler.App/Services/DashboardRecordingCommandService.cs`
3. `DashboardViewModel` commands now delegate to command services while keeping bindings and behavior unchanged.
4. Added focused command-service tests:
- `tests/NxTiler.Tests/DashboardWorkspaceCommandServiceTests.cs`
- `tests/NxTiler.Tests/DashboardRecordingCommandServiceTests.cs`
5. Existing behavior validated by full build + test + governance checks.
6. Extracted shared dashboard busy/error execution policy:
- `src/NxTiler.App/Services/IDashboardCommandExecutionService.cs`
- `src/NxTiler.App/Services/DashboardCommandExecutionService.cs`
- `src/NxTiler.App/ViewModels/DashboardViewModel.Commands.cs` now delegates `ExecuteBusyAsync` to the service.
- Added unit coverage in `tests/NxTiler.Tests/DashboardCommandExecutionServiceTests.cs`.
7. Validation passed:
- `dotnet build NxTiler.sln -c Debug`
- `dotnet test tests/NxTiler.Tests/NxTiler.Tests.csproj -c Debug` (`93/93`)
- `powershell -ExecutionPolicy Bypass -File .\scripts\governance\validate-canonical-track.ps1`
Test Cases:
1. Existing `DashboardViewModel` behavior tests remain green.
2. New tests for extracted command handlers.
3. New tests for extracted dashboard command execution policy.
Docs to update:
1. `docs/architecture/01-target-architecture.md`
2. `docs/changelog/CHANGELOG.md`

## NX-063
Status: Done  
Priority: P2  
Estimate: M

Title: Settings normalization decomposition
Scope: Split `JsonSettingsService.Normalize` into per-section normalizers.
In: section normalizers for Hotkeys, Vision, OverlayPolicies, FeatureFlags.
Out: changing settings schema semantics.
Dependencies: NX-051.
Risk: Low.
Acceptance:
1. `JsonSettingsService` keeps behavior but delegates normalization to dedicated helpers.
2. Settings migration/normalization tests remain green.
Progress:
1. Decomposed `JsonSettingsService.Normalize` into section-specific helpers:
- `NormalizeFilters`
- `NormalizeLayout`
- `NormalizePaths`
- `NormalizeRecording`
- `NormalizeHotkeys`
- `NormalizeUi`
- `NormalizeCapture`
- `NormalizeVision`
- `NormalizeFeatureFlags`
- `NormalizeDisabledSessions`
- `NormalizeSchemaVersion`
2. Preserved existing behavior and data-contract semantics.
3. Verified by full `SettingsServiceTests` regression through solution test run.
Test Cases:
1. `SettingsServiceTests` full pass.
Docs to update:
1. `docs/architecture/00-current-state.md`
2. `docs/changelog/CHANGELOG.md`

## NX-064
Status: Done  
Priority: P2  
Estimate: M

Title: YOLO engine internal split
Scope: Decompose `YoloVisionEngine` into session provider, preprocessor, and output parser components.
In: `IYoloSessionProvider`, `IYoloPreprocessor`, `IYoloOutputParser` internal interfaces/classes.
Out: changing public `IVisionEngine` contract.
Dependencies: NX-032.
Risk: Medium.
Acceptance:
1. `YoloVisionEngine` acts as orchestration shell with extracted internals.
2. Existing detection output parity is preserved within tolerance.
Progress:
1. Introduced internal YOLO component contracts:
- `src/NxTiler.Infrastructure/Vision/IYoloSessionProvider.cs`
- `src/NxTiler.Infrastructure/Vision/IYoloPreprocessor.cs`
- `src/NxTiler.Infrastructure/Vision/IYoloOutputParser.cs`
2. Added extracted implementations:
- `src/NxTiler.Infrastructure/Vision/YoloSessionProvider.cs`
- `src/NxTiler.Infrastructure/Vision/YoloPreprocessor.cs`
- `src/NxTiler.Infrastructure/Vision/YoloOutputParser.cs`
- `src/NxTiler.Infrastructure/Vision/YoloDetectionPostProcessor.cs`
3. Refactored `src/NxTiler.Infrastructure/Vision/YoloVisionEngine.cs` into orchestration shell preserving `IVisionEngine` contract.
4. Added focused component tests:
- `tests/NxTiler.Tests/YoloPreprocessorTests.cs`
- `tests/NxTiler.Tests/YoloOutputParserTests.cs`
- `tests/NxTiler.Tests/YoloDetectionPostProcessorTests.cs`
5. Validation passed: `dotnet build`, `dotnet test`, canonical governance check.
Test Cases:
1. `YoloVisionEngineTests` pass.
2. Additional parser/preprocessor unit tests.
Docs to update:
1. `docs/architecture/01-target-architecture.md`
2. `docs/testing/TEST-PLAN.md`

## NX-065
Status: Done  
Priority: P2  
Estimate: L

Title: Integrated library structural cleanup
Scope: Decompose oversized classes/files in integrated libraries without changing public behavior.
In: split `WindowCaptureCL` exception catalog, split `ImageSearchCL` tracking internals, split `WindowManagerCL` query/command concerns.
Out: breaking public API contracts.
Dependencies: NX-002.
Risk: Medium.
Acceptance:
1. Large library files are split into focused units with unchanged public API.
2. Solution build and tests remain green.
Progress:
1. Discovery refresh completed against integrated libraries:
- `src/WindowCaptureCL/API/Exceptions.cs` (~771 LOC, now split)
- `src/WindowCaptureCL/Core/CaptureSession.cs` (~627 LOC)
- `src/ImageSearchCL/Core/TrackingSession.cs` (~659 LOC)
- `src/ImageSearchCL/Infrastructure/DebugOverlay.cs` (~631 LOC)
- `src/WindowManagerCL/WindowManagerCL/API/WindowControl.cs` (~584 LOC)
2. Refactor slicing candidates documented in `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md` with low-risk decomposition order.
3. Completed first structural slice in `WindowCaptureCL`:
- removed monolithic `src/WindowCaptureCL/API/Exceptions.cs`
- added grouped files:
  - `src/WindowCaptureCL/API/CaptureExceptionBase.cs`
  - `src/WindowCaptureCL/API/CapturePlatformExceptions.cs`
  - `src/WindowCaptureCL/API/CaptureResourceExceptions.cs`
  - `src/WindowCaptureCL/API/CaptureValidationExceptions.cs`
  - `src/WindowCaptureCL/API/CaptureSessionExceptions.cs`
4. Completed second structural slice in `WindowManagerCL`:
- `src/WindowManagerCL/WindowManagerCL/API/WindowControl.cs` converted to `partial`.
- command/mutation methods extracted into:
  - `src/WindowManagerCL/WindowManagerCL/API/WindowControl.Commands.cs`
5. Extended `WindowManagerCL` split:
- query properties extracted to `src/WindowManagerCL/WindowManagerCL/API/WindowControl.Properties.cs`
- hierarchy helpers extracted to `src/WindowManagerCL/WindowManagerCL/API/WindowControl.Hierarchy.cs`
- object overrides extracted to `src/WindowManagerCL/WindowManagerCL/API/WindowControl.Overrides.cs`
6. Started `ImageSearchCL` tracking decomposition:
- `src/ImageSearchCL/Core/TrackingSession.cs` converted to `partial`
- private frame-processing/state-event methods extracted to:
  - `src/ImageSearchCL/Core/TrackingSession.Processing.cs`
8. Lifecycle/public-session methods extracted to:
  - `src/ImageSearchCL/Core/TrackingSession.Lifecycle.cs`
9. Started `ImageSearchCL` debug-overlay decomposition:
- `src/ImageSearchCL/Infrastructure/DebugOverlay.cs` converted to `partial`
- interop declarations extracted to:
  - `src/ImageSearchCL/Infrastructure/DebugOverlay.Interop.cs`
10. Extended `ImageSearchCL` debug-overlay decomposition:
- runtime/orchestration methods extracted to:
  - `src/ImageSearchCL/Infrastructure/DebugOverlay.Runtime.cs`
- overlay window rendering type extracted to:
  - `src/ImageSearchCL/Infrastructure/DebugOverlay.OverlayWindow.cs`
11. Started `WindowCaptureCL` capture-session decomposition:
- `src/WindowCaptureCL/Core/CaptureSession.cs` converted to `partial` core shell
- WGC init/event internals extracted to:
  - `src/WindowCaptureCL/Core/CaptureSession.Wgc.cs`
- single-frame/frame-conversion internals extracted to:
  - `src/WindowCaptureCL/Core/CaptureSession.Frame.cs`
- lifecycle/config/disposal methods extracted to:
  - `src/WindowCaptureCL/Core/CaptureSession.Lifecycle.cs`
13. Extended `WindowCaptureCL` API facade decomposition:
- `src/WindowCaptureCL/API/CaptureFacade.cs` converted to `partial` root
- window capture methods moved to:
  - `src/WindowCaptureCL/API/CaptureFacade.Window.cs`
- monitor/region capture methods moved to:
  - `src/WindowCaptureCL/API/CaptureFacade.Screen.cs`
- file-save methods moved to:
  - `src/WindowCaptureCL/API/CaptureFacade.File.cs`
- defaults/settings methods moved to:
  - `src/WindowCaptureCL/API/CaptureFacade.Settings.cs`
- utility/introspection methods moved to:
  - `src/WindowCaptureCL/API/CaptureFacade.Utility.cs`
- `WindowInfo` moved to dedicated file:
  - `src/WindowCaptureCL/API/WindowInfo.cs`
14. Extended `WindowManagerCL` exception decomposition:
- removed monolithic `src/WindowManagerCL/WindowManagerCL/API/Exceptions.cs`
- added grouped files:
  - `src/WindowManagerCL/WindowManagerCL/API/WindowManagerException.cs`
  - `src/WindowManagerCL/WindowManagerCL/API/WindowNotFoundException.cs`
  - `src/WindowManagerCL/WindowManagerCL/API/WindowOperationException.cs`
  - `src/WindowManagerCL/WindowManagerCL/API/InvalidWindowHandleException.cs`
12. Extended `ImageSearchCL` tracking decomposition:
- state transition and event-marshalling internals extracted to:
  - `src/ImageSearchCL/Core/TrackingSession.State.cs`
15. Extended `ImageSearchCL` infrastructure decomposition:
- `src/ImageSearchCL/Infrastructure/TemplateMatchingEngine.cs` converted to partial core shell.
- single-match flow extracted to `src/ImageSearchCL/Infrastructure/TemplateMatchingEngine.Single.cs`.
- multi-match/NMS flow extracted to `src/ImageSearchCL/Infrastructure/TemplateMatchingEngine.Multi.cs`.
16. Extended `WindowCaptureCL` DirectX decomposition:
- `src/WindowCaptureCL/Infrastructure/DirectX/FrameProcessor.cs` converted to partial core shell.
- staging-texture pool/lifecycle extracted to `src/WindowCaptureCL/Infrastructure/DirectX/FrameProcessor.Pool.cs`.
- bitmap conversion overloads extracted to `src/WindowCaptureCL/Infrastructure/DirectX/FrameProcessor.Conversion.cs`.
- async conversion extracted to `src/WindowCaptureCL/Infrastructure/DirectX/FrameProcessor.Async.cs`.
17. Extended `WindowCaptureCL` DPI helper decomposition:
- `src/WindowCaptureCL/Infrastructure/DpiHelper.cs` converted to partial core shell.
- DPI-awareness setup methods moved to `src/WindowCaptureCL/Infrastructure/DpiHelper.Awareness.cs`.
- DPI query/conversion methods moved to `src/WindowCaptureCL/Infrastructure/DpiHelper.Query.cs`.
- native interop declarations moved to `src/WindowCaptureCL/Infrastructure/DpiHelper.Native.cs`.
18. Extended `ImageSearchCL` API contract decomposition:
- `src/ImageSearchCL/API/FindResult.cs` converted to partial core model shell.
- geometry helpers moved to `src/ImageSearchCL/API/FindResult.Geometry.cs`.
- equality/string overrides moved to `src/ImageSearchCL/API/FindResult.Equality.cs`.
- `src/ImageSearchCL/API/IObjectSearch.cs` converted to partial contract shell.
- event contracts moved to `src/ImageSearchCL/API/IObjectSearch.Events.cs`.
- state/lifecycle/wait contracts moved to `src/ImageSearchCL/API/IObjectSearch.Contract.cs`.
19. Extended `ImageSearchCL` configuration decomposition:
- `src/ImageSearchCL/API/TrackingConfiguration.cs` converted to partial core shell.
- validation helpers moved to `src/ImageSearchCL/API/TrackingConfiguration.Validation.cs`.
- immutable withers moved to `src/ImageSearchCL/API/TrackingConfiguration.Withers.cs`.
20. Extended `ImageSearchCL` infrastructure queue decomposition:
- `src/ImageSearchCL/Infrastructure/FrameQueue.cs` converted to partial core shell.
- queue operations moved to `src/ImageSearchCL/Infrastructure/FrameQueue.Operations.cs`.
- lifecycle/disposal moved to `src/ImageSearchCL/Infrastructure/FrameQueue.Lifecycle.cs`.
21. Extended `WindowCaptureCL` DirectX device-manager decomposition:
- `src/WindowCaptureCL/Infrastructure/DirectX/DirectXDeviceManager.cs` converted to partial core shell.
- device initialization moved to `src/WindowCaptureCL/Infrastructure/DirectX/DirectXDeviceManager.Initialization.cs`.
- staging-texture resource allocation moved to `src/WindowCaptureCL/Infrastructure/DirectX/DirectXDeviceManager.Resources.cs`.
- lifecycle/disposal moved to `src/WindowCaptureCL/Infrastructure/DirectX/DirectXDeviceManager.Lifecycle.cs`.
22. Extended `WindowManagerCL` facade decomposition:
- `src/WindowManagerCL/WindowManagerCL/API/Window.cs` converted to partial core shell.
- search/discovery methods moved to `src/WindowManagerCL/WindowManagerCL/API/Window.Find.cs`.
- handle/foreground access methods moved to `src/WindowManagerCL/WindowManagerCL/API/Window.Handle.cs`.
23. Extended `ImageSearchCL` reference-image decomposition:
- `src/ImageSearchCL/API/ReferenceImage.cs` converted to partial core shell.
- static factory helpers moved to `src/ImageSearchCL/API/ReferenceImage.Factory.cs`.
- lifecycle/format-validation helpers moved to `src/ImageSearchCL/API/ReferenceImage.Lifecycle.cs`.
24. Extended `ImageSearchCL` one-shot search facade decomposition:
- `src/ImageSearchCL/API/ImageSearch.cs` converted to partial core shell.
- single-match entry points moved to `src/ImageSearchCL/API/ImageSearch.Find.cs`.
- multi-match entry points moved to `src/ImageSearchCL/API/ImageSearch.FindAll.cs`.
25. Extended `WindowManagerCL` core finder decomposition:
- `src/WindowManagerCL/WindowManagerCL/Core/WindowFinder.cs` converted to partial core shell.
- enumeration moved to `src/WindowManagerCL/WindowManagerCL/Core/WindowFinder.Find.cs`.
- filter/regex helpers moved to `src/WindowManagerCL/WindowManagerCL/Core/WindowFinder.Filter.cs`.
26. Extended `ImageSearchCL` overlay-window decomposition:
- `src/ImageSearchCL/Infrastructure/DebugOverlay.OverlayWindow.cs` converted to partial core shell.
- layered rendering flow moved to `src/ImageSearchCL/Infrastructure/DebugOverlay.OverlayWindow.Rendering.cs`.
- native interop moved to `src/ImageSearchCL/Infrastructure/DebugOverlay.OverlayWindow.Native.cs`.
27. Extended `WindowManagerCL` infrastructure decomposition:
- `src/WindowManagerCL/WindowManagerCL/Infrastructure/WinApi.cs` converted to partial shell.
- P/Invoke declarations moved to `src/WindowManagerCL/WindowManagerCL/Infrastructure/WinApi.Functions.cs`.
- native structs moved to `src/WindowManagerCL/WindowManagerCL/Infrastructure/WinApi.Structures.cs`.
- Win32 constants moved to `src/WindowManagerCL/WindowManagerCL/Infrastructure/WinApi.Constants.cs`.
28. Extended `WindowCaptureCL` WGC enumeration decomposition:
- `src/WindowCaptureCL/Infrastructure/WGC/WindowEnumerator.cs` converted to partial shell.
- Win32 interop declarations moved to `src/WindowCaptureCL/Infrastructure/WGC/WindowEnumerator.Interop.cs`.
- `WindowInfo` moved to dedicated file `src/WindowCaptureCL/Infrastructure/WGC/WindowInfo.cs`.
29. Extended `WindowCaptureCL` pool decomposition:
- separated `KeyedObjectPool<TKey, TValue>` from `src/WindowCaptureCL/Infrastructure/ObjectPool.cs`.
- added dedicated file `src/WindowCaptureCL/Infrastructure/KeyedObjectPool.cs`.
30. Final validation pass after latest decomposition slices:
- `dotnet build NxTiler.sln -c Debug`
- `dotnet test tests/NxTiler.Tests/NxTiler.Tests.csproj -c Debug --no-build` (`99/99`)
- `powershell -ExecutionPolicy Bypass -File .\scripts\governance\validate-canonical-track.ps1`
Test Cases:
1. Full solution build + `NxTiler.Tests` pass.
2. Added focused tests around touched library components where gaps exist.
Docs to update:
1. `docs/architecture/02-integration-map.md`
2. `docs/changelog/CHANGELOG.md`

## NX-066
Status: Done  
Priority: P2  
Estimate: M

Title: ImageSearchCL API facade decomposition
Scope: Split large `ImageSearchCL/API` facade types into focused files without changing public API behavior.
In: structural split of `Search`, `ImageSearchConfiguration`, and `Images` while preserving signatures/contracts.
Out: changing matching algorithms or runtime behavior.
Dependencies: NX-065.
Risk: Medium.
Acceptance:
1. `Search`/config API files are decomposed into focused partial/domain files.
2. Public API signatures and behavior remain unchanged.
3. Build/tests/governance checks remain green.
Progress:
1. `Search` facade decomposed into focused files:
- `src/ImageSearchCL/API/Search.cs`
- `src/ImageSearchCL/API/Search.For.cs`
- `src/ImageSearchCL/API/Search.Find.cs`
- `src/ImageSearchCL/API/Search.SearchBuilder.cs`
2. `ImageSearchConfiguration` decomposed into focused files:
- `src/ImageSearchCL/API/ImageSearchConfiguration.cs`
- `src/ImageSearchCL/API/ImageSearchConfiguration.Properties.cs`
- `src/ImageSearchCL/API/TemplateMatchMode.cs`
3. `Images` collection API decomposed into focused files:
- `src/ImageSearchCL/API/Images.cs`
- `src/ImageSearchCL/API/Images.Collection.cs`
- `src/ImageSearchCL/API/Images.Factory.cs`
- `src/ImageSearchCL/API/Images.Lifecycle.cs`
4. Added API parity tests:
- `tests/NxTiler.Tests/ImageSearchApiFacadeTests.cs`
5. Validation passed:
- `dotnet build NxTiler.sln -c Debug`
- `dotnet test tests/NxTiler.Tests/NxTiler.Tests.csproj -c Debug` (`99/99`)
- `powershell -ExecutionPolicy Bypass -File .\scripts\governance\validate-canonical-track.ps1`
Test Cases:
1. Existing `NxTiler.Tests` suite remains green.
2. Added focused API parity tests for `Search.For/ForAny/Find/FindAll` builder and fluent paths.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-067
Status: Done  
Priority: P1  
Estimate: L

Title: Recording engine command/pipeline decomposition
Scope: Reduce complexity in `FfmpegRecordingEngine` and `WgcVideoRecordingEngine` by extracting shared command assembly and frame-pipe orchestration helpers.
In: Split argument construction, lifecycle transitions, and mask/frame feed helpers into focused internal components.
Out: Changing recording workflow behavior or FFmpeg feature set.
Dependencies: NX-060, NX-061.
Risk: Medium.
Acceptance:
1. Recording engines shrink to orchestration shells with unchanged public contracts.
2. Start/stop/pause/resume behavior remains parity with existing tests.
Progress:
1. Started decomposition slice 1 for recording engines:
- `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.cs` converted to partial orchestration shell.
- extracted files:
  - `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.Arguments.cs`
  - `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.Finalize.cs`
  - `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.Lifecycle.cs`
2. Started decomposition slice 1 for WGC engine:
- `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.cs` converted to partial orchestration shell.
- extracted files:
  - `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.Arguments.cs`
  - `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.FramePump.cs`
  - `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.Lifecycle.cs`
  - `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.Masking.cs`
  - `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.Geometry.cs`
3. Validation passed after split:
- `dotnet build NxTiler.sln -c Debug`
- `dotnet test tests/NxTiler.Tests/NxTiler.Tests.csproj -c Debug --no-build` (`99/99`)
- `powershell -ExecutionPolicy Bypass -File .\scripts\governance\validate-canonical-track.ps1`
4. Added shared ffmpeg argument builder and focused parity tests:
- `src/NxTiler.Infrastructure/Recording/FfmpegArgumentBuilder.cs`
- `tests/NxTiler.Tests/FfmpegArgumentBuilderTests.cs`
5. Validation passed after shared-builder slice:
- `dotnet build NxTiler.sln -c Debug`
- `dotnet test tests/NxTiler.Tests/NxTiler.Tests.csproj -c Debug --no-build` (`103/103`)
- `powershell -ExecutionPolicy Bypass -File .\scripts\governance\validate-canonical-track.ps1`
Test Cases:
1. Existing `RecordingWorkflowServiceTests` remain green.
2. Add focused unit tests for extracted argument/pipeline builders.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-068
Status: Done  
Priority: P1  
Estimate: M

Title: Recording workflow service decomposition
Scope: Decompose `RecordingWorkflowService` into routing, messaging, and output-finalization collaborators.
In: Extract internal services for engine routing and user-status projection while keeping `IRecordingWorkflowService` contract unchanged.
Out: UX redesign of recording controls.
Dependencies: NX-061.
Risk: Medium.
Acceptance:
1. `RecordingWorkflowService` no longer concentrates routing + messaging + finalization concerns in a single class.
2. Existing workflow tests stay green.
Progress:
1. Started decomposition slice 1:
- `src/NxTiler.App/Services/RecordingWorkflowService.cs` converted to partial root shell.
- extracted files:
  - `src/NxTiler.App/Services/RecordingWorkflowService.Commands.cs`
  - `src/NxTiler.App/Services/RecordingWorkflowService.Overlay.cs`
  - `src/NxTiler.App/Services/RecordingWorkflowService.State.cs`
  - `src/NxTiler.App/Services/RecordingWorkflowService.Execution.cs`
2. Validation passed after split:
- `dotnet build NxTiler.sln -c Debug`
- `dotnet test tests/NxTiler.Tests/NxTiler.Tests.csproj -c Debug --no-build` (`99/99`)
- `powershell -ExecutionPolicy Bypass -File .\scripts\governance\validate-canonical-track.ps1`
3. Added engine-routing helper slice:
- `src/NxTiler.App/Services/RecordingWorkflowService.EngineRouting.cs`
- `RecordingWorkflowService.Commands.cs` now delegates ffmpeg resolve/start/finalize/abort routing to helper methods.
4. Validation passed after engine-routing slice:
- `dotnet build NxTiler.sln -c Debug`
- `dotnet test tests/NxTiler.Tests/NxTiler.Tests.csproj -c Debug --no-build` (`103/103`)
- `powershell -ExecutionPolicy Bypass -File .\scripts\governance\validate-canonical-track.ps1`
Test Cases:
1. Existing recording workflow test matrix.
2. Add focused tests for extracted routing/messaging collaborators.
Docs to update:
1. `docs/architecture/00-current-state.md`
2. `docs/changelog/CHANGELOG.md`

## NX-069
Status: Done  
Priority: P2  
Estimate: M

Title: Overlay window code-behind split
Scope: Split `OverlayWindow.xaml.cs` into lifecycle, placement/rendering, and interaction partials/services.
In: Structural decomposition preserving runtime behavior and bindings.
Out: Visual redesign of overlay UI.
Dependencies: NX-040, NX-041, NX-042.
Risk: Medium.
Acceptance:
1. `OverlayWindow` behavior remains unchanged.
2. Code-behind complexity is reduced through focused files.
Progress:
1. Started decomposition slice 1:
- `src/NxTiler.App/Views/OverlayWindow.xaml.cs` converted to partial root shell.
- extracted files:
  - `src/NxTiler.App/Views/OverlayWindow.Interaction.cs`
  - `src/NxTiler.App/Views/OverlayWindow.Positioning.cs`
  - `src/NxTiler.App/Views/OverlayWindow.Messaging.cs`
  - `src/NxTiler.App/Views/OverlayWindow.Tracking.cs`
2. Validation passed after split:
- `dotnet build NxTiler.sln -c Debug`
- `dotnet test tests/NxTiler.Tests/NxTiler.Tests.csproj -c Debug --no-build` (`103/103`)
- `powershell -ExecutionPolicy Bypass -File .\scripts\governance\validate-canonical-track.ps1`
Test Cases:
1. Existing overlay tracking tests remain green.
2. Add focused behavior tests for extracted interaction/placement paths where possible.
Docs to update:
1. `docs/architecture/01-target-architecture.md`
2. `docs/changelog/CHANGELOG.md`

## NX-070
Status: Done  
Priority: P2  
Estimate: M

Title: Snapshot selection window decomposition
Scope: Split `SnapshotSelectionWindow.xaml.cs` into region-selection, mask-editing, and result-projection units.
In: Keep selection and mask behavior unchanged while isolating responsibilities.
Out: New tools/mask types.
Dependencies: NX-012.
Risk: Medium.
Acceptance:
1. Region selection and mask interactions behave identically.
2. Code-behind becomes modular and easier to test.
Progress:
1. Started decomposition slice 1:
- `src/NxTiler.App/Views/SnapshotSelectionWindow.xaml.cs` converted to partial root shell.
- extracted files:
  - `src/NxTiler.App/Views/SnapshotSelectionWindow.Input.cs`
  - `src/NxTiler.App/Views/SnapshotSelectionWindow.Selection.cs`
  - `src/NxTiler.App/Views/SnapshotSelectionWindow.Layout.cs`
  - `src/NxTiler.App/Views/SnapshotSelectionWindow.Conversion.cs`
2. Validation passed after split:
- `dotnet build NxTiler.sln -c Debug`
- `dotnet test tests/NxTiler.Tests/NxTiler.Tests.csproj -c Debug --no-build` (`103/103`)
- `powershell -ExecutionPolicy Bypass -File .\scripts\governance\validate-canonical-track.ps1`
Test Cases:
1. Existing capture workflow tests remain green.
2. Add focused tests for extracted mask/selection logic.
Docs to update:
1. `docs/testing/TEST-PLAN.md`
2. `docs/changelog/CHANGELOG.md`

## NX-071
Status: Done  
Priority: P2  
Estimate: M

Title: OverlayWindowViewModel decomposition
Scope: Split `OverlayWindowViewModel` into focused partials/services for state projection, command handling, and messenger-driven updates.
In: Structural decomposition without UI behavior changes.
Out: Visual redesign of overlay interactions.
Dependencies: NX-069.
Risk: Medium.
Acceptance:
1. `OverlayWindowViewModel` behavior and bindings remain unchanged.
2. File complexity is reduced through concern-based split.
Progress:
1. Completed decomposition slice:
- `src/NxTiler.App/ViewModels/OverlayWindowViewModel.cs` converted to partial root shell.
- extracted files:
  - `src/NxTiler.App/ViewModels/OverlayWindowViewModel.Commands.cs`
  - `src/NxTiler.App/ViewModels/OverlayWindowViewModel.Messaging.cs`
  - `src/NxTiler.App/ViewModels/OverlayWindowViewModel.Snapshot.cs`
  - `src/NxTiler.App/ViewModels/OverlayWindowViewModel.Lifecycle.cs`
2. Validation passed after split:
- `dotnet build NxTiler.sln -c Debug`
- `dotnet test tests/NxTiler.Tests/NxTiler.Tests.csproj -c Debug --no-build` (`103/103`)
- `powershell -ExecutionPolicy Bypass -File .\scripts\governance\validate-canonical-track.ps1`
Test Cases:
1. Existing overlay-related tests remain green.
2. Add focused tests for extracted command/message handlers where practical.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-072
Status: Done  
Priority: P2  
Estimate: M

Title: Recording ViewModel decomposition
Scope: Split `RecordingViewModel` and `RecordingBarViewModel` into state, command, and workflow-integration units.
In: Structural split preserving command semantics and messaging.
Out: Recording UX redesign.
Dependencies: NX-068.
Risk: Medium.
Acceptance:
1. Recording view-model behavior remains unchanged.
2. Command/wiring logic becomes easier to test.
Progress:
1. Completed `RecordingViewModel` decomposition:
- `src/NxTiler.App/ViewModels/RecordingViewModel.cs` converted to partial root shell.
- extracted files:
  - `src/NxTiler.App/ViewModels/RecordingViewModel.Commands.cs`
  - `src/NxTiler.App/ViewModels/RecordingViewModel.State.cs`
  - `src/NxTiler.App/ViewModels/RecordingViewModel.Lifecycle.cs`
2. Completed `RecordingBarViewModel` decomposition:
- `src/NxTiler.App/ViewModels/RecordingBarViewModel.cs` converted to partial root shell.
- extracted files:
  - `src/NxTiler.App/ViewModels/RecordingBarViewModel.Commands.cs`
  - `src/NxTiler.App/ViewModels/RecordingBarViewModel.State.cs`
  - `src/NxTiler.App/ViewModels/RecordingBarViewModel.Lifecycle.cs`
3. Validation passed after split:
- `dotnet build NxTiler.sln -c Debug`
- `dotnet test tests/NxTiler.Tests/NxTiler.Tests.csproj -c Debug --no-build` (`103/103`)
- `powershell -ExecutionPolicy Bypass -File .\scripts\governance\validate-canonical-track.ps1`
Test Cases:
1. Existing recording workflow tests remain green.
2. Add focused view-model tests for extracted command paths.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-073
Status: Done  
Priority: P2  
Estimate: M

Title: OverlayTrackingService decomposition
Scope: Split `OverlayTrackingService` into target-resolution, visibility-policy, and placement-calculation collaborators.
In: Structural split preserving `IOverlayTrackingService` behavior and contracts.
Out: New overlay UX behaviors.
Dependencies: NX-040, NX-041, NX-042.
Risk: Medium.
Acceptance:
1. Overlay tracking behavior remains unchanged.
2. Service complexity is reduced through focused helpers/partials.
Progress:
1. Completed decomposition slice:
- `src/NxTiler.App/Services/OverlayTrackingService.cs` converted to partial root shell.
- extracted files:
  - `src/NxTiler.App/Services/OverlayTrackingService.Lifecycle.cs`
  - `src/NxTiler.App/Services/OverlayTrackingService.Loop.cs`
  - `src/NxTiler.App/Services/OverlayTrackingService.State.cs`
  - `src/NxTiler.App/Services/OverlayTrackingService.Visibility.cs`
  - `src/NxTiler.App/Services/OverlayTrackingService.Geometry.cs`
2. Validation passed after split:
- `dotnet build NxTiler.sln -c Debug`
- `dotnet test tests/NxTiler.Tests/NxTiler.Tests.csproj -c Debug --no-build` (`103/103`)
- `powershell -ExecutionPolicy Bypass -File .\scripts\governance\validate-canonical-track.ps1`
Test Cases:
1. Existing `OverlayTrackingServiceTests` remain green.
2. Add focused tests for extracted policy/placement helpers.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-074
Status: Done  
Priority: P2  
Estimate: M

Title: WgcCaptureService decomposition
Scope: Split `WgcCaptureService` into target-resolution, frame-capture, and output-persistence concerns.
In: Structural split preserving `ICaptureService` behavior and contracts.
Out: Capture feature redesign.
Dependencies: NX-011, NX-012.
Risk: Medium.
Acceptance:
1. Snapshot behavior remains unchanged.
2. Service complexity is reduced through focused helpers/partials.
Progress:
1. Completed decomposition slice:
- `src/NxTiler.Infrastructure/Capture/WgcCaptureService.cs` converted to partial root shell.
- extracted files:
  - `src/NxTiler.Infrastructure/Capture/WgcCaptureService.WindowPreparation.cs`
  - `src/NxTiler.Infrastructure/Capture/WgcCaptureService.Geometry.cs`
  - `src/NxTiler.Infrastructure/Capture/WgcCaptureService.Bitmap.cs`
  - `src/NxTiler.Infrastructure/Capture/WgcCaptureService.Output.cs`
2. Validation passed after split:
- `dotnet build NxTiler.sln -c Debug`
- `dotnet test tests/NxTiler.Tests/NxTiler.Tests.csproj -c Debug --no-build` (`103/103`)
- `powershell -ExecutionPolicy Bypass -File .\scripts\governance\validate-canonical-track.ps1`
Test Cases:
1. Existing capture workflow tests remain green.
2. Add focused tests for extracted capture/output helpers where practical.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-075
Status: Done  
Priority: P2  
Estimate: M

Title: Win32Native decomposition
Scope: Split `Win32Native` into interop declarations, constants, structures, and helper operations.
In: Structural split preserving existing Win32 contracts/behavior.
Out: Interop API redesign.
Dependencies: NX-040, NX-011.
Risk: Medium.
Acceptance:
1. All native calls/constants remain available under `Win32Native`.
2. Behavior stays unchanged with improved structure.
Progress:
1. Completed decomposition slice:
- `src/NxTiler.Infrastructure/Native/Win32Native.cs` converted to partial root shell.
- extracted files:
  - `src/NxTiler.Infrastructure/Native/Win32Native.Interop.cs`
  - `src/NxTiler.Infrastructure/Native/Win32Native.Constants.cs`
  - `src/NxTiler.Infrastructure/Native/Win32Native.Structures.cs`
  - `src/NxTiler.Infrastructure/Native/Win32Native.Helpers.cs`
2. Validation passed after split:
- `dotnet build NxTiler.sln -c Debug`
- `dotnet test tests/NxTiler.Tests/NxTiler.Tests.csproj -c Debug --no-build` (`103/103`)
- `powershell -ExecutionPolicy Bypass -File .\scripts\governance\validate-canonical-track.ps1`
Test Cases:
1. Existing overlay/capture/windowing tests remain green.
2. Add focused interop helper tests if future behavior changes touch helper logic.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-076
Status: Done  
Priority: P2  
Estimate: M

Title: VisionWorkflowService decomposition
Scope: Split `VisionWorkflowService` into engine resolution, scan execution, and fallback policy units.
In: Structural split preserving `IVisionWorkflowService` behavior and feature-flag semantics.
Out: Vision feature redesign.
Dependencies: NX-030, NX-031, NX-032.
Risk: Medium.
Acceptance:
1. Toggle/scan behavior remains unchanged.
2. YOLO->template fallback behavior remains unchanged.
Progress:
1. Completed decomposition slice:
- `src/NxTiler.App/Services/VisionWorkflowService.cs` converted to partial root shell.
- extracted files:
  - `src/NxTiler.App/Services/VisionWorkflowService.Commands.cs`
  - `src/NxTiler.App/Services/VisionWorkflowService.Scan.cs`
  - `src/NxTiler.App/Services/VisionWorkflowService.EngineResolution.cs`
  - `src/NxTiler.App/Services/VisionWorkflowService.TargetResolution.cs`
  - `src/NxTiler.App/Services/VisionWorkflowService.Execution.cs`
2. Validation passed after split:
- `dotnet build NxTiler.sln -c Debug`
- `dotnet test tests/NxTiler.Tests/NxTiler.Tests.csproj -c Debug --no-build` (`103/103`)
- `powershell -ExecutionPolicy Bypass -File .\scripts\governance\validate-canonical-track.ps1`
Test Cases:
1. Existing `VisionWorkflowServiceTests` remain green.
2. Add focused tests for engine selection and fallback routing if helper extraction changes call flow.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-077
Status: Done  
Priority: P2  
Estimate: M

Title: WorkspaceOrchestrator command/message decomposition
Scope: Split orchestration-heavy command and message handling slices into focused collaborators.
In: Structural split preserving hotkey routing, status messaging, and snapshot updates.
Out: Behavior changes in workspace arrangement or recording.
Dependencies: NX-062, NX-068.
Risk: Medium.
Acceptance:
1. Existing command routing behavior remains unchanged.
2. Existing workspace/recording integration tests remain green.
Progress:
1. Completed command decomposition slice:
- `src/NxTiler.App/Services/WorkspaceOrchestrator.Commands.cs` converted to partial shell.
- extracted files:
  - `src/NxTiler.App/Services/WorkspaceOrchestrator.Commands.Arrangement.cs`
  - `src/NxTiler.App/Services/WorkspaceOrchestrator.Commands.Navigation.cs`
  - `src/NxTiler.App/Services/WorkspaceOrchestrator.Commands.Sessions.cs`
2. Completed message-handling decomposition slice:
- `src/NxTiler.App/Services/WorkspaceOrchestrator.MessageHandling.cs` converted to partial shell.
- extracted files:
  - `src/NxTiler.App/Services/WorkspaceOrchestrator.MessageHandling.Events.cs`
  - `src/NxTiler.App/Services/WorkspaceOrchestrator.MessageHandling.Hotkeys.cs`
  - `src/NxTiler.App/Services/WorkspaceOrchestrator.MessageHandling.Execution.cs`
  - `src/NxTiler.App/Services/WorkspaceOrchestrator.MessageHandling.Registration.cs`
3. Validation passed after split:
- `dotnet build NxTiler.sln -c Debug`
- `dotnet test tests/NxTiler.Tests/NxTiler.Tests.csproj -c Debug --no-build` (`103/103`)
- `powershell -ExecutionPolicy Bypass -File .\scripts\governance\validate-canonical-track.ps1`
Test Cases:
1. Existing `WorkspaceOrchestratorTests` remain green.
2. Add focused tests for extracted command/message collaborators where practical.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-078
Status: Done  
Priority: P2  
Estimate: M

Title: JsonSettingsService decomposition
Scope: Split `JsonSettingsService` into loading, persistence, and normalization units.
In: Structural split preserving `ISettingsService` behavior and settings migration/normalization semantics.
Out: Settings schema redesign.
Dependencies: NX-063.
Risk: Medium.
Acceptance:
1. `JsonSettingsService` load/save/reload behavior remains unchanged.
2. Settings normalization behavior remains unchanged.
Progress:
1. Completed decomposition slice:
- `src/NxTiler.Infrastructure/Settings/JsonSettingsService.cs` converted to partial root shell.
- extracted files:
  - `src/NxTiler.Infrastructure/Settings/JsonSettingsService.Loading.cs`
  - `src/NxTiler.Infrastructure/Settings/JsonSettingsService.Persistence.cs`
  - `src/NxTiler.Infrastructure/Settings/JsonSettingsService.Normalization.cs`
2. Validation passed after split:
- `dotnet build NxTiler.sln -c Debug`
- `dotnet test tests/NxTiler.Tests/NxTiler.Tests.csproj -c Debug --no-build` (`103/103`)
- `powershell -ExecutionPolicy Bypass -File .\scripts\governance\validate-canonical-track.ps1`
Test Cases:
1. Existing `SettingsServiceTests` remain green.
2. Existing regression suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-079
Status: Done  
Priority: P2  
Estimate: M

Title: FfmpegRecordingEngine finalize-flow decomposition
Scope: Split `FfmpegRecordingEngine.Finalize` flows into focused helper units while preserving output behavior.
In: Structural split of finalize pipeline internals.
Out: Recording format/codec behavior changes.
Dependencies: NX-067.
Risk: Medium.
Acceptance:
1. Recording stop/finalize behavior remains unchanged.
2. Existing recording workflow tests remain green.
Progress:
1. Completed finalize decomposition slice:
- `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.Finalize.cs` reduced to orchestration shell.
- extracted files:
  - `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.Finalize.Single.cs`
  - `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.Finalize.Concat.cs`
  - `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.Finalize.Masking.cs`
2. Validation passed after split:
- `dotnet build NxTiler.sln -c Debug`
- `dotnet test tests/NxTiler.Tests/NxTiler.Tests.csproj -c Debug --no-build` (`103/103`)
- `powershell -ExecutionPolicy Bypass -File .\scripts\governance\validate-canonical-track.ps1`
Test Cases:
1. Existing `RecordingWorkflowServiceTests` remain green.
2. Existing recording engine tests remain green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-080
Status: Done  
Priority: P2  
Estimate: M

Title: SettingsViewModel decomposition
Scope: Split `SettingsViewModel` into section-specific state/command partials.
In: Structural split preserving settings UI behavior and commands.
Out: UX redesign of settings screens.
Dependencies: NX-078.
Risk: Medium.
Acceptance:
1. Settings save/apply behavior remains unchanged.
2. Existing view-model lifecycle tests remain green.
Progress:
1. Completed decomposition slice:
- `src/NxTiler.App/ViewModels/SettingsViewModel.cs` converted to partial root shell.
- extracted files:
  - `src/NxTiler.App/ViewModels/SettingsViewModel.Commands.cs`
  - `src/NxTiler.App/ViewModels/SettingsViewModel.Snapshot.cs`
2. Validation passed after split:
- `dotnet build NxTiler.sln -c Debug`
- `dotnet test tests/NxTiler.Tests/NxTiler.Tests.csproj -c Debug --no-build` (`103/103`)
- `powershell -ExecutionPolicy Bypass -File .\scripts\governance\validate-canonical-track.ps1`
Test Cases:
1. Existing `ViewModelLifecycleTests` remain green.
2. Add focused tests for extracted section command paths where practical.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-081
Status: Done  
Priority: P2  
Estimate: M

Title: WgcVideoRecordingEngine root-shell decomposition
Scope: Further split high-level orchestration in `WgcVideoRecordingEngine` root file into focused lifecycle/routing units.
In: Structural split preserving `IVideoRecordingEngine` behavior.
Out: Recording pipeline redesign.
Dependencies: NX-067.
Risk: Medium.
Acceptance:
1. WGC recording start/pause/resume/stop behavior remains unchanged.
2. Existing recording tests remain green.
Progress:
1. Completed decomposition slice:
- `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.cs` converted to partial root shell.
- extracted files:
  - `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.Start.cs`
  - `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.Control.cs`
  - `src/NxTiler.Infrastructure/Recording/WgcVideoRecordingEngine.Stop.cs`
2. Validation passed after split:
- `dotnet build NxTiler.sln -c Debug`
- `dotnet test tests/NxTiler.Tests/NxTiler.Tests.csproj -c Debug --no-build` (`103/103`)
- `powershell -ExecutionPolicy Bypass -File .\scripts\governance\validate-canonical-track.ps1`
Test Cases:
1. Existing recording workflow and engine tests remain green.
2. Add focused tests for extracted orchestration helpers where practical.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-082
Status: Done  
Priority: P2  
Estimate: M

Title: WorkspaceOrchestrator lifecycle-core decomposition
Scope: Split large `WorkspaceOrchestrator.cs` lifecycle/startup/disposal orchestration into focused partial units.
In: Structural split preserving `IWorkspaceOrchestrator` behavior.
Out: Runtime behavior changes in orchestration flow.
Dependencies: NX-077.
Risk: Medium.
Acceptance:
1. Start/dispose semantics and startup rollback behavior remain unchanged.
2. Existing orchestrator tests remain green.
Progress:
1. Completed decomposition slice:
- `src/NxTiler.App/Services/WorkspaceOrchestrator.cs` reduced to state/constructor shell.
- extracted files:
  - `src/NxTiler.App/Services/WorkspaceOrchestrator.Lifecycle.cs`
  - `src/NxTiler.App/Services/WorkspaceOrchestrator.Monitoring.cs`
2. Validation passed after split:
- `dotnet build NxTiler.sln -c Debug`
- `dotnet test tests/NxTiler.Tests/NxTiler.Tests.csproj -c Debug --no-build` (`103/103`)
- `powershell -ExecutionPolicy Bypass -File .\scripts\governance\validate-canonical-track.ps1`
Test Cases:
1. Existing `WorkspaceOrchestratorTests` remain green.
2. Add focused tests around lifecycle rollback paths if helper extraction changes call flow.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-083
Status: Done  
Priority: P2  
Estimate: M

Title: LegacyAppSettings decomposition
Scope: Split `LegacyAppSettings` into parsing, migration-shape, and fallback helpers.
In: Structural split preserving legacy migration behavior.
Out: Settings migration logic redesign.
Dependencies: NX-078.
Risk: Medium.
Acceptance:
1. Legacy settings import behavior remains unchanged.
2. Existing settings migration tests remain green.
Progress:
1. Completed decomposition slice:
- `src/NxTiler.Infrastructure/Legacy/LegacyAppSettings.cs` converted to partial root shell.
- extracted files:
  - `src/NxTiler.Infrastructure/Legacy/LegacyAppSettings.Hotkeys.cs`
  - `src/NxTiler.Infrastructure/Legacy/LegacyAppSettings.Recording.cs`
2. Validation passed after split:
- `dotnet build NxTiler.sln -c Debug`
- `dotnet test tests/NxTiler.Tests/NxTiler.Tests.csproj -c Debug --no-build` (`103/103`)
- `powershell -ExecutionPolicy Bypass -File .\scripts\governance\validate-canonical-track.ps1`
Test Cases:
1. Existing `SettingsServiceTests` remain green.
2. Add focused tests for extracted legacy parsing helpers where practical.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-084
Status: Done  
Priority: P2  
Estimate: M

Title: FfmpegRecordingEngine root-shell decomposition
Scope: Split high-level start/segment orchestration in `FfmpegRecordingEngine.cs` into focused lifecycle/startup units.
In: Structural split preserving `IRecordingEngine` behavior.
Out: Recording pipeline behavior redesign.
Dependencies: NX-067, NX-079.
Risk: Medium.
Acceptance:
1. Start/segment lifecycle behavior remains unchanged.
2. Existing recording workflow tests remain green.
Progress:
1. Completed decomposition slice:
- `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.cs` reduced to root shell.
- extracted files:
  - `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.Start.cs`
  - `src/NxTiler.Infrastructure/Recording/FfmpegRecordingEngine.Segments.cs`
2. Validation passed after split:
- `dotnet build NxTiler.sln -c Debug`
- `dotnet test tests/NxTiler.Tests/NxTiler.Tests.csproj -c Debug --no-build` (`103/103`)
- `powershell -ExecutionPolicy Bypass -File .\scripts\governance\validate-canonical-track.ps1`
Test Cases:
1. Existing `RecordingWorkflowServiceTests` remain green.
2. Existing recording engine tests remain green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-085
Status: Done  
Priority: P2  
Estimate: M

Title: LogsViewModel decomposition
Scope: Split `LogsViewModel` into lifecycle, source-collection, and command units.
In: Structural split preserving log-view behavior.
Out: Logging UX redesign.
Dependencies: NX-062.
Risk: Medium.
Acceptance:
1. Existing logs panel behavior remains unchanged.
2. Existing view-model tests remain green.
Progress:
1. `LogsViewModel` converted to partial root shell with state/properties only.
2. Lifecycle, filtering, and command concerns extracted to focused partial files.
3. Validation green (`dotnet build`, `dotnet test`, canonical governance check).
Test Cases:
1. Existing `ViewModelLifecycleTests` remain green.
2. Add focused tests for extracted log command paths where practical.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-086
Status: Done  
Priority: P2  
Estimate: M

Title: App bootstrap decomposition
Scope: Split `App.xaml.cs` startup and host/notification wiring into focused partial units.
In: Structural split preserving app bootstrap behavior.
Out: Startup behavior redesign.
Dependencies: NX-004.
Risk: Medium.
Acceptance:
1. Startup/shutdown behavior remains unchanged.
2. Existing lifecycle tests remain green.
Progress:
1. `App.xaml.cs` reduced to root shell (`Host` field + `Services` property).
2. Host registration moved to `App.Hosting.cs`; startup/shutdown moved to `App.Lifecycle.cs`; exception handlers moved to `App.Errors.cs`.
3. Validation green (`dotnet build`, `dotnet test`, canonical governance check).
Test Cases:
1. Existing `ViewModelLifecycleTests` and startup-related tests remain green.
2. Add focused tests for extracted startup helpers where practical.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-087
Status: Done  
Priority: P2  
Estimate: M

Title: RecordingWorkflowService command-slice decomposition
Scope: Split `RecordingWorkflowService.Commands.cs` into focused command groups to reduce change concentration.
In: Structural split preserving recording workflow behavior.
Out: Recording behavior redesign.
Dependencies: NX-068.
Risk: Medium.
Acceptance:
1. Recording command behavior remains unchanged.
2. Existing recording workflow tests remain green.
Progress:
1. `RecordingWorkflowService.Commands.cs` reduced to root shell.
2. Command groups extracted into `Commands.MaskEditing`, `Commands.Control`, and `Commands.Completion` partial files.
3. Validation green (`dotnet build`, `dotnet test`, canonical governance check).
Test Cases:
1. Existing `RecordingWorkflowServiceTests` remain green.
2. Existing WGC/ffmpeg fallback tests remain green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-088
Status: Done  
Priority: P2  
Estimate: M

Title: JsonSettingsService normalization micro-split
Scope: Split `JsonSettingsService.Normalization.cs` into focused policy helpers.
In: Structural split preserving settings normalization and migration behavior.
Out: Settings policy redesign.
Dependencies: NX-078.
Risk: Medium.
Acceptance:
1. Normalization behavior remains unchanged.
2. Existing settings tests remain green.
Progress:
1. `JsonSettingsService.Normalization.cs` reduced to root shell.
2. Normalization policies split into focused partial files (`Normalization.Core`, `Normalization.General`, `Normalization.Features`, `Normalization.Overlay`).
3. Validation green (`dotnet build`, `dotnet test`, canonical governance check).
Test Cases:
1. Existing settings migration/normalization tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-089
Status: Done  
Priority: P2  
Estimate: M

Title: ArrangementService decomposition
Scope: Split `ArrangementService` into focused layout strategy and coordinate helper units.
In: Structural split preserving arrangement behavior.
Out: Layout algorithm redesign.
Dependencies: NX-010.
Risk: Medium.
Acceptance:
1. Arrangement outputs remain behaviorally unchanged.
2. Existing arrangement-related tests remain green.
Progress:
1. `ArrangementService.cs` reduced to orchestration shell (`BuildPlacements` + shared constant).
2. Strategy/algorithm concerns extracted into `ArrangementService.Grid.cs`, `ArrangementService.Focus.cs`, and `ArrangementService.Layout.cs`.
3. Validation green (`dotnet build`, `dotnet test`, canonical governance check).
Test Cases:
1. Existing `ArrangementServiceTests` and workspace orchestration tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-090
Status: Done  
Priority: P2  
Estimate: M

Title: Dashboard command-slice micro-split
Scope: Split `DashboardViewModel.Commands.cs` into focused command groups to reduce command-surface concentration.
In: Structural split preserving dashboard command behavior.
Out: Dashboard UX/behavior redesign.
Dependencies: NX-062.
Risk: Medium.
Acceptance:
1. Dashboard command behavior remains unchanged.
2. Existing dashboard/workspace tests remain green.
Progress:
1. `DashboardViewModel.Commands.cs` reduced to root shell.
2. Command concerns extracted into focused partial files: `Commands.Workspace`, `Commands.Recording`, `Commands.State`, `Commands.Execution`.
3. Validation green (`dotnet build`, `dotnet test`, canonical governance check).
Test Cases:
1. Existing dashboard command tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-091
Status: Done  
Priority: P2  
Estimate: M

Title: Overlay tracking code-behind micro-split
Scope: Split `OverlayWindow.Tracking.cs` into focused tracking, transform, and scheduling helpers.
In: Structural split preserving overlay tracking behavior.
Out: Overlay behavior redesign.
Dependencies: NX-069.
Risk: Medium.
Acceptance:
1. Overlay tracking behavior remains unchanged.
2. Existing overlay tracking tests remain green.
Progress:
1. `OverlayWindow.Tracking.cs` reduced to root shell.
2. Tracking concerns extracted into focused partial files: `Tracking.Lifecycle`, `Tracking.State`, `Tracking.Scale`.
3. Validation green (`dotnet build`, `dotnet test`, canonical governance check).
Test Cases:
1. Existing `OverlayTrackingServiceTests` remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-092
Status: Done  
Priority: P2  
Estimate: M

Title: HotkeysViewModel decomposition
Scope: Split `HotkeysViewModel` into load/projection, validation, and save command units.
In: Structural split preserving hotkey editing behavior.
Out: Hotkey feature redesign.
Dependencies: NX-003.
Risk: Medium.
Acceptance:
1. Hotkeys UI behavior remains unchanged.
2. Existing hotkeys tests remain green.
Progress:
1. `HotkeysViewModel.cs` reduced to root shell with state and ctor wiring.
2. Command and projection concerns extracted into `HotkeysViewModel.Commands.cs` and `HotkeysViewModel.Snapshot.cs`.
3. Validation green (`dotnet build`, `dotnet test`, canonical governance check).
Test Cases:
1. Existing hotkeys-related tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-093
Status: Done  
Priority: P2  
Estimate: M

Title: RecordingOverlayService decomposition
Scope: Split `RecordingOverlayService` into focused lifecycle, window-hosting, and interaction helper units.
In: Structural split preserving recording overlay behavior.
Out: Overlay UX redesign.
Dependencies: NX-021.
Risk: Medium.
Acceptance:
1. Recording overlay behavior remains unchanged.
2. Existing recording overlay tests remain green.
Progress:
1. `RecordingOverlayService.cs` reduced to root shell.
2. Concerns extracted into focused partial files: `RecordingOverlayService.Lifecycle.cs`, `RecordingOverlayService.Mode.cs`, `RecordingOverlayService.Masks.cs`, `RecordingOverlayService.Ui.cs`.
3. Validation green (`dotnet build`, `dotnet test`, canonical governance check).
Test Cases:
1. Existing recording workflow and overlay tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-094
Status: Done  
Priority: P2  
Estimate: M

Title: WindowEventMonitorService decomposition
Scope: Split `WindowEventMonitorService` into hook lifecycle and event projection units.
In: Structural split preserving monitor behavior.
Out: Monitor behavior redesign.
Dependencies: NX-082.
Risk: Medium.
Acceptance:
1. Window monitor behavior remains unchanged.
2. Existing orchestrator/window-monitor tests remain green.
Progress:
1. `WindowEventMonitorService.cs` reduced to root shell.
2. Hook lifecycle and event handling concerns extracted into `WindowEventMonitorService.Lifecycle.cs` and `WindowEventMonitorService.Events.cs`.
3. Validation green (`dotnet build`, `dotnet test`, canonical governance check).
Test Cases:
1. Existing workspace orchestration tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-095
Status: Done  
Priority: P2  
Estimate: M

Title: MaskOverlayViewModel decomposition
Scope: Split `MaskOverlayViewModel` into command handling and state projection units.
In: Structural split preserving mask editor behavior.
Out: Mask UX redesign.
Dependencies: NX-070.
Risk: Medium.
Acceptance:
1. Mask overlay behavior remains unchanged.
2. Existing recording/mask-related tests remain green.
Progress:
1. `MaskOverlayViewModel.cs` reduced to root shell.
2. Concerns extracted into focused partial files: `MaskOverlayViewModel.Masks.cs`, `MaskOverlayViewModel.State.cs`, and `MaskOverlayViewModel.Commands.cs`.
3. Validation green (`dotnet build`, `dotnet test`, canonical governance check).
Test Cases:
1. Existing recording workflow tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-096
Status: Done  
Priority: P2  
Estimate: M

Title: FfmpegSetupService decomposition
Scope: Split `FfmpegSetupService` into detection/probe and download/install helper units.
In: Structural split preserving setup behavior.
Out: Setup behavior redesign.
Dependencies: NX-003.
Risk: Medium.
Acceptance:
1. FFmpeg setup behavior remains unchanged.
2. Existing setup-related tests remain green.
Progress:
1. `FfmpegSetupService.cs` reduced to root shell.
2. Setup concerns extracted into focused partial files: `FfmpegSetupService.Resolve.cs`, `FfmpegSetupService.Download.cs`, `FfmpegSetupService.Probe.cs`.
3. Validation green (`dotnet build`, `dotnet test`, canonical governance check).
Test Cases:
1. Existing recording/setup tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-097
Status: Done  
Priority: P2  
Estimate: M

Title: YoloVisionEngine micro-decomposition
Scope: Split `YoloVisionEngine` orchestration into focused inference and settings-resolution helper units.
In: Structural split preserving YOLO engine behavior.
Out: Vision behavior redesign.
Dependencies: NX-064.
Risk: Medium.
Acceptance:
1. YOLO engine behavior remains unchanged.
2. Existing vision tests remain green.
Progress:
1. `YoloVisionEngine.cs` reduced to root shell.
2. Orchestration concerns extracted into focused partial files: `YoloVisionEngine.Detection.cs`, `YoloVisionEngine.Configuration.cs`, `YoloVisionEngine.Lifecycle.cs`.
3. Validation green (`dotnet build`, `dotnet test`, canonical governance check).
Test Cases:
1. Existing `YoloVisionEngineTests` remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-098
Status: Done  
Priority: P2  
Estimate: M

Title: CaptureWorkflowService decomposition
Scope: Split `CaptureWorkflowService` into target-resolution, capture execution, and output reporting helpers.
In: Structural split preserving capture workflow behavior.
Out: Capture behavior redesign.
Dependencies: NX-011.
Risk: Medium.
Acceptance:
1. Capture workflow behavior remains unchanged.
2. Existing capture tests remain green.
Progress:
1. `CaptureWorkflowService.cs` reduced to root shell.
2. Workflow concerns extracted into focused partial files: `CaptureWorkflowService.Commands.cs`, `CaptureWorkflowService.TargetResolution.cs`, `CaptureWorkflowService.Execution.cs`.
3. Validation green (`dotnet build`, `dotnet test`, canonical governance check).
Test Cases:
1. Existing capture workflow tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-099
Status: Done  
Priority: P2  
Estimate: M

Title: NomachineSessionService decomposition
Scope: Split `NomachineSessionService` into parsing, scan traversal, and filtering helpers.
In: Structural split preserving session discovery behavior.
Out: Session behavior redesign.
Dependencies: NX-062.
Risk: Medium.
Acceptance:
1. Session discovery behavior remains unchanged.
2. Existing session-related tests remain green.
Progress:
1. `NomachineSessionService.cs` reduced to root shell.
2. Service concerns extracted into focused partial files: `NomachineSessionService.Discovery.cs`, `NomachineSessionService.Launch.cs`, `NomachineSessionService.Filtering.cs`.
3. Validation green (`dotnet build`, `dotnet test`, canonical governance check).
Test Cases:
1. Existing session/service tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-100
Status: Done  
Priority: P2  
Estimate: M

Title: SnapshotSelectionWindow input-flow micro-split
Scope: Split `SnapshotSelectionWindow.Input.cs` into pointer-flow and keyboard-action helper units.
In: Structural split preserving snapshot selection behavior.
Out: Snapshot UX redesign.
Dependencies: NX-070.
Risk: Medium.
Acceptance:
1. Snapshot selection behavior remains unchanged.
2. Existing snapshot/overlay tests remain green.
Progress:
1. `SnapshotSelectionWindow.Input.cs` reduced to root shell.
2. Input concerns extracted into `SnapshotSelectionWindow.Input.Keyboard.cs` and `SnapshotSelectionWindow.Input.Pointer.cs`.
3. Validation green (`dotnet build`, `dotnet test`, canonical governance check).
Test Cases:
1. Existing capture and overlay tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-101
Status: Done  
Priority: P2  
Estimate: M

Title: GlobalHotkeyService micro-split
Scope: Split `GlobalHotkeyService` into registration, lifecycle, and Win32 message handler units.
In: Structural split preserving hotkey registration behavior.
Out: Hotkey behavior redesign.
Dependencies: NX-003.
Risk: Medium.
Acceptance:
1. Global hotkey behavior remains unchanged.
2. Existing orchestrator/hotkey tests remain green.
Progress:
1. `GlobalHotkeyService.cs` reduced to root shell.
2. Service concerns extracted into `GlobalHotkeyService.Registration.cs`, `GlobalHotkeyService.Lifecycle.cs`, and `GlobalHotkeyService.WndProc.cs`.
3. Validation green (`dotnet build`, `dotnet test`, canonical governance check).
Test Cases:
1. Existing workspace orchestration tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-102
Status: Done  
Priority: P2  
Estimate: M

Title: MaskOverlayWindow code-behind decomposition
Scope: Split `MaskOverlayWindow.xaml.cs` into pointer-input, keyboard, and visual-update helper units.
In: Structural split preserving mask overlay window behavior.
Out: Overlay behavior redesign.
Dependencies: NX-069.
Risk: Medium.
Acceptance:
1. Mask overlay window behavior remains unchanged.
2. Existing overlay tests remain green.
Progress:
1. `MaskOverlayWindow.xaml.cs` reduced to root shell.
2. Code-behind concerns extracted into focused partial files: `MaskOverlayWindow.Lifecycle.cs`, `MaskOverlayWindow.Mode.cs`, `MaskOverlayWindow.Input.cs`.
3. Validation green (`dotnet build`, `dotnet test`, canonical governance check).
Test Cases:
1. Existing overlay/mask tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-103
Status: Done  
Priority: P2  
Estimate: M

Title: YoloOutputParser decomposition
Scope: Split `YoloOutputParser` into tensor-shape routing and candidate-extraction helpers.
In: Structural split preserving parser behavior.
Out: Vision parsing behavior redesign.
Dependencies: NX-097.
Risk: Medium.
Acceptance:
1. YOLO output parsing behavior remains unchanged.
2. Existing vision tests remain green.
Progress:
1. `YoloOutputParser.cs` reduced to root shell.
2. Parser concerns extracted into `YoloOutputParser.Parse.cs` and `YoloOutputParser.Math.cs`.
3. Validation green (`dotnet build`, `dotnet test`, canonical governance check).
Test Cases:
1. Existing `YoloVisionEngineTests` remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-104
Status: Done  
Priority: P2  
Estimate: M

Title: RecordingWorkflowService engine-routing micro-split
Scope: Split `RecordingWorkflowService.EngineRouting.cs` into start/finalize/abort helper units.
In: Structural split preserving recording engine-routing behavior.
Out: Recording behavior redesign.
Dependencies: NX-068.
Risk: Medium.
Acceptance:
1. Recording routing behavior remains unchanged.
2. Existing recording workflow tests remain green.
Progress:
1. `RecordingWorkflowService.EngineRouting.cs` reduced to root shell.
2. Engine-routing concerns extracted into `RecordingWorkflowService.EngineRouting.Start.cs` and `RecordingWorkflowService.EngineRouting.Completion.cs`.
3. Validation green (`dotnet build`, `dotnet test`, canonical governance check).
Test Cases:
1. Existing `RecordingWorkflowServiceTests` remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-105
Status: Done  
Priority: P2  
Estimate: M

Title: Win32WindowQueryService decomposition
Scope: Split `Win32WindowQueryService` into query options, filtering, and projection helpers.
In: Structural split preserving window query behavior.
Out: Window query behavior redesign.
Dependencies: NX-010.
Risk: Medium.
Acceptance:
1. Window query behavior remains unchanged.
2. Existing orchestration/query tests remain green.
Progress:
1. `Win32WindowQueryService.cs` reduced to root shell.
2. Query concerns extracted into `Win32WindowQueryService.Query.cs` and `Win32WindowQueryService.Regex.cs`.
3. Validation green (`dotnet build`, `dotnet test`, canonical governance check).
Test Cases:
1. Existing workspace orchestration tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-106
Status: Done  
Priority: P2  
Estimate: M

Title: HotkeyBox control decomposition
Scope: Split `HotkeyBox.xaml.cs` into key-capture logic and display/update helper units.
In: Structural split preserving hotkey control behavior.
Out: Hotkey control behavior redesign.
Dependencies: NX-092.
Risk: Medium.
Acceptance:
1. Hotkey control behavior remains unchanged.
2. Existing hotkey UI tests remain green.
Progress:
1. `HotkeyBox.xaml.cs` reduced to root shell (dependency properties + ctor).
2. Key/input concerns extracted to `HotkeyBox.Input.cs`, display/update concerns extracted to `HotkeyBox.Display.cs`.
3. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing hotkeys-related tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-107
Status: Done  
Priority: P2  
Estimate: M

Title: Ffmpeg finalize-masking flow decomposition
Scope: Split `FfmpegRecordingEngine.Finalize.Masking.cs` into focused masking-input, process orchestration, and output-finalization helper units.
In: Structural split preserving ffmpeg masking/finalize behavior.
Out: Recording behavior redesign.
Dependencies: NX-079.
Risk: Medium.
Acceptance:
1. Finalize masking behavior remains unchanged.
2. Existing recording tests remain green.
Progress:
1. `FfmpegRecordingEngine.Finalize.Masking.cs` reduced to orchestration shell.
2. Masking concerns extracted into focused partial files:
- `FfmpegRecordingEngine.Finalize.Masking.Filters.cs`
- `FfmpegRecordingEngine.Finalize.Masking.Process.cs`
- `FfmpegRecordingEngine.Finalize.Masking.Output.cs`
3. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing `RecordingWorkflowServiceTests` remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-108
Status: Done  
Priority: P2  
Estimate: M

Title: WorkspaceOrchestrator arrangement-flow micro-split
Scope: Split `WorkspaceOrchestrator.Arrangement.cs` into focused arrange/apply and viewport-fit helper units.
In: Structural split preserving workspace arrangement behavior.
Out: Arrangement behavior redesign.
Dependencies: NX-077.
Risk: Medium.
Acceptance:
1. Arrangement behavior remains unchanged.
2. Existing workspace orchestration tests remain green.
Progress:
1. `WorkspaceOrchestrator.Arrangement.cs` reduced to root shell.
2. Arrangement concerns extracted into focused partial files:
- `WorkspaceOrchestrator.Arrangement.Targets.cs`
- `WorkspaceOrchestrator.Arrangement.Flow.cs`
- `WorkspaceOrchestrator.Arrangement.Messaging.cs`
3. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing workspace orchestration tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-109
Status: Done  
Priority: P2  
Estimate: M

Title: RecordingBarViewModel state-surface micro-split
Scope: Split `RecordingBarViewModel.State.cs` into focused workflow-message handlers and state-projection helpers.
In: Structural split preserving recording-bar behavior.
Out: Recording-bar UX behavior redesign.
Dependencies: NX-072.
Risk: Medium.
Acceptance:
1. Recording-bar state behavior remains unchanged.
2. Existing recording view-model tests remain green.
Progress:
1. `RecordingBarViewModel.State.cs` reduced to root shell.
2. State concerns extracted into focused partial files:
- `RecordingBarViewModel.State.Workflow.cs`
- `RecordingBarViewModel.State.Timer.cs`
3. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing recording workflow/view-model tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-110
Status: Done  
Priority: P2  
Estimate: M

Title: App host-registration micro-split
Scope: Split `App.Hosting.cs` into focused registration groups for app services, infrastructure wiring, and UI host setup helpers.
In: Structural split preserving startup/DI behavior.
Out: Startup behavior redesign.
Dependencies: NX-086.
Risk: Medium.
Acceptance:
1. App startup/DI behavior remains unchanged.
2. Existing startup/lifecycle tests remain green.
Progress:
1. `App.Hosting.cs` reduced to host bootstrap + orchestration shell.
2. Service registration concerns extracted into focused partial files:
- `App.Hosting.Services.Core.cs`
- `App.Hosting.Services.Ui.cs`
- `App.Hosting.Services.Workflow.cs`
- `App.Hosting.Services.ViewModels.cs`
- `App.Hosting.Services.Views.cs`
3. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing startup/view-model lifecycle tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-111
Status: Done  
Priority: P2  
Estimate: M

Title: OverlayWindowViewModel command-surface micro-split
Scope: Split `OverlayWindowViewModel.Commands.cs` into focused overlay action commands and command-execution policy helpers.
In: Structural split preserving overlay-viewmodel behavior.
Out: Overlay behavior redesign.
Dependencies: NX-071.
Risk: Medium.
Acceptance:
1. Overlay view-model command behavior remains unchanged.
2. Existing overlay/workspace tests remain green.
Progress:
1. `OverlayWindowViewModel.Commands.cs` reduced to root shell.
2. Command concerns extracted into focused partial files:
- `OverlayWindowViewModel.Commands.Actions.cs`
- `OverlayWindowViewModel.Commands.Toggles.cs`
- `OverlayWindowViewModel.Commands.Execution.cs`
3. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing overlay-viewmodel/workspace tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-112
Status: Done  
Priority: P2  
Estimate: M

Title: VisionWorkflowService scan-flow micro-split
Scope: Split `VisionWorkflowService.Scan.cs` into focused scan trigger, candidate-evaluation, and result-application helpers.
In: Structural split preserving vision workflow behavior.
Out: Vision behavior redesign.
Dependencies: NX-076.
Risk: Medium.
Acceptance:
1. Vision scan behavior remains unchanged.
2. Existing vision workflow tests remain green.
Progress:
1. `VisionWorkflowService.Scan.cs` reduced to root shell.
2. Scan concerns extracted into focused partial files:
- `VisionWorkflowService.Scan.Run.cs`
- `VisionWorkflowService.Scan.Fallback.cs`
- `VisionWorkflowService.Scan.Request.cs`
3. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing vision workflow tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-113
Status: Done  
Priority: P2  
Estimate: M

Title: WgcVideoRecordingEngine start-flow micro-split
Scope: Split `WgcVideoRecordingEngine.Start.cs` into focused startup validation, ffmpeg bootstrap, and frame-pump boot helpers.
In: Structural split preserving WGC recording start behavior.
Out: Recording behavior redesign.
Dependencies: NX-081.
Risk: Medium.
Acceptance:
1. WGC recording start behavior remains unchanged.
2. Existing recording workflow tests remain green.
Progress:
1. `WgcVideoRecordingEngine.Start.cs` reduced to orchestration shell.
2. Start-flow concerns extracted into focused partial files:
- `WgcVideoRecordingEngine.Start.Validation.cs`
- `WgcVideoRecordingEngine.Start.Session.cs`
- `WgcVideoRecordingEngine.Start.Bootstrap.cs`
3. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing recording workflow tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-114
Status: Done  
Priority: P2  
Estimate: M

Title: RecordingViewModel command-surface micro-split
Scope: Split `RecordingViewModel.Commands.cs` into focused recording command handlers and command-execution helper units.
In: Structural split preserving recording view-model behavior.
Out: Recording UI behavior redesign.
Dependencies: NX-072.
Risk: Medium.
Acceptance:
1. Recording view-model command behavior remains unchanged.
2. Existing recording workflow/view-model tests remain green.
Progress:
1. `RecordingViewModel.Commands.cs` reduced to root shell.
2. Command concerns extracted into focused partial files:
- `RecordingViewModel.Commands.Settings.cs`
- `RecordingViewModel.Commands.Workflow.cs`
- `RecordingViewModel.Commands.Execution.cs`
3. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing recording workflow/view-model tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-115
Status: Done  
Priority: P2  
Estimate: M

Title: YoloOutputParser parse-flow micro-split
Scope: Split `YoloOutputParser.Parse.cs` into focused tensor-shape branching and candidate-collection helpers.
In: Structural split preserving YOLO parser behavior.
Out: Vision parser behavior redesign.
Dependencies: NX-103.
Risk: Medium.
Acceptance:
1. YOLO parser behavior remains unchanged.
2. Existing YOLO/vision tests remain green.
Progress:
1. `YoloOutputParser.Parse.cs` reduced to root shell.
2. Parse concerns extracted into focused partial files:
- `YoloOutputParser.Parse.Shape.cs`
- `YoloOutputParser.Parse.Candidates.cs`
3. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing `YoloOutputParserTests` remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-116
Status: Done  
Priority: P2  
Estimate: M

Title: WorkspaceOrchestrator lifecycle micro-split
Scope: Split `WorkspaceOrchestrator.Lifecycle.cs` into focused startup, shutdown/dispose, and monitor-activation helpers.
In: Structural split preserving orchestrator lifecycle behavior.
Out: Orchestrator behavior redesign.
Dependencies: NX-082.
Risk: Medium.
Acceptance:
1. Workspace orchestrator lifecycle behavior remains unchanged.
2. Existing workspace/orchestrator tests remain green.
Progress:
1. `WorkspaceOrchestrator.Lifecycle.cs` reduced to root shell.
2. Lifecycle concerns extracted into focused partial files:
- `WorkspaceOrchestrator.Lifecycle.Start.cs`
- `WorkspaceOrchestrator.Lifecycle.Dispose.cs`
- `WorkspaceOrchestrator.Lifecycle.Rollback.cs`
3. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing workspace/orchestrator tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-117
Status: Done  
Priority: P2  
Estimate: M

Title: WorkspaceOrchestrator hotkey-routing micro-split
Scope: Split `WorkspaceOrchestrator.MessageHandling.Hotkeys.cs` into focused action routing and action-handler helpers.
In: Structural split preserving hotkey routing behavior.
Out: Orchestrator behavior redesign.
Dependencies: NX-077.
Risk: Medium.
Acceptance:
1. Hotkey routing behavior remains unchanged.
2. Existing orchestrator/hotkey tests remain green.
Progress:
1. `WorkspaceOrchestrator.MessageHandling.Hotkeys.cs` reduced to root shell.
2. Hotkey concerns extracted into focused partial files:
- `WorkspaceOrchestrator.MessageHandling.Hotkeys.Routing.cs`
- `WorkspaceOrchestrator.MessageHandling.Hotkeys.Recording.cs`
- `WorkspaceOrchestrator.MessageHandling.Hotkeys.CaptureVision.cs`
3. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing orchestrator/hotkey tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-118
Status: Done  
Priority: P2  
Estimate: M

Title: OverlayWindow lifecycle micro-split
Scope: Split `OverlayWindow.xaml.cs` lifecycle handlers into focused partial file.
In: Structural split preserving overlay-window behavior.
Out: Overlay behavior redesign.
Dependencies: NX-069.
Risk: Medium.
Acceptance:
1. Overlay-window lifecycle behavior remains unchanged.
2. Existing overlay/workspace tests remain green.
Progress:
1. `OverlayWindow.xaml.cs` reduced by moving lifecycle handlers.
2. Lifecycle concerns extracted to `OverlayWindow.Lifecycle.cs`.
3. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing overlay/workspace tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-119
Status: Done  
Priority: P2  
Estimate: M

Title: LegacyAppSettings property-group micro-split
Scope: Split `LegacyAppSettings.cs` into focused property groups (`filters/layout`, `overlay/ui`, `collections`).
In: Structural split preserving legacy settings behavior.
Out: Legacy settings behavior redesign.
Dependencies: NX-083.
Risk: Medium.
Acceptance:
1. Legacy settings behavior remains unchanged.
2. Existing settings tests remain green.
Progress:
1. `LegacyAppSettings.cs` reduced to singleton/type shell.
2. Property groups extracted into focused partial files:
- `LegacyAppSettings.Filters.cs`
- `LegacyAppSettings.Layout.cs`
- `LegacyAppSettings.OverlayUi.cs`
- `LegacyAppSettings.Collections.cs`
3. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing settings tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-120
Status: Done  
Priority: P2  
Estimate: M

Title: TemplateVisionEngine detection-flow micro-split
Scope: Split `TemplateVisionEngine.cs` into focused template discovery, frame-capture, and detection-collection helpers.
In: Structural split preserving template vision behavior.
Out: Vision behavior redesign.
Dependencies: NX-031.
Risk: Medium.
Acceptance:
1. Template-vision behavior remains unchanged.
2. Existing vision tests remain green.
Progress:
1. `TemplateVisionEngine.cs` converted to partial root shell.
2. Detection/configuration concerns extracted into focused partial files:
- `TemplateVisionEngine.Detection.cs`
- `TemplateVisionEngine.Configuration.cs`
3. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing vision tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-121
Status: Done  
Priority: P2  
Estimate: M

Title: RecordingWorkflow engine-start routing micro-split
Scope: Split `RecordingWorkflowService.EngineRouting.Start.cs` into focused ffmpeg-resolve and engine-start helper units.
In: Structural split preserving recording engine-start routing behavior.
Out: Recording behavior redesign.
Dependencies: NX-104.
Risk: Medium.
Acceptance:
1. Recording engine-start routing behavior remains unchanged.
2. Existing recording workflow tests remain green.
Progress:
1. `RecordingWorkflowService.EngineRouting.Start.cs` reduced to root shell.
2. Start-routing concerns extracted into focused partial files:
- `RecordingWorkflowService.EngineRouting.Start.Ffmpeg.cs`
- `RecordingWorkflowService.EngineRouting.Start.Engine.cs`
3. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing `RecordingWorkflowServiceTests` remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-122
Status: Done  
Priority: P2  
Estimate: M

Title: RecordingBar workflow-state micro-split
Scope: Split `RecordingBarViewModel.State.Workflow.cs` into focused state-transition handlers and per-state UI projection helpers.
In: Structural split preserving recording-bar behavior.
Out: Recording-bar UI behavior redesign.
Dependencies: NX-109.
Risk: Medium.
Acceptance:
1. Recording-bar workflow-state behavior remains unchanged.
2. Existing recording workflow/view-model tests remain green.
Progress:
1. `RecordingBarViewModel.State.Workflow.cs` reduced to state-dispatch shell.
2. Per-state UI projection extracted to `RecordingBarViewModel.State.Workflow.Projection.cs`.
3. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing recording workflow/view-model tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-123
Status: Done  
Priority: P2  
Estimate: M

Title: SnapshotSelection pointer-flow micro-split
Scope: Split `SnapshotSelectionWindow.Input.Pointer.cs` into focused drag/resize routing and mask/selection update helpers.
In: Structural split preserving snapshot pointer interaction behavior.
Out: Snapshot UX redesign.
Dependencies: NX-100.
Risk: Medium.
Acceptance:
1. Snapshot pointer interaction behavior remains unchanged.
2. Existing snapshot/capture tests remain green.
Progress:
1. `SnapshotSelectionWindow.Input.Pointer.cs` reduced to root shell.
2. Pointer-input concerns extracted into focused partial files:
- `SnapshotSelectionWindow.Input.Pointer.Down.cs`
- `SnapshotSelectionWindow.Input.Pointer.Drag.cs`
- `SnapshotSelectionWindow.Input.Pointer.Context.cs`
3. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing snapshot/capture tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-124
Status: Done  
Priority: P2  
Estimate: M

Title: YoloOutputParser candidate-flow micro-split
Scope: Split `YoloOutputParser.Parse.Candidates.cs` into focused candidate scoring and bounds-projection helpers.
In: Structural split preserving YOLO parser behavior.
Out: Vision parser behavior redesign.
Dependencies: NX-115.
Risk: Medium.
Acceptance:
1. YOLO parser behavior remains unchanged.
2. Existing YOLO/vision tests remain green.
Progress:
1. Candidate orchestration in `YoloOutputParser.Parse.Candidates.cs` reduced to high-level flow.
2. Candidate scoring and bounds projection helpers extracted into:
- `YoloOutputParser.Parse.Candidates.Scoring.cs`
- `YoloOutputParser.Parse.Candidates.Bounds.cs`
3. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing `YoloOutputParserTests` remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-125
Status: Done  
Priority: P2  
Estimate: M

Title: Dashboard snapshot-projection micro-split
Scope: Split `DashboardViewModel.Snapshot.cs` into focused workspace snapshot mapping and recording-state projection helpers.
In: Structural split preserving dashboard behavior.
Out: Dashboard behavior redesign.
Dependencies: NX-062.
Risk: Medium.
Acceptance:
1. Dashboard snapshot/recording projection behavior remains unchanged.
2. Existing dashboard/workspace tests remain green.
Progress:
1. `DashboardViewModel.Snapshot.cs` reduced to root shell.
2. Snapshot concerns extracted into focused partial files:
- `DashboardViewModel.Snapshot.Messages.cs`
- `DashboardViewModel.Snapshot.Projection.cs`
3. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing dashboard/workspace tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-126
Status: Done  
Priority: P2  
Estimate: M

Title: OverlayTrackingService lifecycle micro-split
Scope: Split `OverlayTrackingService.cs` into focused start/update-target and stop/dispose lifecycle helpers.
In: Structural split preserving overlay tracking behavior.
Out: Overlay tracking behavior redesign.
Dependencies: NX-073.
Risk: Medium.
Acceptance:
1. Overlay tracking lifecycle behavior remains unchanged.
2. Existing overlay tracking tests remain green.
Progress:
1. `OverlayTrackingService.cs` reduced to core state/event shell.
2. Lifecycle concerns extracted into focused partial files:
- `OverlayTrackingService.Start.cs`
- `OverlayTrackingService.Stop.cs`
3. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing overlay tracking tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-127
Status: Done  
Priority: P2  
Estimate: M

Title: FfmpegRecordingEngine start-flow micro-split
Scope: Split `FfmpegRecordingEngine.Start.cs` into focused start validation and process-bootstrap helpers.
In: Structural split preserving ffmpeg recording start behavior.
Out: Recording behavior redesign.
Dependencies: NX-084.
Risk: Medium.
Acceptance:
1. FFmpeg recording start behavior remains unchanged.
2. Existing recording workflow tests remain green.
Progress:
1. `FfmpegRecordingEngine.Start.cs` reduced to orchestration shell.
2. Start concerns extracted into focused partial files:
- `FfmpegRecordingEngine.Start.Validation.cs`
- `FfmpegRecordingEngine.Start.Geometry.cs`
- `FfmpegRecordingEngine.Start.Session.cs`
3. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing recording workflow tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-128
Status: Done  
Priority: P2  
Estimate: M

Title: MainWindow lifecycle micro-split
Scope: Split `MainWindow.xaml.cs` into focused startup initialization and close-to-tray/exit lifecycle handlers.
In: Structural split preserving main window behavior.
Out: Main window behavior redesign.
Dependencies: NX-086.
Risk: Medium.
Acceptance:
1. Main window lifecycle behavior remains unchanged.
2. Existing startup/tray workflow tests remain green.
Progress:
1. `MainWindow.xaml.cs` reduced by extracting lifecycle handlers.
2. Lifecycle concerns moved to `MainWindow.Lifecycle.cs`.
3. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing startup/tray workflow tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-129
Status: Done  
Priority: P2  
Estimate: M

Title: Win32WindowControlService command-surface micro-split
Scope: Split `Win32WindowControlService.cs` into focused window command operations and geometry/monitor helper units.
In: Structural split preserving window-control behavior.
Out: Window-control behavior redesign.
Dependencies: NX-010.
Risk: Medium.
Acceptance:
1. Window-control behavior remains unchanged.
2. Existing workspace/window-control tests remain green.
Progress:
1. `Win32WindowControlService.cs` reduced to partial root shell.
2. Window command operations extracted to `Win32WindowControlService.Commands.cs`.
3. Placement workflow extracted to `Win32WindowControlService.Placement.cs` and geometry/monitor queries extracted to `Win32WindowControlService.Bounds.cs`.
4. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing workspace/window-control tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-130
Status: Done  
Priority: P2  
Estimate: M

Title: WgcVideoRecordingEngine lifecycle-surface micro-split
Scope: Split `WgcVideoRecordingEngine.Lifecycle.cs` into focused stop/finalize and cancel/cleanup helper units.
In: Structural split preserving WGC recording lifecycle behavior.
Out: Recording lifecycle behavior redesign.
Dependencies: NX-081.
Risk: Medium.
Acceptance:
1. WGC recording lifecycle behavior remains unchanged.
2. Existing recording workflow tests remain green.
Progress:
1. `WgcVideoRecordingEngine.Lifecycle.cs` reduced to partial root shell.
2. Stop/finalize lifecycle flow extracted to `WgcVideoRecordingEngine.Lifecycle.StopInternal.cs`.
3. Cancel/cleanup helpers extracted to `WgcVideoRecordingEngine.Lifecycle.Cleanup.cs`.
4. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing recording workflow tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-131
Status: Done  
Priority: P2  
Estimate: M

Title: AppSettingsSnapshot defaults-factory micro-split
Scope: Split `AppSettingsSnapshot.cs` into focused default-section builders and root snapshot factory shell.
In: Structural split preserving default settings values and schema behavior.
Out: Domain settings contract redesign.
Dependencies: NX-003.
Risk: Medium.
Acceptance:
1. Default snapshot values remain unchanged.
2. Existing settings serialization/normalization tests remain green.
Progress:
1. `AppSettingsSnapshot.cs` reduced to partial root shell.
2. Default optional sections extracted to `AppSettingsSnapshot.DefaultSections.cs`.
3. Default factory and section helpers extracted to `AppSettingsSnapshot.Factory.cs`.
4. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing settings serialization/normalization tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-132
Status: Done  
Priority: P2  
Estimate: M

Title: OverlayWindow tracking-lifecycle micro-split
Scope: Split `OverlayWindow.Tracking.Lifecycle.cs` into focused tracking start/stop and callback-update helper units.
In: Structural split preserving overlay tracking behavior.
Out: Overlay behavior redesign.
Dependencies: NX-091.
Risk: Medium.
Acceptance:
1. Overlay tracking behavior remains unchanged.
2. Existing overlay/window-tracking tests remain green.
Progress:
1. `OverlayWindow.Tracking.Lifecycle.cs` reduced to partial root shell.
2. Visibility change handling extracted to `OverlayWindow.Tracking.Lifecycle.Visibility.cs`.
3. Tracking start/update and stop flows extracted to `OverlayWindow.Tracking.Lifecycle.Start.cs` and `OverlayWindow.Tracking.Lifecycle.Stop.cs`.
4. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing overlay/window-tracking tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-133
Status: Done  
Priority: P2  
Estimate: M

Title: WgcCaptureService bitmap-flow micro-split
Scope: Split `WgcCaptureService.Bitmap.cs` into focused bitmap projection, mask rendering, and clipboard/output helper units.
In: Structural split preserving capture bitmap behavior.
Out: Capture algorithm redesign.
Dependencies: NX-074.
Risk: Medium.
Acceptance:
1. Snapshot rendering behavior remains unchanged.
2. Existing capture/snapshot tests remain green.
Progress:
1. `WgcCaptureService.Bitmap.cs` reduced to partial root shell.
2. Bitmap output projection flow extracted to `WgcCaptureService.Bitmap.Output.cs`.
3. Mask rendering and PNG encoding helpers extracted to `WgcCaptureService.Bitmap.Masks.cs` and `WgcCaptureService.Bitmap.Encoding.cs`.
4. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing capture/snapshot tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-134
Status: Done  
Priority: P2  
Estimate: M

Title: FfmpegSetupService download-flow micro-split
Scope: Split `FfmpegSetupService.Download.cs` into focused download, extraction, and archive discovery helper units.
In: Structural split preserving ffmpeg setup behavior.
Out: Setup behavior redesign.
Dependencies: NX-096.
Risk: Medium.
Acceptance:
1. FFmpeg setup/download behavior remains unchanged.
2. Existing ffmpeg setup tests remain green.
Progress:
1. `FfmpegSetupService.Download.cs` reduced to partial root shell.
2. Download orchestration extracted to `FfmpegSetupService.Download.Flow.cs`.
3. Transfer, extraction/discovery, and cleanup helpers extracted to `FfmpegSetupService.Download.Transfer.cs`, `FfmpegSetupService.Download.Extract.cs`, and `FfmpegSetupService.Download.Cleanup.cs`.
4. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing ffmpeg setup tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-135
Status: Done  
Priority: P2  
Estimate: M

Title: RecordingWorkflowService engine-start helper micro-split
Scope: Split `RecordingWorkflowService.EngineRouting.Start.Engine.cs` into focused WGC/legacy engine-start helper units.
In: Structural split preserving recording start-routing behavior.
Out: Recording workflow behavior redesign.
Dependencies: NX-121.
Risk: Medium.
Acceptance:
1. Recording start-routing behavior remains unchanged.
2. Existing recording workflow tests remain green.
Progress:
1. `RecordingWorkflowService.EngineRouting.Start.Engine.cs` reduced to orchestration shell.
2. Preferred WGC engine-start helper extracted to `RecordingWorkflowService.EngineRouting.Start.Engine.Preferred.cs`.
3. Legacy engine-start + start-failure helper extracted to `RecordingWorkflowService.EngineRouting.Start.Engine.Legacy.cs`.
4. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing recording workflow tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-136
Status: Done  
Priority: P2  
Estimate: M

Title: SnapshotSelectionWindow selection-flow micro-split
Scope: Split `SnapshotSelectionWindow.Selection.cs` into focused region-selection and mask-collection helper units.
In: Structural split preserving snapshot-selection behavior.
Out: Selection UX redesign.
Dependencies: NX-070.
Risk: Medium.
Acceptance:
1. Region/mask selection behavior remains unchanged.
2. Existing snapshot selection tests remain green.
Progress:
1. `SnapshotSelectionWindow.Selection.cs` reduced to partial root shell.
2. Region selection helpers extracted to `SnapshotSelectionWindow.Selection.Region.cs`.
3. Mask create/move helpers extracted to `SnapshotSelectionWindow.Selection.MaskCreate.cs` and `SnapshotSelectionWindow.Selection.MaskMove.cs`.
4. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing snapshot selection tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-137
Status: Done  
Priority: P2  
Estimate: M

Title: AppSettingsSnapshot factory-section micro-split
Scope: Split `AppSettingsSnapshot.Factory.cs` into focused base snapshot construction and feature-section helper units.
In: Structural split preserving default settings values.
Out: Settings schema redesign.
Dependencies: NX-131.
Risk: Medium.
Acceptance:
1. `CreateDefault()` output remains unchanged.
2. Existing settings tests remain green.
Progress:
1. `AppSettingsSnapshot.Factory.cs` reduced до базовой сборки default snapshot.
2. Hotkey-default helpers moved to `AppSettingsSnapshot.Factory.Hotkeys.cs`.
3. Feature-section helpers moved to `AppSettingsSnapshot.Factory.Features.cs`.
4. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing settings tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`

## NX-138
Status: Done  
Priority: P2  
Estimate: M

Title: DashboardViewModel lifecycle-surface micro-split (final refactor gate)
Scope: Split `DashboardViewModel.Lifecycle.cs` into focused lifecycle subscription/disposal and startup state-sync helper units.
In: Structural split preserving dashboard lifecycle behavior.
Out: Dashboard behavior redesign.
Dependencies: NX-062.
Risk: Medium.
Acceptance:
1. Dashboard lifecycle behavior remains unchanged.
2. Existing dashboard/view-model tests remain green.
3. Refactor Freeze Gate is activated after completion (`no new NX-1xx micro-split tasks`).
Progress:
1. `DashboardViewModel.Lifecycle.cs` reduced to partial root shell.
2. Lifecycle flow extracted to `DashboardViewModel.Lifecycle.Activate.cs`, `DashboardViewModel.Lifecycle.Deactivate.cs`, `DashboardViewModel.Lifecycle.Dispose.cs`.
3. Message registration + initial state sync extracted to `DashboardViewModel.Lifecycle.Messaging.cs`.
4. Validation green (`dotnet build NxTiler.sln -c Debug`, `dotnet test NxTiler.sln -c Debug --no-build`, canonical governance check).
Test Cases:
1. Existing dashboard/view-model tests remain green.
2. Full `NxTiler.Tests` suite remains green.
Docs to update:
1. `docs/roadmap/REFRACTOR-AUDIT-2026-02-13.md`
2. `docs/changelog/CHANGELOG.md`
 3. `docs/roadmap/ROADMAP.md`
