# Jebby Jump — Privacy Policy DRAFT v0.1 (P31)

> **STATUS: DRAFT — USER APPROVAL REQUIRED — NOT SUBMITTED — `PrivacyPolicyStatus = Blocked`.**
> This is draft content, **not legal advice** and not a claim of legal completeness. A public
> hosted `https://` URL does **not** exist yet, so the policy is Blocked. `<PROVIDE: …>` marks
> values the user must supply.

## Draft policy content (reflects actual current behavior)

**Developer:** SparkLibrary · **App:** Jebby Jump (`com.sparklibrary.jebbyjump`)
**Effective date:** `<PROVIDE: effective date>` · **Contact:** `<PROVIDE: support email>`

**Data we collect:** Jebby Jump does **not** collect or transmit personal data. There is no
account system, no login, and no cloud profile.

**Data we do not collect / share:** no names, emails, contacts, location, photos/media,
files, or advertising identifiers are collected or shared. No analytics, advertising, or
backend service is integrated.

**Local storage:** game progress and settings (e.g., level progress, stars, equipped outfit,
audio/accessibility settings) are stored **only on the device** using local app storage and
are removed when the app is uninstalled. They are not transmitted off the device.

**Permissions note (honest):** the app package currently declares `INTERNET`,
`ACCESS_NETWORK_STATE`, and `com.android.vending.BILLING` permissions. These come from the
game engine's defaults and an **unused** in-app-billing package; the game does not transmit
data, show ads, or sell anything. (Recommendation for a real submission: remove the unused
billing package or confirm these permissions are acceptable.)

**Children / Families:** the audience includes children; the app does not knowingly collect
personal information from children. No personalized advertising is shown (no ads are present).

**Third-party services:** none are integrated for data collection. Distribution is via Google
Play (Google's own policies apply to the store).

**Data deletion:** because data is local-only, uninstalling the app removes it. There is no
server-side account to delete.

**Changes:** updates to this policy will be posted at the hosted URL with a revised effective
date.

## Evidence / sources

- App behavior: no analytics/ads/backend SDK wired; local PlayerPrefs save only; no
  first-party network calls.
- Artifact audit (aapt2 on a local build): declared permissions `INTERNET`,
  `ACCESS_NETWORK_STATE`, `BILLING` (engine defaults + present-but-unused IAP package).
- Records: P26/P27 declaration worksheets.

## Approvals / external still required

A real hosted public `https://` URL; the effective date; the support email; legal review by
the user (this draft is not legal advice).
