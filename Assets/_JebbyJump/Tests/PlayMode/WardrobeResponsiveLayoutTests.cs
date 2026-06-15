using JebbyJump.Wardrobe.Visual;
using NUnit.Framework;
using UnityEngine;

namespace JebbyJump.Tests
{
    // P20 mobile (landscape) accessibility: pure CanvasScaler/safe-area/layout
    // math validated across every approved landscape aspect ratio and safe-area
    // shape. No scene; objective bounds + non-overlap + touch-target checks.
    public class WardrobeResponsiveLayoutTests
    {
        private static readonly Vector2 Ref = new Vector2(1920f, 1080f);
        private const float Match = 0.5f;
        private const float Eps = 0.5f;

        // screenW, screenH, insetLeft, insetRight, insetBottom (device px).
        // Covers 16:9 / 18:9 / 19.5:9 / 20:9 / 4:3 landscape x
        // full / notch-left / notch-right / home-bottom / combined safe areas.
        [TestCase(1920f, 1080f, 0f, 0f, 0f)]
        [TestCase(2160f, 1080f, 0f, 0f, 0f)]
        [TestCase(2340f, 1080f, 0f, 0f, 0f)]
        [TestCase(2400f, 1080f, 0f, 0f, 0f)]
        [TestCase(1440f, 1080f, 0f, 0f, 0f)]
        [TestCase(1920f, 1080f, 100f, 0f, 0f)]   // notch left
        [TestCase(1920f, 1080f, 0f, 100f, 0f)]   // notch right
        [TestCase(1920f, 1080f, 0f, 0f, 44f)]    // home indicator
        [TestCase(1920f, 1080f, 100f, 0f, 44f)]  // combined
        [TestCase(2400f, 1080f, 100f, 0f, 44f)]  // tightest (20:9 + combined)
        public void Layout_WithinBounds_NonOverlapping_TouchTargetsMet(
            float sw, float sh, float insetL, float insetR, float insetB)
        {
            Vector2 canvas = CanvasScalerMath.LogicalCanvasSize(
                new Vector2(sw, sh), Ref, Match);
            float scale = canvas.x / sw; // uniform scale factor inverse
            Vector2 content = new Vector2(
                (sw - insetL - insetR) * scale, (sh - insetB) * scale);

            var l = WardrobeResponsiveLayout.Compute(content);

            AssertWithin(l.Header, content, "Header");
            AssertWithin(l.List, content, "List");
            AssertWithin(l.Preview, content, "Preview");
            AssertWithin(l.Actions, content, "Actions");

            AssertNoOverlap(l.Header, l.List, "Header/List");
            AssertNoOverlap(l.Header, l.Preview, "Header/Preview");
            AssertNoOverlap(l.Header, l.Actions, "Header/Actions");
            AssertNoOverlap(l.List, l.Preview, "List/Preview");
            AssertNoOverlap(l.List, l.Actions, "List/Actions");
            AssertNoOverlap(l.Preview, l.Actions, "Preview/Actions");

            Assert.GreaterOrEqual(l.Actions.height,
                WardrobeLayoutMetrics.MinTouchTargetCanvasUnits,
                "action region below touch minimum");
            Assert.GreaterOrEqual(l.List.height,
                WardrobeLayoutMetrics.MinListHeight, "list too short");
            Assert.Greater(l.Preview.width, 0f);
            Assert.Greater(l.Preview.height, 0f);
        }

        [Test]
        public void Layout_ShortScreenIsCompact_TallIsNot()
        {
            // 20:9 landscape -> logical height ~966 -> compact.
            Vector2 tall = CanvasScalerMath.LogicalCanvasSize(
                new Vector2(2400f, 1080f), Ref, Match);
            Assert.IsTrue(WardrobeResponsiveLayout.Compute(tall).IsCompact);

            // 16:9 -> 1080 logical height -> standard side-by-side.
            Vector2 std = CanvasScalerMath.LogicalCanvasSize(
                new Vector2(1920f, 1080f), Ref, Match);
            Assert.IsFalse(WardrobeResponsiveLayout.Compute(std).IsCompact);
        }

        [Test]
        public void Layout_Standard_ListAndPreviewSideBySide()
        {
            var l = WardrobeResponsiveLayout.Compute(new Vector2(1920f, 1080f));
            Assert.IsFalse(l.IsCompact);
            // Preview to the right of the list, vertically aligned (same band).
            Assert.Greater(l.Preview.xMin, l.List.xMax - Eps);
            Assert.AreEqual(l.List.yMin, l.Preview.yMin, Eps);
        }

        [Test]
        public void Layout_Compact_PreviewBandAboveFullWidthList()
        {
            var l = WardrobeResponsiveLayout.Compute(new Vector2(2057f, 926f));
            Assert.IsTrue(l.IsCompact);
            Assert.Greater(l.Preview.yMin, l.List.yMax - Eps); // preview above
            Assert.AreEqual(l.List.width, l.Preview.width, Eps); // both full-width
        }

        [Test]
        public void Layout_DegenerateContent_NoException()
        {
            Assert.DoesNotThrow(() =>
                WardrobeResponsiveLayout.Compute(Vector2.zero));
        }

        private static void AssertWithin(Rect r, Vector2 size, string name)
        {
            Assert.GreaterOrEqual(r.xMin, -Eps, name + ".xMin");
            Assert.GreaterOrEqual(r.yMin, -Eps, name + ".yMin");
            Assert.LessOrEqual(r.xMax, size.x + Eps, name + ".xMax");
            Assert.LessOrEqual(r.yMax, size.y + Eps, name + ".yMax");
        }

        private static void AssertNoOverlap(Rect a, Rect b, string pair)
        {
            // Shrink slightly so shared edges (touching, not overlapping) pass.
            var a2 = new Rect(a.x + Eps, a.y + Eps,
                a.width - 2f * Eps, a.height - 2f * Eps);
            Assert.IsFalse(a2.Overlaps(b), pair + " overlap");
        }
    }

    public class CanvasScalerMathTests
    {
        private static readonly Vector2 Ref = new Vector2(1920f, 1080f);

        [Test]
        public void ReferenceResolution_ProducesReferenceCanvas()
        {
            var c = CanvasScalerMath.LogicalCanvasSize(Ref, Ref, 0.5f);
            Assert.AreEqual(1920f, c.x, 0.5f);
            Assert.AreEqual(1080f, c.y, 0.5f);
        }

        [Test]
        public void TallPhoneLandscape_ShrinksLogicalHeight()
        {
            // 20:9 (2400x1080), match 0.5 -> ~2147x966.
            var c = CanvasScalerMath.LogicalCanvasSize(
                new Vector2(2400f, 1080f), Ref, 0.5f);
            Assert.AreEqual(2147f, c.x, 2f);
            Assert.AreEqual(966f, c.y, 2f);
        }

        [Test]
        public void TabletLandscape_GrowsLogicalHeight()
        {
            // 4:3 (1440x1080), match 0.5 -> ~1663x1247.
            var c = CanvasScalerMath.LogicalCanvasSize(
                new Vector2(1440f, 1080f), Ref, 0.5f);
            Assert.AreEqual(1663f, c.x, 2f);
            Assert.AreEqual(1247f, c.y, 2f);
        }

        [Test]
        public void ZeroScreen_ReturnsReference()
        {
            var c = CanvasScalerMath.LogicalCanvasSize(Vector2.zero, Ref, 0.5f);
            Assert.AreEqual(Ref, c);
        }
    }

    public class SafeAreaCalculatorTests
    {
        private static readonly Vector2 Screen = new Vector2(2400f, 1080f);

        [Test]
        public void FullScreen_ProducesFullAnchors()
        {
            SafeAreaCalculator.ComputeAnchors(
                new Rect(0f, 0f, 2400f, 1080f), Screen,
                out var min, out var max);
            Assert.AreEqual(Vector2.zero, min);
            Assert.AreEqual(Vector2.one, max);
        }

        [Test]
        public void NotchLeft_InsetsMinX()
        {
            SafeAreaCalculator.ComputeAnchors(
                new Rect(100f, 0f, 2300f, 1080f), Screen,
                out var min, out var max);
            Assert.AreEqual(100f / 2400f, min.x, 1e-4f);
            Assert.AreEqual(0f, min.y, 1e-4f);
            Assert.AreEqual(1f, max.x, 1e-4f);
        }

        [Test]
        public void HomeIndicatorBottom_InsetsMinY()
        {
            SafeAreaCalculator.ComputeAnchors(
                new Rect(0f, 44f, 2400f, 1036f), Screen,
                out var min, out var max);
            Assert.AreEqual(44f / 1080f, min.y, 1e-4f);
            Assert.AreEqual(1f, max.y, 1e-4f);
        }

        [Test]
        public void ZeroResolution_FallsBackToFull()
        {
            SafeAreaCalculator.ComputeAnchors(
                new Rect(0f, 0f, 100f, 100f), Vector2.zero,
                out var min, out var max);
            Assert.AreEqual(Vector2.zero, min);
            Assert.AreEqual(Vector2.one, max);
        }

        [Test]
        public void InvertedSafeArea_FallsBackToFull()
        {
            SafeAreaCalculator.ComputeAnchors(
                new Rect(2000f, 0f, -500f, 1080f), Screen,
                out var min, out var max);
            Assert.AreEqual(Vector2.zero, min);
            Assert.AreEqual(Vector2.one, max);
        }

        [Test]
        public void RepeatedApply_IsDeterministic()
        {
            var sa = new Rect(100f, 44f, 2200f, 1036f);
            SafeAreaCalculator.ComputeAnchors(sa, Screen, out var min1, out var max1);
            SafeAreaCalculator.ComputeAnchors(sa, Screen, out var min2, out var max2);
            Assert.AreEqual(min1, min2);
            Assert.AreEqual(max1, max2);
        }
    }

    public class WardrobeLayoutMetricsTests
    {
        [Test]
        public void TouchTargets_MeetMinimum()
        {
            float min = WardrobeLayoutMetrics.MinTouchTargetCanvasUnits;
            Assert.GreaterOrEqual(WardrobeLayoutMetrics.RowMinHeight, min);
            Assert.GreaterOrEqual(WardrobeLayoutMetrics.ActionButtonHeight, min);
            Assert.GreaterOrEqual(WardrobeLayoutMetrics.CeremonyButtonHeight, min);
        }
    }

    public class ScrollIntoViewCalculatorTests
    {
        [Test]
        public void ItemAboveView_ScrollsUpToTop()
        {
            // current at bottom (0); item at content top -> v -> 1 (top).
            float v = ScrollIntoViewCalculator.ComputeVerticalNormalized(
                1000f, 500f, 0f, 90f, 0f);
            Assert.AreEqual(1f, v, 1e-3f);
        }

        [Test]
        public void ItemBelowView_ScrollsDown()
        {
            // current at top (1, showing 0..500); item at very bottom.
            float v = ScrollIntoViewCalculator.ComputeVerticalNormalized(
                1000f, 500f, 910f, 90f, 1f);
            Assert.AreEqual(0f, v, 1e-3f);
        }

        [Test]
        public void ItemWithinView_Unchanged()
        {
            float v = ScrollIntoViewCalculator.ComputeVerticalNormalized(
                1000f, 500f, 100f, 90f, 1f);
            Assert.AreEqual(1f, v, 1e-3f);
        }

        [Test]
        public void ContentFits_ReturnsCurrent()
        {
            float v = ScrollIntoViewCalculator.ComputeVerticalNormalized(
                400f, 500f, 100f, 90f, 0.3f);
            Assert.AreEqual(0.3f, v, 1e-3f);
        }
    }
}
