using JebbyJump.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// Builds the modal story-card overlay into MainMenu.unity and wires the
// StoryCardPresenter + MainMenuController._storyOverlay (WorldExpansion100,
// phase P34F). Hidden by default; MainMenuController shows the opening card on
// first launch. Idempotent: the overlay is rebuilt fresh each run.
public static class BuildStoryOverlay
{
    private const string MainMenuScenePath = "Assets/_JebbyJump/Scenes/MainMenu.unity";
    private const string PanelSprite = "Assets/_JebbyJump/Art/Sprites/UI/ui_panel_soft_9s.png";
    private const string PillSprite = "Assets/_JebbyJump/Art/Sprites/UI/ui_btn_pill_9s.png";
    private const string RootName = "StoryOverlay";

    private static readonly Color Cream = new Color(1f, 0.96f, 0.88f);
    private static readonly Color Gold = new Color(0.94f, 0.78f, 0.41f);
    private static readonly Color SoftBody = new Color(0.95f, 0.93f, 0.88f);

    [MenuItem("Jebby Jump/Scaffold/Build Story Overlay")]
    public static void Run()
    {
        var scene = EditorSceneManager.OpenScene(MainMenuScenePath, OpenSceneMode.Single);

        Canvas canvas = null;
        MainMenuController menu = null;
        foreach (var go in scene.GetRootGameObjects())
        {
            if (canvas == null) canvas = go.GetComponentInChildren<Canvas>(true);
            if (menu == null) menu = go.GetComponentInChildren<MainMenuController>(true);
        }
        if (canvas == null) { Debug.LogError("[StoryOverlay] No Canvas in MainMenu."); return; }

        var existing = FindChild(canvas.transform, RootName);
        if (existing != null) Object.DestroyImmediate(existing.gameObject);

        var panel = AssetDatabase.LoadAssetAtPath<Sprite>(PanelSprite);
        var pill = AssetDatabase.LoadAssetAtPath<Sprite>(PillSprite);

        // Root (full-screen, sits above the menu; last sibling = drawn on top).
        var root = new GameObject(RootName, typeof(RectTransform));
        root.transform.SetParent(canvas.transform, false);
        Stretch((RectTransform)root.transform);
        root.transform.SetAsLastSibling();

        // Backdrop: dark, raycast-blocking (modal).
        var backdrop = new GameObject("Backdrop", typeof(RectTransform), typeof(Image));
        backdrop.transform.SetParent(root.transform, false);
        Stretch((RectTransform)backdrop.transform);
        var bimg = backdrop.GetComponent<Image>();
        bimg.color = new Color(0f, 0f, 0f, 0.6f);
        bimg.raycastTarget = true;

        // Card panel (centered).
        var card = new GameObject("Card", typeof(RectTransform), typeof(Image));
        card.transform.SetParent(root.transform, false);
        var cardRt = (RectTransform)card.transform;
        cardRt.anchorMin = cardRt.anchorMax = new Vector2(0.5f, 0.5f);
        cardRt.pivot = new Vector2(0.5f, 0.5f);
        cardRt.sizeDelta = new Vector2(1000f, 560f);
        cardRt.anchoredPosition = Vector2.zero;
        var cimg = card.GetComponent<Image>();
        if (panel != null) { cimg.sprite = panel; cimg.type = Image.Type.Sliced; }
        cimg.color = Color.white;

        // Headline (top, display gradient).
        var headline = MakeText(card.transform, "Headline", "Headline", 48, FontStyles.Bold, true);
        AnchorTop(headline, 90f, -46f, 60f);

        // Body (middle, soft, wraps).
        var body = MakeText(card.transform, "Body", "Body copy.", 30, FontStyles.Normal, false);
        body.anchorMin = new Vector2(0f, 0f); body.anchorMax = new Vector2(1f, 1f);
        // Bottom inset clears the button row (buttons top out ~154 from the
        // card bottom); top inset clears the headline band.
        body.offsetMin = new Vector2(70f, 176f); body.offsetMax = new Vector2(-70f, -150f);
        var bt = body.GetComponent<TextMeshProUGUI>();
        bt.enableWordWrapping = true;
        bt.alignment = TextAlignmentOptions.Center;

        // Button row: Skip (left) + Continue (right).
        var skip = MakeButton(card.transform, "SkipButton", "Skip", pill);
        AnchorBottomLeftish(skip, new Vector2(0.5f, 0f), new Vector2(-170f, 66f));
        var cont = MakeButton(card.transform, "ContinueButton", "Continue", pill);
        AnchorBottomLeftish(cont, new Vector2(0.5f, 0f), new Vector2(170f, 66f));

        // Presenter.
        var presenter = root.AddComponent<StoryCardPresenter>();
        var so = new SerializedObject(presenter);
        so.FindProperty("_root").objectReferenceValue = root;
        so.FindProperty("_headline").objectReferenceValue = headline.GetComponent<TextMeshProUGUI>();
        so.FindProperty("_body").objectReferenceValue = bt;
        so.FindProperty("_continueButton").objectReferenceValue = cont.GetComponent<Button>();
        so.FindProperty("_skipButton").objectReferenceValue = skip.GetComponent<Button>();
        so.ApplyModifiedPropertiesWithoutUndo();

        // Wire the menu + hide by default.
        if (menu != null)
        {
            var mso = new SerializedObject(menu);
            var p = mso.FindProperty("_storyOverlay");
            if (p != null) { p.objectReferenceValue = presenter; mso.ApplyModifiedPropertiesWithoutUndo(); EditorUtility.SetDirty(menu); }
        }
        else Debug.LogWarning("[StoryOverlay] No MainMenuController found to wire.");
        root.SetActive(false);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[StoryOverlay] Built + wired StoryCardPresenter"
            + (menu != null ? " and MainMenuController._storyOverlay." : " (menu NOT wired)."));
    }

    // ---- helpers ----

    private static RectTransform MakeText(Transform parent, string name, string text,
        float size, FontStyles style, bool gradient)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text; tmp.fontSize = size; tmp.fontStyle = style;
        tmp.alignment = TextAlignmentOptions.Center;
        if (gradient)
        {
            tmp.color = Color.white; tmp.enableVertexGradient = true;
            tmp.colorGradient = new VertexGradient(Cream, Cream, Gold, Gold);
        }
        else tmp.color = SoftBody;
        return (RectTransform)go.transform;
    }

    private static RectTransform MakeButton(Transform parent, string name, string label, Sprite pill)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var rt = (RectTransform)go.transform;
        rt.sizeDelta = new Vector2(280f, 88f);
        var img = go.GetComponent<Image>();
        if (pill != null) { img.sprite = pill; img.type = Image.Type.Sliced; }
        img.color = Color.white;
        var lbl = MakeText(go.transform, "Label", label, 32, FontStyles.Bold, false);
        Stretch(lbl); lbl.GetComponent<TextMeshProUGUI>().color = new Color(0.97f, 0.95f, 0.91f);
        return rt;
    }

    private static void AnchorTop(RectTransform rt, float height, float y, float xInset)
    {
        rt.anchorMin = new Vector2(0f, 1f); rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.sizeDelta = new Vector2(-xInset * 2f, height);
        rt.anchoredPosition = new Vector2(0f, y);
    }

    private static void AnchorBottomLeftish(RectTransform rt, Vector2 anchor, Vector2 pos)
    {
        rt.anchorMin = rt.anchorMax = anchor; rt.pivot = new Vector2(0.5f, 0f);
        rt.anchoredPosition = pos;
    }

    private static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
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
