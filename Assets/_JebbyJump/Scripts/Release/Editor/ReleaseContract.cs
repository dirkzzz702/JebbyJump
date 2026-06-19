using System;

namespace JebbyJump.Release
{
    // Single immutable source of the release scene list (order matters): Boot
    // (entry) -> MainMenu -> Game. The applier WRITES EditorBuildSettings from
    // this; the builder's BuildPlayerOptions.scenes USES this directly; the
    // preflight VALIDATES EditorBuildSettings against this. Never re-derived from
    // a second mutable read.
    public static class ReleaseSceneContract
    {
        public static readonly string[] Scenes =
        {
            "Assets/_JebbyJump/Scenes/Boot.unity",
            "Assets/_JebbyJump/Scenes/MainMenu.unity",
            "Assets/_JebbyJump/Scenes/Game.unity",
        };

        public static string[] Copy() => (string[])Scenes.Clone();
    }

    // Approved release identity (user-authorized). The applier SETS these; the
    // preflight VALIDATES the gathered snapshot matches them. Single source so
    // the two can never disagree.
    public static class ReleaseIdentity
    {
        public const string CompanyName = "SparkLibrary";
        public const string ApplicationIdentifier = "com.sparklibrary.jebbyjump";
        public const string ProductName = "JebbyJump";
    }

    // Expected target invariants the preflight pins.
    public static class ReleaseTargetInvariants
    {
        public const int NewInputSystemOnly = 1;   // PlayerSettings activeInputHandler
        public const int IL2CPP = 1;               // ScriptingImplementation
        public const int ARM64 = 2;                // AndroidArchitecture bitmask (ARM64)
    }
}
