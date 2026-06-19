# Jebby Jump — Build & Preflight Guide v0.1 (P23)

How to validate and produce the automated Android release-candidate build. All
tooling is editor-only (`JebbyJump.Release.Editor`) and excluded from the player.

## Menu commands (`Jebby Jump/Release/…`)

- **Apply Approved Build Config** — the ONLY writer of tracked config. Sets the
  approved identity (SparkLibrary / com.sparklibrary.jebbyjump) and writes the build
  scene list from the immutable `ReleaseSceneContract` (Boot→MainMenu→Game). Run this
  once after pulling; commit the resulting `ProjectSettings.asset` +
  `EditorBuildSettings.asset` changes.
- **Run RC Preflight** — read-only validation (identity, scenes, orientation, input,
  backend/arch, packages, required assets, TMP digits). Logs each check + PASS/FAIL.
  Never mutates assets.
- **Build Release Candidate** — runs preflight then builds; never exits the editor.
- **Open Latest Build Report** — reveals `Builds/P23/`.

## CLI / headless

```bash
UNITY=".../Unity.exe"; PROJ="."

# 1. apply approved config (commit the ProjectSettings/EditorBuildSettings diff)
"$UNITY" -batchmode -quit -nographics -projectPath "$PROJ" \
  -executeMethod JebbyJump.Release.JebbyJumpReleaseConfig.ApplyApprovedBuildConfig

# 2. tests (record exact counts)
"$UNITY" -batchmode -nographics -projectPath "$PROJ" -runTests -testPlatform EditMode -testResults edit.xml
"$UNITY" -batchmode -nographics -projectPath "$PROJ" -runTests -testPlatform PlayMode -testResults play.xml

# 3. outfit QA gate
"$UNITY" -batchmode -quit -nographics -projectPath "$PROJ" \
  -executeMethod CheckOutfitSpriteAlpha.RunOnAllOutfitSprites

# 4. preflight
"$UNITY" -batchmode -quit -nographics -projectPath "$PROJ" \
  -executeMethod JebbyJump.Release.ReleasePreflight.RunAndLog

# 5. release-candidate build (export the test gate first)
export JJ_TESTS_PASSED=1 JJ_TESTS_TOTAL=<n>
"$UNITY" -batchmode -nographics -projectPath "$PROJ" \
  -executeMethod JebbyJump.Release.JebbyJumpReleaseBuilder.BuildReleaseCandidateFromCommandLine
# exit code 0 = ready verdict; non-zero = not ready (see Builds/P23/.../release-report.json)
```

On Windows PowerShell use `Start-Process -FilePath $UNITY -ArgumentList @(...) -Wait
-PassThru` to capture the exit code reliably (the `&` call operator does not), and set
`$env:JJ_TESTS_PASSED = "1"` before the build.

## Authoritative verification order

1. working tree clean (record reproducibility level = clean-working-tree) →
2. apply approved config → 3. compile/import → 4. EditMode + PlayMode tests →
5. outfit QA → 6. preflight (errors == 0) → 7. build Android AAB (or Windows smoke if
the toolchain is unavailable) → 8. BuildReport + warning gate → 9. SHA-256 + report →
10. forbidden-file/package/network scan → 11. tree clean (no binaries/secrets).

## Toolchain note

The builder checks Android module + SDK/NDK/JDK availability (reflection). If the
toolchain cannot be confirmed it builds the Windows smoke artifact and reports
`AndroidToolchainBlocked_WindowsSmokePassed`; a real Android build failure (compile/
asset/IL2CPP/Gradle) reports `AndroidBuildFailed` with no Windows fallback. To produce
a true Android AAB, install Android Build Support + SDK/NDK/JDK via Unity Hub.

## Editor/build state safety

The builder captures and restores active build target, `buildAppBundle`, and dev/
debug/profiler/deep-profiling flags in `try/finally`. Menu commands never call
`EditorApplication.Exit`; only the CLI entry points exit (non-zero on failure).

## What P23 does NOT do

No signing config, no store upload, no package add/remove, no gameplay/economy/
wardrobe/migration changes, no performance/balance work. Manual device/visual/
performance/accessibility/balance/art/signing/store verification remain DEFERRED /
NOT VERIFIED.
