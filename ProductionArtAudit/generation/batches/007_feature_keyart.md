# Batch 007_feature_keyart

Read `../MASTER_CHATGPT_ART_PRODUCTION_PROMPT.md` first. Generate ONLY the
assets below, then validate, ZIP, and STOP.

## Assets (briefs are authoritative)
- **ART-016** — Marketing key art → `../../asset_briefs/ART-016.md`
- **ART-004** — Play feature graphic → `../../asset_briefs/ART-004.md`

## References to upload BEFORE generating (see ../REFERENCE_UPLOAD_INDEX.md)
- jebby_color_knight_character_sheet_v01.png
- jebby_outfit_variations_board_v01.png
- Game__PlayingHUD__1686x948__none__level1-timer-running.png
Generation without a listed reference is FORBIDDEN — ask for the upload.

## Batch notes / consistency
Generate the 3840x2160 key art (ART-016) FIRST, then compose/crop the 1024x500 feature graphic (ART-004) from it (dependent outputs). Feature graphic ships TEXT-FREE unless the approved ART-010 wordmark is supplied.

## Output paths (exact; repo-root relative)
- `Marketing/KeyArt/jebby_jump_keyart_3840x2160.png`
- `StoreAssets/Feature/jebby_jump_feature_1024x500.png`

## Acceptance gate for this batch
Identity exact; ART-004 crop keeps Jebby's face; NO alpha in either.

## After generating
Run the validator (subset): `python tools/validate_generated_art.py --manifest
zip_blueprint/zip_manifest.json --dir <outdir> --ids ART-016,ART-004` then build the ZIP
only on PASS. Report failures honestly. Do NOT mix Unity-native files into the
ZIP. STOP after this batch.
