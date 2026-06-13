using JebbyJump.Progression;
using JebbyJump.UI;
using JebbyJump.Wardrobe.Visual;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Idempotent scaffold: adds a Wardrobe button + text-only WardrobePanel to
// MainMenu.unity and wires MainMenuController + WardrobePanelController.
// Clones the Start button for the new menu button and restacks the five
// menu buttons (Continue / Level Select / Settings / Wardrobe / Quit).
// Re-runs reuse existing objects; nothing is duplicated. Run this AFTER
// "Build Settings Panel" - it owns the final 5-button stack layout.
public static class BuildWardrobePanel
{
    private const string MainMenuScenePath =
        "Assets/_JebbyJump/Scenes/MainMenu.unity";

    // Five-button centered stack (top -> bottom), 100px spacing.
    private const float ContinueY = 200f;
    private const float StartY    = 100f;
    private const float SettingsY = 0f;
    private const float WardrobeY = -100f;
    private const float QuitY     = -200f;

    [MenuItem("Jebby Jump/Scaffold/Build Wardrobe Panel")]
    public static void Run()
    {
        if (!System.IO.File.Exists(MainMenuScenePath))
        {
            Debug.LogError(
                "[Wardrobe] MainMenu scene not found at " + MainMenuScenePath);
            return;
        }
        OpenScene(MainMenuScenePath);

        var menu = Object.FindFirstObjectByType<MainMenuController>(
            FindObjectsInactive.Include);
        if (menu == null)
        {
            Debug.LogError("[Wardrobe] No MainMenuController. Abort.");
            return;
        }

        var menuSo = new SerializedObject(menu);
        var startButton = menuSo.FindProperty("_startButton")
            .objectReferenceValue as Button;
        var continueButton = menuSo.FindProperty("_continueButton")
            .objectReferenceValue as Button;
        var settingsButton = menuSo.FindProperty("_settingsButton")
            .objectReferenceValue as Button;
        var quitButton = menuSo.FindProperty("_quitButton")
            .objectReferenceValue as Button;
        if (startButton == null)
        {
            Debug.LogError(
                "[Wardrobe] MainMenuController._startButton not wired; "
                + "need it as the clone template. Abort.");
            return;
        }

        Transform parent = startButton.transform.parent;
        float x = startButton.GetComponent<RectTransform>().anchoredPosition.x;

        Button wardrobeButton = EnsureWardrobeButton(parent, startButton);

        SetY(continueButton, x, ContinueY);
        SetY(startButton, x, StartY);
        SetY(settingsButton, x, SettingsY);
        SetY(wardrobeButton, x, WardrobeY);
        SetY(quitButton, x, QuitY);
        // Order Wardrobe between Settings and Quit in sibling order.
        if (quitButton != null)
            wardrobeButton.transform.SetSiblingIndex(
                quitButton.transform.GetSiblingIndex());

        Canvas canvas = FindMainCanvas();
        if (canvas == null)
        {
            Debug.LogError("[Wardrobe] No Canvas in MainMenu. Abort.");
            return;
        }

        WardrobePanelController panel = EnsureWardrobePanel(canvas);

        menuSo.FindProperty("_wardrobeButton").objectReferenceValue =
            wardrobeButton;
        menuSo.FindProperty("_wardrobePanel").objectReferenceValue = panel;
        menuSo.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(menu);

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());

        bool catalogWired =
            new SerializedObject(panel).FindProperty("_catalog")
                .objectReferenceValue != null;
        Debug.Log(
            "[Wardrobe] Scaffolded WardrobeButton + WardrobePanel "
            + "(5-button stack Continue/Level Select/Settings/Wardrobe/Quit). "
            + "_catalog wired: " + catalogWired);
    }

    private static Button EnsureWardrobeButton(
        Transform parent, Button template)
    {
        var existing = parent.Find("WardrobeButton");
        if (existing != null)
        {
            SetLabel(existing.GetComponent<Button>(), "Wardrobe");
            return existing.GetComponent<Button>();
        }
        var clone = Object.Instantiate(template.gameObject, parent);
        clone.name = "WardrobeButton";
        SetLabel(clone.GetComponent<Button>(), "Wardrobe");
        Debug.Log("[Wardrobe] Created WardrobeButton (cloned Start).");
        return clone.GetComponent<Button>();
    }

    private static WardrobePanelController EnsureWardrobePanel(Canvas canvas)
    {
        var existingCtrl =
            Object.FindFirstObjectByType<WardrobePanelController>(
                FindObjectsInactive.Include);
        if (existingCtrl != null)
        {
            WirePanel(existingCtrl);
            existingCtrl.gameObject.SetActive(false);
            return existingCtrl;
        }

        var panelGO = new GameObject(
            "WardrobePanel",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panelGO.transform.SetParent(canvas.transform, false);
        StretchToParent(panelGO.GetComponent<RectTransform>());
        panelGO.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.85f);

        var title = CreateText(panelGO.transform, "Title", "Wardrobe", 60,
            FontStyles.Bold);
        SetRect(title, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0.5f, 1f), new Vector2(0f, -40f),
            new Vector2(600f, 80f));

        // Scrollable outfit-row container.
        var scrollGO = new GameObject(
            "ScrollView", typeof(RectTransform), typeof(CanvasRenderer),
            typeof(Image), typeof(ScrollRect));
        scrollGO.transform.SetParent(panelGO.transform, false);
        var scrollRT = scrollGO.GetComponent<RectTransform>();
        scrollRT.anchorMin = new Vector2(0.5f, 0.5f);
        scrollRT.anchorMax = new Vector2(0.5f, 0.5f);
        scrollRT.pivot = new Vector2(0.5f, 0.5f);
        scrollRT.anchoredPosition = new Vector2(0f, 20f);
        scrollRT.sizeDelta = new Vector2(720f, 520f);
        scrollGO.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.04f);

        var viewportGO = new GameObject(
            "Viewport", typeof(RectTransform), typeof(CanvasRenderer),
            typeof(Image), typeof(Mask));
        viewportGO.transform.SetParent(scrollGO.transform, false);
        StretchToParent(viewportGO.GetComponent<RectTransform>());
        viewportGO.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.02f);
        viewportGO.GetComponent<Mask>().showMaskGraphic = false;

        var content = new GameObject(
            "Content", typeof(RectTransform), typeof(VerticalLayoutGroup),
            typeof(ContentSizeFitter));
        content.transform.SetParent(viewportGO.transform, false);
        var contentRT = content.GetComponent<RectTransform>();
        contentRT.anchorMin = new Vector2(0f, 1f);
        contentRT.anchorMax = new Vector2(1f, 1f);
        contentRT.pivot = new Vector2(0.5f, 1f);
        contentRT.anchoredPosition = Vector2.zero;
        contentRT.sizeDelta = Vector2.zero;
        var vlg = content.GetComponent<VerticalLayoutGroup>();
        vlg.spacing = 12f;
        vlg.padding = new RectOffset(12, 12, 12, 12);
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        var fitter = content.GetComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var scroll = scrollGO.GetComponent<ScrollRect>();
        scroll.content = contentRT;
        scroll.viewport = viewportGO.GetComponent<RectTransform>();
        scroll.horizontal = false;
        scroll.vertical = true;

        var preview = CreateText(panelGO.transform, "PreviewLabel",
            "Selected: --", 30, FontStyles.Bold);
        SetRect(preview, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(0f, -300f),
            new Vector2(700f, 40f));

        var stateLbl = CreateText(panelGO.transform, "StateLabel", "", 24,
            FontStyles.Normal);
        SetRect(stateLbl, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(0f, -336f),
            new Vector2(700f, 32f));

        var equip = CreateButton(panelGO.transform, "EquipButton", "Equip");
        SetRect(equip, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
            new Vector2(0.5f, 0f), new Vector2(-110f, 60f),
            new Vector2(180f, 72f));

        var back = CreateButton(panelGO.transform, "BackButton", "Back");
        SetRect(back, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
            new Vector2(0.5f, 0f), new Vector2(110f, 60f),
            new Vector2(180f, 72f));

        var controller = panelGO.AddComponent<WardrobePanelController>();
        WirePanel(controller);
        panelGO.SetActive(false);
        Debug.Log("[Wardrobe] Created WardrobePanel.");
        return controller;
    }

    private static void WirePanel(WardrobePanelController ctrl)
    {
        var panel = ctrl.gameObject;
        var so = new SerializedObject(ctrl);
        Set(so, "_panelRoot", panel);
        Set(so, "_rowContainer",
            panel.transform.Find("ScrollView/Viewport/Content") as RectTransform);
        Set(so, "_previewLabel",
            panel.transform.Find("PreviewLabel")?.GetComponent<TextMeshProUGUI>());
        Set(so, "_stateLabel",
            panel.transform.Find("StateLabel")?.GetComponent<TextMeshProUGUI>());
        Set(so, "_equipButton",
            panel.transform.Find("EquipButton")?.GetComponent<Button>());
        Set(so, "_backButton",
            panel.transform.Find("BackButton")?.GetComponent<Button>());
        Set(so, "_selectedPreviewImage", EnsureSelectedPreview(panel));
        Set(so, "_previewLibrary", LoadPreviewLibrary());
        var catalogProp = so.FindProperty("_catalog");
        if (catalogProp.objectReferenceValue == null)
        {
            var catalog = FindSingleCatalog();
            if (catalog != null) catalogProp.objectReferenceValue = catalog;
            else Debug.LogWarning(
                "[Wardrobe] No single LevelCatalog asset; wire "
                + "WardrobePanelController._catalog by hand.");
        }
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(ctrl);
    }

    // ---- helpers ------------------------------------------------------

    // Optional larger thumbnail of the selected outfit. Created once at a
    // conservative left-of-preview-label spot; null-safe in the controller.
    // Exact placement/readability is part of the deferred wardrobe visual QA.
    private static Image EnsureSelectedPreview(GameObject panel)
    {
        var existing = panel.transform.Find("SelectedPreview");
        if (existing != null) return existing.GetComponent<Image>();

        var go = new GameObject(
            "SelectedPreview",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(panel.transform, false);
        var img = go.GetComponent<Image>();
        img.preserveAspect = true;
        img.raycastTarget = false;
        img.enabled = false; // shown only when a sprite is assigned
        SetRect(go, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(-300f, -300f),
            new Vector2(56f, 64f));
        Debug.Log("[Wardrobe] Created SelectedPreview image.");
        return img;
    }

    private static WardrobePreviewLibrary LoadPreviewLibrary()
    {
        const string path =
            "Assets/_JebbyJump/Art/Characters/Jebby/Outfits/WardrobePreviewLibrary.asset";
        var lib = AssetDatabase.LoadAssetAtPath<WardrobePreviewLibrary>(path);
        if (lib == null)
            Debug.LogWarning(
                "[Wardrobe] WardrobePreviewLibrary not found; run "
                + "'Build Wardrobe Preview Library' first. _previewLibrary "
                + "left unwired (panel falls back to text-only rows).");
        return lib;
    }

    private static LevelCatalog FindSingleCatalog()
    {
        string[] guids = AssetDatabase.FindAssets("t:LevelCatalog");
        if (guids == null || guids.Length != 1) return null;
        return AssetDatabase.LoadAssetAtPath<LevelCatalog>(
            AssetDatabase.GUIDToAssetPath(guids[0]));
    }

    private static void OpenScene(string path)
    {
        var active = SceneManager.GetActiveScene();
        if (active.path != path)
        {
            if (active.isDirty) EditorSceneManager.SaveScene(active);
            EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
        }
    }

    private static Canvas FindMainCanvas()
    {
        var canvases = Object.FindObjectsByType<Canvas>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < canvases.Length; i++)
            if (canvases[i].isRootCanvas) return canvases[i];
        return canvases.Length > 0 ? canvases[0] : null;
    }

    private static void Set(SerializedObject so, string field, Object value)
    {
        var prop = so.FindProperty(field);
        if (prop != null) prop.objectReferenceValue = value;
    }

    private static void SetLabel(Button button, string text)
    {
        if (button == null) return;
        var label = button.GetComponentInChildren<TextMeshProUGUI>(true);
        if (label != null) { label.text = text; EditorUtility.SetDirty(label); }
    }

    private static void SetY(Button b, float x, float y)
    {
        if (b == null) return;
        var rt = b.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(x, y);
        EditorUtility.SetDirty(rt);
    }

    private static void SetRect(
        GameObject go, Vector2 aMin, Vector2 aMax, Vector2 pivot,
        Vector2 pos, Vector2 size)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = aMin; rt.anchorMax = aMax; rt.pivot = pivot;
        rt.anchoredPosition = pos; rt.sizeDelta = size;
    }

    private static void StretchToParent(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero; rt.sizeDelta = Vector2.zero;
    }

    private static GameObject CreateText(
        Transform parent, string name, string text, int fontSize,
        FontStyles style)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text; tmp.fontSize = fontSize; tmp.fontStyle = style;
        tmp.alignment = TextAlignmentOptions.Center; tmp.color = Color.white;
        return go;
    }

    private static GameObject CreateButton(
        Transform parent, string name, string label)
    {
        var go = new GameObject(
            name, typeof(RectTransform), typeof(CanvasRenderer),
            typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.25f, 0.95f);
        var labelGO = CreateText(go.transform, "Label", label, 28,
            FontStyles.Normal);
        StretchToParent(labelGO.GetComponent<RectTransform>());
        return go;
    }
}
