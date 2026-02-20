# NxTiler Test Plan

Last updated: 2026-02-13

## Scope

This document tracks acceptance-level tests for roadmap waves and regression safety.

## Baseline checks

1. `dotnet build NxTiler.sln`
2. `dotnet test NxTiler.sln`
3. `powershell -ExecutionPolicy Bypass -File .\scripts\perf\perf-smoke.ps1 -Configuration Release`
4. `powershell -ExecutionPolicy Bypass -File .\scripts\perf\perf-regression-matrix.ps1`
5. `powershell -ExecutionPolicy Bypass -File .\scripts\governance\validate-canonical-track.ps1`
6. `powershell -ExecutionPolicy Bypass -File .\scripts\release\verify-release-baseline.ps1`

## Wave-aligned scenarios

## Wave 1 (Capture)

1. Instant snapshot:
- target window is maximized
- stable bounds wait is applied
- capture is saved to disk and clipboard
- workflow unit coverage exists in `tests/NxTiler.Tests/CaptureWorkflowServiceTests.cs`
 - global hotkey route triggers snapshot workflow
 - manual validation matrix:
   - single monitor, non-overlapped target: file is created in `Capture.SnapshotFolder`, clipboard contains image
   - single monitor, overlapped target: captured image matches target window content (no overlap artifacts)
   - multi-monitor, target on secondary monitor: bounds and captured content are correct
   - DPI 100/150/200: output image dimensions remain consistent with captured bounds
   - target not found: workflow returns explicit failure message without crash
2. Shift snapshot:
- area selection works
- mask add/remove works
- mask drag/move works inside selected region
- output image contains baked masks
 - cancel path returns controlled workflow failure without writing output
 - manual validation matrix:
   - draw region then confirm: output contains selected area only
   - add 2+ masks via drag and confirm: output contains blacked regions
   - move mask inside region: mask stays clamped within selected bounds
   - right-click on mask: mask is removed
   - press `Esc` in selection flow: workflow returns canceled result

## Wave 2 (Recording)

1. Start/pause/resume/stop flow remains valid.
2. Masks are applied to final recording output.
3. Fallback engine and primary engine both pass smoke tests.
4. Workflow unit coverage includes feature-flag routing to `IVideoRecordingEngine`.
5. WGC route validates pause/resume + mask propagation on finalize and discard/abort path.
6. WGC start failure triggers legacy fallback path in the same workflow run.

## Wave 3 (Vision)

1. Template fallback mode works with no YOLO runtime.
2. YOLO mode returns detections with confidence threshold behavior.
3. Engine routing by feature flag is validated.
4. Vision mode hotkey toggles workflow state and reports scan outcome.
5. Unit coverage baseline exists in `tests/NxTiler.Tests/VisionWorkflowServiceTests.cs`:
- mode toggle on/off
- engine routing by flags
- target window resolution failure path
6. YOLO failure fallback:
- when `yolo` is selected and throws, workflow falls back to template engine if enabled.
- when template fallback is disabled, workflow returns vision failure.
7. YOLO engine config validation:
- missing model path returns explicit configuration error
- missing model file returns file-not-found error
8. Add model-based smoke suite (pending):
8. Model-based smoke suite:
- optional smoke test `tests/NxTiler.Tests/YoloModelSmokeTests.cs`
- set `NXTILER_YOLO_MODEL_SMOKE` (or `NXTILER_YOLO_MODEL`) to ONNX model path
- optional `NXTILER_YOLO_SMOKE_IMAGES` (folder or `;`-separated image files)
- verifies model inference + parser path executes without runtime failures

## Wave 4 (Overlay)

1. Overlay follows target window move/resize.
2. Visibility policies:
- `Always`
- `OnHover`
- `HideOnHover`
3. Overlay scale behavior remains correct across DPI and resize changes.
4. Tracking service baseline coverage exists in `tests/NxTiler.Tests/OverlayTrackingServiceTests.cs`.
5. Policy coverage includes cursor transition tests for `OnHover` and `HideOnHover` in `tests/NxTiler.Tests/OverlayTrackingServiceTests.cs`.
6. Settings compatibility coverage includes legacy `HideOnHover` normalization in `tests/NxTiler.Tests/SettingsServiceTests.cs`.
7. Scaling coverage includes non-uniform resize and minimum size clamping in `tests/NxTiler.Tests/OverlayTrackingServiceTests.cs`.
8. Anchor coverage includes right-edge placement and monitor-bound clamping in `tests/NxTiler.Tests/OverlayTrackingServiceTests.cs`.
9. Settings compatibility coverage includes invalid overlay anchor fallback to `TopLeft` in `tests/NxTiler.Tests/SettingsServiceTests.cs`.

## Wave 6 (Refactor)

1. Recording regression:
- start/pause/resume/stop/discard flows stay behaviorally identical after internal extraction.
2. Shared FFmpeg process support coverage:
- `tests/NxTiler.Tests/FfmpegProcessSupportTests.cs` validates timeout handling and stderr tail capture.
3. Recording transition-table coverage:
- `tests/NxTiler.Tests/RecordingWorkflowStateMachineTests.cs` validates allowed/denied transitions.
4. Settings regression:
- normalization and legacy migration tests remain stable across decomposition.
5. Vision regression:
- YOLO output consistency checks pass within confidence/IoU tolerance.
- component tests cover parser/preprocessor/NMS internals:
  - `tests/NxTiler.Tests/YoloOutputParserTests.cs`
  - `tests/NxTiler.Tests/YoloPreprocessorTests.cs`
  - `tests/NxTiler.Tests/YoloDetectionPostProcessorTests.cs`
6. Governance and perf gates remain green after structural refactors.
7. Dashboard command-service regression:
- `tests/NxTiler.Tests/DashboardWorkspaceCommandServiceTests.cs` validates workspace command delegation.
- `tests/NxTiler.Tests/DashboardRecordingCommandServiceTests.cs` validates recording command delegation.
8. Dashboard command execution policy regression:
- `tests/NxTiler.Tests/DashboardCommandExecutionServiceTests.cs` validates busy-guard, busy transitions, and error reporting behavior.
9. ImageSearch API facade regression:
- `tests/NxTiler.Tests/ImageSearchApiFacadeTests.cs` validates fluent `Search` entry points, `ImageSearchConfiguration.Reset`, and case-insensitive `Images` collection behavior after structural split.

## Regression matrix

1. Existing `NxTiler.Tests` suite must stay green.
2. Record startup/shutdown smoke checks for app.
3. Validate settings migration compatibility after schema changes.
4. Perf smoke thresholds from `docs/testing/PERF-BASELINE.md` must remain green.
5. Canonical governance check must remain green.

## Environment matrix

1. Windows 10 21H1+
2. Windows 11
3. Single-monitor and multi-monitor
4. DPI scale: 100%, 150%, 200%
