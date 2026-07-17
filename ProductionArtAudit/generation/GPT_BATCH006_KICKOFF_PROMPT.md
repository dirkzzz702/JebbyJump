# Paste-Ready Kickoff Prompt — Batch 006 (Skill Icons + VFX Sprites)

---

You are producing FINAL production art for the Unity game **Jebby Jump**,
continuing an audited pipeline (batches 001–005 accepted & integrated). This
session executes **batch 006 only**: four assets — ART-011 (Bubble Shield
icon), ART-012 (Health Potion icon), ART-013 (shield aura VFX), ART-014
(colour-echo ring VFX). The two icons are one ornate family; the two VFX are
clean shapes. Precise EXECUTION, not redesign; the specs win over instinct.

## Repository access
Public repo `https://github.com/dirkzzz702/JebbyJump` (branch `main`). Fetch
via raw URLs, e.g.
`https://raw.githubusercontent.com/dirkzzz702/JebbyJump/main/ProductionArtAudit/asset_briefs/ART-011.md`

## Step 1 — load instructions (ALL, before generating)
1. `ProductionArtAudit/generation/MASTER_CHATGPT_ART_PRODUCTION_PROMPT.md` — binding rules.
2. `ProductionArtAudit/generation/batches/006_skill_icons_vfx.md` — this batch.
3. `ProductionArtAudit/asset_briefs/ART-011.md`, `ART-012.md`, `ART-013.md`, `ART-014.md` — full specs.
4. `ProductionArtAudit/zip_blueprint/zip_manifest.json` + `ProductionArtAudit/tools/validate_generated_art.py` + `ProductionArtAudit/tools/build_final_art_zip.py`.

## Step 2 — references (HARD GATE for the two ICONS)
Upload before generating (Git-LFS: follow raw-URL redirects, confirm real
PNGs, not pointer text; if unavailable STOP and ask me to upload):
- `Assets/_JebbyJump/Art/Sprites/Items/spr_item_rocket_boots_01.png` — THE
  icon style anchor (the existing skill-1 icon; match its rendering).
- `Assets/_JebbyJump/Art/Sprites/UI/ui_btn_jump.png` — button family context.
- `Assets/_JebbyJump/Art/Sprites/UI/ui_icon_life_01.png` — heart motif for the
  potion.
The two VFX sprites (ART-013/014) are clean geometric shapes and need no
reference uploads.

## Step 3 — execute
**ART-011** — `Assets/_JebbyJump/Art/Sprites/UI/ui_icon_skill_shield_01.png`,
exactly **256×256 RGBA** (generate at 512, downscale): a glossy translucent
aqua-blue magic bubble (#3878E6 family) with a soft white rim highlight and a
tiny shield-shaped glint at its centre, in the painterly-ornate style of the
rocket-boots icon. Centred, ~78% of canvas, fully transparent background.

**ART-012** — `Assets/_JebbyJump/Art/Sprites/UI/ui_icon_skill_potion_01.png`,
exactly **256×256 RGBA**: a cute rounded glass potion bottle with a cork,
glowing warm pink-red liquid and ONE tiny heart-shaped bubble, gold-tinted
glass rim matching the icon family. NO medical cross. Centred ~78%,
transparent background.

**ART-013** — `Assets/_JebbyJump/Art/Sprites/VFX/fx_bubble_shield_01.png`,
exactly **512×512 RGBA** (generate at 1024): a PERFECT circular soap-bubble
shield — thin bright aqua-white rim, interior at only ~30% opacity with soft
radial falloff, two gentle curved specular highlights upper-left, fully
transparent outside the circle. Radially clean (no directionality). Gameplay
must stay visible through it. Brief §20: verify radial symmetry (rotate-90
mean diff < 6/255) with Pillow.

**ART-014** — `Assets/_JebbyJump/Art/Sprites/VFX/fx_color_echo_ring_01.png`,
exactly **512×512 RGBA**: a single perfect hollow ring — PURE WHITE ONLY
(zero colour saturation; the game tints it at runtime to six different
colours), crisp outer edge, soft-faded inner edge, ring thickness ~8% of
diameter, completely transparent centre and corners. The validator REJECTS
any coloured pixel. Prefer deterministic drawing (Pillow) over generation for
this one.

## Step 4 — validate + deliver
- `python ProductionArtAudit/tools/validate_generated_art.py --manifest ProductionArtAudit/zip_blueprint/zip_manifest.json --dir <out> --ids ART-011,ART-012,ART-013,ART-014`
- On PASS: `python ProductionArtAudit/tools/build_final_art_zip.py --manifest ProductionArtAudit/zip_blueprint/zip_manifest.json --dir <out> --out jebby_art_batch006.zip --ids ART-011,ART-012,ART-013,ART-014`
- Return: ZIP + validation_report.md + a preview strip showing both ICONS
  downscaled to 70px inside a dark circle (the in-game button size — I judge
  readability there) and the shield VFX composited at 30% over a busy pastel
  mock + the briefs' §22 acceptance checkboxes.

## Non-negotiables
Exact filenames/paths/sizes; transparent corners on all four; icons match the
rocket-boots rendering style; ART-014 strictly neutral white; no text; no
characters; no halos; no .meta/Unity-native files; report the validator's
real output. Image delivery only — Unity wiring (Skill2Icon/Skill3Icon Image
assignment + the two effect SpriteRenderers) happens later in the project.

When delivered, STOP. Batch 007 (key art + feature graphic) runs in a new
session.
