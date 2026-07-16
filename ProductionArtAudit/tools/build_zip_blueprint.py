#!/usr/bin/env python3
"""Renders zip_blueprint/* from missing_art_manifest.json (single source)."""
import csv, json, os

HERE = os.path.dirname(__file__)
MAN = os.path.join(HERE, "..", "manifests", "missing_art_manifest.json")
OUT = os.path.join(HERE, "..", "zip_blueprint")
rows = json.load(open(MAN, encoding="utf-8"))

# Expand multi-file assets
FILES = []
for r in rows:
    if r["asset_id"] == "ART-009":
        for i, nm in enumerate(["shot_01_gameplay.png","shot_02_memory.png",
                                "shot_03_wardrobe.png","shot_04_levelselect.png"], 1):
            FILES.append(dict(r, archive_path="StoreAssets/Screenshots/"+nm,
                              target_filename=nm, _w=1920, _h=1080, _alpha=False))
    elif r["asset_id"] == "ART-010":
        FILES.append(dict(r, _w=1200, _h=400, _alpha=True))
        FILES.append(dict(r, archive_path="Marketing/Brand/jebby_jump_wordmark_master.svg",
                          target_filename="jebby_jump_wordmark_master.svg",
                          artifact_format="SVG", _w=None, _h=None, _alpha=None))
    else:
        FILES.append(dict(r, _w=r["runtime_width_px"], _h=r["runtime_height_px"],
                          _alpha=("NO" not in str(r["alpha_requirement"]).upper()
                                  and "OPAQUE" not in str(r["alpha_requirement"]).upper())))

os.makedirs(OUT, exist_ok=True)

# ZIP_TREE.txt
tree = {}
for f in FILES:
    tree.setdefault(os.path.dirname(f["archive_path"]), []).append(os.path.basename(f["archive_path"]))
with open(os.path.join(OUT, "ZIP_TREE.txt"), "w", encoding="utf-8") as fh:
    fh.write("ZIP extract root = repository root (no enclosing folder)\n\n")
    for d in sorted(tree):
        fh.write(d + "/\n")
        for n in sorted(tree[d]):
            fh.write("    " + n + "\n")

# exact_target_paths.csv
with open(os.path.join(OUT, "exact_target_paths.csv"), "w", newline="", encoding="utf-8") as fh:
    w = csv.writer(fh); w.writerow(["asset_id","archive_path","case_sensitive_filename","blocked_on_decision"])
    for f in FILES:
        w.writerow([f["asset_id"], f["archive_path"], os.path.basename(f["archive_path"]),
                    f.get("blocker","") or ""])

# preserve_existing_meta.csv — no drop-in replacements exist in this gap set
with open(os.path.join(OUT, "preserve_existing_meta.csv"), "w", newline="", encoding="utf-8") as fh:
    w = csv.writer(fh); w.writerow(["asset_id","existing_path","existing_guid","rule"])
    w.writerow(["(none)","","","No DROP_IN_REPLACEMENT rows in this manifest: every gap is a "
                "NEW asset or store/marketing file. If a future batch replaces an existing "
                "sprite (e.g. outfit re-cert), copy the image OVER the file and NEVER touch its .meta."])

# requires_unity_wiring.csv
with open(os.path.join(OUT, "requires_unity_wiring.csv"), "w", newline="", encoding="utf-8") as fh:
    w = csv.writer(fh); w.writerow(["asset_id","archive_path","wiring"])
    for f in FILES:
        if str(f.get("requires_unity_wiring","")).lower() == "yes":
            w.writerow([f["asset_id"], f["archive_path"], f.get("unity_wiring_steps","")])

# zip_manifest.json
manifest = {
  "schema_version": "1.0",
  "project": "JebbyJump",
  "project_commit": "355dba3",
  "zip_extract_root": "repository_root",
  "files": [
    {
      "asset_id": f["asset_id"],
      "archive_path": f["archive_path"],
      "filename": os.path.basename(f["archive_path"]),
      "format": ("SVG" if f["archive_path"].endswith(".svg") else "PNG"),
      "width_px": f["_w"], "height_px": f["_h"],
      "colour_mode": (None if f["_w"] is None else ("RGBA" if f["_alpha"] else "RGB")),
      "has_alpha": f["_alpha"],
      "preserve_existing_meta": False,
      "meta_action": "OMIT_FROM_ZIP",
      "existing_guid": "",
      "sha256_expected": None,
      "requires_unity_wiring": str(f.get("requires_unity_wiring","")).lower() == "yes",
      "blocked_on_decision": bool(f.get("blocker")) and f["asset_id"] in ("ART-007","ART-008","ART-010","ART-017"),
      "acceptance_profile": f["asset_id"],
    } for f in FILES
  ],
  "excluded_patterns": ["*.meta","*.unity","*.prefab","*.anim","*.controller",
                        "*.overrideController","*.asset","*.spriteatlas","*.mat"],
}
json.dump(manifest, open(os.path.join(OUT, "zip_manifest.json"), "w", encoding="utf-8"), indent=1)

# EXCLUDED_FILES.md
open(os.path.join(OUT, "EXCLUDED_FILES.md"), "w", encoding="utf-8").write(
"""# Excluded From The Final-Art ZIP

- `*.meta` — ALWAYS. New assets get metas from Unity on import; replacements keep theirs.
- Unity-native: `*.anim`, `*.controller`, `*.overrideController`, `*.mat`, `*.asset`,
  `*.prefab`, `*.unity`, `*.spriteatlas`, TMP font assets — created only inside Unity
  (see `integration/unity_native_assets_required.csv`).
- Fonts (`*.ttf/*.otf`) — never AI-generated; licence-gated.
- Editable masters other than the wordmark SVG (PSD/AI) — delivered separately to
  `Marketing/Sources/` at the artist's discretion, not in the drop-in ZIP.
- Anything not listed in `zip_manifest.json` — the validator rejects unexpected files.
""")
print("zip blueprint files:", 6, "| files in manifest:", len(FILES))
