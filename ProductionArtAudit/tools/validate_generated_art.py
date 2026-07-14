#!/usr/bin/env python3
"""Validates a generated-art directory (or ZIP) against zip_manifest.json.

Usage:
  python validate_generated_art.py --manifest <zip_manifest.json> --dir <artdir>
      [--ids ART-001,ART-002] [--out <reportdir>]
  (--dir may also be a .zip file)

Checks (hard errors unless noted):
  - every required file exists (subset via --ids); none zero-byte
  - no duplicate paths differing only by case
  - no .meta / Unity-native / forbidden files anywhere
  - exact archive path + filename (case-sensitive)
  - exact pixel width/height; expected colour mode; alpha presence/absence
  - transparent corners where has_alpha (warning->error if fully opaque corners)
  - per-asset extra rules: safe-zone band (ART-002/015), tile seam (ART-005),
    flat 9-slice stretch bands (ART-007/008), neutral ring (ART-014),
    centre-band luminance (ART-006)
  - checkerboard-background heuristic (warning)
  - unexpected files not in manifest (error unless allowlisted)
Outputs validation_report.json + validation_report.md. Exit 1 on hard errors.
Subjective style approval is NEVER automatic (see qa/VISUAL_ACCEPTANCE_CHECKLIST.md).
"""
import argparse, json, os, sys, tempfile, zipfile

try:
    from PIL import Image
except ImportError:
    print("Pillow required (pip install Pillow)", file=sys.stderr)
    sys.exit(2)

FORBIDDEN_SUFFIXES = (".meta", ".unity", ".prefab", ".anim", ".controller",
                      ".overridecontroller", ".asset", ".spriteatlas", ".mat",
                      ".ttf", ".otf")
ALLOWED_EXTRA = {"zip_manifest.json", "validation_report.json",
                 "validation_report.md", "ART_PACKAGE_README.md"}


def load_dir(path):
    if path.lower().endswith(".zip"):
        tmp = tempfile.mkdtemp(prefix="artval_")
        with zipfile.ZipFile(path) as z:
            z.extractall(tmp)
        return tmp
    return path


def walk_files(root):
    for r, _, files in os.walk(root):
        for fn in files:
            full = os.path.join(r, fn)
            yield os.path.relpath(full, root).replace("\\", "/"), full


def corner_alphas(im):
    a = im.convert("RGBA").getchannel("A")
    w, h = im.size
    return [a.getpixel(p) for p in ((0, 0), (w-1, 0), (0, h-1), (w-1, h-1))]


def band_uniform(im, horizontal, err):
    """Middle stretch band of a 9-slice must be flat colour."""
    px = im.convert("RGBA")
    w, h = im.size
    if horizontal:
        y = h // 2
        colors = {px.getpixel((x, y)) for x in range(w // 3, 2 * w // 3)}
    else:
        x = w // 2
        colors = {px.getpixel((x, y)) for y in range(h // 3, 2 * h // 3)}
    if len(colors) > 3:
        err(f"9-slice stretch band not flat ({len(colors)} distinct colours)")


def check_asset(rel, full, spec, errors, warnings):
    def err(m): errors.append(f"{rel}: {m}")
    def warn(m): warnings.append(f"{rel}: {m}")
    if os.path.getsize(full) == 0:
        err("zero-byte file"); return
    if spec["format"] == "SVG":
        head = open(full, "rb").read(512)
        if b"<svg" not in head:
            err("not an SVG document")
        return
    try:
        im = Image.open(full)
    except Exception as e:
        err(f"unreadable image: {e}"); return
    if spec["width_px"] and im.size != (spec["width_px"], spec["height_px"]):
        err(f"size {im.size[0]}x{im.size[1]} != required {spec['width_px']}x{spec['height_px']}")
    has_alpha = im.mode in ("RGBA", "LA", "PA")
    if spec["has_alpha"] is True and not has_alpha:
        err("alpha channel required but missing")
    if spec["has_alpha"] is False and has_alpha:
        a = im.getchannel("A").getextrema()
        if a != (255, 255):
            err("must be fully opaque (alpha varies)")
    # ART-001 is deliberately full-bleed (Play applies its own corner mask),
    # so opaque corners are CORRECT there despite the RGBA requirement.
    if spec["has_alpha"] and spec["asset_id"] != "ART-001":
        cs = corner_alphas(im)
        if all(c == 255 for c in cs):
            err(f"all four corners opaque (expected transparent corners): {cs}")
        # checkerboard heuristic: repeating 8/16px light-grey grid
        rgb = im.convert("RGB")
        p1, p2 = rgb.getpixel((4, 4)), rgb.getpixel((12, 12))
        if p1 != p2 and {p1, p2} <= {(255, 255, 255), (204, 204, 204), (191, 191, 191)}:
            warn("checkerboard-like corner pattern - verify no baked transparency grid")
    aid = spec["asset_id"]
    if aid in ("ART-002", "ART-015"):
        a = im.convert("RGBA").getchannel("A")
        w, h = im.size
        band = int(w * 72 / 432)
        bad = 0
        for x in range(0, w, 8):
            for y in list(range(0, band, 4)) + list(range(h - band, h, 4)):
                if a.getpixel((x, y)) > 8: bad += 1
        for y in range(0, h, 8):
            for x in list(range(0, band, 4)) + list(range(w - band, w, 4)):
                if a.getpixel((x, y)) > 8: bad += 1
        if bad:
            err(f"adaptive-icon mask band not transparent ({bad} samples >8 alpha)")
    if aid == "ART-005":
        rgb = im.convert("RGBA")
        w, h = im.size
        diff = 0
        for y in range(h):
            l, r = rgb.getpixel((0, y)), rgb.getpixel((w - 1, y))
            diff += sum(abs(l[i] - r[i]) for i in range(4))
        if diff / (h * 4) > 2:
            err(f"tile seam: left/right edge mean diff {diff/(h*4):.2f} > 2")
    if aid == "ART-007":
        band_uniform(im, True, err); band_uniform(im, False, err)
    if aid == "ART-008":
        band_uniform(im, True, err)
    if aid == "ART-014":
        rgb = im.convert("RGBA")
        w, h = im.size
        for x in range(0, w, 16):
            for y in range(0, h, 16):
                r, g, b, a = rgb.getpixel((x, y))
                if a > 16 and (max(r, g, b) - min(r, g, b)) > 12:
                    err("ring must be neutral white/grey (found coloured pixel)"); return
    if aid == "ART-006":
        g = im.convert("L")
        w, h = im.size
        vals = [g.getpixel((x, y)) for x in range(int(w*0.3), int(w*0.7), 24)
                for y in range(int(h*0.25), int(h*0.75), 24)]
        if sum(vals) / len(vals) > 200:
            warn("centre band very bright - verify white menu text stays readable")


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--manifest", required=True)
    ap.add_argument("--dir", required=True)
    ap.add_argument("--ids", default="")
    ap.add_argument("--out", default=".")
    args = ap.parse_args()

    man = json.load(open(args.manifest, encoding="utf-8"))
    want_ids = {i.strip() for i in args.ids.split(",") if i.strip()}
    specs = [f for f in man["files"] if not want_ids or f["asset_id"] in want_ids]
    root = load_dir(args.dir)

    present = dict(walk_files(root))
    errors, warnings = [], []

    lower_seen = {}
    for rel in present:
        low = rel.lower()
        if low in lower_seen and lower_seen[low] != rel:
            errors.append(f"case-duplicate paths: {rel} vs {lower_seen[low]}")
        lower_seen[low] = rel
        if rel.lower().endswith(FORBIDDEN_SUFFIXES):
            errors.append(f"forbidden file type in package: {rel}")

    expected = {s["archive_path"] for s in specs}
    checked = 0
    for s in specs:
        rel = s["archive_path"]
        if rel not in present:
            if s.get("blocked_on_decision"):
                warnings.append(f"{rel}: missing (asset is decision-blocked - allowed)")
            else:
                errors.append(f"missing required file: {rel}")
            continue
        check_asset(rel, present[rel], s, errors, warnings)
        checked += 1

    all_expected = {f["archive_path"] for f in man["files"]}
    for rel in present:
        base = os.path.basename(rel)
        if rel not in all_expected and base not in ALLOWED_EXTRA:
            errors.append(f"unexpected file not in manifest: {rel}")

    report = {"manifest": os.path.abspath(args.manifest), "dir": os.path.abspath(args.dir),
              "assets_checked": checked, "required": len(specs),
              "errors": errors, "warnings": warnings,
              "result": "PASS" if not errors else "FAIL"}
    os.makedirs(args.out, exist_ok=True)
    json.dump(report, open(os.path.join(args.out, "validation_report.json"), "w",
              encoding="utf-8"), indent=1)
    with open(os.path.join(args.out, "validation_report.md"), "w", encoding="utf-8") as f:
        f.write(f"# Validation: {report['result']}\n\nchecked {checked}/{len(specs)} required files\n\n")
        f.write("## Errors\n" + ("\n".join("- " + e for e in errors) or "none") + "\n")
        f.write("\n## Warnings\n" + ("\n".join("- " + w for w in warnings) or "none") + "\n")
        f.write("\nStyle approval is HUMAN: see qa/VISUAL_ACCEPTANCE_CHECKLIST.md\n")
    print(report["result"], f"({len(errors)} errors, {len(warnings)} warnings)")
    for e in errors[:20]:
        print("  ERROR:", e)
    sys.exit(0 if not errors else 1)


if __name__ == "__main__":
    main()
