# 20 — World Art Generation Briefs (ChatGPT-ready)

Rules for every brief: follow the locked Jebby Jump art bible + Jebby identity; deliver PNG only
(never .meta/prefab/scene/material/atlas/SO); exact canvas + filename as stated; keep the six
gameplay colours identity-locked; respect transparent/opaque rule; leave crop-safe margins;
no text baked into gameplay art; child-safe, warm palette. Post-processing is deterministic
(documented in doc 22). Validate against doc 22 before acceptance.

## W01 — Cloud Meadow

**Shared direction:** A gentle sky-meadow of drifting cloudpuffs. Mood: soft, welcoming, pastel. Material: cloud & meadow grass. Palette: warm sky blue + cream (support peach + mint). Lighting: soft midday, top-left. Atmosphere: calm breeze, drifting fluff.

### W01-BG — Background
- Canvas 2400×1080 RGB opaque, PPU 100 → `Assets/_JebbyJump/Art/Worlds/W01_CloudMeadow/Backgrounds/bg_cloudmeadow_01.png`.
- Full-bleed Cloud Meadow sky/environment; must fill 16:9→20:9 when camera-locked at 1.5×; warm, no debug UI, no characters, no text.

### LANDMARK — Distant Rainbow Tower
- Canvas 900×1400 RGBA transparent → `Assets/_JebbyJump/Art/Worlds/W01_CloudMeadow/Backgrounds/landmark_tower_cloudmeadow_01.png`.
- The recurring Rainbow Tower, a tiny far-off glimmer on the horizon. Tower stage per doc 05 (gets closer/clearer across worlds).

### W01-PLAT — Six platforms
- Canvas 256×96 RGBA each, PPU 100, 9-slice-safe → `Assets/_JebbyJump/Art/Worlds/W01_CloudMeadow/Platforms/plat_cloudmeadow_<color>_01.png` for red/blue/green/yellow/purple/orange.
- Each rendered as cloud & meadow grass, but the base colour MUST still read as its locked hue at 70px. Do not change silhouette/width.

### W01-FLOOR — Start ledge
- Canvas 512×128 RGB, PPU 100 → `Assets/_JebbyJump/Art/Worlds/W01_CloudMeadow/Floor/floor_cloudmeadow_01.png`. Themed ledge; keep the surface line where the cactus/Jebby stand.

### W01-HAZ — Hazard (cloud_thorn)
- Canvas 1254×1254 RGBA transparent, ink-base pivot → `Assets/_JebbyJump/Art/Worlds/W01_CloudMeadow/Obstacles/hazard_cloud_thorn_01.png`.
- A Cloud Meadow version of the spike/thorn hazard; clearly a HAZARD (never mistakable for a safe platform); same footprint as the cactus.

### W01-DECO / VFX / UI
- Decoration set 1024² RGBA (non-colliding props) → `Assets/_JebbyJump/Art/Worlds/W01_CloudMeadow/Decorations/`. Ambient VFX texture → `Assets/_JebbyJump/Art/Worlds/W01_CloudMeadow/VFX/`.
- UI: world thumbnail 512² , world badge 192² , story card 1600×900, finale 1920×1080, World Gem 256², cosmetic (Cloudpuff cape) 512² → `Assets/_JebbyJump/Art/Worlds/W01_CloudMeadow/UI/` (exact names in manifest, doc 18).

## W02 — Enchanted Forest

**Shared direction:** A glowing woodland of mossy boughs. Mood: cozy, magical, verdant. Material: mossy wood & leaves. Palette: deep green + amber (support teal + gold). Lighting: dappled canopy light. Atmosphere: floating spores, fireflies.

### W02-BG — Background
- Canvas 2400×1080 RGB opaque, PPU 100 → `Assets/_JebbyJump/Art/Worlds/W02_EnchantedForest/Backgrounds/bg_enchantedforest_01.png`.
- Full-bleed Enchanted Forest sky/environment; must fill 16:9→20:9 when camera-locked at 1.5×; warm, no debug UI, no characters, no text.

### LANDMARK — Distant Rainbow Tower
- Canvas 900×1400 RGBA transparent → `Assets/_JebbyJump/Art/Worlds/W02_EnchantedForest/Backgrounds/landmark_tower_enchantedforest_01.png`.
- The recurring Rainbow Tower, a distant spire seen through the trees. Tower stage per doc 05 (gets closer/clearer across worlds).

### W02-PLAT — Six platforms
- Canvas 256×96 RGBA each, PPU 100, 9-slice-safe → `Assets/_JebbyJump/Art/Worlds/W02_EnchantedForest/Platforms/plat_enchantedforest_<color>_01.png` for red/blue/green/yellow/purple/orange.
- Each rendered as mossy wood & leaves, but the base colour MUST still read as its locked hue at 70px. Do not change silhouette/width.

### W02-FLOOR — Start ledge
- Canvas 512×128 RGB, PPU 100 → `Assets/_JebbyJump/Art/Worlds/W02_EnchantedForest/Floor/floor_enchantedforest_01.png`. Themed ledge; keep the surface line where the cactus/Jebby stand.

### W02-HAZ — Hazard (forest_thorn_bloom)
- Canvas 1254×1254 RGBA transparent, ink-base pivot → `Assets/_JebbyJump/Art/Worlds/W02_EnchantedForest/Obstacles/hazard_forest_thorn_bloom_01.png`.
- A Enchanted Forest version of the spike/thorn hazard; clearly a HAZARD (never mistakable for a safe platform); same footprint as the cactus.

### W02-DECO / VFX / UI
- Decoration set 1024² RGBA (non-colliding props) → `Assets/_JebbyJump/Art/Worlds/W02_EnchantedForest/Decorations/`. Ambient VFX texture → `Assets/_JebbyJump/Art/Worlds/W02_EnchantedForest/VFX/`.
- UI: world thumbnail 512² , world badge 192² , story card 1600×900, finale 1920×1080, World Gem 256², cosmetic (Leafcrown hood) 512² → `Assets/_JebbyJump/Art/Worlds/W02_EnchantedForest/UI/` (exact names in manifest, doc 18).

## W03 — Crystal Caves

**Shared direction:** Luminous caverns of singing crystal. Mood: wondrous, cool-but-warm-lit. Material: faceted crystal. Palette: violet + warm torchglow (support cyan + rose). Lighting: crystal glow + torchlight. Atmosphere: gentle sparkle motes.

### W03-BG — Background
- Canvas 2400×1080 RGB opaque, PPU 100 → `Assets/_JebbyJump/Art/Worlds/W03_CrystalCaves/Backgrounds/bg_crystalcaves_01.png`.
- Full-bleed Crystal Caves sky/environment; must fill 16:9→20:9 when camera-locked at 1.5×; warm, no debug UI, no characters, no text.

### LANDMARK — Distant Rainbow Tower
- Canvas 900×1400 RGBA transparent → `Assets/_JebbyJump/Art/Worlds/W03_CrystalCaves/Backgrounds/landmark_tower_crystalcaves_01.png`.
- The recurring Rainbow Tower, the tower gleams down a crystal shaft. Tower stage per doc 05 (gets closer/clearer across worlds).

### W03-PLAT — Six platforms
- Canvas 256×96 RGBA each, PPU 100, 9-slice-safe → `Assets/_JebbyJump/Art/Worlds/W03_CrystalCaves/Platforms/plat_crystalcaves_<color>_01.png` for red/blue/green/yellow/purple/orange.
- Each rendered as faceted crystal, but the base colour MUST still read as its locked hue at 70px. Do not change silhouette/width.

### W03-FLOOR — Start ledge
- Canvas 512×128 RGB, PPU 100 → `Assets/_JebbyJump/Art/Worlds/W03_CrystalCaves/Floor/floor_crystalcaves_01.png`. Themed ledge; keep the surface line where the cactus/Jebby stand.

### W03-HAZ — Hazard (crystal_spike_cluster)
- Canvas 1254×1254 RGBA transparent, ink-base pivot → `Assets/_JebbyJump/Art/Worlds/W03_CrystalCaves/Obstacles/hazard_crystal_spike_cluster_01.png`.
- A Crystal Caves version of the spike/thorn hazard; clearly a HAZARD (never mistakable for a safe platform); same footprint as the cactus.

### W03-DECO / VFX / UI
- Decoration set 1024² RGBA (non-colliding props) → `Assets/_JebbyJump/Art/Worlds/W03_CrystalCaves/Decorations/`. Ambient VFX texture → `Assets/_JebbyJump/Art/Worlds/W03_CrystalCaves/VFX/`.
- UI: world thumbnail 512² , world badge 192² , story card 1600×900, finale 1920×1080, World Gem 256², cosmetic (Geode pauldrons) 512² → `Assets/_JebbyJump/Art/Worlds/W03_CrystalCaves/UI/` (exact names in manifest, doc 18).

## W04 — Sunshine Desert

**Shared direction:** Warm dunes under a friendly sun. Mood: bright, adventurous, warm. Material: sun-baked sandstone. Palette: golden sand + sky (support coral + turquoise). Lighting: strong warm sun, high. Atmosphere: heat shimmer, soft wind.

### W04-BG — Background
- Canvas 2400×1080 RGB opaque, PPU 100 → `Assets/_JebbyJump/Art/Worlds/W04_SunshineDesert/Backgrounds/bg_sunshinedesert_01.png`.
- Full-bleed Sunshine Desert sky/environment; must fill 16:9→20:9 when camera-locked at 1.5×; warm, no debug UI, no characters, no text.

### LANDMARK — Distant Rainbow Tower
- Canvas 900×1400 RGBA transparent → `Assets/_JebbyJump/Art/Worlds/W04_SunshineDesert/Backgrounds/landmark_tower_sunshinedesert_01.png`.
- The recurring Rainbow Tower, the tower rises beyond the far dunes. Tower stage per doc 05 (gets closer/clearer across worlds).

### W04-PLAT — Six platforms
- Canvas 256×96 RGBA each, PPU 100, 9-slice-safe → `Assets/_JebbyJump/Art/Worlds/W04_SunshineDesert/Platforms/plat_sunshinedesert_<color>_01.png` for red/blue/green/yellow/purple/orange.
- Each rendered as sun-baked sandstone, but the base colour MUST still read as its locked hue at 70px. Do not change silhouette/width.

### W04-FLOOR — Start ledge
- Canvas 512×128 RGB, PPU 100 → `Assets/_JebbyJump/Art/Worlds/W04_SunshineDesert/Floor/floor_sunshinedesert_01.png`. Themed ledge; keep the surface line where the cactus/Jebby stand.

### W04-HAZ — Hazard (desert_cactus)
- Canvas 1254×1254 RGBA transparent, ink-base pivot → `Assets/_JebbyJump/Art/Worlds/W04_SunshineDesert/Obstacles/hazard_desert_cactus_01.png`.
- A Sunshine Desert version of the spike/thorn hazard; clearly a HAZARD (never mistakable for a safe platform); same footprint as the cactus.

### W04-DECO / VFX / UI
- Decoration set 1024² RGBA (non-colliding props) → `Assets/_JebbyJump/Art/Worlds/W04_SunshineDesert/Decorations/`. Ambient VFX texture → `Assets/_JebbyJump/Art/Worlds/W04_SunshineDesert/VFX/`.
- UI: world thumbnail 512² , world badge 192² , story card 1600×900, finale 1920×1080, World Gem 256², cosmetic (Sunwrap scarf) 512² → `Assets/_JebbyJump/Art/Worlds/W04_SunshineDesert/UI/` (exact names in manifest, doc 18).

## W05 — Ocean Sky

**Shared direction:** Floating reefs where sea meets sky. Mood: dreamy, fresh, buoyant. Material: coral & sea-glass. Palette: aqua + warm coral (support seafoam + sunset). Lighting: bright aquatic light. Atmosphere: bubbles, drifting spray.

### W05-BG — Background
- Canvas 2400×1080 RGB opaque, PPU 100 → `Assets/_JebbyJump/Art/Worlds/W05_OceanSky/Backgrounds/bg_oceansky_01.png`.
- Full-bleed Ocean Sky sky/environment; must fill 16:9→20:9 when camera-locked at 1.5×; warm, no debug UI, no characters, no text.

### LANDMARK — Distant Rainbow Tower
- Canvas 900×1400 RGBA transparent → `Assets/_JebbyJump/Art/Worlds/W05_OceanSky/Backgrounds/landmark_tower_oceansky_01.png`.
- The recurring Rainbow Tower, the tower stands on a far sky-island. Tower stage per doc 05 (gets closer/clearer across worlds).

### W05-PLAT — Six platforms
- Canvas 256×96 RGBA each, PPU 100, 9-slice-safe → `Assets/_JebbyJump/Art/Worlds/W05_OceanSky/Platforms/plat_oceansky_<color>_01.png` for red/blue/green/yellow/purple/orange.
- Each rendered as coral & sea-glass, but the base colour MUST still read as its locked hue at 70px. Do not change silhouette/width.

### W05-FLOOR — Start ledge
- Canvas 512×128 RGB, PPU 100 → `Assets/_JebbyJump/Art/Worlds/W05_OceanSky/Floor/floor_oceansky_01.png`. Themed ledge; keep the surface line where the cactus/Jebby stand.

### W05-HAZ — Hazard (coral_spine)
- Canvas 1254×1254 RGBA transparent, ink-base pivot → `Assets/_JebbyJump/Art/Worlds/W05_OceanSky/Obstacles/hazard_coral_spine_01.png`.
- A Ocean Sky version of the spike/thorn hazard; clearly a HAZARD (never mistakable for a safe platform); same footprint as the cactus.

### W05-DECO / VFX / UI
- Decoration set 1024² RGBA (non-colliding props) → `Assets/_JebbyJump/Art/Worlds/W05_OceanSky/Decorations/`. Ambient VFX texture → `Assets/_JebbyJump/Art/Worlds/W05_OceanSky/VFX/`.
- UI: world thumbnail 512² , world badge 192² , story card 1600×900, finale 1920×1080, World Gem 256², cosmetic (Tidal fin cloak) 512² → `Assets/_JebbyJump/Art/Worlds/W05_OceanSky/UI/` (exact names in manifest, doc 18).

## W06 — Candy Cloud Kingdom

**Shared direction:** A sweet kingdom of frosting clouds. Mood: playful, sugary, joyful. Material: candy & frosting. Palette: pink + cream + mint (support lilac + lemon). Lighting: soft candy glow. Atmosphere: sugar sparkle, sprinkles.

### W06-BG — Background
- Canvas 2400×1080 RGB opaque, PPU 100 → `Assets/_JebbyJump/Art/Worlds/W06_CandyCloudKingdom/Backgrounds/bg_candycloudkingdom_01.png`.
- Full-bleed Candy Cloud Kingdom sky/environment; must fill 16:9→20:9 when camera-locked at 1.5×; warm, no debug UI, no characters, no text.

### LANDMARK — Distant Rainbow Tower
- Canvas 900×1400 RGBA transparent → `Assets/_JebbyJump/Art/Worlds/W06_CandyCloudKingdom/Backgrounds/landmark_tower_candycloudkingdom_01.png`.
- The recurring Rainbow Tower, a candy-bright tower on the skyline. Tower stage per doc 05 (gets closer/clearer across worlds).

### W06-PLAT — Six platforms
- Canvas 256×96 RGBA each, PPU 100, 9-slice-safe → `Assets/_JebbyJump/Art/Worlds/W06_CandyCloudKingdom/Platforms/plat_candycloudkingdom_<color>_01.png` for red/blue/green/yellow/purple/orange.
- Each rendered as candy & frosting, but the base colour MUST still read as its locked hue at 70px. Do not change silhouette/width.

### W06-FLOOR — Start ledge
- Canvas 512×128 RGB, PPU 100 → `Assets/_JebbyJump/Art/Worlds/W06_CandyCloudKingdom/Floor/floor_candycloudkingdom_01.png`. Themed ledge; keep the surface line where the cactus/Jebby stand.

### W06-HAZ — Hazard (candy_thorn)
- Canvas 1254×1254 RGBA transparent, ink-base pivot → `Assets/_JebbyJump/Art/Worlds/W06_CandyCloudKingdom/Obstacles/hazard_candy_thorn_01.png`.
- A Candy Cloud Kingdom version of the spike/thorn hazard; clearly a HAZARD (never mistakable for a safe platform); same footprint as the cactus.

### W06-DECO / VFX / UI
- Decoration set 1024² RGBA (non-colliding props) → `Assets/_JebbyJump/Art/Worlds/W06_CandyCloudKingdom/Decorations/`. Ambient VFX texture → `Assets/_JebbyJump/Art/Worlds/W06_CandyCloudKingdom/VFX/`.
- UI: world thumbnail 512² , world badge 192² , story card 1600×900, finale 1920×1080, World Gem 256², cosmetic (Frosting crown) 512² → `Assets/_JebbyJump/Art/Worlds/W06_CandyCloudKingdom/UI/` (exact names in manifest, doc 18).

## W07 — Clockwork Heights

**Shared direction:** Brass towers of turning gears. Mood: inventive, warm-industrial. Material: brass & painted metal. Palette: copper + teal (support gold + warm grey). Lighting: warm workshop light. Atmosphere: steam puffs, glints.

### W07-BG — Background
- Canvas 2400×1080 RGB opaque, PPU 100 → `Assets/_JebbyJump/Art/Worlds/W07_ClockworkHeights/Backgrounds/bg_clockworkheights_01.png`.
- Full-bleed Clockwork Heights sky/environment; must fill 16:9→20:9 when camera-locked at 1.5×; warm, no debug UI, no characters, no text.

### LANDMARK — Distant Rainbow Tower
- Canvas 900×1400 RGBA transparent → `Assets/_JebbyJump/Art/Worlds/W07_ClockworkHeights/Backgrounds/landmark_tower_clockworkheights_01.png`.
- The recurring Rainbow Tower, the tower ticks and turns just ahead. Tower stage per doc 05 (gets closer/clearer across worlds).

### W07-PLAT — Six platforms
- Canvas 256×96 RGBA each, PPU 100, 9-slice-safe → `Assets/_JebbyJump/Art/Worlds/W07_ClockworkHeights/Platforms/plat_clockworkheights_<color>_01.png` for red/blue/green/yellow/purple/orange.
- Each rendered as brass & painted metal, but the base colour MUST still read as its locked hue at 70px. Do not change silhouette/width.

### W07-FLOOR — Start ledge
- Canvas 512×128 RGB, PPU 100 → `Assets/_JebbyJump/Art/Worlds/W07_ClockworkHeights/Floor/floor_clockworkheights_01.png`. Themed ledge; keep the surface line where the cactus/Jebby stand.

### W07-HAZ — Hazard (clockwork_spike)
- Canvas 1254×1254 RGBA transparent, ink-base pivot → `Assets/_JebbyJump/Art/Worlds/W07_ClockworkHeights/Obstacles/hazard_clockwork_spike_01.png`.
- A Clockwork Heights version of the spike/thorn hazard; clearly a HAZARD (never mistakable for a safe platform); same footprint as the cactus.

### W07-DECO / VFX / UI
- Decoration set 1024² RGBA (non-colliding props) → `Assets/_JebbyJump/Art/Worlds/W07_ClockworkHeights/Decorations/`. Ambient VFX texture → `Assets/_JebbyJump/Art/Worlds/W07_ClockworkHeights/VFX/`.
- UI: world thumbnail 512² , world badge 192² , story card 1600×900, finale 1920×1080, World Gem 256², cosmetic (Gearwork goggles) 512² → `Assets/_JebbyJump/Art/Worlds/W07_ClockworkHeights/UI/` (exact names in manifest, doc 18).

## W08 — Moonlit Dreamscape

**Shared direction:** A soft night of stars and dreams. Mood: serene, magical, night. Material: moon-glass & starlight. Palette: indigo + warm moon (support violet + gold). Lighting: moonlight + starglow. Atmosphere: drifting stardust.

### W08-BG — Background
- Canvas 2400×1080 RGB opaque, PPU 100 → `Assets/_JebbyJump/Art/Worlds/W08_MoonlitDreamscape/Backgrounds/bg_moonlitdreamscape_01.png`.
- Full-bleed Moonlit Dreamscape sky/environment; must fill 16:9→20:9 when camera-locked at 1.5×; warm, no debug UI, no characters, no text.

### LANDMARK — Distant Rainbow Tower
- Canvas 900×1400 RGBA transparent → `Assets/_JebbyJump/Art/Worlds/W08_MoonlitDreamscape/Backgrounds/landmark_tower_moonlitdreamscape_01.png`.
- The recurring Rainbow Tower, the tower shines under the moon, close now. Tower stage per doc 05 (gets closer/clearer across worlds).

### W08-PLAT — Six platforms
- Canvas 256×96 RGBA each, PPU 100, 9-slice-safe → `Assets/_JebbyJump/Art/Worlds/W08_MoonlitDreamscape/Platforms/plat_moonlitdreamscape_<color>_01.png` for red/blue/green/yellow/purple/orange.
- Each rendered as moon-glass & starlight, but the base colour MUST still read as its locked hue at 70px. Do not change silhouette/width.

### W08-FLOOR — Start ledge
- Canvas 512×128 RGB, PPU 100 → `Assets/_JebbyJump/Art/Worlds/W08_MoonlitDreamscape/Floor/floor_moonlitdreamscape_01.png`. Themed ledge; keep the surface line where the cactus/Jebby stand.

### W08-HAZ — Hazard (moon_thorn)
- Canvas 1254×1254 RGBA transparent, ink-base pivot → `Assets/_JebbyJump/Art/Worlds/W08_MoonlitDreamscape/Obstacles/hazard_moon_thorn_01.png`.
- A Moonlit Dreamscape version of the spike/thorn hazard; clearly a HAZARD (never mistakable for a safe platform); same footprint as the cactus.

### W08-DECO / VFX / UI
- Decoration set 1024² RGBA (non-colliding props) → `Assets/_JebbyJump/Art/Worlds/W08_MoonlitDreamscape/Decorations/`. Ambient VFX texture → `Assets/_JebbyJump/Art/Worlds/W08_MoonlitDreamscape/VFX/`.
- UI: world thumbnail 512² , world badge 192² , story card 1600×900, finale 1920×1080, World Gem 256², cosmetic (Starlace veil) 512² → `Assets/_JebbyJump/Art/Worlds/W08_MoonlitDreamscape/UI/` (exact names in manifest, doc 18).

## W09 — Stormfire Volcano

**Shared direction:** Ember peaks under a charged sky. Mood: dramatic, warm, heroic. Material: volcanic rock & ember. Palette: ember orange + deep plum (support gold + crimson). Lighting: ember glow + lightning warmth. Atmosphere: ash motes, warm sparks.

### W09-BG — Background
- Canvas 2400×1080 RGB opaque, PPU 100 → `Assets/_JebbyJump/Art/Worlds/W09_StormfireVolcano/Backgrounds/bg_stormfirevolcano_01.png`.
- Full-bleed Stormfire Volcano sky/environment; must fill 16:9→20:9 when camera-locked at 1.5×; warm, no debug UI, no characters, no text.

### LANDMARK — Distant Rainbow Tower
- Canvas 900×1400 RGBA transparent → `Assets/_JebbyJump/Art/Worlds/W09_StormfireVolcano/Backgrounds/landmark_tower_stormfirevolcano_01.png`.
- The recurring Rainbow Tower, the tower looms against the firelit sky. Tower stage per doc 05 (gets closer/clearer across worlds).

### W09-PLAT — Six platforms
- Canvas 256×96 RGBA each, PPU 100, 9-slice-safe → `Assets/_JebbyJump/Art/Worlds/W09_StormfireVolcano/Platforms/plat_stormfirevolcano_<color>_01.png` for red/blue/green/yellow/purple/orange.
- Each rendered as volcanic rock & ember, but the base colour MUST still read as its locked hue at 70px. Do not change silhouette/width.

### W09-FLOOR — Start ledge
- Canvas 512×128 RGB, PPU 100 → `Assets/_JebbyJump/Art/Worlds/W09_StormfireVolcano/Floor/floor_stormfirevolcano_01.png`. Themed ledge; keep the surface line where the cactus/Jebby stand.

### W09-HAZ — Hazard (volcanic_ember_spike)
- Canvas 1254×1254 RGBA transparent, ink-base pivot → `Assets/_JebbyJump/Art/Worlds/W09_StormfireVolcano/Obstacles/hazard_volcanic_ember_spike_01.png`.
- A Stormfire Volcano version of the spike/thorn hazard; clearly a HAZARD (never mistakable for a safe platform); same footprint as the cactus.

### W09-DECO / VFX / UI
- Decoration set 1024² RGBA (non-colliding props) → `Assets/_JebbyJump/Art/Worlds/W09_StormfireVolcano/Decorations/`. Ambient VFX texture → `Assets/_JebbyJump/Art/Worlds/W09_StormfireVolcano/VFX/`.
- UI: world thumbnail 512² , world badge 192² , story card 1600×900, finale 1920×1080, World Gem 256², cosmetic (Emberguard mantle) 512² → `Assets/_JebbyJump/Art/Worlds/W09_StormfireVolcano/UI/` (exact names in manifest, doc 18).

## W10 — Rainbow Tower Castle

**Shared direction:** The radiant castle at journey's end. Mood: triumphant, radiant, warm. Material: rainbow crystal & gold. Palette: full warm spectrum (support gold + rose + sky). Lighting: radiant golden-hour. Atmosphere: gentle rainbow shimmer.

### W10-BG — Background
- Canvas 2400×1080 RGB opaque, PPU 100 → `Assets/_JebbyJump/Art/Worlds/W10_RainbowTowerCastle/Backgrounds/bg_rainbowtowercastle_01.png`.
- Full-bleed Rainbow Tower Castle sky/environment; must fill 16:9→20:9 when camera-locked at 1.5×; warm, no debug UI, no characters, no text.

### LANDMARK — Distant Rainbow Tower
- Canvas 900×1400 RGBA transparent → `Assets/_JebbyJump/Art/Worlds/W10_RainbowTowerCastle/Backgrounds/landmark_tower_rainbowtowercastle_01.png`.
- The recurring Rainbow Tower, you have arrived — the tower is the stage. Tower stage per doc 05 (gets closer/clearer across worlds).

### W10-PLAT — Six platforms
- Canvas 256×96 RGBA each, PPU 100, 9-slice-safe → `Assets/_JebbyJump/Art/Worlds/W10_RainbowTowerCastle/Platforms/plat_rainbowtowercastle_<color>_01.png` for red/blue/green/yellow/purple/orange.
- Each rendered as rainbow crystal & gold, but the base colour MUST still read as its locked hue at 70px. Do not change silhouette/width.

### W10-FLOOR — Start ledge
- Canvas 512×128 RGB, PPU 100 → `Assets/_JebbyJump/Art/Worlds/W10_RainbowTowerCastle/Floor/floor_rainbowtowercastle_01.png`. Themed ledge; keep the surface line where the cactus/Jebby stand.

### W10-HAZ — Hazard (rainbow_ward_spike)
- Canvas 1254×1254 RGBA transparent, ink-base pivot → `Assets/_JebbyJump/Art/Worlds/W10_RainbowTowerCastle/Obstacles/hazard_rainbow_ward_spike_01.png`.
- A Rainbow Tower Castle version of the spike/thorn hazard; clearly a HAZARD (never mistakable for a safe platform); same footprint as the cactus.

### W10-DECO / VFX / UI
- Decoration set 1024² RGBA (non-colliding props) → `Assets/_JebbyJump/Art/Worlds/W10_RainbowTowerCastle/Decorations/`. Ambient VFX texture → `Assets/_JebbyJump/Art/Worlds/W10_RainbowTowerCastle/VFX/`.
- UI: world thumbnail 512² , world badge 192² , story card 1600×900, finale 1920×1080, World Gem 256², cosmetic (Radiant heirloom set) 512² → `Assets/_JebbyJump/Art/Worlds/W10_RainbowTowerCastle/UI/` (exact names in manifest, doc 18).
