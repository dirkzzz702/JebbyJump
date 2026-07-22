using System.Collections.Generic;
using JebbyJump.Core;
using JebbyJump.Rewards;
using NUnit.Framework;

namespace JebbyJump.Tests.EditMode
{
    // P34G world-reward foundation: the World Gem store (per-world trophy,
    // set-once) and the pure mastery/finale logic. PlayerPrefs is available in
    // EditMode; keys are cleaned up in TearDown so a real save is untouched.
    public class WorldRewardTests
    {
        private static readonly List<string> Worlds = new List<string>
        {
            "W01","W02","W03","W04","W05","W06","W07","W08","W09","W10",
        };

        [TearDown]
        public void TearDown() => WorldGemStore.ResetAll(Worlds);

        // ---- WorldGemStore ----

        [Test]
        public void Gem_TryGrant_GrantsOnce_ThenIsIdempotent()
        {
            Assert.IsFalse(WorldGemStore.IsGranted("W03"));
            Assert.IsTrue(WorldGemStore.TryGrant("W03"), "first grant returns true");
            Assert.IsTrue(WorldGemStore.IsGranted("W03"));
            Assert.IsFalse(WorldGemStore.TryGrant("W03"), "replay grants nothing");
            Assert.IsTrue(WorldGemStore.IsGranted("W03"), "still held");
        }

        [Test]
        public void Gem_NullOrEmptyId_Ignored()
        {
            Assert.IsFalse(WorldGemStore.TryGrant(null));
            Assert.IsFalse(WorldGemStore.TryGrant(""));
            Assert.IsFalse(WorldGemStore.IsGranted(null));
        }

        [Test]
        public void Gem_TotalGranted_CountsHeldWorlds()
        {
            Assert.AreEqual(0, WorldGemStore.TotalGranted(Worlds));
            WorldGemStore.TryGrant("W01");
            WorldGemStore.TryGrant("W05");
            WorldGemStore.TryGrant("W10");
            Assert.AreEqual(3, WorldGemStore.TotalGranted(Worlds));
        }

        [Test]
        public void Gem_Clear_RemovesJustThatWorld()
        {
            WorldGemStore.TryGrant("W02");
            WorldGemStore.TryGrant("W04");
            WorldGemStore.Clear("W02");
            Assert.IsFalse(WorldGemStore.IsGranted("W02"));
            Assert.IsTrue(WorldGemStore.IsGranted("W04"));
        }

        // ---- eligibility mirrors WorldMapping finale detection ----

        [Test]
        public void Gem_EligibleOnlyOnEachWorldsFinaleIndex()
        {
            // Finale index of world n (0-based) = n*10-1.
            for (int n = 1; n <= WorldMapping.WorldCount; n++)
            {
                int last = WorldMapping.LastLevelIndexOfWorld(n);
                Assert.IsTrue(WorldRewardLogic.IsFinaleClear(last, last));
                Assert.IsTrue(WorldMapping.IsFinaleLevelIndex(last));
                // a non-finale level in the same world is not eligible
                Assert.IsFalse(WorldRewardLogic.IsFinaleClear(last - 1, last));
            }
        }

        // ---- WorldRewardLogic.IsWorldMastered ----

        [Test]
        public void Mastery_TrueOnlyWhenEveryLevelInRangeCleared()
        {
            // World 2 = indices 10..19.
            int first = WorldMapping.FirstLevelIndexOfWorld(2);
            int last = WorldMapping.LastLevelIndexOfWorld(2);

            var cleared = new HashSet<int>();
            for (int i = first; i <= last; i++) cleared.Add(i);

            Assert.IsTrue(WorldRewardLogic.IsWorldMastered(first, last, cleared.Contains));

            cleared.Remove(last);   // one level missing
            Assert.IsFalse(WorldRewardLogic.IsWorldMastered(first, last, cleared.Contains));
        }

        [Test]
        public void Mastery_GuardsBadInput()
        {
            Assert.IsFalse(WorldRewardLogic.IsWorldMastered(0, 9, null));
            Assert.IsFalse(WorldRewardLogic.IsWorldMastered(-1, 9, i => true));
            Assert.IsFalse(WorldRewardLogic.IsWorldMastered(10, 9, i => true)); // last<first
        }

        // ---- world cosmetic catalog + store ----

        [Test]
        public void CosmeticCatalog_TenUniqueIds_MappedByWorld()
        {
            Assert.AreEqual(10, WorldCosmeticCatalog.Count);
            var ids = new HashSet<string>();
            for (int n = 1; n <= 10; n++)
            {
                string id = WorldCosmeticCatalog.CosmeticIdForWorld(n);
                Assert.IsFalse(string.IsNullOrEmpty(id), "world " + n + " has no cosmetic");
                Assert.IsTrue(ids.Add(id), "duplicate cosmetic id " + id);
                Assert.IsFalse(string.IsNullOrEmpty(
                    WorldCosmeticCatalog.DisplayNameForWorld(n)));
            }
            Assert.AreEqual(string.Empty, WorldCosmeticCatalog.CosmeticIdForWorld(0));
            Assert.AreEqual(string.Empty, WorldCosmeticCatalog.CosmeticIdForWorld(11));
        }

        [Test]
        public void Cosmetic_TryUnlock_SetOnce_AndTotals()
        {
            var ids = new List<string>(WorldCosmeticCatalog.AllIds);
            try
            {
                string w7 = WorldCosmeticCatalog.CosmeticIdForWorld(7);
                Assert.IsFalse(WorldCosmeticStore.IsUnlocked(w7));
                Assert.IsTrue(WorldCosmeticStore.TryUnlock(w7));
                Assert.IsFalse(WorldCosmeticStore.TryUnlock(w7), "replay unlocks nothing");
                Assert.IsTrue(WorldCosmeticStore.IsUnlocked(w7));

                WorldCosmeticStore.TryUnlock(WorldCosmeticCatalog.CosmeticIdForWorld(2));
                Assert.AreEqual(2, WorldCosmeticStore.TotalUnlocked(ids));
                Assert.IsFalse(WorldCosmeticStore.TryUnlock(null));
            }
            finally { WorldCosmeticStore.ResetAll(ids); }
        }
    }
}
