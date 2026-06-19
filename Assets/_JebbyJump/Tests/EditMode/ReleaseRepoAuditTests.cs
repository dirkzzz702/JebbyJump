using System.Collections.Generic;
using System.IO;
using JebbyJump.Release;
using NUnit.Framework;
using UnityEngine;

namespace JebbyJump.Tests.EditMode
{
    // File/manifest audits (like the PlayMode scene-integrity tests, but they read
    // source/manifest so they live with the release suite).
    public class ReleaseRepoAuditTests
    {
        [Test]
        public void Runtime_HasNoUnityEditorOrAssetDatabaseRefs()
        {
            string root = Path.GetFullPath(Path.Combine(Application.dataPath, "_JebbyJump/Scripts"));
            var offenders = new List<string>();
            foreach (var file in Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories))
            {
                string norm = file.Replace('\\', '/');
                if (norm.Contains("/Editor/") || norm.Contains("/Tests/")) continue;
                if (HasUnguardedEditorRef(File.ReadAllText(file)))
                    offenders.Add(norm);
            }
            Assert.IsEmpty(offenders,
                "runtime scripts must not reference UnityEditor/AssetDatabase outside "
                + "#if UNITY_EDITOR: " + string.Join(", ", offenders));
        }

        // Flags UnityEditor/AssetDatabase references that are NOT inside an
        // #if UNITY_EDITOR ... (#else/#endif) guard. Guarded editor calls (e.g.
        // SceneLoader.QuitGame) compile out of the player and are acceptable.
        private static bool HasUnguardedEditorRef(string text)
        {
            bool inEditorOnly = false;
            foreach (var raw in text.Split('\n'))
            {
                string trimmed = raw.TrimStart();
                if (trimmed.StartsWith("#if") && trimmed.Contains("UNITY_EDITOR"))
                {
                    inEditorOnly = true;
                    continue;
                }
                if (inEditorOnly && (trimmed.StartsWith("#else") || trimmed.StartsWith("#endif")))
                {
                    inEditorOnly = false;
                    continue;
                }
                if (inEditorOnly) continue;
                if (raw.Contains("UnityEditor") || raw.Contains("AssetDatabase"))
                    return true;
            }
            return false;
        }

        [Test]
        public void Manifest_HasNoUnexpectedRuntimeSdk()
        {
            var ids = ReleasePreflight.GatherSnapshot().PackageIds;
            var unexpected = new List<string>();
            foreach (var id in ids)
                if (ReleasePackageClassification.Classify(id).Category
                    == ReleasePackageCategory.UnexpectedRuntimeSdk)
                    unexpected.Add(id);
            Assert.IsEmpty(unexpected, "unexpected runtime SDK(s): " + string.Join(", ", unexpected));
        }
    }
}
