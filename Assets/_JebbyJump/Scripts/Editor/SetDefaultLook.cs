using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JebbyJump.EditorTools
{
    // The default outfit (classic_color_knight) has NO art of its own - it points
    // at a VARIANT's sprite set. This idempotent tool retargets everything that
    // defines "the default look" to the variant below:
    //   1) the 7 base Jebby_<State>.anim clips' sprite keyframes,
    //   2) the Jebby prefab's initial SpriteRenderer sprite (idle),
    //   3) the WardrobePreviewLibrary's classic_color_knight pose entries.
    // To point the default at a different variant before production, change
    // VariantId and re-run the menu. Never touches the variant AOCs/clips, the
    // catalog, or import settings.
    public static class SetDefaultLook
    {
        // The variant whose art the default outfit currently shows.
        public const string VariantId = "rookie_page";

        private const string ClipFolder = "Assets/_JebbyJump/Art/Animations/";
        private const string OutfitsRoot = "Assets/_JebbyJump/Art/Characters/Jebby/Outfits/";
        private const string PrefabPath = "Assets/_JebbyJump/Prefabs/Player/Jebby.prefab";
        private const string PreviewLibraryPath =
            OutfitsRoot + "WardrobePreviewLibrary.asset";
        private const string DefaultOutfitId = "classic_color_knight";

        // state (sprite/pose suffix), base clip name, preview field name
        private static readonly (string state, string clip, string pose)[] States =
        {
            ("idle", "Jebby_Idle", "Idle"),
            ("run", "Jebby_Run", "Run"),
            ("jump", "Jebby_Jump", "Jump"),
            ("fall", "Jebby_Fall", "Fall"),
            ("land", "Jebby_Land", "Land"),
            ("hurt", "Jebby_Hurt", "Hurt"),
            ("victory", "Jebby_Victory", "Victory"),
        };

        public static string VariantSpritePath(string state)
            => OutfitsRoot + ToPascal(VariantId) + "/Sprites/spr_jebby_"
               + VariantId + "_" + state + "_01.png";

        [MenuItem("Jebby Jump/Wardrobe/Set Default Look")]
        public static void Run()
        {
            int changes = 0;

            // 1) base clips -> variant sprites
            foreach (var (state, clipName, _) in States)
            {
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(
                    ClipFolder + clipName + ".anim");
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(
                    VariantSpritePath(state));
                if (clip == null || sprite == null)
                {
                    Debug.LogWarning("[SetDefaultLook] missing "
                        + (clip == null ? "clip " + clipName : "sprite " + state)
                        + " - skipped");
                    continue;
                }
                foreach (var binding in
                    AnimationUtility.GetObjectReferenceCurveBindings(clip))
                {
                    if (binding.propertyName != "m_Sprite") continue;
                    var keys = AnimationUtility.GetObjectReferenceCurve(clip, binding);
                    bool dirty = false;
                    for (int i = 0; i < keys.Length; i++)
                    {
                        if (ReferenceEquals(keys[i].value, sprite)) continue;
                        keys[i].value = sprite;
                        dirty = true;
                    }
                    if (dirty)
                    {
                        AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);
                        EditorUtility.SetDirty(clip);
                        changes++;
                    }
                }
            }

            // 2) prefab initial sprite -> variant idle
            var idle = AssetDatabase.LoadAssetAtPath<Sprite>(VariantSpritePath("idle"));
            var root = PrefabUtility.LoadPrefabContents(PrefabPath);
            try
            {
                var sr = root.GetComponentInChildren<SpriteRenderer>(true);
                if (sr != null && idle != null && sr.sprite != idle)
                {
                    sr.sprite = idle;
                    PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
                    changes++;
                }
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }

            // 3) preview library default entry -> variant pose sprites
            var lib = AssetDatabase.LoadAssetAtPath<Object>(PreviewLibraryPath);
            if (lib != null)
            {
                var so = new SerializedObject(lib);
                var entries = so.FindProperty("_entries");
                for (int i = 0; i < entries.arraySize; i++)
                {
                    var entry = entries.GetArrayElementAtIndex(i);
                    if (entry.FindPropertyRelative("OutfitId").stringValue
                        != DefaultOutfitId) continue;
                    bool dirty = false;
                    foreach (var (state, _, pose) in States)
                    {
                        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(
                            VariantSpritePath(state));
                        var field = entry.FindPropertyRelative(pose);
                        if (sprite == null || field == null
                            || field.objectReferenceValue == sprite) continue;
                        field.objectReferenceValue = sprite;
                        dirty = true;
                    }
                    if (dirty) { so.ApplyModifiedPropertiesWithoutUndo(); changes++; }
                    break;
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log("[SetDefaultLook] default look -> " + VariantId
                + "; " + changes + " asset change(s).");
        }

        private static string ToPascal(string snake)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var part in snake.Split('_'))
            {
                if (part.Length == 0) continue;
                sb.Append(char.ToUpperInvariant(part[0]));
                if (part.Length > 1) sb.Append(part.Substring(1));
            }
            return sb.ToString();
        }
    }
}
