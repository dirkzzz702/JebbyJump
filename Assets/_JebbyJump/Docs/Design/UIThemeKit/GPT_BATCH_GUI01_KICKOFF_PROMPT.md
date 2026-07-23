# Paste-Ready Kickoff Prompt — Batch GUI01 (in-game UI from the approved mockup)

> Paste into ChatGPT **and attach the approved gameplay-UI mockup** (Option B) +
> the finalized MAIN MENU mockup (style anchor). Produces the final in-game UI
> assets. **One change from the mockup: the buttons are CLEAN — no flowers/leaves.**

---

You are producing FINAL production UI art for the Unity game **Jebby Jump**,
continuing an audited pipeline (menu batches UI01/UI02 accepted & integrated).
This session executes **batch GUI01 only**: the in-game (gameplay) UI, reproducing
the attached Option-B mockup exactly, **except the buttons must be clean** (see below).

## Repository access
Public repo `https://github.com/dirkzzz702/JebbyJump` (branch `main`). Fetch via raw URLs, e.g.
`https://raw.githubusercontent.com/dirkzzz702/JebbyJump/main/Assets/_JebbyJump/Docs/Design/UIThemeKit/GUI01_zip_manifest.json`

## Step 1 - load
1. `ProductionArtAudit/generation/MASTER_CHATGPT_ART_PRODUCTION_PROMPT.md` - binding rules.
2. `Assets/_JebbyJump/Docs/Design/UIThemeKit/GUI01_zip_manifest.json` - exact sizes/paths/alpha.
3. `ProductionArtAudit/tools/validate_generated_art.py` + `build_final_art_zip.py`.

## Step 2 - references
- **The attached Option-B mockup = the exact target** for style, palette, and each piece.
- `Assets/_JebbyJump/Art/Sprites/Backgrounds/bg_menu_01.png` + the menu mockup - the finalized cloud-kingdom look these must match (cream/ivory panels, warm-brown outline, honey-gold accents, cloud puffs, blue gem, rounded friendly forms).

## CLEAN UI — minimal decoration (IMPORTANT)
Keep every piece CLEAN and simple:
- **No cloud puffs anywhere.** Do NOT attach the little cloud tufts to the badges,
  banners, hearts, or buttons.
- **No flowers, no leaves, no vines** on any piece.
- **Keep the blue gem (diamond)** as the SINGLE accent — on the level badge, the
  timer banner, the hint banner, and the top of the result card. That is the only
  decoration allowed.
- Result buttons are plain clean rounded plates (cream or gold), warm-brown
  outline, a soft top sheen, nothing else.
The frames stay pretty in SHAPE (the cream plaque / ribbon / card outlines) but
carry NO attached clutter beyond the single gem.

## Engine rules (same as the menu kit)
- **9-slice frames** (`*_9s`): the level badge, timer banner, hint banner, result
  card, and both result buttons are stretched by the engine. Deliver each as a
  clean frame with a **uniform interior and a clean, even border**; the top/bottom/
  left/right edges and centre must be uniform so they tile seamlessly. Decorative
  motifs (gem, cloud tufts, corner vines) may sit only in the **fixed corners**, not
  along the stretchable edges/centre.
- **Blank text zones, NO baked text**: the level badge (holds "Level 10"), timer
  banner (holds "00:07.34"), hint banner (holds a message), result card, and result
  buttons all leave their centre **clean and empty** for the engine's rounded-font
  text. The rank medal leaves its centre blank for the rank letter. Do NOT bake any
  numbers, times, or words.
- Fixed pieces (heart, pause, medal, star, row icons, mascot) are cute-cartoon,
  centred with padding, on clean transparency.
- Warm, child-safe, rounded throughout; no AI artefacts; no characters except the
  Game-Over mascot.

## Step 3 - produce the 14 GUI01 assets
All under `Assets/_JebbyJump/Art/Sprites/UI/`. All PNG, RGBA, transparent.

**Request A - HUD + banners (7):**
| File | Size | Notes |
|---|---|---|
| `ui_hud_heart_01.png` | 256x256 | cream/gold life heart, CLEAN (no cloud, no leaves) |
| `ui_hud_pause_btn.png` | 256x256 | round cream/gold button + simple pause glyph, CLEAN (no cloud) |
| `ui_hud_level_badge_9s.png` | 640x256 | ornate cream "level" plaque; blue gem on top, CLEAN corners (no clouds); **blank centre** for "Level X" |
| `ui_hud_timer_banner_9s.png` | 640x224 | cream ribbon/banner; blue gem only, CLEAN (no clouds); **blank centre** for the time |
| `ui_hint_banner_9s.png` | 768x256 | soft cream banner; blue gem only, CLEAN (no clouds); **blank centre** for a hint message |
| `ui_row_icon_time_01.png` | 128x128 | cute stopwatch icon |
| `ui_row_icon_best_01.png` | 128x128 | cute crown icon |

**Request B - result cards + pieces (7):**
| File | Size | Notes |
|---|---|---|
| `ui_result_card_9s.png` | 768x768 | cream card, warm-brown outline, blue gem on top, CLEAN corners (no clouds/vines); **blank interior** (used for both Level Complete and Game Over) |
| `ui_result_btn_9s.png` | 384x160 | **clean** cream result button, blank centre - NO flowers/leaves |
| `ui_result_btn_primary_9s.png` | 384x160 | **clean** gold result button, blank centre - NO flowers/leaves |
| `ui_rank_medal_01.png` | 320x320 | ornate round medal with laurel + gem, **blank centre** for the rank letter |
| `ui_star_gold_01.png` | 128x128 | plump gold star |
| `ui_row_icon_rank_01.png` | 128x128 | cute shield icon |
| `ui_gameover_mascot_01.png` | 640x640 | the sad little cactus sitting on a soft cloud (gentle, not scary) |

## Split delivery — two requests (image cap)
Run in two requests via `--ids` on BOTH validate and build:
- **Request A:** `GUI01-HEART,GUI01-PAUSE,GUI01-LEVELBADGE,GUI01-TIMER,GUI01-HINT,GUI01-ICON-TIME,GUI01-ICON-BEST` → `jebby_art_GUI01_A.zip`
- **Request B:** `GUI01-CARD,GUI01-BTN-SEC,GUI01-BTN-PRI,GUI01-MEDAL,GUI01-STAR,GUI01-ICON-RANK,GUI01-MASCOT` → `jebby_art_GUI01_B.zip`

Example (Request A):
`python ProductionArtAudit/tools/validate_generated_art.py --manifest Assets/_JebbyJump/Docs/Design/UIThemeKit/GUI01_zip_manifest.json --dir <out> --ids GUI01-HEART,GUI01-PAUSE,GUI01-LEVELBADGE,GUI01-TIMER,GUI01-HINT,GUI01-ICON-TIME,GUI01-ICON-BEST`
then the same `--ids` on `build_final_art_zip.py --out jebby_art_GUI01_A.zip`.
Drop both zips in `Downloads/jebby-jump/jebby_art_GUI01`.

## Step 4 - deliver
Each zip: exactly its manifest PNGs at their manifest paths - nothing else. No
`.meta`/prefab/scene/material/README/report files inside. Attach separately: the
validation report; a **stretch test** of the 9-slice frames at a wide size proving
seam-free, empty centres; and a mock of the assembled HUD + both cards with sample
text drawn in, next to the mockup.

## Non-negotiables
Exact filenames/paths/sizes/alpha (all RGBA transparent); **CLEAN — no cloud puffs,
no flowers, no leaves, no vines on any piece; the blue gem is the ONLY accent**;
9-slice frames seam-free with blank centres; NO baked text/numbers; warm and
child-safe; reproduce the mockup's shapes/palette faithfully otherwise; no stray
annotations; no .meta/prefab/scene/material files; report the validator's real
output. When delivered, STOP.
