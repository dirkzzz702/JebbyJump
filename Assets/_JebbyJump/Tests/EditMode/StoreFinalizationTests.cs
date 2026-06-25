using JebbyJump.Release;
using NUnit.Framework;

namespace JebbyJump.Tests.EditMode
{
    public class StoreFinalizationTests
    {
        // the finalized DRAFT listing copy (must mirror the committed doc)
        private const string Title = "Jebby Jump";
        private const string ShortDesc = "Memory platformer: watch the color sequence, then hop the matching tiles.";
        private const string FullDesc =
            "Jebby Jump is a colorful memory platformer. Each level shows a short color sequence; "
            + "remember it, then jump across the matching platforms before the clock runs out. "
            + "Chase faster clear times and time ranks, and dress Jebby up with cosmetic outfits. "
            + "Designed mobile-first for landscape play.";

        [Test]
        public void ListingCopy_RespectsLengthLimits()
        {
            Assert.IsTrue(DistributionDeclarationPolicy.ListingCopyWithinLimits(Title, ShortDesc, FullDesc));
            Assert.LessOrEqual(Title.Length, DistributionDeclarationPolicy.TitleMax);
            Assert.LessOrEqual(ShortDesc.Length, DistributionDeclarationPolicy.ShortDescMax);
            Assert.IsFalse(DistributionDeclarationPolicy.ListingCopyWithinLimits(new string('x', 31), ShortDesc, FullDesc));
        }

        [Test]
        public void ListingCopy_HasNoUnverifiedClaims()
        {
            Assert.IsFalse(DistributionDeclarationPolicy.ListingCopyHasUnverifiedClaim(ShortDesc));
            Assert.IsFalse(DistributionDeclarationPolicy.ListingCopyHasUnverifiedClaim(FullDesc));
            Assert.IsTrue(DistributionDeclarationPolicy.ListingCopyHasUnverifiedClaim("the final, certified best game"));
        }

        [Test]
        public void PrivacyPolicy_PlaceholderCannotPass()
        {
            Assert.IsFalse(DistributionDeclarationPolicy.PrivacyPolicyValid("<PROVIDE: privacy-policy URL>"));
            Assert.IsFalse(DistributionDeclarationPolicy.PrivacyPolicyValid(""));
            Assert.IsTrue(StoreReadinessPolicy.ContainsPlaceholder("<PROVIDE: privacy-policy URL>"));
        }

        [Test]
        public void DataSafety_RequiresArtifactAuditAndUserApproval()
        {
            Assert.IsFalse(DistributionDeclarationPolicy.DataSafetyCanFinalize(false, false));
            Assert.IsFalse(DistributionDeclarationPolicy.DataSafetyCanFinalize(true, false));
            Assert.IsFalse(DistributionDeclarationPolicy.DataSafetyCanFinalize(false, true));
            Assert.IsTrue(DistributionDeclarationPolicy.DataSafetyCanFinalize(true, true));
        }

        [Test]
        public void TargetAudience_MustBeExplicit()
        {
            Assert.IsTrue(DistributionDeclarationPolicy.TargetAudienceExplicit("Mixed (children + older) -> Families"));
            Assert.IsFalse(DistributionDeclarationPolicy.TargetAudienceExplicit(""));
            Assert.IsFalse(DistributionDeclarationPolicy.TargetAudienceExplicit("unknown"));
        }

        [Test]
        public void ContentRating_NotPredicted()
        {
            Assert.IsFalse(DistributionDeclarationPolicy.IsPredictedRating(""));      // worksheet leaves it empty
            Assert.IsTrue(DistributionDeclarationPolicy.IsPredictedRating("Everyone")); // a prediction = violation
        }

        [Test]
        public void GraphicsChecklist_SeparatesLauncherAndListingAssets()
        {
            Assert.IsTrue(StoreGraphicsPolicy.IsInArtifact(GraphicAssetKind.LauncherIcon));
            Assert.IsTrue(StoreGraphicsPolicy.IsInArtifact(GraphicAssetKind.AdaptiveForeground));
            Assert.IsTrue(StoreGraphicsPolicy.IsInArtifact(GraphicAssetKind.AdaptiveBackground));
            Assert.IsTrue(StoreGraphicsPolicy.IsConsoleListing(GraphicAssetKind.ListingIcon512));
            Assert.IsTrue(StoreGraphicsPolicy.IsConsoleListing(GraphicAssetKind.FeatureGraphic1024x500));
            Assert.IsTrue(StoreGraphicsPolicy.IsConsoleListing(GraphicAssetKind.PhoneScreenshot));
            Assert.IsFalse(StoreGraphicsPolicy.IsInArtifact(GraphicAssetKind.ListingIcon512));
            Assert.AreEqual(3, StoreGraphicsPolicy.Tiers().Length); // internal / public / production (corr #5)
        }

        [Test]
        public void Placeholder_CannotPassFinalReadiness() // corr #8/#10
        {
            // a remaining placeholder forces blocked even if everything else is "present"
            Assert.AreEqual(StoreReadinessPolicy.Blocked, StoreReadinessPolicy.Decide(
                privacyUrlProvided: true, supportEmailProvided: true, listingGraphicsProvided: true,
                userApproved: true, anyPlaceholderRemains: true));
            // current P31 state: externals missing -> blocked
            Assert.AreEqual(StoreReadinessPolicy.Blocked, StoreReadinessPolicy.Decide(
                false, false, false, false, false));
            Assert.AreEqual("Store listing package blocked - missing external content", StoreReadinessPolicy.Blocked);
            // only a fully-provided, approved, placeholder-free package is ready
            Assert.AreEqual(StoreReadinessPolicy.Ready, StoreReadinessPolicy.Decide(
                true, true, true, true, false));
        }
    }
}
