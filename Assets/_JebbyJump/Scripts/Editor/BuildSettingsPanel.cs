using JebbyJump.Audio;
using JebbyJump.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Idempotent scaffold for the audio settings foundation. Handles both
// scenes in one run:
//   MainMenu - adds a Settings button (cloned from Start/Level Select),
//              re-stacks Continue/LevelSelect/Settings/Quit, builds the
//              SettingsPanel with sliders/toggle/back/reset, wires
//              SettingsPanelController and an AudioSettingsApplier.
//   Game     - adds an AudioSettingsApplier to the Canvas and auto-wires
//              its _sfxSource to AudioFeedbackController._audioSource.
// Re-runs only refresh wiring/positions; nothing is duplicated.
public static class BuildSettingsPanel
{
    private const string MainMenuScenePath =
        "Assets/_JebbyJump/Scenes/MainMenu.unity";
    private const string GameScenePath =
        "Assets/_JebbyJump/Scenes/Game.unity";

    private const float ContinueY = 180f;
    private const float StartY    =  80f;
    private const float SettingsY = -20f;
    private const float QuitY     = -120f;

    // Pause panel 4-button stack (Resume / Restart / Settings / Main Menu).
    private const float PauseResumeY   =  120f;
    private const float PauseRestartY  =   20f;
    private const float PauseSettingsY =  -80f;
    private const float PauseMainMenuY = -180f;

    [MenuItem("Jebby Jump/Scaffold/Build Settings Panel")]
    public static void Run()
    {
        ScaffoldMainMenu();
        ScaffoldGame();
    }

    // ---- MainMenu scene -----------------------------------------------

    private static void ScaffoldMainMenu()
    {
        if (!System.IO.File.Exists(MainMenuScenePath))
        {
            Debug.LogError(
                "[Settings] MainMenu scene not found at " + MainMenuScenePath);
            return;
        }
        OpenScene(MainMenuScenePath);

        var menu = Object.FindFirstObjectByType<MainMenuController>(
            FindObjectsInactive.Include);
        if (menu == null)
        {
            Debug.LogError(
                "[Settings] No MainMenuController in MainMenu. Abort.");
            return;
        }

        var menuSo = new SerializedObject(menu);
        var startButton = menuSo.FindProperty("_startButton")
            .objectReferenceValue as Button;
        var continueButton = menuSo.FindProperty("_continueButton")
            .objectReferenceValue as Button;
        var quitButton = menuSo.FindProperty("_quitButton")
            .objectReferenceValue as Button;
        if (startButton == null)
        {
            Debug.LogError(
                "[Settings] MainMenuController._startButton not wired; "
                + "need it as the clone template. Abort.");
            return;
        }

        Transform parent = startButton.transform.parent;
        float x = startButton.GetComponent<RectTransform>()
            .anchoredPosition.x;

        Button settingsButton = EnsureSettingsButton(parent, startButton);

        // Restack the four buttons consistently every run.
        SetY(continueButton, x, ContinueY);
        SetY(startButton, x, StartY);
        SetY(settingsButton, x, SettingsY);
        SetY(quitButton, x, QuitY);

        // Place Settings between Level Select and Quit in sibling order.
        settingsButton.transform.SetSiblingIndex(
            startButton.transform.GetSiblingIndex() + 1);

        // Canvas root for the panel + applier.
        Canvas canvas = FindMainCanvas();
        if (canvas == null)
        {
            Debug.LogError("[Settings] No Canvas in MainMenu. Abort.");
            return;
        }

        SettingsPanelController panelController =
            EnsureSettingsPanel(canvas);
        AudioSettingsApplier applier = EnsureApplier(canvas);

        WireApplierIntoPanel(panelController, applier);

        // Wire MainMenuController refs (overwrite Settings refs - they are
        // canonical from this scaffold; leave others alone if already set).
        menuSo.FindProperty("_settingsButton").objectReferenceValue =
            settingsButton;
        menuSo.FindProperty("_settingsPanel").objectReferenceValue =
            panelController;
        menuSo.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(menu);

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());

        Debug.Log(
            "[Settings] MainMenu scaffolded: SettingsButton, SettingsPanel "
            + "(MusicSlider, SfxSlider, MuteToggle, Back, Reset), "
            + "AudioSettingsApplier. Wiring complete.");
    }

    private static Button EnsureSettingsButton(
        Transform parent, Button template)
    {
        var existing = parent.Find("SettingsButton");
        if (existing != null)
        {
            SetLabel(existing.GetComponent<Button>(), "Settings");
            return existing.GetComponent<Button>();
        }
        var clone = Object.Instantiate(template.gameObject, parent);
        clone.name = "SettingsButton";
        SetLabel(clone.GetComponent<Button>(), "Settings");
        Debug.Log("[Settings] Created SettingsButton (cloned).");
        return clone.GetComponent<Button>();
    }

    private static SettingsPanelController EnsureSettingsPanel(Canvas canvas)
    {
        // Reuse any existing panel anywhere in the active scene. A per-
        // canvas Find() is not enough: a scene may have several root
        // canvases, and re-runs must never create a second panel.
        var existingCtrl =
            Object.FindFirstObjectByType<SettingsPanelController>(
                FindObjectsInactive.Include);
        if (existingCtrl != null)
        {
            // P20: ensure the Reduce Motion toggle exists on already-scaffolded
            // panels (both MainMenu and the Game/pause panel) on re-run.
            EnsureReduceMotionToggle(existingCtrl.gameObject);
            // Rewire children into the controller in case they were
            // recreated by hand.
            WirePanelChildren(existingCtrl);
            existingCtrl.gameObject.SetActive(false);
            return existingCtrl;
        }

        var panelGO = new GameObject(
            "SettingsPanel",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panelGO.transform.SetParent(canvas.transform, false);
        StretchToParent(panelGO.GetComponent<RectTransform>());
        panelGO.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.85f);

        var title = CreateText(
            panelGO.transform, "Title", "Settings", 60, FontStyles.Bold);
        SetRect(title, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(0f, 240f),
            new Vector2(600f, 80f));

        CreateLabeledSlider(panelGO.transform, "MusicSlider", "Music", 120f);
        CreateLabeledSlider(panelGO.transform, "SfxSlider", "SFX", 30f);
        CreateLabeledToggle(panelGO.transform, "MuteToggle", "Mute", -60f);
        EnsureReduceMotionToggle(panelGO); // P20 accessibility (at -120)

        var back = CreateButton(panelGO.transform, "BackButton", "Back");
        SetRect(back, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(-110f, -210f),
            new Vector2(180f, 72f));

        var reset = CreateButton(panelGO.transform, "ResetButton", "Reset");
        SetRect(reset, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(110f, -210f),
            new Vector2(180f, 72f));

        var controller =
            panelGO.AddComponent<SettingsPanelController>();
        WirePanelChildren(controller);

        panelGO.SetActive(false);
        Debug.Log("[Settings] Created SettingsPanel.");
        return controller;
    }

    private static void WirePanelChildren(SettingsPanelController ctrl)
    {
        var panel = ctrl.gameObject;
        var so = new SerializedObject(ctrl);
        Set(so, "_panelRoot", panel);
        Set(so, "_musicSlider",
            panel.transform.Find("MusicSlider/Slider")?.GetComponent<Slider>());
        Set(so, "_sfxSlider",
            panel.transform.Find("SfxSlider/Slider")?.GetComponent<Slider>());
        Set(so, "_muteToggle",
            panel.transform.Find("MuteToggle/Toggle")?.GetComponent<Toggle>());
        Set(so, "_reduceMotionToggle",
            panel.transform.Find("ReduceMotionToggle/Toggle")?.GetComponent<Toggle>());
        Set(so, "_backButton",
            panel.transform.Find("BackButton")?.GetComponent<Button>());
        Set(so, "_resetButton",
            panel.transform.Find("ResetButton")?.GetComponent<Button>());
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(ctrl);
    }

    private static AudioSettingsApplier EnsureApplier(Canvas canvas)
    {
        var existing = canvas.GetComponent<AudioSettingsApplier>();
        if (existing != null) return existing;
        var added = canvas.gameObject.AddComponent<AudioSettingsApplier>();
        EditorUtility.SetDirty(canvas.gameObject);
        Debug.Log("[Settings] Added AudioSettingsApplier to Canvas.");
        return added;
    }

    private static void WireApplierIntoPanel(
        SettingsPanelController ctrl, AudioSettingsApplier applier)
    {
        var so = new SerializedObject(ctrl);
        var prop = so.FindProperty("_applier");
        if (prop.objectReferenceValue == null && applier != null)
            prop.objectReferenceValue = applier;
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(ctrl);
    }

    // ---- Game scene ---------------------------------------------------

    private static void ScaffoldGame()
    {
        if (!System.IO.File.Exists(GameScenePath))
        {
            Debug.LogError(
                "[Settings] Game scene not found at " + GameScenePath);
            return;
        }
        OpenScene(GameScenePath);

        // The Game scene has several root canvases (HUD, MobileControls,
        // Sequence). Anchor new Settings objects to the SAME canvas the
        // PausePanel lives on, so Pause -> Settings layering is coherent
        // and placement is deterministic across re-runs (driven by an
        // existing wired ref, not arbitrary canvas iteration order).
        Canvas canvas = FindPauseCanvas() ?? FindMainCanvas();
        if (canvas == null)
        {
            Debug.LogError("[Settings] No Canvas in Game. Abort.");
            return;
        }

        // Reuse an existing applier anywhere in the scene (the P5D one may
        // live on a different canvas); only add if none exists at all.
        var applier = Object.FindFirstObjectByType<AudioSettingsApplier>(
            FindObjectsInactive.Include);
        bool created = false;
        if (applier == null)
        {
            applier = canvas.gameObject.AddComponent<AudioSettingsApplier>();
            created = true;
        }

        // Auto-wire _sfxSource to AudioFeedbackController._audioSource.
        var feedback = Object.FindFirstObjectByType<AudioFeedbackController>(
            FindObjectsInactive.Include);
        AudioSource sfxSource = null;
        if (feedback != null)
        {
            var fbSo = new SerializedObject(feedback);
            sfxSource = fbSo.FindProperty("_audioSource")
                .objectReferenceValue as AudioSource;
        }
        if (sfxSource == null)
        {
            Debug.LogWarning(
                "[Settings] Could not find AudioFeedbackController._audioSource "
                + "in Game; wire AudioSettingsApplier._sfxSource manually.");
        }

        var so = new SerializedObject(applier);
        var sfxProp = so.FindProperty("_sfxSource");
        if (sfxProp.objectReferenceValue == null && sfxSource != null)
            sfxProp.objectReferenceValue = sfxSource;
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(applier);

        // Build a Game-scene SettingsPanel (mirrors MainMenu's) so Pause
        // can route into the same controller pattern. Reuses
        // EnsureSettingsPanel + WirePanelChildren from the MainMenu path.
        SettingsPanelController gameSettingsPanel =
            EnsureSettingsPanel(canvas);
        WireApplierIntoPanel(gameSettingsPanel, applier);

        bool pauseWired = WirePauseSettings(canvas, gameSettingsPanel);

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());

        Debug.Log(
            "[Settings] Game scaffolded: AudioSettingsApplier "
            + (created ? "added" : "reused")
            + "; _sfxSource wired: " + (sfxSource != null)
            + "; SettingsPanel ready; PauseSettings wired: " + pauseWired);
    }

    // Add Settings button to PausePanel, restack the 4 buttons, and wire
    // PauseMenuController._settingsButton + _settingsPanel. Idempotent:
    // reuses existing SettingsButton if present. Returns true on success.
    private static bool WirePauseSettings(
        Canvas canvas, SettingsPanelController settingsPanel)
    {
        var pauseCtrl = Object.FindFirstObjectByType<PauseMenuController>(
            FindObjectsInactive.Include);
        if (pauseCtrl == null)
        {
            Debug.LogWarning(
                "[Settings] No PauseMenuController in Game; skipping "
                + "Pause Settings wiring.");
            return false;
        }
        var ctrlSo = new SerializedObject(pauseCtrl);
        var pausePanelGO = ctrlSo.FindProperty("_pausePanel")
            .objectReferenceValue as GameObject;
        var resumeBtn = ctrlSo.FindProperty("_resumeButton")
            .objectReferenceValue as Button;
        var restartBtn = ctrlSo.FindProperty("_restartButton")
            .objectReferenceValue as Button;
        var mainMenuBtn = ctrlSo.FindProperty("_mainMenuButton")
            .objectReferenceValue as Button;
        if (pausePanelGO == null || resumeBtn == null
            || restartBtn == null || mainMenuBtn == null)
        {
            Debug.LogWarning(
                "[Settings] PauseMenuController is missing core refs "
                + "(_pausePanel/_resumeButton/_restartButton/_mainMenuButton); "
                + "run Build Pause Menu first.");
            return false;
        }
        Transform pausePanel = pausePanelGO.transform;
        float x = resumeBtn.GetComponent<RectTransform>()
            .anchoredPosition.x;

        // Idempotent SettingsButton (cloned from Resume so style matches).
        Button settingsBtn;
        var existing = pausePanel.Find("SettingsButton");
        if (existing != null)
        {
            settingsBtn = existing.GetComponent<Button>();
            SetLabel(settingsBtn, "Settings");
        }
        else
        {
            var clone = Object.Instantiate(
                resumeBtn.gameObject, pausePanel);
            clone.name = "SettingsButton";
            settingsBtn = clone.GetComponent<Button>();
            SetLabel(settingsBtn, "Settings");
            Debug.Log(
                "[Settings] Created Pause SettingsButton (cloned Resume).");
        }

        // Restack the four buttons consistently every run.
        SetY(resumeBtn, x, PauseResumeY);
        SetY(restartBtn, x, PauseRestartY);
        SetY(settingsBtn, x, PauseSettingsY);
        SetY(mainMenuBtn, x, PauseMainMenuY);

        // Sibling order: place Settings between Restart and Main Menu.
        settingsBtn.transform.SetSiblingIndex(
            mainMenuBtn.transform.GetSiblingIndex());

        ctrlSo.FindProperty("_settingsButton").objectReferenceValue =
            settingsBtn;
        ctrlSo.FindProperty("_settingsPanel").objectReferenceValue =
            settingsPanel;
        ctrlSo.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(pauseCtrl);
        return true;
    }

    // ---- helpers ------------------------------------------------------

    private static void OpenScene(string path)
    {
        var active = SceneManager.GetActiveScene();
        if (active.path != path)
        {
            if (active.isDirty)
                EditorSceneManager.SaveScene(active);
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

    // The root canvas hosting the PausePanel (via PauseMenuController).
    // Deterministic across re-runs: derived from an existing wired ref
    // rather than canvas iteration order. Null if pause UI is absent.
    private static Canvas FindPauseCanvas()
    {
        var pauseCtrl = Object.FindFirstObjectByType<PauseMenuController>(
            FindObjectsInactive.Include);
        if (pauseCtrl == null) return null;
        var panel = new SerializedObject(pauseCtrl)
            .FindProperty("_pausePanel").objectReferenceValue as GameObject;
        if (panel == null) return null;
        var c = panel.GetComponentInParent<Canvas>();
        return c != null ? c.rootCanvas : null;
    }

    private static void Set(SerializedObject so, string field, Object value)
    {
        var prop = so.FindProperty(field);
        if (prop == null) return;
        prop.objectReferenceValue = value;
    }

    private static void SetLabel(Button button, string text)
    {
        if (button == null) return;
        var label = button.GetComponentInChildren<TextMeshProUGUI>(true);
        if (label != null)
        {
            label.text = text;
            EditorUtility.SetDirty(label);
        }
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
        Vector2 anchoredPos, Vector2 size)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = aMin;
        rt.anchorMax = aMax;
        rt.pivot = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
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
        var labelGO = CreateText(go.transform, "Label", label, 28,
            FontStyles.Normal);
        StretchToParent(labelGO.GetComponent<RectTransform>());
        return go;
    }

    private static void CreateLabeledSlider(
        Transform parent, string name, string labelText, float y)
    {
        var rowGO = new GameObject(name, typeof(RectTransform));
        rowGO.transform.SetParent(parent, false);
        SetRect(rowGO, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(0f, y),
            new Vector2(700f, 60f));

        var labelGO = CreateText(rowGO.transform, "Label", labelText, 32,
            FontStyles.Normal);
        SetRect(labelGO, new Vector2(0f, 0f), new Vector2(0f, 1f),
            new Vector2(0f, 0.5f), new Vector2(0f, 0f),
            new Vector2(220f, 0f));
        labelGO.GetComponent<TextMeshProUGUI>().alignment =
            TextAlignmentOptions.Left;

        var sliderGO = DefaultControls.CreateSlider(
            new DefaultControls.Resources());
        sliderGO.name = "Slider";
        sliderGO.transform.SetParent(rowGO.transform, false);
        SetRect(sliderGO, new Vector2(0f, 0.5f), new Vector2(1f, 0.5f),
            new Vector2(0f, 0.5f), new Vector2(230f, 0f),
            new Vector2(-230f, 24f));
        var slider = sliderGO.GetComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;
        slider.value = 1f;
    }

    private static void CreateLabeledToggle(
        Transform parent, string name, string labelText, float y)
    {
        var rowGO = new GameObject(name, typeof(RectTransform));
        rowGO.transform.SetParent(parent, false);
        SetRect(rowGO, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(0f, y),
            new Vector2(700f, 60f));

        var labelGO = CreateText(rowGO.transform, "Label", labelText, 32,
            FontStyles.Normal);
        SetRect(labelGO, new Vector2(0f, 0f), new Vector2(0f, 1f),
            new Vector2(0f, 0.5f), new Vector2(0f, 0f),
            new Vector2(220f, 0f));
        labelGO.GetComponent<TextMeshProUGUI>().alignment =
            TextAlignmentOptions.Left;

        var toggleGO = DefaultControls.CreateToggle(
            new DefaultControls.Resources());
        toggleGO.name = "Toggle";
        toggleGO.transform.SetParent(rowGO.transform, false);
        SetRect(toggleGO, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f),
            new Vector2(0f, 0.5f), new Vector2(240f, 0f),
            new Vector2(60f, 60f));
    }

    // P20: idempotently add a "Reduce Motion" toggle and nudge Back/Reset down
    // so they do not overlap it. Safe on fresh AND already-scaffolded panels
    // (both MainMenu and the Game/pause SettingsPanel reuse this builder).
    private static void EnsureReduceMotionToggle(GameObject panel)
    {
        if (panel.transform.Find("ReduceMotionToggle") == null)
            CreateLabeledToggle(
                panel.transform, "ReduceMotionToggle", "Reduce Motion", -120f);

        MoveTo(panel, "BackButton", new Vector2(-110f, -210f));
        MoveTo(panel, "ResetButton", new Vector2(110f, -210f));
    }

    private static void MoveTo(GameObject panel, string name, Vector2 pos)
    {
        var t = panel.transform.Find(name);
        if (t == null) return;
        var rt = t.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        EditorUtility.SetDirty(rt);
    }
}
