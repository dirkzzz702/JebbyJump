# Jebby Jump — Store Graphics Checklist v0.1 (P31)

> **STATUS: DRAFT — `GraphicAssetsStatus = Blocked` for public listing — NOT SUBMITTED.** No
> final listing graphics exist; prototype-build screenshots may be used for **internal
> testing only** (user-approved), never for public listing or production.

## Launcher / adaptive icon vs Console-listing graphics (distinct)

| Asset | Where it lives | Available | Internal testing | Public listing | Production release |
| --- | --- | --- | --- | --- | --- |
| Launcher icon | In-artifact (Player Settings) | Default Unity icon only | Acceptable (default) | **Needs final art** | **Needs final art** |
| Adaptive icon foreground | In-artifact | Missing (P26 flagged none) | Acceptable w/o | **Needs final art** | **Needs final art** |
| Adaptive icon background | In-artifact | Missing | Acceptable w/o | **Needs final art** | **Needs final art** |
| Listing icon 512×512 | Console listing | Missing | ExternalRequired | **ExternalRequired** | **ExternalRequired** |
| Feature graphic 1024×500 | Console listing | Missing | ExternalRequired | **ExternalRequired** | **ExternalRequired** |
| Phone screenshots | Console listing | None final | Prototype OK (approved) | **Needs real/representative** | **Needs final** |
| Tablet screenshots | Console listing | None (only if tablet enabled) | Optional | Optional | Optional |

Tiers are evaluated separately (`StoreGraphicsPolicy.Tiers()` = InternalTesting / PublicListing
/ ProductionRelease). In-artifact vs Console-listing separation is enforced by
`StoreGraphicsPolicy.IsInArtifact` / `IsConsoleListing`.

## Screenshot rules

Use the current build; no debug overlays; no personal notifications; no private account info;
show real gameplay/UI; do not misrepresent features. Prototype art screenshots are **internal
testing only** and must be replaced for public listing.

## Evidence / sources

P26 store-compliance audit (no adaptive launcher icon configured); repo art state (prototype
outfit art only; no listing graphics); user decision (prototype screenshots internal-only).

## Approvals / external still required

Final adaptive launcher icon; 512×512 listing icon; 1024×500 feature graphic; real/final
public screenshots. Until provided, public/production graphics readiness is **Blocked**.
