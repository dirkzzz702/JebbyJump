# Paste-Ready Kickoff Prompt — Batch W03 (Crystal Caves world art)

---

You are producing FINAL production art for the Unity game **Jebby Jump**,
continuing an audited pipeline (batches 001-007 and W02 accepted & integrated).
This session executes **batch W03 only**: the complete art family for
**World 03 - Crystal Caves**. Precise EXECUTION, not redesign.

## Repository access
Public repo `https://github.com/dirkzzz702/JebbyJump` (branch `main`). Fetch via raw URLs, e.g.
`https://raw.githubusercontent.com/dirkzzz702/JebbyJump/main/Assets/_JebbyJump/Docs/Design/WorldExpansion100/art_batches/W03_zip_manifest.json`

## Step 1 - load instructions (ALL, before generating)
1. `ProductionArtAudit/generation/MASTER_CHATGPT_ART_PRODUCTION_PROMPT.md` - binding rules.
2. `Assets/_JebbyJump/Docs/Design/WorldExpansion100/art_batches/W03_zip_manifest.json` - exact sizes/paths/alpha the validator enforces.
3. `Assets/_JebbyJump/Docs/Design/WorldExpansion100/04_TEN_WORLD_CREATIVE_BIBLE.md` (W03 section) + `17_WORLD_ART_SYSTEM_AND_TECHNICAL_SPECS.md`.
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
W03 stage: the tower gleams far down a crystal shaft - small, clearer than W02, base tiers becoming visible.

## Step 3 - produce the 12 W03 assets
All under `Assets/_JebbyJump/Art/Worlds/W03_CrystalCaves/`:

| # | File | Size | Alpha |
|---|---|---|---|
| 1 | `Backgrounds/bg_crystalcaves_01.png` | 2400x1080 | opaque |
| 2 | `Backgrounds/landmark_tower_crystalcaves_01.png` | 900x1400 | transparent |
| 3 | `Floor/floor_crystalcaves_01.png` | 512x128 | opaque |
| 4 | `Platforms/plat_crystalcaves_base_01.png` | 256x96 | transparent |
| 5 | `Obstacles/hazard_crystal_spike_cluster_01.png` | 1254x1254 | transparent |
| 6 | `Decorations/deco_crystalcaves_01.png` | 1024x1024 | transparent |
| 7 | `UI/world_thumb_crystalcaves_01.png` | 512x512 | opaque |
| 8 | `UI/world_badge_crystalcaves_01.png` | 192x192 | transparent |
| 9 | `UI/story_card_crystalcaves_01.png` | 1600x900 | opaque |
| 10 | `UI/finale_crystalcaves_01.png` | 1920x1080 | transparent |
| 11 | `UI/world_gem_crystalcaves_01.png` | 256x256 | transparent |
| 12 | `UI/cosmetic_geode_pauldrons_01.png` | 512x512 | transparent |

World identity: luminous caverns of singing crystal. Mood: wondrous, sparkling, warmly lit. Material: faceted crystal.
Palette: violet + warm torchglow (support cyan + rose). Lighting: crystal glow plus warm torchlight from below-left. Atmosphere: gentle drifting sparkle motes.
Hazard: **a cluster of stubby crystal shards** - rendered in the cute-cartoon language above.
Finale set-piece: a cathedral of crystal. Cosmetic reward: **Geode pauldrons**.

### The one asset that must not be wrong - the platform base
It is **MULTIPLIED BY A COLOUR at runtime** (`sr.color = PlatformColorPalette.GetColor(...)`).
It must be **near-greyscale (target saturation < 0.05), mid-value (roughly 0.30-0.95),
NO inherent hue** - only material detail (faceted crystal texture, soft top-light, rounded edge).
W02's base measured saturation 0.000 and is the reference standard.

Prove it: render your base tinted to **#E63838, #3878E6, #38BF59, #FAD12E, #9938E6,
#F28C26** and confirm all six stay bright and mutually distinguishable at ~70 px.
Include that six-tint strip **over your new background** in the delivery.

## Step 4 - validate + deliver
- `python ProductionArtAudit/tools/validate_generated_art.py --manifest Assets/_JebbyJump/Docs/Design/WorldExpansion100/art_batches/W03_zip_manifest.json --dir <out>`
- On PASS: `python ProductionArtAudit/tools/build_final_art_zip.py --manifest .../W03_zip_manifest.json --dir <out> --out jebby_art_W03.zip`
- Deliver to `C:\Users\dontb\Downloads\jebby-jump\jebby_art_W03`
- Return: ZIP + validation_report + the six-tint strip over the new background +
  the measured mean-luma / edge-energy / stddev of the background.

## Non-negotiables
Exact filenames/paths/sizes/alpha; platform base near-greyscale and tint-safe;
hazard cute-cartoon and unmistakably a hazard, never a safe platform, never scary;
tower = the tiered rainbow-disc spire at the W03 distance; background meets the
calm targets; warm and child-safe throughout; no text baked into any asset; no
characters; no AI artefacts; no .meta/prefab/scene/material/atlas/ScriptableObject
files; report the validator's real output - do not claim a PASS you did not run.

When delivered, STOP.
