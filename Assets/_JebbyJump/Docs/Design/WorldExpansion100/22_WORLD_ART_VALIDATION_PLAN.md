# 22 — World Art Validation Plan

Status: `PROPOSED`. Two layers: **structural** (dimensions/format/import) and **visual** (looks
right at display size). Reuses `ProductionArtAudit/tools/validate_generated_art.py` + PIL mocks.

## Structural checks (per delivered asset, before import)

- exact dimensions match manifest (doc 18) `runtime_width/height_px`.
- format PNG; colour mode + alpha rule match (opaque vs transparent).
- transparent padding within tolerance; content bounds present.
- filename + target path exactly match manifest (no typos, correct world folder).

## Visual checks (PIL mocks at display size)

- **Platforms:** render each of the 6 at ~70px; assert the base colour still reads as its locked
  hue (sample centroid hue within tolerance of `#E63838/#3878E6/#38BF59/#FAD12E/#9938E6/#F28C26`);
  assert the six are mutually distinguishable (pairwise hue separation).
- **9-slice:** stretch platform/UI 9-slices; no seam artefacts at gameplay size.
- **Background:** fills 16:9→20:9 when camera-locked 1.5×; warm/child-safe; no debug UI/text.
- **Hazard:** silhouette reads clearly as a hazard, never as a safe platform; ink-base pivot;
  footprint matches the cactus trigger.
- **Tower landmark:** correct progression stage (doc 05); recognisably the same structure.
- **Story/finale:** text-safe zone; child-safe; tower stage correct.

## Import-time / Unity validators (editor tool, doc 24)

- every world art reference non-null (no missing/pink art) → fail build if null.
- platform visual swap does not change collider/width (compare against prefab) → R5.
- hazard prefab shares `CactusObstacle` behaviour; trigger/pivot unchanged.
- no wrong-world asset referenced (path world-id matches WorldDefinition id).

## Acceptance rule

`validator PASS ≠ visual accept` (lesson from batches 001–007). A world art family is accepted only
when **both** structural and PIL-visual checks pass at display size, plus an in-editor capture
review of at least the gameplay background + one platform row + hazard.

## Cross-world QA sweep (final)

- all 60 platform sprites (10 worlds × 6) pass colour-identity + separation.
- no two worlds accidentally share a background/hazard.
- Memory Cues legible on every world background.
