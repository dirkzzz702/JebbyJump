using System.Collections.Generic;
using JebbyJump.Balance;
using NUnit.Framework;
using UnityEngine;

namespace JebbyJump.Tests.EditMode
{
    public class BalancePlaytestPolicyTests
    {
        [Test]
        public void Readiness_PreparationNotRun_WhenNoAttemptsNoTuning() // corr #9
        {
            Assert.AreEqual("P28 preparation complete - manual balance playtest NOT RUN",
                BalancePlaytestPolicy.PreparationNotRun);
            Assert.AreEqual(BalancePlaytestPolicy.PreparationNotRun,
                BalancePlaytestPolicy.Decide(sampleSize: 0, tuningApplied: false));
        }

        [Test]
        public void Readiness_PlaytestNoTuning_WhenAttemptsButNoTuning()
        {
            Assert.AreEqual(BalancePlaytestPolicy.PlaytestNoTuning,
                BalancePlaytestPolicy.Decide(sampleSize: 5, tuningApplied: false));
        }

        [Test]
        public void Readiness_TuningPass_WhenTuningApplied()
        {
            Assert.AreEqual(BalancePlaytestPolicy.TuningPassComplete,
                BalancePlaytestPolicy.Decide(sampleSize: 5, tuningApplied: true));
        }

        [Test]
        public void ManualPlaytestNotRun_CannotClaimBalanceValidated()
        {
            Assert.IsFalse(BalancePlaytestPolicy.CanClaimValidated(0));
            Assert.IsTrue(BalancePlaytestPolicy.CanClaimValidated(1));
        }

        [Test]
        public void HeuristicOnly_CannotAuthorizeTuningAutomatically() // corr #7
        {
            Assert.IsFalse(BalancePlaytestPolicy.HeuristicCanAuthorizeTuning());
        }

        [Test]
        public void AllReviewCandidates_DeferredAndNotAuthorized() // corr #3
        {
            var cands = BalanceReviewCandidateCatalog.ReviewCandidates();
            Assert.Greater(cands.Length, 0);
            Assert.IsTrue(BalancePlaytestPolicy.AllCandidatesDeferred(cands));
            foreach (var c in cands)
            {
                Assert.IsFalse(c.AuthorizedForApply, c.Id);
                Assert.IsTrue(c.DeferredPendingPlaytest, c.Id);
            }
            // a tampered candidate must fail the guard
            var bad = new[] { new BalanceReviewCandidate { AuthorizedForApply = true, DeferredPendingPlaytest = true } };
            Assert.IsFalse(BalancePlaytestPolicy.AllCandidatesDeferred(bad));
        }

        [Test]
        public void ProgressionObservations_SeparateAndDeferred() // corr #8
        {
            var obs = BalanceReviewCandidateCatalog.ProgressionObservations();
            Assert.Greater(obs.Length, 0);
            Assert.IsTrue(BalancePlaytestPolicy.AllObservationsDeferred(obs));
            foreach (var o in obs)
            {
                Assert.IsFalse(o.AuthorizedForApply);
                Assert.IsTrue(o.DeferredPendingPlaytest);
            }
        }

        [Test]
        public void ReviewCandidates_HaveStableUniqueIds()
        {
            var seen = new HashSet<string>();
            foreach (var c in BalanceReviewCandidateCatalog.ReviewCandidates())
            {
                Assert.IsFalse(string.IsNullOrEmpty(c.Id));
                Assert.IsTrue(seen.Add(c.Id), "duplicate id " + c.Id);
            }
        }

        [Test]
        public void Report_SerializesRequiredFields()
        {
            string json = JsonUtility.ToJson(new BalancePlaytestReport
            {
                LevelSummaries = new[] { new BalanceLevelSummary { LevelIndex = 1, EvidenceStatus = "NotRun" } },
                ReviewCandidates = BalanceReviewCandidateCatalog.ReviewCandidates(),
                ProgressionObservations = BalanceReviewCandidateCatalog.ProgressionObservations(),
            });
            foreach (var f in new[]
            {
                "LevelSummaries", "ReviewCandidates", "ProgressionObservations", "ReadinessDecision",
                "SampleSize", "NoHiddenTuningProof", "EvidenceStatus", "AuthorizedForApply",
                "DeferredPendingPlaytest",
            })
                StringAssert.Contains(f, json);
        }
    }
}
