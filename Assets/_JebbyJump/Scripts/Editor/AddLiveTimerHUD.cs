using JebbyJump.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Adds a top-right LiveTimerText to HUDCanvas and wires HUDController._liveTimerText.
// Idempotent.
public static class AddLiveTimerHUD
{
    private const string ScenePath = "Assets/_JebbyJump/Scenes/Game.unity";
    private const string FontPath  = "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset";

    public static void Execute()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("[LiveTimer] Cannot run in Play Mode.");
            return;
        }
        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        var hud = Object.FindAnyObjectByType<HUDController>(FindObjectsInactive.Include);
        if (hud == null) { Debug.LogError("[LiveTimer] HUDController not found."); return; }

        var canvas = FindByName("HUDCanvas");
        if (canvas == null) { Debug.LogError("[LiveTimer] HUDCanvas not found."); return; }

        var existing = canvas.transform.Find("LiveTimerText");
        GameObject go;
        if (existing != null) go = existing.gameObject;
        else
        {
            go = new GameObject("LiveTimerText", typeof(RectTransform), typeof(TextMeshProUGUI));
            go.transform.SetParent(canvas.transform, false);
            Debug.Log("[LiveTimer] Created LiveTimerText under HUDCanvas.");
        }

        // Anchor top-right with 36 px safe-area padding from the corner.
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot     = new Vector2(1f, 1f);
        rt.sizeDelta = new Vector2(260f, 60f);
        rt.anchoredPosition = new Vector2(-36f, -36f);

        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text         = "00:00.00";
        tmp.fontSize     = 36f;
        tmp.fontStyle    = FontStyles.Bold;
        tmp.alignment    = TextAlignmentOptions.MidlineRight;
        tmp.color        = Color.white;
        tmp.outlineWidth = 0.2f;
        tmp.outlineColor = Color.black;
        tmp.raycastTarget = false;
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        if (font != null && tmp.font == null) tmp.font = font;

        // Wire HUDController._liveTimerText
        var so = new SerializedObject(hud);
        var prop = so.FindProperty("_liveTimerText");
        if (prop != null && prop.objectReferenceValue != tmp)
        {
            prop.objectReferenceValue = tmp;
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(hud);
            Debug.Log("[LiveTimer] HUDController._liveTimerText wired.");
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("[LiveTimer] Done — top-right live timer at (-36, -36), 36pt bold outlined.");
    }

    private static GameObject FindByName(string name)
    {
        var all = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var go in all)
            if (go.name == name && go.scene.IsValid()) return go;
        return null;
    }
}
