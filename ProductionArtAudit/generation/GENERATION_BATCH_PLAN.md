# Generation Batch Plan

Order chosen by dependency + risk (identity assets first, captures last).
One batch per ChatGPT session. Every batch: upload the listed references,
generate, validate, deliver ZIP, STOP.

| # | Batch file | Assets | Depends on | Gate |
|---|---|---|---|---|
| 001 | 001_store_identity.md | ART-001, ART-002, ART-003, ART-015 | — | identity match vs locked sheet; safe-zone validator PASS |
| 002 | 002_wordmark_brand.md | ART-010 | — | **human brand/legal review before any store use** |
| 003 | 003_gameplay_floor.md | ART-005 | design decision D1 (strip vs ledge) | tile-seam validator PASS; in-editor look check |
| 004 | 004_menu_background.md | ART-006 | — | centre-band contrast check; style match vs sky |
| 005 | 005_ui_kit.md | ART-007, ART-008 | decision D2 (corner radius) — candidate values usable | flat-stretch-region validator PASS |
| 006 | 006_skill_icons_vfx.md | ART-011, ART-012, ART-013, ART-014 | — | 70px readability; ring neutrality |
| 007 | 007_feature_keyart.md | ART-004, ART-016 | ART-010 approved if logo composite wanted (else text-free) | identity + crop tests |
| 008 | 008_store_screenshots.md | ART-009 | batches 003–006 INTEGRATED in Unity | real captures only |
| 009 | 009_hazard_variant.md | ART-017 | gameplay design decision D4 | bbox-match validator PASS |

Consistency requirements inside batches: 001 is one visual family (icon set);
005 is one chrome family; 006 shares the ornate icon style; 007 shares the
hero-scene style. Across batches, the locked references carry consistency.
