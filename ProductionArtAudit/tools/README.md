# Audit Tools

All Python 3 + Pillow (stdlib otherwise). None of these write under `Assets/`.

| Script | Purpose |
|---|---|
| `measure_existing_art.py` | Regenerates `manifests/existing_art_inventory.csv/.json` (pixel measurement + .meta parse + GUID reference index). Read-only on the project. |
| `build_missing_art_manifest.py` | Single source of truth for the 17 gap rows → `manifests/missing_art_manifest.csv/.json`. |
| `build_asset_briefs.py` | Renders the 23-section briefs in `asset_briefs/` from the manifest + bespoke creative content. |
| `build_batch_files.py` | Renders `generation/batches/*.md`. |
| `build_zip_blueprint.py` | Renders `zip_blueprint/*` (incl. `zip_manifest.json`) from the manifest. |
| `validate_generated_art.py` | Validates a generated-art dir/ZIP against `zip_manifest.json` (sizes, alpha, corners, safe zones, tile seams, 9-slice flatness, forbidden files). Exit 1 on hard errors. |
| `build_final_art_zip.py` | Deterministic drop-in ZIP; refuses on validation errors; embeds manifest + report + README. |

Regeneration order after editing the gap set:
`build_missing_art_manifest.py` → `build_asset_briefs.py` → `build_batch_files.py` → `build_zip_blueprint.py`.

Fixture self-test (run from `ProductionArtAudit/`):
```
python tools/validate_generated_art.py --manifest zip_blueprint/zip_manifest.json --dir tools/fixtures/empty --out tools/fixtures/empty
python tools/validate_generated_art.py --manifest zip_blueprint/zip_manifest.json --dir tools/fixtures/invalid --ids ART-001 --out tools/fixtures/invalid
```
Both must FAIL with useful messages (missing files; wrong size/opaque corners).
