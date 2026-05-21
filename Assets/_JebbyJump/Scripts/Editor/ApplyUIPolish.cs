using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// Surgical UI polish — runs once, idempotent.
// HUD hierarchy after this pass:
//   top-left:   LivesIconContainer (48 px hearts, 32 px padding)
//   top-center: LevelBadgeRoot (pill BG + TMP text)
//   below:      SequencePanel (memory swatches)
//   bottom:     TutorialHintRoot (compact banner, above mobile buttons)
//   bottom-LR:  movement buttons
//   bottom-RR:  jump (large) + skill (smaller, ui_btn_bg + RocketBoots icon)
public static class ApplyUIPolish
{
    private const string ScenePath = "Assets/_JebbyJump/Scenes/Game.unity";
    private const string BtnBgPath = "Assets/_JebbyJump/Art/Sprites/UI/ui_btn_bg.png";
    private const string RocketBootsPath = "Assets/_JebbyJump/Art/Sprites/Items/spr_item_rocket_boots_01.png";

    // Reference resolution 1920x1080, screen-space overlay. Tutorial banner y from bottom
    // must clear the mobile button row (buttons sit at y=140 from bottom with ~140 size,
    // so their top edge ≈ y=210). Banner pivot bottom, anchor (0.5, 0), y=240 keeps it
    // above the buttons with a small gap.
    private const float TutorialBannerY = 280f;
    private const float TutorialBannerHeight = 90f;

    public static void Execute()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        BuildLevelBadge();
        RepositionSequencePanel();
        MoveTutorialToBottomBanner();
        PadHearts();
        VerifyMobileButtons();
        StyleSkillButtonAndCooldown();
        ReportFloorArt();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[UIPolish] Done.");
    }

    // ── LevelBadgeRoot: pill BG + TMP text child ─────────────────────────────
    private static void BuildLevelBadge()
    {
        var levelText = FindInActiveScene("LevelText");
        if (levelText == null) { Debug.LogWarning("[UIPolish] LevelText not found."); return; }

        var parent = levelText.transform.parent;
        if (parent == null) { Debug.LogWarning("[UIPolish] LevelText has no parent."); return; }

        // If a LevelBadgeRoot already exists, reuse it.
        var badge = parent.Find("LevelBadgeRoot") as RectTransform;
        if (badge == null)
        {
            var go = new GameObject("LevelBadgeRoot", typeof(RectTransform), typeof(Image));
            badge = go.GetComponent<RectTransform>();
            badge.SetParent(parent, false);

            var bg = go.GetComponent<Image>();
            bg.color = new Color(0f, 0f, 0f, 0.55f);
            // Use Unity's default UI sprite for rounded rectangle look.
            var defaultSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            if (defaultSprite != null) { bg.sprite = defaultSprite; bg.type = Image.Type.Sliced; }
            Debug.Log("[UIPolish] LevelBadgeRoot created.");
        }

        badge.anchorMin = new Vector2(0.5f, 1f);
        badge.anchorMax = new Vector2(0.5f, 1f);
        badge.pivot     = new Vector2(0.5f, 1f);
        badge.sizeDelta = new Vector2(280f, 68f);
        badge.anchoredPosition = new Vector2(0f, -28f);

        // Re-parent LevelText into the badge as a child so it always renders ABOVE the BG.
        var levelRT = levelText.GetComponent<RectTransform>();
        if (levelText.transform.parent != badge)
        {
            levelText.transform.SetParent(badge, false);
            Debug.Log("[UIPolish] LevelText re-parented under LevelBadgeRoot.");
        }
        levelText.transform.SetAsLastSibling(); // text renders after BG → on top
        levelRT.anchorMin = Vector2.zero;
        levelRT.anchorMax = Vector2.one;
        levelRT.pivot     = new Vector2(0.5f, 0.5f);
        levelRT.offsetMin = new Vector2(16f, 6f);
        levelRT.offsetMax = new Vector2(-16f, -6f);

        var tmp = levelText.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.fontSize = 40f;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            tmp.fontStyle = FontStyles.Bold;
            tmp.outlineWidth = 0.2f;
            tmp.outlineColor = Color.black;
            EditorUtility.SetDirty(tmp);
        }

        EditorUtility.SetDirty(badge);
        EditorUtility.SetDirty(levelText);
    }

    // ── SequencePanel: position below LevelBadge ─────────────────────────────
    private static void RepositionSequencePanel()
    {
        var panel = FindInActiveScene("SequencePanel");
        if (panel == null) { Debug.LogWarning("[UIPolish] SequencePanel not found."); return; }

        var rt = panel.GetComponent<RectTransform>();
        if (rt == null) return;
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot     = new Vector2(0.5f, 1f);
        rt.sizeDelta = new Vector2(400f, 80f);
        rt.anchoredPosition = new Vector2(0f, -110f);   // below the 68-tall badge at y=-28
        EditorUtility.SetDirty(panel);
        Debug.Log("[UIPolish] SequencePanel anchored top-center, y=-110.");
    }

    // ── TutorialHintRoot: bottom banner ──────────────────────────────────────
    private static void MoveTutorialToBottomBanner()
    {
        var ctrl = Object.FindFirstObjectByType<JebbyJump.UI.TutorialHintController>(FindObjectsInactive.Include);
        if (ctrl == null) { Debug.LogWarning("[UIPolish] TutorialHintController not found."); return; }

        var so = new SerializedObject(ctrl);
        var hintRoot = so.FindProperty("_hintRoot")?.objectReferenceValue as GameObject;
        if (hintRoot == null) { Debug.LogWarning("[UIPolish] TutorialHintController._hintRoot null."); return; }

        var rt = hintRoot.GetComponent<RectTransform>();
        if (rt == null) { Debug.LogWarning("[UIPolish] Tutorial hint root no RectTransform."); return; }

        rt.anchorMin = new Vector2(0.5f, 0f);
        rt.anchorMax = new Vector2(0.5f, 0f);
        rt.pivot     = new Vector2(0.5f, 0f);
        rt.sizeDelta = new Vector2(720f, TutorialBannerHeight);
        rt.anchoredPosition = new Vector2(0f, TutorialBannerY);

        // Sanity check vs mobile buttons. Buttons bottom-anchored at y=140 with size ~140
        // → top edge ≈ y=210. Banner bottom edge = TutorialBannerY = 280. Gap ≈ 70. OK.
        Debug.Log($"[UIPolish] TutorialHintRoot bottom banner: y={TutorialBannerY}, " +
                  $"size=720x{TutorialBannerHeight}. Gap above buttons ≈ 70 px (1920x1080 ref).");
        EditorUtility.SetDirty(hintRoot);
    }

    // ── Hearts: pad from corner ──────────────────────────────────────────────
    private static void PadHearts()
    {
        var go = FindInActiveScene("LivesIconContainer");
        if (go == null) { Debug.LogWarning("[UIPolish] LivesIconContainer not found."); return; }
        var rt = go.GetComponent<RectTransform>();
        if (rt == null) return;
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot     = new Vector2(0f, 1f);
        rt.sizeDelta = new Vector2(260f, 60f);
        rt.anchoredPosition = new Vector2(32f, -32f);

        var hlg = go.GetComponent<HorizontalLayoutGroup>();
        if (hlg != null) { hlg.spacing = 8f; EditorUtility.SetDirty(hlg); }

        EditorUtility.SetDirty(go);
        Debug.Log("[UIPolish] LivesIconContainer: anchored (32, -32), 260x60.");
    }

    // ── Mobile buttons: verify positions & safe-area ─────────────────────────
    private static void VerifyMobileButtons()
    {
        VerifyButton("Btn_Left",  new Vector2(0f, 0f), new Vector2(110f, 140f), new Vector2(130f, 130f));
        VerifyButton("Btn_Right", new Vector2(0f, 0f), new Vector2(270f, 140f), new Vector2(130f, 130f));
        VerifyButton("Btn_Jump",  new Vector2(1f, 0f), new Vector2(-130f, 140f), new Vector2(140f, 140f));
        VerifyButton("Btn_Skill", new Vector2(1f, 0f), new Vector2(-290f, 140f), new Vector2(110f, 110f));
    }

    private static void VerifyButton(string name, Vector2 anchor, Vector2 anchoredPos, Vector2 size)
    {
        var go = FindInActiveScene(name);
        if (go == null) { Debug.LogWarning($"[UIPolish] {name} not found."); return; }
        var rt = go.GetComponent<RectTransform>();
        if (rt == null) return;
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot     = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;
        rt.anchoredPosition = anchoredPos;
        EditorUtility.SetDirty(go);
        Debug.Log($"[UIPolish] {name}: anchor {anchor}, pos {anchoredPos}, size {size}.");
    }

    // ── Skill button: ui_btn_bg background + RocketBoots icon centered ───────
    private static void StyleSkillButtonAndCooldown()
    {
        var skill = FindInActiveScene("Btn_Skill");
        if (skill == null) { Debug.LogWarning("[UIPolish] Btn_Skill not found."); return; }

        // Background sprite
        var bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>(BtnBgPath);
        if (bgSprite == null) { Debug.LogError($"[UIPolish] {BtnBgPath} not found."); return; }

        var skillImg = skill.GetComponent<Image>();
        if (skillImg != null)
        {
            skillImg.sprite = bgSprite;
            skillImg.preserveAspect = true;
            skillImg.color = Color.white;
            EditorUtility.SetDirty(skillImg);
            Debug.Log("[UIPolish] Btn_Skill BG sprite set to ui_btn_bg.");
        }

        // RocketBoots icon child (added once)
        Transform iconT = skill.transform.Find("RocketBootsIcon");
        if (iconT == null)
        {
            var iconGO = new GameObject("RocketBootsIcon", typeof(RectTransform), typeof(Image));
            iconGO.transform.SetParent(skill.transform, false);
            iconT = iconGO.transform;
            Debug.Log("[UIPolish] RocketBootsIcon added under Btn_Skill.");
        }

        var iconRT = iconT.GetComponent<RectTransform>();
        iconRT.anchorMin = Vector2.zero;
        iconRT.anchorMax = Vector2.one;
        iconRT.pivot     = new Vector2(0.5f, 0.5f);
        iconRT.offsetMin = new Vector2(20f, 20f);
        iconRT.offsetMax = new Vector2(-20f, -20f);
        var iconImg = iconT.GetComponent<Image>();
        var iconSprite = AssetDatabase.LoadAssetAtPath<Sprite>(RocketBootsPath);
        if (iconSprite != null) { iconImg.sprite = iconSprite; iconImg.preserveAspect = true; }
        iconImg.raycastTarget = false; // background button receives the tap
        EditorUtility.SetDirty(iconImg);

        // Order: BG (skill Image) → RocketBootsIcon → CooldownOverlay → CooldownLabel
        // The Image component IS the BG. Set sibling order for the children.
        var overlay = skill.transform.Find("CooldownOverlay");
        var label   = skill.transform.Find("CooldownLabel");

        iconT.SetSiblingIndex(0);
        if (overlay != null) overlay.SetSiblingIndex(1);
        if (label != null)   label.SetSiblingIndex(2);

        // Cooldown overlay: confirm it's a Filled-Radial dark dim (already set), keep dim
        if (overlay != null)
        {
            var ovImg = overlay.GetComponent<Image>();
            if (ovImg != null)
            {
                ovImg.color = new Color(0f, 0f, 0f, 0.6f);
                ovImg.raycastTarget = false;
                EditorUtility.SetDirty(ovImg);
            }
        }

        // Cooldown label: larger, bold, outlined for readability
        if (label != null)
        {
            var tmp = label.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.fontSize = 40f;
                tmp.fontStyle = FontStyles.Bold;
                tmp.color = Color.white;
                tmp.outlineWidth = 0.25f;
                tmp.outlineColor = Color.black;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.raycastTarget = false;
                EditorUtility.SetDirty(tmp);
                Debug.Log("[UIPolish] CooldownLabel: 40 pt bold + black outline.");
            }
        }
    }

    // ── Floor visual: report only ─────────────────────────────────────────────
    private static void ReportFloorArt()
    {
        // We deliberately do NOT add a soft top highlight without a dedicated sprite
        // (the only candidates today are the platform bar which would re-introduce a
        // "debug strip" look, or a generated gradient with no source). Per scope: report.
        Debug.LogWarning("[UIPolish] Floor polish needs a dedicated soft-cloud / highlight " +
                         "sprite. None available today. Floor left as solid beige rectangle. " +
                         "Please supply 'Assets/_JebbyJump/Art/Sprites/World/floor_highlight_01.png' " +
                         "(e.g. white-to-transparent vertical gradient or cloud strip) and we will " +
                         "wire a non-collider child SpriteRenderer at the floor top y=-0.5.");
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
