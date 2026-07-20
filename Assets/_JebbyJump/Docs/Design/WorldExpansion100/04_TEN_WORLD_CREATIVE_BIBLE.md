# 04 — Ten-World Creative Bible

Status: `PROPOSED` (roster CONFIRMED). Each world is a recognisable art family; the six
semantic gameplay colours (`#E63838/#3878E6/#38BF59/#FAD12E/#9938E6/#F28C26`) keep their
identity in every world — only the *material treatment* changes. No coloured lighting may make
two gameplay colours look alike. Difficulty maps to world number (see docs 12–14).

## Colour-identity contract (applies to all worlds)

| Semantic | Locked base | Treatment varies by world | Accessibility cue | Contrast rule |
|---|---|---|---|---|
| Red | `#E63838` | material only (ice/leaf/lava/etc.) | Memory Cue glyph | must stay clearly red |
| Blue | `#3878E6` | material only | Memory Cue glyph | never merge with green |
| Green | `#38BF59` | material only | Memory Cue glyph | never merge with blue |
| Yellow | `#FAD12E` | material only | Memory Cue glyph | never merge with orange |
| Purple | `#9938E6` | material only | Memory Cue glyph | keep distinct from blue |
| Orange | `#F28C26` | material only | Memory Cue glyph | never merge with yellow |

Prohibited everywhere: a 7th gameplay colour; hazard art that reads as a safe platform;
coloured light that lowers colour separation; anything scary/violent (kids-first).

## W01 — Cloud Meadow

- **World id:** `W01` · slug `CloudMeadow` · number `1`
- **One-line fantasy:** A gentle sky-meadow of drifting cloudpuffs.
- **Visual mood:** soft, welcoming, pastel
- **Primary material:** cloud & meadow grass
- **Background palette:** warm sky blue + cream  ·  **Support palette:** peach + mint
- **Lighting:** soft midday, top-left  ·  **Weather/atmosphere:** calm breeze, drifting fluff
- **Foreground/midground decoration:** themed props (non-colliding) — see art manifest `W01` deco row
- **Distant landmark (Rainbow Tower):** a tiny far-off glimmer on the horizon  (progression stage in doc 05)
- **Platform material identity:** six locked colours rendered as cloud & meadow grass
- **Floor identity:** themed start ledge (`floor_cloudmeadow_01`), collider unchanged
- **Hazard identity:** `cloud_thorn` — visual reskin of the cactus behaviour (identical timing/damage/collision)
- **Story-card concept:** "Welcome to the sky!" (doc 06)
- **Finale concept:** Cloud Meadow Crown — set-piece composition + landmark focus, no new mechanic (doc 15)
- **World collectible:** Cloud Meadow World Gem trophy (non-spendable)
- **Cosmetic reward:** Cloudpuff cape (wardrobe unlock on world mastery)
- **Ambient VFX:** calm breeze, drifting fluff (texture in `W01/VFX`)
- **Audio/music (recommendation, PROPOSED):** a soft theme; no new audio SDK
- **Mobile-readability risks:** ensure cloud & meadow grass treatment keeps 6-colour separation at 70px
- **Accessibility risks:** verify Memory Cues legible on warm sky blue + cream background
- **Performance risks:** single camera-locked bg (no parallax at launch); texture ≤2048
- **Prohibited here:** anything that dims colour separation; scary imagery

## W02 — Enchanted Forest

- **World id:** `W02` · slug `EnchantedForest` · number `2`
- **One-line fantasy:** A glowing woodland of mossy boughs.
- **Visual mood:** cozy, magical, verdant
- **Primary material:** mossy wood & leaves
- **Background palette:** deep green + amber  ·  **Support palette:** teal + gold
- **Lighting:** dappled canopy light  ·  **Weather/atmosphere:** floating spores, fireflies
- **Foreground/midground decoration:** themed props (non-colliding) — see art manifest `W02` deco row
- **Distant landmark (Rainbow Tower):** a distant spire seen through the trees  (progression stage in doc 05)
- **Platform material identity:** six locked colours rendered as mossy wood & leaves
- **Floor identity:** themed start ledge (`floor_enchantedforest_01`), collider unchanged
- **Hazard identity:** `forest_thorn_bloom` — visual reskin of the cactus behaviour (identical timing/damage/collision)
- **Story-card concept:** "Into the whispering woods." (doc 06)
- **Finale concept:** Enchanted Forest Crown — set-piece composition + landmark focus, no new mechanic (doc 15)
- **World collectible:** Enchanted Forest World Gem trophy (non-spendable)
- **Cosmetic reward:** Leafcrown hood (wardrobe unlock on world mastery)
- **Ambient VFX:** floating spores, fireflies (texture in `W02/VFX`)
- **Audio/music (recommendation, PROPOSED):** a cozy theme; no new audio SDK
- **Mobile-readability risks:** ensure mossy wood & leaves treatment keeps 6-colour separation at 70px
- **Accessibility risks:** verify Memory Cues legible on deep green + amber background
- **Performance risks:** single camera-locked bg (no parallax at launch); texture ≤2048
- **Prohibited here:** anything that dims colour separation; scary imagery

## W03 — Crystal Caves

- **World id:** `W03` · slug `CrystalCaves` · number `3`
- **One-line fantasy:** Luminous caverns of singing crystal.
- **Visual mood:** wondrous, cool-but-warm-lit
- **Primary material:** faceted crystal
- **Background palette:** violet + warm torchglow  ·  **Support palette:** cyan + rose
- **Lighting:** crystal glow + torchlight  ·  **Weather/atmosphere:** gentle sparkle motes
- **Foreground/midground decoration:** themed props (non-colliding) — see art manifest `W03` deco row
- **Distant landmark (Rainbow Tower):** the tower gleams down a crystal shaft  (progression stage in doc 05)
- **Platform material identity:** six locked colours rendered as faceted crystal
- **Floor identity:** themed start ledge (`floor_crystalcaves_01`), collider unchanged
- **Hazard identity:** `crystal_spike_cluster` — visual reskin of the cactus behaviour (identical timing/damage/collision)
- **Story-card concept:** "Follow the glimmer down." (doc 06)
- **Finale concept:** Crystal Caves Crown — set-piece composition + landmark focus, no new mechanic (doc 15)
- **World collectible:** Crystal Caves World Gem trophy (non-spendable)
- **Cosmetic reward:** Geode pauldrons (wardrobe unlock on world mastery)
- **Ambient VFX:** gentle sparkle motes (texture in `W03/VFX`)
- **Audio/music (recommendation, PROPOSED):** a wondrous theme; no new audio SDK
- **Mobile-readability risks:** ensure faceted crystal treatment keeps 6-colour separation at 70px
- **Accessibility risks:** verify Memory Cues legible on violet + warm torchglow background
- **Performance risks:** single camera-locked bg (no parallax at launch); texture ≤2048
- **Prohibited here:** anything that dims colour separation; scary imagery

## W04 — Sunshine Desert

- **World id:** `W04` · slug `SunshineDesert` · number `4`
- **One-line fantasy:** Warm dunes under a friendly sun.
- **Visual mood:** bright, adventurous, warm
- **Primary material:** sun-baked sandstone
- **Background palette:** golden sand + sky  ·  **Support palette:** coral + turquoise
- **Lighting:** strong warm sun, high  ·  **Weather/atmosphere:** heat shimmer, soft wind
- **Foreground/midground decoration:** themed props (non-colliding) — see art manifest `W04` deco row
- **Distant landmark (Rainbow Tower):** the tower rises beyond the far dunes  (progression stage in doc 05)
- **Platform material identity:** six locked colours rendered as sun-baked sandstone
- **Floor identity:** themed start ledge (`floor_sunshinedesert_01`), collider unchanged
- **Hazard identity:** `desert_cactus` — visual reskin of the cactus behaviour (identical timing/damage/collision)
- **Story-card concept:** "Over the golden dunes." (doc 06)
- **Finale concept:** Sunshine Desert Crown — set-piece composition + landmark focus, no new mechanic (doc 15)
- **World collectible:** Sunshine Desert World Gem trophy (non-spendable)
- **Cosmetic reward:** Sunwrap scarf (wardrobe unlock on world mastery)
- **Ambient VFX:** heat shimmer, soft wind (texture in `W04/VFX`)
- **Audio/music (recommendation, PROPOSED):** a bright theme; no new audio SDK
- **Mobile-readability risks:** ensure sun-baked sandstone treatment keeps 6-colour separation at 70px
- **Accessibility risks:** verify Memory Cues legible on golden sand + sky background
- **Performance risks:** single camera-locked bg (no parallax at launch); texture ≤2048
- **Prohibited here:** anything that dims colour separation; scary imagery

## W05 — Ocean Sky

- **World id:** `W05` · slug `OceanSky` · number `5`
- **One-line fantasy:** Floating reefs where sea meets sky.
- **Visual mood:** dreamy, fresh, buoyant
- **Primary material:** coral & sea-glass
- **Background palette:** aqua + warm coral  ·  **Support palette:** seafoam + sunset
- **Lighting:** bright aquatic light  ·  **Weather/atmosphere:** bubbles, drifting spray
- **Foreground/midground decoration:** themed props (non-colliding) — see art manifest `W05` deco row
- **Distant landmark (Rainbow Tower):** the tower stands on a far sky-island  (progression stage in doc 05)
- **Platform material identity:** six locked colours rendered as coral & sea-glass
- **Floor identity:** themed start ledge (`floor_oceansky_01`), collider unchanged
- **Hazard identity:** `coral_spine` — visual reskin of the cactus behaviour (identical timing/damage/collision)
- **Story-card concept:** "Where the sea floats up." (doc 06)
- **Finale concept:** Ocean Sky Crown — set-piece composition + landmark focus, no new mechanic (doc 15)
- **World collectible:** Ocean Sky World Gem trophy (non-spendable)
- **Cosmetic reward:** Tidal fin cloak (wardrobe unlock on world mastery)
- **Ambient VFX:** bubbles, drifting spray (texture in `W05/VFX`)
- **Audio/music (recommendation, PROPOSED):** a dreamy theme; no new audio SDK
- **Mobile-readability risks:** ensure coral & sea-glass treatment keeps 6-colour separation at 70px
- **Accessibility risks:** verify Memory Cues legible on aqua + warm coral background
- **Performance risks:** single camera-locked bg (no parallax at launch); texture ≤2048
- **Prohibited here:** anything that dims colour separation; scary imagery

## W06 — Candy Cloud Kingdom

- **World id:** `W06` · slug `CandyCloudKingdom` · number `6`
- **One-line fantasy:** A sweet kingdom of frosting clouds.
- **Visual mood:** playful, sugary, joyful
- **Primary material:** candy & frosting
- **Background palette:** pink + cream + mint  ·  **Support palette:** lilac + lemon
- **Lighting:** soft candy glow  ·  **Weather/atmosphere:** sugar sparkle, sprinkles
- **Foreground/midground decoration:** themed props (non-colliding) — see art manifest `W06` deco row
- **Distant landmark (Rainbow Tower):** a candy-bright tower on the skyline  (progression stage in doc 05)
- **Platform material identity:** six locked colours rendered as candy & frosting
- **Floor identity:** themed start ledge (`floor_candycloudkingdom_01`), collider unchanged
- **Hazard identity:** `candy_thorn` — visual reskin of the cactus behaviour (identical timing/damage/collision)
- **Story-card concept:** "A kingdom made of sweets!" (doc 06)
- **Finale concept:** Candy Cloud Kingdom Crown — set-piece composition + landmark focus, no new mechanic (doc 15)
- **World collectible:** Candy Cloud Kingdom World Gem trophy (non-spendable)
- **Cosmetic reward:** Frosting crown (wardrobe unlock on world mastery)
- **Ambient VFX:** sugar sparkle, sprinkles (texture in `W06/VFX`)
- **Audio/music (recommendation, PROPOSED):** a playful theme; no new audio SDK
- **Mobile-readability risks:** ensure candy & frosting treatment keeps 6-colour separation at 70px
- **Accessibility risks:** verify Memory Cues legible on pink + cream + mint background
- **Performance risks:** single camera-locked bg (no parallax at launch); texture ≤2048
- **Prohibited here:** anything that dims colour separation; scary imagery

## W07 — Clockwork Heights

- **World id:** `W07` · slug `ClockworkHeights` · number `7`
- **One-line fantasy:** Brass towers of turning gears.
- **Visual mood:** inventive, warm-industrial
- **Primary material:** brass & painted metal
- **Background palette:** copper + teal  ·  **Support palette:** gold + warm grey
- **Lighting:** warm workshop light  ·  **Weather/atmosphere:** steam puffs, glints
- **Foreground/midground decoration:** themed props (non-colliding) — see art manifest `W07` deco row
- **Distant landmark (Rainbow Tower):** the tower ticks and turns just ahead  (progression stage in doc 05)
- **Platform material identity:** six locked colours rendered as brass & painted metal
- **Floor identity:** themed start ledge (`floor_clockworkheights_01`), collider unchanged
- **Hazard identity:** `clockwork_spike` — visual reskin of the cactus behaviour (identical timing/damage/collision)
- **Story-card concept:** "Mind the gears." (doc 06)
- **Finale concept:** Clockwork Heights Crown — set-piece composition + landmark focus, no new mechanic (doc 15)
- **World collectible:** Clockwork Heights World Gem trophy (non-spendable)
- **Cosmetic reward:** Gearwork goggles (wardrobe unlock on world mastery)
- **Ambient VFX:** steam puffs, glints (texture in `W07/VFX`)
- **Audio/music (recommendation, PROPOSED):** a inventive theme; no new audio SDK
- **Mobile-readability risks:** ensure brass & painted metal treatment keeps 6-colour separation at 70px
- **Accessibility risks:** verify Memory Cues legible on copper + teal background
- **Performance risks:** single camera-locked bg (no parallax at launch); texture ≤2048
- **Prohibited here:** anything that dims colour separation; scary imagery

## W08 — Moonlit Dreamscape

- **World id:** `W08` · slug `MoonlitDreamscape` · number `8`
- **One-line fantasy:** A soft night of stars and dreams.
- **Visual mood:** serene, magical, night
- **Primary material:** moon-glass & starlight
- **Background palette:** indigo + warm moon  ·  **Support palette:** violet + gold
- **Lighting:** moonlight + starglow  ·  **Weather/atmosphere:** drifting stardust
- **Foreground/midground decoration:** themed props (non-colliding) — see art manifest `W08` deco row
- **Distant landmark (Rainbow Tower):** the tower shines under the moon, close now  (progression stage in doc 05)
- **Platform material identity:** six locked colours rendered as moon-glass & starlight
- **Floor identity:** themed start ledge (`floor_moonlitdreamscape_01`), collider unchanged
- **Hazard identity:** `moon_thorn` — visual reskin of the cactus behaviour (identical timing/damage/collision)
- **Story-card concept:** "Dream among the stars." (doc 06)
- **Finale concept:** Moonlit Dreamscape Crown — set-piece composition + landmark focus, no new mechanic (doc 15)
- **World collectible:** Moonlit Dreamscape World Gem trophy (non-spendable)
- **Cosmetic reward:** Starlace veil (wardrobe unlock on world mastery)
- **Ambient VFX:** drifting stardust (texture in `W08/VFX`)
- **Audio/music (recommendation, PROPOSED):** a serene theme; no new audio SDK
- **Mobile-readability risks:** ensure moon-glass & starlight treatment keeps 6-colour separation at 70px
- **Accessibility risks:** verify Memory Cues legible on indigo + warm moon background
- **Performance risks:** single camera-locked bg (no parallax at launch); texture ≤2048
- **Prohibited here:** anything that dims colour separation; scary imagery

## W09 — Stormfire Volcano

- **World id:** `W09` · slug `StormfireVolcano` · number `9`
- **One-line fantasy:** Ember peaks under a charged sky.
- **Visual mood:** dramatic, warm, heroic
- **Primary material:** volcanic rock & ember
- **Background palette:** ember orange + deep plum  ·  **Support palette:** gold + crimson
- **Lighting:** ember glow + lightning warmth  ·  **Weather/atmosphere:** ash motes, warm sparks
- **Foreground/midground decoration:** themed props (non-colliding) — see art manifest `W09` deco row
- **Distant landmark (Rainbow Tower):** the tower looms against the firelit sky  (progression stage in doc 05)
- **Platform material identity:** six locked colours rendered as volcanic rock & ember
- **Floor identity:** themed start ledge (`floor_stormfirevolcano_01`), collider unchanged
- **Hazard identity:** `volcanic_ember_spike` — visual reskin of the cactus behaviour (identical timing/damage/collision)
- **Story-card concept:** "Brave the emberpeaks." (doc 06)
- **Finale concept:** Stormfire Volcano Crown — set-piece composition + landmark focus, no new mechanic (doc 15)
- **World collectible:** Stormfire Volcano World Gem trophy (non-spendable)
- **Cosmetic reward:** Emberguard mantle (wardrobe unlock on world mastery)
- **Ambient VFX:** ash motes, warm sparks (texture in `W09/VFX`)
- **Audio/music (recommendation, PROPOSED):** a dramatic theme; no new audio SDK
- **Mobile-readability risks:** ensure volcanic rock & ember treatment keeps 6-colour separation at 70px
- **Accessibility risks:** verify Memory Cues legible on ember orange + deep plum background
- **Performance risks:** single camera-locked bg (no parallax at launch); texture ≤2048
- **Prohibited here:** anything that dims colour separation; scary imagery

## W10 — Rainbow Tower Castle

- **World id:** `W10` · slug `RainbowTowerCastle` · number `10`
- **One-line fantasy:** The radiant castle at journey's end.
- **Visual mood:** triumphant, radiant, warm
- **Primary material:** rainbow crystal & gold
- **Background palette:** full warm spectrum  ·  **Support palette:** gold + rose + sky
- **Lighting:** radiant golden-hour  ·  **Weather/atmosphere:** gentle rainbow shimmer
- **Foreground/midground decoration:** themed props (non-colliding) — see art manifest `W10` deco row
- **Distant landmark (Rainbow Tower):** you have arrived — the tower is the stage  (progression stage in doc 05)
- **Platform material identity:** six locked colours rendered as rainbow crystal & gold
- **Floor identity:** themed start ledge (`floor_rainbowtowercastle_01`), collider unchanged
- **Hazard identity:** `rainbow_ward_spike` — visual reskin of the cactus behaviour (identical timing/damage/collision)
- **Story-card concept:** "Home of the rainbow." (doc 06)
- **Finale concept:** Rainbow Tower Castle Crown — set-piece composition + landmark focus, no new mechanic (doc 15)
- **World collectible:** Rainbow Tower Castle World Gem trophy (non-spendable)
- **Cosmetic reward:** Radiant heirloom set (wardrobe unlock on world mastery)
- **Ambient VFX:** gentle rainbow shimmer (texture in `W10/VFX`)
- **Audio/music (recommendation, PROPOSED):** a triumphant theme; no new audio SDK
- **Mobile-readability risks:** ensure rainbow crystal & gold treatment keeps 6-colour separation at 70px
- **Accessibility risks:** verify Memory Cues legible on full warm spectrum background
- **Performance risks:** single camera-locked bg (no parallax at launch); texture ≤2048
- **Prohibited here:** anything that dims colour separation; scary imagery
