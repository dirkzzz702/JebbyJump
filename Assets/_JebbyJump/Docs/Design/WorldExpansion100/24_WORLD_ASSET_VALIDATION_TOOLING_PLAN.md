# 24 — World Asset Validation Tooling Plan

Status: `PROPOSED` (built in P34H). Editor-only validators that fail loudly rather than shipping
missing/wrong art. Mirrors the existing QA audit tooling (`Jebby Jump/QA/Audit …`).

## Validators (menu `Jebby Jump/QA/Audit World Assets`)

1. **Reference completeness:** every `WorldDefinition` has non-null background, floor, 6 platform
   visuals, hazard visual, thumbnail, badge, gem, story card; no null → fail.
2. **Colour completeness:** each world's `PlatformColorVisual[]` covers all six locked colours
   exactly once.
3. **Colour identity:** platform sprite centroid hue within tolerance of the locked hex; six
   mutually distinguishable (ties into doc 22 PIL check, runnable in-editor on the texture).
4. **Collider invariance:** platform visual swap does not change the prefab collider or
   `PlatformWidth`; hazard prefab keeps the cactus trigger/pivot → R5/R6 guard.
5. **World-id path match:** an asset referenced by `Wnn` lives under `Art/Worlds/Wnn_*` (no
   wrong-world art).
6. **Behaviour equivalence:** hazard prefab has exactly the `CactusObstacle` behaviour component
   (no extra/altered behaviour) — themed prefab, one behaviour.
7. **Import contract:** dimensions/PPU/alpha/pivot match doc 17 (read importer).

## Output

- Console + optional report file; integrates with the EditMode test suite (doc 25) so CI-style
  runs catch regressions.
- Reuses the `UiOverlapMeasurement`-style audit pattern already in the repo.

## When it runs

- P34H (pipeline) as a gate; per world in P34I–P34R; full sweep in P34S/P34U.
