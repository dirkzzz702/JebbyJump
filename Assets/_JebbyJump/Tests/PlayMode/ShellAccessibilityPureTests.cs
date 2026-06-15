using System.Collections.Generic;
using JebbyJump.Shell;
using JebbyJump.Wardrobe.Visual; // CanvasScalerMath (reused)
using NUnit.Framework;
using UnityEngine;

namespace JebbyJump.Tests
{
    public class ShellLayoutMetricsTests
    {
        [Test]
        public void TouchTarget_Is90_AndSingleSourced()
        {
            Assert.AreEqual(90f, ShellLayoutMetrics.MinTouchTargetCanvasUnits);
            // Wardrobe reuses the shell constant (no duplicate touch metric).
            Assert.AreEqual(ShellLayoutMetrics.MinTouchTargetCanvasUnits,
                WardrobeLayoutMetrics.MinTouchTargetCanvasUnits);
            Assert.GreaterOrEqual(ShellLayoutMetrics.ButtonHeight,
                ShellLayoutMetrics.MinTouchTargetCanvasUnits);
        }
    }

    public class ShellFocusResolverTests
    {
        [Test]
        public void FirstAvailable_ReturnsFirstTrue_ElseMinusOne()
        {
            Assert.AreEqual(1, ShellFocusResolver.FirstAvailableIndex(
                new[] { false, true, false }));
            Assert.AreEqual(0, ShellFocusResolver.FirstAvailableIndex(
                new[] { true, true }));
            Assert.AreEqual(-1, ShellFocusResolver.FirstAvailableIndex(
                new[] { false, false }));
            Assert.AreEqual(-1, ShellFocusResolver.FirstAvailableIndex(null));
        }

        [Test]
        public void PreferredOrFirst_Validates()
        {
            Assert.AreEqual(-1, ShellFocusResolver.ResolvePreferredOrFirst(0, 3));
            Assert.AreEqual(2, ShellFocusResolver.ResolvePreferredOrFirst(5, 2));
            Assert.AreEqual(0, ShellFocusResolver.ResolvePreferredOrFirst(5, -1));
            Assert.AreEqual(0, ShellFocusResolver.ResolvePreferredOrFirst(5, 9));
        }

        // Result fallback: Next->Retry->Menu ; Game Over: Retry->Menu.
        [Test]
        public void ResultFallback_PrefersFirstActiveAction()
        {
            // Final level: Next inactive -> Retry.
            Assert.AreEqual(1, ShellFocusResolver.FirstAvailableIndex(
                new[] { false, true, true })); // Next, Retry, Menu
            // Non-final: Next active.
            Assert.AreEqual(0, ShellFocusResolver.FirstAvailableIndex(
                new[] { true, true, true }));
            // Game Over: Retry first.
            Assert.AreEqual(0, ShellFocusResolver.FirstAvailableIndex(
                new[] { true, true })); // Retry, Menu
        }
    }

    public class GridNavigationBuilderTests
    {
        [Test]
        public void Neighbors_FullGrid()
        {
            // 10 items, 5 columns (rows: 0-4, 5-9).
            Assert.AreEqual(1, GridNavigationBuilder.Neighbor(0, 10, 5, GridDir.Right));
            Assert.AreEqual(-1, GridNavigationBuilder.Neighbor(4, 10, 5, GridDir.Right));
            Assert.AreEqual(-1, GridNavigationBuilder.Neighbor(0, 10, 5, GridDir.Left));
            Assert.AreEqual(-1, GridNavigationBuilder.Neighbor(0, 10, 5, GridDir.Up));
            Assert.AreEqual(5, GridNavigationBuilder.Neighbor(0, 10, 5, GridDir.Down));
            Assert.AreEqual(0, GridNavigationBuilder.Neighbor(5, 10, 5, GridDir.Up));
            Assert.AreEqual(-1, GridNavigationBuilder.Neighbor(9, 10, 5, GridDir.Down));
        }

        [Test]
        public void Neighbors_PartialLastRow()
        {
            // 12 items, 5 columns (rows: 0-4, 5-9, 10-11).
            Assert.AreEqual(11, GridNavigationBuilder.Neighbor(6, 12, 5, GridDir.Down));
            Assert.AreEqual(-1, GridNavigationBuilder.Neighbor(7, 12, 5, GridDir.Down)); // no item below
            Assert.AreEqual(-1, GridNavigationBuilder.Neighbor(11, 12, 5, GridDir.Right));
            Assert.AreEqual(11, GridNavigationBuilder.Neighbor(10, 12, 5, GridDir.Right));
            Assert.AreEqual(6, GridNavigationBuilder.Neighbor(11, 12, 5, GridDir.Up));
        }

        [Test]
        public void OutOfRange_ReturnsMinusOne()
        {
            Assert.AreEqual(-1, GridNavigationBuilder.Neighbor(-1, 10, 5, GridDir.Right));
            Assert.AreEqual(-1, GridNavigationBuilder.Neighbor(0, 10, 0, GridDir.Right));
        }
    }

    // Per-surface landscape bounds matrix (correction #3: each surface
    // validated with its OWN real element heights/sizes, not one shared proof).
    public class ShellBoundsMatrixTests
    {
        private static readonly (float w, float h, float l, float r, float b)[] Matrix =
        {
            (1920f, 1080f, 0f, 0f, 0f),
            (2160f, 1080f, 0f, 0f, 0f),
            (2340f, 1080f, 0f, 0f, 0f),
            (2400f, 1080f, 0f, 0f, 0f),
            (1440f, 1080f, 0f, 0f, 0f),
            (1920f, 1080f, 100f, 0f, 0f),
            (1920f, 1080f, 0f, 100f, 0f),
            (1920f, 1080f, 0f, 0f, 44f),
            (2400f, 1080f, 100f, 0f, 44f),
        };

        private static Vector2 Content(float sw, float sh, float l, float r, float b)
        {
            Vector2 canvas = CanvasScalerMath.LogicalCanvasSize(
                new Vector2(sw, sh), new Vector2(1920f, 1080f), 0.5f);
            float scale = canvas.x / sw;
            return new Vector2((sw - l - r) * scale, (sh - b) * scale);
        }

        private const float Pad = 40f;   // ShellLayoutMetrics.EdgePadding
        private const float Sp = 24f;     // StackSpacing
        private const float Btn = 90f;    // ButtonHeight
        private const float Title = 90f;
        private const float Txt = 48f;

        private void AssertStackFitsMatrix(
            List<float> elems, float maxWidth, string name)
        {
            foreach (var c in Matrix)
            {
                var content = Content(c.w, c.h, c.l, c.r, c.b);
                Assert.IsTrue(
                    ShellStackLayoutPolicy.Fits(content, Pad, Sp, elems, maxWidth),
                    $"{name} does not fit {c.w}x{c.h} insets({c.l},{c.r},{c.b}) " +
                    $"content={content} reqH=" +
                    ShellStackLayoutPolicy.RequiredHeight(Pad, Sp, elems));
            }
        }

        [Test]
        public void MainMenu_FitsApprovedLandscapeMatrix()
            => AssertStackFitsMatrix(
                new List<float> { Btn, Btn, Btn, Btn, Btn }, // 5 buttons
                ShellLayoutMetrics.MenuButtonWidth, "MainMenu");

        [Test]
        public void Pause_FitsApprovedLandscapeMatrix()
            => AssertStackFitsMatrix(
                new List<float> { Title, Btn, Btn, Btn, Btn }, // title + 4
                ShellLayoutMetrics.MenuButtonWidth, "Pause");

        [Test]
        public void Settings_FitsApprovedLandscapeMatrix()
            => AssertStackFitsMatrix(
                new List<float> { Title, Btn, Btn, Btn, Btn, Btn }, // title+5 rows
                ShellLayoutMetrics.SettingsRowWidth, "Settings");

        [Test]
        public void Result_FitsApprovedLandscapeMatrix()
            => AssertStackFitsMatrix(
                new List<float> { Title, Txt, Txt, Txt, Txt, Btn, Btn, Btn },
                600f, "Result");

        [Test]
        public void GameOver_FitsApprovedLandscapeMatrix()
            => AssertStackFitsMatrix(
                new List<float> { Title, Btn, Btn }, // title + Retry + Menu
                ShellLayoutMetrics.MenuButtonWidth, "GameOver");

        [Test]
        public void LevelSelect_GridFitsApprovedLandscapeMatrix()
        {
            var spec = new ShellGridSpec(
                10, ShellLayoutMetrics.LevelSelectColumns,
                new Vector2(ShellLayoutMetrics.LevelSelectCellWidth,
                    ShellLayoutMetrics.LevelSelectCellHeight),
                ShellLayoutMetrics.GridSpacing, ShellLayoutMetrics.GridPadding,
                Title, Btn);
            foreach (var c in Matrix)
            {
                var content = Content(c.w, c.h, c.l, c.r, c.b);
                Assert.IsTrue(ShellGridLayoutPolicy.FitsScrolling(content, spec),
                    $"LevelSelect grid does not fit {c.w}x{c.h} content={content} " +
                    $"reqW={spec.RequiredWidth}");
            }
        }
    }
}
