# Batch 009_hazard_variant

Read `../MASTER_CHATGPT_ART_PRODUCTION_PROMPT.md` first. Generate ONLY the
assets below, then validate, ZIP, and STOP.

## Assets (briefs are authoritative)
- **ART-017** — Cactus warning/contact variant → `../../asset_briefs/ART-017.md`

## References to upload BEFORE generating (see ../REFERENCE_UPLOAD_INDEX.md)
- spr_cactus_obstacle_01.png
Generation without a listed reference is FORBIDDEN — ask for the upload.

## Batch notes / consistency
GATED: requires gameplay design decision D4 (anticipation behaviour). If unresolved, STOP. Silhouette must match the uploaded base within 4% bbox.

## Output paths (exact; repo-root relative)
- `Assets/_JebbyJump/Art/Sprites/Obstacles/spr_cactus_obstacle_warn_01.png`

## Acceptance gate for this batch
Bbox-match validator PASS vs base cactus.

## After generating
Run the validator (subset): `python tools/validate_generated_art.py --manifest
zip_blueprint/zip_manifest.json --dir <outdir> --ids ART-017` then build the ZIP
only on PASS. Report failures honestly. Do NOT mix Unity-native files into the
ZIP. STOP after this batch.
