# ADR-008: Canonical Track Governance Automation

Date: 2026-02-13  
Status: Accepted

## Context

After migrating to `src/NxTiler.*`, legacy root files remain in repository for archive/emergency compatibility.  
`NX-051` requires deprecated paths to be removed or explicitly archived without allowing accidental re-wiring back to legacy root runtime.

## Decision

1. Add automated canonical-track validator:
- `scripts/governance/validate-canonical-track.ps1`
2. Enforce it in CI before build/test:
- `.github/workflows/ci.yml`
3. Add explicit PR checklist requirement for governance check.
4. Remove stale dead type `OverlayVisibilityPolicy` as part of deprecated-path cleanup.

## Consequences

1. Solution/project drift back to `NxTiler.csproj` is detected early.
2. Canonical runtime boundary (`src/*`) remains enforceable and reproducible.
3. Legacy root remains archived/frozen but not part of active delivery path.
