using JebbyJump.Wardrobe;
using JebbyJump.Wardrobe.Visual;
using UnityEditor;
using UnityEngine;

// Idempotent scaffold: creates/populates the WardrobePreviewLibrary asset
// (UI-only outfit thumbnails) by loading each outfit's existing idle sprite.
// One entry per WardrobeCatalog outfit, including the default. Replace-by-id
// keeps entries unique on rerun. Read-only on the source sprites; does NOT
// change import settings or any other asset.
public static class BuildWardrobePreviewLibrary
{
    private const string LibraryPath =
        "Assets/_JebbyJump/Art/Characters/Jebby/Outfits/WardrobePreviewLibrary.asset";
    private const string DefaultIdlePath =
        "Assets/_JebbyJump/Art/Sprites/Characters/Jebby/spr_jebby_idle_01.png";
    private const string OutfitsRoot =
        "Assets/_JebbyJump/Art/Characters/Jebby/Outfits/";

    [MenuItem("Jebby Jump/Scaffold/Build Wardrobe Preview Library")]
    public static void Run()
    {
        var library = AssetDatabase.LoadAssetAtPath<WardrobePreviewLibrary>(LibraryPath);
        if (library == null)
        {
            library = ScriptableObject.CreateInstance<WardrobePreviewLibrary>();
            AssetDatabase.CreateAsset(library, LibraryPath);
            Debug.Log("[WardrobePreview] Created " + LibraryPath);
        }

        int wired = 0, missing = 0;
        foreach (var def in WardrobeCatalog.Outfits)
        {
            string path = IdleSpritePath(def.Id);
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null)
            {
                missing++;
                Debug.LogWarning("[WardrobePreview] " + def.Id
                    + ": idle sprite not found at " + path);
                library.AddEntry(def.Id, null);
                continue;
            }
            library.AddEntry(def.Id, sprite);
            wired++;
        }

        EditorUtility.SetDirty(library);
        AssetDatabase.SaveAssets();
        Debug.Log("[WardrobePreview] Done: " + wired + " previews wired, "
            + missing + " missing; library has " + library.Count + " entries.");
    }

    private static string IdleSpritePath(string outfitId)
    {
        if (outfitId == WardrobeCatalog.DefaultOutfitId) return DefaultIdlePath;
        return OutfitsRoot + ToPascal(outfitId) + "/Sprites/spr_jebby_"
            + outfitId + "_idle_01.png";
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
