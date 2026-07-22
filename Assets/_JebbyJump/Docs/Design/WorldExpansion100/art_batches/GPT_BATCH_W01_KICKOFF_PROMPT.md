# Paste-Ready Kickoff Prompt — Batch W01 (Cloud Meadow world art)

---

You are producing FINAL production art for the Unity game **Jebby Jump**,
continuing an audited pipeline (batches 001-007 and W02-W10 accepted &
integrated). This session executes **batch W01 only**: the complete art family
for **World 01 - Cloud Meadow**, the game's FIRST world. Precise EXECUTION, not
redesign. W01 is the last world without its own bespoke family - it currently
borrows generic shipped art; this batch gives it a themed family matching W02-W10.

## Repository access
Public repo `https://github.com/dirkzzz702/JebbyJump` (branch `main`). Fetch via raw URLs, e.g.
`https://raw.githubusercontent.com/dirkzzz702/JebbyJump/main/Assets/_JebbyJump/Docs/Design/WorldExpansion100/art_batches/W01_zip_manifest.json`

## Step 1 - load instructions (ALL, before generating)
1. `ProductionArtAudit/generation/MASTER_CHATGPT_ART_PRODUCTION_PROMPT.md` - binding rules.
2. `Assets/_JebbyJump/Docs/Design/WorldExpansion100/art_batches/W01_zip_manifest.json` - exact sizes/paths/alpha the validator enforces.
3. `Assets/_JebbyJump/Docs/Design/WorldExpansion100/04_TEN_WORLD_CREATIVE_BIBLE.md` (W01 section) + `17_WORLD_ART_SYSTEM_AND_TECHNICAL_SPECS.md`.
4. `ProductionArtAudit/tools/validate_generated_art.py` + `build_final_art_zip.py`.

## Step 2 - references (HARD GATE)
Fetch/upload before generating (Git-LFS: follow raw-URL redirects; confirm real PNGs, not pointer text):
- `Assets/_JebbyJump/Art/Sprites/Backgrounds/bg_menu_01.png` - **style anchor + the Rainbow Tower's canonical design.** This warm cloud/rainbow key art IS Cloud Meadow's visual language - match its softness, warmth and palette closely. (It stays the game's menu background; your `bg_cloudmeadow_01` is W01's own distinct GAMEPLAY background in the same language.)
- `Assets/_JebbyJump/Art/Sprites/Obstacles/spr_cactus_obstacle_01.png` - **hazard language anchor** (cute cartoon, smiling face).
- `Assets/_JebbyJump/Art/Sprites/Platforms/spr_platform_base_01.png` - platform silhouette / 9-slice proportions **and the neutral-base standard** (this shipped base measures ~0 saturation; match it).

## THE TWO ART LANGUAGES (non-negotiable - this is what W02 got wrong)

| Layer | Language | Anchor |
|---|---|---|
| Backgrounds / story art | soft **painted** storybook, atmospheric, LOW contrast + LOW detail | `bg_menu_01.png` |
| **Gameplay objects** (hazard, platform base) | **cute cartoon**: chunky, friendly, clean silhouette, warm, often a simple face | `spr_cactus_obstacle_01.png` |

The shipped cactus is a smiling, rosy-cheeked, chunky green cactus. Cloud
Meadow's hazard must speak that language, re-themed. Painterly / realistic /
menacing gameplay objects are rejected - this is a children's game, and W01 is
the very first thing a new player sees: it must be the softest, most welcoming
world of all ten.

### Background calm targets (measured on the centre 62% crop) - HARD
| Metric | Target |
|---|---|
| mean luma (0-255) | **>= 150** |
| edge energy (busyness) | **<= 4.5** |
| luma stddev | **<= 40** |
| warm-pixel share (R>=B) | **>= 45%** |

### WARM-SHARE WATCH (W01 is a cold-risk world)
Cloud Meadow's named palette is "warm sky blue + cream" - a blue sky can easily
fall below the 45% warm-pixel floor. Steer it warm deliberately: make **cream
and soft peach the dominant tones**, light the scene with a **warm midday sun**,
give the cloudpuffs **honey-warm highlights**, and keep any sky blue pale and
minor. The centre band where platforms sit especially must read warm and calm.
(W03/W05/W07/W08 were cold-risk too and all cleared 45% with this steering.)

### NO BAKED ANNOTATIONS (W03 lesson) - applies to every asset
Do not burn index numbers, labels, watermarks, captions, filenames or any other
annotation into any image - **including faint or low-contrast marks in corners**.
W03 shipped its manifest index in the bottom-left of most assets, including the
platform base, which would have tiled a faint digit across every level.

The central horizontal band where platforms sit must be visually calm so six
tinted platforms read instantly. Think "soft painted backdrop", not "detailed
illustration". Detail may live in the outer left/right thirds.

### The Rainbow Tower is one fixed structure
A tall slender spire of **stacked tiered rainbow discs / saucers** in pastel pink,
blue, teal and lilac with small flags and a bright finial (see `bg_menu_01.png`,
right side). It is the SAME structure in all ten worlds - never a stone castle.
W01 stage: this is where the journey BEGINS - the tower is **a tiny far-off
glimmer on the horizon**, small and distant, a promise of the climb ahead (still
a soft painted backdrop, calm centre band for platforms).

**Two separate uses, do not confuse them (W02 lesson):**
- The **background** shows the tower at this world's stage (here: tiny, far, on the horizon), painted into the scene.
- The standalone `landmark_tower_*` asset must be the **tower itself only** at full detail, plus at
  most a wisp of cloud/haze - tight to its content, on clean transparency with a soft
  feathered edge. Do NOT deliver a large oval 'scene blob' containing terrain or
  meadow; it is a layerable element, not a second illustration.

## Step 3 - produce the 12 W01 assets
All under `Assets/_JebbyJump/Art/Worlds/W01_CloudMeadow/`:

| # | File | Size | Alpha |
|---|---|---|---|
| 1 | `Backgrounds/bg_cloudmeadow_01.png` | 2400x1080 | opaque |
| 2 | `Backgrounds/landmark_tower_cloudmeadow_01.png` | 900x1400 | transparent |
| 3 | `Floor/floor_cloudmeadow_01.png` | 512x128 | opaque |
| 4 | `Platforms/plat_cloudmeadow_base_01.png` | 256x96 | transparent |
| 5 | `Obstacles/hazard_cloud_thorn_01.png` | 1254x1254 | transparent |
| 6 | `Decorations/deco_cloudmeadow_01.png` | 1024x1024 | transparent |
| 7 | `UI/world_thumb_cloudmeadow_01.png` | 512x512 | opaque |
| 8 | `UI/world_badge_cloudmeadow_01.png` | 192x192 | transparent |
| 9 | `UI/story_card_cloudmeadow_01.png` | 1600x900 | opaque |
| 10 | `UI/finale_cloudmeadow_01.png` | 1920x1080 | transparent |
| 11 | `UI/world_gem_cloudmeadow_01.png` | 256x256 | transparent |
| 12 | `UI/cosmetic_cloudpuff_cape_01.png` | 512x512 | transparent |

World identity: a gentle sky-meadow of drifting cloudpuffs - the welcoming first world. Mood: soft, welcoming, pastel, cozy. Material: cloud & meadow grass.
Palette: cream + soft peach dominant with pale warm sky blue + mint accents. Lighting: soft warm midday sun from top-left. Atmosphere: calm breeze, drifting cloud-fluff.
Hazard: **`cloud_thorn` - a chunky friendly thorn/bramble tuft** rendered in the cute-cartoon language above (a re-skin of the cactus behaviour - unmistakably a hazard, never scary, never a safe platform).
Finale set-piece: the **Cloud Meadow Crown** - a celebratory meadow-summit composition with the distant tower in focus. Cosmetic reward: **Cloudpuff cape**.

### The one asset that must not be wrong - the platform base
It is **MULTIPLIED BY A COLOUR at runtime** (`sr.color = PlatformColorPalette.GetColor(...)`).
It must be **near-greyscale (target saturation < 0.05), mid-value (roughly 0.30-0.95),
NO inherent hue** - only material detail (soft cloud & meadow-grass texture, soft top-light, rounded edge).
W02's base measured saturation 0.000 and is the reference standard, alongside the
shipped `spr_platform_base_01`.

Prove it: render your base tinted to **#E63838, #3878E6, #38BF59, #F9DE2C, #9938E6,
#E58A1B** and confirm all six stay bright and mutually distinguishable at ~70 px.
Include that six-tint strip **over your new background** in the delivery.

### Floor must be a TILEABLE GROUND STRIP (W03 lesson)
The floor fills the full 512x128 canvas edge to edge - no void, no framing, not an
isolated island. A **flat, straight, unbroken top surface line** runs the entire
width (characters stand on it), with the material body below filling to the bottom
edge. Left and right edges match so it repeats seamlessly. See the accepted
`Art/Worlds/W02_EnchantedForest/Floor/` for the correct shape.

## Step 4 - validate + deliver

**Deliver exactly ONE zip. Do NOT return loose PNG files.**

1. Write all 12 PNGs into a working dir `<out>`, preserving their full
   repo-relative paths (i.e. `<out>/Assets/_JebbyJump/Art/Worlds/W01_CloudMeadow/...`).
2. Validate:
   `python ProductionArtAudit/tools/validate_generated_art.py --manifest Assets/_JebbyJump/Docs/Design/WorldExpansion100/art_batches/W01_zip_manifest.json --dir <out>`
3. On PASS build the zip:
   `python ProductionArtAudit/tools/build_final_art_zip.py --manifest Assets/_JebbyJump/Docs/Design/WorldExpansion100/art_batches/W01_zip_manifest.json --dir <out> --out jebby_art_W01.zip`
4. Deliver the single file `jebby_art_W01.zip` to the folder
   `Downloads/jebby-jump/jebby_art_W01` on the user's machine.

**Zip contents rule:** exactly the 12 manifest PNGs at exactly their manifest
paths - nothing else. Do NOT put `.meta`/prefab/scene/material files or your own
README/validation/report files inside the zip; the validator rejects unexpected
files in the package.

Attach as **separate chat attachments** (not inside the zip): the validation
report output, the six-tint platform strip rendered over your new background, and
the measured warm-share / mean-luma / edge-energy / stddev of the background, and a
**bottom-left corner crop of all 12 assets proving no baked annotations**.

(If a later fix-pass asks for only some assets, pass `--ids` on BOTH commands with
just those asset ids; the zip then contains only those files.)

## Non-negotiables
Exact filenames/paths/sizes/alpha; platform base near-greyscale and tint-safe;
hazard cute-cartoon and unmistakably a hazard, never a safe platform, never scary;
tower = the tiered rainbow-disc spire, tiny and distant at the W01 stage;
background meets the calm targets AND the warm-share floor; warm, welcoming and
child-safe throughout (this is the first world a player sees); no text baked into
any asset; no characters; no AI artefacts; no
.meta/prefab/scene/material/atlas/ScriptableObject files; report the validator's
real output - do not claim a PASS you did not run.

When delivered, STOP.
