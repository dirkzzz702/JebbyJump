#!/usr/bin/env python3
"""Read-only measurement pass for the JebbyJump Production Art Audit.

Walks Assets/ for image + font files, measures pixel data (Pillow), parses
sibling Unity .meta importer settings, counts scene/prefab/asset references
per GUID, and merges audit classifications (evidence-based judgments from the
live 2026-07-14 review session at commits 66eb4d0..355dba3).

Writes:
  ProductionArtAudit/manifests/existing_art_inventory.csv
  ProductionArtAudit/manifests/existing_art_inventory.json

Never writes anywhere under Assets/.
"""
import csv, hashlib, json, os, re, sys

REPO = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))
ASSETS = os.path.join(REPO, "Assets")
OUT_DIR = os.path.join(REPO, "ProductionArtAudit", "manifests")

IMAGE_EXTS = {".png", ".jpg", ".jpeg", ".webp", ".psd", ".psb", ".tif", ".tiff"}
FONT_EXTS = {".ttf", ".otf"}
YAML_REF_EXTS = {".unity", ".prefab", ".asset", ".controller", ".anim",
                 ".overrideController", ".mat", ".spriteatlas"}

try:
    from PIL import Image
except ImportError:
    print("Pillow required", file=sys.stderr); sys.exit(1)


def sha256(path):
    h = hashlib.sha256()
    with open(path, "rb") as f:
        for chunk in iter(lambda: f.read(1 << 20), b""):
            h.update(chunk)
    return h.hexdigest()


def parse_meta(meta_path):
    """Extract importer fields from a Unity .meta (flat regex parse)."""
    d = {}
    if not os.path.exists(meta_path):
        return d
    text = open(meta_path, encoding="utf-8", errors="replace").read()
    def grab(key):
        m = re.search(r"^\s*" + key + r":\s*(.+)$", text, re.M)
        return m.group(1).strip() if m else ""
    d["unity_guid"] = grab("guid")
    d["texture_type"] = grab("textureType")          # 8 = Sprite
    d["sprite_mode"] = grab("spriteMode")            # 1 = Single
    d["pixels_per_unit"] = grab("spritePixelsToUnits")
    # Effective pivot: Unity uses the alignment PRESET unless alignment==9
    # (Custom); the spritePivot field is stale for presets.
    align = grab("alignment")
    presets = {"0": "(0.5, 0.5)", "1": "(0, 1)", "2": "(0.5, 1)", "3": "(1, 1)",
               "4": "(0, 0.5)", "5": "(1, 0.5)", "6": "(0, 0)", "7": "(0.5, 0)",
               "8": "(1, 0)"}
    if align == "9":
        pivot = re.search(r"spritePivot:\s*\{x:\s*([\d.\-]+),\s*y:\s*([\d.\-]+)\}", text)
        d["pivot_normalized"] = f"({pivot.group(1)}, {pivot.group(2)})" if pivot else ""
    else:
        d["pivot_normalized"] = presets.get(align, "")
    d["mesh_type"] = grab("spriteMeshType")          # 0 FullRect / 1 Tight
    border = re.search(r"spriteBorder:\s*\{x:\s*([\d.\-]+),\s*y:\s*([\d.\-]+),\s*z:\s*([\d.\-]+),\s*w:\s*([\d.\-]+)\}", text)
    d["sprite_border"] = f"L{border.group(1)} B{border.group(2)} R{border.group(3)} T{border.group(4)}" if border else ""
    d["filter_mode"] = grab("filterMode")            # 0 Point 1 Bilinear 2 Trilinear
    d["wrap_mode"] = grab("wrapU")
    d["mipmaps"] = grab("enableMipMap")
    d["alpha_is_transparency"] = grab("alphaIsTransparency")
    d["sRGB"] = grab("sRGBTexture")
    d["max_texture_size"] = grab("maxTextureSize")
    d["compression"] = grab("textureCompression")
    d["platform_overrides"] = "yes" if re.search(r"- serializedVersion: \d+\s*\n\s*buildTarget: (Android|iPhone)", text) else "none-detected"
    d["importer_type"] = "TextureImporter" if "TextureImporter" in text else ("TrueTypeFontImporter" if "TrueTypeFontImporter" in text else "other")
    return d


def measure_image(path):
    d = {}
    try:
        im = Image.open(path)
        d["width_px"], d["height_px"] = im.size
        d["colour_mode"] = im.mode
        d["bit_depth"] = {"1": 1, "L": 8, "P": 8, "RGB": 24, "RGBA": 32, "LA": 16}.get(im.mode, "")
        has_alpha = im.mode in ("RGBA", "LA", "PA") or "transparency" in im.info
        d["has_alpha"] = has_alpha
        if has_alpha:
            a = im.convert("RGBA").getchannel("A")
            w, h = im.size
            corners = [a.getpixel((0, 0)), a.getpixel((w - 1, 0)),
                       a.getpixel((0, h - 1)), a.getpixel((w - 1, h - 1))]
            d["transparent_corner_result"] = "PASS" if all(c == 0 for c in corners) else "FAIL(" + ",".join(str(c) for c in corners) + ")"
            bbox = a.getbbox()  # (l, t, r, b) of non-zero alpha
            if bbox:
                l, t, r, b = bbox
                d["nontransparent_bounds_px"] = f"x{l} y{t} w{r - l} h{b - t}"
                d["transparent_padding_left_px"] = l
                d["transparent_padding_right_px"] = w - r
                d["transparent_padding_top_px"] = t
                d["transparent_padding_bottom_px"] = h - b
                extrema = a.getextrema()
                d["alpha_usage"] = "binary" if extrema in ((0, 255),) and len(a.getcolors(257) or [0]*300) <= 2 else "soft"
            else:
                d["nontransparent_bounds_px"] = "EMPTY"
        else:
            d["transparent_corner_result"] = "N/A(opaque)"
            d["alpha_usage"] = "none"
    except Exception as e:
        d["measure_error"] = str(e)
    return d


def build_reference_index():
    """guid -> list of repo-relative YAML files referencing it (single pass)."""
    guid_files = {}
    yaml_files = []
    for root, dirs, files in os.walk(ASSETS):
        dirs[:] = [x for x in dirs if x not in (".git",)]
        for fn in files:
            if os.path.splitext(fn)[1] in YAML_REF_EXTS:
                yaml_files.append(os.path.join(root, fn))
    # ProjectSettings too (icons/splash references)
    ps = os.path.join(REPO, "ProjectSettings")
    for fn in os.listdir(ps):
        if fn.endswith(".asset"):
            yaml_files.append(os.path.join(ps, fn))
    corpus = []
    for yf in yaml_files:
        try:
            corpus.append((os.path.relpath(yf, REPO).replace("\\", "/"),
                           open(yf, encoding="utf-8", errors="replace").read()))
        except OSError:
            pass
    return corpus


# ---- Audit classifications (evidence from the 2026-07-14 live review) ----
# Keys are repo-relative path prefixes (forward slashes). First match wins.
OVERRIDES = [
    ("Assets/_JebbyJump/Art/Characters/Jebby/Outfits/RookiePage/", dict(
        category="character", subcategory="default-look source (RookiePage set)",
        visual_status="PROTOTYPE", production_status="LAUNCH_REQUIRED-as-is; final-art certification pending",
        style_consistency="matches locked reference (palette copy of Classic)",
        licence_status="user-supplied AI palette transfer; provenance/licence record MISSING",
        source_file_status="no editable source in repo",
        recommended_action="final-art certification pass; record provenance/licence",
        evidence="Roadmap P13B PROTOTYPE note; RUNTIME-OBSERVED in gameplay (review shots); default-look pointer since 355dba3",
        runtime_observed="yes", confidence="high")),
    ("Assets/_JebbyJump/Art/Characters/Jebby/Outfits/", dict(
        category="character", subcategory="outfit variant set",
        visual_status="PROTOTYPE", production_status="launch-usable; not final-art-certified",
        style_consistency="palette transfer of locked default poses; per-outfit theming minimal",
        licence_status="user-supplied AI palette transfer; provenance/licence record MISSING",
        source_file_status="no editable source in repo",
        recommended_action="per-outfit rendered QA + final-art certification; record provenance",
        evidence="Roadmap P13B PROTOTYPE note; pixel-diff vs default 2.5-9.4/255 (FILE-MEASURED 2026-07-14)",
        runtime_observed="wardrobe thumbnails/preview only", confidence="high")),
    ("Assets/_JebbyJump/Art/Sprites/Backgrounds/bg_sky_layer_01.png", dict(
        category="background", subcategory="gameplay sky",
        visual_status="PRODUCTION_ACCEPTABLE", production_status="launch-usable single layer",
        style_consistency="matches Art Bible (soft pastel fantasy) - RUNTIME-OBSERVED",
        licence_status="provenance record MISSING", source_file_status="no source",
        recommended_action="keep for launch; parallax layers are post-launch polish",
        evidence="RUNTIME-OBSERVED review screenshots (Game scene)", runtime_observed="yes", confidence="high")),
    ("Assets/_JebbyJump/Art/Sprites/Items/spr_item_rocket_boots_01.png", dict(
        category="item/skill", subcategory="Rocket Boots skill icon source",
        visual_status="PRODUCTION_ACCEPTABLE", production_status="used as HUD skill icon",
        style_consistency="ornate style matches control buttons",
        licence_status="provenance record MISSING", source_file_status="no source",
        recommended_action="TECHNICAL_REEXPORT: 1122x1402 source displayed ~100px; add max-size platform override or downsized icon",
        evidence="RUNTIME-OBSERVED as Btn_Skill1 icon (review shots); size FILE-MEASURED",
        runtime_observed="yes", confidence="high")),
    ("Assets/_JebbyJump/Art/Sprites/Obstacles/spr_cactus_obstacle_01.png", dict(
        category="obstacle", subcategory="cactus hazard",
        visual_status="PRODUCTION_ACCEPTABLE", production_status="launch hazard art",
        style_consistency="not runtime-observed this review (L1 spawns no cactus)",
        licence_status="provenance record MISSING", source_file_status="no source",
        recommended_action="rendered QA on a cactus level; consider contact/warning variant (recommended, not required)",
        evidence="DEPENDENCY-VERIFIED via spawner; NOT runtime-observed", runtime_observed="no", confidence="medium")),
    ("Assets/_JebbyJump/Art/Sprites/Platforms/spr_platform_base_01.png", dict(
        category="platform", subcategory="base pill (runtime-tinted per color)",
        visual_status="PRODUCTION_ACCEPTABLE", production_status="single base sprite tinted per PlatformColor",
        style_consistency="simple rounded pill; reads clearly (RUNTIME-OBSERVED)",
        licence_status="n/a simple shape", source_file_status="no source",
        recommended_action="optional style upgrade (texture/edge highlight) - LAUNCH_RECOMMENDED polish only",
        evidence="RUNTIME-OBSERVED review screenshots; tint via SpriteRenderer.color (STATIC-INSPECTED)",
        runtime_observed="yes", confidence="high")),
    ("Assets/_JebbyJump/Art/Sprites/UI/", dict(
        category="ui", subcategory="mobile control / HUD sprite",
        visual_status="PRODUCTION_ACCEPTABLE", production_status="launch-usable",
        style_consistency="ornate gold/blue controls match Art Bible premium bar (RUNTIME-OBSERVED)",
        licence_status="provenance record MISSING", source_file_status="no source",
        recommended_action="TECHNICAL_REEXPORT: ~1MB 1122px sources displayed at 100-160px; add max-size overrides/atlas",
        evidence="RUNTIME-OBSERVED review screenshots (HUD)", runtime_observed="yes", confidence="high")),
    ("Assets/_JebbyJump/Art/Sprites/Placeholder.png", dict(
        category="placeholder", subcategory="scaffold placeholder",
        visual_status="PLACEHOLDER", production_status="not production",
        style_consistency="n/a", licence_status="n/a", source_file_status="n/a",
        recommended_action="delete once confirmed unreferenced",
        evidence="explicit placeholder name; near-zero size", runtime_observed="no", confidence="high")),
    ("Assets/_JebbyJump/Docs/Art/References/", dict(
        category="reference", subcategory="locked visual reference (source of truth)",
        visual_status="FINAL_PRODUCTION", production_status="SOURCE_ONLY (never shipped; unreferenced by scenes)",
        style_consistency="IS the style authority", licence_status="provenance record MISSING (AI-generated boards)",
        source_file_status="the boards ARE the source", recommended_action="keep locked; record provenance",
        evidence="Art Bible section 2 locked references", runtime_observed="no", confidence="high")),
    ("Assets/TextMesh Pro/Fonts/LiberationSans.ttf", dict(
        category="typography", subcategory="TMP default font",
        visual_status="PRODUCTION_ACCEPTABLE", production_status="functional; generic",
        style_consistency="generic sans; below premium-indie wordmark bar for title",
        licence_status="SIL OFL (licence file present alongside)", source_file_status="ttf is source",
        recommended_action="keep for body/numerals; add display/wordmark treatment (see missing-art manifest)",
        evidence="LiberationSans - OFL.txt sibling; digits verified by release preflight",
        runtime_observed="yes", confidence="high")),
]


def classify(rel):
    for prefix, data in OVERRIDES:
        if rel.startswith(prefix) or rel == prefix:
            return dict(data)
    return dict(category="", subcategory="", visual_status="UNKNOWN",
                production_status="", style_consistency="", licence_status="",
                source_file_status="", recommended_action="", evidence="",
                runtime_observed="", confidence="low")


FIELDS = ["asset_id", "current_path", "filename", "extension", "category",
          "subcategory", "file_size_bytes", "sha256", "width_px", "height_px",
          "colour_mode", "bit_depth", "has_alpha", "alpha_usage",
          "transparent_corner_result", "nontransparent_bounds_px",
          "transparent_padding_left_px", "transparent_padding_right_px",
          "transparent_padding_top_px", "transparent_padding_bottom_px",
          "unity_guid", "importer_type", "texture_type", "sprite_mode",
          "pixels_per_unit", "pivot_normalized", "pivot_pixels", "mesh_type",
          "sprite_border", "filter_mode", "wrap_mode", "mipmaps",
          "alpha_is_transparency", "sRGB", "max_texture_size", "compression",
          "platform_overrides", "atlas_membership", "referenced_by_count",
          "referenced_by", "runtime_observed", "visual_status",
          "production_status", "style_consistency", "licence_status",
          "source_file_status", "recommended_action", "evidence", "confidence"]


def main():
    corpus = build_reference_index()
    rows = []
    idx = 0
    for root, dirs, files in os.walk(ASSETS):
        for fn in sorted(files):
            ext = os.path.splitext(fn)[1].lower()
            if ext not in IMAGE_EXTS | FONT_EXTS:
                continue
            full = os.path.join(root, fn)
            rel = os.path.relpath(full, REPO).replace("\\", "/")
            idx += 1
            row = {k: "" for k in FIELDS}
            row["asset_id"] = f"EXIST-{idx:03d}"
            row["current_path"] = rel
            row["filename"] = fn
            row["extension"] = ext
            row["file_size_bytes"] = os.path.getsize(full)
            row["sha256"] = sha256(full)
            if ext in IMAGE_EXTS:
                row.update({k: v for k, v in measure_image(full).items() if k in row})
            row.update({k: v for k, v in parse_meta(full + ".meta").items() if k in row})
            # pivot pixels (from normalized x size), when both known
            try:
                if row["pivot_normalized"] and row["width_px"]:
                    px, py = [float(x) for x in row["pivot_normalized"].strip("()").split(",")]
                    row["pivot_pixels"] = f"({px * row['width_px']:.0f}, {py * row['height_px']:.0f})"
            except Exception:
                pass
            row["atlas_membership"] = "none (no SpriteAtlas assets in project)"
            guid = row["unity_guid"]
            refs = [p for p, text in corpus if guid and guid in text] if guid else []
            row["referenced_by_count"] = len(refs)
            row["referenced_by"] = ";".join(refs[:12]) + ("..." if len(refs) > 12 else "")
            row.update(classify(rel))
            rows.append(row)

    os.makedirs(OUT_DIR, exist_ok=True)
    with open(os.path.join(OUT_DIR, "existing_art_inventory.csv"), "w",
              newline="", encoding="utf-8") as f:
        w = csv.DictWriter(f, fieldnames=FIELDS)
        w.writeheader()
        w.writerows(rows)
    with open(os.path.join(OUT_DIR, "existing_art_inventory.json"), "w",
              encoding="utf-8") as f:
        json.dump(rows, f, indent=1)
    print(f"inventory: {len(rows)} assets")
    unref = [r["current_path"] for r in rows if r["referenced_by_count"] == 0]
    print("unreferenced:", len(unref))
    for u in unref:
        print("  -", u)


if __name__ == "__main__":
    main()
