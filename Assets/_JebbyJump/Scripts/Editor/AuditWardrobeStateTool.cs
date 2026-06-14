using System.Collections.Generic;
using System.Text;
using JebbyJump.Progression;
using JebbyJump.Rewards;
using JebbyJump.Wardrobe;
using JebbyJump.Wardrobe.Visual;
using UnityEditor;
using UnityEngine;

// Read-only QA command: logs a structured snapshot of the persisted wardrobe
// save state (via the runtime WardrobeStateAuditor) plus asset-registration
// status for the real OutfitVisualLibrary + WardrobePreviewLibrary. It NEVER
// writes PlayerPrefs, equips, resets, grants Stars, acknowledges unlocks, or
// rebuilds assets. Null-safe: missing catalog/library assets are reported, not
// thrown. A diagnostic only - it reports what migration WOULD do, never does it.
public static class AuditWardrobeStateTool
{
    private const string VisualLibraryPath =
        "Assets/_JebbyJump/Art/Characters/Jebby/Outfits/OutfitVisualLibrary.asset";
    private const string PreviewLibraryPath =
        "Assets/_JebbyJump/Art/Characters/Jebby/Outfits/WardrobePreviewLibrary.asset";

    [MenuItem("Jebby Jump/QA/Audit Wardrobe State")]
    public static void AuditWardrobeState()
    {
        var catalog = LoadCatalog();
        int totalStars = catalog != null
            ? StarRewardStore.GetTotalStars(catalog.Count) : 0;

        var result = WardrobeStateAuditor.AuditPersistence(totalStars);
        var s = result.Snapshot;

        var sb = new StringBuilder();
        sb.AppendLine("[Audit Wardrobe State] (read-only)");
        sb.AppendLine("Schema version: " + s.SchemaVersion
            + (s.HasSchemaKey ? "" : " (key absent -> legacy)"));
        sb.AppendLine("Schema support: "
            + (result.IsSupportedSchema
                ? "Supported"
                : "FUTURE / UNSUPPORTED (read-only, non-repairing)"));
        sb.AppendLine("Total Stars: " + totalStars
            + (catalog != null ? "" : " (LevelCatalog not found -> assumed 0)"));
        sb.AppendLine("Raw equipped id: "
            + (s.HasEquippedKey ? Quote(s.RawEquippedOutfitId) : "(key absent)"));
        sb.AppendLine("Normalized equipped id: " + Quote(s.NormalizedEquippedOutfitId));
        sb.AppendLine("Requires migration: " + result.RequiresMigration
            + " | Requires normalization: " + result.RequiresNormalization);
        sb.AppendLine("Unlocked outfits (" + s.UnlockedOutfitIds.Count + "): "
            + Join(s.UnlockedOutfitIds));
        sb.AppendLine("Acknowledged outfits (" + s.AcknowledgedOutfitIds.Count + "): "
            + Join(s.AcknowledgedOutfitIds));

        var newlyUnlocked = WardrobeNewUnlockService.GetNewlyUnlocked(
            totalStars, WardrobeUnlockAcknowledgementStore.IsAcknowledged);
        var newIds = new List<string>();
        foreach (var d in newlyUnlocked) newIds.Add(d.Id);
        sb.AppendLine("New / unacknowledged unlocked (" + newIds.Count + "): "
            + Join(newIds));

        var issues = new List<string>();
        foreach (var i in result.Issues) issues.Add(i.ToString());

        // Asset-registration audit (read-only). Missing assets are reported.
        AppendAssetAudit(sb, issues);

        sb.AppendLine("Issues: " + (issues.Count == 0 ? "none" : Join(issues)));
        Debug.Log(sb.ToString());
    }

    private static void AppendAssetAudit(StringBuilder sb, List<string> issues)
    {
        // Visual overrides: every non-default outfit should have a library entry.
        var visual = AssetDatabase.LoadAssetAtPath<OutfitVisualLibrary>(
            VisualLibraryPath);
        if (visual == null)
        {
            sb.AppendLine("Visual registrations: (OutfitVisualLibrary asset missing)");
            issues.Add("MissingVisualRegistration(asset)");
        }
        else
        {
            int nonDefault = 0, registered = 0;
            foreach (var o in WardrobeCatalog.Outfits)
            {
                if (o.Id == WardrobeCatalog.DefaultOutfitId) continue;
                nonDefault++;
                if (visual.TryGetOverride(o.Id, out _)) registered++;
                else issues.Add(
                    WardrobeAuditIssue.MissingVisualRegistration + ":" + o.Id);
            }
            sb.AppendLine("Visual registrations: " + registered + "/" + nonDefault
                + " non-default");
            if (HasDuplicate(visual.EntryIds()))
                issues.Add(WardrobeAuditIssue.DuplicateVisualId.ToString());
        }

        // Preview entries: every outfit should have an entry with all poses.
        var preview = AssetDatabase.LoadAssetAtPath<WardrobePreviewLibrary>(
            PreviewLibraryPath);
        if (preview == null)
        {
            sb.AppendLine("Preview registrations: (WardrobePreviewLibrary asset missing)");
            issues.Add("MissingPreviewRegistration(asset)");
            return;
        }

        var previewIds = new HashSet<string>(preview.EntryIds());
        int entries = 0, posesPresent = 0, posesExpected = 0;
        foreach (var o in WardrobeCatalog.Outfits)
        {
            if (previewIds.Contains(o.Id)) entries++;
            else issues.Add(
                WardrobeAuditIssue.MissingPreviewRegistration + ":" + o.Id);
            foreach (WardrobePreviewPose pose in
                System.Enum.GetValues(typeof(WardrobePreviewPose)))
            {
                posesExpected++;
                if (preview.TryGetPose(o.Id, pose, out _)) posesPresent++;
                else issues.Add(
                    WardrobeAuditIssue.MissingPreviewPose + ":" + o.Id + "/" + pose);
            }
        }
        sb.AppendLine("Preview registrations: " + entries + "/"
            + WardrobeCatalog.Outfits.Count);
        sb.AppendLine("Preview poses: " + posesPresent + "/" + posesExpected);
        if (HasDuplicate(preview.EntryIds()))
            issues.Add(WardrobeAuditIssue.DuplicatePreviewId.ToString());
    }

    private static bool HasDuplicate(IReadOnlyList<string> ids)
    {
        var seen = new HashSet<string>();
        foreach (var id in ids) if (!seen.Add(id)) return true;
        return false;
    }

    private static string Quote(string v)
        => v == null ? "null" : "\"" + v + "\"";

    private static string Join(IReadOnlyList<string> ids)
        => ids == null || ids.Count == 0 ? "-" : string.Join(", ", ids);

    private static LevelCatalog LoadCatalog()
    {
        string[] guids = AssetDatabase.FindAssets("t:LevelCatalog");
        if (guids == null || guids.Length == 0) return null;
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        return AssetDatabase.LoadAssetAtPath<LevelCatalog>(path);
    }
}
