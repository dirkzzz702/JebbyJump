# Jebby Jump — Store External Readiness Checklist v0.1 (P31G)

> **STATUS: NOT SUBMITTED.** Consolidated external-item status + the package readiness
> decision. Placeholders (`<PROVIDE: …>`) cannot pass final readiness.

## Decision

> **Store listing package blocked — missing external content.**

Everything finalize-able (listing copy draft, privacy-policy draft, data-safety worksheet,
content-rating worksheet, graphics checklist, Families worksheet, release notes) is complete
and marked NOT SUBMITTED. The package is blocked only on external content the user must supply.

## External-item status

| Item | Status | Doc |
| --- | --- | --- |
| Listing copy | Draft — user approval required | Store Listing Copy (DRAFT) |
| Privacy policy content | Draft ready | Privacy Policy DRAFT |
| Privacy policy public URL | **Blocked (missing)** | Privacy Policy DRAFT |
| Support email | **Blocked (`<PROVIDE>`)** | Listing / Privacy |
| Developer display name | SparkLibrary (brand) — confirm | Declaration Record |
| Data Safety answers | Candidate — needs audit + approval | Data Safety Worksheet |
| Content rating | Worksheet — not submitted, not predicted | Content Rating Worksheet |
| Target audience / Families | Mixed → Families (declared); obligations to verify | Families Readiness Worksheet |
| Launcher / adaptive icon | **Needs final art** | Store Graphics Checklist |
| Listing 512 icon | **ExternalRequired (missing)** | Store Graphics Checklist |
| Feature graphic 1024×500 | **ExternalRequired (missing)** | Store Graphics Checklist |
| Public screenshots | **Missing** (prototype OK internal-only) | Store Graphics Checklist |
| Release notes | Draft ready | Internal Testing Release Notes |
| Pricing / monetization | Free, no ads, no IAP (decided) | Listing / Data Safety |

## Checks (automated)

`ListingCopyWithinLimits`, `ListingCopyHasUnverifiedClaim`, `PrivacyPolicyValid`
(placeholder/non-https fails), `DataSafetyCanFinalize` (needs audit + approval),
`TargetAudienceExplicit`, `IsPredictedRating` (stays empty), `StoreGraphicsPolicy`
(launcher/adaptive vs listing; 3 tiers), `StoreReadinessPolicy.ContainsPlaceholder` +
`Decide` (placeholders/missing externals ⇒ blocked).

## Approvals still required (to lift the block)

Hosted privacy-policy URL; support email; final launcher/adaptive icon; 512 listing icon;
1024×500 feature graphic; final public screenshots; user approval of copy + Data Safety; IARC
questionnaire completion. Never claim submitted / approved-by-Google / legal-compliance /
rating-finalized.
