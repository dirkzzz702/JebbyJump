#!/usr/bin/env python3
"""Renders generation/batches/<NNN>_<name>.md from the batch plan data."""
import os

OUT = os.path.join(os.path.dirname(__file__), "..", "generation", "batches")

BATCHES = [
("001_store_identity", ["ART-001","ART-002","ART-003","ART-015"],
 ["jebby_color_knight_character_sheet_v01.png","jebby_outfit_variations_board_v01.png","bg_sky_layer_01.png"],
 "ONE consistent icon family. Generate ART-001 first; derive ART-002 subject from it; "
 "ART-003 is background-only; ART-015 is a DETERMINISTIC derivation of ART-002 (no fresh generation). "
 "Outputs are dependent (same family).",
 "Identity vs locked sheet + safe-zone/corner validator PASS + <=1024KB icon."),
("002_wordmark_brand", ["ART-010"],
 ["jebby_color_knight_character_sheet_v01.png"],
 "Standalone brand asset. Deliver PNG render AND vector (SVG) master. Output is independent.",
 "HUMAN BRAND/LEGAL REVIEW REQUIRED before any store use — deliver as CANDIDATE."),
("003_gameplay_floor", ["ART-005"],
 ["bg_sky_layer_01.png","Game__PlayingHUD__1686x948__none__level1-timer-running.png"],
 "Single asset. Design decision D1 is RESOLVED: taller cloud-meadow LEDGE (512x128, drawMode "
 "Tiled). Generate to the brief as written - no open decision.",
 "Tile-seam validator PASS (left/right column mean diff < 2/255)."),
("004_menu_background", ["ART-006"],
 ["bg_sky_layer_01.png","jebby_color_knight_character_sheet_v01.png","Game__PlayingHUD__1686x948__none__level1-timer-running.png"],
 "Single asset; must read as the SAME world as the uploaded gameplay sky.",
 "Centre-band luminance low enough for white TMP text (validator rule)."),
("005_ui_kit", ["ART-007","ART-008"],
 ["ui_btn_jump.png"],
 "ONE chrome family (panel + pill share rim colour + radius language). Outputs dependent. "
 "Corner-radius decision D2 has candidate values in the briefs — usable as-is; note the choice.",
 "Flat-stretch-region validator PASS on both; corner symmetry PASS."),
("006_skill_icons_vfx", ["ART-011","ART-012","ART-013","ART-014"],
 ["ui_btn_jump.png","spr_item_rocket_boots_01.png","ui_icon_life_01.png","jebby_color_knight_character_sheet_v01.png"],
 "Icons (011/012) share the ornate icon family; VFX (013/014) are clean shapes. ART-014 must be "
 "PURE WHITE/neutral (runtime tinting).",
 "70px readability check (downscale preview); ART-014 hue==neutral validator."),
("007_feature_keyart", ["ART-016","ART-004"],
 ["jebby_color_knight_character_sheet_v01.png","jebby_outfit_variations_board_v01.png","Game__PlayingHUD__1686x948__none__level1-timer-running.png"],
 "Generate the 3840x2160 key art (ART-016) FIRST, then compose/crop the 1024x500 feature "
 "graphic (ART-004) from it (dependent outputs). Feature graphic ships TEXT-FREE unless the "
 "approved ART-010 wordmark is supplied.",
 "Identity exact; ART-004 crop keeps Jebby's face; NO alpha in either."),
("008_store_screenshots", ["ART-009"],
 [],
 "NOT an image-generation batch. Real captures at 1920x1080 from the game AFTER batches "
 "003-006 are INTEGRATED in Unity. Only deterministic caption framing afterwards. If asked "
 "to fabricate gameplay images, refuse.",
 "Truthfulness: every frame is a real capture; 24-bit no alpha; min 4 frames."),
("009_hazard_variant", ["ART-017"],
 ["spr_cactus_obstacle_01.png"],
 "GATED: requires gameplay design decision D4 (anticipation behaviour). If unresolved, STOP. "
 "Silhouette must match the uploaded base within 4% bbox.",
 "Bbox-match validator PASS vs base cactus."),
]

TPL = """# Batch {name}

Read `../MASTER_CHATGPT_ART_PRODUCTION_PROMPT.md` first. Generate ONLY the
assets below, then validate, ZIP, and STOP.

## Assets (briefs are authoritative)
{assets}

## References to upload BEFORE generating (see ../REFERENCE_UPLOAD_INDEX.md)
{refs}
Generation without a listed reference is FORBIDDEN — ask for the upload.

## Batch notes / consistency
{notes}

## Output paths (exact; repo-root relative)
{outs}

## Acceptance gate for this batch
{gate}

## After generating
Run the validator (subset): `python tools/validate_generated_art.py --manifest
zip_blueprint/zip_manifest.json --dir <outdir> --ids {ids}` then build the ZIP
only on PASS. Report failures honestly. Do NOT mix Unity-native files into the
ZIP. STOP after this batch.
"""

# archive paths from the manifest
import json
MAN = os.path.join(os.path.dirname(__file__), "..", "manifests", "missing_art_manifest.json")
rows = {r["asset_id"]: r for r in json.load(open(MAN, encoding="utf-8"))}

os.makedirs(OUT, exist_ok=True)
for name, ids, refs, notes, gate in BATCHES:
    assets = "\n".join(f"- **{i}** — {rows[i]['title']} → `../../asset_briefs/{i}.md`" for i in ids)
    outs = "\n".join(f"- `{rows[i]['archive_path']}`" for i in ids)
    reflist = "\n".join(f"- {r}" for r in refs) if refs else "- (none — capture batch)"
    open(os.path.join(OUT, name + ".md"), "w", encoding="utf-8").write(
        TPL.format(name=name, assets=assets, refs=reflist, notes=notes,
                   outs=outs, gate=gate, ids=",".join(ids)))
print("batches:", len(BATCHES))
