using System.Collections.Generic;
using JebbyJump.Wardrobe;
using JebbyJump.Wardrobe.Visual;
using NUnit.Framework;
using UnityEngine;

namespace JebbyJump.Tests
{
    // P15: pure tests for the wardrobe preview library + row-model builder
    // (UI-only; no scene, no MonoBehaviour). The WardrobePanelController
    // itself is Assembly-CSharp and not test-reachable, so these pin the
    // data/logic it renders.
    public class WardrobePreviewTests
    {
        private readonly List<Object> _temp = new List<Object>();

        [TearDown]
        public void TearDown()
        {
            foreach (var o in _temp) if (o != null) Object.DestroyImmediate(o);
            _temp.Clear();
        }

        private Sprite MakeSprite()
        {
            var tex = new Texture2D(4, 4);
            _temp.Add(tex);
            var s = Sprite.Create(
                tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f));
            _temp.Add(s);
            return s;
        }

        private WardrobePreviewLibrary MakeLibrary()
        {
            var lib = ScriptableObject.CreateInstance<WardrobePreviewLibrary>();
            _temp.Add(lib);
            return lib;
        }

        private static WardrobeOutfitRowModel Row(
            IReadOnlyList<WardrobeOutfitRowModel> rows, string id)
        {
            foreach (var r in rows) if (r.OutfitId == id) return r;
            Assert.Fail("no row for " + id);
            return default;
        }

        // ---- WardrobePreviewLibrary ----

        [Test]
        public void Library_TryGetPreview_ReturnsSetSprite()
        {
            var lib = MakeLibrary();
            var s = MakeSprite();
            lib.AddEntry("forest_cavalier", s);
            Assert.IsTrue(lib.TryGetPreview("forest_cavalier", out var got));
            Assert.AreSame(s, got);
        }

        [Test]
        public void Library_MissingOrNullSprite_ReturnsFalse()
        {
            var lib = MakeLibrary();
            Assert.IsFalse(lib.TryGetPreview("forest_cavalier", out _));
            lib.AddEntry("forest_cavalier", null);
            Assert.IsFalse(lib.TryGetPreview("forest_cavalier", out _));
        }

        [Test]
        public void Library_AddEntry_ReplacesByIdKeepsUnique()
        {
            var lib = MakeLibrary();
            var a = MakeSprite();
            var b = MakeSprite();
            lib.AddEntry("x", a);
            lib.AddEntry("x", b);
            Assert.AreEqual(1, lib.Count);
            Assert.IsTrue(lib.TryGetPreview("x", out var got));
            Assert.AreSame(b, got);
        }

        // ---- WardrobeRowModelBuilder ----

        // Count tracks the catalog (not hardcoded 5 or 8); ids in order.
        [Test]
        public void Builder_OneRowPerCatalogOutfitInOrder()
        {
            var rows = WardrobeRowModelBuilder.Build(
                WardrobeCatalog.DefaultOutfitId, 0, null);
            Assert.AreEqual(WardrobeCatalog.Outfits.Count, rows.Count);
            for (int i = 0; i < rows.Count; i++)
                Assert.AreEqual(WardrobeCatalog.Outfits[i].Id, rows[i].OutfitId);
        }

        [Test]
        public void Builder_LockedOutfit_CannotEquip()
        {
            var rows = WardrobeRowModelBuilder.Build(
                WardrobeCatalog.DefaultOutfitId, 0, null);
            var silver = Row(rows, "silver_dreamer");
            Assert.IsFalse(silver.IsUnlocked);
            Assert.IsFalse(silver.CanEquip);
            Assert.AreEqual("Locked (30 Stars)", silver.StateText);
        }

        [Test]
        public void Builder_UnlockedNotEquipped_CanEquip()
        {
            var rows = WardrobeRowModelBuilder.Build(
                WardrobeCatalog.DefaultOutfitId, 8, null);
            var forest = Row(rows, "forest_cavalier");
            Assert.IsTrue(forest.IsUnlocked);
            Assert.IsFalse(forest.IsEquipped);
            Assert.IsTrue(forest.CanEquip);
            Assert.AreEqual("Unlocked", forest.StateText);
        }

        [Test]
        public void Builder_EquippedOutfit_DisablesEquip()
        {
            var rows = WardrobeRowModelBuilder.Build("forest_cavalier", 8, null);
            var forest = Row(rows, "forest_cavalier");
            Assert.IsTrue(forest.IsEquipped);
            Assert.IsFalse(forest.CanEquip);
            Assert.AreEqual("Equipped", forest.StateText);
        }

        // Contract: builder normalizes a locked/unknown equipped id to default.
        [Test]
        public void Builder_NormalizesLockedEquippedToDefault()
        {
            var rows = WardrobeRowModelBuilder.Build("silver_dreamer", 0, null);
            Assert.IsTrue(Row(rows, WardrobeCatalog.DefaultOutfitId).IsEquipped);
            Assert.IsFalse(Row(rows, "silver_dreamer").IsEquipped);
        }

        [Test]
        public void Builder_UnknownEquippedNormalizesToDefault()
        {
            var rows = WardrobeRowModelBuilder.Build("does_not_exist", 100, null);
            Assert.IsTrue(Row(rows, WardrobeCatalog.DefaultOutfitId).IsEquipped);
        }

        [Test]
        public void Builder_NullLibrary_AllPreviewsNull_NoCrash()
        {
            var rows = WardrobeRowModelBuilder.Build(
                WardrobeCatalog.DefaultOutfitId, 100, null);
            foreach (var r in rows) Assert.IsNull(r.PreviewSprite);
        }

        [Test]
        public void Builder_PreviewFromLibrary_MissingEntryIsNull()
        {
            var lib = MakeLibrary();
            var s = MakeSprite();
            lib.AddEntry("forest_cavalier", s);
            var rows = WardrobeRowModelBuilder.Build(
                WardrobeCatalog.DefaultOutfitId, 100, lib);
            Assert.AreSame(s, Row(rows, "forest_cavalier").PreviewSprite);
            Assert.IsNull(Row(rows, "aqua_knight").PreviewSprite);
        }

        // ---- P16 "New" badge (IsNew) ----

        [Test]
        public void Builder_IsNew_TrueForUnlockedUnacknowledgedNonDefault()
        {
            var rows = WardrobeRowModelBuilder.Build(
                WardrobeCatalog.DefaultOutfitId, 8, null, _ => false);
            Assert.IsTrue(Row(rows, "forest_cavalier").IsNew);
        }

        [Test]
        public void Builder_IsNew_FalseForLocked()
        {
            var rows = WardrobeRowModelBuilder.Build(
                WardrobeCatalog.DefaultOutfitId, 8, null, _ => false);
            Assert.IsFalse(Row(rows, "silver_dreamer").IsNew);
        }

        [Test]
        public void Builder_IsNew_FalseForAcknowledged()
        {
            var rows = WardrobeRowModelBuilder.Build(
                WardrobeCatalog.DefaultOutfitId, 8, null,
                id => id == "forest_cavalier");
            Assert.IsFalse(Row(rows, "forest_cavalier").IsNew);
        }

        [Test]
        public void Builder_IsNew_FalseForDefaultAndNullPredicate()
        {
            var rowsNull = WardrobeRowModelBuilder.Build(
                WardrobeCatalog.DefaultOutfitId, 100, null);
            foreach (var r in rowsNull) Assert.IsFalse(r.IsNew, r.OutfitId);

            var rows = WardrobeRowModelBuilder.Build(
                WardrobeCatalog.DefaultOutfitId, 100, null, _ => false);
            Assert.IsFalse(Row(rows, WardrobeCatalog.DefaultOutfitId).IsNew);
        }
    }

#if UNITY_EDITOR
    // Validates the REAL WardrobePreviewLibrary.asset: catalog-complete,
    // unique, no unknown ids, every entry non-null and matching the known
    // idle sprite. Editor-only (AssetDatabase); read-only on assets.
    public class WardrobePreviewAssetIntegrityTests
    {
        private const string LibraryPath =
            "Assets/_JebbyJump/Art/Characters/Jebby/Outfits/WardrobePreviewLibrary.asset";
        private const string OutfitsRoot =
            "Assets/_JebbyJump/Art/Characters/Jebby/Outfits/";

        private static WardrobePreviewLibrary Load()
        {
            var lib = UnityEditor.AssetDatabase
                .LoadAssetAtPath<WardrobePreviewLibrary>(LibraryPath);
            Assert.IsNotNull(lib, "preview library asset missing at " + LibraryPath);
            return lib;
        }

        [Test]
        public void PreviewLibrary_UniqueCatalogCompleteNoUnknownIds()
        {
            var lib = Load();
            var ids = lib.EntryIds();
            Assert.AreEqual(WardrobeCatalog.Outfits.Count, ids.Count,
                "entry count must equal catalog (complete, no extras)");
            var seen = new HashSet<string>();
            foreach (var id in ids)
            {
                Assert.IsTrue(seen.Add(id), "duplicate entry id: " + id);
                Assert.IsTrue(WardrobeCatalog.Exists(id), "unknown id: " + id);
            }
            foreach (var o in WardrobeCatalog.Outfits)
            {
                Assert.IsTrue(lib.TryGetPreview(o.Id, out var s),
                    o.Id + " has no preview sprite");
                Assert.IsNotNull(s, o.Id);
            }
        }

        [Test]
        public void PreviewLibrary_SpritesMatchKnownIdleSprites()
        {
            var lib = Load();
            foreach (var o in WardrobeCatalog.Outfits)
            {
                lib.TryGetPreview(o.Id, out var sprite);
                string path = IdlePath(o.Id);
                var expected = UnityEditor.AssetDatabase
                    .LoadAssetAtPath<Sprite>(path);
                Assert.IsNotNull(expected, "idle sprite missing: " + path);
                Assert.AreSame(expected, sprite,
                    o.Id + " preview is not its idle sprite");
            }
        }

        // P18: every outfit must have all 7 pose sprites, each matching the
        // expected per-state asset path.
        [Test]
        public void PreviewLibrary_AllOutfitsHaveAllPoseSpritesMatchingPaths()
        {
            var lib = Load();
            foreach (var o in WardrobeCatalog.Outfits)
            {
                foreach (WardrobePreviewPose pose in
                    System.Enum.GetValues(typeof(WardrobePreviewPose)))
                {
                    Assert.IsTrue(lib.TryGetPose(o.Id, pose, out var sprite),
                        o.Id + "/" + pose + " missing");
                    string path = PosePath(o.Id, pose);
                    var expected = UnityEditor.AssetDatabase
                        .LoadAssetAtPath<Sprite>(path);
                    Assert.IsNotNull(expected, "sprite missing: " + path);
                    Assert.AreSame(expected, sprite, o.Id + "/" + pose);
                }
            }
        }

        private static string IdlePath(string outfitId)
            => PosePath(outfitId, WardrobePreviewPose.Idle);

        private static string PosePath(string outfitId, WardrobePreviewPose pose)
        {
            string state = pose.ToString().ToLowerInvariant();
            if (outfitId == WardrobeCatalog.DefaultOutfitId)
                return "Assets/_JebbyJump/Art/Sprites/Characters/Jebby/spr_jebby_"
                    + state + "_01.png";
            return OutfitsRoot + ToPascal(outfitId) + "/Sprites/spr_jebby_"
                + outfitId + "_" + state + "_01.png";
        }

        private static string ToPascal(string snake)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var part in snake.Split('_'))
            {
                if (part.Length == 0) continue;
                sb.Append(char.ToUpperInvariant(part[0]));
                if (part.Length > 1) sb.Append(part.Substring(1));
            }
            return sb.ToString();
        }
    }
#endif
}
