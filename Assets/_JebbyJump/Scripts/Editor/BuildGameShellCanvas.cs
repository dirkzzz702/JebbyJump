using System.Collections.Generic;
using JebbyJump.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// P21: isolates the Game-scene SHELL panels from the gameplay canvases.
// Verified (correction #1): the result/game-over panels live on HUDCanvas and
// the pause/settings panels on MobileControlsCanvas - both shared with gameplay
// (timer/lives, mobile controls). The 800x600 SequenceCanvas is the memory
// gameplay display and is left untouched. This scaffold creates a dedicated
// GameShellCanvas (1920x1080 Scale-With-Screen-Size, sorted above gameplay) and
// moves the shell panels onto it, then makes the centered result/game-over
// cards proper modals (full-screen raycast backdrop + centered Card). Pause /
// Settings safe-area is added by their own scaffolds. Idempotent.
public static class BuildGameShellCanvas
{
    private const string GameScenePath =
        "Assets/_JebbyJump/Scenes/Game.unity";

    [MenuItem("Jebby Jump/Scaffold/Build Game Shell Canvas")]
    public static void Run()
    {
        if (!System.IO.File.Exists(GameScenePath))
        {
            Debug.LogError("[Shell] Game scene not found at " + GameScenePath);
            return;
        }
        EnsureSceneOpen();

        Canvas shell = EnsureGameShellCanvas();

        var pause = Object.FindFirstObjectByType<PauseMenuController>(
            FindObjectsInactive.Include);
        var settings = Object.FindFirstObjectByType<SettingsPanelController>(
            FindObjectsInactive.Include);
        var hud = Object.FindFirstObjectByType<HUDController>(
            FindObjectsInactive.Include);

        if (pause != null) MoveTo(pause.gameObject, shell);
        if (settings != null) MoveTo(settings.gameObject, shell);

        GameObject levelComplete = null, gameOver = null;
        if (hud != null)
        {
            var so = new SerializedObject(hud);
            levelComplete = so.FindProperty("_levelCompletePanel")
                .objectReferenceValue as GameObject;
            gameOver = so.FindProperty("_gameOverPanel")
                .objectReferenceValue as GameObject;
        }
        if (levelComplete != null) MoveTo(levelComplete, shell);
        if (gameOver != null) MoveTo(gameOver, shell);

        // Centered cards -> proper modals (backdrop + Card) + >=90 buttons.
        if (levelComplete != null)
            MakeModalCard(levelComplete, hud, new[]
            {
                "_levelCompleteNextButton", "_levelCompleteRetryButton",
                "_levelCompleteMenuButton",
            });
        if (gameOver != null)
            MakeModalCard(gameOver, hud, new[]
            {
                "_gameOverRetryButton", "_gameOverMenuButton",
            });

        // Keep panels hidden by default (controllers toggle them).
        if (levelComplete != null) levelComplete.SetActive(false);
        if (gameOver != null) gameOver.SetActive(false);

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        Debug.Log("[Shell] GameShellCanvas ready; shell panels moved off the "
            + "gameplay canvases; result/game-over cards made modal.");
    }

    private static void EnsureSceneOpen()
    {
        var active = SceneManager.GetActiveScene();
        if (active.path != GameScenePath)
        {
            if (active.isDirty) EditorSceneManager.SaveScene(active);
            EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
        }
    }

    private static Canvas EnsureGameShellCanvas()
    {
        var all = Object.FindObjectsByType<Canvas>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var c in all)
            if (c.gameObject.name == "GameShellCanvas") return c;

        var go = new GameObject("GameShellCanvas",
            typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 500; // above gameplay HUD / mobile / sequence
        ShellScaffold.EnsureStandardScaler(canvas);
        Debug.Log("[Shell] Created GameShellCanvas (1920x1080 SWSS, sort 500).");
        return canvas;
    }

    private static void MoveTo(GameObject go, Canvas shell)
    {
        if (go != null && go.transform.parent != shell.transform)
            go.transform.SetParent(shell.transform, false);
    }

    // Turns a centered card panel into a full-screen modal: the panel becomes a
    // raycast-blocking dim backdrop and its content moves into a centered Card
    // (kept within safe bounds by being centered + small). Idempotent.
    private static void MakeModalCard(
        GameObject panel, HUDController hud, string[] buttonFields)
    {
        var prt = panel.GetComponent<RectTransform>();
        if (panel.transform.Find("Card") == null)
        {
            var card = new GameObject("Card",
                typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            card.transform.SetParent(panel.transform, false);
            var crt = card.GetComponent<RectTransform>();
            crt.anchorMin = crt.anchorMax = new Vector2(0.5f, 0.5f);
            crt.pivot = new Vector2(0.5f, 0.5f);
            crt.anchoredPosition = Vector2.zero;
            crt.sizeDelta = prt.sizeDelta.sqrMagnitude > 1f
                ? prt.sizeDelta : new Vector2(700f, 400f);
            card.GetComponent<Image>().color =
                new Color(0.15f, 0.15f, 0.2f, 0.98f);

            var toMove = new List<Transform>();
            foreach (Transform ch in panel.transform)
                if (ch.name != "Card") toMove.Add(ch);
            foreach (var ch in toMove) ch.SetParent(card.transform, false);
        }

        // Panel = full-screen dim raycast blocker (pointer trap).
        ShellScaffold.Stretch(prt);
        var img = panel.GetComponent<Image>() ?? panel.AddComponent<Image>();
        img.color = new Color(0f, 0f, 0f, 0.85f);
        img.raycastTarget = true;

        if (hud != null)
        {
            var so = new SerializedObject(hud);
            foreach (var f in buttonFields)
            {
                var b = so.FindProperty(f).objectReferenceValue as Button;
                if (b != null) ShellScaffold.EnsureMinHeight(b.gameObject);
            }
        }
    }
}
