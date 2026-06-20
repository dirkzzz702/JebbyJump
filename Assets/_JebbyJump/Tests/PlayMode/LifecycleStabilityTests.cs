using System;
using JebbyJump.Analytics;
using JebbyJump.Settings;
using NUnit.Framework;

namespace JebbyJump.Tests
{
    // P24 correction #6: concrete static-subscriber-leak + analytics-cap checks.
    public class LifecycleStabilityTests
    {
        [SetUp]
        public void SetUp()
        {
            AccessibilitySettingsStore.ResetForTests();
            AnalyticsService.ResetForTests();
        }

        [TearDown]
        public void TearDown()
        {
            AccessibilitySettingsStore.ResetForTests();
            AnalyticsService.ResetForTests();
        }

        private static int MemCueSubs() =>
            EventSubscriberProbe.Count(typeof(AccessibilitySettingsStore), "MemoryCuesChanged");
        private static int ReduceSubs() =>
            EventSubscriberProbe.Count(typeof(AccessibilitySettingsStore), "ReduceMotionChanged");

        [Test]
        public void AccessibilitySettings_SubscribersReturnToBaseline()
        {
            Assert.AreEqual(0, MemCueSubs());
            Assert.AreEqual(0, ReduceSubs());

            Action<bool> a = _ => { };
            Action<bool> b = _ => { };
            AccessibilitySettingsStore.MemoryCuesChanged += a;
            AccessibilitySettingsStore.ReduceMotionChanged += b;
            Assert.AreEqual(1, MemCueSubs());
            Assert.AreEqual(1, ReduceSubs());

            AccessibilitySettingsStore.MemoryCuesChanged -= a;
            AccessibilitySettingsStore.ReduceMotionChanged -= b;
            Assert.AreEqual(0, MemCueSubs());
            Assert.AreEqual(0, ReduceSubs());
        }

        [Test]
        public void AccessibilitySettings_RepeatedSubscribeUnsubscribe_DoesNotGrow()
        {
            int baseline = MemCueSubs();
            for (int i = 0; i < 50; i++)
            {
                Action<bool> h = _ => { };
                AccessibilitySettingsStore.MemoryCuesChanged += h;
                AccessibilitySettingsStore.MemoryCuesChanged -= h;
            }
            Assert.AreEqual(baseline, MemCueSubs());
        }

        [Test]
        public void AnalyticsBuffer_RemainsCapped()
        {
            AnalyticsService.SetSink(new DebugAnalyticsSink());
            for (int i = 0; i < 200; i++) AnalyticsService.Track("perf_soak_event");
            Assert.LessOrEqual(AnalyticsService.Recent.Count, DebugAnalyticsSink.BufferCapacity);
        }
    }
}
