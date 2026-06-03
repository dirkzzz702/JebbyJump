# Jebby Jump - Reward / Economy Design Spec v0.1

Status: DESIGN SPEC ONLY (P6C). No runtime, no economy code, no monetization
code, no ads SDK. This document defines future direction; implementation is
deferred to the planned P7 Economy Foundation phase and gated on P4B manual
playtest + analytics data.

---

## 1. Purpose

Define a safe, child-friendly reward/economy direction so future work
(rewards, cosmetics, optional rewarded ads) has a clear, agreed target before
any code is written. This is a design artifact only. It does **not** add
currencies, stores, ads, or balance changes to the game now.

It answers: what rewards exist, what (if any) currencies exist, how players
earn rewards, what rewards are spent on, where rewarded ads could fit later,
what analytics are needed first, and what to avoid for a child-friendly,
mobile-first game.

---

## 2. Current game context

Grounded in the existing accepted state (GDD, Roadmap, Production Plan):

- 10-level vertical slice (P3 baseline). Mobile-first, desktop-supported.
- **Time ranking** replaces score: per-level clear time, best time, and an
  S/A/B/C rank (per-level `TimeRankConfig`).
- **Local unlock progression**: Level 1 unlocked; completing a level unlocks
  the next; per-level best time persisted (`BestTimeStore`), rank computed
  dynamically.
- **Equipped skills** are *gameplay mechanics* (equipment + consumable;
  Rocket Boots / Bubble Shield / Color Echo). Skills must not bypass the
  memory sequence and are **not** a power-sale surface.
- **Local analytics** (P6A/P6B): `AnalyticsService` + `IAnalyticsSink` +
  `DebugAnalyticsSink`, with a central `AnalyticsEvents` / `AnalyticsParams`
  catalog. Local/debug-only; no provider, no network, no PII.
- GDD launch vision already names **Rainbow Gems**, rewarded ads, and a
  **cosmetic-only wardrobe** (Classic Cavalier, Forest Cavalier, Sunshine
  Knight, Aqua Knight, Silver Dreamer). "Avoid at launch: forced ads, hard
  stamina, pay-to-win wardrobe, aggressive monetization."
- Deferred items remain deferred: P4B manual playtest + balance tuning, and
  the P5B-P5F manual UI QA backlog.

---

## 3. Design principles

```text
child-friendly and low-pressure
no gambling-style mechanics
no loot boxes / no gacha
no pay-to-win
no dark patterns / no nag screens
no punishment for not watching ads
short-session friendly
offline-friendly first
simple enough for a young player to understand
parents should never feel tricked or pressured
```

The design supports future monetization only in a safe, optional, cosmetic
way. Rewards must never affect level rank fairness.

---

## 4. Recommended reward model

A deliberately small model:

```text
Primary reward:        Stars (per-level mastery)
Optional soft currency: Spark Coins (earned only, cosmetics)
Premium currency:      Rainbow Gems - DEFERRED (launch-era, not now)
Paid / hard currency:  none in the current plan
```

- **Stars** are a permanent, progression-flavored mastery reward that mirror
  the existing S/A/B/C rank. They make mastery visible and feel good without
  introducing a wallet.
- **Spark Coins** are an *optional, earned-only* soft currency for cosmetics,
  introduced only if/when a cosmetic store exists. No purchase path now.
- **Rainbow Gems** (the GDD launch currency) are intentionally **deferred** so
  no paid/hard currency is designed into the near-term loop. Honors the GDD
  vision by sequencing, not contradiction.

---

## 5. Currency options and chosen direction

| Currency | Role | Earned | Purchasable | Permanent | Decision |
|---|---|---|---|---|---|
| **Stars** | Per-level mastery indicator | Yes (clear / rank) | No | Yes | **Adopt now (mastery, not a wallet)** |
| **Spark Coins** | Soft cosmetic currency | Yes (earned only) | No | Yes (balance) | **Reserve for P7; earned-only** |
| **Rainbow Gems** | Premium currency (GDD launch vision) | TBD | TBD (later) | TBD | **Deferred - not in near-term loop** |
| Hard / paid currency | Direct-buy currency | - | - | - | **Rejected for now** |

Chosen near-term direction: **Stars primary, Spark Coins reserved (earned
only), Rainbow Gems deferred, no paid currency.**

---

## 6. Reward sources

All quantities are **PLACEHOLDER** until P4B manual playtest + analytics data
exist. Numbers here illustrate shape only and must not be treated as final or
implemented.

| Source | Reward (placeholder) | Type | Notes |
|---|---|---|---|
| Complete a level | +1 Star | Permanent (per level) | Base mastery |
| Rank B or above | +1 Star | Permanent (per level) | Encourages clean clears |
| Rank A / S | +1 Star | Permanent (per level) | Top mastery tier |
| First clear of a level | fixed Spark Coin bonus | One-time | Not farmable |
| New best time | small Spark Coin bonus | On genuine improvement only | Not farmable by repetition |
| After Level 10 (slice end) | "All clear" badge + message | One-time | See section 13 for endgame |

Rank affects reward (more Stars for better rank) but **must never affect rank
fairness** - the rank itself is earned purely by clear time.

---

## 7. Spend / unlock targets

Cosmetic-first. No paid power. Skills are gameplay, never store power.

```text
Jebby cosmetic colors / palettes
trail effects
jump / button skins
background themes
badge collection (display only)
practice helper unlocks (non-competitive aids)
extra visual / celebration effects
```

Wardrobe outfits (from GDD) are cosmetic only: Classic Cavalier, Forest
Cavalier, Sunshine Knight, Aqua Knight, Silver Dreamer.

Explicitly **not** for sale: skill power, extra lives that affect rank, rank
boosts, sequence hints that bypass memory, or anything that changes fairness.

---

## 8. Farming / anti-abuse rules

```text
First-clear and mastery rewards are one-time and permanent.
Replaying a cleared level grants no farmable soft currency
  (or a hard daily/diminishing cap if any is granted at all).
Best-time bonus only fires on a genuine improvement over the stored best.
No reward path rewards grinding a trivial early level repeatedly.
Stars are capped per level by design (you cannot exceed a level's max).
```

Goal: a young player cannot accidentally (or deliberately) farm currency by
repeating Level 1; rewards track genuine mastery/progress.

---

## 9. Ads / monetization candidates (design only - NOT implemented)

If rewarded ads are added later, keep them optional and child-safe.

Candidate placements (later):

```text
watch ad for a bonus cosmetic-currency grant after a level (optional)
watch ad to double a NON-essential cosmetic reward (optional)
watch ad to preview a cosmetic
```

Avoid:

```text
forced interstitials after failure
ads that interrupt active gameplay
ads required to progress
revive-via-ad that affects rank / best-time fairness
ads shown too frequently
ads for child-inappropriate products
```

Hard rule for P6C: **no ad SDK, no ad code, no fake ad UI, no placeholder ad
buttons.** Ads are a future, optional, cosmetic-only revenue path behind a
service abstraction (GDD section 16).

---

## 10. Analytics events required before implementation (future - documented only)

These extend the existing P6A/P6B catalog (`AnalyticsEvents` /
`AnalyticsParams`, dispatched through `IAnalyticsSink`). They are **not**
implemented in P6C - listed so economy work has its hooks pre-agreed.

| Event (future) | Trigger | Key parameters | Why needed | Privacy / safety |
|---|---|---|---|---|
| `reward_granted` | A reward is given (Star/Coin) | reward_type, amount, source, level_index | Measure reward flow / pacing | No PII; values are primitives |
| `currency_balance_changed` | Soft-currency balance changes | currency, delta, new_balance, reason | Detect sinks/sources, anti-abuse | No PII |
| `cosmetic_unlocked` | A cosmetic is unlocked | cosmetic_id, cost, currency | Cosmetic funnel | No PII; cosmetic_id is an internal key |
| `cosmetic_equipped` | A cosmetic is equipped | cosmetic_id | Preference / engagement | No PII |
| `shop_opened` | Cosmetic store opened | source | Store reach | No PII |
| `rewarded_ad_offer_shown` | Optional ad offer displayed | placement | Offer reach | No PII; no ad-network IDs logged locally |
| `rewarded_ad_started` | Player opts into an ad | placement | Opt-in rate | No PII |
| `rewarded_ad_completed` | Ad finished | placement | Completion rate | No PII |
| `rewarded_ad_failed` | Ad failed/unavailable | placement, reason | Reliability | No PII |
| `rewarded_ad_reward_granted` | Reward granted post-ad | placement, reward_type, amount | Verify opt-in reward loop | No PII |

All future events must keep the P6B rules: snake_case names, primitive
payloads, no PII, sanitized at the service boundary.

---

## 11. UX guardrails for children and parents

```text
No countdown/pressure timers pushing a purchase or an ad.
No confusing "free vs paid" framing; earned vs optional must be obvious.
No nag screens, no interruptive upsells, no misleading buttons.
Ads (later) are always opt-in and clearly labelled.
Parental-friendly: nothing that pressures spending or feels deceptive.
Rewards celebrate effort/mastery, not spending.
```

---

## 12. Deferred decisions

```text
Whether/when to introduce Rainbow Gems (premium currency).
Whether Spark Coins ship at all, and when.
Real ad vendor choice and integration (service-abstracted).
Any "convenience" purchases (currently leaning: none / cosmetic-only).
Soft stamina / event tickets (GDD lists as "later"; not planned near-term).
All exact reward numbers and costs.
```

These wait for: manual playtest feedback (P4B), analytics from level
completion / failure / rank, retention goals, and a parent/child UX review.

---

## 13. Implementation roadmap

P6C is spec only. Implementation maps to the existing roadmap phases:

```text
P6C (this doc)      - reward/economy design spec, docs only
P7 Economy Foundation - Stars + (optional) Spark Coins store, earned-only,
                        reward hooks, future analytics events; no paid currency
P8 Cosmetic Wardrobe  - cosmetic unlock/equip on top of the economy
P9 Backend/Analytics/Ads - provider sink, optional rewarded ads behind
                        service abstraction, cloud/account capability
```

Endgame note (after Level 10 today): show an "all clear" celebration + badge;
do **not** gate further content behind currency. World/level expansion is a
separate future phase, not part of this spec.

Before any economy tuning: **do not** tune rank thresholds, level difficulty,
or reward numbers. Wait for P4B + analytics.

---

## 14. Open questions

```text
1. Are Stars purely mastery (display), or also a light spend gate for some
   free cosmetics? (Recommended: mastery only at first.)
2. Confirm Rainbow Gems stays deferred vs. introduced at launch (GDD lists it
   as launch-era). Recommended: deferred until after P7 earned-only economy.
3. Should replaying a level grant any (capped/diminishing) soft currency, or
   strictly none? (Recommended: none / first-clear only to start.)
4. When is the first soft currency actually introduced - P7, or later once
   cosmetics exist to spend on?
5. Which cosmetics are the initial unlock set, and their (placeholder) costs?
```

---

## Recommended decisions (summary)

```text
No hard currency for now.
No paid currency for now.
No loot boxes.
No forced ads.
No ads required for progression.
Cosmetic-first reward spending.
Rewards do not affect level rank fairness.
Reward numbers are placeholders.
Economy implementation deferred until after manual playtest (P4B) + analytics.
```

P4B manual playtest + balance tuning and the P5B-P5F manual UI QA backlog
remain DEFERRED and are not affected by this spec.
