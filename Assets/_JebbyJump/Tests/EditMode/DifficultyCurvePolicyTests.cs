using System;
using System.IO;
using JebbyJump.Balance;
using NUnit.Framework;
using UnityEngine;

namespace JebbyJump.Tests.EditMode
{
    public class DifficultyCurvePolicyTests
    {
        [Serializable] private class AsmdefShape { public string name; public string[] includePlatforms; }

        private static LevelDifficultySnapshot Lvl(int idx, int seq, float s, float a, float b)
            => new LevelDifficultySnapshot
            {
                LevelName = "Level" + idx, Index = idx, SequenceLength = seq, ColorCount = 3,
                MemorySeconds = 5f, StartingLives = 3, CactusChance = 0f, PlatformsPerRow = 2,
                SThreshold = s, AThreshold = a, BThreshold = b,
            };

        [Test]
        public void ThresholdOrder_IsHardInvariant()
        {
            Assert.IsTrue(DifficultyCurvePolicy.ThresholdOrderValid(Lvl(1, 3, 8, 12, 18)));
            Assert.IsFalse(DifficultyCurvePolicy.ThresholdOrderValid(Lvl(1, 3, 12, 12, 18))); // s == a
            Assert.IsFalse(DifficultyCurvePolicy.ThresholdOrderValid(Lvl(1, 3, 8, 20, 18)));  // a > b
        }

        [Test]
        public void Analyze_FlagsThresholdInversion_AsInvariantFail()
        {
            var f = DifficultyCurvePolicy.Analyze(new[] { Lvl(1, 3, 8, 20, 18) });
            Assert.AreEqual(1, DifficultyCurvePolicy.InvariantFailCount(f));
        }

        [Test]
        public void Analyze_CleanCurve_HasNoInvariantFails()
        {
            var f = DifficultyCurvePolicy.Analyze(new[] { Lvl(1, 3, 8, 12, 18), Lvl(2, 4, 9, 13, 19) });
            Assert.AreEqual(0, DifficultyCurvePolicy.InvariantFailCount(f));
        }

        [Test]
        public void Heuristic_IsDeterministic()
        {
            var s = Lvl(1, 4, 8, 12, 18);
            Assert.AreEqual(DifficultyCurvePolicy.HeuristicScore(s), DifficultyCurvePolicy.HeuristicScore(s));
        }

        [Test]
        public void Heuristic_LongerSequence_ScoresHarder()
        {
            float lo = DifficultyCurvePolicy.HeuristicScore(Lvl(1, 3, 8, 12, 18));
            float hi = DifficultyCurvePolicy.HeuristicScore(Lvl(2, 6, 8, 12, 18));
            Assert.Greater(hi, lo);
        }

        [Test]
        public void DecreasingDifficulty_IsObservation_NotInvariantFailure()
        {
            // Level 2 is EASIER (shorter sequence): a cross-level heuristic OBSERVATION,
            // never an invariant failure (correction #9).
            var f = DifficultyCurvePolicy.Analyze(new[] { Lvl(1, 6, 8, 12, 18), Lvl(2, 3, 9, 13, 19) });
            Assert.AreEqual(0, DifficultyCurvePolicy.InvariantFailCount(f));
            bool observed = false;
            foreach (var x in f)
                if (x.CheckId == "curve.monotonicity" && x.Severity == "OBSERVATION") observed = true;
            Assert.IsTrue(observed);
        }

        [Test]
        public void BalanceEditor_IsExcludedFromReleasePlayer()
        {
            var text = File.ReadAllText(Path.GetFullPath(Path.Combine(Application.dataPath, "..",
                "Assets/_JebbyJump/Scripts/Balance/Editor/JebbyJump.Balance.Editor.asmdef")));
            var shape = JsonUtility.FromJson<AsmdefShape>(text);
            CollectionAssert.AreEqual(new[] { "Editor" }, shape.includePlatforms);
        }

        [Test]
        public void BalanceReport_SerializesRequiredFields()
        {
            var r = new BalanceReport { Levels = new[] { Lvl(1, 3, 8, 12, 18) }, Scores = new[] { 1f } };
            string json = JsonUtility.ToJson(r);
            foreach (var fld in new[] { "Levels", "Scores", "Findings", "Formula", "ReadOnlyProof", "InvariantFailCount" })
                StringAssert.Contains(fld, json);
        }
    }
}
