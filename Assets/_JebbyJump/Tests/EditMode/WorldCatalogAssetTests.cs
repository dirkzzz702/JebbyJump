using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace JebbyJump.Tests.EditMode
{
    // Real-asset invariants for the ten WorldDefinitions + WorldCatalog
    // (WorldExpansion100, P34B). Read via AssetDatabase + SerializedObject
    // (no Assembly-CSharp type reference), matching LevelBalanceAssetTests.
    // Read-only: inspecting the assets must never change them.
    public class WorldCatalogAssetTests
    {
        private static string[] WorldDefinitionPaths()
        {
            var guids = AssetDatabase.FindAssets("t:WorldDefinition");
            var paths = new string[guids.Length];
            for (int i = 0; i < guids.Length; i++)
                paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
            Array.Sort(paths, StringComparer.Ordinal);
            return paths;
        }

        private static SerializedObject Load(string path)
            => new SerializedObject(
                AssetDatabase.LoadAssetAtPath<ScriptableObject>(path));

        [Test]
        public void TenWorldDefinitions_Exist()
        {
            Assert.AreEqual(10, WorldDefinitionPaths().Length);
        }

        [Test]
        public void WorldIds_AreUnique_AndZeroPadded()
        {
            var ids = new HashSet<string>();
            foreach (var path in WorldDefinitionPaths())
            {
                string id = Load(path).FindProperty("_worldId").stringValue;
                StringAssert.IsMatch("^W(0[1-9]|10)$", id, path);
                Assert.IsTrue(ids.Add(id), "duplicate world id " + id);
            }
            Assert.AreEqual(10, ids.Count);
        }

        [Test]
        public void WorldNumbers_Cover1To10_Once()
        {
            var numbers = new HashSet<int>();
            foreach (var path in WorldDefinitionPaths())
            {
                int n = Load(path).FindProperty("_worldNumber").intValue;
                Assert.GreaterOrEqual(n, 1, path);
                Assert.LessOrEqual(n, 10, path);
                Assert.IsTrue(numbers.Add(n), "duplicate world number " + n);
            }
            Assert.AreEqual(10, numbers.Count);
        }

        [Test]
        public void EveryWorld_HasDisplayName()
        {
            foreach (var path in WorldDefinitionPaths())
            {
                string name = Load(path).FindProperty("_displayName").stringValue;
                Assert.IsFalse(string.IsNullOrWhiteSpace(name),
                    path + " missing _displayName");
            }
        }

        [Test]
        public void LevelRanges_AreContiguous_And_Cover1To100()
        {
            // worldNumber -> (first,last)
            var byNumber = new Dictionary<int, (int first, int last)>();
            foreach (var path in WorldDefinitionPaths())
            {
                var so = Load(path);
                int n = so.FindProperty("_worldNumber").intValue;
                int first = so.FindProperty("_firstGlobalLevelId").intValue;
                int last = so.FindProperty("_lastGlobalLevelId").intValue;
                Assert.AreEqual(10, last - first + 1,
                    path + " must own exactly 10 levels");
                byNumber[n] = (first, last);
            }

            int expectedFirst = 1;
            for (int n = 1; n <= 10; n++)
            {
                Assert.IsTrue(byNumber.ContainsKey(n), "missing world " + n);
                var (first, last) = byNumber[n];
                Assert.AreEqual(expectedFirst, first,
                    "world " + n + " must start at level " + expectedFirst);
                Assert.AreEqual(expectedFirst + 9, last);
                expectedFirst += 10;
            }
            Assert.AreEqual(101, expectedFirst, "ranges must end at level 100");
        }

        [Test]
        public void World1_OwnsExistingLevels1To10()
        {
            // Save-key safety: the shipped ten levels stay in World 1.
            foreach (var path in WorldDefinitionPaths())
            {
                var so = Load(path);
                if (so.FindProperty("_worldNumber").intValue != 1) continue;
                Assert.AreEqual(1, so.FindProperty("_firstGlobalLevelId").intValue);
                Assert.AreEqual(10, so.FindProperty("_lastGlobalLevelId").intValue);
                return;
            }
            Assert.Fail("World 1 definition not found");
        }

        [Test]
        public void WorldCatalog_Exists_AndListsTenWorldsInOrder()
        {
            var guids = AssetDatabase.FindAssets("t:WorldCatalog");
            Assert.AreEqual(1, guids.Length, "expected exactly one WorldCatalog");

            var so = Load(AssetDatabase.GUIDToAssetPath(guids[0]));
            var worlds = so.FindProperty("_worlds");
            Assert.IsNotNull(worlds);
            Assert.IsTrue(worlds.isArray);
            Assert.AreEqual(10, worlds.arraySize);

            for (int i = 0; i < worlds.arraySize; i++)
            {
                var entry = worlds.GetArrayElementAtIndex(i).objectReferenceValue;
                Assert.IsNotNull(entry, "catalog slot " + i + " is unassigned");
                int n = new SerializedObject(entry)
                    .FindProperty("_worldNumber").intValue;
                Assert.AreEqual(i + 1, n,
                    "catalog must be ordered by world number");
            }
        }
    }
}
