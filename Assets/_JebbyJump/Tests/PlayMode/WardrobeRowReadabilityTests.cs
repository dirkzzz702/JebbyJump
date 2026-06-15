using JebbyJump.Wardrobe;
using JebbyJump.Wardrobe.Visual;
using NUnit.Framework;

namespace JebbyJump.Tests
{
    // P20 readability: row view-models communicate state through non-empty text
    // (not color alone), and distinct states stay distinguishable.
    public class WardrobeRowReadabilityTests
    {
        [Test]
        public void StateCopy_IsNeverEmpty_ForEveryOutfit()
        {
            var rows = WardrobeRowModelBuilder.Build(
                WardrobeCatalog.DefaultOutfitId, 8, null, _ => false);
            foreach (var r in rows)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(r.DisplayName),
                    r.OutfitId + " display name empty");
                Assert.IsFalse(string.IsNullOrWhiteSpace(r.StateText),
                    r.OutfitId + " state text empty");
            }
        }

        [Test]
        public void LockedState_IncludesRequiredStars()
        {
            var rows = WardrobeRowModelBuilder.Build(
                WardrobeCatalog.DefaultOutfitId, 0, null);
            foreach (var r in rows)
            {
                if (r.IsUnlocked) continue;
                StringAssert.Contains(r.RequiredStars.ToString(), r.StateText,
                    r.OutfitId + " locked text missing required stars");
                StringAssert.Contains("Stars", r.StateText);
            }
        }

        // Equipped and New are independent signals: an equipped+acknowledged
        // outfit reads "Equipped" (not New), while another unlocked+
        // unacknowledged outfit reads New (not Equipped). (Per P16, equipping
        // does NOT acknowledge - semantics preserved; this only asserts the
        // two flags are distinguishable.)
        [Test]
        public void EquippedAndNew_AreDistinctSignals()
        {
            // forest equipped (8 Stars) + acknowledged; rookie unlocked + new.
            var rows = WardrobeRowModelBuilder.Build(
                "forest_cavalier", 8, null, id => id == "forest_cavalier");
            WardrobeOutfitRowModel forest = default, rookie = default;
            foreach (var r in rows)
            {
                if (r.OutfitId == "forest_cavalier") forest = r;
                if (r.OutfitId == "rookie_page") rookie = r;
            }
            Assert.IsTrue(forest.IsEquipped);
            Assert.AreEqual("Equipped", forest.StateText);
            Assert.IsFalse(forest.IsNew, "acknowledged equipped row is not New");
            Assert.IsTrue(rookie.IsNew, "unlocked unacknowledged row is New");
            Assert.IsFalse(rookie.IsEquipped);
        }

        [Test]
        public void LongestDisplayName_ProducesNonEmptyModelData()
        {
            string longestId = null;
            int max = -1;
            foreach (var o in WardrobeCatalog.Outfits)
                if (o.DisplayName.Length > max)
                { max = o.DisplayName.Length; longestId = o.Id; }

            var rows = WardrobeRowModelBuilder.Build(longestId, 100, null);
            foreach (var r in rows)
            {
                if (r.OutfitId != longestId) continue;
                Assert.IsFalse(string.IsNullOrWhiteSpace(r.DisplayName));
                Assert.IsFalse(string.IsNullOrWhiteSpace(r.StateText));
            }
        }
    }
}
