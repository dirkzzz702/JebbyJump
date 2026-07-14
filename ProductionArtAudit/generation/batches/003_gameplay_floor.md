# Batch 003_gameplay_floor

Read `../MASTER_CHATGPT_ART_PRODUCTION_PROMPT.md` first. Generate ONLY the
assets below, then validate, ZIP, and STOP.

## Assets (briefs are authoritative)
- **ART-005** — Gameplay floor/start strip sprite → `../../asset_briefs/ART-005.md`

## References to upload BEFORE generating (see ../REFERENCE_UPLOAD_INDEX.md)
- bg_sky_layer_01.png
- Game__PlayingHUD__1686x948__none__level1-timer-running.png
Generation without a listed reference is FORBIDDEN — ask for the upload.

## Batch notes / consistency
Single asset. Confirm design decision D1 (strip vs ledge) from decisions/OPEN_DECISIONS.md before generating; if unresolved, STOP and ask.

## Output paths (exact; repo-root relative)
- `Assets/_JebbyJump/Art/Sprites/Platforms/spr_floor_strip_01.png`

## Acceptance gate for this batch
Tile-seam validator PASS (left/right column mean diff < 2/255).

## After generating
Run the validator (subset): `python tools/validate_generated_art.py --manifest
zip_blueprint/zip_manifest.json --dir <outdir> --ids ART-005` then build the ZIP
only on PASS. Report failures honestly. Do NOT mix Unity-native files into the
ZIP. STOP after this batch.
