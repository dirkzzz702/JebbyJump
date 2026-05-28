using JebbyJump.Progression;
using JebbyJump.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// One-shot scaffold: builds a LevelSelectPanel (hidden by default)
// inside MainMenu.unity, creates a LevelSelectCard prefab if missing,
// and wires references where it can safely guess. Re-running is
// idempotent: if the panel already exists, only the wiring is
// refreshed. The user still has to drag the LevelCatalog asset into
// LevelSelectController._catalog (the scaffold can't safely guess
// across assets if multiple catalogs exist) and save the scene.
public static class BuildLevelSelectPanel
{
    private const string MainMenuScenePath =
        "Assets/_JebbyJump/Scenes/MainMenu.unity";
    private const string CardPrefabPath =
        "Assets/_JebbyJump/Prefabs/UI/LevelSelectCard.prefab";

    [MenuItem("Jebby Jump/Scaffold/Build Level Select Panel")]
    public static void Run()
    {
        if (!System.IO.File.Exists(MainMenuScenePath))
        {
            Debug.LogError(
                "[Scaffold] MainMenu scene not found at "
                + MainMenuScenePath);
            return;
        }

        EnsureSceneOpenInEditor();

        Canvas canvas = FindMainCanvas();
        if (canvas == null)
        {
            Debug.LogError(
                "[Scaffold] No Canvas found in MainMenu.unity. "
                + "Abort: the Main Menu must already have a Canvas.");
            return;
        }

        var panelRoot = canvas.transform.Find("LevelSelectPanel") as RectTransform;
        bool panelCreated = false;
        if (panelRoot == null)
        {
            panelRoot = CreatePanelHierarchy(canvas);
            panelCreated = true;
        }
        else
        {
            Debug.Log(
                "[Scaffold] LevelSelectPanel already present - "
                + "skipping creation, refreshing wiring only.");
        }

        var cardPrefab = EnsureCardPrefab();

        var controller =
            panelRoot.GetComponent<LevelSelectController>()
            ?? panelRoot.gameObject.AddComponent<LevelSelectController>();

        var content =
            panelRoot.Find("ScrollView/Viewport/Content") as RectTransform;
        var backButton = panelRoot.Find("BackButton")?.GetComponent<Button>();

        var catalog = FindSingleCatalog();
        WireController(
            controller, panelRoot, content, backButton, cardPrefab, catalog);
        bool menuWired = WireMainMenuStart(controller);

        // Default to hidden; Main Menu controls visibility via Open().
        panelRoot.gameObject.SetActive(false);

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());

        if (panelCreated)
            Debug.Log("[Scaffold] LevelSelectPanel built.");
        Debug.Log(
            "[Scaffold] Wired controller: panelRoot, cardContainer, "
            + "backButton, cardPrefab.");
        Debug.Log(
            "[Scaffold] MainMenuController._levelSelect wired: " + menuWired);

        if (catalog != null)
        {
            Debug.Log(
                "[Scaffold] Wired LevelSelectController._catalog to "
                + AssetDatabase.GetAssetPath(catalog));
        }
        else
        {
            Debug.LogWarning(
                "[Scaffold] STILL MANUAL: no single LevelCatalog asset "
                + "found to wire into LevelSelectController._catalog. Run "
                + "'Jebby Jump/Progression/Create Or Sync Level Catalog' "
                + "first, then re-run this scaffold or wire it by hand.");
        }
    }

    // Returns the only LevelCatalog asset in the project, or null if there
    // are zero or more than one (ambiguous -> leave for manual wiring).
    private static LevelCatalog FindSingleCatalog()
    {
        string[] guids = AssetDatabase.FindAssets("t:LevelCatalog");
        if (guids == null || guids.Length != 1) return null;
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        return AssetDatabase.LoadAssetAtPath<LevelCatalog>(path);
    }

    private static void EnsureSceneOpenInEditor()
    {
        var active = SceneManager.GetActiveScene();
        if (active.path != MainMenuScenePath)
        {
            if (active.isDirty)
                EditorSceneManager.SaveScene(active);
            EditorSceneManager.OpenScene(
                MainMenuScenePath, OpenSceneMode.Single);
        }
    }

    private static Canvas FindMainCanvas()
    {
        var canvases = Object.FindObjectsByType<Canvas>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < canvases.Length; i++)
        {
            if (canvases[i].isRootCanvas) return canvases[i];
        }
        return canvases.Length > 0 ? canvases[0] : null;
    }

    private static RectTransform CreatePanelHierarchy(Canvas canvas)
    {
        var panelGO = new GameObject(
            "LevelSelectPanel",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panelGO.transform.SetParent(canvas.transform, false);
        var panelRT = panelGO.GetComponent<RectTransform>();
        StretchToParent(panelRT);
        panelGO.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.85f);

        // Title.
        var titleGO = CreateText(
            panelGO.transform, "Title", "Select Level", 48, FontStyles.Bold);
        var titleRT = titleGO.GetComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0.5f, 1f);
        titleRT.anchorMax = new Vector2(0.5f, 1f);
        titleRT.pivot     = new Vector2(0.5f, 1f);
        titleRT.anchoredPosition = new Vector2(0f, -32f);
        titleRT.sizeDelta = new Vector2(600f, 64f);

        // Back button (top-left).
        var backGO = CreateButton(panelGO.transform, "BackButton", "Back");
        var backRT = backGO.GetComponent<RectTransform>();
        backRT.anchorMin = new Vector2(0f, 1f);
        backRT.anchorMax = new Vector2(0f, 1f);
        backRT.pivot     = new Vector2(0f, 1f);
        backRT.anchoredPosition = new Vector2(32f, -32f);
        backRT.sizeDelta = new Vector2(180f, 72f);

        // Scroll view.
        var scrollGO = new GameObject(
            "ScrollView",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image),
            typeof(ScrollRect));
        scrollGO.transform.SetParent(panelGO.transform, false);
        var scrollRT = scrollGO.GetComponent<RectTransform>();
        scrollRT.anchorMin = new Vector2(0.5f, 0.5f);
        scrollRT.anchorMax = new Vector2(0.5f, 0.5f);
        scrollRT.pivot     = new Vector2(0.5f, 0.5f);
        scrollRT.anchoredPosition = new Vector2(0f, -40f);
        scrollRT.sizeDelta = new Vector2(960f, 1200f);
        scrollGO.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.05f);

        var viewportGO = new GameObject(
            "Viewport",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image),
            typeof(Mask));
        viewportGO.transform.SetParent(scrollGO.transform, false);
        var viewportRT = viewportGO.GetComponent<RectTransform>();
        StretchToParent(viewportRT);
        viewportGO.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.02f);
        viewportGO.GetComponent<Mask>().showMaskGraphic = false;

        var contentGO = new GameObject(
            "Content",
            typeof(RectTransform), typeof(GridLayoutGroup),
            typeof(ContentSizeFitter));
        contentGO.transform.SetParent(viewportGO.transform, false);
        var contentRT = contentGO.GetComponent<RectTransform>();
        contentRT.anchorMin = new Vector2(0f, 1f);
        contentRT.anchorMax = new Vector2(1f, 1f);
        contentRT.pivot     = new Vector2(0.5f, 1f);
        contentRT.anchoredPosition = Vector2.zero;
        contentRT.sizeDelta = new Vector2(0f, 0f);
        var grid = contentGO.GetComponent<GridLayoutGroup>();
        grid.cellSize        = new Vector2(180f, 220f);
        grid.spacing         = new Vector2(24f, 24f);
        grid.padding         = new RectOffset(24, 24, 24, 24);
        grid.startCorner     = GridLayoutGroup.Corner.UpperLeft;
        grid.startAxis       = GridLayoutGroup.Axis.Horizontal;
        grid.childAlignment  = TextAnchor.UpperCenter;
        grid.constraint      = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 5;
        var fitter = contentGO.GetComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit   = ContentSizeFitter.FitMode.PreferredSize;

        var scroll = scrollGO.GetComponent<ScrollRect>();
        scroll.content    = contentRT;
        scroll.viewport   = viewportRT;
        scroll.horizontal = false;
        scroll.vertical   = true;

        return panelRT;
    }

    private static void StretchToParent(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot     = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = Vector2.zero;
    }

    private static GameObject CreateText(
        Transform parent, string name, string text,
        int fontSize, FontStyles style)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        return go;
    }

    private static GameObject CreateButton(
        Transform parent, string name, string label)
    {
        var go = new GameObject(
            name,
            typeof(RectTransform), typeof(CanvasRenderer),
            typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.25f, 0.9f);

        var labelGO = CreateText(go.transform, "Label", label, 28,
            FontStyles.Normal);
        var labelRT = labelGO.GetComponent<RectTransform>();
        StretchToParent(labelRT);
        return go;
    }

    private static LevelSelectCard EnsureCardPrefab()
    {
        var existing =
            AssetDatabase.LoadAssetAtPath<GameObject>(CardPrefabPath);
        if (existing != null)
        {
            var c = existing.GetComponent<LevelSelectCard>();
            if (c != null) return c;
            Debug.LogWarning(
                "[Scaffold] " + CardPrefabPath
                + " exists but has no LevelSelectCard component. "
                + "Add one manually or delete the prefab and re-run.");
            return null;
        }

        var dir = System.IO.Path.GetDirectoryName(CardPrefabPath);
        if (!AssetDatabase.IsValidFolder(dir))
        {
            Debug.LogError(
                "[Scaffold] Prefab folder missing: " + dir);
            return null;
        }

        var root = new GameObject(
            "LevelSelectCard",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image),
            typeof(Button));
        var rt = root.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(180f, 220f);
        var bg = root.GetComponent<Image>();
        bg.color = new Color(0.95f, 0.95f, 0.97f, 1f);

        var numberGO = CreateText(root.transform, "LevelNumber",
            "1", 64, FontStyles.Bold);
        var numRT = numberGO.GetComponent<RectTransform>();
        numRT.anchorMin = new Vector2(0.5f, 1f);
        numRT.anchorMax = new Vector2(0.5f, 1f);
        numRT.pivot     = new Vector2(0.5f, 1f);
        numRT.anchoredPosition = new Vector2(0f, -12f);
        numRT.sizeDelta = new Vector2(180f, 80f);
        numberGO.GetComponent<TextMeshProUGUI>().color = Color.black;

        var bestGO = CreateText(root.transform, "BestTime",
            "Best: --", 22, FontStyles.Normal);
        var bestRT = bestGO.GetComponent<RectTransform>();
        bestRT.anchorMin = new Vector2(0.5f, 0.5f);
        bestRT.anchorMax = new Vector2(0.5f, 0.5f);
        bestRT.pivot     = new Vector2(0.5f, 0.5f);
        bestRT.anchoredPosition = new Vector2(0f, -16f);
        bestRT.sizeDelta = new Vector2(170f, 30f);
        bestGO.GetComponent<TextMeshProUGUI>().color = Color.black;

        var rankGO = CreateText(root.transform, "BestRank",
            "Rank: --", 22, FontStyles.Bold);
        var rankRT = rankGO.GetComponent<RectTransform>();
        rankRT.anchorMin = new Vector2(0.5f, 0.5f);
        rankRT.anchorMax = new Vector2(0.5f, 0.5f);
        rankRT.pivot     = new Vector2(0.5f, 0.5f);
        rankRT.anchoredPosition = new Vector2(0f, -52f);
        rankRT.sizeDelta = new Vector2(170f, 30f);
        rankGO.GetComponent<TextMeshProUGUI>().color = Color.black;

        var lockedGO = new GameObject(
            "LockedOverlay",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        lockedGO.transform.SetParent(root.transform, false);
        var lockRT = lockedGO.GetComponent<RectTransform>();
        StretchToParent(lockRT);
        lockedGO.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.55f);
        var lockedTextGO = CreateText(lockedGO.transform, "LockedText",
            "LOCKED", 28, FontStyles.Bold);
        StretchToParent(lockedTextGO.GetComponent<RectTransform>());

        var card = root.AddComponent<LevelSelectCard>();
        var so = new SerializedObject(card);
        so.FindProperty("_button").objectReferenceValue =
            root.GetComponent<Button>();
        so.FindProperty("_levelNumberText").objectReferenceValue =
            numberGO.GetComponent<TextMeshProUGUI>();
        so.FindProperty("_bestTimeText").objectReferenceValue =
            bestGO.GetComponent<TextMeshProUGUI>();
        so.FindProperty("_bestRankText").objectReferenceValue =
            rankGO.GetComponent<TextMeshProUGUI>();
        so.FindProperty("_lockedOverlay").objectReferenceValue = lockedGO;
        so.FindProperty("_background").objectReferenceValue = bg;
        so.ApplyModifiedPropertiesWithoutUndo();

        var savedPrefab = PrefabUtility.SaveAsPrefabAsset(root, CardPrefabPath);
        Object.DestroyImmediate(root);
        Debug.Log("[Scaffold] Created " + CardPrefabPath);
        return savedPrefab != null
            ? savedPrefab.GetComponent<LevelSelectCard>()
            : null;
    }

    private static void WireController(
        LevelSelectController controller,
        RectTransform panelRoot,
        RectTransform cardContainer,
        Button backButton,
        LevelSelectCard cardPrefab,
        LevelCatalog catalog)
    {
        var so = new SerializedObject(controller);
        SetIfEmpty(so, "_panelRoot", panelRoot != null
            ? panelRoot.gameObject : null);
        SetIfEmpty(so, "_cardContainer", cardContainer);
        SetIfEmpty(so, "_backButton", backButton);
        SetIfEmpty(so, "_cardPrefab", cardPrefab);
        SetIfEmpty(so, "_catalog", catalog);
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(controller);
    }

    // Returns true if MainMenuController._levelSelect was (or already is)
    // wired. The controller can live anywhere in the scene, not only
    // under the Canvas, so search the whole scene.
    private static bool WireMainMenuStart(LevelSelectController controller)
    {
        var menu = Object.FindFirstObjectByType<MainMenuController>(
            FindObjectsInactive.Include);
        if (menu == null)
        {
            Debug.LogWarning(
                "[Scaffold] No MainMenuController found in the scene. "
                + "Wire the Start button to LevelSelectController.Open() "
                + "manually.");
            return false;
        }
        var so = new SerializedObject(menu);
        SetIfEmpty(so, "_levelSelect", controller);
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(menu);
        return true;
    }

    private static void SetIfEmpty(
        SerializedObject so, string fieldName, Object value)
    {
        var prop = so.FindProperty(fieldName);
        if (prop == null) return;
        if (prop.objectReferenceValue == null && value != null)
            prop.objectReferenceValue = value;
    }
}
