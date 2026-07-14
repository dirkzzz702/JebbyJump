# Batch 005_ui_kit

Read `../MASTER_CHATGPT_ART_PRODUCTION_PROMPT.md` first. Generate ONLY the
assets below, then validate, ZIP, and STOP.

## Assets (briefs are authoritative)
- **ART-007** — Shell UI panel (9-slice) → `../../asset_briefs/ART-007.md`
- **ART-008** — Shell UI pill button (9-slice) → `../../asset_briefs/ART-008.md`

## References to upload BEFORE generating (see ../REFERENCE_UPLOAD_INDEX.md)
- ui_btn_jump.png
Generation without a listed reference is FORBIDDEN — ask for the upload.

## Batch notes / consistency
ONE chrome family (panel + pill share rim colour + radius language). Outputs dependent. Corner-radius decision D2 has candidate values in the briefs — usable as-is; note the choice.

## Output paths (exact; repo-root relative)
- `Assets/_JebbyJump/Art/Sprites/UI/ui_panel_soft_9s.png`
- `Assets/_JebbyJump/Art/Sprites/UI/ui_btn_pill_9s.png`

## Acceptance gate for this batch
Flat-stretch-region validator PASS on both; corner symmetry PASS.

## After generating
Run the validator (subset): `python tools/validate_generated_art.py --manifest
zip_blueprint/zip_manifest.json --dir <outdir> --ids ART-007,ART-008` then build the ZIP
only on PASS. Report failures honestly. Do NOT mix Unity-native files into the
ZIP. STOP after this batch.
