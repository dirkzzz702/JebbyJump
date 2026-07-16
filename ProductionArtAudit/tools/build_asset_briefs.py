#!/usr/bin/env python3
"""Renders the 23-section per-asset production briefs from
missing_art_manifest.json + the bespoke creative content below.
Keeps numeric specs single-sourced in the manifest."""
import json, os

HERE = os.path.dirname(__file__)
MAN = os.path.join(HERE, "..", "manifests", "missing_art_manifest.json")
OUT = os.path.join(HERE, "..", "asset_briefs")

STYLE_CORE = ("Soft storybook fantasy, bright and warm, child-safe, premium indie "
"quality. Clean shapes, gentle outlines, soft shading, restrained magical sparkle. "
"NO gritty/scary fantasy, NO hyper-casual ad-style gloss, NO photo-realism, "
"NO AI noise/artefacts.")

JEBBY_ID = ("Jebby the Color Knight: chibi HUMANOID knight (not a literal dog), "
"warm large brown eyes, brown-and-ivory ear-feather hair, small blue cape, brown "
"boots, rainbow gem badge on the chest strap. Match the uploaded character sheet "
"EXACTLY - same face, proportions, palette.")

B = {
"ART-001": dict(
 purpose="Google Play store listing icon - the game's primary brand mark in the store.",
 refs=["Docs/Art/References/jebby_color_knight_character_sheet_v01.png (identity - REQUIRED upload)",
       "Docs/Art/References/jebby_outfit_variations_board_v01.png (palette)"],
 contract=("Jebby's head/bust, centred, joyful expression, rainbow gem badge visible. "
   "Soft sky-gradient backdrop. Composition survives Play's circular mask (keep "
   "critical detail inside a centred circle of ~85% width). " + STYLE_CORE),
 prompt=("Create a 1024x1024 app icon (will be downscaled to exactly 512x512). Subject: "
   + JEBBY_ID + " Head-and-shoulders bust, centred, facing slightly left, joyful smile. "
   "Background: soft vertical pastel sky gradient (light blue to warm cream) filling the "
   "full canvas edge-to-edge (opaque). Keep Jebby's full silhouette inside the central 85% "
   "circle. Rounded storybook rendering, crisp edges at small sizes. " + STYLE_CORE +
   " Output: PNG, sRGB. Filename: jebby_jump_icon_512.png"),
 negative=("No text or lettering. No border/frame. No drop shadow outside canvas. No white "
   "halo. No checkerboard. No extra characters/objects. No redesign of Jebby's face, hair "
   "silhouette, cape colour, or gem badge. No dog snout/anatomy."),
 post=("Downscale master to exactly 512x512 (Lanczos), sRGB, 32-bit RGBA, verify <=1024KB, "
   "verify still readable at 48x48 preview."),
 wiring="None (store upload asset; lives in StoreAssets/, never imported by Unity).",
 accept=["512x512 exact; RGBA; <=1024KB", "identity matches locked sheet", "readable at 48px",
   "no text/halo/checkerboard"],
 blockers="None."),
"ART-002": dict(
 purpose="Android adaptive launcher icon FOREGROUND layer (all PlayerSettings icon slots are empty today - the app installs with the default Unity icon).",
 refs=["Docs/Art/References/jebby_color_knight_character_sheet_v01.png (REQUIRED upload)",
       "ART-001 output (visual family consistency)"],
 contract=("Foreground = Jebby head + rainbow gem badge ONLY, on FULL transparency. All "
   "critical content inside the centred 264x264px safe zone (66dp of the 108dp layer at 4x); "
   "outer 72px on every side is mask/parallax reserve and must be fully transparent. "
   + STYLE_CORE),
 prompt=("Create an 864x864 image (will be downscaled to exactly 432x432): the head of "
   + JEBBY_ID + " with the rainbow gem badge below it, floating on a FULLY TRANSPARENT "
   "background. The entire subject must fit inside a centred square that is 61% of the "
   "canvas (safe zone); everything outside it stays 100% transparent. No background, no "
   "gradient, no shadow. Output: PNG RGBA. Filename: ic_launcher_foreground_432.png"),
 negative=("No opaque background. No text. No partial-alpha fringe/halo. Nothing outside the "
   "61% centre square. No redesign of identity."),
 post=("Downscale to exactly 432x432; assert alpha==0 outside centre 264x264 (validator "
   "checks corners + border band); assert subject bbox inside safe zone."),
 wiring=("Unity: PlayerSettings > Android > Icons > Adaptive - assign foreground. Then "
   "rebuild AAB + rerun release preflight. (Or extend Apply Approved Build Config - approval "
   "required per CLAUDE.md.)"),
 accept=["432x432 exact; RGBA", "subject fully inside 264px centre", "outer 72px band fully transparent",
   "survives circle + squircle masks (manual check)"],
 blockers="None."),
"ART-003": dict(
 purpose="Android adaptive launcher icon BACKGROUND layer.",
 refs=["Art/Sprites/Backgrounds/bg_sky_layer_01.png (palette source - upload)"],
 contract=("Plain soft pastel sky gradient sampled from the gameplay sky family. NO focal "
   "detail anywhere (the launcher masks and pans this layer). Fully opaque."),
 prompt=("Create an 864x864 fully opaque image (downscaled to exactly 432x432): a smooth "
   "vertical pastel gradient from soft light blue (top) through pale lavender-pink to warm "
   "cream (bottom), matching a gentle storybook sky. Optionally 2-3 extremely faint soft "
   "cloud shapes, low contrast. No characters, no objects, no text, no vignette. Output: "
   "PNG. Filename: ic_launcher_background_432.png"),
 negative="No focal elements, no sparkles near edges, no transparency, no banding.",
 post="Downscale to exactly 432x432; strip alpha (opaque 24/32-bit with A=255); check for banding.",
 wiring="PlayerSettings > Android > Icons > Adaptive - assign background; rebuild.",
 accept=["432x432 exact; fully opaque", "no focal detail (mask/pan safe)", "palette matches sky family"],
 blockers="None."),
"ART-004": dict(
 purpose="Google Play feature graphic - the banner at the top of the store listing.",
 refs=["Docs/Art/References/jebby_color_knight_character_sheet_v01.png (REQUIRED)",
       "evidence/screenshots/Game__PlayingHUD__...png (world style)"],
 contract=("Landscape hero composition: Jebby mid-jump toward colourful floating platforms "
   "(use the six semantic platform colours #E63838 #3878E6 #38BF59 #FAD12E #9938E6 #F28C26 "
   "as accents), pastel sky. Keep the focal subject inside the central 80% (edge crop "
   "tolerance). NO text (wordmark composited later once ART-010 is approved). Opaque. "
   + STYLE_CORE),
 prompt=("Create a 2048x1000 landscape banner (downscaled to exactly 1024x500). Scene: "
   + JEBBY_ID + " leaping joyfully up-right toward a rising trail of rounded floating "
   "platform pills coloured exactly #E63838, #3878E6, #38BF59, #FAD12E; soft pastel sky "
   "with gentle clouds and a few restrained sparkles; sense of cheerful upward adventure. "
   "Jebby occupies the left-centre third. Leave the right third calmer (space for a future "
   "logo overlay). Fully opaque. Output: PNG. Filename: jebby_jump_feature_1024x500.png"),
 negative=("NO text/lettering/logo. No UI elements or fake screenshots. No alpha. No "
   "characters other than Jebby. No identity redesign. Nothing critical in the outer 10%."),
 post="Downscale to exactly 1024x500; strip alpha; verify 24-bit.",
 wiring="None (StoreAssets/; Play Console upload at listing time).",
 accept=["1024x500 exact; NO alpha", "identity + platform colours exact", "right third calm for logo",
   "no text baked"],
 blockers="Wordmark compositing deferred until ART-010 approved (banner ships text-free)."),
"ART-005": dict(
 purpose=("Replace the literal Placeholder.png (32x32, tinted cream, stretched 37x) that is "
   "the gameplay floor/start strip every player stands on at spawn."),
 refs=["Art/Sprites/Backgrounds/bg_sky_layer_01.png (palette)",
   "evidence/screenshots/Game__MemoryPhase__...png (current cream band in context)"],
 contract=("Horizontally SEAMLESS ground strip: soft cream/warm cloud-stone top surface with "
   "gentle grass or cloud tufts, reading clearly as 'ground' against the pastel sky while "
   "never competing with the six platform colours. Top edge may be softly irregular; bottom "
   "bleeds to solid fill. Base colour family #F2EBD9 (the current tint)."),
 prompt=("Create a 1024x256 horizontally TILEABLE ground strip (downscaled to exactly "
   "512x128): a soft storybook cloud-meadow ledge - warm cream (#F2EBD9 family) stone/cloud "
   "surface, tiny pastel grass tufts and pebbles on top, solid warm fill below. The LEFT and "
   "RIGHT edges must tile seamlessly. Muted saturation so red/blue/green/yellow platforms "
   "pop above it. Transparent above the ledge silhouette only. Output: PNG RGBA. Filename: "
   "spr_floor_strip_01.png"),
 negative=("No characters/objects. No strong saturated colours (never matches the six "
   "platform hues). No visible tiling seam. No hard black outline."),
 post=("Downscale to exactly 512x128; verify wrap-seam (left column vs right column diff < "
   "2/255 mean); verify top-corner transparency."),
 wiring=("Game.unity Floor SpriteRenderer: assign sprite, set drawMode Tiled (width ~37.6 "
   "world units), reset m_Color tint to white, keep BoxCollider2D unchanged; visual-only. "
   "Then delete Art/Sprites/Placeholder.png (becomes unreferenced). Scene-integrity + "
   "overlap tests rerun."),
 accept=["512x128; tiles seamlessly on X", "reads as ground; platforms still pop",
   "collider alignment unchanged (visual top within 4px of collider top)"],
 blockers=("RESOLVED (D1, 2026-07-16): taller cloud-meadow LEDGE. The spec above is final - "
   "512x128 art, drawMode Tiled, ~0.6 world-unit visible ground. No open decision remains.")),
"ART-006": dict(
 purpose="Main-menu background (first screen players see; currently a flat navy clear colour).",
 refs=["Art/Sprites/Backgrounds/bg_sky_layer_01.png (REQUIRED upload - same world)",
   "Docs/Art/References/jebby_color_knight_character_sheet_v01.png"],
 contract=("Wide pastel-sky vista in the exact family of the gameplay sky: floating islands, "
   "soft clouds, a distant rainbow-gem tower motif (the game's theme), calm centre band "
   "where the menu stack sits (buttons must stay readable). 2400x1080 so 16:9..20:9 crops "
   "safely. " + STYLE_CORE),
 prompt=("Create a 2400x1080 landscape menu background continuing the EXACT style of the "
   "uploaded gameplay sky: soft pastel blue-pink sky, fluffy clouds, a few small floating "
   "grassy islands at the edges, and a subtle distant tower of tiny colourful platforms "
   "rising on the right horizon with a faint rainbow glint. Keep the CENTRE band "
   "(middle 40% width) low-detail and slightly darker/calmer so white UI text reads on it. "
   "Fully opaque. No characters. Output: PNG. Filename: bg_menu_01.png"),
 negative=("No text. No characters. No high-contrast detail in the centre band. No alpha. "
   "No style drift from the uploaded sky."),
 post="Exact 2400x1080; opaque; centre-band contrast check vs white text (validator luminance rule).",
 wiring=("MainMenu.unity: add full-screen Image as FIRST child of MainMenuCanvas (behind "
   "TitleText), assign sprite, preserveAspect envelope; re-run MainMenu overlap regression "
   "tests."),
 accept=["2400x1080 opaque", "white TMP text readable over centre band", "style-continuous with gameplay sky"],
 blockers="None."),
"ART-007": dict(
 purpose="9-slice panel/card chrome replacing the flat dark scaffold rectangles on every shell panel.",
 refs=["Art/Sprites/UI/ui_btn_jump.png (ornament style anchor - upload)"],
 contract=("Rounded soft panel: deep blue-charcoal body (#262633 family) with a thin warm "
   "gold rim and very soft inner glow, corners uniformly rounded (candidate radius 48px on "
   "a 256 canvas - OPEN decision), all four stretch regions PERFECTLY flat so 9-slice "
   "scaling never distorts. Subtle, not ornate - content sits on it."),
 prompt=("Create a 512x512 UI panel background (downscaled to exactly 256x256) for 9-slice "
   "use: rounded rectangle filling the canvas minus 8px margin, body deep blue-charcoal "
   "#262633 at ~97% opacity feel (render opaque; alpha only outside the shape), a 3px warm "
   "gold (#E6C36B) rim, faint soft inner top-light. Corners identical, radius exactly 96px "
   "on this 512 canvas. The middle of every edge and the centre must be COMPLETELY flat "
   "colour (no texture/gradient) for slicing. Output: PNG RGBA. Filename: ui_panel_soft_9s.png"),
 negative=("No texture/noise/gradient in stretch regions. No decorations except the rim. No "
   "drop shadow (Unity handles depth). No text."),
 post=("Downscale to 256x256; assert edge-middle rows/columns are uniform colour; assert "
   "4-corner alpha 0; record final border px for the Sprite Editor (expected 64)."),
 wiring=("Sprite Editor: border L/R/T/B = 64 (from radius+rim). Swap Image sprite + set "
   "Image.type=Sliced + colour=white on: MainMenu panels (3) + level cards + Game shell "
   "panels (4) + wardrobe card/ceremony. Multiple scene/prefab edits; rerun overlap tests."),
 accept=["flat stretch regions (validator)", "uniform corners", "text contrast >= current dark rects"],
 blockers="Corner-radius system is an OPEN_SCOPE_DECISION (candidate 48px@256 documented)."),
"ART-008": dict(
 purpose="9-slice pill button chrome for all shell buttons (currently flat dark rects; Art Bible demands pills).",
 refs=["Art/Sprites/UI/ui_btn_jump.png (style anchor - upload)", "ART-007 output (family)"],
 contract=("True pill (corner radius = half height), slightly lighter than the panel "
   "(#333340 family) with the same thin gold rim; drawn NEUTRAL-bright enough that Unity "
   "tint states (normal/highlighted/pressed/disabled) read; ONE bitmap only - no per-state "
   "art. Focus ring stays Unity-procedural."),
 prompt=("Create a 512x192 UI button (downscaled to exactly 256x96) for 9-slice use: a "
   "horizontal pill (corner radius exactly 96px on this canvas = fully rounded ends), body "
   "#333340, 3px warm gold rim (#E6C36B), soft top highlight inside the rim. Ends identical; "
   "the horizontal middle 50% must be COMPLETELY flat for stretching. Alpha 0 outside the "
   "pill. Output: PNG RGBA. Filename: ui_btn_pill_9s.png"),
 negative="No text/icon. No gradient in the stretch band. No outer shadow. No square corners.",
 post="Downscale 256x96; flat-band check; corner symmetry check; record borders (48/48/40/40).",
 wiring=("Sprite Editor borders 48/48/40/40; swap Button Image sprites across MainMenu stack "
   "(5), panel Back/Reset/Equip buttons, Game result/pause buttons; keep >=90u hit areas "
   "(ShellLayoutMetrics tests) + tint transitions; rerun shell tests."),
 accept=["pill silhouette; flat stretch band", "tint states legible", ">=90u touch targets preserved"],
 blockers="Shares the radius OPEN decision with ART-007 (pill is self-consistent regardless)."),
"ART-009": dict(
 purpose="Play store screenshots (min 2 required; 4+ recommended) - REAL captures, never AI-fabricated gameplay.",
 refs=["capture sources: the game itself after ART-005..008 land"],
 contract=("1920x1080 landscape PNG (no alpha), truthful frames: (1) gameplay mid-jump with "
   "memory HUD, (2) memory-sequence moment, (3) wardrobe with outfits, (4) level select. "
   "Optional caption band composited above/below AFTER capture (no fake UI)."),
 prompt=("NOT an image-generation task. Capture real frames at 1920x1080 from the editor "
   "Game view (focused editor, Play Mode) or an Android device AFTER the UI-kit batch "
   "lands. Only deterministic post-processing (crop/caption frame) is permitted."),
 negative="No AI-generated or staged fake gameplay. No feature claims. No device frames with wrong aspect.",
 post="Verify 1920x1080, 24-bit, no alpha; filenames shot_01_gameplay.png..shot_04_wardrobe.png.",
 wiring="None (StoreAssets/Screenshots/).",
 accept=["real captures; truthful", "1920x1080 no alpha", ">=4 frames covering gameplay/memory/wardrobe/levels"],
 blockers="Capture AFTER ART-005..008 integration (screenshots of placeholder chrome would need retaking)."),
"ART-010": dict(
 purpose="'Jebby Jump' wordmark for title screen, splash, store feature graphic and press.",
 refs=["Docs/Art/References/jebby_color_knight_character_sheet_v01.png (palette/mood)"],
 contract=("Rounded, friendly, storybook letterforms; warm cream/gold letters with a subtle "
   "rainbow-gem glint on the 'J' dots or the 'u'; must read on BOTH pastel sky and dark "
   "panel. Deliver VECTOR master (SVG) + PNG render. ORIGINAL lettering (no unlicensed "
   "font); legal/brand review before store use."),
 prompt=("Create a 2400x800 wordmark reading exactly 'Jebby Jump' (two words, capital J's): "
   "plump rounded storybook letterforms, warm ivory fill with soft gold outline and gentle "
   "top-light, ONE small rainbow gem accent dotting a letter. Transparent background. "
   "Slight playful arc allowed (<=8 degrees). Must stay readable when 200px wide. Output: "
   "PNG RGBA. Filename: ui_wordmark_jebbyjump.png. ALSO deliver the vector source (SVG)."),
 negative=("No misspelling/case change. No more than one gem accent. No drop shadow baked. "
   "No existing commercial font traced. No taglines."),
 post="Trim to content + 4% padding; render exact 1200x400 runtime PNG from the master.",
 wiring=("MainMenu: replace TitleText TMP with an Image (or overlay) sized ~700x233 canvas "
   "units at the P33 title slot (y=380); optional Unity splash logo slot; reuse in ART-004 "
   "composite later. Rerun MainMenu overlap tests."),
 accept=["exact spelling", "reads at 200px", "works on sky + dark panel", "SVG master delivered",
   "legal/brand sign-off recorded"],
 blockers="LEGAL_OR_BRAND_REVIEW_REQUIRED before store use (originality/trademark check)."),
"ART-011": dict(
 purpose="Bubble Shield skill icon (Skill 2 button currently shows the generic circle bg).",
 refs=["Art/Sprites/Items/spr_item_rocket_boots_01.png (icon style anchor - upload)",
   "Art/Sprites/UI/ui_btn_jump.png (button family)"],
 contract=("A shiny protective bubble with a small shield glint, aqua-blue (#3878E6 family), "
   "same rendered-icon style as the rocket boots icon; centred, transparent corners; reads "
   "at 70px inside the existing round button."),
 prompt=("Create a 512x512 skill icon (downscaled to exactly 256x256): a glossy translucent "
   "aqua-blue magic bubble with a soft white rim highlight and a tiny shield-shaped glint "
   "in its centre, matching the painterly-ornate style of the uploaded rocket-boots icon. "
   "Centred, ~78% of canvas, fully transparent background. Output: PNG RGBA. Filename: "
   "ui_icon_skill_shield_01.png"),
 negative="No text. No character. No metallic heraldic shield (keep it a bubble). No halo.",
 post="Downscale 256x256; corner alpha 0; centre-of-mass within 4px of canvas centre.",
 wiring="Game.unity Btn_Skill2/Skill2Icon Image: assign sprite (one field).",
 accept=["reads as protective bubble at 70px", "style matches rocket boots icon"],
 blockers="None."),
"ART-012": dict(
 purpose="Health Potion skill icon (Skill 3 button currently shows the generic circle bg).",
 refs=["Art/Sprites/UI/ui_icon_life_01.png (heart motif - upload)",
   "Art/Sprites/Items/spr_item_rocket_boots_01.png (style anchor)"],
 contract=("A rounded glass potion bottle with warm pink-red liquid and a tiny heart bubble, "
   "tying visually to the life hearts; same icon style family; child-safe (no medical "
   "cross)."),
 prompt=("Create a 512x512 skill icon (downscaled to exactly 256x256): a cute rounded glass "
   "potion bottle, cork stopper, glowing warm pink-red liquid with one tiny heart-shaped "
   "bubble, gold-tinted glass rim matching the uploaded ornate icon style. Centred ~78% of "
   "canvas, transparent background. Output: PNG RGBA. Filename: ui_icon_skill_potion_01.png"),
 negative="No medical cross. No text. No skull/poison imagery. No halo.",
 post="Downscale 256x256; corner alpha 0.",
 wiring="Game.unity Btn_Skill3/Skill3Icon Image: assign sprite.",
 accept=["reads as potion at 70px", "heart motif echoes lives"],
 blockers="None."),
"ART-013": dict(
 purpose="Bubble-shield aura sprite (the active effect currently renders an ARROW button sprite).",
 refs=["ART-011 output (colour family)"],
 contract=("Radially symmetric translucent bubble: bright thin rim, very soft interior "
   "(centre ~25-35% opacity), 2 subtle specular arcs; #3878E6 family; gameplay must stay "
   "visible through it."),
 prompt=("Create a 1024x1024 VFX sprite (downscaled to exactly 512x512): a perfect circular "
   "soap-bubble shield - thin bright aqua-white rim, interior filled at only ~30% opacity "
   "with a soft radial falloff, two gentle curved specular highlights upper-left, fully "
   "transparent outside the circle. Radially clean (no direction). Output: PNG RGBA. "
   "Filename: fx_bubble_shield_01.png"),
 negative="No opaque interior. No sparkles outside the rim. No texture noise. No text.",
 post="Downscale 512x512; radial-symmetry check (rotate 90 diff < 6/255); corner alpha 0.",
 wiring="Game.unity BubbleShieldEffect SpriteRenderer: assign sprite; code/tint untouched.",
 accept=["Jebby + platforms visible through it", "reads as protection instantly"],
 blockers="Displayed world diameter derived at wiring time (BLOCKED live measure - effect object inactive in edit mode)."),
"ART-014": dict(
 purpose="Color-echo pulse ring (currently the circle button bg tinted).",
 refs=[],
 contract=("NEUTRAL white/grey hollow ring designed for runtime tinting to any of the six "
   "platform colours; crisp outer edge, soft inner falloff, fully hollow centre."),
 prompt=("Create a 1024x1024 VFX ring (downscaled to exactly 512x512): a single perfect "
   "hollow circle ring - pure white, outer edge crisp, inner edge soft-faded, ring "
   "thickness ~8% of diameter, completely transparent centre and corners. Grayscale/white "
   "only. Output: PNG RGBA. Filename: fx_color_echo_ring_01.png"),
 negative="No colour (white only - runtime tints it). No sparkles. No gaps in the ring.",
 post="Downscale 512x512; verify hollow centre alpha<=2; verify ring uniformity.",
 wiring="Game.unity ColorEchoEffect SpriteRenderer: assign sprite.",
 accept=["tints cleanly to all six platform hexes", "perfectly circular"],
 blockers="None."),
"ART-015": dict(
 purpose="Adaptive icon monochrome layer (Android 13+ themed icons).",
 refs=["ART-002 output (REQUIRED - this is a derivative)"],
 contract="Single-colour silhouette of the ART-002 foreground (alpha carries the shape).",
 prompt=("Deterministic derivation preferred over generation: flatten ART-002's foreground "
   "to a single #FFFFFF silhouette preserving alpha; simplify interior holes so the shape "
   "reads as Jebby's head + gem at 48dp. If generating: 432x432, one flat colour + alpha, "
   "same silhouette as ART-002. Filename: ic_launcher_monochrome_432.png"),
 negative="No shading/gradients/second colour. No new shapes.",
 post="Threshold+flatten from ART-002; assert single colour; safe-zone check as ART-002.",
 wiring="Android monochrome icon slot (Unity exposes via adaptive icon settings when available; else Gradle template note in integration manifest).",
 accept=["recognizable at 48dp", "single colour + alpha only"],
 blockers="Depends on ART-002 final."),
"ART-016": dict(
 purpose="Marketing key art master (press kit, future store reuse, feature-graphic source).",
 refs=["Docs/Art/References/jebby_color_knight_character_sheet_v01.png (REQUIRED)",
   "Docs/Art/References/jebby_outfit_variations_board_v01.png"],
 contract=("Hero scene: Jebby leaping up a spiral of colourful platform pills toward a "
   "glowing rainbow gem sky-castle; joyful, adventurous; full locked palette; composition "
   "crops cleanly to 1024x500 (feature) and 1:1 (icon echoes). " + STYLE_CORE),
 prompt=("Create a 3840x2160 key art: " + JEBBY_ID + " mid-leap (jump pose, arms up) on a "
   "rising spiral of rounded platform pills coloured exactly #E63838 #3878E6 #38BF59 "
   "#FAD12E #9938E6 #F28C26, ascending toward a distant glowing rainbow-gem tower among "
   "pastel clouds; warm light from upper right; restrained sparkles. Jebby left-of-centre "
   "at ~35% width, face clearly visible. Opaque. Output: PNG. Filename: "
   "jebby_jump_keyart_3840x2160.png"),
 negative="No text. No other characters. No identity drift. No dark/scary sky.",
 post="Verify 3840x2160 opaque; produce 1024x500 crop test.",
 wiring="None (Marketing/).",
 accept=["identity exact", "crops to 1024x500 without losing Jebby's face", "palette exact"],
 blockers="Brand review recommended before public use."),
"ART-017": dict(
 purpose="Cactus anticipation/warning variant (fairness read before contact).",
 refs=["Art/Sprites/Obstacles/spr_cactus_obstacle_01.png (REQUIRED - silhouette must match)"],
 contract=("SAME silhouette and content bounds as the base cactus (validator compares "
   "bboxes), with a friendly 'about to be prickly' read: slight lean, brighter spine tips, "
   "soft warning sparkle. Child-safe, not scary."),
 prompt=("Using the uploaded cactus sprite as the EXACT base (same pose, same silhouette, "
   "same canvas 1254x1254, same bounds): create a variant where the spines' tips glow "
   "softly warm-yellow and the cactus leans 2-3 degrees as a gentle warning. Everything "
   "else identical. Transparent background. Output: PNG RGBA. Filename: "
   "spr_cactus_obstacle_warn_01.png"),
 negative="No silhouette change beyond the 3-degree lean. No angry face. No red flashing look.",
 post="Bbox delta vs base <= 4% each side; corner alpha 0; canvas exactly 1254x1254.",
 wiring=("Requires a small gameplay design decision + code (swap/flash before contact "
   "window). NOT a drop-in; hold until approved."),
 accept=["silhouette matches base within 4%", "reads as warning, stays friendly"],
 blockers="Gameplay anticipation behaviour = OPEN design decision; art can wait for it."),
}

TPL = """# {id} — {title}

## 1. Purpose
{purpose}

## 2. Launch priority
{scope} / {priority}

## 3. Current problem
{problem}

## 4. Evidence
{evidence} (labels: {labels})

## 5. Exact target file and archive path
`{archive}` (target dir `{tdir}`, filename `{fname}`)

## 6. Replacement / .meta / GUID handling
{replacement}. {meta}

## 7. Locked references
{refs}

## 8. Visual design contract
{contract}

## 9. Exact pixel canvas
Runtime: **{rw}x{rh}px**; master: {mw}x{mh}. Colour space {cs}, {bd}-bit. Format {fmt}.

## 10. Exact content bounds and transparent padding
{bounds}

## 11. Pivot, baseline and contact points
{pivot}

## 12. Unity import settings
{import_}

## 13. Scene/prefab placement
{placement}

## 14. Colour and rendering specification
{palette}

## 15. Animation specification
Not applicable (single static image){animnote}

## 16. Accessibility requirements
{access}

## 17. ChatGPT generation feasibility
{feas}

## 18. ChatGPT generation prompt
{prompt}

## 19. Negative prompt / forbidden changes
{negative}

## 20. Deterministic post-processing instructions
{post}

## 21. Unity integration steps
{wiring}

## 22. Acceptance checklist
{accept}

## 23. Blockers/open decisions
{blockers}
"""

def main():
    rows = {r["asset_id"]: r for r in json.load(open(MAN, encoding="utf-8"))}
    os.makedirs(OUT, exist_ok=True)
    for aid, b in B.items():
        r = rows[aid]
        bounds = r.get("content_bounds_px") or r.get("safe_area_rule") or "Full-canvas composition; corners transparent where alpha applies."
        pad = r.get("transparent_padding_px")
        if pad: bounds += f" Transparent padding: {pad}."
        pivot = "Not applicable (UI/marketing image)."
        if r.get("pivot_normalized_x") != "":
            pivot = f"Pivot normalized ({r['pivot_normalized_x']}, {r['pivot_normalized_y']})."
        if aid == "ART-005":
            pivot = "Pivot top-centre (0.5, 1.0) so the strip hangs from the collider top; ground contact = sprite top edge."
        imp = []
        for k, lbl in [("sprite_mode","spriteMode"),("pixels_per_unit","PPU"),("mesh_type","meshType"),
                       ("sprite_border_px","9-slice border"),("filter_mode","filter"),("wrap_mode","wrap"),
                       ("compression","compression"),("max_texture_size","maxSize")]:
            if r.get(k): imp.append(f"{lbl}: {r[k]}")
        import_ = "; ".join(imp) if imp else "Not applicable (never imported by Unity — store/marketing asset)."
        placement = []
        for k in ("used_by_scenes","used_by_gameobjects","canvas_path","recttransform_size","world_size","image_type","minimum_touch_target"):
            if r.get(k): placement.append(f"{k}: {r[k]}")
        placement = "; ".join(placement) or "Store/marketing only — no scene placement."
        access = "Child-safe imagery; readable at minimum display size named in acceptance."
        if aid in ("ART-007","ART-008"): access = "Must preserve >=90 canvas-unit touch targets and text contrast (P20/P21 tested metrics); focus ring stays visible on top."
        if aid == "ART-014": access = "Neutral base guarantees colour-semantic tint fidelity; pairs with TMP digit cues (no colour-only info)."
        text = TPL.format(
            id=aid, title=r["title"], purpose=b["purpose"], scope=r["launch_scope"],
            priority=r["priority"], problem=r["current_status"] + f" (problem: {r['problem_type']})",
            evidence=r["evidence"], labels=r["evidence_labels"],
            archive=r["archive_path"], tdir=r["target_asset_path"], fname=r["target_filename"],
            replacement=r["replacement_type"],
            meta=("Never include any .meta in the ZIP. " + (f"Existing meta note: {r['preserve_existing_meta']}." if r.get("preserve_existing_meta") else "New file: Unity creates the .meta on import.")),
            refs="\n".join("- " + x for x in b["refs"]) if b["refs"] else "None required (neutral shape).",
            contract=b["contract"], rw=r["runtime_width_px"], rh=r["runtime_height_px"],
            mw=r.get("master_width_px") or "same", mh=r.get("master_height_px") or "same",
            cs=r["colour_space"], bd=r["bit_depth"], fmt=r["artifact_format"],
            bounds=bounds, pivot=pivot, import_=import_, placement=placement,
            palette=(r.get("palette_hex") or "See design contract.") + (f" Gradient: {r['gradient_spec']}." if r.get("gradient_spec") else ""),
            animnote="" if aid not in ("ART-013","ART-014") else " — animation is runtime scale/alpha tween in code; the sprite itself is static.",
            access=access, feas=r["generation_feasibility"] + f" (reference image required: {r['requires_reference_image']}; human review: {r['requires_human_review']})",
            prompt=b["prompt"], negative=b["negative"], post=b["post"], wiring=b["wiring"],
            accept="\n".join("- [ ] " + a for a in b["accept"]),
            blockers=b["blockers"])
        open(os.path.join(OUT, aid + ".md"), "w", encoding="utf-8").write(text)
    print(f"briefs written: {len(B)}")

if __name__ == "__main__":
    main()
