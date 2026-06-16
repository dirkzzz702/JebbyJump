using JebbyJump.Inputs;
using JebbyJump.Level;
using JebbyJump.Sequence;
using JebbyJump.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Idempotent scaffold: builds an in-game pause flow inside Game.unity.
// Creates a HUD Pause button and a hidden Pause panel (Resume / Restart /
// Main Menu), adds a PauseMenuController, and wires its references. Re-runs
// only refresh wiring; nothing is duplicated. Pause button placement is a
// reasonable default (top-right corner) - nudge it in-editor if it overlaps
// the live timer.
public static class BuildPauseMenu
{
    private const string GameScenePath =
        "Assets/_JebbyJump/Scenes/Game.unity";

    [MenuItem("Jebby Jump/Scaffold/Build Pause Menu")]
    public static void Run()
    {
        if (!System.IO.File.Exists(GameScenePath))
        {
            Debug.LogError("[Pause] Game scene not found at " + GameScenePath);
            return;
        }

        EnsureSceneOpen();

        Canvas canvas = FindMainCanvas();
        if (canvas == null)
        {
            Debug.LogError(
                "[Pause] No Canvas found in Game.unity. Abort.");
            return;
        }

        Button pauseButton = EnsurePauseButton(canvas);
        Transform pauseMenu = EnsurePauseMenuRoot(canvas);
        GameObject pausePanel = EnsurePausePanel(pauseMenu);
        // Deep search: P21's safe-area move relocates the buttons under a
        // SafeArea root, so direct-child paths would null out the refs.
        Button resume   = ShellScaffold.FindDeep(pausePanel.transform, "ResumeButton")?.GetComponent<Button>();
        Button restart  = ShellScaffold.FindDeep(pausePanel.transform, "RestartButton")?.GetComponent<Button>();
        Button mainMenu = ShellScaffold.FindDeep(pausePanel.transform, "MainMenuButton")?.GetComponent<Button>();

        var controller =
            pauseMenu.GetComponent<PauseMenuController>()
            ?? pauseMenu.gameObject.AddComponent<PauseMenuController>();

        WireController(
            controller, pauseButton, pausePanel, resume, restart, mainMenu);

        // P21: >=90 touch targets + safe-area content root (backdrop stays
        // edge-to-edge). Done before hiding the panel.
        ShellScaffold.EnsureMinHeight(pausePanel.transform, "ResumeButton");
        ShellScaffold.EnsureMinHeight(pausePanel.transform, "RestartButton");
        ShellScaffold.EnsureMinHeight(pausePanel.transform, "MainMenuButton");
        ShellScaffold.EnsureSafeAreaMoveAll(pausePanel);

        // Panel hidden by default; controller toggles it.
        pausePanel.SetActive(false);

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());

        Debug.Log(
            "[Pause] Pause flow scaffolded. Verify the Pause button "
            + "position does not overlap the live timer; nudge if needed.");
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

    private static Canvas FindMainCanvas()
    {
        var canvases = Object.FindObjectsByType<Canvas>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < canvases.Length; i++)
            if (canvases[i].isRootCanvas) return canvases[i];
        return canvases.Length > 0 ? canvases[0] : null;
    }

    private static Button EnsurePauseButton(Canvas canvas)
    {
        // Scene-wide search (robust to the P21 GameShellCanvas move).
        var all = Object.FindObjectsByType<Button>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var b in all)
            if (b.gameObject.name == "PauseButton")
            {
                Debug.Log("[Pause] PauseButton already present - reusing.");
                return b;
            }

        var go = CreateButton(canvas.transform, "PauseButton", "||");
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot     = new Vector2(1f, 1f);
        // Below the live timer (which sits at top-right ~36-96px from the
        // top) so the two no longer overlap. See P5F.
        rt.anchoredPosition = new Vector2(-24f, -116f);
        rt.sizeDelta = new Vector2(96f, 96f);
        Debug.Log("[Pause] Created PauseButton (top-right, below timer).");
        return go.GetComponent<Button>();
    }

    private static Transform EnsurePauseMenuRoot(Canvas canvas)
    {
        // Prefer the existing PauseMenu (found via its controller) wherever it
        // lives - P21 moves it onto a dedicated GameShellCanvas.
        var ctrl = Object.FindFirstObjectByType<PauseMenuController>(
            FindObjectsInactive.Include);
        if (ctrl != null) return ctrl.transform;

        var existing = canvas.transform.Find("PauseMenu");
        if (existing != null) return existing;

        var go = new GameObject("PauseMenu", typeof(RectTransform));
        go.transform.SetParent(canvas.transform, false);
        StretchToParent(go.GetComponent<RectTransform>());
        return go.transform;
    }

    private static GameObject EnsurePausePanel(Transform pauseMenu)
    {
        var existing = pauseMenu.Find("PausePanel");
        if (existing != null)
        {
            Debug.Log("[Pause] PausePanel already present - reusing.");
            return existing.gameObject;
        }

        var panelGO = new GameObject(
            "PausePanel",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panelGO.transform.SetParent(pauseMenu, false);
        StretchToParent(panelGO.GetComponent<RectTransform>());
        panelGO.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.85f);

        var title = CreateText(
            panelGO.transform, "Title", "Paused", 64, FontStyles.Bold);
        var titleRT = title.GetComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0.5f, 0.5f);
        titleRT.anchorMax = new Vector2(0.5f, 0.5f);
        titleRT.pivot     = new Vector2(0.5f, 0.5f);
        titleRT.anchoredPosition = new Vector2(0f, 200f);
        titleRT.sizeDelta = new Vector2(600f, 90f);

        CreateMenuButton(panelGO.transform, "ResumeButton", "Resume", 60f);
        CreateMenuButton(panelGO.transform, "RestartButton", "Restart Level", -40f);
        CreateMenuButton(panelGO.transform, "MainMenuButton", "Main Menu", -140f);

        Debug.Log("[Pause] Created PausePanel (Resume/Restart/Main Menu).");
        return panelGO;
    }

    private static void CreateMenuButton(
        Transform parent, string name, string label, float y)
    {
        var go = CreateButton(parent, name, label);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot     = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(0f, y);
        rt.sizeDelta = new Vector2(360f, 84f);
    }

    private static void WireController(
        PauseMenuController controller,
        Button pauseButton,
        GameObject pausePanel,
        Button resume,
        Button restart,
        Button mainMenu)
    {
        var so = new SerializedObject(controller);
        Set(so, "_pauseButton", pauseButton);
        Set(so, "_pausePanel", pausePanel);
        Set(so, "_resumeButton", resume);
        Set(so, "_restartButton", restart);
        Set(so, "_mainMenuButton", mainMenu);
        Set(so, "_phaseController",
            Object.FindFirstObjectByType<MemoryPhaseController>(
                FindObjectsInactive.Include));
        Set(so, "_progressTracker",
            Object.FindFirstObjectByType<LevelProgressTracker>(
                FindObjectsInactive.Include));

        var inputProp = so.FindProperty("_input");
        if (inputProp.objectReferenceValue == null)
        {
            var input = FindSingleInputReader();
            if (input != null) inputProp.objectReferenceValue = input;
            else Debug.LogWarning(
                "[Pause] No single InputReader asset found; wire "
                + "PauseMenuController._input by hand for keyboard pause.");
        }

        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(controller);

        Debug.Log(
            "[Pause] Wired PauseMenuController (pauseButton, panel, "
            + "resume, restart, mainMenu, phaseController, progressTracker, "
            + "input).");
    }

    private static void Set(SerializedObject so, string field, Object value)
    {
        var prop = so.FindProperty(field);
        if (prop != null) prop.objectReferenceValue = value;
    }

    private static InputReader FindSingleInputReader()
    {
        string[] guids = AssetDatabase.FindAssets("t:InputReader");
        if (guids == null || guids.Length != 1) return null;
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        return AssetDatabase.LoadAssetAtPath<InputReader>(path);
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
        go.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.25f, 0.95f);

        var labelGO = CreateText(go.transform, "Label", label, 32,
            FontStyles.Normal);
        StretchToParent(labelGO.GetComponent<RectTransform>());
        return go;
    }
}
