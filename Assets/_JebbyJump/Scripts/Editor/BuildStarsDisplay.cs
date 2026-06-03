using JebbyJump.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Idempotent scaffold: adds a "StarsText" line to the Level Complete
// result panel in Game.unity by cloning the existing rank-text element
// (so font/style match), placing it just below the rank line, and wiring
// HUDController._levelCompleteStarsText. Re-runs reuse the existing object;
// nothing is duplicated. Star logic/store/analytics work even before this
// runs - this only adds the visible text.
public static class BuildStarsDisplay
{
    private const string GameScenePath =
        "Assets/_JebbyJump/Scenes/Game.unity";

    [MenuItem("Jebby Jump/Scaffold/Build Stars Display")]
    public static void Run()
    {
        if (!System.IO.File.Exists(GameScenePath))
        {
            Debug.LogError("[Stars] Game scene not found at " + GameScenePath);
            return;
        }
        EnsureSceneOpen();

        var hud = Object.FindFirstObjectByType<HUDController>(
            FindObjectsInactive.Include);
        if (hud == null)
        {
            Debug.LogError("[Stars] No HUDController in Game.unity. Abort.");
            return;
        }

        var so = new SerializedObject(hud);
        var rankText = so.FindProperty("_levelCompleteRankText")
            .objectReferenceValue as TextMeshProUGUI;
        if (rankText == null)
        {
            Debug.LogError(
                "[Stars] HUDController._levelCompleteRankText not wired; "
                + "need it as the clone template. Abort.");
            return;
        }

        var existing = so.FindProperty("_levelCompleteStarsText")
            .objectReferenceValue as TextMeshProUGUI;
        if (existing == null)
        {
            existing = FindStarsChild(rankText.transform.parent);
        }

        TextMeshProUGUI stars = existing;
        if (stars == null)
        {
            var clone = Object.Instantiate(
                rankText.gameObject, rankText.transform.parent);
            clone.name = "StarsText";
            stars = clone.GetComponent<TextMeshProUGUI>();

            // Sit just below the rank line.
            var src = rankText.GetComponent<RectTransform>();
            var rt = stars.GetComponent<RectTransform>();
            rt.anchorMin = src.anchorMin;
            rt.anchorMax = src.anchorMax;
            rt.pivot = src.pivot;
            rt.sizeDelta = src.sizeDelta;
            rt.anchoredPosition =
                src.anchoredPosition + new Vector2(0f, -48f);
            stars.text = "Stars: -/3";
            Debug.Log("[Stars] Created StarsText (cloned rank text).");
        }
        else
        {
            Debug.Log("[Stars] StarsText already present - reusing.");
        }

        so.FindProperty("_levelCompleteStarsText").objectReferenceValue = stars;
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(hud);

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        Debug.Log("[Stars] Wired HUDController._levelCompleteStarsText.");
    }

    private static TextMeshProUGUI FindStarsChild(Transform parent)
    {
        if (parent == null) return null;
        var t = parent.Find("StarsText");
        return t != null ? t.GetComponent<TextMeshProUGUI>() : null;
    }

    private static void EnsureSceneOpen()
    {
        var active = SceneManager.GetActiveScene();
        if (active.path != GameScenePath)
        {
            if (active.isDirty) EditorSceneManager.SaveScene(active);
            EditorSceneManager.OpenScene(
                GameScenePath, OpenSceneMode.Single);
        }
    }
}
