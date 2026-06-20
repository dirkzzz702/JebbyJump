using System;
using System.Text;
using JebbyJump.Core;
using NUnit.Framework;
using UnityEngine.TestTools.Constraints;
using GcIs = UnityEngine.TestTools.Constraints.Is;

namespace JebbyJump.Tests
{
    // P24 correction #1: preserved evidence that the PRE-FIX per-frame formatting
    // allocated GC memory, and that the fix removes it. The "old" expressions are
    // the exact ones HUDController.FormatTime and ActiveSkillHUD used before P24
    // (git f38a7ec). Uses the Unity Test Framework GC constraint (reliable in the
    // editor; GC.GetAllocatedBytesForCurrentThread is not tracked under Boehm).
    public class FormattingAllocationEvidenceTests
    {
        private static string OldTimer(float seconds)
        {
            int minutes = (int)(seconds / 60f);
            float rest = seconds - minutes * 60f;
            return $"{minutes:00}:{rest:00.00}";
        }

        private static string OldCooldown(float remaining) => $"{remaining:F1}";

        [Test]
        public void PreFix_OldTimerExpression_Allocates()
        {
            float t = 83.27f;
            var _ = OldTimer(t); // warm
            Assert.That(() => { var s = OldTimer(t); GC.KeepAlive(s); },
                GcIs.AllocatingGCMemory());
        }

        [Test]
        public void PreFix_OldCooldownExpression_Allocates()
        {
            float x = 3.2f;
            var _ = OldCooldown(x); // warm
            Assert.That(() => { var s = OldCooldown(x); GC.KeepAlive(s); },
                GcIs.AllocatingGCMemory());
        }

        [Test]
        public void PostFix_TimerAndCooldownFormatters_DoNotAllocate()
        {
            var sb = new StringBuilder(16);
            sb.Clear(); TimeFormat.AppendClock(sb, 83.27f);
            sb.Clear(); TimeFormat.AppendF1(sb, 3.2f); // warm both
            Assert.That(() => { sb.Clear(); TimeFormat.AppendClock(sb, 83.27f); },
                GcIs.Not.AllocatingGCMemory());
            Assert.That(() => { sb.Clear(); TimeFormat.AppendF1(sb, 3.2f); },
                GcIs.Not.AllocatingGCMemory());
        }
    }
}
