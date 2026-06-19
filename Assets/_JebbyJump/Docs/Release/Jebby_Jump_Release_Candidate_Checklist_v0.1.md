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
