using System;
using System.Collections.Generic;
using JebbyJump.Core;
using NUnit.Framework;

namespace JebbyJump.Tests
{
    // P22 correction #3: the PlatformColor -> cue mapping is explicit and
    // tested, shared by swatches + platforms. Guards against ordinal coupling
    // and against a new color being added without a cue.
    public class PlatformCueMappingTests
    {
        private static readonly PlatformColor[] PlayableColors =
        {
            PlatformColor.Red, PlatformColor.Blue, PlatformColor.Green,
            PlatformColor.Yellow, PlatformColor.Purple, PlatformColor.Orange,
        };

        [Test]
        public void EveryPlayableColorHasDistinctNonEmptyCue()
        {
            var seen = new HashSet<string>();
            foreach (var c in PlayableColors)
            {
                string cue = PlatformCueMapping.CueFor(c);
                Assert.IsFalse(string.IsNullOrEmpty(cue), $"{c} must have a cue");
                Assert.IsTrue(seen.Add(cue), $"{c} cue '{cue}' must be distinct");
                Assert.IsTrue(PlatformCueMapping.HasCue(c));
            }
        }

        [Test]
        public void NoneHasNoCue()
        {
            Assert.AreEqual(string.Empty, PlatformCueMapping.CueFor(PlatformColor.None));
            Assert.IsFalse(PlatformCueMapping.HasCue(PlatformColor.None));
        }

        [Test]
        public void EveryNonNoneEnumValueIsCovered()
        {
            foreach (PlatformColor c in Enum.GetValues(typeof(PlatformColor)))
            {
                if (c == PlatformColor.None) continue;
                Assert.IsFalse(string.IsNullOrEmpty(PlatformCueMapping.CueFor(c)),
                    $"PlatformColor.{c} has no cue mapping");
            }
        }

        [Test]
        public void MappingIsStable()
        {
            Assert.AreEqual("1", PlatformCueMapping.CueFor(PlatformColor.Red));
            Assert.AreEqual(PlatformCueMapping.CueFor(PlatformColor.Purple),
                            PlatformCueMapping.CueFor(PlatformColor.Purple));
        }
    }
}
