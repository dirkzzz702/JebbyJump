using System.Reflection;
using System.Text.RegularExpressions;
using JebbyJump.Analytics;
using NUnit.Framework;

namespace JebbyJump.Tests
{
    // Guards the central catalog: every event name / parameter key const
    // must be a non-empty snake_case wire string. Reflection keeps this in
    // sync automatically as the catalog grows.
    public class AnalyticsCatalogTests
    {
        private static readonly Regex SnakeCase =
            new Regex("^[a-z][a-z0-9_]*$");

        [Test]
        public void AnalyticsEvents_AreNonEmptySnakeCase()
        {
            AssertAllConstStringsSnakeCase(typeof(AnalyticsEvents));
        }

        [Test]
        public void AnalyticsParams_AreNonEmptySnakeCase()
        {
            AssertAllConstStringsSnakeCase(typeof(AnalyticsParams));
        }

        // Pins the reward analytics wire names so a rename can't silently
        // break the contract a future provider/dashboard depends on. The
        // generic snake_case test above would still pass on a rename.
        [Test]
        public void RewardConstants_HaveStableWireNames()
        {
            Assert.AreEqual("reward_granted", AnalyticsEvents.RewardGranted);
            Assert.AreEqual("star_total_changed", AnalyticsEvents.StarTotalChanged);
            Assert.AreEqual("reward_type", AnalyticsParams.RewardType);
            Assert.AreEqual("amount", AnalyticsParams.Amount);
            Assert.AreEqual("total_for_level", AnalyticsParams.TotalForLevel);
            Assert.AreEqual("previous_for_level", AnalyticsParams.PreviousForLevel);
            Assert.AreEqual("old_total", AnalyticsParams.OldTotal);
            Assert.AreEqual("new_total", AnalyticsParams.NewTotal);
            Assert.AreEqual("delta", AnalyticsParams.Delta);
        }

        // Pins the wardrobe analytics wire names (P9) for the same
        // contract-stability reason as the reward names above.
        [Test]
        public void WardrobeConstants_HaveStableWireNames()
        {
            Assert.AreEqual("wardrobe_opened", AnalyticsEvents.WardrobeOpened);
            Assert.AreEqual("cosmetic_previewed", AnalyticsEvents.CosmeticPreviewed);
            Assert.AreEqual("cosmetic_equipped", AnalyticsEvents.CosmeticEquipped);
            Assert.AreEqual("cosmetic_unlock_failed", AnalyticsEvents.CosmeticUnlockFailed);
            Assert.AreEqual("cosmetic_id", AnalyticsParams.CosmeticId);
            Assert.AreEqual("cosmetic_category", AnalyticsParams.CosmeticCategory);
            Assert.AreEqual("required_stars", AnalyticsParams.RequiredStars);
            Assert.AreEqual("current_stars", AnalyticsParams.CurrentStars);
            Assert.AreEqual("is_owned", AnalyticsParams.IsOwned);
            // P16 unlock ceremony.
            Assert.AreEqual("cosmetic_unlock_presented", AnalyticsEvents.CosmeticUnlockPresented);
            Assert.AreEqual("cosmetic_unlock_acknowledged", AnalyticsEvents.CosmeticUnlockAcknowledged);
            Assert.AreEqual("queue_position", AnalyticsParams.QueuePosition);
            Assert.AreEqual("queue_count", AnalyticsParams.QueueCount);
            Assert.AreEqual("acknowledgement_action", AnalyticsParams.AcknowledgementAction);
        }

        private static void AssertAllConstStringsSnakeCase(System.Type type)
        {
            var consts = type.GetFields(
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            int checkedCount = 0;
            foreach (var f in consts)
            {
                if (!f.IsLiteral || f.IsInitOnly) continue;
                if (f.FieldType != typeof(string)) continue;
                var value = (string)f.GetRawConstantValue();
                Assert.IsFalse(string.IsNullOrEmpty(value),
                    type.Name + "." + f.Name + " is empty.");
                Assert.IsTrue(SnakeCase.IsMatch(value),
                    type.Name + "." + f.Name + " = '" + value
                    + "' is not snake_case.");
                checkedCount++;
            }
            Assert.Greater(checkedCount, 0,
                "Expected at least one const string in " + type.Name);
        }
    }
}
