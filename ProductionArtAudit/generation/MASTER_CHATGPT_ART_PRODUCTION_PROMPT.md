# MASTER ChatGPT Art-Production Prompt — Jebby Jump

You are producing FINAL raster art files for the Unity game **Jebby Jump**
(commit context `355dba3`). You work from a validated audit package. Follow it
exactly; where it conflicts with your instincts, the package wins.

## Inputs you will be given
1. This prompt.
2. ONE batch brief from `batches/` (never work across batches in one session).
3. The per-asset briefs (`asset_briefs/ART-xxx.md`) named by that batch.
4. Uploaded locked reference images listed in `REFERENCE_UPLOAD_INDEX.md`.
5. `zip_blueprint/zip_manifest.json` + `tools/validate_generated_art.py`.

## Hard rules
1. Generate ONLY the asset IDs listed in the selected batch. No extras.
2. **Do not start any asset whose brief names a reference image that was not
   uploaded** — stop and ask for it instead.
3. Use image generation for the visual; use deterministic Python (Pillow) for
   EXACT canvas size, alpha handling, padding, safe-zone and tiling checks —
   never eyeball them. Each brief's §20 lists the required post-processing.
4. Never invent dimensions, filenames or paths: §5/§9 of each brief are exact.
   Filenames are case-sensitive; never "clean up" naming.
5. Never include `.meta` files or any Unity-native file (`.anim`,
   `.controller`, `.overrideController`, `.mat`, `.asset`, `.prefab`,
   `.unity`, `.spriteatlas`, TMP assets) in your output.
6. Never bake UI text into localisable art. The wordmark (ART-010) is the only
   lettering asset, and it is exactly "Jebby Jump".
7. Character identity is LOCKED: match the uploaded
   `jebby_color_knight_character_sheet_v01.png` — face, chibi proportions,
   brown/ivory ear-feather hair, blue cape, rainbow gem badge. No dog anatomy,
   no redesign, no style drift, no AI artefacts (extra fingers/limbs, melted
   shapes, noise).
8. Do NOT claim frame-to-frame character consistency or pixel compliance
   without checking: run the validator and report its actual output.
9. Store screenshots (ART-009) are REAL captures — refuse to generate them.

## Deliverable per batch
1. Files at their exact `archive_path`s (paths relative to the repository
   root, e.g. `Assets/_JebbyJump/Art/...`, `StoreAssets/...`, `Marketing/...`).
2. Run `tools/validate_generated_art.py --manifest zip_manifest.json --dir <outdir>`
   (subset validation for the batch is fine: `--ids ART-...`).
3. Build the ZIP with `tools/build_final_art_zip.py` ONLY if validation has no
   hard errors.
4. Return: the ZIP + `validation_report.md` + a per-asset acceptance table
   (brief §22 checkboxes, honestly filled) + a list of anything that FAILED
   or was skipped and why.

## What this batch does NOT do
Image generation here is **not Unity integration**. Wiring (icon slots, sprite
assignment, 9-slice borders, scene edits) happens later inside Unity per
`integration/UNITY_INTEGRATION_MANIFEST.md`. Say so in your final message so
nobody treats the ZIP as "integrated".
