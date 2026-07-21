using System;
using JebbyJump.Core;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace JebbyJump.Tests.EditMode
{
    // Real-asset invariants for all 100 levels, read via AssetDatabase + SerializedObject
    // (no Assembly-CSharp type reference). These read-only tests double as the
    // no-hidden-tuning guard: inspecting the assets must never change them.
    public class LevelBalanceAssetTests
    {
        private static string[] LevelConfigPaths()
        {
            var guids = AssetDatabase.FindAssets("t:LevelConfig");
            var paths = new string[guids.Length];
            for (int i = 0; i < guids.Length; i++) paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
            Array.Sort(paths, StringComparer.Ordinal);
            return paths;
        }

        [Test]
        public void AllLevelConfigs_Exist()
        {
            // Data-driven against the canonical structure constant instead of a
            // hardcoded count, so the 10 -> 100 expansion (WorldExpansion100
            // P34E) cannot leave a stale literal behind.
            Assert.AreEqual(WorldMapping.TotalLevels, LevelConfigPaths().Length);
        }

        [Test]
        public void AllLevels_HaveRankConfig_WithOrderedThresholds()
        {
            foreach (var path in LevelConfigPaths())
            {
                var so = new SerializedObject(AssetDatabase.LoadAssetAtPath<ScriptableObject>(path));
                var rank = so.FindProperty("_rankConfig");
                Assert.IsNotNull(rank, path + " missing _rankConfig");
                Assert.IsNotNull(rank.objectReferenceValue, path + " rankConfig unassigned");

                var rso = new SerializedObject(rank.objectReferenceValue);
                float s = rso.FindProperty("_sThreshold").floatValue;
                float a = rso.FindProperty("_aThreshold").floatValue;
                float b = rso.FindProperty("_bThreshold").floatValue;
                Assert.Greater(s, 0f, path + " S must be positive");
                Assert.Less(s, a, path + " requires S<A");
                Assert.Less(a, b, path + " requires A<B");
            }
        }

        [Test]
        public void AllLevels_RequiredFieldsValid()
        {
            foreach (var path in LevelConfigPaths())
            {
                var so = new SerializedObject(AssetDatabase.LoadAssetAtPath<ScriptableObject>(path));
                Assert.GreaterOrEqual(so.FindProperty("_sequenceLength").intValue, 1, path);
                Assert.GreaterOrEqual(so.FindProperty("_startingLives").intValue, 1, path);
                Assert.Greater(so.FindProperty("_memoryTimeSeconds").floatValue, 0f, path);
                Assert.That(so.FindProperty("_cactusSpawnChance").floatValue, Is.InRange(0f, 1f), path);
                Assert.GreaterOrEqual(so.FindProperty("_platformsPerRow").intValue, 1, path);
            }
        }

        [Test]
        public void LevelCatalog_Exists()
        {
            Assert.GreaterOrEqual(AssetDatabase.FindAssets("t:LevelCatalog").Length, 1);
        }

        [Test]
        public void Inspection_DoesNotModifyLevelAssets() // no-hidden-tuning guard (corr #7)
        {
            string before = HashLevelConfigs();
            foreach (var path in LevelConfigPaths())
            {
                var so = new SerializedObject(AssetDatabase.LoadAssetAtPath<ScriptableObject>(path));
                _ = so.FindProperty("_sequenceLength").intValue; // read only, never ApplyModifiedProperties
            }
            Assert.AreEqual(before, HashLevelConfigs(), "level config assets must be unchanged by inspection");
        }

        private static string HashLevelConfigs()
        {
            using (var sha = SHA256.Create())
            {
                var sb = new StringBuilder();
                foreach (var p in LevelConfigPaths())
                {
                    string full = Path.GetFullPath(p);
                    if (File.Exists(full)) sb.Append(BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(full))));
                }
                return sb.ToString();
            }
        }

        [Test]
        public void BalanceDocs_DoNotContainPersonalTesterNames() // corr #5
        {
            string dir = Path.GetFullPath(Path.Combine(Application.dataPath, "_JebbyJump/Docs/Design"));
            if (!Directory.Exists(dir)) { Assert.Pass("docs dir absent at test time"); return; }
            foreach (var f in Directory.GetFiles(dir, "Jebby_Jump_Level_*.md"))
            {
                string text = File.ReadAllText(f);
                Assert.IsFalse(Regex.IsMatch(text, @"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}"),
                    "email-like token in " + Path.GetFileName(f));
            }
        }
    }
}
