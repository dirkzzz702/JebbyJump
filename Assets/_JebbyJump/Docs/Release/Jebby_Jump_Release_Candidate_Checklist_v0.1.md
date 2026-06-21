# Jebby Jump — Release Candidate Checklist v0.1 (P23)

Automated production-build readiness for the **Android AAB** release candidate.
P23 proves the repo can produce a clean, correctly configured player build with a
validated preflight, deterministic CLI build, and an artifact/hash report. It does
**not** claim store/signing/device/visual/performance/balance/art certification.

## Scope

- Target: **Android — AAB** (IL2CPP / ARM64). Windows standalone is built **only**
  as a smoke test when the Android toolchain is unavailable (never to mask an
  Android build failure).
- Editor-only tooling (`JebbyJump.Release.Editor`, `includePlatforms: ["Editor"]`) —
  never compiled into the player.

## Identity / version

- Company: **SparkLibrary**  |  Product: **JebbyJump**
- Application id: **com.sparklibrary.jebbyjump** (Android / Standalone / iOS)
- Version: **1.0**  |  Android version code: **1**
- Set only by the explicit `Jebby Jump/Release/Apply Approved Build Config` command.

## Scene order (immutable contract)

`Boot` (entry) → `MainMenu` → `Game` (`ReleaseSceneContract`). The applier writes
`EditorBuildSettings` from the contract; the builder's `BuildPlayerOptions.scenes`
uses the contract directly; the preflight validates `EditorBuildSettings` == contract
(order, existence, enabled, no duplicates, no SampleScene/test scene).

## Player settings pinned by preflight

Landscape-only; new Input System only; Android IL2CPP; Android ARM64; non-empty
identity matching approved; valid version/version code. Drift in any of these fails
the preflight (preflight validates, never fixes).

## Tests / asset integrity

- EditMode release suite + PlayMode suite both pass (exact counts recorded in the
  build report and the run log).
- Outfit sprite-alpha QA gate passes (49/49).
- Read-only required-asset checks: LevelCatalog, Jebby prefab + animator,
  OutfitVisualLibrary, WardrobePreviewLibrary, InputActionAsset, Platform prefab,
  TMP default font supports digits 0–9 (memory cues). Outfit override controllers are
  reported (not required — prototype art is not final-certified).

## Network / package audit

Every manifest package is explicitly classified (`ReleasePackageClassification`).
`com.unity.purchasing` (IAP) = present-but-unused (no runtime code refs);
`com.unity.services.cloud-build` = editor service; `com.unity.microsoft.gdk(.tools)`
= target-irrelevant; `com.coplaydev.coplay`/IDE/test-framework = editor-only. No
unexpected runtime SDK; no network usage in runtime scripts. P23 adds/removes no
packages.

## Build command

```
Unity -batchmode -nographics -projectPath . \
  -executeMethod JebbyJump.Release.JebbyJumpReleaseBuilder.BuildReleaseCandidateFromCommandLine
```

Set `JJ_TESTS_PASSED=1` (+ `JJ_TESTS_TOTAL`) after the test gate passes so the verdict
includes the test gate honestly. CLI exits non-zero on any non-ready outcome. Menu:
`Jebby Jump/Release/Build Release Candidate` (never exits the editor).

## Artifact / hash report

`Builds/P23/<target>/<version>/release-report.json` + `.md` (ignored path). Android
hashes the `.aab`; Windows smoke produces a sorted per-file SHA-256 manifest of the
whole build directory. All artifact paths are relative to `Builds/P23`. The report
never contains secrets/keystore data/full local paths.

## Warning policy

Classified allowlist (narrow patterns only). Preflight warnings and **post-build**
warnings are separate; any new/unclassified build/compiler/IL2CPP/Gradle warning
fails the RC warning gate. The report is written on every failure path.

## Signing status

No signing configured → the AAB is **debug-signed (development; NOT production)**.
Custom/upload-key signing and store upload remain **external / DEFERRED**. No
keystores, passwords, certificates, or provisioning profiles are ever committed.

## Independent statuses (no single broad "Passed")

Preflight · Tests · Android build (`AndroidAabBuilt` / `AndroidToolchainBlocked_
WindowsSmokePassed` / `AndroidBuildFailed`) · Windows smoke · Signing · Warning gate ·
Artifact hashing · Manual QA. A single `ReadinessVerdict` is computed from these.

## Readiness verdict (exact wording)

- `Automated Android release-candidate build readiness complete` — only when the AAB
  built, preflight passed, tests passed, warning gate passed, hashing succeeded, and
  approved config matched.
- `Android RC blocked by toolchain; generic build pipeline smoke test passed` — when
  the Android toolchain is unavailable but the Windows smoke build succeeded.

Never claimed: store-ready, upload-ready, signed-for-production, Play-compliant,
device-/visual-/performance-certified, balance-/art-final.

## Reproducibility level tested

**clean-working-tree** (built from the verified-clean checkout). Fresh-clone
reproducibility was NOT executed; the Library folder is never auto-deleted.

## Manual QA status — DEFERRED / NOT VERIFIED

Manual device, visual, performance, accessibility, balance, art-final, signing, and
store-submission verification all remain DEFERRED / NOT VERIFIED.

## Known blockers / limitations

- Debug-signed only; production signing + store upload are external.
- Prototype outfit art not final-certified; reward thresholds are placeholders.
- IAP/GDK/cloud-build packages present but unused (kept; not removed in P23).

## P24 note

P24 added an automated performance / build-size baseline (see `Docs/Performance/`).
The compressed AAB is ~113.6 MB / ~108.4 MiB; P24's display-only zero-alloc fixes
added +~4.6 KiB. The Android AAB build + preflight + warning gate were re-verified
complete after P24. Device FPS / battery / thermal + manual visual QA remain
DEFERRED / NOT VERIFIED.

## P26 note

P26 added release-distribution-readiness scaffolding on top of the P23 pipeline:

- **Signing intent is explicit** (`JJ_SIGNING_MODE`): default `debug`; `upload` applies a
  custom UPLOAD key from env and **fails the build** on any missing/invalid keystore var
  (never a silent debug fallback). The signing MODE (`DebugSigned` / `EnvUploadKeySigned`)
  is independent of store-readiness; an upload key is NOT the Play App Signing key.
- **Signature is verified on the built artifact** (apksigner for APK, jarsigner for AAB;
  recorded `Verified`/`Failed`/`Skipped`, never a false pass), and the PlayerSettings
  signing config is **restored byte-for-byte** after every build (`SigningConfigRestored`).
- **Separate APK + AAB** outputs/reports (`JJ_BUILD_FORMAT=apk|aab`): AAB → Play
  (re-signed by App Signing), APK → direct sideload. The report records `ArtifactFormat`,
  `DistributionPurpose`, signature status + signer fingerprint, resolved artifact target
  SDK (APK, via aapt2), and 16 KB page alignment (APK, via zipalign).
- **Store Compliance Audit** (read-only, `Builds/P26`): configured target SDK (Automatic)
  vs resolved (API 36 meets the assumed ≥35 minimum — Automatic flagged for
  reproducibility, not non-compliance), launcher/adaptive icon vs Console listing
  graphics, and DATED Play-policy assumptions (verify — they age). See
  `Jebby_Jump_Store_Submission_Readiness_v0.1.md` +
  `Jebby_Jump_Production_Signing_and_Internal_Testing_v0.1.md`.

`*.keystore`/`*.jks`/`*.p12` are gitignored; no keystore/password is committed or
persisted. Production signing, Play App Signing enrolment, store upload, IARC rating,
and device QA all remain DEFERRED / external / NOT VERIFIED.
