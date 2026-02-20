# ADR-005: YOLO ONNX Runtime CPU Baseline

Date: 2026-02-13  
Status: Accepted

## Context

`NX-032` requires a production-capable YOLO execution path in NxTiler.  
The system already has feature-flag routing and template fallback, but lacked actual model inference.

## Decision

1. Implement YOLO inference in `YoloVisionEngine` using `Microsoft.ML.OnnxRuntime` CPU baseline.
2. Use built-in preprocessing and postprocessing inside infrastructure:
- letterbox to `640x640`
- tensor conversion `NCHW`
- output parsing for common YOLO export layouts
- per-class NMS
3. Keep template engine fallback as runtime safety net.
4. Keep GPU providers and advanced model optimization as future hardening tasks.

## Consequences

1. NxTiler can execute ONNX YOLO models without waiting for GPU-specific integration.
2. Runtime behavior remains stable through fallback if model or inference fails.
3. Additional validation is required for model compatibility/performance across different YOLO exports.
