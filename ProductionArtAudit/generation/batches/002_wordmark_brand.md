# Batch 002_wordmark_brand

Read `../MASTER_CHATGPT_ART_PRODUCTION_PROMPT.md` first. Generate ONLY the
assets below, then validate, ZIP, and STOP.

## Assets (briefs are authoritative)
- **ART-010** — Jebby Jump wordmark / logo → `../../asset_briefs/ART-010.md`

## References to upload BEFORE generating (see ../REFERENCE_UPLOAD_INDEX.md)
- jebby_color_knight_character_sheet_v01.png
Generation without a listed reference is FORBIDDEN — ask for the upload.

## Batch notes / consistency
Standalone brand asset. Deliver PNG render AND vector (SVG) master. Output is independent.

## Output paths (exact; repo-root relative)
- `Assets/_JebbyJump/Art/Sprites/UI/ui_wordmark_jebbyjump.png`

## Acceptance gate for this batch
HUMAN BRAND/LEGAL REVIEW REQUIRED before any store use — deliver as CANDIDATE.

## After generating
Run the validator (subset): `python tools/validate_generated_art.py --manifest
zip_blueprint/zip_manifest.json --dir <outdir> --ids ART-010` then build the ZIP
only on PASS. Report failures honestly. Do NOT mix Unity-native files into the
ZIP. STOP after this batch.
