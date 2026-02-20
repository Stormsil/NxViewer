# ADR-007: Overlay Visibility Policies and Scaling Baseline

Date: 2026-02-13  
Status: Accepted

## Context

`NX-041` requires deterministic support for `Always`, `OnHover`, and `HideOnHover`.  
`NX-042` requires proportional overlay scaling and anchor-aware placement while preserving current overlay composition.

## Decision

1. Introduce `ICursorPositionProvider` in app services and use Win32-backed implementation (`Win32CursorPositionProvider`) in runtime.
2. Route visibility checks in `OverlayTrackingService` through injected cursor provider (instead of static cursor interop calls) to enable deterministic tests.
3. Normalize legacy settings compatibility:
- if `OverlayPolicies.HideOnHover=true` with `VisibilityMode=Always`, normalize to `VisibilityMode=HideOnHover`.
4. Extend overlay policy model with anchor support (`OverlayAnchor`) in runtime request/settings.
5. Apply anchor-aware placement in `OverlayTrackingService` and clamp to monitor bounds.
6. Apply proportional overlay scaling in `OverlayWindow` using `ScaleTransform` from `OverlayTrackingState` dimensions and reset transform on tracking stop/hide.

## Consequences

1. Visibility policy behavior is stable and testable without desktop cursor side effects.
2. Legacy settings remain backward compatible while moving to canonical visibility mode field.
3. Overlay content scaling and anchor placement are active in baseline form; further tuning can focus on advanced DPI and UX polish.
