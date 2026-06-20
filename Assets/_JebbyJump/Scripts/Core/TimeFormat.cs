using System;
using System.Text;

namespace JebbyJump.Core
{
    // P24: zero-allocation formatters for the live HUD timer + skill cooldown.
    // Uses .NET's OWN formatter via Span-based TryFormat into a stack buffer, then
    // appends the chars into a reused StringBuilder. This is EXACTLY equivalent to
    // $"{minutes:00}:{rest:00.00}" / $"{value:F1}" (same formatter + culture; proven
    // by dense sweep tests) while allocating no string. A hand-rolled rounder can't
    // match .NET float formatting (it rounds the shortest round-trippable value),
    // so TryFormat is used to guarantee identical text.
    public static class TimeFormat
    {
        public static void AppendClock(StringBuilder sb, float seconds)
        {
            if (float.IsNaN(seconds) || seconds < 0f) seconds = 0f;
            int minutes = (int)(seconds / 60f);
            float rest = seconds - minutes * 60f;
            Span<char> buf = stackalloc char[16];
            AppendFormatted(sb, buf, minutes, "00");
            sb.Append(':');
            AppendFormatted(sb, buf, rest, "00.00");
        }

        public static void AppendF1(StringBuilder sb, float value)
        {
            if (float.IsNaN(value)) value = 0f;
            Span<char> buf = stackalloc char[16];
            AppendFormatted(sb, buf, value, "F1");
        }

        // Default provider (CurrentCulture) matches string interpolation; appends
        // char-by-char so no Append(ReadOnlySpan<char>) dependency is assumed.
        private static void AppendFormatted(StringBuilder sb, Span<char> buf, int v, string fmt)
        {
            if (v.TryFormat(buf, out int n, fmt))
                for (int i = 0; i < n; i++) sb.Append(buf[i]);
            else sb.Append(v.ToString(fmt));
        }

        private static void AppendFormatted(StringBuilder sb, Span<char> buf, float v, string fmt)
        {
            if (v.TryFormat(buf, out int n, fmt))
                for (int i = 0; i < n; i++) sb.Append(buf[i]);
            else sb.Append(v.ToString(fmt));
        }
    }
}
