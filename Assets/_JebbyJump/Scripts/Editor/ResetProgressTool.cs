using JebbyJump.Progression;
using JebbyJump.Rewards;
using UnityEditor;
using UnityEngine;

// Dev-only menu items for clearing local progression and/or best
// times. Useful for verifying locked-card states and fresh-install
// behaviour without uninstalling the app. No best-rank reset path;
// rank is never persisted in this design.
public static class ResetProgressTool
{
    [MenuItem("Jebby Jump/Reset/Reset Local Progress")]
    public static void ResetLocalProgress()
    {
        LevelProgressStore.ResetLocalProgress();
        Debug.Log(
            "[ResetProgress] Highest-unlocked reset to 0 (Level 1 only).");
    }

    [MenuItem("Jebby Jump/Reset/Reset Best Times")]
    public static void ResetBestTimes()
    {
        var catalog = LoadCatalog();
        if (catalog == null)
        {
            Debug.LogWarning(
                "[ResetProgress] LevelCatalog not found; "
                + "cannot enumerate best-time keys.");
            return;
        }
        for (int i = 0; i < catalog.Count; i++)
        {
            string levelKey = catalog.GetLevelKey(i);
            if (string.IsNullOrEmpty(levelKey)) continue;
            PlayerPrefs.DeleteKey("JebbyJump.BestTime." + levelKey);
        }
        PlayerPrefs.Save();
        Debug.Log("[ResetProgress] Best times cleared for all levels.");
    }

    [MenuItem("Jebby Jump/Reset/Reset Stars")]
    public static void ResetStars()
    {
        var catalog = LoadCatalog();
        if (catalog == null)
        {
            Debug.LogWarning(
                "[ResetProgress] LevelCatalog not found; "
                + "cannot enumerate star keys.");
            return;
        }
        StarRewardStore.ResetAll(catalog.Count);
        Debug.Log("[ResetProgress] Mastery stars cleared for all levels.");
    }

    [MenuItem("Jebby Jump/Reset/Reset Everything")]
    public static void ResetEverything()
    {
        ResetLocalProgress();
        ResetBestTimes();
        ResetStars();
        Debug.Log("[ResetProgress] Full reset complete.");
    }

    private static LevelCatalog LoadCatalog()
    {
        string[] guids = AssetDatabase.FindAssets("t:LevelCatalog");
        if (guids == null || guids.Length == 0) return null;
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        return AssetDatabase.LoadAssetAtPath<LevelCatalog>(path);
    }
}
