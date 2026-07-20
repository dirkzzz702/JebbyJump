using JebbyJump.Core;
using NUnit.Framework;

namespace JebbyJump.Tests.EditMode
{
    // Pure level<->world arithmetic for the 100-level / 10-world structure
    // (WorldExpansion100, P34B). WorldMapping lives in JebbyJump.Core.Runtime,
    // which this test assembly already references.
    public class WorldMappingTests
    {
        [Test]
        public void Structure_Is100Levels_Across10Worlds()
        {
            Assert.AreEqual(10, WorldMapping.LevelsPerWorld);
            Assert.AreEqual(10, WorldMapping.WorldCount);
            Assert.AreEqual(100, WorldMapping.TotalLevels);
        }

        [Test]
        public void ExistingLevels1To10_MapToWorld1()
        {
            // Save-key safety: the shipped ten levels keep indices 0-9.
            for (int i = 0; i <= 9; i++)
                Assert.AreEqual(1, WorldMapping.WorldNumberForLevelIndex(i),
                    "level index " + i + " must belong to World 1");
        }

        [Test]
        public void WorldNumber_ForLevelIndex_IsContiguous()
        {
            Assert.AreEqual(1, WorldMapping.WorldNumberForLevelIndex(0));
            Assert.AreEqual(1, WorldMapping.WorldNumberForLevelIndex(9));
            Assert.AreEqual(2, WorldMapping.WorldNumberForLevelIndex(10));
            Assert.AreEqual(5, WorldMapping.WorldNumberForLevelIndex(49));
            Assert.AreEqual(10, WorldMapping.WorldNumberForLevelIndex(90));
            Assert.AreEqual(10, WorldMapping.WorldNumberForLevelIndex(99));
        }

        [Test]
        public void WorldNumber_OutOfRange_ReturnsZero()
        {
            Assert.AreEqual(0, WorldMapping.WorldNumberForLevelIndex(-1));
            Assert.AreEqual(0, WorldMapping.WorldNumberForLevelIndex(100));
            Assert.AreEqual(0, WorldMapping.WorldNumberForLevelIndex(int.MaxValue));
        }

        [Test]
        public void WorldRanges_CoverEveryLevelExactlyOnce()
        {
            var seen = new bool[WorldMapping.TotalLevels];
            for (int w = 1; w <= WorldMapping.WorldCount; w++)
            {
                int first = WorldMapping.FirstLevelIndexOfWorld(w);
                int last = WorldMapping.LastLevelIndexOfWorld(w);
                Assert.AreEqual(10, last - first + 1, "world " + w + " size");
                for (int i = first; i <= last; i++)
                {
                    Assert.IsFalse(seen[i], "level index " + i + " claimed twice");
                    seen[i] = true;
                    Assert.AreEqual(w, WorldMapping.WorldNumberForLevelIndex(i));
                }
            }
            for (int i = 0; i < seen.Length; i++)
                Assert.IsTrue(seen[i], "level index " + i + " unclaimed");
        }

        [Test]
        public void FirstAndLast_OfEachWorld_AreExpected()
        {
            Assert.AreEqual(0, WorldMapping.FirstLevelIndexOfWorld(1));
            Assert.AreEqual(9, WorldMapping.LastLevelIndexOfWorld(1));
            Assert.AreEqual(90, WorldMapping.FirstLevelIndexOfWorld(10));
            Assert.AreEqual(99, WorldMapping.LastLevelIndexOfWorld(10));
            Assert.AreEqual(-1, WorldMapping.FirstLevelIndexOfWorld(0));
            Assert.AreEqual(-1, WorldMapping.LastLevelIndexOfWorld(11));
        }

        [Test]
        public void LevelNumberWithinWorld_Is1To10()
        {
            Assert.AreEqual(1, WorldMapping.LevelNumberWithinWorld(0));
            Assert.AreEqual(10, WorldMapping.LevelNumberWithinWorld(9));
            Assert.AreEqual(1, WorldMapping.LevelNumberWithinWorld(10));
            Assert.AreEqual(10, WorldMapping.LevelNumberWithinWorld(99));
            Assert.AreEqual(0, WorldMapping.LevelNumberWithinWorld(-1));
        }

        [Test]
        public void ExactlyTenFinales_AtEachWorldsLastLevel()
        {
            int finales = 0;
            for (int i = 0; i < WorldMapping.TotalLevels; i++)
                if (WorldMapping.IsFinaleLevelIndex(i)) finales++;
            Assert.AreEqual(10, finales);

            Assert.IsTrue(WorldMapping.IsFinaleLevelIndex(9));   // World 1 finale
            Assert.IsTrue(WorldMapping.IsFinaleLevelIndex(99));  // World 10 finale
            Assert.IsFalse(WorldMapping.IsFinaleLevelIndex(0));
            Assert.IsFalse(WorldMapping.IsFinaleLevelIndex(98));
            Assert.IsFalse(WorldMapping.IsFinaleLevelIndex(100));
        }

        [Test]
        public void WorldId_IsStableZeroPadded()
        {
            Assert.AreEqual("W01", WorldMapping.WorldIdForNumber(1));
            Assert.AreEqual("W09", WorldMapping.WorldIdForNumber(9));
            Assert.AreEqual("W10", WorldMapping.WorldIdForNumber(10));
            Assert.AreEqual(string.Empty, WorldMapping.WorldIdForNumber(0));
            Assert.AreEqual(string.Empty, WorldMapping.WorldIdForNumber(11));
        }
    }
}
