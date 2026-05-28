using JebbyJump.Progression;
using NUnit.Framework;
using UnityEngine;

namespace JebbyJump.Tests
{
    // Automated coverage for the P5A local-progression logic. These
    // exercise the unlock/lock/monotonic/reset invariants behind the
    // Level Select behaviour. Scene/UI flow (Start opens panel, 10 cards
    // render, card click loads the right level, best/rank text) is not
    // covered here and still needs a manual play-test.
    public class ProgressionPlayModeTests
    {
        private const string HighestUnlockedKey =
            "jebby.level.highestUnlocked";

        private bool _hadKey;
        private int _savedHighest;

        [SetUp]
        public void SetUp()
        {
            // Preserve any real progress so running tests does not wipe
            // the developer's local save.
            _hadKey = PlayerPrefs.HasKey(HighestUnlockedKey);
            _savedHighest = PlayerPrefs.GetInt(HighestUnlockedKey, 0);
            LevelProgressStore.ResetLocalProgress();
        }

        [TearDown]
        public void TearDown()
        {
            if (_hadKey)
                PlayerPrefs.SetInt(HighestUnlockedKey, _savedHighest);
            else
                PlayerPrefs.DeleteKey(HighestUnlockedKey);
            PlayerPrefs.Save();
        }

        [Test]
        public void FreshState_OnlyLevel1Unlocked()
        {
            Assert.AreEqual(0, LevelProgressStore.HighestUnlockedIndex);
            Assert.IsTrue(LevelProgressStore.IsUnlocked(0));
            for (int i = 1; i < 10; i++)
            {
                Assert.IsFalse(
                    LevelProgressStore.IsUnlocked(i),
                    $"Level index {i} must be locked on a fresh save.");
            }
        }

        [Test]
        public void CompletingLevel1_UnlocksLevel2_Only()
        {
            LevelProgressStore.UnlockNext(0);
            Assert.IsTrue(LevelProgressStore.IsUnlocked(1));
            Assert.IsFalse(LevelProgressStore.IsUnlocked(2));
        }

        [Test]
        public void SequentialClears_UnlockProgressivelyThroughLevel10()
        {
            for (int completed = 0; completed < 9; completed++)
            {
                LevelProgressStore.UnlockNext(completed);
                Assert.IsTrue(
                    LevelProgressStore.IsUnlocked(completed + 1),
                    $"Clearing {completed} should unlock {completed + 1}.");
                if (completed + 2 < 10)
                {
                    Assert.IsFalse(
                        LevelProgressStore.IsUnlocked(completed + 2),
                        $"Index {completed + 2} should still be locked.");
                }
            }
            // Index 9 is the final (Level 10) and should now be unlocked.
            Assert.IsTrue(LevelProgressStore.IsUnlocked(9));
        }

        [Test]
        public void UnlockNext_IsMonotonic_DoesNotRegress()
        {
            LevelProgressStore.UnlockNext(0); // highest -> 1
            LevelProgressStore.UnlockNext(4); // highest -> 5
            Assert.AreEqual(5, LevelProgressStore.HighestUnlockedIndex);

            // Re-clearing an earlier level must not lower the ceiling.
            // This is why Game Over / Retry (which never call UnlockNext)
            // and replaying a beaten level cannot reduce progress.
            LevelProgressStore.UnlockNext(0);
            Assert.AreEqual(5, LevelProgressStore.HighestUnlockedIndex);
            Assert.IsTrue(LevelProgressStore.IsUnlocked(5));
        }

        [Test]
        public void RepeatedUnlockSameLevel_Idempotent()
        {
            LevelProgressStore.UnlockNext(0);
            LevelProgressStore.UnlockNext(0);
            Assert.AreEqual(1, LevelProgressStore.HighestUnlockedIndex);
        }

        [Test]
        public void IsUnlocked_NegativeIndex_ReturnsFalse()
        {
            Assert.IsFalse(LevelProgressStore.IsUnlocked(-1));
        }

        [Test]
        public void ResetLocalProgress_ReturnsToFreshState()
        {
            LevelProgressStore.UnlockNext(0);
            LevelProgressStore.UnlockNext(1);
            Assert.AreEqual(2, LevelProgressStore.HighestUnlockedIndex);

            LevelProgressStore.ResetLocalProgress();
            Assert.AreEqual(0, LevelProgressStore.HighestUnlockedIndex);
            Assert.IsTrue(LevelProgressStore.IsUnlocked(0));
            Assert.IsFalse(LevelProgressStore.IsUnlocked(1));
        }

        [Test]
        public void PendingLevelSelection_HandoffThenReset()
        {
            PendingLevelSelection.Index = 7;
            Assert.AreEqual(7, PendingLevelSelection.Index);

            PendingLevelSelection.Reset();
            Assert.AreEqual(0, PendingLevelSelection.Index);
        }
    }
}
