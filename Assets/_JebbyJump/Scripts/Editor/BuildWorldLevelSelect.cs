using JebbyJump.Core;
using JebbyJump.UI;
using JebbyJump.World;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// Builds the world tab strip + world title into the existing LevelSelectPanel
// and wires the LevelSelectController's new world fields (WorldExpansion100,
// phase P34D). Ten static WorldTab cells (worlds are a fixed 10) + a title,
// with the ScrollView inset to make room. Idempotent: re-running refreshes
// wiring and leaves an already-built strip in place.
public static class BuildWorldLevelSelect
{
    private const string MainMenuScenePath =
        "Assets/_JebbyJump/Scenes/MainMenu.unity";
    private const string WorldCatalogPath =
        "Assets/_JebbyJump/Settings/World/WorldCatalog.asset";
    private const string PillSpritePath =
        "Assets/_JebbyJump/Art/Sprites/UI/ui_btn_pill_9s.png";

    private const string StripName = "WorldStrip";
    private const string TitleName = "WorldTitle";
    private const float ScrollTopInset = -196f; // below title + strip

    [MenuItem("Jebby Jump/Scaffold/Build World Level Select")]
    public static void Run()
    {
        var scene = EditorSceneManager.OpenScene(MainMenuScenePath, OpenSceneMode.Single);

        GameObject panel = null;
        foreach (var root in scene.GetRootGameObjects())
        {
            panel = FindChild(root.transform, "LevelSelectPanel")?.gameObject;
            if (panel != null) break;
        }
        if (panel == null) { Debug.LogError("[WorldLS] LevelSelectPanel not found."); return; }

        var controller = panel.GetComponent<LevelSelectController>();
        if (controller == null) { Debug.LogError("[WorldLS] LevelSelectController missing."); return; }

        // Parent for the strip/title: the SafeArea if present (that is where
        // Title/ScrollView live after the P21 safe-area move), else the panel.
        var title = FindChild(panel.transform, "Title");
        Transform host = title != null ? title.parent : panel.transform;

        var pill = AssetDatabase.LoadAssetAtPath<Sprite>(PillSpritePath);

        // ---- World title: REPURPOSE the existing panel Title ----
        // A separate title collided with the panel header + Back button (P34D
        // overlap audit). The panel's own "Title" becomes the world name at
        // runtime instead. Clean up any separate WorldTitle from an earlier run.
        var strayTitle = FindChild(host, TitleName);
        if (strayTitle != null) Object.DestroyImmediate(strayTitle.gameObject);
        var worldTitleTmp = title != null ? title.GetComponent<TextMeshProUGUI>() : null;

        // ---- World strip (always rebuilt fresh so cell design stays current) ----
        var existing = FindChild(host, StripName);
        if (existing != null) Object.DestroyImmediate(existing.gameObject);

        var stripGO = new GameObject(StripName,
            typeof(RectTransform), typeof(HorizontalLayoutGroup));
        var strip = (RectTransform)stripGO.transform;
        strip.SetParent(host, false);
        var hlg = stripGO.GetComponent<HorizontalLayoutGroup>();
        hlg.spacing = 8f;
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = true;
        hlg.childForceExpandHeight = true;
        for (int w = 1; w <= WorldMapping.WorldCount; w++)
            CreateWorldTab(strip, w, pill);
        AnchorTop(strip, height: 60f, y: -136f, xInset: 40f); // below title + Back band

        // ---- Inset the ScrollView so cards start below the strip ----
        var scroll = FindChild(panel.transform, "ScrollView") as RectTransform;
        if (scroll != null && !Mathf.Approximately(scroll.offsetMax.y, ScrollTopInset))
            scroll.offsetMax = new Vector2(scroll.offsetMax.x, ScrollTopInset);

        // ---- Wire controller ----
        var so = new SerializedObject(controller);
        var worldCatalog = AssetDatabase.LoadAssetAtPath<WorldCatalog>(WorldCatalogPath);
        SetRef(so, "_worldCatalog", worldCatalog);
        SetRef(so, "_worldTabContainer", strip);
        SetRef(so, "_worldTitle", worldTitleTmp);
        if (so.hasModifiedProperties) so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(controller);

        panel.SetActive(false); // Main Menu controls visibility
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[WorldLS] World strip rebuilt ("
            + WorldMapping.WorldCount + " tabs); controller wired"
            + (worldCatalog == null ? " (WARNING: WorldCatalog not found)" : "") + ".");
    }

    private static void CreateWorldTab(Transform parent, int worldNumber, Sprite pill)
    {
        var go = new GameObject("WorldTab_" + worldNumber,
            typeof(RectTransform), typeof(Image), typeof(Button), typeof(WorldTab));
        go.transform.SetParent(parent, false);
        var img = go.GetComponent<Image>();
        if (pill != null) { img.sprite = pill; img.type = Image.Type.Sliced; }
        img.color = Color.white;

        var label = MakeText(go.transform, "Label", worldNumber.ToString(), 26, pillGradient: false);
        Stretch(label);

        // Selected indicator: a gold underline bar, hidden by default.
        var selGO = new GameObject("Selected", typeof(RectTransform), typeof(Image));
        selGO.transform.SetParent(go.transform, false);
        var selRt = (RectTransform)selGO.transform;
        selRt.anchorMin = new Vector2(0.15f, 0f); selRt.anchorMax = new Vector2(0.85f, 0f);
        selRt.pivot = new Vector2(0.5f, 0f);
        selRt.sizeDelta = new Vector2(0f, 5f); selRt.anchoredPosition = new Vector2(0f, 4f);
        selGO.GetComponent<Image>().color = new Color(0.94f, 0.78f, 0.41f, 1f); // gold
        selGO.SetActive(false);

        // No per-tab lock glyph: the tab is just a number, and a locked world's
        // state is already shown by the locked overlays on all its level cards.
        // A corner glyph over the number only cramps it and trips the overlap
        // audit. _lockedIndicator is left unbound (WorldTab.Bind null-guards it).

        var tab = go.GetComponent<WorldTab>();
        var so = new SerializedObject(tab);
        so.FindProperty("_button").objectReferenceValue = go.GetComponent<Button>();
        so.FindProperty("_label").objectReferenceValue = label.GetComponent<TextMeshProUGUI>();
        so.FindProperty("_selectedIndicator").objectReferenceValue = selGO;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    // ---- helpers ----

    private static RectTransform MakeText(Transform parent, string name, string text,
        float size, bool pillGradient)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        tmp.enableWordWrapping = false;
        if (pillGradient)
        {
            tmp.enableVertexGradient = true;
            tmp.colorGradient = new VertexGradient(
                new Color(1f, 0.96f, 0.88f), new Color(1f, 0.96f, 0.88f),
                new Color(0.94f, 0.78f, 0.41f), new Color(0.94f, 0.78f, 0.41f));
        }
        else tmp.color = new Color(0.97f, 0.95f, 0.91f);
        return (RectTransform)go.transform;
    }

    private static void AnchorTop(RectTransform rt, float height, float y, float xInset)
    {
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.sizeDelta = new Vector2(-xInset * 2f, height);
        rt.anchoredPosition = new Vector2(0f, y);
    }

    private static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
    }

    private static void SetRef(SerializedObject so, string field, Object value)
    {
        var p = so.FindProperty(field);
        if (p != null && p.objectReferenceValue != value) p.objectReferenceValue = value;
    }

    private static Transform FindChild(Transform t, string name)
    {
        for (int i = 0; i < t.childCount; i++)
        {
            var c = t.GetChild(i);
            if (c.name == name) return c;
            var r = FindChild(c, name);
            if (r != null) return r;
        }
        return null;
    }
}
