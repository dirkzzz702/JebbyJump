# Audit Self-Validation (2026-07-15)

| # | Rule | Result |
|---|---|---|
| 1 | Every LAUNCH_REQUIRED/RECOMMENDED manifest row has an asset brief | PASS — 17/17 (programmatic cross-check) |
| 2 | Every brief has an exact target path | PASS — rendered from manifest §5; no empty archive_path rows |
| 3 | Every target path appears in zip_manifest.json or is Unity-native/source-only | PASS — blueprint generated from the same manifest; 21 files incl. ART-009×4 + wordmark SVG |
| 4 | No .meta requested in the final ZIP | PASS — all meta_action=OMIT_FROM_ZIP; validator rejects any .meta |
| 5 | Every drop-in replacement records existing GUID | PASS (vacuous) — zero DROP_IN rows; rule documented in preserve_existing_meta.csv |
| 6 | Every new asset lists required Unity wiring | PASS — requires_unity_wiring.csv + post_import_wiring.csv cover all yes-rows |
| 7 | Every exact number has evidence | PASS — pixel numbers FILE-MEASURED (inventory), importer numbers IMPORT-VERIFIED, colours code-derived with file:line, store dims EXTERNAL-SPEC-VERIFIED with URL+date |
| 8 | Blocked numbers marked, not guessed | PASS — BLK-1..7 in decisions/BLOCKERS.md; briefs carry BLOCKED wording (ART-005 height, ART-013 diameter) |
| 9 | Every generation prompt names its reference assets | PASS — brief §7 + batch reference lists + REFERENCE_UPLOAD_INDEX.md; batches forbid generation without uploads |
| 10 | External store dimensions official + dated or marked unverified | PASS — Google Play + Android adaptive fetched 2026-07-14 (URLs recorded); iOS/trailer marked not-fetched/out-of-scope |
| 11 | Validator run against empty + invalid fixtures with useful errors | PASS — empty: 16 missing-file errors (blocked assets correctly downgraded to warnings); invalid: wrong-size/missing-alpha/forbidden-.meta/unexpected-file errors; one validator bug found+fixed during testing (ART-001 full-bleed corner exemption) |
| 12 | Only ProductionArtAudit/ changed | PASS — `git status` shows zero entries outside ProductionArtAudit/; HEAD unchanged at 355dba3; no scene/prefab/setting saved; screenshots were reused from the prior review session (no new Assets/Screenshots writes; the one edit-mode capture probe produced no file) |

Known limitations (honest):
- Fresh screenshots at 355dba3 were not capturable (backgrounded editor never
  repaints); evidence screenshots are from commit 66eb4d0 of the same session,
  labelled per-file, with P33/wardrobe deltas documented in sidecars/scene map.
- Live scene inspection this phase reused the same-session UnitySkills data
  (hierarchies/serialized fields at 66eb4d0..355dba3) plus fresh static YAML at
  355dba3; the bridge stayed in `auto` mode (scene_load gated), which the
  static GUID index fully compensated for except BLK-3.
- PPU/pivot columns come from .meta regex parsing (validated against known
  alignment presets); exotic importer settings would need spot re-checks.
