# Jebby Jump - Performance Budget v0.1 (P24)

The performance targets + automated regression policy. Editor/headless metrics are
regression signals only; absolute frame/battery/thermal targets apply ONLY to an
approved physical device (DEFERRED / NOT VERIFIED - no device this phase).

## Frame target (product)

- Target: **60 FPS** (16.67 ms/frame).
- Minimum supported floor: **30 FPS** (33.33 ms/frame).
- `Application.targetFrameRate`/vSync are NOT changed in P24 (none is set today;
  changing them needs separate approval).

## Strict automated invariants (zero tolerance)

These are enforced by automated tests (editor/headless) and must always hold:

- No static-event subscriber growth after open/close or repeated cycles
  (measured via reflection on the event's backing delegate).
- No object/type-count growth after equivalent repeated flows.
- No per-frame GC allocation in steady-state paths that are expected to be zero
  (measured via the Unity Test Framework GC constraint - reliable in the editor;
  `GC.GetAllocatedBytesForCurrentThread` is NOT tracked under the editor Boehm GC).
- Analytics debug buffer stays capped at 64.
- One cue label per swatch/platform (no duplication across repeated sequences).

Verified zero-allocation steady-state paths: `TimeFormat.AppendClock`,
`TimeFormat.AppendF1`, `GameplayInputBlockPolicy.ShouldBlock`,
`SafeAreaCalculator.ComputeAnchors`, `PlatformCueMapping.CueFor`.

## Relative-regression metrics (signals, not pass/fail device limits)

- Scene load-to-readiness wall-clock (full async load + first-frame init).
- Flow durations + GC deltas for menu/wardrobe/memory-phase setup.
  Baseline captured in `Jebby_Jump_Performance_Baseline_v0.1.json`; future runs
  compare relatively (tolerance %), never as device certification.

## Build-size budget

- No hard size fail-gate yet (no product budget approved). Baseline recorded:
  compressed AAB ~113.6 MB (10^6) / ~108.4 MiB (2^20). See the Build Size Audit.
- Any instrumentation that ships must keep the AAB delta negligible (P24: +~4.6 KiB).

## Deferred (require a device / separate scope)

Device FPS, battery, thermal, on-device memory, GPU timing, manual visual QA, and
P4B gameplay balance remain DEFERRED / NOT VERIFIED.
