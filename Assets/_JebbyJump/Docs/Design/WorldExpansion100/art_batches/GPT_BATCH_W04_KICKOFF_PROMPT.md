# Paste-Ready Kickoff Prompt — Batch W04 (Sunshine Desert world art)

---

You are producing FINAL production art for the Unity game **Jebby Jump**,
continuing an audited pipeline (batches 001-007 and W02 accepted & integrated).
This session executes **batch W04 only**: the complete art family for
**World 04 - Sunshine Desert**. Precise EXECUTION, not redesign.

## Repository access
Public repo `https://github.com/dirkzzz702/JebbyJump` (branch `main`). Fetch via raw URLs, e.g.
`https://raw.githubusercontent.com/dirkzzz702/JebbyJump/main/Assets/_JebbyJump/Docs/Design/WorldExpansion100/art_batches/W04_zip_manifest.json`

## Step 1 - load instructions (ALL, before generating)
1. `ProductionArtAudit/generation/MASTER_CHATGPT_ART_PRODUCTION_PROMPT.md` - binding rules.
2. `Assets/_JebbyJump/Docs/Design/WorldExpansion100/art_batches/W04_zip_manifest.json` - exact sizes/paths/alpha the validator enforces.
3. `Assets/_JebbyJump/Docs/Design/WorldExpansion100/04_TEN_WORLD_CREATIVE_BIBLE.md` (W04 section) + `17_WORLD_ART_SYSTEM_AND_TECHNICAL_SPECS.md`.
4. `ProductionArtAudit/tools/validate_generated_art.py` + `build_final_art_zip.py`.

## Step 2 - references (HARD GATE)
Fetch/upload before generating (Git-LFS: follow raw-URL redirects; confirm real PNGs, not pointer text):
- `Assets/_JebbyJump/Art/Sprites/Backgrounds/bg_menu_01.png` - **style anchor + the Rainbow Tower's canonical design.**
- `Assets/_JebbyJump/Art/Sprites/Obstacles/spr_cactus_obstacle_01.png` - **hazard language anchor** (cute cartoon, smiling face).
- `Assets/_JebbyJump/Art/Sprites/Platforms/spr_platform_base_01.png` - platform silhouette / 9-slice proportions.

## THE TWO ART LANGUAGES (non-negotiable - this is what W02 got wrong)

| Layer | Language | Anchor |
|---|---|---|
| Backgrounds / story art | soft **painted** storybook, atmospheric, LOW contrast + LOW detail | `bg_menu_01.png` |
| **Gameplay objects** (hazard, platform base) | **cute cartoon**: chunky, friendly, clean silhouette, warm, often a simple face | `spr_cactus_obstacle_01.png` |

The shipped cactus is a smiling, rosy-cheeked, chunky green cactus. Every world's
hazard must speak that language, re-themed. Painterly / realistic / menacing
gameplay objects are rejected - this is a children's game.

### Background calm targets (measured on the centre 62% crop) - HARD
| Metric | Target |
|---|---|
| mean luma (0-255) | **>= 150** |
| edge energy (busyness) | **<= 4.5** |
| luma stddev | **<= 40** |

The central horizontal band where platforms sit must be visually calm so six
tinted platforms read instantly. Think "soft painted backdrop", not "detailed
illustration". Detail may live in the outer left/right thirds.

### The Rainbow Tower is one fixed structure
A tall slender spire of **stacked tiered rainbow discs / saucers** in pastel pink,
blue, teal and lilac with small flags and a bright finial (see `bg_menu_01.png`,
right side). It is the SAME structure in all ten worlds - never a stone castle.
W04 stage: the tower rises beyond the far dunes - small, clear, tiers hinted.

## Step 3 - produce the 12 W04 assets
All under `Assets/_JebbyJump/Art/Worlds/W04_SunshineDesert/`:

| # | File | Size | Alpha |
|---|---|---|---|
| 1 | `Backgrounds/bg_sunshinedesert_01.png` | 2400x1080 | opaque |
| 2 | `Backgrounds/landmark_tower_sunshinedesert_01.png` | 900x1400 | transparent |
| 3 | `Floor/floor_sunshinedesert_01.png` | 512x128 | opaque |
| 4 | `Platforms/plat_sunshinedesert_base_01.png` | 256x96 | transparent |
| 5 | `Obstacles/hazard_desert_cactus_01.png` | 1254x1254 | transparent |
| 6 | `Decorations/deco_sunshinedesert_01.png` | 1024x1024 | transparent |
| 7 | `UI/world_thumb_sunshinedesert_01.png` | 512x512 | opaque |
| 8 | `UI/world_badge_sunshinedesert_01.png` | 192x192 | transparent |
| 9 | `UI/story_card_sunshinedesert_01.png` | 1600x900 | opaque |
| 10 | `UI/finale_sunshinedesert_01.png` | 1920x1080 | transparent |
| 11 | `UI/world_gem_sunshinedesert_01.png` | 256x256 | transparent |
| 12 | `UI/cosmetic_sunwrap_scarf_01.png` | 512x512 | transparent |

World identity: warm dunes under a friendly sun. Mood: bright, adventurous, warm. Material: sun-baked sandstone.
Palette: golden sand + sky (support coral + turquoise). Lighting: strong warm sun, high. Atmosphere: heat shimmer and soft wind.
Hazard: **a sun-baked desert cactus** - rendered in the cute-cartoon language above.
Finale set-piece: a sunlit ruin arch. Cosmetic reward: **Sunwrap scarf**.

### The one asset that must not be wrong - the platform base
It is **MULTIPLIED BY A COLOUR at runtime** (`sr.color = PlatformColorPalette.GetColor(...)`).
It must be **near-greyscale (target saturation < 0.05), mid-value (roughly 0.30-0.95),
NO inherent hue** - only material detail (sun-baked sandstone texture, soft top-light, rounded edge).
W02's base measured saturation 0.000 and is the reference standard.

Prove it: render your base tinted to **#E63838, #3878E6, #38BF59, #FAD12E, #9938E6,
#F28C26** and confirm all six stay bright and mutually distinguishable at ~70 px.
Include that six-tint strip **over your new background** in the delivery.

## Step 4 - validate + deliver

**Deliver exactly ONE zip. Do NOT return loose PNG files.**

1. Write all 12 PNGs into a working dir `<out>`, preserving their full
   repo-relative paths (i.e. `<out>/Assets/_JebbyJump/Art/Worlds/W04_SunshineDesert/...`).
2. Validate:
   `python ProductionArtAudit/tools/validate_generated_art.py --manifest Assets/_JebbyJump/Docs/Design/WorldExpansion100/art_batches/W04_zip_manifest.json --dir <out>`
3. On PASS build the zip:
   `python ProductionArtAudit/tools/build_final_art_zip.py --manifest Assets/_JebbyJump/Docs/Design/WorldExpansion100/art_batches/W04_zip_manifest.json --dir <out> --out jebby_art_W04.zip`
4. Deliver the single file `jebby_art_W04.zip` to the folder
   `Downloads/jebby-jump/jebby_art_W04` on the user's machine.

**Zip contents rule:** exactly the 12 manifest PNGs at exactly their manifest
paths - nothing else. Do NOT put `.meta`/prefab/scene/material files or your own
README/validation/report files inside the zip; the validator rejects unexpected
files in the package.

Attach as **separate chat attachments** (not inside the zip): the validation
report output, the six-tint platform strip rendered over your new background, and
the measured mean-luma / edge-energy / stddev of the background.

(If a later fix-pass asks for only some assets, pass `--ids` on BOTH commands with
just those asset ids; the zip then contains only those files.)

## Non-negotiables
Exact filenames/paths/sizes/alpha; platform base near-greyscale and tint-safe;
hazard cute-cartoon and unmistakably a hazard, never a safe platform, never scary;
tower = the tiered rainbow-disc spire at the W04 distance; background meets the
calm targets; warm and child-safe throughout; no text baked into any asset; no
characters; no AI artefacts; no .meta/prefab/scene/material/atlas/ScriptableObject
files; report the validator's real output - do not claim a PASS you did not run.

When delivered, STOP.
