# ADR-003: Vision Workflow Baseline with Template Fallback

Date: 2026-02-13  
Status: Accepted

## Context

Wave 3 requires a production integration point for vision scenarios without blocking on YOLO runtime rollout.  
The app already has `IVisionEngine` contract and integrated `WindowCaptureCL`/`ImageSearchCL` libraries.

## Decision

1. Introduce app-level `IVisionWorkflowService` and implement it as `VisionWorkflowService`.
2. Route `HotkeyAction.ToggleVisionMode` through `WorkspaceOrchestrator` into vision workflow.
3. Implement `TemplateVisionEngine` in infrastructure:
- capture frames via `WindowCaptureCL` (WGC path)
- detect templates via `ImageSearchCL`
4. Select vision engines through existing feature flags:
- `EnableTemplateMatchingFallback`
- `EnableYoloEngine`
5. Keep YOLO integration as a separate incremental step (`NX-032`), not coupled to workflow baseline.

## Consequences

1. Vision mode is now a first-class workflow with testable orchestration.
2. Template fallback can run today on existing stack and provides bridge coverage before YOLO.
3. Runtime behavior stays controllable via feature flags and preferred engine setting.
4. Additional work is still required for continuous scanning UX and YOLO engine implementation.
