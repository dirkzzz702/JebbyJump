using JebbyJump.Progression;
using JebbyJump.UI;
using JebbyJump.Wardrobe.Visual;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Idempotent scaffold: adds a Wardrobe button + WardrobePanel to MainMenu.unity
// and wires MainMenuController + WardrobePanelController. P20 adds a
// safe-area-fitted content root + responsive region containers
// (Header/List/Preview/Action) the controller positions at runtime, and a
// safe-area-fitted ceremony card. Re-runs reuse + migrate existing objects;
// nothing is duplicated. Run AFTER "Build Settings Panel".
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
            + "(safe-area + responsive regions). _catalog wired: " + catalogWired);
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
        GameObject panelGO;
        if (existingCtrl != null)
        {
            panelGO = existingCtrl.gameObject;
        }
        else
        {
            panelGO = new GameObject(
                "WardrobePanel",
                typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panelGO.transform.SetParent(canvas.transform, false);
            StretchToParent(panelGO.GetComponent<RectTransform>());
            panelGO.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.85f);
            Debug.Log("[Wardrobe] Created WardrobePanel.");
        }

        BuildStructure(panelGO);

        var controller = panelGO.GetComponent<WardrobePanelController>()
            ?? panelGO.AddComponent<WardrobePanelController>();
        WirePanel(controller);
        panelGO.SetActive(false);
        return controller;
    }

    // Idempotently ensures the safe-area root + 4 responsive region containers,
    // migrating any pre-P20 children into the right region. Region rects are
    // seeded for 1920x1080 (the controller re-applies per device at runtime).
    private static void BuildStructure(GameObject panelGO)
    {
        Transform safeArea = EnsureSafeArea(panelGO).transform;

        var header = EnsureRegion(safeArea, "HeaderRegion");
        var list = EnsureRegion(safeArea, "ListRegion");
        var preview = EnsureRegion(safeArea, "PreviewRegion");
        var action = EnsureRegion(safeArea, "ActionRegion");

        var layout = WardrobeResponsiveLayout.Compute(new Vector2(1920f, 1080f));
        SetRegionRect(header, layout.Header);
        SetRegionRect(list, layout.List);
        SetRegionRect(preview, layout.Preview);
        SetRegionRect(action, layout.Actions);

        Transform panel = panelGO.transform;

        // Header: Title fills the region.
        var title = EnsureText(panel, header, "Title", "Wardrobe", 60, FontStyles.Bold);
        Fill(title.GetComponent<RectTransform>());

        // List: ScrollView (+ Viewport + Content) fills the region.
        EnsureScrollView(panel, list);

        // Preview: large preview image (top) + selected/state labels (below).
        var selPreview = EnsureSelectedPreview(panel, preview);
        SetAnchored(selPreview.GetComponent<RectTransform>(),
            new Vector2(0.05f, 0.4f), new Vector2(0.95f, 1f));
        var prevLabel = EnsureText(
            panel, preview, "PreviewLabel", "Selected: --", 30, FontStyles.Bold);
        SetAnchored(prevLabel.GetComponent<RectTransform>(),
            new Vector2(0.02f, 0.2f), new Vector2(0.98f, 0.38f));
        var stateLbl = EnsureText(
            panel, preview, "StateLabel", "", 24, FontStyles.Normal);
        SetAnchored(stateLbl.GetComponent<RectTransform>(),
            new Vector2(0.02f, 0.02f), new Vector2(0.98f, 0.18f));

        // Action: Equip + Back side by side, centered, metrics-sized.
        var equip = EnsureButton(panel, action, "EquipButton", "Equip");
        SetButton(equip.GetComponent<RectTransform>(), new Vector2(1f, 0.5f),
            new Vector2(-12f, 0f));
        var back = EnsureButton(panel, action, "BackButton", "Back");
        SetButton(back.GetComponent<RectTransform>(), new Vector2(0f, 0.5f),
            new Vector2(12f, 0f));

        EnsureCeremonyOverlay(panelGO);
    }

    private static SafeAreaFitter EnsureSafeArea(GameObject panelGO)
    {
        var existing = panelGO.transform.Find("SafeArea");
        GameObject safeGO;
        if (existing != null) safeGO = existing.gameObject;
        else
        {
            safeGO = new GameObject("SafeArea", typeof(RectTransform));
            safeGO.transform.SetParent(panelGO.transform, false);
            StretchToParent(safeGO.GetComponent<RectTransform>());
            // Keep the safe-area root above the dim backdrop, below the overlay.
            safeGO.transform.SetSiblingIndex(0);
            Debug.Log("[Wardrobe] Created SafeArea content root.");
        }
        var fitter = safeGO.GetComponent<SafeAreaFitter>()
            ?? safeGO.AddComponent<SafeAreaFitter>();
        var so = new SerializedObject(fitter);
        so.FindProperty("_target").objectReferenceValue =
            safeGO.GetComponent<RectTransform>();
        so.ApplyModifiedPropertiesWithoutUndo();
        return fitter;
    }

    private static RectTransform EnsureRegion(Transform parent, string name)
    {
        var existing = parent.Find(name);
        if (existing != null) return existing as RectTransform;
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go.GetComponent<RectTransform>();
    }

    private static void EnsureScrollView(Transform panel, Transform listRegion)
    {
        var existing = FindDeep(panel, "ScrollView");
        GameObject scrollGO;
        if (existing != null)
        {
            scrollGO = existing.gameObject;
            scrollGO.transform.SetParent(listRegion, false);
        }
        else
        {
            scrollGO = new GameObject(
                "ScrollView", typeof(RectTransform), typeof(CanvasRenderer),
                typeof(Image), typeof(ScrollRect));
            scrollGO.transform.SetParent(listRegion, false);
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
            vlg.spacing = WardrobeLayoutMetrics.RowSpacing;
            int p = (int)WardrobeLayoutMetrics.ListPadding;
            vlg.padding = new RectOffset(p, p, p, p);
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
        }
        Fill(scrollGO.GetComponent<RectTransform>());
    }

    private static void WirePanel(WardrobePanelController ctrl)
    {
        var panel = ctrl.gameObject;
        var so = new SerializedObject(ctrl);
        Set(so, "_panelRoot", panel);

        var safeArea = panel.transform.Find("SafeArea");
        Set(so, "_safeAreaRoot", safeArea as RectTransform);
        Set(so, "_headerRegion", safeArea?.Find("HeaderRegion") as RectTransform);
        Set(so, "_listRegion", safeArea?.Find("ListRegion") as RectTransform);
        Set(so, "_previewRegion", safeArea?.Find("PreviewRegion") as RectTransform);
        Set(so, "_actionRegion", safeArea?.Find("ActionRegion") as RectTransform);

        var scroll = FindDeep(panel.transform, "ScrollView");
        Set(so, "_scrollRect", scroll?.GetComponent<ScrollRect>());
        Set(so, "_rowContainer", FindDeep(panel.transform, "Content") as RectTransform);
        Set(so, "_previewLabel", FindText(panel, "PreviewLabel"));
        Set(so, "_stateLabel", FindText(panel, "StateLabel"));
        Set(so, "_equipButton", FindDeep(panel.transform, "EquipButton")?.GetComponent<Button>());
        Set(so, "_backButton", FindDeep(panel.transform, "BackButton")?.GetComponent<Button>());
        Set(so, "_selectedPreviewImage",
            FindDeep(panel.transform, "SelectedPreview")?.GetComponent<Image>());
        Set(so, "_previewLibrary", LoadPreviewLibrary());

        var overlay = panel.transform.Find("UnlockCeremonyOverlay")?.gameObject;
        Set(so, "_ceremonyOverlay", overlay);
        if (overlay != null)
        {
            Set(so, "_ceremonyTitle", FindText(overlay, "CeremonySafeArea/CeremonyCard/Title"));
            Set(so, "_ceremonyOutfitName", FindText(overlay, "CeremonySafeArea/CeremonyCard/OutfitName"));
            Set(so, "_ceremonyMessage", FindText(overlay, "CeremonySafeArea/CeremonyCard/Message"));
            Set(so, "_ceremonyPreviewImage",
                FindDeep(overlay.transform, "PreviewImage")?.GetComponent<Image>());
            Set(so, "_ceremonyEquipButton",
                FindDeep(overlay.transform, "EquipNowButton")?.GetComponent<Button>());
            Set(so, "_ceremonyContinueButton",
                FindDeep(overlay.transform, "ContinueButton")?.GetComponent<Button>());
        }

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

    private static GameObject EnsureSelectedPreview(Transform panel, Transform parent)
    {
        var existing = FindDeep(panel, "SelectedPreview");
        if (existing != null)
        {
            existing.SetParent(parent, false);
            return existing.gameObject;
        }
        var go = new GameObject(
            "SelectedPreview",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        var img = go.GetComponent<Image>();
        img.preserveAspect = true;
        img.raycastTarget = false;
        img.enabled = false;
        Debug.Log("[Wardrobe] Created SelectedPreview image.");
        return go;
    }

    private static TextMeshProUGUI FindText(GameObject root, string path)
    {
        var t = root.transform.Find(path);
        if (t != null) return t.GetComponent<TextMeshProUGUI>();
        // fallback: search by leaf name
        string leaf = path.Contains("/") ? path.Substring(path.LastIndexOf('/') + 1) : path;
        var deep = FindDeep(root.transform, leaf);
        return deep != null ? deep.GetComponent<TextMeshProUGUI>() : null;
    }

    // Full-screen dim ceremony backdrop (raycast block) + a safe-area-fitted
    // root holding the centered card with title / preview / name / message /
    // Equip Now + Continue. Idempotent: reuses + migrates the existing card.
    private static void EnsureCeremonyOverlay(GameObject panel)
    {
        var existing = panel.transform.Find("UnlockCeremonyOverlay");
        GameObject overlay;
        if (existing != null) overlay = existing.gameObject;
        else
        {
            overlay = new GameObject(
                "UnlockCeremonyOverlay",
                typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            overlay.transform.SetParent(panel.transform, false);
            StretchToParent(overlay.GetComponent<RectTransform>());
            overlay.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.85f);
            Debug.Log("[Wardrobe] Created UnlockCeremonyOverlay.");
        }
        overlay.transform.SetAsLastSibling(); // above the SafeArea content

        // Safe-area-fitted root for the card (backdrop stays full-screen).
        var safeExisting = overlay.transform.Find("CeremonySafeArea");
        GameObject safeGO;
        if (safeExisting != null) safeGO = safeExisting.gameObject;
        else
        {
            safeGO = new GameObject("CeremonySafeArea", typeof(RectTransform));
            safeGO.transform.SetParent(overlay.transform, false);
            StretchToParent(safeGO.GetComponent<RectTransform>());
            Debug.Log("[Wardrobe] Created CeremonySafeArea.");
        }
        var fitter = safeGO.GetComponent<SafeAreaFitter>()
            ?? safeGO.AddComponent<SafeAreaFitter>();
        var fso = new SerializedObject(fitter);
        fso.FindProperty("_target").objectReferenceValue =
            safeGO.GetComponent<RectTransform>();
        fso.ApplyModifiedPropertiesWithoutUndo();

        var c = new Vector2(0.5f, 0.5f);
        var cardExisting = FindDeep(overlay.transform, "CeremonyCard");
        GameObject card;
        if (cardExisting != null)
        {
            card = cardExisting.gameObject;
            card.transform.SetParent(safeGO.transform, false);
        }
        else
        {
            card = new GameObject(
                "CeremonyCard",
                typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            card.transform.SetParent(safeGO.transform, false);
            card.GetComponent<Image>().color = new Color(0.15f, 0.15f, 0.2f, 0.98f);
            SetRect(card, c, c, c, new Vector2(0f, 0f), new Vector2(640f, 560f));

            var title = CreateText(card.transform, "Title",
                "New Outfit Unlocked!", 48, FontStyles.Bold);
            SetRect(title, c, c, c, new Vector2(0f, 210f), new Vector2(600f, 80f));

            var previewGo = new GameObject(
                "PreviewImage",
                typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            previewGo.transform.SetParent(card.transform, false);
            var previewImg = previewGo.GetComponent<Image>();
            previewImg.preserveAspect = true;
            previewImg.raycastTarget = false;
            previewImg.enabled = false;
            SetRect(previewGo, c, c, c, new Vector2(0f, 40f), new Vector2(220f, 240f));

            var name = CreateText(card.transform, "OutfitName", "", 36, FontStyles.Bold);
            SetRect(name, c, c, c, new Vector2(0f, -120f), new Vector2(600f, 60f));

            var msg = CreateText(card.transform, "Message", "", 28, FontStyles.Normal);
            SetRect(msg, c, c, c, new Vector2(0f, -172f), new Vector2(600f, 48f));

            var equip = CreateButton(card.transform, "EquipNowButton", "Equip Now");
            SetRect(equip, c, c, c, new Vector2(-150f, -240f),
                new Vector2(WardrobeLayoutMetrics.CeremonyButtonWidth,
                    WardrobeLayoutMetrics.CeremonyButtonHeight));

            var cont = CreateButton(card.transform, "ContinueButton", "Continue");
            SetRect(cont, c, c, c, new Vector2(150f, -240f),
                new Vector2(WardrobeLayoutMetrics.CeremonyButtonWidth,
                    WardrobeLayoutMetrics.CeremonyButtonHeight));
        }

        overlay.SetActive(false);
    }

    private static WardrobePreviewLibrary LoadPreviewLibrary()
    {
        const string path =
            "Assets/_JebbyJump/Art/Characters/Jebby/Outfits/WardrobePreviewLibrary.asset";
        var lib = AssetDatabase.LoadAssetAtPath<WardrobePreviewLibrary>(path);
        if (lib == null)
            Debug.LogWarning(
                "[Wardrobe] WardrobePreviewLibrary not found; run "
                + "'Build Wardrobe Preview Library' first.");
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

    // Depth-first search by name among descendants (not the root itself).
    private static Transform FindDeep(Transform root, string name)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            var c = root.GetChild(i);
            if (c.name == name) return c;
            var r = FindDeep(c, name);
            if (r != null) return r;
        }
        return null;
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

    // Region: anchored bottom-left of the SafeArea root; positioned by rect.
    private static void SetRegionRect(RectTransform rt, Rect r)
    {
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.zero;
        rt.pivot = Vector2.zero;
        rt.anchoredPosition = new Vector2(r.x, r.y);
        rt.sizeDelta = new Vector2(r.width, r.height);
    }

    private static void Fill(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
    }

    private static void SetAnchored(RectTransform rt, Vector2 min, Vector2 max)
    {
        rt.anchorMin = min; rt.anchorMax = max;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
    }

    // Action button: anchored to the region center, metrics-sized.
    private static void SetButton(RectTransform rt, Vector2 pivot, Vector2 pos)
    {
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = pivot;
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(
            WardrobeLayoutMetrics.ActionButtonWidth,
            WardrobeLayoutMetrics.ActionButtonHeight);
    }

    private static GameObject EnsureText(
        Transform panel, Transform parent, string name, string text,
        int fontSize, FontStyles style)
    {
        var existing = FindDeep(panel, name);
        if (existing != null && existing.GetComponent<TextMeshProUGUI>() != null)
        {
            existing.SetParent(parent, false);
            return existing.gameObject;
        }
        return CreateText(parent, name, text, fontSize, style);
    }

    private static GameObject EnsureButton(
        Transform panel, Transform parent, string name, string label)
    {
        var existing = FindDeep(panel, name);
        if (existing != null && existing.GetComponent<Button>() != null)
        {
            existing.SetParent(parent, false);
            SetLabel(existing.GetComponent<Button>(), label);
            return existing.gameObject;
        }
        return CreateButton(parent, name, label);
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
        Fill(labelGO.GetComponent<RectTransform>());
        return go;
    }

    private static void StretchToParent(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero; rt.sizeDelta = Vector2.zero;
    }
}
