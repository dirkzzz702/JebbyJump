#!/usr/bin/env python3
"""Builds the final drop-in art ZIP from a VALIDATED directory.

Usage:
  python build_final_art_zip.py --manifest <zip_manifest.json> --dir <artdir>
      --out <jebby_art_package.zip> [--ids ART-...]

Refuses to build when validation reports hard errors (runs the validator
internally). Uses exact archive paths (repo-root relative), omits .meta and
Unity-native files, produces a deterministic ZIP (sorted entries, fixed
timestamps), and embeds zip_manifest.json + validation_report.json +
ART_PACKAGE_README.md. Never touches the Unity project.
"""
import argparse, json, os, subprocess, sys, zipfile

HERE = os.path.dirname(os.path.abspath(__file__))

README = """# Jebby Jump Final Art Package

Extract at the REPOSITORY ROOT (paths are repo-root relative; no enclosing
folder). Files under StoreAssets/ and Marketing/ are never imported by Unity.
Files under Assets/ still require the Unity wiring listed in
ProductionArtAudit/integration/ (this ZIP is NOT integration).

NEVER delete or replace an existing .meta file. This ZIP intentionally
contains no .meta and no Unity-native assets.
"""

def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--manifest", required=True)
    ap.add_argument("--dir", required=True)
    ap.add_argument("--out", required=True)
    ap.add_argument("--ids", default="")
    args = ap.parse_args()

    cmd = [sys.executable, os.path.join(HERE, "validate_generated_art.py"),
           "--manifest", args.manifest, "--dir", args.dir, "--out", args.dir]
    if args.ids:
        cmd += ["--ids", args.ids]
    if subprocess.call(cmd) != 0:
        print("REFUSED: validation has hard errors; fix them first.")
        sys.exit(1)

    man = json.load(open(args.manifest, encoding="utf-8"))
    want = {i.strip() for i in args.ids.split(",") if i.strip()}
    entries = []
    for f in man["files"]:
        if want and f["asset_id"] not in want:
            continue
        src = os.path.join(args.dir, f["archive_path"])
        if os.path.exists(src):
            entries.append((f["archive_path"], src))
    entries.sort()

    with zipfile.ZipFile(args.out, "w", zipfile.ZIP_DEFLATED) as z:
        for arc, src in entries:
            zi = zipfile.ZipInfo(arc, date_time=(2026, 1, 1, 0, 0, 0))
            zi.compress_type = zipfile.ZIP_DEFLATED
            z.writestr(zi, open(src, "rb").read())
        z.writestr(zipfile.ZipInfo("zip_manifest.json", (2026, 1, 1, 0, 0, 0)),
                   json.dumps(man, indent=1))
        vr = os.path.join(args.dir, "validation_report.json")
        if os.path.exists(vr):
            z.writestr(zipfile.ZipInfo("validation_report.json", (2026, 1, 1, 0, 0, 0)),
                       open(vr, "rb").read())
        z.writestr(zipfile.ZipInfo("ART_PACKAGE_README.md", (2026, 1, 1, 0, 0, 0)), README)
    print(f"built {args.out} with {len(entries)} art files")


if __name__ == "__main__":
    main()
