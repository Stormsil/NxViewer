# NxTiler Runbook

Last updated: 2026-02-13

## Prerequisites

1. Windows 10 21H1+ or Windows 11.
2. .NET SDK 8.x+.
3. FFmpeg available in PATH or managed by app workflow.
4. `x64` runtime environment (current app/test baseline).

## Build

```powershell
dotnet build .\NxTiler.sln -c Debug
```

## Test

```powershell
dotnet test .\NxTiler.sln -c Debug
```

Optional YOLO model smoke:

```powershell
$env:NXTILER_YOLO_MODEL_SMOKE="C:\models\nxtiler-yolo.onnx"
$env:NXTILER_YOLO_SMOKE_IMAGES="C:\models\smoke-images"
dotnet test .\NxTiler.sln -c Debug --filter "FullyQualifiedName~YoloModelSmokeTests"
```

## Perf smoke

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\perf\perf-smoke.ps1 -Configuration Release
```

Latest summary artifact:
`artifacts/perf/perf-smoke-release-latest.json`

Perf matrix:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\perf\perf-regression-matrix.ps1
```

Matrix artifacts:
1. `artifacts/perf/perf-smoke-debug-latest.json`
2. `artifacts/perf/perf-smoke-release-latest.json`

## Canonical governance check

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\governance\validate-canonical-track.ps1
```

## Release baseline verification

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\release\verify-release-baseline.ps1
```

## Run app

```powershell
dotnet run --project .\src\NxTiler.App\NxTiler.App.csproj -c Debug
```

## Known operational notes

1. Root `NxTiler.csproj` is legacy and frozen.
2. Canonical app runtime is `src/NxTiler.App`.
3. Library projects are now part of main solution and should be validated in full solution builds.
4. Snapshot output path is controlled by `AppSettingsSnapshot.Capture.SnapshotFolder` (default: Pictures).
5. Region snapshot interaction: Enter confirms phase/action, Escape cancels, right-click removes mask.
6. Capture hotkeys are configurable in Hotkeys page (`Instant snapshot`, `Region snapshot`).
7. Recording engine routing is controlled by `FeatureFlags.UseWgcRecordingEngine`.
8. Vision mode hotkey is configurable in Hotkeys page (`Toggle vision mode`).
9. Template vision engine template root:
- `%LocalAppData%\NxTiler\VisionTemplates`
- override with env var `NXTILER_VISION_TEMPLATES`
10. YOLO model path sources:
- `Vision.YoloModelPath` in settings
- fallback env var `NXTILER_YOLO_MODEL`
11. If YOLO engine fails and template fallback is enabled, workflow automatically retries with template engine.
12. YOLO labels auto-discovery (optional):
- `<model>.labels.txt`
- `<model>.names`
- `classes.txt` in model folder
13. Overlay window tracks focused/active target window automatically when visible (based on `OverlayPolicies`).
14. Overlay visibility policies are resolved by `OverlayPolicies.VisibilityMode` (`Always` / `OnHover` / `HideOnHover`).
15. Legacy compatibility: if `OverlayPolicies.HideOnHover=true` and `VisibilityMode=Always`, runtime normalizes to `HideOnHover`.
16. Overlay scaling follows tracked target window size when `OverlayPolicies.ScaleWithWindow=true`.
17. Overlay anchor placement is controlled by `OverlayPolicies.Anchor` (`TopLeft`..`BottomRight`).

## Troubleshooting quick checks

1. If capture fails, verify Windows Graphics Capture support and window visibility/state.
2. If snapshot is not copied to clipboard, verify the app has active desktop session and clipboard access is not locked by another process.
3. If recording fails, verify FFmpeg path and output directory permissions.
4. If template vision returns zero detections, verify template files exist in vision template directory and confidence threshold is not too strict.
5. If YOLO mode fails, verify model path exists and temporarily keep `EnableTemplateMatchingFallback=true` to preserve vision operation.
6. If settings behave unexpectedly, inspect `%LocalAppData%\NxTiler\settings.json`.
7. If detections look shifted, verify model expects 640x640 input or adjust model/export to standard YOLO shape.
8. If overlay position jitters, verify target window bounds are stable and temporarily switch overlay policy to `Always` for diagnostics.
9. If overlay is unexpectedly hidden, verify `OverlayPolicies.VisibilityMode` and cursor location relative to target window bounds.
10. If overlay scale looks stale after hiding/showing, toggle overlay once; runtime resets transform on hide/stop.
11. If overlay appears in unexpected corner, inspect `OverlayPolicies.Anchor` in `%LocalAppData%\NxTiler\settings.json`.

## Manual validation checklist (NX-011 instant snapshot)

1. Configure a valid NoMachine target window and set `Capture.SnapshotFolder`.
2. Trigger `Instant snapshot` hotkey.
3. Verify target window is brought to foreground/maximized before capture.
4. Verify image file appears in snapshot folder.
5. Verify clipboard now contains image data (paste into Paint).
6. Repeat with overlapped window and ensure capture reflects target window content.
7. Repeat on secondary monitor (if available) and verify bounds/content correctness.
8. Repeat on DPI 100%, 150%, 200% and compare output dimensions with expected bounds.

## Manual validation checklist (NX-012 shift snapshot + masks)

1. Trigger `Region snapshot` hotkey.
2. Draw a selection rectangle and confirm (`Enter`).
3. Create one or more masks by drag in selection area.
4. Move mask by drag; verify mask remains inside selected region bounds.
5. Remove one mask with right-click.
6. Confirm output and verify masked areas are baked as black rectangles.
7. Repeat and cancel with `Esc`; verify workflow reports cancellation and no output file is produced.

## Change management

1. Any architecture or behavior change requires docs update in `docs/`.
2. Backlog task status must be updated in `docs/roadmap/BACKLOG.md`.
