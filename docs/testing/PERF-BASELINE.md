# NxTiler Performance Baseline

Last updated: 2026-02-13

## Scope

This baseline tracks core developer-loop performance for:

1. Solution build.
2. Test project execution.

## Execution command

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\perf\perf-smoke.ps1 -Configuration Release
```

Matrix command:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\perf\perf-regression-matrix.ps1
```

## Current thresholds

1. Build: `<= 120s`
2. Tests (`NxTiler.Tests`): `<= 40s`
3. Total (build + tests): `<= 160s`

## Output artifact

Script stores latest run summary in:

1. `artifacts/perf/perf-smoke-debug-latest.json`
2. `artifacts/perf/perf-smoke-release-latest.json`

## Latest validated run

Date: 2026-02-13

1. Debug:
- Build: `1.84s`
- Tests: `2.15s`
- Total: `3.99s`
2. Release:
- Build: `6.37s`
- Tests: `2.74s`
- Total: `9.11s`

## Notes

1. Thresholds are regression guards, not synthetic microbenchmarks.
2. If thresholds fail, attach machine specs and run output in PR/issue before tuning thresholds.
