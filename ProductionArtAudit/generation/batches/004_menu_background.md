# Batch 004_menu_background

Read `../MASTER_CHATGPT_ART_PRODUCTION_PROMPT.md` first. Generate ONLY the
assets below, then validate, ZIP, and STOP.

## Assets (briefs are authoritative)
- **ART-006** — Main-menu background → `../../asset_briefs/ART-006.md`

## References to upload BEFORE generating (see ../REFERENCE_UPLOAD_INDEX.md)
- bg_sky_layer_01.png
- jebby_color_knight_character_sheet_v01.png
- Game__PlayingHUD__1686x948__none__level1-timer-running.png
Generation without a listed reference is FORBIDDEN — ask for the upload.

## Batch notes / consistency
Single asset; must read as the SAME world as the uploaded gameplay sky.

## Output paths (exact; repo-root relative)
- `Assets/_JebbyJump/Art/Sprites/Backgrounds/bg_menu_01.png`

## Acceptance gate for this batch
Centre-band luminance low enough for white TMP text (validator rule).

## After generating
Run the validator (subset): `python tools/validate_generated_art.py --manifest
zip_blueprint/zip_manifest.json --dir <outdir> --ids ART-006` then build the ZIP
only on PASS. Report failures honestly. Do NOT mix Unity-native files into the
ZIP. STOP after this batch.
