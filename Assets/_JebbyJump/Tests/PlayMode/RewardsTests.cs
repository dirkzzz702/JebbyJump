using JebbyJump.Rewards;
using NUnit.Framework;
using UnityEngine;

namespace JebbyJump.Tests
{
    // Pure logic for the Stars-only mastery reward foundation.
    // SetUp/TearDown clear the store's key range so tests do not leak or
    // disturb a developer's local stars.
    public class RewardsTests
    {
        private const int LevelCount = 10;

        [SetUp]
        public void SetUp() => StarRewardStore.ResetAll(LevelCount);

        [TearDown]
        public void TearDown() => StarRewardStore.ResetAll(LevelCount);

        // ---- StarRewardCalculator ----

        [Test]
        public void StarsForRank_NotCompleted_IsZero()
        {
            Assert.AreEqual(0, StarRewardCalculator.StarsForRank("S", false));
            Assert.AreEqual(0, StarRewardCalculator.StarsForRank(null, false));
        }

        [Test]
        public void StarsForRank_SandA_AreThree()
        {
            Assert.AreEqual(3, StarRewardCalculator.StarsForRank("S", true));
            Assert.AreEqual(3, StarRewardCalculator.StarsForRank("A", true));
        }

        [Test]
        public void StarsForRank_B_IsTwo()
        {
            Assert.AreEqual(2, StarRewardCalculator.StarsForRank("B", true));
        }

        [Test]
        public void StarsForRank_C_IsOne()
        {
            Assert.AreEqual(1, StarRewardCalculator.StarsForRank("C", true));
        }

        [Test]
        public void StarsForRank_UnknownOrEmptyButCompleted_IsOne()
        {
            Assert.AreEqual(1, StarRewardCalculator.StarsForRank("?", true));
            Assert.AreEqual(1, StarRewardCalculator.StarsForRank("", true));
            Assert.AreEqual(1, StarRewardCalculator.StarsForRank(null, true));
        }

        // ---- StarRewardStore ----

        [Test]
        public void GetStars_DefaultIsZero()
        {
            Assert.AreEqual(0, StarRewardStore.GetStars(0));
        }

        [Test]
        public void SetStarsIfHigher_StoresValue()
        {
            Assert.AreEqual(2, StarRewardStore.SetStarsIfHigher(0, 2));
            Assert.AreEqual(2, StarRewardStore.GetStars(0));
        }

        [Test]
        public void SetStarsIfHigher_NeverDecreases()
        {
            StarRewardStore.SetStarsIfHigher(0, 3);
            int result = StarRewardStore.SetStarsIfHigher(0, 1);
            Assert.AreEqual(3, result);
            Assert.AreEqual(3, StarRewardStore.GetStars(0));
        }

        [Test]
        public void SetStarsIfHigher_ClampsToZeroThree()
        {
            Assert.AreEqual(3, StarRewardStore.SetStarsIfHigher(0, 5));
            Assert.AreEqual(3, StarRewardStore.GetStars(0));
        }

        [Test]
        public void NegativeIndex_IsNoOp()
        {
            Assert.AreEqual(0, StarRewardStore.SetStarsIfHigher(-1, 3));
            Assert.AreEqual(0, StarRewardStore.GetStars(-1));
        }

        [Test]
        public void GetTotalStars_SumsAcrossLevels()
        {
            StarRewardStore.SetStarsIfHigher(0, 3);
            StarRewardStore.SetStarsIfHigher(1, 2);
            StarRewardStore.SetStarsIfHigher(2, 1);
            Assert.AreEqual(6, StarRewardStore.GetTotalStars(LevelCount));
        }

        [Test]
        public void GetTotalStars_NonPositiveCount_IsZero()
        {
            StarRewardStore.SetStarsIfHigher(0, 3);
            Assert.AreEqual(0, StarRewardStore.GetTotalStars(0));
            Assert.AreEqual(0, StarRewardStore.GetTotalStars(-5));
        }

        [Test]
        public void ResetAll_ClearsRange()
        {
            StarRewardStore.SetStarsIfHigher(0, 3);
            StarRewardStore.SetStarsIfHigher(5, 2);
            StarRewardStore.ResetAll(LevelCount);
            Assert.AreEqual(0, StarRewardStore.GetStars(0));
            Assert.AreEqual(0, StarRewardStore.GetStars(5));
            Assert.AreEqual(0, StarRewardStore.GetTotalStars(LevelCount));
        }
    }
}
