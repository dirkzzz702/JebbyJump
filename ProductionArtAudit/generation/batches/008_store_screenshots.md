# Batch 008_store_screenshots

Read `../MASTER_CHATGPT_ART_PRODUCTION_PROMPT.md` first. Generate ONLY the
assets below, then validate, ZIP, and STOP.

## Assets (briefs are authoritative)
- **ART-009** — Play store screenshots (set of 4+) → `../../asset_briefs/ART-009.md`

## References to upload BEFORE generating (see ../REFERENCE_UPLOAD_INDEX.md)
- (none — capture batch)
Generation without a listed reference is FORBIDDEN — ask for the upload.

## Batch notes / consistency
NOT an image-generation batch. Real captures at 1920x1080 from the game AFTER batches 003-006 are INTEGRATED in Unity. Only deterministic caption framing afterwards. If asked to fabricate gameplay images, refuse.

## Output paths (exact; repo-root relative)
- `StoreAssets/Screenshots/`

## Acceptance gate for this batch
Truthfulness: every frame is a real capture; 24-bit no alpha; min 4 frames.

## After generating
Run the validator (subset): `python tools/validate_generated_art.py --manifest
zip_blueprint/zip_manifest.json --dir <outdir> --ids ART-009` then build the ZIP
only on PASS. Report failures honestly. Do NOT mix Unity-native files into the
ZIP. STOP after this batch.
