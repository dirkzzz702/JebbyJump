# Batch 006_skill_icons_vfx

Read `../MASTER_CHATGPT_ART_PRODUCTION_PROMPT.md` first. Generate ONLY the
assets below, then validate, ZIP, and STOP.

## Assets (briefs are authoritative)
- **ART-011** — Bubble Shield skill icon → `../../asset_briefs/ART-011.md`
- **ART-012** — Health Potion skill icon → `../../asset_briefs/ART-012.md`
- **ART-013** — Bubble shield VFX sprite → `../../asset_briefs/ART-013.md`
- **ART-014** — Color echo ring VFX sprite → `../../asset_briefs/ART-014.md`

## References to upload BEFORE generating (see ../REFERENCE_UPLOAD_INDEX.md)
- ui_btn_jump.png
- spr_item_rocket_boots_01.png
- ui_icon_life_01.png
- jebby_color_knight_character_sheet_v01.png
Generation without a listed reference is FORBIDDEN — ask for the upload.

## Batch notes / consistency
Icons (011/012) share the ornate icon family; VFX (013/014) are clean shapes. ART-014 must be PURE WHITE/neutral (runtime tinting).

## Output paths (exact; repo-root relative)
- `Assets/_JebbyJump/Art/Sprites/UI/ui_icon_skill_shield_01.png`
- `Assets/_JebbyJump/Art/Sprites/UI/ui_icon_skill_potion_01.png`
- `Assets/_JebbyJump/Art/Sprites/VFX/fx_bubble_shield_01.png`
- `Assets/_JebbyJump/Art/Sprites/VFX/fx_color_echo_ring_01.png`

## Acceptance gate for this batch
70px readability check (downscale preview); ART-014 hue==neutral validator.

## After generating
Run the validator (subset): `python tools/validate_generated_art.py --manifest
zip_blueprint/zip_manifest.json --dir <outdir> --ids ART-011,ART-012,ART-013,ART-014` then build the ZIP
only on PASS. Report failures honestly. Do NOT mix Unity-native files into the
ZIP. STOP after this batch.
