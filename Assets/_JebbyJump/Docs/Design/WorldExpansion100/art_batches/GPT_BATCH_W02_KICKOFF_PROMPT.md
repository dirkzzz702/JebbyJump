# Paste-Ready Kickoff Prompt — Batch W02 (Enchanted Forest world art)

---

You are producing FINAL production art for the Unity game **Jebby Jump**,
continuing an audited pipeline (batches 001–007 accepted & integrated). This
session executes **batch W02 only**: the complete art family for **World 02 —
Enchanted Forest**, the second of ten themed worlds. Precise EXECUTION, not
redesign: the art direction, palette and technical contracts are already locked.

## Repository access
Public repo `https://github.com/dirkzzz702/JebbyJump` (branch `main`). Fetch
via raw URLs, e.g.
`https://raw.githubusercontent.com/dirkzzz702/JebbyJump/main/Assets/_JebbyJump/Docs/Design/WorldExpansion100/art_batches/W02_ENCHANTED_FOREST_BATCH.md`

## Step 1 — load instructions (ALL, before generating)
1. `ProductionArtAudit/generation/MASTER_CHATGPT_ART_PRODUCTION_PROMPT.md` — binding rules (still authoritative).
2. `Assets/_JebbyJump/Docs/Design/WorldExpansion100/art_batches/W02_ENCHANTED_FOREST_BATCH.md` — **this batch, full per-asset spec**.
3. `Assets/_JebbyJump/Docs/Design/WorldExpansion100/art_batches/W02_zip_manifest.json` — exact sizes/paths/alpha the validator enforces.
4. `Assets/_JebbyJump/Docs/Design/WorldExpansion100/04_TEN_WORLD_CREATIVE_BIBLE.md` (W02 section) and `17_WORLD_ART_SYSTEM_AND_TECHNICAL_SPECS.md`.
5. `ProductionArtAudit/tools/validate_generated_art.py` + `ProductionArtAudit/tools/build_final_art_zip.py`.

## Step 2 — references (HARD GATE)
Upload/fetch before generating (Git-LFS: follow raw-URL redirects, confirm real
PNGs, not pointer text; if unavailable STOP and ask me to upload):
- `Assets/_JebbyJump/Art/Sprites/Backgrounds/bg_menu_01.png` — **the style anchor.** Match this warmth, softness and painted-storybook quality.
- `Assets/_JebbyJump/Art/Sprites/Platforms/spr_platform_base_01.png` — platform silhouette + 9-slice proportions to match.
- `Assets/_JebbyJump/Art/Sprites/Obstacles/spr_cactus_obstacle_01.png` — hazard footprint/scale to match.
- `Assets/_JebbyJump/Docs/Art/References/jebby_color_knight_character_sheet_v01.png` — LOCKED identity (context only; **Jebby does not appear in any of these assets**).

## Step 3 — produce the 12 W02 assets
Follow `W02_ENCHANTED_FOREST_BATCH.md` exactly. Summary of outputs (all under
`Assets/_JebbyJump/Art/Worlds/W02_EnchantedForest/`):

| # | File | Size | Alpha |
|---|---|---|---|
| 1 | `Backgrounds/bg_enchantedforest_01.png` | 2400×1080 | opaque |
| 2 | `Backgrounds/landmark_tower_enchantedforest_01.png` | 900×1400 | transparent |
| 3 | `Floor/floor_enchantedforest_01.png` | 512×128 | opaque |
| 4 | `Platforms/plat_enchantedforest_base_01.png` | 256×96 | transparent |
| 5 | `Obstacles/hazard_forest_thorn_bloom_01.png` | 1254×1254 | transparent |
| 6 | `Decorations/deco_enchantedforest_01.png` | 1024×1024 | transparent |
| 7 | `UI/world_thumb_enchantedforest_01.png` | 512×512 | opaque |
| 8 | `UI/world_badge_enchantedforest_01.png` | 192×192 | transparent |
| 9 | `UI/story_card_enchantedforest_01.png` | 1600×900 | opaque |
| 10 | `UI/finale_enchantedforest_01.png` | 1920×1080 | transparent |
| 11 | `UI/world_gem_enchantedforest_01.png` | 256×256 | transparent |
| 12 | `UI/cosmetic_leafcrown_hood_01.png` | 512×512 | transparent |

World identity: glowing woodland of mossy boughs; cozy/magical/verdant; deep
green + amber with teal + gold support; dappled canopy light; floating spores
and fireflies. The recurring **Rainbow Tower** appears at its W02 stage only:
*glimpsed through the trees* — small, hazy, very far, silhouette-only.

### The one asset that must not be wrong — #4 platform base
This sprite is **MULTIPLIED BY A COLOUR at runtime**
(`sr.color = PlatformColorPalette.GetColor(...)`). It must be **near-greyscale,
mid-value, with NO inherent hue** — only material detail (moss texture, bark
grain, soft top-light, rounded edge). A green forest plank multiplies to mud
under the red and purple tints and will be rejected.

Prove it: render your base tinted to **#E63838, #3878E6, #38BF59, #FAD12E,
#9938E6, #F28C26** and confirm all six stay bright and mutually distinguishable
at ~70 px. Include that six-tint strip in your delivery.

## Step 4 — validate + deliver
- `python ProductionArtAudit/tools/validate_generated_art.py --manifest Assets/_JebbyJump/Docs/Design/WorldExpansion100/art_batches/W02_zip_manifest.json --dir <out>`
- On PASS: `python ProductionArtAudit/tools/build_final_art_zip.py --manifest Assets/_JebbyJump/Docs/Design/WorldExpansion100/art_batches/W02_zip_manifest.json --dir <out> --out jebby_art_W02.zip`
- Return: ZIP + validation_report.md + the **six-tint platform strip** + a 70 px
  preview of the platform base and the hazard (I judge readability there).

## Non-negotiables
Exact filenames/paths/sizes/alpha per the manifest; platform base near-greyscale
and tint-safe; hazard unmistakably a hazard and never mistakable for a safe
platform; tower is the same structure as other worlds at the W02 distance;
warm and child-safe throughout (no cold/grey wash, nothing scary); no text baked
into any asset; no characters; no AI artefacts (melted shapes, extra limbs); no
`.meta`/prefab/scene/material/atlas/ScriptableObject files; report the
validator's real output — do not claim PASS you did not run.

When delivered, STOP. Batch W03 (Crystal Caves) is a separate session.
