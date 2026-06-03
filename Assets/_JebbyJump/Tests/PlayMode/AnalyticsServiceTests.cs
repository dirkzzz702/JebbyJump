using System.Collections.Generic;
using JebbyJump.Analytics;
using NUnit.Framework;

namespace JebbyJump.Tests
{
    // Pure-logic coverage for the analytics facade: null/disabled safety,
    // empty-name guard, sink dispatch, and the debug sink buffer. No scene
    // or UI dependencies.
    public class AnalyticsServiceTests
    {
        // Minimal capturing sink for assertions.
        private sealed class CaptureSink : IAnalyticsSink
        {
            public readonly List<AnalyticsEvent> Events =
                new List<AnalyticsEvent>();
            public void Track(in AnalyticsEvent e) => Events.Add(e);
        }

        private bool _savedEnabled;

        [SetUp]
        public void SetUp()
        {
            _savedEnabled = AnalyticsService.Enabled;
            AnalyticsService.Enabled = true;
            AnalyticsService.ResetForTests();
        }

        [TearDown]
        public void TearDown()
        {
            AnalyticsService.Enabled = _savedEnabled;
            // Restore a default debug sink so other suites are unaffected.
            AnalyticsService.SetSink(new DebugAnalyticsSink());
            AnalyticsService.ResetForTests();
        }

        [Test]
        public void Track_EmptyOrWhitespaceName_IsIgnored()
        {
            var sink = new CaptureSink();
            AnalyticsService.SetSink(sink);

            AnalyticsService.Track("");
            AnalyticsService.Track("   ");
            AnalyticsService.Track(null);

            Assert.IsFalse(
                sink.Events.Exists(e => string.IsNullOrWhiteSpace(e.Name)),
                "No empty-named event should reach the sink.");
        }

        [Test]
        public void Track_WhenDisabled_EmitsNothing()
        {
            var sink = new CaptureSink();
            AnalyticsService.SetSink(sink);
            AnalyticsService.Enabled = false;

            AnalyticsService.Track("level_started");

            Assert.AreEqual(0, sink.Events.Count);
        }

        [Test]
        public void Track_NullSink_IsSafeNoOp()
        {
            AnalyticsService.SetSink(null);
            // Must not throw even though no sink is set.
            Assert.DoesNotThrow(() => AnalyticsService.Track("level_started"));
        }

        [Test]
        public void Track_DeliversNameAndParams_ToSink()
        {
            var sink = new CaptureSink();
            AnalyticsService.SetSink(sink);

            AnalyticsService.Track("level_completed",
                AnalyticsParam.Of("level_index", 2),
                AnalyticsParam.Of("rank", "A"));

            // Find our event (app_session_started may precede it).
            var e = sink.Events.Find(x => x.Name == "level_completed");
            Assert.AreEqual("level_completed", e.Name);
            Assert.AreEqual(2, e.Parameters.Count);
            Assert.AreEqual("level_index", e.Parameters[0].Key);
            Assert.AreEqual(2, e.Parameters[0].Value);
            Assert.AreEqual("rank", e.Parameters[1].Key);
            Assert.AreEqual("A", e.Parameters[1].Value);
        }

        [Test]
        public void FirstTrack_EmitsAppSessionStartedOnce()
        {
            var sink = new CaptureSink();
            AnalyticsService.SetSink(sink);

            AnalyticsService.Track("level_started");
            AnalyticsService.Track("level_started");

            int sessionStarts = sink.Events.FindAll(
                e => e.Name == AnalyticsService.EventAppSessionStarted).Count;
            Assert.AreEqual(1, sessionStarts,
                "app_session_started must be emitted exactly once per session.");
        }

        [Test]
        public void DebugSink_BuffersEvents()
        {
            var sink = new DebugAnalyticsSink();
            AnalyticsService.SetSink(sink);

            AnalyticsService.Track("pause_opened");

            Assert.IsTrue(sink.Recent.Count > 0);
            Assert.IsTrue(
                ListContainsName(sink.Recent, "pause_opened"));
        }

        [Test]
        public void Sanitize_DropsNullAndUnsupportedValues_KeepsPrimitives()
        {
            var sink = new CaptureSink();
            AnalyticsService.SetSink(sink);

            // Built via the public object ctor with a mix of values.
            Assert.DoesNotThrow(() => AnalyticsService.Track("evt",
                new AnalyticsParam("good_int", 7),
                new AnalyticsParam("good_str", "ok"),
                new AnalyticsParam("null_val", null),
                new AnalyticsParam("bad_obj", new object())));

            var e = sink.Events.Find(x => x.Name == "evt");
            Assert.AreEqual("evt", e.Name);
            // Only the two supported primitives survive sanitization.
            Assert.AreEqual(2, e.Parameters.Count);
            Assert.IsTrue(HasKey(e.Parameters, "good_int"));
            Assert.IsTrue(HasKey(e.Parameters, "good_str"));
            Assert.IsFalse(HasKey(e.Parameters, "null_val"));
            Assert.IsFalse(HasKey(e.Parameters, "bad_obj"));
        }

        [Test]
        public void DebugSink_BufferCapsAtCapacity()
        {
            var sink = new DebugAnalyticsSink();
            AnalyticsService.SetSink(sink);

            for (int i = 0; i < DebugAnalyticsSink.BufferCapacity + 40; i++)
                AnalyticsService.Track("level_started");

            Assert.AreEqual(
                DebugAnalyticsSink.BufferCapacity, sink.Recent.Count);
        }

        [Test]
        public void SetSink_DoesNotReEmitAppSessionStarted()
        {
            var first = new CaptureSink();
            AnalyticsService.SetSink(first);
            AnalyticsService.Track("level_started"); // starts the session

            var second = new CaptureSink();
            AnalyticsService.SetSink(second);
            AnalyticsService.Track("pause_opened");

            Assert.IsFalse(
                second.Events.Exists(
                    e => e.Name == AnalyticsService.EventAppSessionStarted),
                "Swapping sinks must not re-emit app_session_started.");
            Assert.IsTrue(
                second.Events.Exists(e => e.Name == "pause_opened"));
        }

        private static bool HasKey(
            IReadOnlyList<AnalyticsParam> ps, string key)
        {
            for (int i = 0; i < ps.Count; i++)
                if (ps[i].Key == key) return true;
            return false;
        }

        private static bool ListContainsName(
            IReadOnlyList<AnalyticsEvent> list, string name)
        {
            for (int i = 0; i < list.Count; i++)
                if (list[i].Name == name) return true;
            return false;
        }
    }
}
