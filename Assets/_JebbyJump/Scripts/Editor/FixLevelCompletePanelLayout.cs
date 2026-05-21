using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Re-lays out the LevelCompletePanel so Title / Time / Best / Rank don't
// collide with the Retry / Next / Main Menu buttons. Idempotent.
//
// Panel is 700x400, pivot (0.5, 0.5). Anchored Y is from panel centre:
//   +200 = panel top, -200 = panel bottom.
//
//   Title       y = +140
//   Time        y =  +60
//   Best        y =  +20
//   Rank        y =  -30
//   Buttons     y = -135  (Retry | Next | Main Menu, horizontally spaced)
public static class FixLevelCompletePanelLayout
{
    private const string ScenePath = "Assets/_JebbyJump/Scenes/Game.unity";

    public static void Execute()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("[LCPanel] Cannot run in Play Mode.");
            return;
        }
        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        var panel = FindByName("LevelCompletePanel");
        if (panel == null) { Debug.LogError("[LCPanel] LevelCompletePanel not found."); return; }

        // Re-stack stat texts.
        Place(panel.transform, "TitleText",     0f,  140f, 600f, 70f);
        Place(panel.transform, "TimeText",      0f,   60f, 500f, 48f);
        Place(panel.transform, "BestTimeText",  0f,   20f, 500f, 40f);
        Place(panel.transform, "RankText",      0f,  -30f, 500f, 50f);

        // Reposition buttons in a row near the bottom.
        var retry = FindChild(panel.transform, "Btn_Retry") ?? FindChild(panel.transform, "RetryButton");
        var next  = FindChild(panel.transform, "Btn_NextLevel") ?? FindChild(panel.transform, "NextLevelButton") ?? FindChild(panel.transform, "Btn_Next");
        var menu  = FindChild(panel.transform, "Btn_MainMenu") ?? FindChild(panel.transform, "MainMenuButton") ?? FindChild(panel.transform, "Btn_Menu");

        // Width 180, height 64. Spaced 200 px between centres.
        if (retry != null) SetRT(retry, -200f, -135f, 180f, 64f);
        if (next  != null) SetRT(next,     0f, -135f, 180f, 64f);
        if (menu  != null) SetRT(menu,   200f, -135f, 180f, 64f);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("[LCPanel] Layout corrected — texts top half, buttons bottom row.");
    }

    private static void Place(Transform parent, string childName, float x, float y, float w, float h)
    {
        var t = FindChild(parent, childName);
        if (t == null) { Debug.LogWarning($"[LCPanel] '{childName}' not found."); return; }
        SetRT(t, x, y, w, h);
    }

    private static void SetRT(Transform t, float x, float y, float w, float h)
    {
        var rt = t.GetComponent<RectTransform>();
        if (rt == null) return;
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot     = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(w, h);
        rt.anchoredPosition = new Vector2(x, y);
        EditorUtility.SetDirty(t);
    }

    private static Transform FindChild(Transform parent, string name)
    {
        foreach (Transform t in parent.GetComponentsInChildren<Transform>(true))
            if (t.name == name) return t;
        return null;
    }

    private static GameObject FindByName(string name)
    {
        var all = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var go in all)
            if (go.name == name && go.scene.IsValid()) return go;
        return null;
    }
}
