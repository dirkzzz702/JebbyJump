# Jebby Jump — Production Art Audit Summary

## 1. Audit identity
- Repo: `D:\Workspace\JebbyJump`, branch `main`, commit `355dba3` (clean at
  start AND end outside `ProductionArtAudit/`).
- Unity 6000.4.7f1, URP 2D; target platform Android (AAB pipeline P23+);
  landscape-only.
- UnitySkills v2.1.3 connected (`auto` mode this phase; live scene/serialized
  data from the same session's review at 66eb4d0..355dba3; fresh inspection
  this phase was static YAML + .meta + GUID index at 355dba3).
- Scenes inspected: Boot, MainMenu, Game (all three; hierarchy + serialized
  fields + runtime routes Boot→Menu→LevelSelect/Settings/Wardrobe→Game→Pause).
- Evidence screenshots: 9 (labelled with source commit + state sidecars).
- Tests run this audit: none needed (read-only); validator fixture self-tests
  run twice (both correctly FAIL).

## 2. Commercial art-readiness verdict
**`FOUNDATION_READY_ART_INCOMPLETE`**

Evidence: the gameplay layer is visually coherent and runtime-verified (sky,
platforms, ornate controls, 49-sprite character pipeline with test-pinned
import contract) — but the store surface has ZERO assets (no icon, feature
graphic, or screenshots; PlayerSettings icon slots empty), the first-seen
shell is scaffold-flat (built-in sprites tinted), the gameplay floor is
literally `Placeholder.png`, and no provenance/licence record exists for any
shipped art.

## 3. Counts
- Existing production-ready/acceptable art: **12 files** (sky, platform pill,
  cactus, rocket icon, 6 UI sprites, font+licence) + **49 prototype outfit
  sprites** (launch-usable, certification pending).
- Placeholders in production: **3 usages** (floor=Placeholder.png; Skill2/3
  icons=generic circle; shield VFX=arrow sprite) → 1 placeholder file + 3
  wrong-reuse wirings.
- Missing LAUNCH_REQUIRED files: **13** (ART-001..010 incl. 4 screenshots).
- Missing LAUNCH_RECOMMENDED files: **8** (ART-011..017 + wordmark SVG master).
- POST_LAUNCH/deferred documented: 11 groups (decisions/DEFERRED_...md).
- Drop-in replacements: **0** (every gap is a new file or store asset).
- Requiring Unity wiring: **11 files** (integration/post_import_wiring.csv).
- Blocked by design decision: **4** (ART-005 height, ART-007/008 radius
  confirm, ART-017 behaviour).
- Licence/source gaps: **62 files** lack provenance records (register D3);
  1 font fully licensed (OFL, file present).
- Store/marketing gaps: icon, adaptive fg/bg(+mono), feature graphic, ≥4
  screenshots, wordmark, key art.

## 4. Top launch blockers (ordered)
1. No app icon anywhere (device shows default Unity icon) — ART-001/002/003.
2. No store screenshots/feature graphic — listing cannot be filled (P31 stays
   blocked on graphics rows) — ART-004/009.
3. Gameplay floor is a placeholder file — ART-005.
4. Shell chrome below Art-Bible bar (flat scaffold rects) — ART-007/008 (+006).
5. No wordmark for title/splash/store — ART-010 (legal gate).
6. Provenance/licence register missing for all art — D3 (public-release gate).
7. Skill 2/3 icons + shield/echo VFX are wrong-sprite reuses — ART-011..014.

## 5. Art system strengths (evidence-based)
- Character pipeline is exemplary: 49 sprites, exact shared canvas 1122×1402,
  PPU 100, bottom-centre pivot, transparent corners 49/49, AOC-based variant
  system fully test-pinned, default-look pointer tool (`SetDefaultLook`).
- Runtime tinting architecture (platform pill + memory swatch + echo ring
  pattern) minimizes needed bitmaps and keeps colour semantics in code
  (`PlatformColorPalette.cs`).
- The ornate control set + pastel sky already hit the "premium indie" bar
  (RUNTIME-OBSERVED) — new art has a real style anchor.
- Zero missing GUID references project-wide; no Resources.Load art loading;
  complete dependency map was possible.
- Typography licence is clean (OFL file sits next to the font).

## 6. Recommended production sequence
See `ART_PRODUCTION_ROADMAP.md` (A: decisions D1–D3 → B: batches 001–007 →
C: wiring order 1–7 with test gates → D: screenshot capture + store → E:
public-release items). Batches 001/002 can start TODAY (no blockers).

## 7. What ChatGPT can generate safely (with listed references uploaded)
ART-001, 002, 003, 015 (icons); ART-006 (menu bg); ART-005 (after D1);
ART-007, 008 (UI kit; candidates usable); ART-011, 012 (skill icons);
ART-013, 014 (VFX sprites); ART-016→004 (key art → feature crop);
ART-010 as a CANDIDATE (human review before use). = **15 of 17 asset IDs**.

## 8. What ChatGPT must NOT generate alone
- ART-009 store screenshots (real captures only — refuse fabrication).
- ART-017 until design decision D4.
- Any Unity-native file (.meta/.anim/.controller/.overrideController/
  .spriteatlas/.mat/.asset/.prefab/.unity/TMP assets).
- Fonts (ttf/otf) — licence-gated; wordmark is lettering-as-image + SVG.
- Multi-frame character animation sets (RIG_OR_ANIMATION_PIPELINE_REQUIRED —
  not in scope; current single-still clips are the honest shipped state).
- Anything whose reference upload is missing (hard rule in the master prompt).

## 9. Physical-device and human-review gaps
- Launcher icon under real OEM masks; store-listing rendering on devices.
- Touch comfort / thermal / battery (P24/P25 still deferred — unrelated to art
  but shares the device gap).
- All subjective style approval (qa/VISUAL_ACCEPTANCE_CHECKLIST.md).
- Brand/legal review for the wordmark; provenance register sign-off (D3).
- Rendered check of new art at 20:9/4:3 + notches (capture tool renders only
  the current Game-view aspect).
