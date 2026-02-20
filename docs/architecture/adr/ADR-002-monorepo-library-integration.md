# ADR-002: Monorepo Library Integration via ProjectReference

Date: 2026-02-13  
Status: Accepted

## Context

`WindowCaptureCL`, `WindowManagerCL`, and `ImageSearchCL` already exist in the repository as class libraries.  
We need tight iteration speed and shared refactoring across app and libraries.

## Decision

1. Integrate these libraries into `NxTiler.sln`.
2. Consume them through `ProjectReference`, not NuGet packages, during active refactor/migration period.
3. Keep libraries as independent projects (not merged into `NxTiler.Infrastructure`).

## Consequences

1. Faster cross-project refactoring and debugging.
2. Shared CI/build validation across app and library surfaces.
3. Future packaging to NuGet remains possible after architecture stabilization.
