# Batch 001_store_identity

Read `../MASTER_CHATGPT_ART_PRODUCTION_PROMPT.md` first. Generate ONLY the
assets below, then validate, ZIP, and STOP.

## Assets (briefs are authoritative)
- **ART-001** — Google Play listing app icon → `../../asset_briefs/ART-001.md`
- **ART-002** — Android adaptive icon foreground → `../../asset_briefs/ART-002.md`
- **ART-003** — Android adaptive icon background → `../../asset_briefs/ART-003.md`
- **ART-015** — Adaptive icon monochrome layer → `../../asset_briefs/ART-015.md`

## References to upload BEFORE generating (see ../REFERENCE_UPLOAD_INDEX.md)
- jebby_color_knight_character_sheet_v01.png
- jebby_outfit_variations_board_v01.png
- bg_sky_layer_01.png
Generation without a listed reference is FORBIDDEN — ask for the upload.

## Batch notes / consistency
ONE consistent icon family. Generate ART-001 first; derive ART-002 subject from it; ART-003 is background-only; ART-015 is a DETERMINISTIC derivation of ART-002 (no fresh generation). Outputs are dependent (same family).

## Output paths (exact; repo-root relative)
- `StoreAssets/Icons/jebby_jump_icon_512.png`
- `Assets/_JebbyJump/Art/Icons/ic_launcher_foreground_432.png`
- `Assets/_JebbyJump/Art/Icons/ic_launcher_background_432.png`
- `Assets/_JebbyJump/Art/Icons/ic_launcher_monochrome_432.png`

## Acceptance gate for this batch
Identity vs locked sheet + safe-zone/corner validator PASS + <=1024KB icon.

## After generating
Run the validator (subset): `python tools/validate_generated_art.py --manifest
zip_blueprint/zip_manifest.json --dir <outdir> --ids ART-001,ART-002,ART-003,ART-015` then build the ZIP
only on PASS. Report failures honestly. Do NOT mix Unity-native files into the
ZIP. STOP after this batch.
