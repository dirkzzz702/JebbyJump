# 32 — Self-Validation

## Status: COMPLETE — full planning package delivered.

## A. Files present

- [x] `00`–`32` (33 top-level docs incl. `13`/`18` CSV + `19` TXT) — verified 33 files.
- [x] `implementation_prompts/` P34A…P34U — verified 21 prompts.

## B. Hard constraints (machine-verified against `13_100_LEVEL_MASTER_TABLE.csv`)

- [x] exactly **100** unique level rows
- [x] exactly **10** worlds, **10** levels each, **10** finales
- [x] every sequence length ≤ 10
- [x] every row choice count ≤ 6
- [x] no skill-required level (`skill_required = false` × 100)
- [x] no unapproved mechanic; every themed hazard = cactus reskin (behaviour-equivalent)
- [x] all six semantic colours retained; no 7th (identity contract in doc 04)
- [x] cactus never on the correct platform (distractor-only, doc 08/12 — REPO-VERIFIED invariant)
- [x] failure = full-level restart (unchanged)
- [x] every level references a valid world/background config (mapping doc 09)
- [x] every world has a full art family; every art row has a target path (170 rows, verified)
- [x] every implementation phase has a prompt (21/21)
- [x] every proposed balance value labelled `PLAYTEST-HYPOTHESIS` (World 1 = `EXISTING-PRESERVED`)
- [x] save compatibility addressed (L1–10 indices + asset names preserved, doc 10)
- [x] ranks ordered S < A < B on all 100 (verified)

## C. Write-boundary check

- [x] No runtime files modified (Scripts/Scenes/Prefabs/Art/Settings/ProjectSettings/Packages/CLAUDE.md).
- [x] Only new files under `Assets/_JebbyJump/Docs/Design/WorldExpansion100/`.
- [x] `git status` shows only this folder (+ pre-existing unrelated `.claude/settings.json`).
- Note: Unity auto-generates `.meta` companions for docs under `Assets/`; expected, allowed within
  the planning folder.

## D. Confirmed vs open

- **Confirmed at Approval Gate:** roster (recommended), reward model **C**, Levels 1–10 **unchanged**,
  build-full-package.
- **Still open (doc 31):** story-card before/after (recommended before), parallax scope
  (recommended out), themed-hazard-prefab allowance (recommended yes), launch shape (single vs
  staged), art-production order, human-playtest owner. These gate their specific phases, not P34A–B.
- **Blocked:** performance/memory budgets (need measured baseline, doc 28); some art import
  max-sizes (derive at P34H); all non-World-1 balance (needs P34T human playtest).

## E. Errors found & corrected

- None outstanding. CSV/manifest generated deterministically and validated in-script + re-validated
  post-write.

## F. Not done (by design — this is planning only)

- No runtime implementation, no final art, no edits to existing production docs / CLAUDE / GDD /
  Roadmap (those are patch **proposals** in doc 03, applied in P34A).
