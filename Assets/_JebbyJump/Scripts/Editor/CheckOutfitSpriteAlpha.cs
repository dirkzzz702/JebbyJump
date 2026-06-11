using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Editor-only, READ-ONLY QA gate for outfit sprite intake (P13 Mode A).
// Validates textures against the recorded default-Jebby import contract
// (P12 art request pack section 7) before any outfit art is imported:
//   Sprite (2D and UI), Single, PPU 100, pivot Custom (0.5, 0),
//   Alpha Is Transparency on, fully transparent corners.
// Never modifies importers or assets - it only logs PASS/FAIL per check.
//
// Menu: validates the selected texture assets; with nothing selected it
// falls back to the 7 default Jebby state sprites (which the contract was
// recorded from). RunOnDefaultJebbySprites() is the batchmode entry.
public static class CheckOutfitSpriteAlpha
{
    private const string JebbySpriteFolder =
        "Assets/_JebbyJump/Art/Sprites/Characters/Jebby/";

    private static readonly string[] States =
        { "idle", "run", "jump", "fall", "land", "hurt", "victory" };

    [MenuItem("Jebby Jump/QA/Check Outfit Sprite Alpha")]
    public static void CheckSelection()
    {
        var paths = new List<string>();
        foreach (var obj in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path)
                && path.EndsWith(".png", System.StringComparison.OrdinalIgnoreCase))
                paths.Add(path);
        }

        if (paths.Count == 0)
        {
            Debug.Log("[OutfitQA] No textures selected - checking the 7 "
                + "default Jebby sprites instead.");
            RunOnDefaultJebbySprites();
            return;
        }
        Check(paths);
    }

    // Batchmode entry: checks the 7 default Jebby state sprites.
    public static void RunOnDefaultJebbySprites()
    {
        var paths = new List<string>();
        foreach (var state in States)
            paths.Add(JebbySpriteFolder + "spr_jebby_" + state + "_01.png");
        Check(paths);
    }

    private static void Check(List<string> paths)
    {
        int pass = 0, fail = 0;
        foreach (var path in paths)
        {
            if (CheckOne(path)) pass++;
            else fail++;
        }
        Debug.Log("[OutfitQA] Summary: " + pass + " PASS, " + fail
            + " FAIL of " + paths.Count + " texture(s).");
    }

    private static bool CheckOne(string path)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
        {
            Debug.LogWarning("[OutfitQA] FAIL " + path
                + ": not found or not a texture.");
            return false;
        }

        var problems = new List<string>();

        if (importer.textureType != TextureImporterType.Sprite)
            problems.Add("textureType=" + importer.textureType
                + " (expected Sprite)");
        if (importer.spriteImportMode != SpriteImportMode.Single)
            problems.Add("spriteImportMode=" + importer.spriteImportMode
                + " (expected Single)");
        if (!Mathf.Approximately(importer.spritePixelsPerUnit, 100f))
            problems.Add("PPU=" + importer.spritePixelsPerUnit
                + " (expected 100)");
        if (!importer.alphaIsTransparency)
            problems.Add("alphaIsTransparency=false (expected true)");

        // Pivot must be bottom-center, i.e. (0.5, 0). Default Jebby uses
        // the BottomCenter alignment preset; an explicit Custom (0.5, 0)
        // is equivalent and also accepted.
        var settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);
        bool bottomCenterPreset =
            settings.spriteAlignment == (int)SpriteAlignment.BottomCenter;
        bool customBottomCenter =
            settings.spriteAlignment == (int)SpriteAlignment.Custom
            && Mathf.Approximately(settings.spritePivot.x, 0.5f)
            && Mathf.Approximately(settings.spritePivot.y, 0f);
        if (!bottomCenterPreset && !customBottomCenter)
            problems.Add("spriteAlignment="
                + (SpriteAlignment)settings.spriteAlignment
                + " pivot=" + settings.spritePivot
                + " (expected BottomCenter or Custom (0.5, 0))");

        CheckCornerAlpha(path, problems);

        if (problems.Count == 0)
        {
            Debug.Log("[OutfitQA] PASS " + path);
            return true;
        }
        Debug.LogWarning("[OutfitQA] FAIL " + path + ": "
            + string.Join("; ", problems));
        return false;
    }

    // Reads the PNG bytes from disk into a temp texture so the check works
    // without the asset being marked Read/Write Enabled. Destroys the temp.
    private static void CheckCornerAlpha(string path, List<string> problems)
    {
        byte[] bytes;
        try
        {
            bytes = System.IO.File.ReadAllBytes(path);
        }
        catch (System.Exception e)
        {
            problems.Add("could not read file for corner check ("
                + e.GetType().Name + ")");
            return;
        }

        var tmp = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        try
        {
            if (!ImageConversion.LoadImage(tmp, bytes))
            {
                problems.Add("could not decode PNG for corner check");
                return;
            }
            int w = tmp.width, h = tmp.height;
            var corners = new (int x, int y, string name)[]
            {
                (0, 0, "bottom-left"),
                (w - 1, 0, "bottom-right"),
                (0, h - 1, "top-left"),
                (w - 1, h - 1, "top-right"),
            };
            foreach (var c in corners)
            {
                float a = tmp.GetPixel(c.x, c.y).a;
                if (a > 0f)
                    problems.Add(c.name + " corner alpha=" + a
                        + " (expected 0 - opaque background / halo risk)");
            }
        }
        finally
        {
            Object.DestroyImmediate(tmp);
        }
    }
}
