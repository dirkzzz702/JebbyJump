# Jebby Jump — Monetization Plan v0.1

Status: `PROPOSED` · plan-only. **Nothing here is implemented.** Every phase is
gated and must be reviewed before build. This refines the monetization bullet
list in `Jebby_Jump_Full_Production_Plan_v1.0.md` (§ Monetization) with market
research and a concrete, service-abstracted phase breakdown.

Author's note: skill/obstacle content picks are being decided separately with
the owner; this doc covers only how the game earns.

## Thesis — model on Crossy Road, not Candy Crush

Jebby Jump's closest market peer is **Crossy Road**: cute, family-appealing,
cosmetic-rich, arcade. Its model is the template:

```text
Rewarded video as the revenue engine (opt-in, never forced)
Every cosmetic earnable by play/gems OR buyable (level playing field)
Strictly cosmetic purchases — no pay-to-win
```

Crossy Road grossed **$10M+ from rewarded ads alone**. We deliberately do **not**
copy Candy Crush's lives/energy gate + gold-bar currency + personalised
"you're-stuck-here's-a-deal" offers: that model conflicts with our own launch
rules (no hard stamina wall, no forced ads) and with kid-appeal ad policy
(below).

## The compliance constraint that shapes everything

**Decision on record:** the app ships in the **normal game category, not Kids.**

Research correction the owner should hold onto: store category does **not**
exempt a child-appealing app from child-privacy rules. Google Play's Families
policy and the FTC's COPPA rule hinge on whether an app **appeals to children** —
which Jebby Jump's art plainly does. Consequence:

```text
Serve NON-PERSONALISED ads through a COPPA/families-certified ad SDK,
regardless of store category or age rating.
Do not transmit the advertising identifier for children / unknown-age users.
No iOS ATT prompt (we are not tracking).
No personalised, behaviour-timed offers.
```

Net: lower eCPM, far lower rejection/enforcement risk. Use a families-certified
network (AdMob families programme, or a kids specialist such as SuperAwesome /
Kidoz). This barely dents the plan — the Crossy Road model never needed
personalised ads.

## Guardrails (from CLAUDE.md / Full Production Plan — unchanged)

```text
Cosmetics never pay-to-win
Skills helpful, never required
No hard stamina / energy wall at launch
No forced mid-gameplay ads (rewarded is opt-in; interstitials between levels only)
Economy / ads / backend only through service abstractions
No vendor SDK without explicit approval
```

## Launch stack (kept light + on-brand)

| Lever | How it works in Jebby Jump | Build cost |
|---|---|---|
| Rewarded video (engine) | Continue (revive in place, once/level); earn-or-buy cosmetics; double this level's gems; earn a memory helper (cooldown); daily gift | med (SDK + placements) |
| Remove Ads (IAP) | One-time; suppresses interstitials, keeps opt-in rewarded. Highest ROI-per-effort | low |
| Direct-buy cosmetics (IAP) | Buy an outfit/character; also earnable by play/gems. Reuses the existing wardrobe | low–med |
| Rainbow Gem packs (IAP) | Buy the soft currency → cosmetics / consumable skills / convenience | med (needs economy) |
| Starter pack (IAP) | One-time discounted bundle once the economy exists | low once economy exists |
| Between-level interstitials | Optional, frequency-capped, suppressed by Remove Ads. Never mid-gameplay | low |

## Deferred to post-launch (once retention data justifies build)

```text
Rainbow Pass — seasonal free + premium cosmetic tracks (fits the 10-world journey)
Rainbow Club — subscription: no-ads + daily gems + monthly exclusive cosmetic
World early-unlock — convenience only, never a progression wall
```

Skip banners entirely — weak for this genre and they cheapen the pastel look.

## Gated phase breakdown

Ad/economy code depends only on interfaces, mirroring the existing
`IAnalyticsSink` pattern. A `Null*` implementation ships for editor/tests; the
real SDK slots in behind the interface in a separate approved phase.

| Phase | Title | Gate to start | Ships an SDK? |
|---|---|---|---|
| P38A | Ad service abstraction (`IAdService` + placements + `NullAdService` + tests) | monetization approval | No |
| P38B | Rewarded placements wired to gameplay behind the stub (Continue, double-gems, daily gift, earn-or-buy cosmetic, helper) | P38A | No |
| P38C | Certified ad SDK integration (families-certified network) | P38B + SDK/vendor approval + privacy review | **Yes** |
| P39A | Economy foundation (Rainbow Gems ledger + store abstraction) | currency/economy approval | No |
| P39B | IAP: Remove Ads + direct-buy cosmetics (`com.unity.purchasing`, already in project) | P39A + IAP approval; receipt-validation decision | **Yes** |
| P39C | Gem packs + Starter pack | P39B | — |
| P40+ | Rainbow Pass / Rainbow Club subscription | post-launch + retention evidence | — |

Cheapest high-ROI first: **P38A/B (rewarded) + Remove Ads + direct-buy
cosmetics** — all reuse what exists and need almost no economy. Gem packs
(P39A–C) are a larger lift: currency ledger + shop UI + IAP + light backend for
receipt validation — hence gated separately.

## Per-phase fields (each phase's prompt carries these)

```text
objective · scope · out-of-scope · files likely changed · migration/save impact ·
tests · manual QA · SDK/approval dependency · success criteria · rollback plan ·
commit strategy · next-phase gate
```

## Success expectations (2025–26 hybrid-casual benchmarks)

```text
Blended ARPDAU ~ $0.15–0.50 (most from rewarded at launch)
Non-payers ~ $0.08–0.15 from rewarded alone; payers add ~$0.30+ IAP
Casual IAP ARPU ~ $1.34 by D90; payer conversion ~1–3%; iOS spends/retains more
Rewarded video: 80–90% completion, ~87% positive sentiment
IAP/ads settles ~ 50/50 for casual over time
```

These are targets to validate against live data, not guarantees.

## Sources

Crossy Road ad case studies (TouchArcade, GameDeveloper); hybrid-casual
benchmarks (Verve, CAS.ai, Studio Krew); Subway Surfers monetisation (Business
of Apps, RevenueCat); Candy Crush model (Juego Studio, GameDeveloper); Google
Play Families / COPPA (SuperAwesome, Google Play Console Help, AdMob families);
conversion & ARPU (MAF, AppsFlyer); subscriptions (RevenueCat State of
Subscription Apps). Retrieved 2026-07.
