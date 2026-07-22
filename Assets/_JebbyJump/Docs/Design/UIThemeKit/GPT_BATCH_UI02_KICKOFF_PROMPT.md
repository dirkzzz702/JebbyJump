# Paste-Ready Kickoff Prompt — Batch UI02 (Main Menu bespoke button plates)

> Paste this into ChatGPT **and attach the approved main-menu mockup image** (the
> warm cloud-kingdom menu). That mockup is the EXACT target. This batch replaces
> the generic frame+overlay kit with **complete, bespoke button plates** that
> reproduce each button in the mockup precisely.

---

You are producing FINAL production UI art for the Unity game **Jebby Jump**,
continuing an audited pipeline (batches 001-007, world batches W01-W10, and UI01
accepted & integrated). This session executes **batch UI02 only**: 5 bespoke
button plates + the island base + the gem, reproducing the attached mockup exactly.

## Repository access
Public repo `https://github.com/dirkzzz702/JebbyJump` (branch `main`). Fetch via raw URLs, e.g.
`https://raw.githubusercontent.com/dirkzzz702/JebbyJump/main/Assets/_JebbyJump/Docs/Design/UIThemeKit/UI02_zip_manifest.json`

## Step 1 - load instructions (ALL, before generating)
1. `ProductionArtAudit/generation/MASTER_CHATGPT_ART_PRODUCTION_PROMPT.md` - binding rules.
2. `Assets/_JebbyJump/Docs/Design/UIThemeKit/UI02_zip_manifest.json` - exact sizes/paths/alpha the validator enforces.
3. `ProductionArtAudit/tools/validate_generated_art.py` + `build_final_art_zip.py`.

## Step 2 - references
- **The attached mockup = the exact visual target.** Match every button's frame
  colour, icon, and decoration placement as closely as possible.
- `Assets/_JebbyJump/Art/Sprites/Backgrounds/bg_menu_01.png` - the menu background these sit on; harmonise.
- `Assets/_JebbyJump/Art/Sprites/Obstacles/spr_cactus_obstacle_01.png` - cute-cartoon anchor for the baked icons.

## The plate concept (this is the whole point)
Each button in the mockup is a **complete, self-contained art plate**: a clean
rounded frame with its icon, cloud puffs, vines, flowers and stars all baked in.
We render only the word on top. So each plate is ONE finished button minus text.

### Shared plate spec — HARD (all 5 plates)
- Canvas **1280 x 384 px**, RGBA, transparent outside the artwork.
- The **button frame** is a clean **rounded rectangle** (corner radius ~60, soft
  warm-brown outline) occupying the centre: about **1040 wide x 208 tall**,
  horizontally centred, vertically centred (frame spans ~x120-1160, y88-296).
  **Smooth rounded ends - NO pointed ribbon/banner ends.**
- The **margin around the frame** (~120 px sides, ~88 px top/bottom) is where
  decorations that overflow the frame live (cloud puffs at corners, vines,
  flowers, a star). Decorations may cross the frame edge but must NOT enter the
  label zone.
- **Icon**: baked INSIDE the frame at the left, vertically centred, ~150 px,
  centred near x=210. (This is the mockup's left icon.)
- **Label safe-zone**: the frame interior from ~x=390 to ~x=1120 must be **clean,
  uniform fill, EMPTY** - no text, no icon, no decoration. The engine draws the
  label there in a rounded font.
- **NO baked text anywhere.** Not even placeholder. The label zone stays blank.
- Primary plate = **golden/honey** gradient fill; secondary plates = **cream/
  ivory** fill. Both with the soft warm-brown rounded outline from the mockup.

### Per-plate content (match the mockup)
| # | File | Frame | Icon | Decorations (from the mockup) |
|---|---|---|---|---|
| 1 | `ui_plate_continue.png` | GOLD (selected) | smiling gold star | cloud puffs at all four corners; green leaf sprig top-right; yellow flower bottom-right; a small orange star just off the left end |
| 2 | `ui_plate_levelselect.png` | cream | grassy island + red flag | cloud puffs at corners; leafy vine trailing down the right side; tiny stars |
| 3 | `ui_plate_wardrobe.png` | cream | gold hanger + star | leafy vine + small flowers down the right; cloud puff bottom-left |
| 4 | `ui_plate_settings.png` | cream | friendly gear/cog | small leafy vine accent on the right; cloud puff bottom-left; tiny stars |
| 5 | `ui_plate_quit.png` | cream | cosy wooden arched door | cloud puffs at the bottom corners; small grass/flower tufts |

### The other two assets
| # | File | Size | Content |
|---|---|---|---|
| 6 | `ui_menu_island.png` | 1400x520 | the grassy floating-island base the whole stack rests on: rounded earthy underside, green grass top with little flower dots, vines wrapping under, a few dangling leaves - exactly as the mockup |
| 7 | `ui_menu_gem.png` | 320x384 | the blue faceted crystal gem with a soft glow, as in the mockup, on clean transparency |

(The wordmark is already final in the repo - do NOT regenerate it.)

### NO stray annotations
No index numbers, filenames, watermarks or captions burned into any asset. The
baked icons and decorations are intended content; this rule is about production marks.

## Step 3 - validate + deliver
**Deliver exactly ONE zip. Do NOT return loose PNG files.**
1. Write all 7 PNGs into `<out>` preserving full repo-relative paths
   (`<out>/Assets/_JebbyJump/Art/Sprites/UI/...`).
2. Validate:
   `python ProductionArtAudit/tools/validate_generated_art.py --manifest Assets/_JebbyJump/Docs/Design/UIThemeKit/UI02_zip_manifest.json --dir <out>`
3. On PASS build the zip:
   `python ProductionArtAudit/tools/build_final_art_zip.py --manifest Assets/_JebbyJump/Docs/Design/UIThemeKit/UI02_zip_manifest.json --dir <out> --out jebby_art_UI02.zip`
4. Deliver `jebby_art_UI02.zip` to `Downloads/jebby-jump/jebby_art_UI02`.

**Zip contents rule:** exactly the 7 manifest PNGs at their manifest paths -
nothing else. No `.meta`/prefab/scene/material/README/report files inside.

Attach as **separate chat attachments**: the validation report; and a **mock of
the assembled menu** (the 5 plates stacked with label text drawn in, on the island
base, with the gem) placed next to the mockup so the match can be judged.

## Non-negotiables
Exact filenames/paths/sizes/alpha (all RGBA transparent); clean rounded frames
(no ribbon ends); icon + decorations baked, label zone clean and empty; no baked
text; reproduce the mockup's per-button design faithfully; warm and child-safe;
no stray annotations; no .meta/prefab/scene/material/atlas/ScriptableObject files;
report the validator's real output - do not claim a PASS you did not run.

When delivered, STOP.
