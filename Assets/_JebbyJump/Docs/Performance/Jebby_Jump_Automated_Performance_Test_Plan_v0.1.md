# Jebby Jump - Automated Performance Test Plan v0.1 (P24)

What the automated performance/lifecycle suite covers, and the deliberate boundary
between editor-only tooling and runtime-driven tests.

## Assembly boundary (correction #3)

- **Editor-only** `JebbyJump.Performance.Editor` (`includePlatforms: ["Editor"]`,
  excluded from the player): pure report models, `PerformanceRegressionPolicy`,
  `BuildSizeMath`, `PerfReportWriter`, `BuildSizeAuditTool`. Tested by EditMode tests.
- **Runtime flows** are exercised by **PlayMode tests via reflection / scene loads**
  (UI controllers are Assembly-CSharp and not referenceable by a test asmdef); no UI
  was restructured into asmdefs.

## Measurement methods

- **Allocations:** Unity Test Framework GC constraint
  (`Is.[Not.]AllocatingGCMemory()`), warmed first. Reliable in the editor, unlike
  `GC.GetAllocatedBytesForCurrentThread` (returns 0 under the editor Boehm GC).
- **Subscriber leaks:** `EventSubscriberProbe` reflects a static event's backing
  delegate and counts `GetInvocationList()`; asserted to return to baseline.
- **Scene timing:** full async load + a frame for Awake/Start (load-to-readiness),
  via `[UnityTest]` + `Stopwatch`. Wall-clock is a regression signal, not device FPS.

## Tests

EditMode (`JebbyJump.Tests.EditMode`): `PerformanceRegressionPolicy_*` (RejectsLeak,
RejectsUnexpectedGcAlloc, ClassifiesTimingRegression, DoesNotClaimDeviceCertification),
`PerfTooling_IsEditorOnly`, `Reports_SerializeRequiredFields`,
`Reports_UseRelativePaths`, `BuildSizeMath_UnitsAndTopContributors`.

PlayMode (`JebbyJump.Tests.PlayMode`): `TimeFormatTests` (AppendClock/AppendF1 dense
sweep equivalence + zero-alloc), `FormattingAllocationEvidenceTests` (pre-fix old
expressions allocate; post-fix formatters do not), `LifecycleStabilityTests`
(subscriber return-to-baseline, repeated subscribe/unsubscribe no-growth,
`AnalyticsBuffer_RemainsCapped`), `SteadyStateAllocationTests`
(GameplayInputBlockPolicy / SafeAreaCalculator / PlatformCueMapping zero-alloc),
`SceneReadinessTests` (MainMenu load-to-readiness).

Counts at P24: EditMode 46, PlayMode 333 (total 379); outfit QA 49/49. All P5-P23
tests remain green.

## Instrumentation

Static `ProfilerMarker`s (negligible overhead, ship): `JebbyJump.Scene.LoadMainMenu`,
`JebbyJump.Scene.LoadGame`, `JebbyJump.Memory.SpawnPlatforms`,
`JebbyJump.Memory.BuildSwatches`. Read them in a profiler session on a device
(deferred).

## Headless limitations

Frame rate, GPU timing, battery, thermal, and on-device memory cannot be measured
headless and are DEFERRED / NOT VERIFIED. Deep UI-driven soak (full open/close of
Assembly-CSharp panels) is covered structurally by P20-P23 lifecycle code +
subscriber/cap tests rather than full reflection-driven UI automation.
