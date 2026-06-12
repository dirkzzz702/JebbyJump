# Jebby Jump - Cosmetic Wardrobe Design Spec v0.1

Status: DESIGN SPEC (P8). The local foundation was implemented in **P9** (see
note below). No art swap / currency / shop / ads / backend. All star
thresholds here remain STRICT PLACEHOLDERS (see section 8).

> **P9 update (implemented):** the local wardrobe foundation shipped -
> `JebbyJump.Wardrobe.Runtime` (`CosmeticItemDefinition`, `WardrobeCatalog`
> with the 5 outfits, `WardrobeStore` persisting only the equipped id, pure
> Stars-gated `WardrobeUnlockService`), a text-only Wardrobe panel from a new
> Main Menu Wardrobe button, local wardrobe analytics, Reset Wardrobe, and
> pure tests. Stars **gate but are NOT consumed**; thresholds 0/8/15/22/30
> are PLACEHOLDERS. **No art/sprite swap, no PlayerAnimator coupling, no
> shop/Spark Coins/Rainbow Gems currency/ads/backend.** Outfits have no
> gameplay effect. Manual visual QA of the panel remains DEFERRED / NOT
> VERIFIED. Section 8's "Recommended first wardrobe implementation after P8 /
> future P9A" is now realized by P9.
>
> **P10 update (docs only):** a wardrobe visual-QA checklist + art / sprite-
> swap readiness plan was added at
> `Assets/_JebbyJump/Docs/QA/Jebby_Jump_Wardrobe_Visual_QA_and_Art_Readiness_Plan_v0.1.md`
> (records actual P9 UI copy + the real PlayerAnimator contract; required
> outfit states idle/run/jump/fall/land/hurt/victory; sprite-swap options).
> No art/sprite swap implemented; manual visual QA remains deferred.
>
> **P11 update (implemented):** the equipped outfit id now has a safe visual
> application path. A new `JebbyJump.Wardrobe.Visual` asmdef adds
> `OutfitVisualDefinition`, `OutfitVisualCatalog` (resolver), and a
> `PlayerOutfitVisualController` wired onto the Jebby prefab (Game.unity is not
> edited - the scene inherits the prefab). On Start it applies the equipped
> outfit; every outfit currently resolves to `HasVisualOverride=false` (no art
> yet), so the apply is a **no-op** and Jebby looks unchanged. The default
> outfit maps to the current visuals. Final outfit art is still required;
> sprite-swap is not visually meaningful until art exists. A future
> `AnimatorOverrideController` / sprite-sheet plugs into the resolver without
> changing the controller. No gameplay effect; no shop/Spark Coins/Rainbow Gems
> currency/ads/backend. Manual visual QA remains DEFERRED / NOT VERIFIED.
>
> **P12 update (docs + test seam):** the First Outfit Art Asset Request Pack for
> **Forest Cavalier** was added at
> `Assets/_JebbyJump/Docs/Art/Jebby_Jump_First_Outfit_Art_Asset_Request_Pack_v0.1.md`
> (identity guardrails, the 7 animation states, actual sprite import settings,
> naming/folder conventions, and the AnimatorOverrideController pipeline over
> `JebbyAnimator`). A pure-static `OutfitVisualApplier` seam was added and
> `ApplyOutfit` delegates to it with unchanged behavior; 4 tests prove override
> assignment (80/80 pass). No final art imported; the catalog stays no-op for
> all outfits; outfits remain cosmetic-only (Art Bible "possible later" perks
> stay deferred). Manual visual QA remains DEFERRED / NOT VERIFIED.
>
> **P13 update (Mode A - blocked on art):** Forest Cavalier art intake was
> attempted; no art exists anywhere and none was generated, so nothing was
> imported and no override was wired - the catalog stays no-op and
> `forest_cavalier` still looks like default Jebby. A read-only editor QA
> gate was added for future intake (`Jebby Jump/QA/Check Outfit Sprite
> Alpha`). Next step: provide/generate the 7 state sprites per the P12 pack.
> Outfits remain cosmetic-only; manual visual QA remains DEFERRED / NOT
> VERIFIED.
>
> **P13 Mode B update (art imported, catalog expanded):** the user supplied 7
> complete outfit sprite sets (palette-transfer PROTOTYPE art; 49/49 QA-gate
> PASS). Each outfit now has 7 clips + an `aoc_jebby_<id>` override controller
> (base `JebbyAnimator`) registered in the new `OutfitVisualLibrary` SO, wired
> into the Jebby prefab - equipping a non-default outfit visibly swaps Jebby's
> sprites at spawn. With approval the runtime catalog expanded **5 -> 8**:
> rookie_page (4), crimson_hero (12), pastel_prince (26) - STRICT PLACEHOLDER
> thresholds, original five untouched; section 6's "initial set" is therefore
> superseded by the full 8-design board as the runtime set. Outfits remain
> cosmetic-only (no gameplay effect); manual visual QA of the rendered outfits
> remains DEFERRED / NOT VERIFIED.
>
> **P14 update (stabilization):** asset-integrity tests now pin the real
> `OutfitVisualLibrary` entries (all 7 non-default outfits; default
> intentionally no-op), each override controller's `JebbyAnimator` base + 7
> clip overrides, and the Jebby prefab wiring (94/94 tests). The wardrobe
> panel was verified already ScrollRect-based and data-driven, so 8+ rows
> scroll without UI changes. Spawn-only apply semantics documented (no live
> mid-scene re-sync). Manual visual QA remains DEFERRED / NOT VERIFIED.

---

## 1. Purpose

Define what the cosmetic wardrobe is, how outfits are unlocked, and how it
relates to the existing Stars reward foundation - before any code is written.
This is a product/design artifact only. It adds no cosmetics, currencies,
stores, screens, or art to the game now.

It answers: which outfits exist first, how they unlock, how ownership/equip/
preview should behave later, how Stars relate to cosmetics, what analytics
will be needed, what art is required, and what must stay out of scope.

## 2. Current game / reward context

- 10-level vertical slice (P3 baseline), mobile-first.
- **Stars** (P7A-P7E): local, per-level, best-only, 0..3 (S/A=3, B=2, C=1,
  completed-no-rank=1, failure=0). Shown as `Stars: N/3` on result panel +
  Level Select. `StarRewardStore` (per-level PlayerPrefs keys) + on-demand
  `GetTotalStars(levelCount)`. Reward analytics are local/debug-only with
  pinned wire names. Stars are a **mastery record, not spendable currency**.
- No wardrobe, shop, Spark Coins, Rainbow Gems currency, ads, or backend exist.
- Manual visual QA (P5B-P5F, P7A, P7B) and P4B playtest/balance remain
  **DEFERRED / NOT VERIFIED**.

## 3. Existing assumptions from GDD / Roadmap / Art Bible

- **GDD section 15:** wardrobe is **cosmetic only**; outfits listed: Classic
  Cavalier, Forest Cavalier, Sunshine Knight, Aqua Knight, Silver Dreamer;
  "do not add gameplay stats to outfits."
- **Art Bible section 2.2:** outfit board (8 candidates): Classic Cavalier, Forest
  Cavalier, Silver Dreamer, Sunshine Knight, Crimson Hero, Aqua Knight,
  Rookie Page, Pastel Prince - "**not separate characters**; future outfits
  for the same Jebby."
- **Art Bible section 2.3 Design Lock Rule** (authoritative identity guardrails):
  - *May change:* cape color, clothing theme, boots, decorative accessories,
    VFX color, small hat / trim / seasonal detail.
  - *Must NOT change:* Jebby's face identity, chibi proportions, warm eyes,
    long ear-feather / hair silhouette, **rainbow-gem motif**, friendly brave
    personality, core Cavalier-inspired softness.
- **Art Bible section 10:** wardrobe is post-MVP (V2: 1-2 unlockable outfits; V3:
  full "Jebby's Wardrobe"); mentions a future "outfit fragments" unlock idea.
- **Roadmap:** "Wardrobe cosmetic only"; existing phase "P8 - Cosmetic
  Wardrobe Foundation"; equipped skills are gameplay mechanics, not paid power.
- **Reconciliations:** this spec uses **"Classic Color Knight"** as the
  wardrobe display name for the default outfit and maps it to the Art Bible/
  GDD default **"Classic Cavalier"** (open question 19.1). The Art Bible
  "outfit fragments" idea is recorded as a **deferred alternative** (section
  7); the near-term recommendation is deterministic Stars-gating (no loot).

## 4. Wardrobe design principles

```text
cosmetic-only; never affects gameplay power, rank, time, or progression
child-friendly and low-pressure
no loot boxes / no random draws / no gambling
no paid pressure / no forced ads / no FOMO / no streak pressure
preserve Jebby's identity (Art Bible Design Lock Rule)
mobile-readable at small size; animation-compatible
honest, simple wording a young player understands
parents never feel tricked or pressured
```

## 5. Cosmetic item taxonomy

| Category | Example | P8 recommendation |
|---|---|---|
| Outfits / skins | Forest Cavalier | **Start here (first)** |
| Cape variants | green/gold/teal cape | Later |
| Boot variants | seasonal boots | Later |
| Trail effects | sparkle trail | Later |
| Badge / emblem variants | alt rainbow badge | Later |
| Background themes | cloud / desert bg | Later |
| Button / UI frame skins | rounded frame | Later |

P8 recommends **Outfits only** first: clearest player value, easy to
understand, cosmetic-first, no gameplay impact, good future reward target.
Trails / UI skins / backgrounds are deferred.

## 6. Initial outfit set

Reconciled with GDD/Art Bible. Identity guardrails per outfit follow the section 2.3
"must not change" list (face, proportions, eyes, ear-feather silhouette,
rainbow-gem motif, softness). Suggested unlocks are **PLACEHOLDER** (section 8).

| Outfit | Theme | Visual changes (may change) | Identity guardrails (must not change) | Suggested unlock (PLACEHOLDER) | Risk |
|---|---|---|---|---|---|
| Classic Color Knight (= GDD/Art Bible "Classic Cavalier") | Default Jebby | brown/white base, blue cape, rainbow badge | all section 2.3 must-not-change items | Always unlocked | None |
| Forest Cavalier | Woodland | green cape, leaf accent, warm-brown boots | face/ears/eyes/silhouette/gem motif | Stars-gated (PLACEHOLDER) | Low |
| Sunshine Knight | Sunny | golden cape, sunny badge trim, bright accents | keep gentle, avoid premium-pressure look | Stars-gated (PLACEHOLDER) | Low |
| Aqua Knight | Water | teal/blue cape, water-sparkle VFX | keep silhouette readable | Stars-gated (PLACEHOLDER) | Low-Med (VFX readability) |
| Silver Dreamer | Night/dream | silver/lavender theme, soft glow | avoid overly dark/low-contrast | Stars-gated (PLACEHOLDER) | Med (contrast on mobile) |

No art is generated or imported in P8. Broader board names (Crimson Hero,
Rookie Page, Pastel Prince) remain a future pool, not in the initial set.

## 7. Unlock model options

**Option A - Stars unlock outfits directly.** Total Stars reaching a
threshold unlocks an outfit (Stars are NOT consumed). Pros: simple,
child-friendly, no wallet/shop, reuses P7 foundation. Cons: less flexible,
harder to price many cosmetics, needs enough levels/Stars.

**Option B - Spark Coins purchase.** Earn soft currency from clears/best
improvements; spend on cosmetics. Pros: flexible, scales to many cosmetics.
Cons: adds economy + anti-farming complexity, can feel grindy, not needed yet.

**Option C - Hybrid.** Stars unlock tiers; Spark Coins buy options within
unlocked tiers. Pros: balanced long-term. Cons: most complex; premature
without playtest/analytics.

**Deferred alternative (Art Bible section 10) - outfit fragments.** Collect
deterministic fragments to complete an outfit. Acceptable *only if* fully
deterministic (no random draws / no gambling). Deferred; not recommended
near-term.

## 8. Recommended unlock model

**Recommended first wardrobe implementation after P8 / future P9A: Stars-gated
direct outfit unlocks (Option A).** Reasons: no paid currency, no wallet, no
shop, simple for children, reuses the existing Stars foundation, cosmetic-first,
low risk.

Rules:
```text
Stars remain a mastery record, NOT spendable currency.
Star thresholds UNLOCK cosmetics but do NOT consume Stars.
Unlock is deterministic (no random / no loot).
```

Example thresholds - **STRICT PLACEHOLDERS, NOT TUNED**:
```text
Classic Color Knight : always unlocked
Forest Cavalier      : 8 total Stars   (PLACEHOLDER)
Sunshine Knight      : 15 total Stars  (PLACEHOLDER)
Aqua Knight          : 22 total Stars  (PLACEHOLDER)
Silver Dreamer       : 30 total Stars  (PLACEHOLDER)
```
> All thresholds above are STRICT PLACEHOLDERS pending P4B playtest, final
> level count, and reward/balance review. Do not treat as final or implement
> as tuned values.

## 9. Stars / Spark Coins / Rainbow Gems relationship

```text
Stars (implemented):
- local per-level mastery record, 0..3, never consumed, not paid
- MAY gate cosmetic unlocks later (Option A) via total-Star thresholds

Spark Coins (NOT implemented):
- optional future earned soft currency
- only potentially useful if the wardrobe later has many cosmetics
- NOT implemented in P8; NOT required for the recommended model

Rainbow Gems CURRENCY (NOT implemented):
- long-term GDD/Roadmap premium/launch-era currency
- deferred; NOT near-term; NOT implemented in P8
```

> **Important distinction:** the **rainbow-gem visual motif** (badge / color
> theme) is an ALLOWED, required part of Jebby's identity and art (Art Bible
> section 2.3). The **Rainbow Gems currency** is a separate, DEFERRED monetization
> concept and is NOT implemented. The motif staying in the art does not imply
> any currency.

Explicit rule: **No paid currency, no Spark Coins, no Rainbow Gems currency,
no shop in P8** (or in the recommended near-term wardrobe model).

## 10. Preview / equip UX concept (design only)

```text
Main Menu -> Wardrobe
Wardrobe shows a Jebby preview (left/top) + outfit list (right/bottom).
Each outfit card: name, lock/unlock state, unlock requirement, Preview, Equip.
```
States: Owned+equipped / Owned+not-equipped / Locked+requirement-visible /
Locked+coming-soon. Rules: preview does not imply ownership; Equip only for
owned outfits; locked outfits show honest requirements; no buy/countdown/
ad-gated pressure.

## 11. Future wardrobe screen requirements (design only)

```text
entry point: a Wardrobe button on Main Menu (future)
layout: preview pane + scrollable outfit list, mobile portrait first
honest lock/requirement display; obvious equipped indicator
Back returns to Main Menu; Equip applies an owned outfit
no shop framing, no currency balance header (none exists)
```

## 12. Future data model outline (proposal only - NOT implemented)

```csharp
CosmeticItemDefinition
{
    string Id;
    string DisplayName;
    CosmeticCategory Category;   // Outfit first; others later
    string Description;
    int    RequiredStars;        // PLACEHOLDER values per section 8
    bool   AlwaysUnlocked;
    string PreviewAssetKey;
}
```
Future local state: owned cosmetic ids + equipped outfit id. Proposed future
PlayerPrefs keys (NOT implemented in P8): `jebby.wardrobe.ownedOutfits`,
`jebby.wardrobe.equippedOutfit`. Keep it simple/local; avoid backend-ready
complexity. (Ownership may be derived from Stars for Option A; an explicit
owned-set becomes useful if Spark Coins/purchases ever arrive.)

## 13. Future analytics event requirements (design only - NOT implemented)

Extends the P6A/P6B catalog via `IAnalyticsSink` later; snake_case, primitive
params, no PII; local-first like all current events.

| Event (future) | Trigger | Key params | Why | Privacy |
|---|---|---|---|---|
| `wardrobe_opened` | Wardrobe screen opened | source | reach/engagement | no PII |
| `cosmetic_previewed` | preview an outfit | cosmetic_id, cosmetic_category | interest | no PII |
| `cosmetic_unlocked` | outfit becomes unlocked | cosmetic_id, unlock_method, required_stars, current_stars | unlock funnel | no PII |
| `cosmetic_equipped` | outfit equipped | cosmetic_id, is_owned | preference | no PII |
| `cosmetic_unlock_failed` | equip/unlock attempt blocked | cosmetic_id, required_stars, current_stars | friction | no PII |

Not implemented in P8.

## 14. Art production requirements (design only - no art made/imported in P8)

```text
transparent PNG / sprite; no white/opaque background
consistent Jebby proportions and pivot; compatible bounds
full animation-set compatibility: idle / jump / fall / run / land / hurt /
  victory (matches PlayerAnimator)
same animation naming + sorting layers; consistent outline/shading
mobile readability at small size; clear silhouette; no style drift
follows Art Bible Design Lock Rule (section 3)
```

## 15. Child-safety / parent-trust guardrails

Avoid: loot boxes, random draws, paid pressure, forced ads, daily-streak
pressure, FOMO, confusing purchase language, power-selling, cosmetics implying
stronger gameplay, violent/scary cosmetics, weapons-as-appeal.

Recommended wording: "Unlocked with Stars", "Earn Stars by completing levels",
"Preview", "Equip", "Coming later". Avoid: "limited time", "exclusive deal",
"buy now", "premium only", "watch ad to unlock".

## 16. Deferred decisions

```text
whether/when Spark Coins are introduced (and whether wardrobe needs them)
whether/when Rainbow Gems currency is introduced
fragments-vs-Stars unlock approach
exact star thresholds and outfit count at first launch
later cosmetic categories (trails, UI skins, backgrounds)
any purchasable cosmetics
```
Pending: P4B playtest, analytics, level count, parent/child UX review.

## 17. Implementation roadmap

```text
P8 (this doc)  - cosmetic wardrobe design spec, docs only
Future P9A     - Wardrobe Data Model + Local Ownership Store (recommended
                 first implementation step), Stars-gated direct unlocks
Future P9B     - Wardrobe UI (preview/equip), Stars-gated
Future         - optional Spark Coins / additional categories, only after
                 manual QA + P4B + analytics review
```
Recommended first wardrobe implementation after P8 / future P9A: **Stars-gated
direct outfit unlocks.** Do not start implementation without explicit approval.

## 18. Manual QA / art QA checklist (for future implementation)

```text
[ ] outfit preview does not break Jebby identity (Design Lock Rule)
[ ] locked cards are clear and not frustrating
[ ] Equip button only appears when owned
[ ] equipped state is obvious
[ ] unlock requirements are readable
[ ] no text overlap on mobile
[ ] no sprite background / alpha issues
[ ] no animation mismatch across the full set
[ ] no visual implication of gameplay power
```
Current P5/P7 manual visual QA remains DEFERRED / NOT VERIFIED; this checklist
is for the future wardrobe phase and has not been run.

## 19. Open questions

```text
19.1 Default outfit name: "Classic Color Knight" (wardrobe display) vs
     "Classic Cavalier" (GDD/Art Bible) - confirm at art sign-off.
19.2 Stars-gated direct unlock vs Art Bible "outfit fragments" - confirm
     near-term model (recommended: Stars-gated, deterministic).
19.3 Exact star thresholds + how many outfits at first launch (placeholders).
19.4 Is an explicit owned-set needed for Option A, or is unlock derived from
     Stars at display time (no store)?
19.5 Whether Spark Coins is ever needed for the wardrobe at all.
```
