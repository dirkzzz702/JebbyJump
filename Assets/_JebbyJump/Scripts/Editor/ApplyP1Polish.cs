using JebbyJump.Level;
using JebbyJump.Sequence;
using JebbyJump.UI;
using TMPro;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// One-shot polish pass:
//  • Create DefaultTimeRankConfig.asset if missing.
//  • Find / create LevelTimer GameObject and wire references.
//  • Fix result-panel TMP fonts.
//  • Build LevelCompletePanel time/best/rank TMP children.
//  • Wire HUDController + MemoryPhaseController fields.
//  • Reposition TutorialHint to top-center below HUD.
//  • Disable LivesText (LivesIconContainer is canonical).
//  • Move Jebby initial transform.y so capsule bottom = floor surface.
//  • Animator: shorten Land state exit time so Land→Idle is snappy.
public static class ApplyP1Polish
{
    private const string SceneAsset = "Assets/_JebbyJump/Scenes/Game.unity";
    private const string DefaultRankPath = "Assets/_JebbyJump/Settings/Level/DefaultTimeRankConfig.asset";
    private const string FontPath = "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset";
    private const string AnimatorPath = "Assets/_JebbyJump/Art/Animations/JebbyAnimator.controller";

    public static void Execute()
    {
        EnsureDefaultRankConfig();
        TweakLandExitTime();

        var scene = EditorSceneManager.OpenScene(SceneAsset, OpenSceneMode.Single);

        EnsureLevelTimer(out var levelTimer);
        EnsureTMPFontOnResultPanels(out var sdfFont);
        BuildLevelCompleteTimeChildren(sdfFont, out var timeText, out var bestText, out var rankText);
        WireHUDController(levelTimer, timeText, bestText, rankText);
        WireMemoryPhaseController(levelTimer);
        RepositionTutorialHint();
        DisableLegacyLivesText();
        MoveJebbyToFloorSurface();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[P1Polish] Done.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    private static void EnsureDefaultRankConfig()
    {
        if (AssetDatabase.LoadAssetAtPath<TimeRankConfig>(DefaultRankPath) != null)
        {
            Debug.Log($"[P1Polish] DefaultTimeRankConfig already exists at {DefaultRankPath}");
            return;
        }
        var dir = "Assets/_JebbyJump/Settings/Level";
        if (!AssetDatabase.IsValidFolder(dir))
            AssetDatabase.CreateFolder("Assets/_JebbyJump/Settings", "Level");

        var asset = ScriptableObject.CreateInstance<TimeRankConfig>();
        AssetDatabase.CreateAsset(asset, DefaultRankPath);
        Debug.Log($"[P1Polish] Created {DefaultRankPath}");
    }

    private static void TweakLandExitTime()
    {
        var ctrl = AssetDatabase.LoadAssetAtPath<AnimatorController>(AnimatorPath);
        if (ctrl == null) { Debug.LogWarning($"[P1Polish] Animator not found: {AnimatorPath}"); return; }

        foreach (var layer in ctrl.layers)
        {
            foreach (var state in layer.stateMachine.states)
            {
                if (state.state.name != "Land") continue;
                foreach (var t in state.state.transitions)
                {
                    if (t.hasExitTime && t.exitTime > 0.3f)
                    {
                        Debug.Log($"[P1Polish] Land transition exitTime {t.exitTime:F2} → 0.25");
                        t.exitTime = 0.25f;
                    }
                }
            }
        }
        EditorUtility.SetDirty(ctrl);
    }

    // ─────────────────────────────────────────────────────────────────────────
    private static void EnsureLevelTimer(out LevelTimer timer)
    {
        timer = Object.FindFirstObjectByType<LevelTimer>(FindObjectsInactive.Include);
        if (timer != null) return;

        var go = new GameObject("LevelTimer");
        timer = go.AddComponent<LevelTimer>();
        Debug.Log("[P1Polish] Created LevelTimer GameObject.");
    }

    private static void EnsureTMPFontOnResultPanels(out TMP_FontAsset font)
    {
        font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        if (font == null)
        {
            // Fall back to whatever font LevelText already uses (already known good).
            var any = Object.FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var t in any)
            {
                if (t != null && t.font != null && t.name == "LevelText") { font = t.font; break; }
            }
        }
        if (font == null) { Debug.LogWarning("[P1Polish] No TMP font found to assign to result panels."); return; }

        int fixedCount = 0;
        var allText = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
        foreach (var t in allText)
        {
            if (t == null || t.gameObject.scene.name == null) continue;          // skip asset previews
            var go = t.transform;
            // Only target text under GameOverPanel / LevelCompletePanel
            bool underPanel = false;
            for (var p = go; p != null; p = p.parent)
            {
                if (p.name == "GameOverPanel" || p.name == "LevelCompletePanel") { underPanel = true; break; }
            }
            if (!underPanel) continue;
            if (t.font == null)
            {
                t.font = font;
                EditorUtility.SetDirty(t);
                fixedCount++;
                Debug.Log($"[P1Polish] Assigned font to TMP '{t.name}' under {t.transform.parent?.name}");
            }
        }
        Debug.Log($"[P1Polish] Fixed {fixedCount} TMP font(s) on result panels.");
    }

    private static void BuildLevelCompleteTimeChildren(TMP_FontAsset font,
        out TextMeshProUGUI timeText, out TextMeshProUGUI bestText, out TextMeshProUGUI rankText)
    {
        timeText = null; bestText = null; rankText = null;

        var panel = FindGameObjectIncludingInactive("LevelCompletePanel");
        if (panel == null) { Debug.LogWarning("[P1Polish] LevelCompletePanel not found."); return; }

        timeText = EnsureChildText(panel.transform, "TimeText",     "Time: --",    new Vector2(0f,  -40f), 40, font);
        bestText = EnsureChildText(panel.transform, "BestTimeText", "Best: --",    new Vector2(0f,  -90f), 32, font);
        rankText = EnsureChildText(panel.transform, "RankText",     "Rank: -",     new Vector2(0f, -140f), 44, font);

        Debug.Log("[P1Polish] LevelCompletePanel children: TimeText, BestTimeText, RankText (built or reused).");
    }

    private static TextMeshProUGUI EnsureChildText(Transform parent, string name, string defaultText,
        Vector2 anchoredPos, float fontSize, TMP_FontAsset font)
    {
        var existing = parent.Find(name);
        GameObject go;
        if (existing != null) go = existing.gameObject;
        else
        {
            go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            go.transform.SetParent(parent, false);
        }

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot     = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(500f, 60f);
        rt.anchoredPosition = anchoredPos;

        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text       = defaultText;
        tmp.fontSize   = fontSize;
        tmp.alignment  = TextAlignmentOptions.Center;
        tmp.color      = Color.white;
        if (tmp.font == null && font != null) tmp.font = font;
        return tmp;
    }

    // ─────────────────────────────────────────────────────────────────────────
    private static void WireHUDController(LevelTimer timer, TextMeshProUGUI timeT, TextMeshProUGUI bestT, TextMeshProUGUI rankT)
    {
        var hud = Object.FindFirstObjectByType<HUDController>(FindObjectsInactive.Include);
        if (hud == null) { Debug.LogWarning("[P1Polish] HUDController not found."); return; }

        var so = new SerializedObject(hud);
        SetIfNull(so, "_levelTimer", timer);
        SetIfNull(so, "_levelCompleteTimeText", timeT);
        SetIfNull(so, "_levelCompleteBestTimeText", bestT);
        SetIfNull(so, "_levelCompleteRankText", rankT);

        var rank = AssetDatabase.LoadAssetAtPath<TimeRankConfig>(DefaultRankPath);
        SetIfNull(so, "_rankConfig", rank);

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(hud);
        Debug.Log("[P1Polish] HUDController wired.");
    }

    private static void WireMemoryPhaseController(LevelTimer timer)
    {
        var mpc = Object.FindFirstObjectByType<MemoryPhaseController>(FindObjectsInactive.Include);
        if (mpc == null) { Debug.LogWarning("[P1Polish] MemoryPhaseController not found."); return; }

        var so = new SerializedObject(mpc);
        SetIfNull(so, "_levelTimer", timer);
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(mpc);
        Debug.Log("[P1Polish] MemoryPhaseController._levelTimer wired.");
    }

    private static void SetIfNull(SerializedObject so, string field, Object value)
    {
        var prop = so.FindProperty(field);
        if (prop == null) { Debug.LogWarning($"[P1Polish] Field '{field}' not found"); return; }
        if (prop.objectReferenceValue == null && value != null)
        {
            prop.objectReferenceValue = value;
            Debug.Log($"[P1Polish]   {field} ← {value.name}");
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    private static void RepositionTutorialHint()
    {
        // The tutorial root has no fixed name — get it through the controller reference.
        var ctrl = Object.FindFirstObjectByType<JebbyJump.UI.TutorialHintController>(FindObjectsInactive.Include);
        if (ctrl == null) { Debug.LogWarning("[P1Polish] TutorialHintController not found."); return; }

        var so = new SerializedObject(ctrl);
        var prop = so.FindProperty("_hintRoot");
        var hintRoot = prop?.objectReferenceValue as GameObject;
        if (hintRoot == null) { Debug.LogWarning("[P1Polish] TutorialHintController._hintRoot not assigned."); return; }

        var rt = hintRoot.GetComponent<RectTransform>();
        if (rt == null) { Debug.LogWarning("[P1Polish] Tutorial hint root has no RectTransform."); return; }

        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot     = new Vector2(0.5f, 1f);
        rt.sizeDelta = new Vector2(800f, 100f);
        rt.anchoredPosition = new Vector2(0f, -120f); // below the HUD header line
        EditorUtility.SetDirty(hintRoot);
        Debug.Log($"[P1Polish] '{hintRoot.name}' anchored top-center, y = -120.");
    }

    private static void DisableLegacyLivesText()
    {
        // LivesIconContainer is the canonical display; HUDController falls back to LivesText
        // only when the container is null. Hide LivesText to avoid duplicate visuals.
        var lt = FindGameObjectIncludingInactive("LivesText");
        if (lt == null) { Debug.Log("[P1Polish] LivesText not present (already removed)."); return; }
        if (lt.activeSelf)
        {
            lt.SetActive(false);
            EditorUtility.SetDirty(lt);
            Debug.Log("[P1Polish] LivesText disabled (LivesIconContainer is canonical).");
        }
    }

    private static void MoveJebbyToFloorSurface()
    {
        var jebby = FindGameObjectIncludingInactive("Jebby");
        var floor = FindGameObjectIncludingInactive("Floor");
        if (jebby == null || floor == null) { Debug.LogWarning("[P1Polish] Jebby or Floor not found."); return; }

        var fc = floor.GetComponent<BoxCollider2D>();
        if (fc == null) { Debug.LogWarning("[P1Polish] Floor has no BoxCollider2D."); return; }
        var cap = jebby.GetComponentInChildren<CapsuleCollider2D>(true);
        if (cap == null) { Debug.LogWarning("[P1Polish] Jebby has no CapsuleCollider2D."); return; }

        float floorTopY = floor.transform.position.y
                          + (fc.offset.y + fc.size.y * 0.5f) * floor.transform.localScale.y;
        // capsule bottom local offset from Jebby root: (offset.y - size.y/2) * scale.y
        float capBottomOffset = (cap.offset.y - cap.size.y * 0.5f) * jebby.transform.localScale.y;
        float targetY = floorTopY - capBottomOffset;
        var p = jebby.transform.position;
        if (Mathf.Abs(p.y - targetY) > 0.001f)
        {
            Debug.Log($"[P1Polish] Jebby y: {p.y:F3} → {targetY:F3}  (floorTop={floorTopY:F3}, capBottomOffset={capBottomOffset:F3})");
            p.y = targetY;
            jebby.transform.position = p;
            EditorUtility.SetDirty(jebby);
        }
        else
        {
            Debug.Log($"[P1Polish] Jebby y already at {p.y:F3} (target {targetY:F3}).");
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    private static GameObject FindGameObjectIncludingInactive(string name)
    {
        var all = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var go in all)
        {
            if (go.name != name) continue;
            if (!go.scene.IsValid()) continue; // skip prefab assets
            return go;
        }
        return null;
    }
}
