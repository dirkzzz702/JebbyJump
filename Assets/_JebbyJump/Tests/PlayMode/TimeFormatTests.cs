using System.Text;
using JebbyJump.Core;
using NUnit.Framework;
using UnityEngine.TestTools.Constraints;
using GcIs = UnityEngine.TestTools.Constraints.Is;

namespace JebbyJump.Tests
{
    public class TimeFormatTests
    {
        private static string OldClock(float seconds)
        {
            if (float.IsNaN(seconds) || seconds < 0f) seconds = 0f;
            int minutes = (int)(seconds / 60f);
            float rest = seconds - minutes * 60f;
            return $"{minutes:00}:{rest:00.00}";
        }

        private static string Clock(StringBuilder sb, float seconds)
        {
            sb.Clear();
            TimeFormat.AppendClock(sb, seconds);
            return sb.ToString();
        }

        private static string F1(StringBuilder sb, float v)
        {
            sb.Clear();
            TimeFormat.AppendF1(sb, v);
            return sb.ToString();
        }

        [Test]
        public void AppendClock_MatchesKnownValues()
        {
            var sb = new StringBuilder();
            Assert.AreEqual("00:00.00", Clock(sb, 0f));
            Assert.AreEqual("00:12.34", Clock(sb, 12.34f));
            Assert.AreEqual("01:05.00", Clock(sb, 65f));
            Assert.AreEqual("10:00.00", Clock(sb, 600f));
            Assert.AreEqual("00:12.13", Clock(sb, 12.125f)); // half rounds away from zero
            Assert.AreEqual("00:00.00", Clock(sb, float.NaN));
            Assert.AreEqual("00:00.00", Clock(sb, -5f));
        }

        [Test]
        public void AppendF1_MatchesKnownValues()
        {
            var sb = new StringBuilder();
            Assert.AreEqual("0.0", F1(sb, 0f));
            Assert.AreEqual("3.2", F1(sb, 3.2f));
            Assert.AreEqual("0.3", F1(sb, 0.25f)); // half rounds away from zero
        }

        // Correction #7: exact text equivalence at formatting boundaries.
        [Test]
        public void AppendClock_ExactlyEqualsOldExpression_AcrossDenseSweep()
        {
            var sb = new StringBuilder();
            int mismatches = 0; string first = null;
            for (int cs = 0; cs <= 600_00; cs++)
            {
                float t = cs / 100f;
                string a = OldClock(t), b = Clock(sb, t);
                if (a != b) { mismatches++; first ??= $"t={t}: old='{a}' new='{b}'"; }
            }
            Assert.AreEqual(0, mismatches, "first mismatch: " + first);
        }

        [Test]
        public void AppendF1_ExactlyEqualsOldExpression_AcrossDenseSweep()
        {
            var sb = new StringBuilder();
            int mismatches = 0; string first = null;
            for (int dm = 0; dm <= 30_000; dm++)
            {
                float x = dm / 1000f;
                string a = $"{x:F1}", b = F1(sb, x);
                if (a != b) { mismatches++; first ??= $"x={x}: old='{a}' new='{b}'"; }
            }
            Assert.AreEqual(0, mismatches, "first mismatch: " + first);
        }

        // Correction #2: reliable GC-allocation assertion (editor Boehm GC does not
        // track GC.GetAllocatedBytesForCurrentThread). Warm once, then assert the
        // reused-builder path allocates nothing.
        [Test]
        public void AppendClock_DoesNotAllocate()
        {
            var sb = new StringBuilder(16);
            sb.Clear(); TimeFormat.AppendClock(sb, 83.27f); // warm
            Assert.That(() => { sb.Clear(); TimeFormat.AppendClock(sb, 83.27f); },
                GcIs.Not.AllocatingGCMemory());
        }

        [Test]
        public void AppendF1_DoesNotAllocate()
        {
            var sb = new StringBuilder(8);
            sb.Clear(); TimeFormat.AppendF1(sb, 3.2f); // warm
            Assert.That(() => { sb.Clear(); TimeFormat.AppendF1(sb, 3.2f); },
                GcIs.Not.AllocatingGCMemory());
        }
    }
}
