# ADR-006: Overlay Tracking Service Baseline

Date: 2026-02-13  
Status: Accepted

## Context

Overlay UI was previously static/manual and not reliably synchronized with target window movement or focus changes.  
Roadmap `NX-040` requires deterministic alignment and policy-based visibility.

## Decision

1. Introduce `IOverlayTrackingService` in application abstractions.
2. Implement runtime tracking in `NxTiler.App.Services.OverlayTrackingService`:
- polls target window bounds via `IWindowControlService`
- computes overlay placement within monitor bounds
- applies `OverlayVisibilityMode` (`Always`, `OnHover`, `HideOnHover`)
3. Integrate tracking into `OverlayWindow` lifecycle and workspace snapshot updates.

## Consequences

1. Overlay follows focused/active window in real time without manual repositioning.
2. Overlay policies are now executed by a dedicated service, not ad-hoc view logic.
3. Follow-up for scaling/anchor hardening was completed in `ADR-007`.
