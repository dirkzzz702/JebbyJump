using JebbyJump.Shell;
using JebbyJump.Wardrobe.Visual;
using NUnit.Framework;
using UnityEngine;

namespace JebbyJump.Tests
{
    // P22 correction #5: combined cross-canvas gameplay layout validation across
    // the landscape safe-area matrix. HUD (HUDCanvas) + controls/pause
    // (MobileControlsCanvas) share one SWSS 1920x1080 logical space; the matrix
    // adds a top inset to exercise top-notch landscape (HUD top row + pause).
    public class GameplayLayoutPolicyTests
    {
        // (screenW, screenH, leftInset, rightInset, topInset, bottomInset)
        private static readonly (float w, float h, float l, float r, float t, float b)[] Matrix =
        {
            (1920f, 1080f, 0f, 0f, 0f, 0f),     // 16:9
            (2160f, 1080f, 0f, 0f, 0f, 0f),     // 18:9
            (2340f, 1080f, 0f, 0f, 0f, 0f),     // 19.5:9
            (2400f, 1080f, 0f, 0f, 0f, 0f),     // 20:9
            (1440f, 1080f, 0f, 0f, 0f, 0f),     // 4:3
            (1920f, 1080f, 100f, 0f, 0f, 0f),   // notch left
            (1920f, 1080f, 0f, 100f, 44f, 0f),  // notch right + top
            (1920f, 1080f, 0f, 0f, 0f, 44f),    // home indicator
            (2400f, 1080f, 100f, 0f, 44f, 44f), // tightest (20:9 + combined)
        };

        private static Rect Content(float sw, float sh, float l, float r, float t, float b)
        {
            Vector2 canvas = CanvasScalerMath.LogicalCanvasSize(
                new Vector2(sw, sh), new Vector2(1920f, 1080f), 0.5f);
            float scale = canvas.x / sw;
            return new Rect(0f, 0f, (sw - l - r) * scale, (sh - t - b) * scale);
        }

        [Test]
        public void AllElementsWithinSafeAreaAcrossMatrix()
        {
            foreach (var c in Matrix)
            {
                Rect content = Content(c.w, c.h, c.l, c.r, c.t, c.b);
                var layout = GameplayLayoutMetrics.Compute(content);
                Assert.IsTrue(GameplayLayoutPolicy.AllWithin(layout, content),
                    $"HUD/controls overflow safe area at {c.w}x{c.h} " +
                    $"insets(l{c.l},r{c.r},t{c.t},b{c.b}) content={content}");
            }
        }

        [Test]
        public void PauseClearsTimerAndSkillsAcrossMatrix()
        {
            foreach (var c in Matrix)
            {
                Rect content = Content(c.w, c.h, c.l, c.r, c.t, c.b);
                var layout = GameplayLayoutMetrics.Compute(content);
                Assert.IsTrue(GameplayLayoutPolicy.PauseClear(layout),
                    $"Pause overlaps timer/skills at {c.w}x{c.h} " +
                    $"insets(l{c.l},r{c.r},t{c.t},b{c.b})");
            }
        }

        [Test]
        public void ClustersDoNotCollideAcrossMatrix()
        {
            foreach (var c in Matrix)
            {
                Rect content = Content(c.w, c.h, c.l, c.r, c.t, c.b);
                var layout = GameplayLayoutMetrics.Compute(content);
                Assert.IsTrue(GameplayLayoutPolicy.ClustersClear(layout),
                    $"HUD/control clusters collide at {c.w}x{c.h} " +
                    $"insets(l{c.l},r{c.r},t{c.t},b{c.b})");
            }
        }
    }
}
