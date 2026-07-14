# Unity Import Acceptance Checklist (after extraction + wiring)

- [ ] Importer settings match brief §12 (PPU/pivot/wrap/sprite mode); IMPORT-VERIFIED in .meta.
- [ ] No existing `.meta` was replaced; new metas committed with the wiring change.
- [ ] Zero missing references in both scenes (scene-integrity PlayMode tests green).
- [ ] No unexpected prefab overrides introduced (LevelSelectCard, Jebby prefabs clean).
- [ ] Rendered size correct at 16:9 AND 20:9 (no stretch/blur; 9-slice corners crisp).
- [ ] flipX still safe for any character-adjacent art.
- [ ] Overlap audits: `Audit UI Overlaps` + `Audit Main Menu UI Overlaps` → 0 pairs.
- [ ] EditMode + PlayMode suites green (incl. ≥90u touch-target tests).
- [ ] Floor collider alignment unchanged (Jebby stands at same height).
- [ ] Build inclusion sanity: AAB size delta reported (P24 baseline comparison).
- [ ] Placeholder.png deleted ONLY after zero-reference scan.
