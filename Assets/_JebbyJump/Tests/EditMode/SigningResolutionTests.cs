using System;
using System.IO;
using JebbyJump.Release;
using NUnit.Framework;
using UnityEngine;

namespace JebbyJump.Tests.EditMode
{
    public class SigningResolutionTests
    {
        [Serializable] private class AsmdefShape { public string name; public string[] includePlatforms; }

        [Test]
        public void ParseIntent_DefaultsToDebug_UnknownIsFailClosed()
        {
            Assert.AreEqual(SigningIntent.Debug, SigningResolution.ParseIntent(null));
            Assert.AreEqual(SigningIntent.Debug, SigningResolution.ParseIntent(""));
            Assert.AreEqual(SigningIntent.Debug, SigningResolution.ParseIntent("debug"));
            Assert.AreEqual(SigningIntent.Upload, SigningResolution.ParseIntent("upload"));
            Assert.AreEqual(SigningIntent.Upload, SigningResolution.ParseIntent("UPLOAD"));
            // P32 spec token + fail-closed unknown (corr #2)
            Assert.AreEqual(SigningIntent.Upload, SigningResolution.ParseIntent("env-upload"));
            Assert.AreEqual(SigningIntent.Upload, SigningResolution.ParseIntent("env_upload"));
            Assert.AreEqual(SigningIntent.Unknown, SigningResolution.ParseIntent("nonsense"));
        }

        [Test]
        public void Unknown_FailsClosed_NeverDebug()
        {
            var r = SigningResolution.Resolve(SigningIntent.Unknown, envComplete: true, keystoreFileExists: true);
            Assert.IsTrue(r.BuildShouldFail);
            Assert.AreNotEqual(nameof(SigningMode.DebugSigned), r.Mode);
            StringAssert.Contains("fail-closed", r.Reason);
        }

        [Test]
        public void SigningSecretGuard_DetectsNonEmptyPasswordOnly()
        {
            Assert.IsTrue(SigningSecretGuard.ProjectSettingsHasSigningSecret("  AndroidKeystorePass: c29tZXNlY3JldA=="));
            Assert.IsTrue(SigningSecretGuard.ProjectSettingsHasSigningSecret("  AndroidKeyaliasPass: hunter2"));
            // empty passwords + a bare name line are NOT secrets, and an empty value must not
            // let the match spill onto the next line
            Assert.IsFalse(SigningSecretGuard.ProjectSettingsHasSigningSecret(
                "  AndroidKeystorePass: \n  AndroidKeyaliasPass: \n  AndroidKeystoreName: myapp"));
            Assert.IsFalse(SigningSecretGuard.ProjectSettingsHasSigningSecret("  AndroidKeystoreName: com.x"));
            Assert.IsFalse(SigningSecretGuard.ProjectSettingsHasSigningSecret(""));
        }

        [Test]
        public void Debug_NeverFails_AndIsNotProduction()
        {
            var r = SigningResolution.Resolve(SigningIntent.Debug, envComplete: false, keystoreFileExists: false);
            Assert.IsFalse(r.BuildShouldFail);
            Assert.AreEqual(nameof(SigningMode.DebugSigned), r.Mode);
        }

        [Test]
        public void Upload_FailsHard_WhenEnvIncomplete_NeverFallsBackToDebug()
        {
            var r = SigningResolution.Resolve(SigningIntent.Upload, envComplete: false, keystoreFileExists: false);
            Assert.IsTrue(r.BuildShouldFail);                       // correction #2: must fail
            Assert.AreEqual(nameof(SigningMode.EnvIncomplete), r.Mode);
            Assert.AreNotEqual(nameof(SigningMode.DebugSigned), r.Mode); // no silent fallback
        }

        [Test]
        public void Upload_FailsHard_WhenKeystoreFileMissing()
        {
            var r = SigningResolution.Resolve(SigningIntent.Upload, envComplete: true, keystoreFileExists: false);
            Assert.IsTrue(r.BuildShouldFail);
            Assert.AreEqual(nameof(SigningMode.EnvIncomplete), r.Mode);
        }

        [Test]
        public void Upload_Succeeds_AsUploadKey_NotProductionOrAppSigning()
        {
            var r = SigningResolution.Resolve(SigningIntent.Upload, envComplete: true, keystoreFileExists: true);
            Assert.IsFalse(r.BuildShouldFail);
            Assert.AreEqual(nameof(SigningMode.EnvUploadKeySigned), r.Mode);   // correction #1 naming
            string status = SigningResolution.StatusString(r);
            StringAssert.Contains("upload", status.ToLowerInvariant());
            StringAssert.DoesNotContain("production", status.ToLowerInvariant()); // never "production"
        }

        [Test]
        public void ReleaseEditor_StaysEditorOnly()
        {
            var text = File.ReadAllText(Path.GetFullPath(Path.Combine(Application.dataPath, "..",
                "Assets/_JebbyJump/Scripts/Release/Editor/JebbyJump.Release.Editor.asmdef")));
            var shape = JsonUtility.FromJson<AsmdefShape>(text);
            CollectionAssert.AreEqual(new[] { "Editor" }, shape.includePlatforms);
        }
    }
}
