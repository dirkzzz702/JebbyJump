# Pixel Acceptance Checklist (run per generated file; validator automates most)

- [ ] Canvas exactly matches brief §9 (validator: width/height).
- [ ] Alpha mode matches brief (RGBA vs opaque; validator: colour_mode/has_alpha).
- [ ] Four corners fully transparent where alpha applies (validator).
- [ ] No matte/white halo at silhouette edges (zoom 400% on edges).
- [ ] No unintended cropping: subject bbox inside brief §10 bounds (validator where specified).
- [ ] Exact padding/safe-zone rules met (ART-002/015: outer 72px band alpha 0 — validator).
- [ ] Pivot/contact assumptions hold (ART-005: content reaches top edge).
- [ ] 9-slice assets: stretch regions perfectly flat (validator uniform-band check).
- [ ] Tileable assets: seam check passed (ART-005 validator rule).
- [ ] No baked text in localisable sprites (only ART-010 carries lettering: exactly "Jebby Jump").
- [ ] Colour mode/bit depth per brief; sRGB.
- [ ] Extension exactly `.png` lowercase (`.svg` for the wordmark master).
- [ ] No checkerboard pattern, no zero-byte file (validator).
