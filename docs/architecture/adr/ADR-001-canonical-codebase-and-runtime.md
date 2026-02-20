# ADR-001: Canonical Codebase and Runtime Baseline

Date: 2026-02-13  
Status: Accepted

## Context

NxTiler has both a modern modular codebase in `src/*` and a legacy root project.  
The roadmap requires one canonical implementation track and a stable runtime baseline for WGC behavior.

## Decision

1. Canonical track is `src/NxTiler.*` plus integrated class libraries in `src/WindowCaptureCL`, `src/WindowManagerCL`, and `src/ImageSearchCL`.
2. Legacy root project is frozen for new feature work.
3. Runtime baseline is Windows 10 21H1+.
4. .NET baseline remains .NET 8 LTS for refactoring phase.

## Consequences

1. All new architecture and feature development occurs only in `src/*`.
2. Legacy code is preserved temporarily for compatibility/reference.
3. WGC border/cursor capabilities can be handled with a predictable OS baseline.
