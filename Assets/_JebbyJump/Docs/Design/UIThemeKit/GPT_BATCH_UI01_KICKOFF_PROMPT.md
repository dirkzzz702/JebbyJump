# Paste-Ready Kickoff Prompt — Batch UI01 (Main Menu UI theme kit)

> Paste this into ChatGPT **and attach the approved concept mockup image** (the
> warm cloud-kingdom main menu). That mockup is the PRIMARY visual target; the
> notes below translate it into exact, engine-ready assets.

---

You are producing FINAL production UI art for the Unity game **Jebby Jump**,
continuing an audited pipeline (art batches 001-007 and world batches W02-W10
accepted & integrated). This session executes **batch UI01 only**: the Main Menu
UI theme kit shown in the attached concept. Precise EXECUTION of that concept,
not redesign.

## Repository access
Public repo `https://github.com/dirkzzz702/JebbyJump` (branch `main`). Fetch via raw URLs, e.g.
`https://raw.githubusercontent.com/dirkzzz702/JebbyJump/main/Assets/_JebbyJump/Docs/Design/UIThemeKit/UI01_zip_manifest.json`

## Step 1 - load instructions (ALL, before generating)
1. `ProductionArtAudit/generation/MASTER_CHATGPT_ART_PRODUCTION_PROMPT.md` - binding rules.
2. `Assets/_JebbyJump/Docs/Design/UIThemeKit/UI01_zip_manifest.json` - exact sizes/paths/alpha the validator enforces.
3. `ProductionArtAudit/tools/validate_generated_art.py` + `build_final_art_zip.py`.

## Step 2 - references
- **The attached concept mockup = the exact visual target.** Match its palette, its cream/gold "wooden-cloud" button frames, its cute icons, its leafy-vine + cloud-puff accents, and its warm mood.
- `Assets/_JebbyJump/Art/Sprites/Backgrounds/bg_menu_01.png` - the shipped menu background and overall style anchor. Your kit sits ON this background (it is NOT regenerated in this batch); harmonise with it.
- `Assets/_JebbyJump/Art/Sprites/Obstacles/spr_cactus_obstacle_01.png` - the cute-cartoon language anchor for the icons (chunky, friendly, clean silhouette, soft face-friendly shapes).

## Art language
- **Painted / storybook** (soft, atmospheric, low-contrast): the wordmark lockup, the grassy island base, the cloud-puff and vine decorations.
- **Cute cartoon** (chunky, friendly, clean readable silhouette): the five button icons.
- Warm and child-safe throughout: cream, honey-gold, soft peach, gentle greens, sky pastels. No cold/harsh/menacing anything. This is the first screen a child sees.

## CRITICAL rules for an ENGINE UI kit (this is what makes it usable)

### 1. Button + panel frames are 9-SLICE - deliver them CLEAN and stretchable
`ui_btn_primary_9s`, `ui_btn_secondary_9s`, and `ui_panel_frame_9s` are **9-slice
frames**: Unity keeps the four corners fixed and STRETCHES the edges and centre to
any width/height. Therefore:
- Deliver each as a **rounded frame with a simple, uniform interior fill** (soft
  vertical gradient is fine) and a **clean, even border all the way around**.
- The **top edge, bottom edge, left edge, right edge and centre must be uniform**
  along their length so they tile with no seam when stretched. Decorative rounded
  corners are fine (they live in the fixed corner zone); do NOT run vines, clouds,
  flowers, icons or any localized motif along the stretchable edges or centre.
- Keep any corner rounding/thickness within roughly the outer **72 px** on each
  side (the fixed 9-slice border region).
- **NO text of any kind baked into the frames.** Button labels are live text the
  engine draws on top in a rounded font. Leave the centre clean and empty.
- Primary = the golden/honey "selected" frame from the concept. Secondary = the
  cream/ivory frame with the soft warm-brown outline. Panel = the same cream
  language at large size for sub-screens (Level Select / Settings / Wardrobe).

### 2. Decorations are SEPARATE overlay pieces (not baked into frames)
The lush corner clouds and leafy vines from the concept ship as their own
transparent sprites (`ui_deco_cloud_corner`, `ui_deco_vine_corner`) so the engine
can place them at button/panel corners over any size. Each is a tight corner
cluster on clean transparency, designed to sit at a top-or-bottom corner.

### 3. Icons are SEPARATE, one per button, left-aligned
Five transparent icons, each a cute-cartoon object centred in its canvas with a
little padding, reading clearly at ~64 px:
- Continue -> a smiling gold **star**
- Level Select -> a small **grassy island with a red flag**
- Wardrobe -> a gold **clothes hanger with a star**
- Settings -> a friendly **gear/cog**
- Quit -> a cosy **wooden door**

### 4. The wordmark (CANDIDATE - not for store use)
`ui_wordmark_lockup` = the full title lockup from the concept: the puffy 3-D
cream/gold letters on a soft cloud, the rainbow arch topped by a gold star, the
tiny castle, and leafy sprigs - all on clean transparency.
**Must read clearly and unambiguously as "JEBBY JUMP"** (the concept's stylised
first letters can be misread as "Jcbby"; make the E read as an E). This asset is a
BRAND CANDIDATE pending the owner's sign-off and must not be treated as final /
store-approved.

### NO stray annotations (world-batch lesson)
Do not burn index numbers, filenames, watermarks, captions or manifest labels into
any asset. (The wordmark's own lettering and the button icons are intended content;
this rule is about stray production marks.)

## Step 3 - produce the 13 UI01 assets
All under `Assets/_JebbyJump/Art/Sprites/UI/`. All PNG, RGBA, transparent.

| # | File | Size | Notes |
|---|---|---|---|
| 1 | `ui_btn_primary_9s.png` | 512x192 | golden 9-slice button frame, clean/empty centre |
| 2 | `ui_btn_secondary_9s.png` | 512x192 | cream 9-slice button frame, soft brown outline |
| 3 | `ui_panel_frame_9s.png` | 640x640 | cream 9-slice panel frame (sub-screens) |
| 4 | `ui_icon_continue_star.png` | 256x256 | smiling gold star |
| 5 | `ui_icon_levelselect_island.png` | 256x256 | grassy island + red flag |
| 6 | `ui_icon_wardrobe_hanger.png` | 256x256 | gold hanger + star |
| 7 | `ui_icon_settings_gear.png` | 256x256 | friendly gear |
| 8 | `ui_icon_quit_door.png` | 256x256 | wooden door |
| 9 | `ui_deco_cloud_corner.png` | 512x512 | cloud-puff corner cluster (overlay) |
| 10 | `ui_deco_vine_corner.png` | 512x512 | leafy vine + flowers + tiny stars corner (overlay) |
| 11 | `ui_deco_island_base.png` | 1600x560 | grassy floating-island platform the stack rests on |
| 12 | `ui_deco_gem.png` | 256x256 | the blue rainbow gem |
| 13 | `ui_wordmark_lockup.png` | 1600x900 | full title lockup - CANDIDATE, must read "JEBBY JUMP" |

## Split delivery — two requests (image-count cap)

If your image limit is under 13, run this in **two requests**. Each request
produces its own zip via `--ids` (pass the SAME id list to BOTH the validate and
build commands). Each zip extracts to the same repo paths; integration merges them.

- **Request A — Chrome (8):** `UI01-BTN-PRIMARY,UI01-BTN-SECONDARY,UI01-PANEL,UI01-ICON-CONTINUE,UI01-ICON-LEVELS,UI01-ICON-WARDROBE,UI01-ICON-SETTINGS,UI01-ICON-QUIT`
  → deliver `jebby_art_UI01_A.zip`
- **Request B — Decoration + hero (5):** `UI01-DECO-CLOUD,UI01-DECO-VINE,UI01-ISLAND,UI01-GEM,UI01-WORDMARK`
  → deliver `jebby_art_UI01_B.zip`

Example (Request A):
`python ProductionArtAudit/tools/validate_generated_art.py --manifest Assets/_JebbyJump/Docs/Design/UIThemeKit/UI01_zip_manifest.json --dir <out> --ids UI01-BTN-PRIMARY,UI01-BTN-SECONDARY,UI01-PANEL,UI01-ICON-CONTINUE,UI01-ICON-LEVELS,UI01-ICON-WARDROBE,UI01-ICON-SETTINGS,UI01-ICON-QUIT`
then the same `--ids` on `build_final_art_zip.py --out jebby_art_UI01_A.zip`.
Drop both zips in `Downloads/jebby-jump/jebby_art_UI01`.

## Step 4 - validate + deliver

**Deliver ONE zip per request (see split above). Do NOT return loose PNG files.**

1. Write all 13 PNGs into a working dir `<out>`, preserving their full
   repo-relative paths (i.e. `<out>/Assets/_JebbyJump/Art/Sprites/UI/...`).
2. Validate:
   `python ProductionArtAudit/tools/validate_generated_art.py --manifest Assets/_JebbyJump/Docs/Design/UIThemeKit/UI01_zip_manifest.json --dir <out>`
3. On PASS build the zip:
   `python ProductionArtAudit/tools/build_final_art_zip.py --manifest Assets/_JebbyJump/Docs/Design/UIThemeKit/UI01_zip_manifest.json --dir <out> --out jebby_art_UI01.zip`
4. Deliver the single file `jebby_art_UI01.zip` to the folder
   `Downloads/jebby-jump/jebby_art_UI01` on the user's machine.

**Zip contents rule:** exactly the 13 manifest PNGs at exactly their manifest
paths - nothing else. No `.meta`/prefab/scene/material files, no README/report
files inside the zip.

Attach as **separate chat attachments** (not inside the zip): the validation
report output; a **flat preview of the two button frames stretched to a wide
button size** (e.g. 900x150) proving the edges tile with no seam and the centre
is clean/empty; and a mock of the full menu (frames + icons + decorations + label
text) so the composition can be checked against the concept.

## Non-negotiables
Exact filenames/paths/sizes/alpha (all RGBA transparent); frames are clean,
seam-free, text-free 9-slice with motifs only in the fixed corners; icons are
cute-cartoon and readable at 64 px; decorations and icons on clean transparency;
wordmark reads unambiguously "JEBBY JUMP" and is a CANDIDATE only; warm and
child-safe throughout; no stray annotations; no
.meta/prefab/scene/material/atlas/ScriptableObject files; report the validator's
real output - do not claim a PASS you did not run.

When delivered, STOP.
