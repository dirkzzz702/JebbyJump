# 21 — World Art Generation Batch Plan

Status: `PROPOSED`. Mirrors the proven batch workflow used for art batches 001–007 (kickoff prompt →
ChatGPT delivery → local validator + PIL visual checks → fix-pass if needed → idempotent Unity
wiring → audits/tests → commit). No final art generated in this planning task.

## Batch order (Gate #9 default)

| Batch | Contents | Prereq | Acceptance gate |
|---|---|---|---|
| W00 | shared technical templates, colour-identity swatches, validators, placeholder art | doc 17/24 | validators run; placeholders import clean; no collider drift |
| W01 | Cloud Meadow art family (17 assets) | W00 | doc 22 checks pass; colours read at 70px |
| W02 | Enchanted Forest | W00 | " |
| W03 | Crystal Caves | W00 | " |
| W04 | Sunshine Desert | W00 | " |
| W05 | Ocean Sky | W00 | " |
| W06 | Candy Cloud Kingdom | W00 | " |
| W07 | Clockwork Heights | W00 | " |
| W08 | Moonlit Dreamscape | W00 | " |
| W09 | Stormfire Volcano | W00 | " |
| W10 | Rainbow Tower Castle | W00 | " + tower-as-stage |
| UI | world-select/story/reward integration art (cross-world) | W01–W10 | level-select + story + reward render |
| QA | cross-world consistency + colour validation sweep | all | doc 22 full sweep green |

## Each batch package must specify

- prerequisites; exact asset IDs (from doc 18 manifest); locked references (art bible + Jebby id);
  validation commands (doc 22); ZIP delivery path; exact Unity target paths (doc 18/19);
  idempotent wiring steps; acceptance gate.

## Kickoff prompt source

Per-world ChatGPT briefs live in doc 20; a batch kickoff bundles the relevant world's briefs +
the shared rules + delivery/zip path. Reuse the `ProductionArtAudit/tools/validate_generated_art.py`
pattern for structural validation and PIL mocks for visual acceptance (70px platform reads,
9-slice stretch, hazard silhouette, story-card text-safe zones).

## Alignment with implementation phases

Art batches W01…W10 feed the per-world integration phases **P34I…P34R** (doc 29). W00 aligns with
**P34H** (art pipeline). UI/QA batches align with **P34S** (cross-world polish).
