using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace JebbyJump.Analytics
{
    // Local, log-only sink. Writes each event to the Unity console and
    // keeps a small bounded in-memory ring buffer for tests / a future
    // debug overlay. No file writes, no network, no PII.
    public sealed class DebugAnalyticsSink : IAnalyticsSink
    {
        public const int BufferCapacity = 64;
        private readonly List<AnalyticsEvent> _buffer =
            new List<AnalyticsEvent>(BufferCapacity);

        // Most-recent-last read-only snapshot (a copy) so callers/tests
        // cannot observe later mutation of the live buffer.
        public IReadOnlyList<AnalyticsEvent> Recent =>
            new List<AnalyticsEvent>(_buffer);

        public void Track(in AnalyticsEvent e)
        {
            if (_buffer.Count >= BufferCapacity)
                _buffer.RemoveAt(0);
            _buffer.Add(e);

            Debug.Log(Format(e));
        }

        public void Clear() => _buffer.Clear();

        private static string Format(in AnalyticsEvent e)
        {
            var sb = new StringBuilder();
            sb.Append("[Analytics] ").Append(e.Name);
            var ps = e.Parameters;
            if (ps != null && ps.Count > 0)
            {
                sb.Append(" { ");
                for (int i = 0; i < ps.Count; i++)
                {
                    if (i > 0) sb.Append(", ");
                    sb.Append(ps[i].Key).Append('=').Append(ps[i].Value);
                }
                sb.Append(" }");
            }
            return sb.ToString();
        }
    }
}
