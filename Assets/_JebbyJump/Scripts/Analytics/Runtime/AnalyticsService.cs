using System;
using System.Collections.Generic;

namespace JebbyJump.Analytics
{
    // Static, scene-independent analytics facade. Lazily creates a
    // process-lifetime session id and a default DebugAnalyticsSink, and
    // self-emits "app_session_started" once on first use - so analytics
    // works regardless of entry scene (Boot, MainMenu, or Game directly in
    // the editor) and does not depend on BootController.
    //
    // Local / debug-only: no SDK, no backend, no network, no PII.
    // Null-safe and disable-able; never throws into gameplay.
    public static class AnalyticsService
    {
        public const string EventAppSessionStarted = "app_session_started";

        public static bool Enabled { get; set; } = true;

        private static IAnalyticsSink _sink;
        private static string _sessionId;
        private static bool _sessionStarted;

        // Lazily-created default so events have somewhere to go even if
        // SetSink was never called.
        private static IAnalyticsSink Sink =>
            _sink ??= new DebugAnalyticsSink();

        public static void SetSink(IAnalyticsSink sink) => _sink = sink;

        // Recent events if the active sink is the debug sink (tests/overlay).
        public static IReadOnlyList<AnalyticsEvent> Recent =>
            (Sink as DebugAnalyticsSink)?.Recent ?? Array.Empty<AnalyticsEvent>();

        public static void Track(string eventName) =>
            Track(eventName, null);

        public static void Track(string eventName, params AnalyticsParam[] parameters)
        {
            if (!Enabled) return;
            if (string.IsNullOrWhiteSpace(eventName)) return;

            EnsureSessionStarted();
            Dispatch(eventName.Trim(), parameters);
        }

        // Emits app_session_started exactly once per process (gated by
        // Enabled). Sets the flag before dispatching so the emission cannot
        // re-enter itself.
        private static void EnsureSessionStarted()
        {
            if (_sessionStarted) return;
            _sessionStarted = true;
            _sessionId ??= Guid.NewGuid().ToString("N");
            Dispatch(EventAppSessionStarted,
                new[] { AnalyticsParam.Of("session_id", _sessionId) });
        }

        private static void Dispatch(string name, AnalyticsParam[] parameters)
        {
            var sink = Sink;
            if (sink == null) return;
            var list = parameters ?? Array.Empty<AnalyticsParam>();
            sink.Track(new AnalyticsEvent(name, list));
        }

        // Test hook: restore a clean, deterministic state between tests.
        public static void ResetForTests()
        {
            _sessionStarted = false;
            _sessionId = null;
            (_sink as DebugAnalyticsSink)?.Clear();
        }
    }
}
