# Excluded From The Final-Art ZIP

- `*.meta` — ALWAYS. New assets get metas from Unity on import; replacements keep theirs.
- Unity-native: `*.anim`, `*.controller`, `*.overrideController`, `*.mat`, `*.asset`,
  `*.prefab`, `*.unity`, `*.spriteatlas`, TMP font assets — created only inside Unity
  (see `integration/unity_native_assets_required.csv`).
- Fonts (`*.ttf/*.otf`) — never AI-generated; licence-gated.
- Editable masters other than the wordmark SVG (PSD/AI) — delivered separately to
  `Marketing/Sources/` at the artist's discretion, not in the drop-in ZIP.
- Anything not listed in `zip_manifest.json` — the validator rejects unexpected files.
