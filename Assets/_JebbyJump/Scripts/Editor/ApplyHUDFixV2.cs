using JebbyJump.Inputs;
using JebbyJump.Items;
using JebbyJump.Sequence;
using JebbyJump.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

// Surgical HUD fixes + 3 skill cluster wiring. Idempotent — safe to re-run.
public static class ApplyHUDFixV2
{
    private const string ScenePath = "Assets/_JebbyJump/Scenes/Game.unity";
    private const string BtnBgPath = "Assets/_JebbyJump/Art/Sprites/UI/ui_btn_bg.png";
    private const string RocketBootsIconPath = "Assets/_JebbyJump/Art/Sprites/Items/spr_item_rocket_boots_01.png";
    private const string LifeIconPath = "Assets/_JebbyJump/Art/Sprites/UI/ui_icon_life_01.png";
    private const string MemoryIconPath = "Assets/_JebbyJump/Art/Sprites/UI/ui_icon_memory_base_01.png";
    private const string InputReaderPath = "Assets/_JebbyJump/Settings/Input/InputReader.asset";

    // Floor lowered slightly more for better scene composition (top -0.9 → -1.2)
    private const float FloorTargetY = -1.45f;   // top = -1.45 + 0.5*0.5 = -1.2
    private const float JebbyTargetY = -1.33f;   // capsule bottom = -1.2

    public static void Execute()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("[HUDFixV2] Cannot run while in Play Mode — changes will not persist. Stop play and re-run.");
            return;
        }

        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        CleanLevelBadge();
        PadHearts();
        LowerFloorMore();
        CircularizeCooldownOnSkill1();
        WireRocketBootsIconOnSkill1();
        EnsureExtraSkillsObjects(out var bubbleShieldGO, out var colorEchoGO, out var bubbleSkillCtrl, out var echoSkillCtrl);
        BuildThreeSkillButtons();
        WireMemoryPhaseControllerRefs(bubbleShieldGO, bubbleSkillCtrl, echoSkillCtrl);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[HUDFixV2] Done.");
    }

    // ── LevelBadge: remove the light grey border ──────────────────────────────
    private static void CleanLevelBadge()
    {
        var badge = FindInActiveScene("LevelBadgeRoot");
        if (badge == null) { Debug.LogWarning("[HUDFixV2] LevelBadgeRoot not found."); return; }
        var img = badge.GetComponent<Image>();
        if (img != null)
        {
            // Plain dark translucent — no UISprite border.
            img.sprite = null;
            img.type   = Image.Type.Simple;
            img.color  = new Color(0f, 0f, 0f, 0.55f);
            EditorUtility.SetDirty(img);
        }
        var rt = badge.GetComponent<RectTransform>();
        if (rt != null) rt.sizeDelta = new Vector2(260f, 64f);
        Debug.Log("[HUDFixV2] LevelBadge cleaned (no border, dark translucent).");
    }

    // ── Hearts: pad more ─────────────────────────────────────────────────────
    private static void PadHearts()
    {
        var go = FindInActiveScene("LivesIconContainer");
        if (go == null) return;
        var rt = go.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = new Vector2(36f, -36f);
            rt.sizeDelta = new Vector2(280f, 60f);
            EditorUtility.SetDirty(go);
        }
        var hlg = go.GetComponent<HorizontalLayoutGroup>();
        if (hlg != null) { hlg.spacing = 10f; EditorUtility.SetDirty(hlg); }
        Debug.Log("[HUDFixV2] Hearts padded (36, -36), 280×60, spacing 10.");
    }

    // ── Floor: lower more ────────────────────────────────────────────────────
    private static void LowerFloorMore()
    {
        var floor = FindInActiveScene("Floor");
        var jebby = FindInActiveScene("Jebby");
        if (floor == null || jebby == null) return;

        var fp = floor.transform.position;
        float oldFY = fp.y;
        fp.y = FloorTargetY;
        floor.transform.position = fp;
        EditorUtility.SetDirty(floor);

        var jp = jebby.transform.position;
        float oldJY = jp.y;
        jp.y = JebbyTargetY;
        jebby.transform.position = jp;
        EditorUtility.SetDirty(jebby);

        Debug.Log($"[HUDFixV2] Floor y: {oldFY:F3} → {FloorTargetY:F3} (top -1.2). Jebby y: {oldJY:F3} → {JebbyTargetY:F3}.");
    }

    // ── Cooldown overlay: match the circular button BG so the mask is circular
    private static void CircularizeCooldownOnSkill1()
    {
        var skill = FindInActiveScene("Btn_Skill");
        if (skill == null) skill = FindInActiveScene("Btn_Skill1");
        if (skill == null) return;
        CircularizeCooldown(skill);
    }

    private static void CircularizeCooldown(GameObject skillBtn)
    {
        var bg = AssetDatabase.LoadAssetAtPath<Sprite>(BtnBgPath);
        if (bg == null) return;
        var overlay = skillBtn.transform.Find("CooldownOverlay");
        if (overlay == null) return;
        var img = overlay.GetComponent<Image>();
        if (img == null) return;
        img.sprite      = bg;       // same circular silhouette as the button
        img.type        = Image.Type.Filled;
        img.fillMethod  = Image.FillMethod.Radial360;
        img.fillOrigin  = (int)Image.Origin360.Top;
        img.fillClockwise = true;
        img.color       = new Color(0f, 0f, 0f, 0.6f);
        img.raycastTarget = false;
        EditorUtility.SetDirty(img);
        Debug.Log($"[HUDFixV2] {skillBtn.name}/CooldownOverlay → circular mask (ui_btn_bg, Filled Radial360).");
    }

    // ── Skill 1 icon (in case Btn_Skill not yet renamed) ─────────────────────
    private static void WireRocketBootsIconOnSkill1()
    {
        var skill = FindInActiveScene("Btn_Skill");
        if (skill == null) skill = FindInActiveScene("Btn_Skill1");
        if (skill == null) return;
        var rocketSprite = AssetDatabase.LoadAssetAtPath<Sprite>(RocketBootsIconPath);
        var icon = skill.transform.Find("RocketBootsIcon");
        if (icon == null) icon = skill.transform.Find("Icon");
        if (icon == null) return;
        var img = icon.GetComponent<Image>();
        if (img != null && rocketSprite != null) { img.sprite = rocketSprite; EditorUtility.SetDirty(img); }
    }

    // ── Spawn / ensure GameObjects holding BubbleShield + ColorEcho effects ──
    private static void EnsureExtraSkillsObjects(out GameObject bubbleShieldGO, out GameObject colorEchoGO,
        out ActiveSkillController bubbleSkillCtrl, out ActiveSkillController echoSkillCtrl)
    {
        bubbleShieldGO = FindInActiveScene("BubbleShieldEffect");
        if (bubbleShieldGO == null)
        {
            bubbleShieldGO = new GameObject("BubbleShieldEffect");
            bubbleShieldGO.AddComponent<BubbleShieldEffect>();
            Debug.Log("[HUDFixV2] Created BubbleShieldEffect GameObject.");
        }

        colorEchoGO = FindInActiveScene("ColorEchoEffect");
        if (colorEchoGO == null)
        {
            colorEchoGO = new GameObject("ColorEchoEffect");
            colorEchoGO.AddComponent<ColorEchoEffect>();
            Debug.Log("[HUDFixV2] Created ColorEchoEffect GameObject.");
        }

        // Wire BubbleShield._feedbackUI
        var feedback = Object.FindFirstObjectByType<GameFeedbackUI>(FindObjectsInactive.Include);
        if (feedback != null)
        {
            var so = new SerializedObject(bubbleShieldGO.GetComponent<BubbleShieldEffect>());
            so.FindProperty("_feedbackUI").objectReferenceValue = feedback;
            so.ApplyModifiedProperties();
        }
        // Wire ColorEcho refs
        var seqMgr = Object.FindFirstObjectByType<ColorSequenceManager>(FindObjectsInactive.Include);
        if (seqMgr != null || feedback != null)
        {
            var so = new SerializedObject(colorEchoGO.GetComponent<ColorEchoEffect>());
            if (seqMgr != null)  so.FindProperty("_sequenceManager").objectReferenceValue = seqMgr;
            if (feedback != null) so.FindProperty("_feedbackUI").objectReferenceValue    = feedback;
            so.ApplyModifiedProperties();
        }

        // Find or create ActiveSkillControllers for slots 2 and 3
        bubbleSkillCtrl = EnsureSkillController("ActiveSkill_BubbleShield", ActiveSkillController.SkillSlot.Slot2,
            bubbleShieldGO.GetComponent<BubbleShieldEffect>(), "Bubble Shield", 12f);
        echoSkillCtrl   = EnsureSkillController("ActiveSkill_ColorEcho",   ActiveSkillController.SkillSlot.Slot3,
            colorEchoGO.GetComponent<ColorEchoEffect>(),    "Color Echo",   10f);
    }

    private static ActiveSkillController EnsureSkillController(string name,
        ActiveSkillController.SkillSlot slot, ActiveSkillEffect effect, string displayName, float cooldown)
    {
        var go = FindInActiveScene(name);
        if (go == null)
        {
            go = new GameObject(name);
            go.AddComponent<ActiveSkillController>();
            Debug.Log($"[HUDFixV2] Created {name} GameObject.");
        }
        var ctrl = go.GetComponent<ActiveSkillController>();
        var so   = new SerializedObject(ctrl);

        // Wire input reader (look up the existing instance used by Jebby slot1)
        var existing = Object.FindFirstObjectByType<ActiveSkillController>(FindObjectsInactive.Include);
        if (existing != null && existing != ctrl)
        {
            var existSO = new SerializedObject(existing);
            so.FindProperty("_input").objectReferenceValue = existSO.FindProperty("_input").objectReferenceValue;
        }
        else
        {
            var ir = AssetDatabase.LoadAssetAtPath<InputReader>(InputReaderPath);
            if (ir != null) so.FindProperty("_input").objectReferenceValue = ir;
        }

        so.FindProperty("_slot").enumValueIndex = (int)slot;
        so.FindProperty("_effect").objectReferenceValue = effect;
        var feedback = Object.FindFirstObjectByType<GameFeedbackUI>(FindObjectsInactive.Include);
        if (feedback != null) so.FindProperty("_feedbackUI").objectReferenceValue = feedback;
        so.FindProperty("_cooldownSeconds").floatValue = cooldown;
        so.FindProperty("_displayName").stringValue = displayName;
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(ctrl);
        return ctrl;
    }

    // ── 3 skill buttons around Jump (petals) ─────────────────────────────────
    private static void BuildThreeSkillButtons()
    {
        var skill1 = FindInActiveScene("Btn_Skill");
        if (skill1 == null) skill1 = FindInActiveScene("Btn_Skill1");
        if (skill1 == null) { Debug.LogError("[HUDFixV2] Btn_Skill / Btn_Skill1 not found."); return; }

        // Rename original to Btn_Skill1 if needed
        if (skill1.name == "Btn_Skill")
        {
            skill1.name = "Btn_Skill1";
            Debug.Log("[HUDFixV2] Renamed Btn_Skill → Btn_Skill1.");
        }

        // Position Skill1 upper-left of Jump
        SetButton(skill1, new Vector2(-225f, 240f), new Vector2(100f, 100f));

        // Skill2 (Bubble Shield) — left of Jump
        var skill2 = EnsureDuplicateSkillButton(skill1, "Btn_Skill2", "Skill2Icon", LifeIconPath, "<Keyboard>/k");
        SetButton(skill2, new Vector2(-265f, 140f), new Vector2(100f, 100f));

        // Skill3 (Color Echo) — above Jump
        var skill3 = EnsureDuplicateSkillButton(skill1, "Btn_Skill3", "Skill3Icon", MemoryIconPath, "<Keyboard>/l");
        SetButton(skill3, new Vector2(-130f, 270f), new Vector2(100f, 100f));

        // Wire skill2 and skill3 OnClick to their controllers
        var bubble = FindInActiveScene("ActiveSkill_BubbleShield")?.GetComponent<ActiveSkillController>();
        var echo   = FindInActiveScene("ActiveSkill_ColorEcho")?.GetComponent<ActiveSkillController>();
        WireSkillButtonOnClick(skill2, bubble);
        WireSkillButtonOnClick(skill3, echo);

        // Make sure cooldown overlay is circular on the new buttons
        CircularizeCooldown(skill2);
        CircularizeCooldown(skill3);

        Debug.Log("[HUDFixV2] 3 skill buttons placed around Jump (petals).");
    }

    private static void SetButton(GameObject btn, Vector2 anchoredPos, Vector2 size)
    {
        var rt = btn.GetComponent<RectTransform>();
        if (rt == null) return;
        rt.anchorMin = new Vector2(1f, 0f);
        rt.anchorMax = new Vector2(1f, 0f);
        rt.pivot     = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;
        rt.anchoredPosition = anchoredPos;
        EditorUtility.SetDirty(btn);
    }

    private static GameObject EnsureDuplicateSkillButton(GameObject template, string newName,
        string iconChildName, string iconPath, string onScreenKeyPath)
    {
        var existing = FindInActiveScene(newName);
        GameObject dup;
        if (existing != null) dup = existing;
        else
        {
            dup = Object.Instantiate(template, template.transform.parent);
            dup.name = newName;
            Debug.Log($"[HUDFixV2] Duplicated {template.name} → {newName}.");
        }

        // Rename the icon child to a unique name
        var rocketIcon = dup.transform.Find("RocketBootsIcon");
        if (rocketIcon != null && rocketIcon.name != iconChildName)
        {
            rocketIcon.name = iconChildName;
        }
        var iconT = dup.transform.Find(iconChildName);
        var iconSprite = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
        if (iconT != null && iconSprite != null)
        {
            var iconImg = iconT.GetComponent<Image>();
            if (iconImg != null)
            {
                iconImg.sprite = iconSprite;
                iconImg.preserveAspect = true;
                EditorUtility.SetDirty(iconImg);
            }
        }

        // Update OnScreenButton to use the new key path so this button simulates K or L.
        var osb = dup.GetComponent<OnScreenButton>();
        if (osb != null)
        {
            var so = new SerializedObject(osb);
            var pathProp = so.FindProperty("m_ControlPath");
            if (pathProp != null) { pathProp.stringValue = onScreenKeyPath; so.ApplyModifiedProperties(); }
        }

        // Wipe inherited Button.onClick listeners (the duplicate carries the original's link)
        var btn = dup.GetComponent<Button>();
        if (btn != null)
        {
            // Remove all persistent listeners by clearing the event via SerializedObject.
            var so = new SerializedObject(btn);
            var calls = so.FindProperty("m_OnClick.m_PersistentCalls.m_Calls");
            if (calls != null) calls.ClearArray();
            so.ApplyModifiedProperties();
        }

        return dup;
    }

    private static void WireSkillButtonOnClick(GameObject btn, ActiveSkillController ctrl)
    {
        if (ctrl == null) return;
        var b = btn.GetComponent<Button>();
        if (b == null) return;
        // Add persistent listener to ctrl.TryUseSkill (mobile tap path; desktop uses keyboard)
        UnityEditor.Events.UnityEventTools.AddPersistentListener(b.onClick, ctrl.TryUseSkill);
        EditorUtility.SetDirty(b);
        Debug.Log($"[HUDFixV2] Wired {btn.name}.onClick → {ctrl.name}.TryUseSkill.");
    }

    // ── MemoryPhaseController wiring ─────────────────────────────────────────
    private static void WireMemoryPhaseControllerRefs(GameObject bubbleShieldGO,
        ActiveSkillController bubbleCtrl, ActiveSkillController echoCtrl)
    {
        var mpc = Object.FindFirstObjectByType<MemoryPhaseController>(FindObjectsInactive.Include);
        if (mpc == null) return;
        var so = new SerializedObject(mpc);

        // _bubbleShield
        var shieldProp = so.FindProperty("_bubbleShield");
        if (shieldProp != null)
        {
            shieldProp.objectReferenceValue = bubbleShieldGO.GetComponent<BubbleShieldEffect>();
        }

        // _activeSkills array: include existing slot-1 controller + new slot-2 + new slot-3
        var skillsProp = so.FindProperty("_activeSkills");
        if (skillsProp != null)
        {
            // Find slot-1 controller (existing — on Jebby presumably)
            ActiveSkillController slot1 = null;
            foreach (var c in Object.FindObjectsByType<ActiveSkillController>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (c.Slot == ActiveSkillController.SkillSlot.Slot1) { slot1 = c; break; }
            }

            skillsProp.arraySize = 3;
            skillsProp.GetArrayElementAtIndex(0).objectReferenceValue = slot1;
            skillsProp.GetArrayElementAtIndex(1).objectReferenceValue = bubbleCtrl;
            skillsProp.GetArrayElementAtIndex(2).objectReferenceValue = echoCtrl;
        }

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(mpc);
        Debug.Log("[HUDFixV2] MemoryPhaseController wired: _bubbleShield + _activeSkills[3].");
    }

    // ── helpers ──────────────────────────────────────────────────────────────
    private static GameObject FindInActiveScene(string name)
    {
        var all = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var go in all)
        {
            if (go.name != name) continue;
            if (!go.scene.IsValid()) continue;
            return go;
        }
        return null;
    }
}
