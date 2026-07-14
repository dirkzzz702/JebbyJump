# Dependency Evidence (how usage was established)

Method: single-pass GUID reference index over every `.unity/.prefab/.asset/
.controller/.anim/.overrideController/.mat` under `Assets/` plus
`ProjectSettings/*.asset`, built by `tools/measure_existing_art.py`
(STATIC-INSPECTED, commit 355dba3). Per-object sprite assignments resolved by
YAML segment inspection of `Game.unity`; runtime confirmation from the
2026-07-14 live review screenshots (RUNTIME-OBSERVED).

Key results:
- 61/63 image+font files are referenced ≥1 time. The only unreferenced files
  are the two locked reference boards under `Docs/Art/References/` (correct:
  they are sources, not runtime assets, and therefore do not ship).
- `Placeholder.png` (32×32) referenced by `Game.unity` — the **Floor**
  SpriteRenderer (`m_Color` #F2EBD9, transform scale 37.64×0.5×1).
- `Skill2Icon.m_Sprite` and `Skill3Icon.m_Sprite` → `ui_btn_bg.png` GUID.
- `BubbleShieldEffect.m_Sprite` → `ui_btn_right.png`; `ColorEchoEffect.m_Sprite`
  → `ui_btn_bg.png`.
- `LiberationSans.ttf` referenced by 2 assets (TMP font asset + settings);
  release preflight test verifies digits 0–9 coverage.
- Outfit sprites: each referenced by its per-outfit `.anim` clips (and the
  RookiePage set additionally by the 7 base `Jebby_<State>.anim` clips + the
  Jebby prefab initial sprite + preview library since `355dba3`).
- No `Resources.Load`/Addressables image loading exists in runtime scripts
  (grep verified) — all art binds through serialized references, so the GUID
  index is a complete usage map.
- No SpriteAtlas assets exist; `atlas_membership=none` project-wide.
