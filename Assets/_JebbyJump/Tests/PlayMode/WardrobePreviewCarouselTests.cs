using System.Collections.Generic;
using JebbyJump.Wardrobe;
using JebbyJump.Wardrobe.Visual;
using NUnit.Framework;
using UnityEngine;

namespace JebbyJump.Tests
{
    // P18 preview carousel: pure sequence builder + timing player (no scene).
    public class WardrobePreviewSequenceTests
    {
        private readonly List<Object> _temp = new List<Object>();

        [TearDown]
        public void TearDown()
        {
            foreach (var o in _temp) if (o != null) Object.DestroyImmediate(o);
            _temp.Clear();
        }

        private Sprite S()
        {
            var t = new Texture2D(2, 2);
            _temp.Add(t);
            var s = Sprite.Create(t, new Rect(0, 0, 2, 2), new Vector2(.5f, .5f));
            _temp.Add(s);
            return s;
        }

        private WardrobePreviewLibrary FullLib(string id)
        {
            var l = ScriptableObject.CreateInstance<WardrobePreviewLibrary>();
            _temp.Add(l);
            l.AddEntry(id, S(), S(), S(), S(), S(), S(), S());
            return l;
        }

        private static List<WardrobePreviewPose> Poses(
            IReadOnlyList<WardrobePreviewFrame> seq)
        {
            var list = new List<WardrobePreviewPose>();
            foreach (var f in seq) list.Add(f.Pose);
            return list;
        }

        [Test]
        public void KnownOutfit_ApprovedOrder_HurtExcluded()
        {
            var seq = WardrobePreviewSequenceBuilder.Build(
                "forest_cavalier", FullLib("forest_cavalier"), includeHurt: false);
            CollectionAssert.AreEqual(
                new[]
                {
                    WardrobePreviewPose.Idle, WardrobePreviewPose.Run,
                    WardrobePreviewPose.Jump, WardrobePreviewPose.Fall,
                    WardrobePreviewPose.Land, WardrobePreviewPose.Victory,
                },
                Poses(seq));
        }

        [Test]
        public void IncludeHurt_AddsHurtBeforeVictory()
        {
            var seq = WardrobePreviewSequenceBuilder.Build(
                "forest_cavalier", FullLib("forest_cavalier"), includeHurt: true);
            CollectionAssert.AreEqual(
                new[]
                {
                    WardrobePreviewPose.Idle, WardrobePreviewPose.Run,
                    WardrobePreviewPose.Jump, WardrobePreviewPose.Fall,
                    WardrobePreviewPose.Land, WardrobePreviewPose.Hurt,
                    WardrobePreviewPose.Victory,
                },
                Poses(seq));
        }

        [Test]
        public void MissingPose_SkippedSafely()
        {
            var l = ScriptableObject.CreateInstance<WardrobePreviewLibrary>();
            _temp.Add(l);
            // idle, jump, victory present; the rest null.
            l.AddEntry("forest_cavalier", S(), null, S(), null, null, null, S());
            var seq = WardrobePreviewSequenceBuilder.Build("forest_cavalier", l, false);
            CollectionAssert.AreEqual(
                new[]
                {
                    WardrobePreviewPose.Idle, WardrobePreviewPose.Jump,
                    WardrobePreviewPose.Victory,
                },
                Poses(seq));
        }

        [Test]
        public void MissingEntry_ReturnsEmpty()
        {
            var l = ScriptableObject.CreateInstance<WardrobePreviewLibrary>();
            _temp.Add(l);
            Assert.AreEqual(0,
                WardrobePreviewSequenceBuilder.Build("nope", l, false).Count);
        }

        [Test]
        public void NullLibrary_ReturnsEmpty()
        {
            Assert.AreEqual(0,
                WardrobePreviewSequenceBuilder.Build("forest_cavalier", null, false).Count);
        }

        [Test]
        public void Durations_ArePositive()
        {
            var seq = WardrobePreviewSequenceBuilder.Build(
                "forest_cavalier", FullLib("forest_cavalier"), includeHurt: true);
            foreach (var f in seq) Assert.Greater(f.DurationSeconds, 0f);
        }

        // P20 reduce motion: a single static Idle frame, no cycling.
        [Test]
        public void ReduceMotion_UsesIdleOnly()
        {
            var seq = WardrobePreviewSequenceBuilder.Build(
                "forest_cavalier", FullLib("forest_cavalier"),
                includeHurt: false, reduceMotion: true);
            Assert.AreEqual(1, seq.Count);
            Assert.AreEqual(WardrobePreviewPose.Idle, seq[0].Pose);
        }

        [Test]
        public void ReduceMotion_Off_MatchesFullSequence()
        {
            var lib = FullLib("forest_cavalier");
            var off = WardrobePreviewSequenceBuilder.Build(
                "forest_cavalier", lib, includeHurt: false, reduceMotion: false);
            var plain = WardrobePreviewSequenceBuilder.Build(
                "forest_cavalier", lib, includeHurt: false);
            Assert.AreEqual(plain.Count, off.Count);
            Assert.Greater(off.Count, 1);
        }

        [Test]
        public void ReduceMotion_NoIdleSprite_IsEmpty()
        {
            var l = ScriptableObject.CreateInstance<WardrobePreviewLibrary>();
            _temp.Add(l);
            l.AddEntry("forest_cavalier", null, S(), S(), S(), S(), S(), S());
            var seq = WardrobePreviewSequenceBuilder.Build(
                "forest_cavalier", l, includeHurt: false, reduceMotion: true);
            Assert.AreEqual(0, seq.Count);
        }
    }

    public class WardrobePreviewPlayerTests
    {
        private static WardrobePreviewFrame F(float d)
            => new WardrobePreviewFrame(WardrobePreviewPose.Idle, null, d);

        [Test]
        public void Empty_NoFrames_NoThrow()
        {
            var p = new WardrobePreviewPlayer();
            Assert.IsFalse(p.HasFrames);
            Assert.DoesNotThrow(() => p.Tick(1f));
        }

        [Test]
        public void StartsAtFirstFrame()
        {
            var p = new WardrobePreviewPlayer();
            p.SetFrames(new[] { F(1f), F(1f) });
            Assert.AreEqual(0, p.Index);
        }

        [Test]
        public void AdvancesAfterDuration()
        {
            var p = new WardrobePreviewPlayer();
            p.SetFrames(new[] { F(1f), F(1f) });
            p.Tick(1f);
            Assert.AreEqual(1, p.Index);
        }

        [Test]
        public void WrapsToStart()
        {
            var p = new WardrobePreviewPlayer();
            p.SetFrames(new[] { F(1f), F(1f) });
            p.Tick(1f);
            p.Tick(1f);
            Assert.AreEqual(0, p.Index);
        }

        [Test]
        public void LargeDelta_AdvancesMultipleFrames()
        {
            var p = new WardrobePreviewPlayer();
            p.SetFrames(new[] { F(1f), F(1f), F(1f) });
            p.Tick(2.5f);
            Assert.AreEqual(2, p.Index);
        }

        [Test]
        public void SetFrames_Resets()
        {
            var p = new WardrobePreviewPlayer();
            p.SetFrames(new[] { F(1f), F(1f) });
            p.Tick(1f);
            p.SetFrames(new[] { F(1f), F(1f) });
            Assert.AreEqual(0, p.Index);
        }

        [Test]
        public void ZeroOrNegativeDelta_Safe()
        {
            var p = new WardrobePreviewPlayer();
            p.SetFrames(new[] { F(1f), F(1f) });
            p.Tick(0f);
            p.Tick(-5f);
            Assert.AreEqual(0, p.Index);
        }

        [Test]
        public void NonPositiveDuration_NoInfiniteLoop()
        {
            var p = new WardrobePreviewPlayer();
            p.SetFrames(new[] { F(0f), F(1f) });
            Assert.DoesNotThrow(() => p.Tick(5f));
        }
    }
}
