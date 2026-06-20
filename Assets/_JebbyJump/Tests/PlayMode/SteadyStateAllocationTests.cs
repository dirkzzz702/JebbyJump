using JebbyJump.Core;
using JebbyJump.Shell;
using JebbyJump.Wardrobe.Visual;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools.Constraints;
using GcIs = UnityEngine.TestTools.Constraints.Is;

namespace JebbyJump.Tests
{
    // P24: steady-state per-frame paths must not allocate (reliable GC constraint).
    public class SteadyStateAllocationTests
    {
        [Test]
        public void GameplayInputBlockPolicy_ShouldBlock_DoesNotAllocate()
        {
            var _ = GameplayInputBlockPolicy.ShouldBlock(true, false, true, false); // warm
            Assert.That(() => { var r = GameplayInputBlockPolicy.ShouldBlock(true, false, true, false); },
                GcIs.Not.AllocatingGCMemory());
        }

        [Test]
        public void SafeAreaCalculator_ComputeAnchors_DoesNotAllocate()
        {
            var safe = new Rect(40, 0, 1840, 1080);
            var screen = new Vector2(1920, 1080);
            SafeAreaCalculator.ComputeAnchors(safe, screen, out _, out _); // warm
            Assert.That(() => { SafeAreaCalculator.ComputeAnchors(safe, screen, out var mn, out var mx); },
                GcIs.Not.AllocatingGCMemory());
        }

        [Test]
        public void PlatformCueMapping_CueFor_DoesNotAllocate()
        {
            var _ = PlatformCueMapping.CueFor(PlatformColor.Red); // warm (interned literals)
            Assert.That(() => { var s = PlatformCueMapping.CueFor(PlatformColor.Blue); },
                GcIs.Not.AllocatingGCMemory());
        }
    }
}
