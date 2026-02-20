# ADR-004: YOLO Staged Rollout with Automatic Template Fallback

Date: 2026-02-13  
Status: Accepted

## Context

The roadmap requires YOLO integration (`NX-032`), but introducing full ONNX inference in a single step increases delivery and stability risk.  
The app already has a working template vision engine and feature-flag based engine routing.

## Decision

1. Introduce `YoloVisionEngine` as phase-1 integration slot in infrastructure.
2. Expand settings to include:
- `Vision.TemplateDirectory`
- `Vision.YoloModelPath`
3. Keep runtime resilient:
- when active YOLO engine fails, `VisionWorkflowService` automatically retries with template engine if fallback is enabled.
4. Keep full ONNX inference as phase-2 inside `NX-032`.

## Consequences

1. Vision pipeline can evolve incrementally without breaking existing users.
2. Operators can safely test YOLO wiring while preserving template fallback.
3. Additional implementation work is still required to complete true YOLO detections.
