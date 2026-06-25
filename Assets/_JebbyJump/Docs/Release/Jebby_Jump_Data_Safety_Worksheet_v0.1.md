# Jebby Jump — Data Safety Worksheet v0.1 (P31)

> **STATUS: CANDIDATE ANSWER — REQUIRES ARTIFACT AUDIT + USER APPROVAL + PLAY CONSOLE REVIEW —
> NOT SUBMITTED.** `DataSafetyCanFinalize` returns false until artifact-audited **and**
> user-approved. Do not submit a "no data" answer merely because no analytics code is called.

## Multi-source evidence (correction: permissions alone are not enough)

1. **Artifact permissions** (aapt2 on a local build): `INTERNET`, `ACCESS_NETWORK_STATE`,
   `com.android.vending.BILLING`, plus the Android-13 `DYNAMIC_RECEIVER_NOT_EXPORTED`
   internal permission. INTERNET/ACCESS_NETWORK_STATE are **engine defaults**; BILLING comes
   from the **present-but-unused** `com.unity.purchasing` package.
2. **Manifest components:** no app-defined content providers/services that export user data;
   the Unity activity + engine receivers only.
3. **Runtime assemblies:** gameplay/shell/wardrobe/progression/analytics assemblies — the
   "analytics" layer is a **local debug sink only** (no provider, no network transmission).
4. **Direct/transitive packages:** Input System, TMP, URP, etc.; `com.unity.purchasing`
   (unused), Unity services packages (unused). None wired to collect/transmit user data.
5. **SDK behavior:** no ads SDK, no analytics provider, no crash SDK, no backend/cloud SDK.
6. **Actual app behavior:** progress/settings stored in local PlayerPrefs only; no login,
   no cloud save, no first-party network calls.

> The INTERNET + BILLING permissions mean the app *could* technically reach the network /
> billing — so the "no data collected/shared" answer is a **candidate pending confirmation**,
> not an automatic pass. Recommended pre-submission action: remove the unused billing package
> or justify the permissions.

## Candidate answers (per Play Data Safety category)

For each: collected? / shared? / purpose / optional-required / ephemeral? / encrypted-in-transit? / deletion?

| Category | Collected | Shared | Notes |
| --- | --- | --- | --- |
| Personal info | No | No | no accounts/profiles |
| Financial info | No | No | no IAP wired (BILLING permission unused) |
| Location | No | No | no location APIs |
| Photos / video / audio | No | No | not accessed |
| Files / docs | No | No | not accessed |
| Contacts | No | No | not accessed |
| App activity | No | No | not transmitted (local only) |
| App info / performance | No | No | no analytics/crash SDK transmitting |
| Device or other IDs | No | No | no advertising/device IDs collected |
| Diagnostics | No | No | no diagnostics transmitted |

Candidate: **no data collected, no data shared, no data deletion needed (local-only).**

## Evidence / sources

Artifact audit (aapt2, local build) + package manifest + runtime-assembly review + P26/P27
records. No machine paths, timestamps, or identities recorded here.

## Approvals still required

User approval of every answer; Play Console form review; confirmation that the unused
billing/network permissions are acceptable or removed. **Not submitted.**
