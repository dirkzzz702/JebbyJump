using System;

namespace JebbyJump.Release
{
    // Plain editor-gathered snapshot fed to the pure preflight core. Concrete
    // types only (string/int/bool/arrays) so it is trivially testable with
    // synthetic data and never relies on mutating global Player Settings in tests.
    [Serializable]
    public sealed class ReleaseConfigSnapshot
    {
        public string CompanyName = "";
        public string ProductName = "";
        public string ApplicationIdentifier = "";
        public string BundleVersion = "";
        public int AndroidVersionCode;
        public string[] EnabledScenePaths = Array.Empty<string>();
        public int ActiveInputHandler;       // 1 = new Input System only
        public bool LandscapeOnly;
        public int AndroidScriptingBackend;  // 1 = IL2CPP
        public int AndroidArchitectures;     // 2 = ARM64
        public bool DevelopmentBuild;
        public string[] PackageIds = Array.Empty<string>();
    }
}
