using System.IO;
using JebbyJump.Core;
using JebbyJump.Level;
using JebbyJump.Platforms;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Idempotent: generates / re-tunes the 10 LevelConfig + 10 TimeRankConfig
// assets that form the vertical slice, and attempts to wire
// LevelSessionController._levels in Game.unity.
//
// Known limitation: the scene-wiring step uses SerializedObject and is
// reliable only on a re-run after all level assets already exist on disk.
// On the very FIRST run (newly-created level assets), only the previously
// existing slots (Level1-3) consistently persist into the scene array; the
// new slots may show {fileID: 0}. The script self-verifies after saving the
// scene and, if any slot is null, logs an ERROR listing the GUIDs to paste
// manually into the scene's _levels block. Running the script a second time
// (after assets exist on disk) typically fills the array correctly.
//
// Keep this tool in the project — designers can edit the table below to
// retune levels without writing code.
public static class GenerateVerticalSlice
{
    private const string LevelFolder    = "Assets/_JebbyJump/Settings/Level";
    private const string RankFolder     = "Assets/_JebbyJump/Settings/Level/TimeRanks";
    private const string ScenePath      = "Assets/_JebbyJump/Scenes/Game.unity";

    private struct LevelSpec
    {
        public string Name;
        public int    SeqLen;
        public float  MemTime;
        public int    Lives;
        public PlatformColor[] Colors;
        public int    PerRow;
        public float  Cactus;
        public float  Spacing;
        public float  Spread;
        public float  Width;
        public float  Height;
        public float  RowStartY;
        public float  Jitter;
        public float  S;
        public float  A;
        public float  B;
    }

    // ---- The curve (revised per user direction):
    //   L1–L3 retuned for gentle onboarding (gentler than the originals).
    //   L8 memory time 5.0 (was proposed 4.0).
    //   L10 sequence 6 (was proposed 7), memory 5.0.
    //   All rank thresholds are INITIAL — flag any that play unrealistic.
    private static readonly LevelSpec[] Specs = new[]
    {
        new LevelSpec { Name="Level1Config",  SeqLen=3, MemTime=5.0f, Lives=3, Colors=new[]{PlatformColor.Red, PlatformColor.Blue, PlatformColor.Green},                                                 PerRow=2, Cactus=0.00f, Spacing=3.3f, Spread=7.5f,  Width=4.0f, Height=0.5f, RowStartY=3f, Jitter=0.0f, S= 8, A=12, B=18 },
        new LevelSpec { Name="Level2Config",  SeqLen=4, MemTime=5.0f, Lives=3, Colors=new[]{PlatformColor.Red, PlatformColor.Blue, PlatformColor.Green},                                                 PerRow=2, Cactus=0.00f, Spacing=3.4f, Spread=7.8f,  Width=4.0f, Height=0.5f, RowStartY=3f, Jitter=0.0f, S=10, A=14, B=20 },
        new LevelSpec { Name="Level3Config",  SeqLen=4, MemTime=5.0f, Lives=3, Colors=new[]{PlatformColor.Red, PlatformColor.Blue, PlatformColor.Green, PlatformColor.Yellow},                           PerRow=2, Cactus=0.00f, Spacing=3.5f, Spread=8.0f,  Width=3.8f, Height=0.5f, RowStartY=3f, Jitter=0.0f, S=11, A=15, B=22 },
        new LevelSpec { Name="Level4Config",  SeqLen=5, MemTime=5.0f, Lives=3, Colors=new[]{PlatformColor.Red, PlatformColor.Blue, PlatformColor.Green, PlatformColor.Yellow},                           PerRow=2, Cactus=0.20f, Spacing=3.6f, Spread=8.2f,  Width=3.8f, Height=0.5f, RowStartY=3f, Jitter=0.0f, S=13, A=18, B=25 },
        new LevelSpec { Name="Level5Config",  SeqLen=5, MemTime=5.0f, Lives=3, Colors=new[]{PlatformColor.Red, PlatformColor.Blue, PlatformColor.Green, PlatformColor.Yellow},                           PerRow=2, Cactus=0.15f, Spacing=3.8f, Spread=9.5f,  Width=3.6f, Height=0.5f, RowStartY=3f, Jitter=0.0f, S=14, A=19, B=27 },
        new LevelSpec { Name="Level6Config",  SeqLen=5, MemTime=4.5f, Lives=3, Colors=new[]{PlatformColor.Red, PlatformColor.Blue, PlatformColor.Green, PlatformColor.Yellow, PlatformColor.Purple},     PerRow=3, Cactus=0.25f, Spacing=3.7f, Spread=9.0f,  Width=3.4f, Height=0.5f, RowStartY=3f, Jitter=0.0f, S=16, A=22, B=30 },
        new LevelSpec { Name="Level7Config",  SeqLen=5, MemTime=4.5f, Lives=3, Colors=new[]{PlatformColor.Red, PlatformColor.Blue, PlatformColor.Green, PlatformColor.Yellow, PlatformColor.Purple},     PerRow=3, Cactus=0.20f, Spacing=3.8f, Spread=10.0f, Width=3.4f, Height=0.5f, RowStartY=3f, Jitter=0.25f, S=18, A=25, B=33 },
        new LevelSpec { Name="Level8Config",  SeqLen=6, MemTime=5.0f, Lives=3, Colors=new[]{PlatformColor.Red, PlatformColor.Blue, PlatformColor.Green, PlatformColor.Yellow, PlatformColor.Purple, PlatformColor.Orange}, PerRow=2, Cactus=0.15f, Spacing=3.8f, Spread=9.0f, Width=3.6f, Height=0.5f, RowStartY=3f, Jitter=0.0f, S=20, A=27, B=36 },
        new LevelSpec { Name="Level9Config",  SeqLen=6, MemTime=4.5f, Lives=3, Colors=new[]{PlatformColor.Red, PlatformColor.Blue, PlatformColor.Green, PlatformColor.Yellow, PlatformColor.Purple},     PerRow=3, Cactus=0.40f, Spacing=3.8f, Spread=9.5f,  Width=3.4f, Height=0.5f, RowStartY=3f, Jitter=0.2f,  S=22, A=30, B=40 },
        new LevelSpec { Name="Level10Config", SeqLen=6, MemTime=5.0f, Lives=3, Colors=new[]{PlatformColor.Red, PlatformColor.Blue, PlatformColor.Green, PlatformColor.Yellow, PlatformColor.Purple, PlatformColor.Orange}, PerRow=3, Cactus=0.30f, Spacing=3.9f, Spread=10.0f, Width=3.4f, Height=0.5f, RowStartY=3f, Jitter=0.3f, S=25, A=33, B=45 },
    };

    [MenuItem("Jebby Jump/Generate Vertical Slice")]
    public static void Execute()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("[VSlice] Cannot run in Play Mode.");
            return;
        }

        EnsureFolder("Assets/_JebbyJump/Settings", "Level");
        EnsureFolder(LevelFolder, "TimeRanks");

        // Build / refresh all assets.
        var rankAssets  = new TimeRankConfig[Specs.Length];
        var levelAssets = new LevelConfig[Specs.Length];

        for (int i = 0; i < Specs.Length; i++)
        {
            var spec = Specs[i];
            rankAssets[i]  = EnsureRankAsset(i + 1, spec.S, spec.A, spec.B);
            levelAssets[i] = EnsureLevelAsset(spec, rankAssets[i]);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Re-load every asset from disk so we have stable, fully-imported
        // references (newly-created assets above can be tracked but their
        // ScriptableObject instance might not survive a serialization round-trip
        // cleanly when written to a scene serialized field).
        for (int i = 0; i < Specs.Length; i++)
        {
            string path = $"{LevelFolder}/{Specs[i].Name}.asset";
            levelAssets[i] = AssetDatabase.LoadAssetAtPath<LevelConfig>(path);
            if (levelAssets[i] == null)
                Debug.LogError($"[VSlice] Failed to re-load {path} after creation.");
        }

        // Wire LevelSessionController._levels in the scene.
        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        var lsc = Object.FindAnyObjectByType<LevelSessionController>(FindObjectsInactive.Include);
        if (lsc == null)
        {
            Debug.LogError("[VSlice] LevelSessionController not found in scene.");
            return;
        }
        var so = new SerializedObject(lsc);
        var levelsProp = so.FindProperty("_levels");
        levelsProp.ClearArray();
        levelsProp.arraySize = levelAssets.Length;
        for (int i = 0; i < levelAssets.Length; i++)
            levelsProp.GetArrayElementAtIndex(i).objectReferenceValue = levelAssets[i];
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(lsc);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        // Self-verify scene wiring: re-read the saved scene file and confirm
        // every _levels entry has a non-zero asset reference. If not, log the
        // expected GUID list so it can be pasted manually.
        VerifySceneWiring(levelAssets);

        Debug.Log($"[VSlice] Done. {Specs.Length} TimeRankConfigs + {Specs.Length} LevelConfigs written.");
    }

    private static void VerifySceneWiring(LevelConfig[] expected)
    {
        string sceneText = File.ReadAllText(ScenePath);
        int levelsIdx = sceneText.IndexOf("  _levels:", System.StringComparison.Ordinal);
        if (levelsIdx < 0)
        {
            Debug.LogError("[VSlice] Could not find '_levels:' block in scene file.");
            return;
        }
        // Inspect the next N lines.
        var lines = sceneText.Substring(levelsIdx).Split('\n');
        int nullCount = 0;
        var missing = new System.Collections.Generic.List<string>();
        for (int i = 1; i <= expected.Length && i < lines.Length; i++)
        {
            if (lines[i].Contains("{fileID: 0}"))
            {
                nullCount++;
                string p = AssetDatabase.GetAssetPath(expected[i - 1]);
                string guid = AssetDatabase.AssetPathToGUID(p);
                missing.Add($"  - {{fileID: 11400000, guid: {guid}, type: 2}}   // slot {i - 1}: {expected[i - 1].name}");
            }
        }
        if (nullCount == 0)
        {
            Debug.Log($"[VSlice] Scene wiring verified: all {expected.Length} entries persisted.");
            return;
        }

        Debug.LogError($"[VSlice] Scene wiring INCOMPLETE: {nullCount} of {expected.Length} entries are {{fileID: 0}}.");
        Debug.LogError("[VSlice] Run the script again (now that assets exist on disk) — that usually fixes it.");
        Debug.LogError("[VSlice] If the second run still leaves nulls, paste the following lines manually into the _levels: block in Game.unity (replace any {fileID: 0} entries):\n" + string.Join("\n", missing));
    }

    // ─────────────────────────────────────────────────────────────────────────
    private static TimeRankConfig EnsureRankAsset(int levelNumber, float s, float a, float b)
    {
        string path = $"{RankFolder}/Level{levelNumber:D2}_TimeRankConfig.asset";
        var asset = AssetDatabase.LoadAssetAtPath<TimeRankConfig>(path);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<TimeRankConfig>();
            AssetDatabase.CreateAsset(asset, path);
        }
        var so = new SerializedObject(asset);
        so.FindProperty("_sThreshold").floatValue = s;
        so.FindProperty("_aThreshold").floatValue = a;
        so.FindProperty("_bThreshold").floatValue = b;
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(asset);
        return asset;
    }

    private static LevelConfig EnsureLevelAsset(LevelSpec spec, TimeRankConfig rank)
    {
        string path = $"{LevelFolder}/{spec.Name}.asset";
        var asset = AssetDatabase.LoadAssetAtPath<LevelConfig>(path);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<LevelConfig>();
            AssetDatabase.CreateAsset(asset, path);
        }
        var so = new SerializedObject(asset);
        so.FindProperty("_sequenceLength").intValue              = spec.SeqLen;
        so.FindProperty("_memoryTimeSeconds").floatValue         = spec.MemTime;
        so.FindProperty("_startingLives").intValue               = spec.Lives;
        so.FindProperty("_cactusSpawnChance").floatValue         = spec.Cactus;
        so.FindProperty("_platformsPerRow").intValue             = spec.PerRow;
        so.FindProperty("_rowVerticalSpacing").floatValue        = spec.Spacing;
        so.FindProperty("_rowStartY").floatValue                 = spec.RowStartY;
        so.FindProperty("_platformWidth").floatValue             = spec.Width;
        so.FindProperty("_platformHeight").floatValue            = spec.Height;
        so.FindProperty("_rowHorizontalSpread").floatValue       = spec.Spread;
        so.FindProperty("_rowVerticalJitter").floatValue         = spec.Jitter;
        so.FindProperty("_rankConfig").objectReferenceValue      = rank;

        // Available colors
        var colorsProp = so.FindProperty("_availableColors");
        colorsProp.arraySize = spec.Colors.Length;
        for (int i = 0; i < spec.Colors.Length; i++)
            colorsProp.GetArrayElementAtIndex(i).intValue = (int)spec.Colors[i];

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(asset);
        return asset;
    }

    private static void EnsureFolder(string parent, string folder)
    {
        if (!AssetDatabase.IsValidFolder($"{parent}/{folder}"))
            AssetDatabase.CreateFolder(parent, folder);
    }
}
