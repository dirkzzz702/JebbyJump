using System.Collections.Generic;
using JebbyJump.Wardrobe.Visual;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

// Idempotent end-to-end scaffold for the P13 outfit visual import. For each
// outfit set under Art/Characters/Jebby/Outfits/<Outfit>/Sprites/ it:
//   1. configures sprite import settings to match default Jebby
//      (Sprite/Single, PPU 100, BottomCenter pivot, Bilinear, Tight,
//      Alpha Is Transparency),
//   2. creates one single-frame sprite clip per state
//      (anim_jebby_<id>_<state>.anim), settings copied from the matching
//      default Jebby_<State>.anim clip,
//   3. creates aoc_jebby_<id>.overrideController (base JebbyAnimator)
//      overriding the 7 default clips,
//   4. registers <id> -> controller in the OutfitVisualLibrary asset,
//   5. wires the library into the Jebby prefab's
//      PlayerOutfitVisualController._library.
// Re-runs reuse existing assets (no duplicates). Default Jebby assets,
// Animator parameters/states, and gameplay are never modified.
public static class ImportOutfitVisuals
{
    private const string OutfitsRoot =
        "Assets/_JebbyJump/Art/Characters/Jebby/Outfits/";
    private const string BaseControllerPath =
        "Assets/_JebbyJump/Art/Animations/JebbyAnimator.controller";
    private const string DefaultClipFolder =
        "Assets/_JebbyJump/Art/Animations/";
    private const string LibraryPath = OutfitsRoot + "OutfitVisualLibrary.asset";
    private const string JebbyPrefabPath =
        "Assets/_JebbyJump/Prefabs/Player/Jebby.prefab";

    // Folder name -> outfit id (matches WardrobeCatalog ids).
    private static readonly (string folder, string id)[] Outfits =
    {
        ("ForestCavalier", "forest_cavalier"),
        ("SilverDreamer", "silver_dreamer"),
        ("SunshineKnight", "sunshine_knight"),
        ("CrimsonHero", "crimson_hero"),
        ("AquaKnight", "aqua_knight"),
        // RookiePage is NOT imported as an outfit: its sprite set is the
        // default look (classic_color_knight points at it - SetDefaultLook),
        // and rookie_page is no longer a catalog id. Re-add here only if a
        // distinct Rookie Page variant returns to the catalog.
        ("PastelPrince", "pastel_prince"),
    };

    private static readonly (string state, string defaultClip)[] States =
    {
        ("idle", "Jebby_Idle"),
        ("run", "Jebby_Run"),
        ("jump", "Jebby_Jump"),
        ("fall", "Jebby_Fall"),
        ("land", "Jebby_Land"),
        ("hurt", "Jebby_Hurt"),
        ("victory", "Jebby_Victory"),
    };

    [MenuItem("Jebby Jump/Scaffold/Import Outfit Visuals")]
    public static void Run()
    {
        var baseController =
            AssetDatabase.LoadAssetAtPath<AnimatorController>(BaseControllerPath);
        if (baseController == null)
        {
            Debug.LogError("[OutfitImport] Base controller not found at "
                + BaseControllerPath);
            return;
        }

        var library = LoadOrCreateLibrary();

        int outfitsDone = 0;
        foreach (var (folder, id) in Outfits)
        {
            if (ProcessOutfit(folder, id, baseController, library))
                outfitsDone++;
        }

        EditorUtility.SetDirty(library);
        AssetDatabase.SaveAssets();

        WireLibraryIntoPrefab(library);
        Debug.Log("[OutfitImport] Done: " + outfitsDone + "/" + Outfits.Length
            + " outfits processed; library has " + library.Count + " entries.");
    }

    private static bool ProcessOutfit(string folder, string id,
        AnimatorController baseController, OutfitVisualLibrary library)
    {
        string spriteFolder = OutfitsRoot + folder + "/Sprites/";
        string animFolder = OutfitsRoot + folder + "/Animations";
        string ctrlFolder = OutfitsRoot + folder + "/Controllers";

        // 1) sprites: configure import settings + collect per-state sprites.
        var sprites = new Dictionary<string, Sprite>();
        foreach (var (state, _) in States)
        {
            string path = spriteFolder + "spr_jebby_" + id + "_" + state + "_01.png";
            if (!ConfigureSpriteImporter(path)) return Fail(id, "sprite missing: " + path);
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null) return Fail(id, "sprite did not load: " + path);
            sprites[state] = sprite;
        }

        EnsureFolder(animFolder);
        EnsureFolder(ctrlFolder);

        // 2) clips: one single-frame sprite clip per state.
        var clips = new Dictionary<string, AnimationClip>();
        foreach (var (state, defaultClipName) in States)
        {
            string clipPath = animFolder + "/anim_jebby_" + id + "_" + state + ".anim";
            var defaultClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(
                DefaultClipFolder + defaultClipName + ".anim");
            if (defaultClip == null)
                return Fail(id, "default clip missing: " + defaultClipName);
            clips[state] = CreateOrUpdateClip(clipPath, sprites[state], defaultClip);
        }

        // 3) override controller over the shared base.
        string aocPath = ctrlFolder + "/aoc_jebby_" + id + ".overrideController";
        var aoc = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(aocPath);
        if (aoc == null)
        {
            aoc = new AnimatorOverrideController(baseController);
            AssetDatabase.CreateAsset(aoc, aocPath);
            Debug.Log("[OutfitImport] Created " + aocPath);
        }
        else if (aoc.runtimeAnimatorController != baseController)
        {
            aoc.runtimeAnimatorController = baseController;
        }
        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        aoc.GetOverrides(overrides);
        for (int i = 0; i < overrides.Count; i++)
        {
            foreach (var (state, defaultClipName) in States)
            {
                if (overrides[i].Key != null
                    && overrides[i].Key.name == defaultClipName)
                {
                    overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(
                        overrides[i].Key, clips[state]);
                }
            }
        }
        aoc.ApplyOverrides(overrides);
        EditorUtility.SetDirty(aoc);

        // 4) library entry.
        library.AddEntry(id, aoc);
        return true;
    }

    private static bool ConfigureSpriteImporter(string path)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return false;

        var settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);
        bool dirty =
            importer.textureType != TextureImporterType.Sprite
            || importer.spriteImportMode != SpriteImportMode.Single
            || !Mathf.Approximately(importer.spritePixelsPerUnit, 100f)
            || !importer.alphaIsTransparency
            || importer.filterMode != FilterMode.Bilinear
            || settings.spriteMeshType != SpriteMeshType.Tight
            || settings.spriteAlignment != (int)SpriteAlignment.BottomCenter;
        if (!dirty) return true;

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 100f;
        importer.alphaIsTransparency = true;
        importer.filterMode = FilterMode.Bilinear;
        importer.ReadTextureSettings(settings);
        settings.spriteMeshType = SpriteMeshType.Tight;
        settings.spriteAlignment = (int)SpriteAlignment.BottomCenter;
        importer.SetTextureSettings(settings);
        importer.SaveAndReimport();
        return true;
    }

    private static AnimationClip CreateOrUpdateClip(
        string clipPath, Sprite sprite, AnimationClip defaultClip)
    {
        var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
        bool isNew = clip == null;
        if (isNew)
        {
            clip = new AnimationClip();
            clip.frameRate = defaultClip.frameRate;
        }

        var binding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = string.Empty,
            propertyName = "m_Sprite",
        };
        AnimationUtility.SetObjectReferenceCurve(clip, binding,
            new[] { new ObjectReferenceKeyframe { time = 0f, value = sprite } });
        AnimationUtility.SetAnimationClipSettings(clip,
            AnimationUtility.GetAnimationClipSettings(defaultClip));

        if (isNew)
        {
            AssetDatabase.CreateAsset(clip, clipPath);
        }
        EditorUtility.SetDirty(clip);
        return clip;
    }

    private static OutfitVisualLibrary LoadOrCreateLibrary()
    {
        var library =
            AssetDatabase.LoadAssetAtPath<OutfitVisualLibrary>(LibraryPath);
        if (library == null)
        {
            library = ScriptableObject.CreateInstance<OutfitVisualLibrary>();
            AssetDatabase.CreateAsset(library, LibraryPath);
            Debug.Log("[OutfitImport] Created " + LibraryPath);
        }
        return library;
    }

    private static void WireLibraryIntoPrefab(OutfitVisualLibrary library)
    {
        var root = PrefabUtility.LoadPrefabContents(JebbyPrefabPath);
        try
        {
            var ctrl = root.GetComponent<PlayerOutfitVisualController>();
            if (ctrl == null)
            {
                Debug.LogError("[OutfitImport] Jebby prefab has no "
                    + "PlayerOutfitVisualController - run "
                    + "'Attach Player Outfit Visual' first.");
                return;
            }
            var so = new SerializedObject(ctrl);
            var prop = so.FindProperty("_library");
            if (prop.objectReferenceValue != library)
            {
                prop.objectReferenceValue = library;
                so.ApplyModifiedPropertiesWithoutUndo();
                PrefabUtility.SaveAsPrefabAsset(root, JebbyPrefabPath);
                Debug.Log("[OutfitImport] Wired OutfitVisualLibrary into "
                    + "Jebby prefab.");
            }
            else
            {
                Debug.Log("[OutfitImport] Jebby prefab already wired - reusing.");
            }
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(root);
        }
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        int slash = path.LastIndexOf('/');
        AssetDatabase.CreateFolder(path.Substring(0, slash), path.Substring(slash + 1));
    }

    private static bool Fail(string id, string reason)
    {
        Debug.LogError("[OutfitImport] " + id + ": " + reason);
        return false;
    }
}
